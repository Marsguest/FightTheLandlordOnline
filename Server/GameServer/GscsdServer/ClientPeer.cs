using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using Protocol.Tool;

namespace GscsdServer
{
    /// <summary>
    /// 封装的客户端的连接对象
    /// </summary>
    public class ClientPeer
    {
        public Socket ClientSocket { get; set; }

        public ClientPeer()
        {
            this.ReceiveArgs = new SocketAsyncEventArgs();
            this.ReceiveArgs.UserToken = this;
            this.ReceiveArgs.SetBuffer(new byte[1024],0,1024);
            this.SendArgs = new SocketAsyncEventArgs();
            this.SendArgs.Completed += SendArgs_Completed;
        }

        

        #region 接收数据

        public delegate void ReceiveCompletedDlg(ClientPeer client, SocketMsg value);
        /// <summary>
        /// 一个消息解析完成的回调
        /// </summary>
        public ReceiveCompletedDlg receiveCompletedDlg;

        /// <summary>
        /// 一旦接受到数据，就存到缓冲区里面
        /// </summary>
        private List<byte> dataCache = new List<byte>();
        /// <summary>
        /// 接收的异步套接字请求
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; }

        /// <summary>
        /// 是否正在处理接收的数据
        /// </summary>
        private bool isReceiveProcess = false;

        

        /// <summary>
        /// 自身处理数据包
        /// </summary>
        /// <param name="packet"></param>
        /// packet是数据包 是接了包头之后的 data是真实的数据
        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            if (!isReceiveProcess)
            {
                processReceive();
            }

        }
        /// <summary>
        /// 处理接收的数据
        /// </summary>
        private void processReceive()
        {
            isReceiveProcess = true;
            //解析数据包
            byte[] data = EncodeTool.DecodePacket(ref dataCache);
            
            if(data == null)
            {
                isReceiveProcess = false;
                return;
            }
            //需要再次转成一个具体的类型
            SocketMsg msg = EncodeTool.DecodeMsg(data);

            // 回调给上层 这里的上层就是服务器连接对象
            if (receiveCompletedDlg != null)
                receiveCompletedDlg(this, msg);
            //尾递归
            processReceive();


        }

        #region 粘包拆包问题 解决对策：消息头和消息尾
        //粘包拆包问题 解决对策：消息头和消息尾

        void test()
        {
            byte[] bt = Encoding.Default.GetBytes("123456");

            //怎么构造
            //头：消息的长度 bt.length
            //尾：具体的消息 bt

            int length = bt.Length;
            byte[] bt_length = BitConverter.GetBytes(length);
            //得到的消息 bt_length + bt

            //怎么读取
            //int length = 前四个字节转成int类型
            //然后读取这个长度的数据
        }

        #endregion
        #endregion

        #region 断开连接
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            //清空数据
            dataCache.Clear();
            isReceiveProcess = false;
            //给发送数据那里预留的
            sendQueue.Clear();
            isSendProcess = false;


            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            ClientSocket = null;
        }
        #endregion

        #region 服务器向客户端发送数据
        /// <summary>
        /// 发送的消息队列
        /// </summary>
        private Queue<byte[]> sendQueue = new Queue<byte[]>();

        private bool isSendProcess = false;
        /// <summary>
        /// 发送的异步套接字操作
        /// </summary>
        private SocketAsyncEventArgs SendArgs;
        /// <summary>
        /// 发送的时候发现断开连接的回调
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reason"></param>
        public delegate void SendDisconnectDlg(ClientPeer client, string reason);
        public SendDisconnectDlg sendDisconnectDlg;
        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作</param>
        /// <param name="value">参数</param>
        public void Send(int opCode,int subCode,object value)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            try
            {
                Send(packet);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message); ;
            }
        }
        public void Send(byte[] packet)
        {
            //存入消息队列中
            sendQueue.Enqueue(packet);
            if (!isSendProcess)
                send();
        }

        private void send()
        {
            isSendProcess = true;

            //如果数据的条数等于0的话 就停止发送
            if(sendQueue.Count == 0)
            {
                isSendProcess = false;
                return;
            }
            //取出一条数据
            byte[] packet = sendQueue.Dequeue();
            //设置 消息发送异步对象 的发送数据缓冲区
            SendArgs.SetBuffer(packet, 0, packet.Length);
            bool result = ClientSocket.SendAsync(SendArgs);
            if(result == false)
            {
                processSend();
            }
        }

        private void SendArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            processSend();
        }
        /// <summary>
        /// 当异步发送请求完成时调用
        /// </summary>
        private void processSend()
        {
            //发送的有没有错误
            if (SendArgs.SocketError != SocketError.Success)
            {
                //发送出错了 客户端断开连接了
                sendDisconnectDlg(this, SendArgs.SocketError.ToString());
            }
            else
            {
                send();
            }
        }
        #endregion
    }
}

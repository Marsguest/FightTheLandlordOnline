using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Protocol;

namespace GscsdServer
{
    /// <summary>
    /// 服务器端
    /// </summary>
    public class ServerPeer
    {
        /// <summary>
        /// 服务器端的socket对象
        /// </summary>
        private Socket serverSocket;
        /// <summary>
        /// 信号量 用于限制进程的访问
        /// </summary>
        private Semaphore acceptSemaphore;
        /// <summary>
        /// 客户端对象的连接池
        /// </summary>
        private ClientPeerPool clientPeerPool;
        /// <summary>
        /// 应用层
        /// </summary>
        private IApplication applicaton;
        /// <summary>
        /// 设置应用层
        /// </summary>
        /// <param name="app"></param>
        public void SetApplicaton(IApplication app)
        {
            this.applicaton = app;
        }



        
        /// <summary>
        /// 用来开启服务器
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="maxCount">最大连接数量</param>
        public void Start(int port,int maxCount)
        {
            try
            {
                serverSocket = new Socket(
                    AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                acceptSemaphore = new Semaphore(maxCount, maxCount);

                //连接池中客户端的初始化 直接new出最大数量的连接对象
                clientPeerPool = new ClientPeerPool(maxCount);
                ClientPeer tmpClientPeer = null;
                for (int i = 0; i < maxCount; i++)
                {
                    tmpClientPeer = new ClientPeer();
                    tmpClientPeer.ReceiveArgs.Completed += receive_Completed;
                    tmpClientPeer.receiveCompletedDlg = reveiveCompleted;
                    tmpClientPeer.sendDisconnectDlg = Disconnect;
                    clientPeerPool.Enqueue(tmpClientPeer);
                }
                
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                serverSocket.Listen(10);

                Console.WriteLine("服务器启动...");
                startAccpet(null);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        #region 接收客户端连接
        /// <summary>
        /// 开始等待客户端的连接
        /// </summary>
        /// <param name="e"></param>
        private void startAccpet(SocketAsyncEventArgs e)
        {
            if(e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += accept_Completed;//注册完成事件的触发
            }

            

            bool result = serverSocket.AcceptAsync(e);
            //返回值判断异步事件是否执行完毕
            //true代表正在执行 执行完毕后会触发事件
            //false代表执行完毕

            if(result == false)
            {
                processAccept(e);
            }
        }
        /// <summary>
        /// 接受连接请求异步事件执行完毕后触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            processAccept(e);
        }
        /// <summary>
        /// 处理连接请求
        /// </summary>
        /// <param name="e"></param>
        private void processAccept(SocketAsyncEventArgs e)
        {
            //计数 限制线程的访问
            acceptSemaphore.WaitOne();
            //得到客户端的对象
            //Console.WriteLine("进入processAccept...");
            ClientPeer client = clientPeerPool.Dequeue();
            client.ClientSocket = e.AcceptSocket;

            Console.WriteLine("客户端连接成功："+client.ClientSocket.RemoteEndPoint.ToString());
            //applicaton.OnConnect(client);
            //一直接受客户端发来的数据
            startReceive(client);

            e.AcceptSocket = null;
            startAccpet(e);

        }

        #endregion

        #region 接收数据
        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <param name="client"></param>
        private void startReceive(ClientPeer client)
        {
            try
            {
                bool result = client.ClientSocket.ReceiveAsync(client.ReceiveArgs);
                if (result == false)
                {
                    processReveive(client.ReceiveArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        /// <summary>
        /// 处理接收的请求
        /// </summary>
        /// <param name="e"></param>
        private void processReveive(SocketAsyncEventArgs e)
        {
            //Console.WriteLine("进入processReceive...");
            ClientPeer client = e.UserToken as ClientPeer;
            //判断网络消息是否接收成功
            if(client.ReceiveArgs.SocketError == SocketError.Success&& client.ReceiveArgs.BytesTransferred > 0)
            {
                byte[] packet = new byte[client.ReceiveArgs.BytesTransferred];
                Buffer.BlockCopy
                    (client.ReceiveArgs.Buffer,
                    0,
                    packet, 
                    0, 
                    client.ReceiveArgs.BytesTransferred);
                //让客户端自身处理这个数据包 自身解析
                client.StartReceive(packet);
                //尾递归
                startReceive(client);
            }
            //如果没有传输的字节数 就代表断开连接了
            else if (client.ReceiveArgs.BytesTransferred == 0)
            {
                if(client.ReceiveArgs.SocketError == SocketError.Success)
                {
                    //客户端主动断开连接
                    //Console.WriteLine("尚未调用ondisconnect 客户端主动断开连接");
                    Disconnect(client, "客户端主动断开连接");
                }
                else
                {
                    //由于网络异常导致被动断开连接
                    //Console.WriteLine("尚未调用ondisconnect"+client.ReceiveArgs.SocketError.ToString());
                    Disconnect(client, client.ReceiveArgs.SocketError.ToString());
                }
            }
        }
        /// <summary>
        /// 当接收完成时触发的事件
        /// </summary>
        /// <param name=""></param>
        private void receive_Completed(object sender,SocketAsyncEventArgs e)
        {
            processReveive(e);
        }

        /// <summary>
        /// 一条数据解析完成的处理
        /// </summary>
        /// <param name="client">对应的连接对象</param>
        /// <param name="value">解析出来的一个具体能使用的类型</param>
        private void reveiveCompleted(ClientPeer client, SocketMsg msg)
        {
            //给应用层 让其使用
            applicaton.OnReceive(client, msg);
        }
        #endregion

        #region 断开连接
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="client">表示断开的客户端连接对象</param>
        /// <param name="reason">断开的原因</param>
        public void Disconnect(ClientPeer client,string reason)
        {
            try
            {
                //清空一些数据
                if (client == null)
                    throw new Exception("当前指定的客户端连接对象为空 无法断开连接");

                Console.WriteLine(client.ClientSocket.RemoteEndPoint.ToString()+ "客户端断开连接 原因：" +reason);
                //通知应用层 这个客户断开连接了
                applicaton.OnDisconnect(client);

                client.Disconnect();
                //回收对象 方便下次使用
                clientPeerPool.Enqueue(client);
                acceptSemaphore.Release();//释放信号量

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
        #endregion

    }
}

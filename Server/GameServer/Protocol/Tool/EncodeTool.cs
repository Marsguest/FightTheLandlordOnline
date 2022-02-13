using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Protocol;

namespace Protocol.Tool
{
    /// <summary>
    /// 关于编码的工具类
    /// </summary>
    public class EncodeTool
    {
        #region 粘包拆包问题 封装一个有规定的数据包
        /// <summary>
        /// 构造消息体  包头+包尾
        /// </summary>
        /// <returns></returns>
        public static byte[] EncodePacket(byte[] data)
        {
            //内存流对象 可以理解为字节数组的数组
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    //先写入长度
                    bw.Write(data.Length);
                    //再写入数据
                    bw.Write(data);

                    byte[] byteArray = new byte[(int)ms.Length];
                    //这种写入的方法效率高
                    Buffer.BlockCopy(ms.GetBuffer(), 0, byteArray, 0, (int)ms.Length);

                    return byteArray;
                }
            }
        }
        /// <summary>
        /// 解析消息体 从缓存里取出一个个完整的数据包
        /// </summary>
        /// <returns></returns>
        /// ref 方法里面修改会印象到方法外面
        public static byte[] DecodePacket(ref List<byte> dataCache)
        {
            //四个字节构成一个int长度 不能构成一个完整的消息
            if (dataCache.Count < 4)
                return null;
                //throw new Exception("数据缓存长度不组4 不能构成一个完整的消息")；
            //内存流对象 可以理解为字节数组的数组
            using (MemoryStream ms = new MemoryStream(dataCache.ToArray()))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int length = br.ReadInt32();
                    int dataRemainLength = (int)(ms.Length - ms.Position);

                    if(length > dataRemainLength)
                        return null;
                    //throw new Exception("数据长度不够包头约定的长度 不能构成一个完整的消息")；

                    byte[] data = br.ReadBytes(length);
                    //更新一下数据缓存
                    //先清空  之后将剩余的数据写回数据缓冲区
                    dataCache.Clear();
                    dataCache.AddRange(br.ReadBytes(dataRemainLength));

                    return data;
                }
            }
        }
        #endregion

        #region 构造发送的SocketMsg类
        /// <summary>
        /// 把SocketMsg类转化成字节数组发送出去
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] EncodeMsg(SocketMsg msg)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(msg.OpCode);
                    bw.Write(msg.SubCode);
                    //非空才将obj转化成字节数据存起来
                    if(msg.Value!= null)
                    {
                        byte[] valueBytes = EncodeObj(msg.Value);
                        bw.Write(valueBytes);
                    }

                    byte[] data = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);

                    return data;
                }
            }
        }

        public static SocketMsg DecodeMsg(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    SocketMsg msg = new SocketMsg();
                    msg.OpCode = br.ReadInt32();
                    msg.SubCode = br.ReadInt32();
                    //还有剩余的字节没读取 代表value有值
                    if(ms.Length > ms.Position)
                    {
                        byte[] valueBytes = br.ReadBytes((int)(ms.Length - ms.Position));
                        //TODO将读出的数据拼成obj
                        object value = DecodeObj(valueBytes);
                        msg.Value = value;
                    }
                    return msg;
                }
            }
        }


        #endregion
        /// <summary>
        /// 序列化对象 这个类提供给EncodeMsg使用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        #region 把一个object类型转化成byte[]
        private static byte[] EncodeObj(object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, value);

                byte[] valueBytes = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, valueBytes, 0, (int)ms.Length);

                return valueBytes;
            }
        }
        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="valueBytes"></param>
        /// <returns></returns>
        private static object DecodeObj(byte[] valueBytes)
        {
            using (MemoryStream ms = new MemoryStream(valueBytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object value = bf.Deserialize(ms);
                return value;
            }
        }

        #endregion
    }

}

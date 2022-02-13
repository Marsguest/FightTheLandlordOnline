using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GscsdServer;
using Protocol;
using Protocol.Tool;

namespace GameServer.Cache.Match
{
    /// <summary>
    /// 匹配房间
    /// </summary>
    public class MatchRoom
    {
        public int Id { get; set; }//唯一标识
        /// <summary>
        /// 在房间内的用户id 和 连接对象的 映射
        /// </summary>
        public Dictionary<int, ClientPeer> UIdClientDict;
        /// <summary>
        /// 已经准备的玩家id列表
        /// </summary>
        public List<int> ReadyUIdList;

        public List<int> GetUIdList()
        {
            return UIdClientDict.Keys.ToList();
        }
        public MatchRoom(int _id)
        {
            this.Id = _id;
            this.UIdClientDict = new Dictionary<int, ClientPeer>();
            this.ReadyUIdList = new List<int>();
        }
        /// <summary>
        /// 房间是否满了
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return UIdClientDict.Count == 3;
        }
        /// <summary>
        /// 房间是否空了
        /// </summary>
        /// <returns>true代表空了 false代表还有人</returns>
        public bool IsEmpty()
        {
            return UIdClientDict.Count == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAllReady()
        {
            return ReadyUIdList.Count == 3;
        }
        /// <summary>
        /// 进入房间
        /// </summary>
        public void EnterRoom(int _userId,ClientPeer client)
        {
            UIdClientDict.Add(_userId,client);
        }
        /// <summary>
        /// 离开房间
        /// </summary>
        public void LeaveRoom(int _userId)
        {
            UIdClientDict.Remove(_userId);
        }
        public void Ready(int _userId)
        {
            ReadyUIdList.Add(_userId);
        }
        public void Brocast(int opCode,int subCode,object value,ClientPeer exClient = null)
        {
            //这样可以让所有要广播的连接对象不必做复杂的序列化 直接发packet即可
            //之前的方法 每个client都要做以下的打包操作 这个只需要广播的时候做一次即可发出多个相同的包 为了服务器性能优化
            //Console.WriteLine("向房间发送广播"+value.ToString());
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);


            foreach (var client in UIdClientDict.Values)
            {
                if (client == exClient)
                {
                    continue;
                }
                client.Send(packet);
            }
        }

    }
}

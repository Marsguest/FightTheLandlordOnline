using System;
using System.Collections.Generic;
using System.Text;
using GscsdServer;
using GscsdServer.Util.Concurrent;

namespace GameServer.Cache.Match
{
    /// <summary>
    /// 匹配的缓存层
    /// </summary>
    public class MatchCache
    {
        /// <summary>
        /// 代表 正在等待的 用户id 和 房间id 的映射  用户1在房间1里进行等待
        /// </summary>
        private Dictionary<int, int> uidRoomIdDict = new Dictionary<int, int>();
        /// <summary>
        /// 正在等待的 房间id 和房间数据模型 的映射
        /// </summary>
        private Dictionary<int, MatchRoom> idModelDict = new Dictionary<int, MatchRoom>();
        /// <summary>
        /// 代表重用的房间队列 房间池
        /// </summary>
        private Queue<MatchRoom> roomQueue = new Queue<MatchRoom>();
        /// <summary>
        /// 代表 房间id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);
        /// <summary>
        /// 进入匹配队列 进入匹配房间
        /// </summary>
        /// <returns></returns>
        public MatchRoom Enter(int _userId,ClientPeer client)
        {
            //遍历等待的房间 看一下有无正在等待的 如果有 将该玩家加进去
            foreach (MatchRoom mr in idModelDict.Values)
            {
                if (mr.IsFull())
                    continue;
                //没满的话
                mr.EnterRoom(_userId,client);
                uidRoomIdDict.Add(_userId, mr.Id);
                return mr;
            }

            //如果调用到这里 代表没进去 因为没有等待的房间了
            //自己重新创建一个房间
            MatchRoom room = null;
            //判断是否有重用的房间
            if (roomQueue.Count > 0)
                room = roomQueue.Dequeue();
            else
                room = new MatchRoom(id.Add_Get());

            room.EnterRoom(_userId,client);
            idModelDict.Add(room.Id, room);
            uidRoomIdDict.Add(_userId, room.Id);
            return room;
        }
       /// <summary>
       /// 离开匹配队列
       /// </summary>
       /// <param name="_userId"></param>
       /// <returns></returns>
        public MatchRoom Leave(int _userId)
        {
            int roomId = uidRoomIdDict[_userId];
            MatchRoom room = idModelDict[roomId];
            room.LeaveRoom(_userId);

            //还需要进一步的处理
            uidRoomIdDict.Remove(_userId);
            if (room.IsEmpty())
            {
                //放入房间池中
                idModelDict.Remove(roomId);
                //TODO
                roomQueue.Enqueue(room);
            }
            return room;
        }
        /// <summary>
        /// 判断用户是否在匹配房间内
        /// </summary>
        /// <param name="_userId"></param>
        /// <returns></returns>
        public bool IsMatching(int _userId)
        {
            return uidRoomIdDict.ContainsKey(_userId);
        }
        /// <summary>
        /// 获取玩家所在的等待房间
        /// </summary>
        /// <returns></returns>
        public MatchRoom GetRoom(int _userId)
        {
            int roomId = uidRoomIdDict[_userId];
            MatchRoom room = idModelDict[roomId];
            return room;
        }
        /// <summary>
        /// 摧毁房间 其实是房间的初始化
        /// </summary>
        public void Destory(MatchRoom _room)
        {
            idModelDict.Remove(_room.Id);
            foreach (var userId in _room.UIdClientDict.Keys)
            {
                uidRoomIdDict.Remove(userId);
            }
            //清空数据
            _room.UIdClientDict.Clear();
            _room.ReadyUIdList.Clear();
            roomQueue.Enqueue(_room);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GscsdServer.Util.Concurrent;
using Protocol.Dto.Fight;

namespace GameServer.Cache.Fight
{
    /// <summary>
    /// 战斗的缓存层
    /// </summary>
    public  class FightCache
    {
        /// <summary>
        /// 用户id对应的房间id
        /// </summary>
        private Dictionary<int, int> uidRoomIdDict = new Dictionary<int, int>();
        /// <summary>
        /// 房间id对应的房间对象
        /// </summary>
        private Dictionary<int, FightRoom> idRoomDict = new Dictionary<int, FightRoom>();
        /// <summary>
        /// 重用房间队列
        /// </summary>
        private Queue<FightRoom> fightRoomQueue = new Queue<FightRoom>();
        /// <summary>
        /// 房间的id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);
        /// <summary>
        /// 创建战斗房间
        /// </summary>
        /// <returns></returns>
        public FightRoom Create(List<int> uidList)
        {
            FightRoom room = null;
            //先检测有无可以重用的房间 没有就直接创建
            if (fightRoomQueue.Count > 0)
            {
                room = fightRoomQueue.Dequeue();
                //fix bug
                room.Init(uidList);
            }
                
            else
                room = new FightRoom(id.Add_Get(),uidList);
            //绑定映射关系
            foreach (var uid in uidList)
            {
                uidRoomIdDict.Add(uid, room.Id);
            }
            idRoomDict.Add(room.Id, room);

            return room;
        }
        /// <summary>
        /// 通过房间id获取房间
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public FightRoom GetRoomById(int roomId)
        {
            if (idRoomDict.ContainsKey(roomId) == false)
            {
                throw new Exception("不存在这个房间！");
            }
            return idRoomDict[roomId];
        }
        /// <summary>
        /// 通过用户id获取房间
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public FightRoom GetRoomByUId(int uid)
        {
            if(uidRoomIdDict.ContainsKey(uid) == false)
                throw new Exception("当前用户不在战斗房间！");
            int roomId = uidRoomIdDict[uid];
            return idRoomDict[roomId];
        }
        /// <summary>
        /// 摧毁房间
        /// </summary>
        /// <param name="room"></param>
        public void Destory(FightRoom room)
        {
            idRoomDict.Remove(room.Id);
            foreach (PlayerDto player in room.PlayerList)
            {
                uidRoomIdDict.Remove(player.Userid);
            }
            //初始化房间数据
            room.PlayerList.Clear();
            room.LeaveUIdList.Clear();
            room.TableCardList.Clear();
            room.libraryModel.Init();
            room.Multiple = 1;
            room.roundModel.Init();
            //加入重用队列中 等待重用
            fightRoomQueue.Enqueue(room);
        }
        /// <summary>
        /// 是否在战斗房间中
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsFighting(int userId)
        {
            return uidRoomIdDict.ContainsKey(userId);
        }
    }
}

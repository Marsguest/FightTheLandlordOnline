using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Cache;
using GameServer.Cache.Match;
using GameServer.Model;
using GscsdServer;
using Protocol.Code;
using Protocol.Dto;

namespace GameServer.Logic
{
    public delegate void StartFight(List<int> uidList);
    public class MatchHandler : IHandler
    {
        private MatchCache matchCache = Caches.Match;
        private UserCache userCache = Caches.User;

        public StartFight startFight;
        public void OnDisconnect(ClientPeer client)
        {
            leave(client);
        }

        public void onReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case MatchCode.ENTER_MATCH_QUEUE_CREQ:
                    SingleExecute.Instance.Execute(() => enter(client));
                    break;
                case MatchCode.LEAVE_MATCH_QUEUE_CREQ:
                    SingleExecute.Instance.Execute(() => leave(client));
                    break;
                case MatchCode.READY_CREQ:
                    SingleExecute.Instance.Execute(() => ready(client));
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 进入
        /// </summary>
        private void enter(ClientPeer client)
        {
            if (!userCache.IsOnline(client))
                return;

            int userId = userCache.GetIdByClientPeer(client);
            // 进入匹配的队列
            if (matchCache.IsMatching(userId))
            {
                //用户已经在匹配了
                //client.Send(OpCode.MATCH, MatchCode.ENTER_MATCH_QUEUE_SRES, -1); //重复加入
                return;
            }
            //正常进入
            MatchRoom room = matchCache.Enter(userId,client);
            //广播给房间内其他的用户 有新玩家加入了  参数是新进入的玩家的用户UserDto
            UserModel model = userCache.GetModelByUid(userId);
            UserDto userDto = new UserDto(model.Id, model.Name, model.Bean, model.WinCount, model.LoseCount, model.RunCount, model.Lv, model.Exp);
            room.Brocast(OpCode.MATCH, MatchCode.ENTER_MATCH_QUEUE_BRO, userDto, client);
            //返回给当前客户端 给他房间的数据模型
            MatchRoomDto dto = makeRoomDto(room);
            client.Send(OpCode.MATCH, MatchCode.ENTER_MATCH_QUEUE_SRES, dto);

            Console.WriteLine(userId + "玩家进入匹配房间！");


        }
        /// <summary>
        /// 构造MatchRoomDto传输对象
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private MatchRoomDto makeRoomDto(MatchRoom room)
        {
            MatchRoomDto dto = new MatchRoomDto();
            //给UidClientDict赋值
            foreach (var uid in room.UIdClientDict.Keys)
            {
                if (!userCache.IsOnline(uid))
                    continue;
                UserModel model = userCache.GetModelByUid(uid);
                UserDto userDto = new UserDto(model.Id,model.Name, model.Bean, model.WinCount, model.LoseCount, model.RunCount, model.Lv, model.Exp);
                //dto.UIdUserDict.Add(uid, userDto);
                //fix bug
                dto.AddUser(userDto);
            }
            dto.ReadyUIdList = room.ReadyUIdList;
            return dto;
        }
        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="client"></param>
        private void leave(ClientPeer client)
        {
            if (!userCache.IsOnline(client))
                return;
            int userId = userCache.GetIdByClientPeer(client);
            if (matchCache.IsMatching(userId) == false)
            {
                //用户没有匹配 不能退出 非法操作
                //client.Send(OpCode.MATCH,MatchCode.Lea)
                return;
            }
            //正常离开
            MatchRoom room =  matchCache.Leave(userId);
            //广播给房间内所有人 有人离开了（这个时候就是所有人 因为自己已经离开了）
            room.Brocast(OpCode.MATCH, MatchCode.LEAVE_MATCH_QUEUE_BRO, userId); //离开的用户ID

            Console.WriteLine(userId+ "玩家离开匹配房间！");
        }
        private void ready(ClientPeer client)
        {
            if (!userCache.IsOnline(client))
                return;
            int userId = userCache.GetIdByClientPeer(client);
            if (matchCache.IsMatching(userId) == false)
            {
                //用户没有匹配 不能准备 非法操作
                //client.Send(OpCode.MATCH,MatchCode.Lea)
                return;
            }
            //一定要注意安全问题
            MatchRoom room = matchCache.GetRoom(userId);
            room.Ready(userId);
            room.Brocast(OpCode.MATCH, MatchCode.READY_BRO, userId);

            //检测：是否玩家都准备了
            if (room.IsAllReady())
            {
                //开始战斗
                startFight(room.GetUIdList());
                //通知房间内的玩家要进行战斗了 给客户端群发消息
                room.Brocast(OpCode.MATCH, MatchCode.START_BRO, null);
                //销毁房间
                matchCache.Destory(room);
            }
        }

    }
}

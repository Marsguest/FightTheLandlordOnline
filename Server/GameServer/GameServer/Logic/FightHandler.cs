using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Cache;
using GameServer.Cache.Fight;
using GameServer.Model;
using GscsdServer;
using Protocol;
using Protocol.Code;
using Protocol.Dto.Fight;
using Protocol.Tool;

namespace GameServer.Logic
{

    public class FightHandler : IHandler
    {
        public FightCache fightCache= Caches.Fight;
        public UserCache userCache = Caches.User;
        public void OnDisconnect(ClientPeer client)
        {
            leave(client);
        }

        public void onReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case FightCode.GRAB_LANDLORD_CREQ://抢地主
                    bool result = (bool)value; //如果是true的话就是抢
                    SingleExecute.Instance.Execute(() => grabLandLord(result,client));
                    break;
                case FightCode.DEAL_CREQ://出牌
                    DealDto dto = value as DealDto;
                    SingleExecute.Instance.Execute(() => deal(client, dto));
                    break;
                case FightCode.PASS_CREQ://不出
                    SingleExecute.Instance.Execute(() => pass(client));
                    break;

                default:
                    break;
            }
        }
        /// <summary>
        /// 用户离开
        /// </summary>
        /// <param name="client"></param>
        private void leave(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                () =>
                {
                    //必须确保在线
                    if (userCache.IsOnline(client) == false)
                        return;
                    int userId = userCache.GetIdByClientPeer(client);
                    if (!fightCache.IsFighting(userId))
                        return;
                    FightRoom room = fightCache.GetRoomByUId(userId);

                    //就算中途退出的人
                    room.LeaveUIdList.Add(userId);
                    brocast(room, OpCode.FIGHT, FightCode.LEAVE_BRO, userId);

                    if (room.LeaveUIdList.Count == 3)
                    {
                        //所有都走了
                        //给逃跑玩家添加逃跑场次
                        foreach (var item in room.LeaveUIdList)
                        {
                            UserModel userModel = userCache.GetModelByUid(item);
                            userModel.RunCount++;
                            userModel.Bean -= 3 * room.Multiple * 1000;
                            userCache.UpdateUserModel(userModel);
                        }
                        //销毁缓存层的房间数据
                        fightCache.Destory(room);
                    }
                });
        }
        /// <summary>
        /// 不出的处理
        /// </summary>
        /// <param name="client"></param>
        private void pass(ClientPeer client)
        {
            //必须确保在线
            if (userCache.IsOnline(client) == false)
                return;
            int userId = userCache.GetIdByClientPeer(client);
            FightRoom room = fightCache.GetRoomByUId(userId);

            //分两种情况
            //当前玩家是最大出牌者 没人管上 不能不出
            if (room.roundModel.BiggestUId == userId)
            {
                client.Send(OpCode.FIGHT, FightCode.PASS_SRES, -1);//不能不出
            }
            else
            {
                //可以不出
                client.Send(OpCode.FIGHT, FightCode.PASS_SRES, 0);//可以不出
                turn(room);
            }
            
        }
        /// <summary>
        /// 出牌的处理
        /// </summary>
        private void deal(ClientPeer client, DealDto dealDto)
        {
            //必须确保在线
            if (userCache.IsOnline(client) == false)
                return;
            int userId = userCache.GetIdByClientPeer(client);
            if (userId != dealDto.UserId)
            {
                return;
            }
            FightRoom room = fightCache.GetRoomByUId(userId);

            //玩家出牌 2种情况
            //玩家还在  玩家中途退出或者掉线了
            if (room.LeaveUIdList.Contains(userId))
            {
                //玩家掉线
                //直接转换出牌
                turn(room);

            }
            else
            {
                //玩家还在
                bool canDeal =  room.DealCard(dealDto.Type,dealDto.Weight,dealDto.Length,userId,dealDto.SelectCardList);
                if(canDeal == false)
                {
                    //玩家出的牌管不上
                    client.Send(OpCode.FIGHT, FightCode.DEAL_SRES, -1);//通知这个客户端管不上
                    return;
                }
                else
                {
                    //能管上
                    //给自身客户端发送出牌成功的消息
                    client.Send(OpCode.FIGHT, FightCode.DEAL_SRES, 0);
                    //广播给所有的客户端 谁出了什么牌
                    List<CardDto> remainCardList = room.GetPlayerModel(userId).CardList;
                    dealDto.RemainCardList = remainCardList;
                    brocast(room,OpCode.FIGHT, FightCode.DEAL_BRO, dealDto);

                    //检测一下剩余手牌 如果手牌数为0 那就游戏结束了
                    if (remainCardList.Count == 0)
                    {
                        //游戏结束
                        gameOver(userId, room);
                    }
                    else
                    {
                        //直接转换出牌
                        turn(room);
                    }


                }
            }
        }
        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="userId">胜利的玩家id</param>
        /// <param name="room"></param>
        private void gameOver(int userId,FightRoom room)
        {
            int winIdentity = room.GetPlayerIdentity(userId);
            int winBean = room.Multiple * 1000;
            //获取获胜身份所有的玩家id
            List<int> winUIds = room.GetSameIdentiyUIds(winIdentity);

            //给胜利玩家添加胜场 豆子和经验
            foreach (var item in winUIds)
            {
                UserModel userModel = userCache.GetModelByUid(item);
                userModel.WinCount++;
                userModel.Bean += winBean;
                userModel.Exp += 100;
                //看看能不能升级
                int maxExp = userModel.Lv * 100;
                while (userModel.Exp >= maxExp)
                {
                    userModel.Lv++;
                    userModel.Exp -= maxExp;
                    maxExp = userModel.Lv * 100;
                }

                userCache.UpdateUserModel(userModel);
            }
            
            //获取失败身份所有的玩家id
            List<int> loseUIds = room.GetDifferentIdentiyUIds(userId);
            //给失败的玩家添加负场
            foreach (var item in loseUIds)
            {
                UserModel userModel = userCache.GetModelByUid(item);
                userModel.LoseCount++;
                userModel.Bean -= winBean;
                userModel.Exp += 10;
                //看看能不能升级
                int maxExp = userModel.Lv * 100;
                while (userModel.Exp >= maxExp)
                {
                    userModel.Lv++;
                    userModel.Exp -= maxExp;
                    maxExp = userModel.Lv * 100;
                }

                userCache.UpdateUserModel(userModel);
            }
            //给逃跑玩家添加逃跑场次
            foreach (var item in room.LeaveUIdList)
            {
                UserModel userModel = userCache.GetModelByUid(item);
                userModel.RunCount++;
                userModel.Bean -= 2 * winBean;
                userCache.UpdateUserModel(userModel);
            }

            //需要给客户端发消息 赢得身份是什么？ 谁赢了？ 加多少豆子
            OverDto dto = new OverDto();
            dto.WinIdentity = winIdentity;
            dto.WinUIdList = winUIds;
            dto.BeanCount = winBean;

            brocast(room, OpCode.FIGHT, FightCode.OVER_BRO, dto);
            //在缓存层销毁房间数据
            fightCache.Destory(room);
        }
        /// <summary>
        /// 转换出牌
        /// </summary>
        private void turn(FightRoom room)
        {
            //下一个出牌的id
            int nextUId = room.Turn();
            if (room.IsOffline(nextUId))
            {
                //下一个玩家掉线了 递归直到不掉线的玩家出牌为止
                turn(room);
            }
            else
            {
                //如果没掉线 出牌的广播
                //ClientPeer nextClient = userCache.GetClientPeerById(nextUId);
                // nextClient.Send(OpCode.FIGHT, FightCode.TURN_DEAL_BRO, nextUId);
                brocast(room, OpCode.FIGHT, FightCode.TURN_DEAL_BRO, nextUId);
            }
        }
        /// <summary>
        /// 抢地主处理
        /// </summary>
        /// <param name="result"></param>
        /// <param name="client"></param>
        private void grabLandLord(bool result,ClientPeer client)
        {
            if (userCache.IsOnline(client) == false)
                return;
            int userId = userCache.GetIdByClientPeer(client);
            FightRoom room = fightCache.GetRoomByUId(userId);
            if (result == true)
            {
                //抢地主
                room.SetLandlord(userId);
                //房间广播谁当了地主（抢地主成功）
                //  两个信息 地主id 三张底牌
                GrabDto dto = new GrabDto(userId, room.TableCardList,room.getUserCards(userId));
                brocast(room, OpCode.FIGHT, FightCode.GRAB_LANDLORD_BRO, dto);
                //发送一个出牌的命令广播 让userid出牌
                brocast(room, OpCode.FIGHT, FightCode.TURN_DEAL_BRO, userId);

            }
            else
            {
                //不抢地主
                int nextUId = room.GetNextUId(userId);
                //发送到客户端下一个抢地主的玩家id 之后客户端通过比较id来确认是不是自己的回合 从而显示抢或者不抢的按钮
                brocast(room, OpCode.FIGHT, FightCode.TURN_GRAB_BRO, nextUId);
            }
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        public void StartFight(List<int> uidList)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    //创建战斗房间
                    FightRoom room = fightCache.Create(uidList);
                    room.InitPlayCards();
                    room.Sort();
                    //发送给每个客户端 他自身有什么牌
                    foreach (var uid in uidList)
                    {
                        ClientPeer client = userCache.GetClientPeerById(uid);
                        //这里未来可以考虑只发送卡牌的id 可以进一步优化性能 给每张牌都订一个id 之后让客户端去Constant中用id换对应的卡牌即可
                        List<CardDto> cardList = room.getUserCards(uid); 
                        client.Send(OpCode.FIGHT, FightCode.GET_CARD_SRES, cardList);
                    }
                    //发送开始抢地主的广播
                    int firstUserId = room.GetFirstUId();
                    brocast(room, OpCode.FIGHT, FightCode.TURN_GRAB_BRO, firstUserId);
                }
                
                );
        }
        /// <summary>
        /// 给战斗发房间发广播
        /// </summary>
        /// <param name="room"></param>
        /// <param name="opCode"></param>
        /// <param name="subCode"></param>
        /// <param name="value"></param>
        /// <param name="exClient"></param>

        private void brocast(FightRoom room,int opCode, int subCode, object value, ClientPeer exClient = null)
        {
            //这样可以让所有要广播的连接对象不必做复杂的序列化 直接发packet即可
            //之前的方法 每个client都要做以下的打包操作 这个只需要广播的时候做一次即可发出多个相同的包 为了服务器性能优化
            //Console.WriteLine("向房间发送广播" + value.ToString());
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);


            foreach (var player in room.PlayerList)
            {
                //fix bug
                if (userCache.IsOnline(player.Userid))
                {
                    ClientPeer client = userCache.GetClientPeerById(player.Userid);
                    if (client == exClient)
                    {
                        continue;
                    }
                    client.Send(packet);
                }
            }
        }
    }
}

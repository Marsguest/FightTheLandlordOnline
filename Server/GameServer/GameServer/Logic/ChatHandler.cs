using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Cache;
using GameServer.Cache.Match;
using GscsdServer;
using Protocol.Code;
using Protocol.Constant;
using Protocol.Dto;

namespace GameServer.Logic
{
   
    public class ChatHandler : IHandler
    {
        

        private UserCache userCache = Caches.User;
        private MatchCache matchCache = Caches.Match;
        public void OnDisconnect(ClientPeer client)
        {
            
        }

        public void onReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case ChatCode.CREQ:
                    chatRequest(client, (int)value);
                    break;
                default:
                    break;
            }
        }
        private void chatRequest(ClientPeer client,int chatType)
        {
            //接收到的是聊天类型 
            //返回的是什么
            if (userCache.IsOnline(client) == false)
            {
                return;
            }
            //  谁？ 发了什么？
            int userId = userCache.GetIdByClientPeer(client);
            ChatDto chatDto = new ChatDto(userId, chatType);
            //给谁？房间内的每一个玩家
            if (matchCache.IsMatching(userId))
            {
                MatchRoom matchRoom = matchCache.GetRoom(userId);
                matchRoom.Brocast(OpCode.CHAT, ChatCode.SRES, chatDto);
            }
            else 
            {
                //TODO 在这里检测战斗房间
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Logic;
using GscsdServer;
using Protocol;
using Protocol.Code;

namespace GameServer
{
    /// <summary>
    /// 网络的消息中心 仅仅是做消息的转发
    /// </summary>
    class NetMsgCenter : IApplication
    {
        private IHandler account = new AccountHandler();
        private IHandler user = new UserHandler();
        private MatchHandler match = new MatchHandler();
        private IHandler chat = new ChatHandler();
        private FightHandler fight = new FightHandler();

        public NetMsgCenter()
        {
            //中介者模式 将match的开始游戏消息传给fight
            match.startFight += fight.StartFight;
        }


        public void OnDisconnect(ClientPeer client)
        {
            //注意卸载的顺序 要逆向卸载
            fight.OnDisconnect(client);
            chat.OnDisconnect(client);
            match.OnDisconnect(client);
            user.OnDisconnect(client);
            account.OnDisconnect(client);
        }


        public void OnReceive(ClientPeer client, SocketMsg msg)
        {
            switch (msg.OpCode)
            {
                case OpCode.ACCOUNT:
                    account.onReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.USER:
                    user.onReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.MATCH:
                    match.onReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.CHAT:
                    chat.onReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.FIGHT:
                    fight.onReceive(client, msg.SubCode, msg.Value);
                    break;
                default:
                    break;
            }
        }
    }
}

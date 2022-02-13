using GscsdServer;
using System;
using System.Collections.Generic;
using System.Text;
using Protocol;

namespace GameServer.Logic
{
    public interface IHandler
    {
        void onReceive(ClientPeer client,int subCode,object value);

        void OnDisconnect(ClientPeer client);
    }
}

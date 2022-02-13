using System;
using GscsdServer;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServerPeer server = new ServerPeer();
            //指定所关联的应用
            server.SetApplicaton(new NetMsgCenter());
            server.Start(6666,10);

            Console.ReadKey();
        }
    }
}

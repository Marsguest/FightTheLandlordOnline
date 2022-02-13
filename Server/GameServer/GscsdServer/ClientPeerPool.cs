using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GscsdServer
{
    /// <summary>
    /// 客户端的连接池
    ///     作用：重用客户端的连接对象
    /// </summary>
    public class ClientPeerPool
    {
        private Queue<ClientPeer> clientPeerQueue;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity">容量</param>
        public ClientPeerPool(int capacity)
        {
            clientPeerQueue = new Queue<ClientPeer>(capacity);
        }
        public void Enqueue(ClientPeer client)
        {
            clientPeerQueue.Enqueue(client);
        }
        public ClientPeer Dequeue()
        {
            return clientPeerQueue.Dequeue();
        }
    }
}

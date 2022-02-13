using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using GameServer.Model;
using GscsdServer;
using GscsdServer.Util.Concurrent;

namespace GameServer.Cache
{
    /// <summary>
    /// 角色数据缓存层
    /// </summary>
    public class UserCache
    {
        /// <summary>
        /// 角色id 对应的 角色数据模型
        /// </summary>
        private Dictionary<int, UserModel> idModelDict = new Dictionary<int, UserModel>();
        /// <summary>
        /// 账号id 对应的角色id
        /// </summary>
        private Dictionary<int, int> accIdUidDict = new Dictionary<int, int>();
        //ConcurrentDictionary

        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="name">角色名</param>
        /// <param name="accountId">账号id</param>
        public void Create(string name,int accountId)
        {
            UserModel model = new UserModel(id.Add_Get(), name, accountId);
            //保存到字典里 实际上就是写入数据库
            idModelDict.Add(model.Id, model);
            accIdUidDict.Add(model.AccountId, model.Id);
        }
        /// <summary>
        /// 判断此账号下是否有角色
        /// </summary>
        /// <param name="_accountId"></param>
        /// <returns></returns>
        public bool isExist(int _accountId)
        {
            return accIdUidDict.ContainsKey(_accountId);
        }
        /// <summary>
        /// 根据账号id获取角色信息
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public UserModel GetModelByAccountId(int _accountId)
        {
            int userid = accIdUidDict[_accountId];
            UserModel model = idModelDict[userid];
            return model;
        }
        public UserModel GetModelByUid(int _userId)
        {
            return idModelDict[_userId];
        }
        /// <summary>
        /// 根据账号id获取角色id
        /// </summary>
        /// <returns></returns>
        public int GetId(int _accountId)
        {
            return accIdUidDict[_accountId];
        }

        //存储在线玩家 只有在线玩家才有 clientPeer对象
        private Dictionary<int, ClientPeer> idClientDict = new Dictionary<int, ClientPeer>();
        private Dictionary<ClientPeer, int> clientIdDict = new Dictionary<ClientPeer, int>();
        /// <summary>
        /// 是否在线
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsOnline(ClientPeer client)
        {
            return clientIdDict.ContainsKey(client);
        }

        public bool IsOnline(int _id)
        {
            return idClientDict.ContainsKey(_id);
        }
        public void Online(ClientPeer client,int id)
        {
            idClientDict.Add(id, client);
            clientIdDict.Add(client, id);
        }

        public void Offline(ClientPeer client)
        {
            int id = clientIdDict[client];
            clientIdDict.Remove(client);
            idClientDict.Remove(id);
        }
        /// <summary>
        /// 根据连接对象获取角色model
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public UserModel GetModelByClientPeer(ClientPeer client)
        {
            int id = clientIdDict[client];
            UserModel model = idModelDict[id];
            return model;
        }
        /// <summary>
        /// 根据id获取连接对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientPeer GetClientPeerById(int id)
        {
            return idClientDict[id];
        }
        /// <summary>
        /// 根据玩家的在线连接对象获取角色id
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public int GetIdByClientPeer(ClientPeer client)
        {
            if (!clientIdDict.ContainsKey(client))
            {
                throw new IndexOutOfRangeException("这个连接对象不在clientIdDict字典中！");
            }
            return clientIdDict[client];
        }
        /// <summary>
        /// 更新用户数据模型
        /// </summary>
        /// <param name="model"></param>
        public void UpdateUserModel(UserModel model)
        {
            idModelDict[model.Id] = model;
        }
    }
}

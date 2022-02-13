﻿using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Model;
using GscsdServer;
using GscsdServer.Util.Concurrent;

namespace GameServer.Cache
{
    /// <summary>
    /// 账号缓存
    ///     存账号的地方 后期会存到数据库
    /// </summary>
    public class AccountCache
    {
        /// <summary>
        /// 账号对应的 账号数据模型
        /// </summary>
        private Dictionary<string, AccountModel> accModelDict = new Dictionary<string, AccountModel>();
        /// <summary>
        /// 是否存在账号
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns></returns>
        public bool IsExist(string account)
        {
            return accModelDict.ContainsKey(account);
        }
        /// <summary>
        /// 用来存储账号id 
        ///     后期是数据库来处理的
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);
        /// <summary>
        /// 创建账号
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        public void Create(string account, string password)
        {
            AccountModel model = new AccountModel(id.Add_Get(), account, password);
            accModelDict.Add(model.Account, model);
        }
        /// <summary>
        /// 获取账号对应的数据模型
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public AccountModel GetModel(string account)
        {
            return accModelDict[account];
        }

        public bool IsMatch(string account, string password)
        {
            AccountModel model = accModelDict[account];
            return model.Password == password;
        }

        /// <summary>
        /// 账号 对应 连接对象
        /// </summary>
        private Dictionary<string, ClientPeer> accClientDict = new Dictionary<string, ClientPeer>();
        private Dictionary<ClientPeer, string> clientAccDict = new Dictionary<ClientPeer, string>();
        /// <summary>
        /// 是否在线
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool IsOnline(string account)
        {
            return accClientDict.ContainsKey(account);
        }

        public bool IsOnline(ClientPeer client)
        {
            return clientAccDict.ContainsKey(client);
        }
        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="client"></param>
        /// <param name="account"></param>
        public void Online(ClientPeer client,string account)
        {
            accClientDict.Add(account,client);
            clientAccDict.Add(client, account);
        }
        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="client"></param>
        public void Offline(ClientPeer client)
        {
            string account = clientAccDict[client];
            accClientDict.Remove(account);
            clientAccDict.Remove(client);
        }

        public void Offline(string account)
        {
            ClientPeer client = accClientDict[account];
            accClientDict.Remove(account);
            clientAccDict.Remove(client);
        }
        /// <summary>
        /// 获取在线玩家的id
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public int GetId(ClientPeer client)
        {
            string account = clientAccDict[client];
            AccountModel model = accModelDict[account];
            return model.Id;
        }
    }
}

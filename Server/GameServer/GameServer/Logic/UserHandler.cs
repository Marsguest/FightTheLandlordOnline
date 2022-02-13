using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Cache;
using GameServer.Model;
using GscsdServer;
using Protocol.Code;
using Protocol.Dto;
using Protocol.Protocol;

namespace GameServer.Logic
{
    /// <summary>
    /// 用户的逻辑处理层
    /// </summary>
    public class UserHandler : IHandler
    {
        private UserCache userCache = Caches.User;
        private AccountCache accountCache = Caches.Account;
        public void OnDisconnect(ClientPeer client)
        {
            if (userCache.IsOnline(client))
                userCache.Offline(client);
        }

        public void onReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case UserCode.CREATE_CREQ:
                    SingleExecute.Instance.Execute(() => create(client, value.ToString()));
                    break;
                case UserCode.GET_INFO_CREQ:
                    SingleExecute.Instance.Execute(() => getInfo(client));
                    break;
                case UserCode.ONLINE_CREQ:
                    SingleExecute.Instance.Execute(() => online(client));
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="client">客户端的连接对象 </param>
        /// <param name="name">客户端传过来的名字</param>
        private void create(ClientPeer client, string name)
        {
            //判断这个客户端是不是非法登录
            if (!accountCache.IsOnline(client))
            {
                client.Send(OpCode.USER, UserCode.CREATE_SRES, UserProtocol.CREATE_NOT_LAW);
                return;
            }
                
            //获取账号id
            int accountId = accountCache.GetId(client);
            //判断这个账号之前有没有角色
            if (userCache.isExist(accountId))
            {
                client.Send(OpCode.USER, UserCode.CREATE_SRES, UserProtocol.CREATE_ALREADY_HAS_USER);
                return;
            }
                
            //没有问题才可以创建
            userCache.Create(name, accountId);
            client.Send(OpCode.USER, UserCode.CREATE_SRES, UserProtocol.CREATE_SUCCESS);
        }
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="client"></param>
        private void getInfo(ClientPeer client)
        {
            //判断这个客户端是不是非法登录
            if (!accountCache.IsOnline(client))//这里表示还没有client就想登录显然是非法登录
            {
                //client.Send(OpCode.USER, UserCode.GET_INFO_SRES, UserProtocol.CREATE_NOT_LAW);
                Console.WriteLine("非法登录");
                return;
            }
            //获取账号id
            int accountId = accountCache.GetId(client);
            //判断这个账号之前有没有角色
            if (!userCache.isExist(accountId))
            {
                Console.WriteLine("accountId："+accountId+" 没有角色");
                client.Send(OpCode.USER, UserCode.GET_INFO_SRES, null);
                return;
            }
            //代码执行到这里代表有角色
            //上线角色
            if (userCache.IsOnline(client) == false) //防止二次调用上线的方法 导致服务器出现异常
            {
                online(client);
            }
            
            //给客户端发送自己的角色信息
            UserModel model = userCache.GetModelByAccountId(accountId);
            UserDto dto = new UserDto(model.Id,model.Name, model.Bean, model.WinCount, model.LoseCount,model.RunCount ,model.Lv, model.Exp);
            client.Send(OpCode.USER, UserCode.GET_INFO_SRES, dto); //获取成功
        }
        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="client"></param>
        private void online(ClientPeer client)
        {
            //判断这个客户端是不是非法登录
            if (!accountCache.IsOnline(client))
            {
                client.Send(OpCode.USER, UserCode.ONLINE_SRES, UserProtocol.CREATE_NOT_LAW);
                return;
            }
            //获取账号id
            int accountId = accountCache.GetId(client);
            //判断这个账号之前有没有角色
            if (!userCache.isExist(accountId))
            {
                client.Send(OpCode.USER, UserCode.ONLINE_SRES, UserProtocol.ONLINE_HAS_NO_USER);
                return;
            }
            //上线成功了
            int userId = userCache.GetId(accountId);
            userCache.Online(client, userId);
            client.Send(OpCode.USER, UserCode.ONLINE_SRES, UserProtocol.ONLINE_SUCCESS); // 上线成功
        }
    }
}

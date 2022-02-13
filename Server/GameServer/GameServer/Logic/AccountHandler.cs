using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Cache;
using GscsdServer;
using Protocol.Code;
using Protocol.Dto;
using Protocol.Protocol;

namespace GameServer.Logic
{
    /// <summary>
    /// 账号处理的逻辑层
    /// </summary>
    public class AccountHandler : IHandler
    {
        AccountCache accountCache = Caches.Account;
        public void OnDisconnect(ClientPeer client)
        {
            if(accountCache.IsOnline(client))
                accountCache.Offline(client);
        }

        public void onReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case AccountCode.REGIST_CREQ:
                    {
                        AccountDto dto = value as AccountDto;
                        //Console.WriteLine(dto.Account);
                        //Console.WriteLine(dto.Password);
                        //单线程处理
                        SingleExecute.Instance.Execute(() => regist(client, dto.Account, dto.Password)); 
                    }
                    break;
                case AccountCode.LOGIN_CREQ:
                    {
                        AccountDto dto = value as AccountDto;
                        //Console.WriteLine(dto.Account);
                        //Console.WriteLine(dto.Password);
                        //单线程处理
                        SingleExecute.Instance.Execute(() => login(client, dto.Account, dto.Password));
                    }
                    break;
                default:
                    break;
            }
        }
        private void regist(ClientPeer client, string account, string password)
        {
            if (accountCache.IsExist(account))
            {
                //表示账号已经存在
                client.Send(OpCode.ACCOUNT, AccountCode.REGIST_SRES, AccountProtocol.REGIST_AccountIsExist);
                return;
            }

            if (string.IsNullOrEmpty(account))
            {
                //表示账号输入不合法
                client.Send(OpCode.ACCOUNT, AccountCode.REGIST_SRES, AccountProtocol.REGIST_AccountNotLaw);
                return;
            }

            if(string.IsNullOrEmpty(password)||password.Length < 4 || password.Length > 16)
            {
                //表示密码不合法
                client.Send(OpCode.ACCOUNT, AccountCode.REGIST_SRES, AccountProtocol.REGIST_PasswordNotLaw);
                return;
            }
            //可以注册了
            accountCache.Create(account, password);
            client.Send(OpCode.ACCOUNT, AccountCode.REGIST_SRES, AccountProtocol.REGIST_SUCCESS);

        }

        private void login(ClientPeer client,string account,string password)
        {
            if (!accountCache.IsExist(account))
            {
                //表示账号不存在
                client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SRES, AccountProtocol.LOGIN_AccountIsNotExist);
                return;
            }
            if (accountCache.IsOnline(account))
            {
                //表示账号在线
                client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SRES, AccountProtocol.LOGIN_IsOnline);
                return;
            }
            if (!accountCache.IsMatch(account,password))
            {
                //表示账号密码不匹配
                client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SRES, AccountProtocol.LOGIN_IsNotMatch);
                return;
            }

            //登录成功
            accountCache.Online(client, account);
            client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SRES, AccountProtocol.LOGIN_SUCCESS);

        }
    }
}

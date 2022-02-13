using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Model
{
    /// <summary>
    /// 账号的数据模型 domain
    /// </summary>
    public class AccountModel
    {
        public int Id;
        public string Account;
        public string Password;

        public AccountModel(int _id,string _acc,string _pwd)
        {
            this.Id = _id;
            this.Account = _acc;
            this.Password = _pwd;
        }
    }
}

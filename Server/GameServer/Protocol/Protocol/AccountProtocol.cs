using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    public class AccountProtocol
    {
        public static AccountProtocol instance = null;
        private static object singleton_lock = new object();
        public static AccountProtocol Instance
        {
            get
            {
                lock (singleton_lock)
                {
                    if (instance == null)
                        instance = new AccountProtocol();
                    return instance;
                }
            }
        }

        private AccountProtocol()
        {
            codeMsgDict.Add(REGIST_AccountIsExist, "账号已经存在");
            codeMsgDict.Add(REGIST_AccountNotLaw, "账号输入不合法");
            codeMsgDict.Add(REGIST_PasswordNotLaw, "密码不合法");
            codeMsgDict.Add(REGIST_SUCCESS, "注册成功");

            codeMsgDict.Add(LOGIN_AccountIsNotExist, "账号不存在");
            codeMsgDict.Add(LOGIN_IsOnline, "账号已经在线");
            codeMsgDict.Add(LOGIN_IsNotMatch, "账号密码不匹配");
            codeMsgDict.Add(LOGIN_SUCCESS, "登录成功");
        }

        public const int REGIST_AccountIsExist = 0;
        public const int REGIST_AccountNotLaw = 1;
        public const int REGIST_PasswordNotLaw = 2;
        public const int REGIST_SUCCESS = 3;

        public const int LOGIN_AccountIsNotExist = 4;
        public const int LOGIN_IsOnline = 5;
        public const int LOGIN_IsNotMatch = 6;
        public const int LOGIN_SUCCESS = 7;

        public Dictionary<int, string> codeMsgDict = new Dictionary<int, string>();


    }
}

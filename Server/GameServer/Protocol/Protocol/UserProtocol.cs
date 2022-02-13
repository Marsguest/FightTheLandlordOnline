using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    public class UserProtocol
    {
        #region 单例
        public static UserProtocol instance = null;
        private static object singleton_lock = new object();
        public static UserProtocol Instance
        {
            get
            {
                lock (singleton_lock)
                {
                    if (instance == null)
                        instance = new UserProtocol();
                    return instance;
                }
            }
        }
        #endregion
        private UserProtocol()
        {
            codeMsgDict.Add(CREATE_NOT_LAW, "非法登录");
            codeMsgDict.Add(CREATE_ALREADY_HAS_USER, "已经有角色 不能重复创建");
            codeMsgDict.Add(CREATE_SUCCESS,"创建成功");

            codeMsgDict.Add(GET_INFO_HAS_NO_USER, "没有创建角色 不能获取信息");

            codeMsgDict.Add(ONLINE_HAS_NO_USER, "没有创建角色 不能上线");
            codeMsgDict.Add(ONLINE_SUCCESS, "上线成功");
        }

        public const int CREATE_NOT_LAW = 0;
        public const int CREATE_ALREADY_HAS_USER = 1;
        public const int CREATE_SUCCESS = 2;

        public const int GET_INFO_HAS_NO_USER = 3;

        public const int ONLINE_HAS_NO_USER = 4;
        public const int ONLINE_SUCCESS = 5;

        public Dictionary<int, string> codeMsgDict = new Dictionary<int, string>();

    }
}

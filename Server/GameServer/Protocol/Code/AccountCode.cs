using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    public class AccountCode
    {
        //注册的操作码 subCode
        public const int REGIST_CREQ = 0;//clinet request //参数accountDto
        public const int REGIST_SRES = 1;//server response
        //登录的操作码 subCode
        public const int LOGIN_CREQ = 2; //参数accountDto
        public const int LOGIN_SRES = 3; //参数accountDto
    }
}

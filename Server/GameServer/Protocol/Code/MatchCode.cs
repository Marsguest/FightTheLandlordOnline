using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    /// <summary>
    /// 有关匹配的操作码
    /// </summary>
    public class MatchCode
    {
        //进入匹配队列
        public const int ENTER_MATCH_QUEUE_CREQ = 0;
        public const int ENTER_MATCH_QUEUE_SRES = 1;
        public const int ENTER_MATCH_QUEUE_BRO = 8;
        //离开匹配队列
        public const int LEAVE_MATCH_QUEUE_CREQ = 2;
        public const int LEAVE_MATCH_QUEUE_BRO = 3;//广播
        //准备
        public const int READY_CREQ = 4;
        //public const int READY_SRES = 5;
        public const int READY_BRO = 5;  //广播
        //开始游戏
        //public const int START_CREQ = 6;
        //public const int START_SRES = 7;
        public const int START_BRO = 6;//广播
    }
}

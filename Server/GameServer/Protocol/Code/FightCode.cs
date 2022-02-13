using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    public class FightCode
    {
        public const int GRAB_LANDLORD_CREQ = 0; //客户端发起抢地主的请求
        public const int GRAB_LANDLORD_BRO = 1; //服务器 抢地主的广播（这个是确定了谁抢到了地主）
        public const int TURN_GRAB_BRO = 2; //服务器广播让下一个玩家抢地主

        public const int DEAL_CREQ = 3;//客户端发起出牌的请求
        public const int DEAL_SRES = 4;//服务器给客户端出牌的响应
        public const int DEAL_BRO = 5; //服务器广播出牌的结果

        public const int PASS_CREQ = 6; //客户端发起不出的请求
        public const int PASS_SRES = 7; //服务器发送不出的响应

        public const int TURN_DEAL_BRO = 8;//服务器广播转换出牌的结果

        public const int LEAVE_BRO = 9;//服务器 有玩家退出游戏的广播

        public const int OVER_BRO = 10;//服务器 游戏结束的广播

        public const int GET_CARD_SRES = 11;//服务器给客户端卡牌的响应

    }
}

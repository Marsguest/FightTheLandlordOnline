using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Dto.Fight;

namespace Protocol.Constant
{
    /// <summary>
    /// 卡牌的权值
    /// </summary>
    public class CardWeight
    {
        public const int THREE = 3;
        public const int FOUR = 4;
        public const int FIVE = 5;
        public const int SIX = 6;
        public const int SEVEN = 7;
        public const int EIGHT = 8;
        public const int NINE = 9;
        public const int TEN = 10;

        public const int JACK = 11;
        public const int QUEEN = 12;
        public const int KING = 13;

        public const int ONE = 14;
        public const int TWO = 15;

        public const int SJOKER = 16;
        public const int LJOKER = 17;

        public static string GetString(int weight)
        {
            switch (weight)
            {
                case THREE:
                    return "Three";
                case FOUR:
                    return "Four";
                case FIVE:
                    return "Five";
                case SIX:
                    return "Six";
                case SEVEN:
                    return "Seven";
                case EIGHT:
                    return "Eight";
                case NINE:
                    return "Nine";
                case TEN:
                    return "Ten";
                case JACK:
                    return "Jack";
                case QUEEN:
                    return "Queen";
                case KING:
                    return "King";
                case ONE:
                    return "One";
                case TWO:
                    return "Two";
                case SJOKER:
                    return "SJoker";
                case LJOKER:
                    return "LJoker";
                default:
                    throw new Exception("不存在这样的权值！");
            }
        }
        /// <summary>
        /// 获取卡牌的权值
        /// </summary>
        /// <param name="cardList">选中的卡牌</param>
        /// <param name="cardType">卡牌的类型</param>
        /// <returns></returns>
        public static int GetWeight(List<CardDto> cardList,int cardType)
        {
            int totalWeright = 0;
            if(cardType == CardType.THREE_ONE || cardType == CardType.THREE_TWO)
            {
                //如果是三带一或三代二
                //3335 5333
                // fix bug
                for (int i = 0; i < cardList.Count - 2; i++)
                {
                    if (cardList[i].Weight == cardList[i+1].Weight && cardList[i].Weight == cardList[i+2].Weight)
                    {
                        totalWeright += cardList[i].Weight * 3;
                    }
                }
            }
            else
            {
                for (int i = 0; i < cardList.Count; i++)
                {
                    totalWeright += cardList[i].Weight;
                }
            }
            return totalWeright;
        }
    }
}

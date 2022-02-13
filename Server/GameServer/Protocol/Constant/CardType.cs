using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Dto.Fight;

namespace Protocol.Constant
{
    /// <summary>
    /// 卡牌的类型
    /// </summary>
    public class CardType
    {
        public const int NONE = 0;
        public const int SINGLE = 1; //单
        public const int DOUBLE = 2; //对

        public const int STRAIGHT = 3;//顺子
        public const int DOUBLE_STRAIGHT = 4; //双顺子445566
        public const int TRIPLE_STRAIGHT = 5;//三顺子444555666
        public const int THREE = 6; //三不带 333
        public const int THREE_ONE = 7; //三带一
        public const int THREE_TWO = 8; //三代二
        public const int BOOM = 9; //炸弹
        public const int JOKER_BOOM = 10; //王炸
        /// <summary>
        /// 是否是单排
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsSingle(List<CardDto> cardList)
        {
            return cardList.Count == 1;
        }
        /// <summary>
        /// 是否是对
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsDouble(List<CardDto> cardList)
        {
            if (cardList.Count== 2)
            {
                if (cardList[0].Weight == cardList[1].Weight)
                    return true;
                
            }

            return false;
        }
        /// <summary>
        /// 是否是顺子
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsStraight(List<CardDto> cardList)
        {
            if (cardList.Count < 5 || cardList.Count > 12)
                return false;
            for (int i = 0; i < cardList.Count - 1; i++)
            {
                int tmpWeight = cardList[i].Weight;
                //后一个与前一个牌差不能不等于1
                if (cardList[i + 1].Weight - tmpWeight != 1)
                    return false;
                //不能超过A
                if (tmpWeight > CardWeight.ONE || cardList[i + 1].Weight > CardWeight.ONE)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 双顺
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsDoubleStraight(List<CardDto> cardList)
        {
            if (cardList.Count < 6 || cardList.Count % 2 != 0)
                return false;
            for (int i = 0; i < cardList.Count - 2; i+=2)
            {
                if (cardList[i].Weight != cardList[i + 1].Weight)
                    return false;
                if (cardList[i + 2].Weight - cardList[i].Weight != 1)
                    return false;
                //不能超过A
                if (cardList[i].Weight > CardWeight.ONE || cardList[i + 2].Weight > CardWeight.ONE)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 是否是飞机
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsTripleStraight(List<CardDto> cardList)
        {
            //333 444
            if (cardList.Count < 6 || cardList.Count % 3 != 0)
                return false;
            for (int i = 0; i < cardList.Count - 3; i += 3)
            {
                if (cardList[i].Weight != cardList[i + 1].Weight)
                    return false;
                if (cardList[i+1].Weight != cardList[i + 2].Weight)
                    return false;

                if (cardList[i + 3].Weight - cardList[i].Weight != 1)
                    return false;
                //不能超过A
                if (cardList[i].Weight > CardWeight.ONE || cardList[i + 3].Weight > CardWeight.ONE)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 三不带
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsThree(List<CardDto> cardList)
        {
            if (cardList.Count != 3)
                return false;
            if (cardList[0].Weight != cardList[1].Weight)
                return false;
            if (cardList[1].Weight != cardList[2].Weight)
                return false;

            return true;
        }
        /// <summary>
        /// 三带一
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsThreeAndOne(List<CardDto> cardList)
        {
            if (cardList.Count != 4)
                return false;
            if (cardList[0].Weight == cardList[1].Weight && cardList[1].Weight == cardList[2].Weight)
                return true;
            if (cardList[1].Weight == cardList[2].Weight && cardList[2].Weight == cardList[3].Weight)
                return true;
            return false;
        }
        /// <summary>
        /// 三带二
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsThreeAndTwo(List<CardDto> cardList)
        {
            if (cardList.Count != 5)
                return false;
            if (cardList[0].Weight == cardList[1].Weight && cardList[1].Weight == cardList[2].Weight)
            {
                if (cardList[3].Weight == cardList[4].Weight)
                    return true;
            }
                
            if (cardList[2].Weight == cardList[3].Weight && cardList[3].Weight == cardList[4].Weight)
            {
                if (cardList[0].Weight == cardList[1].Weight)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 炸弹
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsBooom(List<CardDto> cardList)
        {
            if (cardList.Count != 4)
                return false;
            if (cardList[0].Weight != cardList[1].Weight)
                return false;
            if (cardList[1].Weight != cardList[2].Weight)
                return false;
            if (cardList[2].Weight != cardList[3].Weight)
                return false;

            return true;
        }
        /// <summary>
        /// 王炸
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static bool IsJokerBoom(List<CardDto> cardList)
        {
            if (cardList.Count != 2)
                return false;
            if (cardList[0].Weight == CardWeight.SJOKER && cardList[1].Weight == CardWeight.LJOKER)
                return true;
            if (cardList[1].Weight == CardWeight.SJOKER && cardList[0].Weight == CardWeight.LJOKER)
                return true;
            return false;
        }
        /// <summary>
        /// 返回出牌类型
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static int GetCardType(List<CardDto> cardList)
        {
            int _cardType = CardType.NONE;
            switch (cardList.Count)
            {
                case 1:
                    if (IsSingle(cardList))
                    {
                        _cardType = CardType.SINGLE;
                    }
                    break;
                case 2:
                    if (IsDouble(cardList))
                    {
                        _cardType = CardType.DOUBLE;
                    }else if (IsJokerBoom(cardList))
                    {
                        _cardType = CardType.JOKER_BOOM;
                    }
                    break;
                case 3:
                    if (IsThree(cardList))
                    {
                        _cardType = CardType.THREE;
                    }
                    break;
                case 4:
                    if (IsThreeAndOne(cardList))
                    {
                        _cardType = CardType.THREE_ONE;
                    }else if (IsBooom(cardList))
                    {
                        _cardType = CardType.BOOM;
                    }
                    break;
                case 5:
                    if (IsStraight(cardList))
                    {
                        _cardType = CardType.STRAIGHT;
                    }
                    else if (IsThreeAndTwo(cardList))
                    {
                        _cardType = CardType.THREE_TWO;
                    }
                    break;
                case 6:
                    if (IsStraight(cardList))
                    {
                        _cardType = CardType.STRAIGHT;
                    }
                    else if (IsDoubleStraight(cardList))
                    {
                        _cardType = CardType.DOUBLE_STRAIGHT;
                    }
                    else if (IsTripleStraight(cardList))
                    {
                        _cardType = CardType.TRIPLE_STRAIGHT;
                    }
                    break;
                case 7:
                    if (IsStraight(cardList))
                    {
                        _cardType = CardType.STRAIGHT;
                    }
                    break;
                case 8:
                    if (IsStraight(cardList))
                    {
                        _cardType = CardType.STRAIGHT;
                    }
                    else if (IsDoubleStraight(cardList))
                    {
                        _cardType = CardType.DOUBLE_STRAIGHT;
                    }
                    break;
                case 9:
                    if (IsStraight(cardList))
                    {
                        _cardType = CardType.STRAIGHT;
                    }
                    else if (IsTripleStraight(cardList))
                    {
                        _cardType = CardType.TRIPLE_STRAIGHT;
                    }
                    break;
                case 10:
                    if (IsStraight(cardList))
                    {
                        _cardType = CardType.STRAIGHT;
                    }
                    else if (IsDoubleStraight(cardList))
                    {
                        _cardType = CardType.DOUBLE_STRAIGHT;
                    }
                    break;
                case 11:
                    if (IsStraight(cardList))
                    {
                        _cardType = CardType.STRAIGHT;
                    }break;
                case 12:
                    if (IsStraight(cardList))
                    {
                        _cardType = CardType.STRAIGHT;
                    }
                    else if (IsDoubleStraight(cardList))
                    {
                        _cardType = CardType.DOUBLE_STRAIGHT;
                    }
                    else if (IsTripleStraight(cardList))
                    {
                        _cardType = CardType.TRIPLE_STRAIGHT;
                    }
                    break;
                case 13:
                    break;
                case 14:
                    if (IsDoubleStraight(cardList))
                    {
                        _cardType = CardType.DOUBLE_STRAIGHT;
                    }
                    break;
                case 15:
                    if (IsTripleStraight(cardList))
                    {
                        _cardType = CardType.TRIPLE_STRAIGHT;
                    }
                    break;
                case 16:
                    if (IsDoubleStraight(cardList))
                    {
                        _cardType = CardType.DOUBLE_STRAIGHT;
                    }
                    break;
                case 17:
                    break;
                case 18:
                    if (IsDoubleStraight(cardList))
                    {
                        _cardType = CardType.DOUBLE_STRAIGHT;
                    }
                    else if (IsTripleStraight(cardList))
                    {
                        _cardType = CardType.TRIPLE_STRAIGHT;
                    }
                    break;
                case 19:
                    break;
                case 20:
                    if (IsDoubleStraight(cardList))
                    {
                        _cardType = CardType.DOUBLE_STRAIGHT;
                    }
                    break;

                default:
                    break;
            }
            return _cardType;
        }

    }
}

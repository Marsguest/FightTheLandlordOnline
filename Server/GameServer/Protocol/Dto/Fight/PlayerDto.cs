using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol.Constant;

namespace Protocol.Dto.Fight
{
    /// <summary>
    /// 玩家的数据传输模型
    /// </summary>
    [Serializable]
    public class PlayerDto
    {
        public int Userid; //用户id
        public int MIdentity; //身份
        public List<CardDto> CardList; //自己拥有的手牌

        public PlayerDto(int userId)
        {
            this.Userid = userId;
            this.MIdentity = Identity.FARMER;
            this.CardList = new List<CardDto>();
        }
        /// <summary>
        /// 是否还有手牌
        /// </summary>
        /// <returns></returns>
        public bool HasCard()
        {
            return CardList.Count > 0;
        }
        /// <summary>
        /// 手牌数量
        /// </summary>
        /// <returns></returns>
        public int CardCount()
        {
            return CardList.Count;
        }
        /// <summary>
        /// 添加卡牌
        /// </summary>
        /// <param name="card"></param>
        public void AddCard(CardDto card)
        {
            CardList.Add(card);
        }
        /// <summary>
        /// 移除卡牌
        /// </summary>
        /// <param name="card"></param>
        public void RemoveCard(CardDto card)
        {
            CardList.Remove(card);
        }
    }
}

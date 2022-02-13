using System;
using System.Collections.Generic;
using System.Text;
using GscsdServer.Util.Concurrent;
using Protocol.Constant;
using Protocol.Dto.Fight;

namespace GameServer.Cache.Fight
{
    /// <summary>
    /// 牌库
    /// </summary>
    public class LibraryModel
    {
        /// <summary>
        /// 所有牌的队列
        /// </summary>
        public Queue<CardDto> CardQueue { get; set; }

        public LibraryModel()
        {
            //创建牌
            create();
            //洗牌
            shuffle();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            //创建牌
            create();
            //洗牌
            shuffle();
        }

        ConcurrentInt cardId = new ConcurrentInt(-1);
        /// <summary>
        /// 创建牌
        /// </summary>
        private void create()
        {
            CardQueue = new Queue<CardDto>();
            //创建普通的牌
            for (int color = CardColor.CLUB; color <= CardColor.SQUARE; color++)
            {
                for (int weight = CardWeight.THREE; weight <= CardWeight.TWO; weight++)
                {
                    CardQueue.Enqueue(new CardDto(cardId.Add_Get(),CardColor.GetString(color)+CardWeight.GetString(weight), color, weight));
                }
            }
            //创建大小王
            CardDto SJoker = new CardDto(cardId.Add_Get(), "SJoker", CardColor.NONE, CardWeight.SJOKER);
            CardDto LJoker = new CardDto(cardId.Add_Get(), "LJoker", CardColor.NONE, CardWeight.LJOKER);
            CardQueue.Enqueue(SJoker);
            CardQueue.Enqueue(LJoker);
        }
        /// <summary>
        /// 洗牌算法
        /// </summary>
        private void shuffle()
        {
            List<CardDto> newCardList = new List<CardDto>();
            Random r = new Random();
            //下面这个循环时乱序插入
            foreach (var card in CardQueue)
            {
                int index = r.Next(0, newCardList.Count + 1);
                // 2 1 
                newCardList.Insert(index, card);
            }
            //在插入回之前的队列
            CardQueue.Clear();
            foreach (var card in newCardList)
            {
                CardQueue.Enqueue(card);
            }
        }
        /// <summary>
        /// 发牌
        /// </summary>
        /// <returns></returns>
        public CardDto Deal()
        {
            return CardQueue.Dequeue();
        }
    }
}

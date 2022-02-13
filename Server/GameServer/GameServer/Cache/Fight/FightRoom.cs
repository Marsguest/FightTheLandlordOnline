using System;
using System.Collections.Generic;
using System.Text;
using GscsdServer;
using Protocol;
using Protocol.Constant;
using Protocol.Dto.Fight;
using Protocol.Tool;

namespace GameServer.Cache.Fight
{
    /// <summary>
    /// 战斗房间
    /// </summary>
    public class FightRoom
    {
        /// <summary>
        /// 房间的唯一标识
        /// </summary>
        public int Id { get;private set; }
        /// <summary>
        /// 存储所有的玩家
        /// </summary>
        public List<PlayerDto> PlayerList { get; private set; }
        /// <summary>
        /// 中途退出的玩家列表
        /// </summary>
        public List<int> LeaveUIdList { get; set; }
        /// <summary>
        /// 牌库
        /// </summary>
        public LibraryModel libraryModel { get; set; }
        /// <summary>
        /// 三张底牌
        /// </summary>
        public List<CardDto> TableCardList { get; set; }
        /// <summary>
        /// 倍数
        /// </summary>
        public int Multiple { get; set; }
        /// <summary>
        /// 回合管理
        /// </summary>
        public RoundModel roundModel { get; set; }
        /// <summary>
        /// 构造方法做初始化
        /// </summary>
        /// <param name="id"></param>
        public FightRoom(int id,List<int> uidList)
        {
            this.Id = id;
            this.PlayerList = new List<PlayerDto>();
            this.LeaveUIdList = new List<int>();
            foreach (var userId in uidList)
            {
                PlayerDto player = new PlayerDto(userId);
                this.PlayerList.Add(player);
            }

            this.libraryModel = new LibraryModel();
            this.TableCardList = new List<CardDto>();
            this.Multiple = 1;
            this.roundModel = new RoundModel();
        }

        /// <summary>
        /// 转换出牌
        /// </summary>
        public int Turn()
        {
            int currUId = roundModel.CurrentUId;
            int nextUId = GetNextUId(currUId);

            //更改回合信息
            roundModel.CurrentUId = nextUId;
            return nextUId;
        }
        /// <summary>
        /// 计算下一个出牌者
        /// </summary>
        /// <param name="currUId"></param>
        /// <returns></returns>
        public int GetNextUId(int currUId)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].Userid == currUId)
                {
                    int nexUId = PlayerList[(i + 1) % 3].Userid;
                    return nexUId;
                }
            }
            //如果执行到这里 说明传来的出牌者时错误的
            throw new Exception("并没有这个出牌者!");
        }
        /// <summary>
        /// 判断能不能管上 上一回合的牌
        /// </summary>
        /// <returns></returns>
        public bool DealCard(int type,int weight,int length,int userId,List<CardDto> cardList)
        {
            bool canDeal = false;

            //用什么牌才能管什么牌 大的才能管小的
            if (type == roundModel.LastCardType && weight > roundModel.LastWeight)
            {
                //特殊的类型：顺子 还要进行长度的限制
                if (type == CardType.STRAIGHT || type == CardType.DOUBLE_STRAIGHT || type == CardType.TRIPLE_STRAIGHT)
                {
                    if (length == roundModel.LastLength)
                    {
                        //满足出牌条件
                        canDeal = true;
                    }
                }
                else
                {
                    canDeal = true;
                }
            }
            //普通的炸弹 可以管 任何不是炸弹的牌 除了王炸
            else if (type == CardType.BOOM && roundModel.LastCardType != CardType.BOOM && roundModel.LastCardType != CardType.JOKER_BOOM)
            {
                canDeal = true;
            }
            //王炸可以管任何牌
            else if (type == CardType.JOKER_BOOM )
            {
                canDeal = true;
            }
            //第一次出牌或者当前自己占地那么永远是可以出的
            else if (userId == roundModel.BiggestUId)
            {
                canDeal = true;
            }
            //如果能管上
            if (canDeal)
            {
                //移除玩家的手牌 
                removeCards(userId, cardList);
                //可能会翻倍
                if (type == CardType.BOOM)
                {
                    this.Multiple *= 2;
                }
                if (type == CardType.JOKER_BOOM)
                {
                    this.Multiple *= 4;
                }
                //保存回合信息
                roundModel.Change(length, type, weight, userId);

            }
            return canDeal;
        }
        /// <summary>
        /// 移除掉手牌
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cardList"></param>
        private void removeCards(int userId,List<CardDto> cardList)
        {
            //移除手牌列表
            //for (int i = currCardList.Count-1; i >=0 ; i--)
            //{
            //    foreach (var item in cardList)
            //    {
            //        if (currCardList[i].Name == item.Name)
            //        {
            //            //fix bug
            //            //currCardList.RemoveAt(i);
            //            currCardList.Remove(currCardList[i]);
            //        }

            //    }
            //}

            //fix bug
            //获取玩家现有的手牌
            List<CardDto> currCardList = getUserCards(userId);
            List<CardDto> tmpCardList = new List<CardDto>();
            foreach (var select in cardList)
            {
                foreach (var item in currCardList)
                {
                    if (select.Name == item.Name)
                    {
                        tmpCardList.Add(item);
                        break;
                    }
                }
            }

            foreach (var item in tmpCardList)
                currCardList.Remove(item);
        }
        /// <summary>
        /// 获取玩家的现有手牌
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CardDto> getUserCards(int userId)
        {
            foreach (var player in PlayerList)
            {
                if (player.Userid == userId)
                {
                    return player.CardList;
                }
            }
            throw new Exception("不存在这个玩家");
        }
        /// <summary>
        /// 初始化玩家手牌 发牌
        /// </summary>
        public void InitPlayCards()
        {
            //54
            //一个人17
            //剩3张 当底牌
            //谁是地主 就 给谁
            for (int i = 0; i < 17; i++)
            {
                CardDto card1 = libraryModel.Deal();
                PlayerList[0].AddCard(card1);
                CardDto card2 = libraryModel.Deal();
                PlayerList[1].AddCard(card2);
                CardDto card3 = libraryModel.Deal();
                PlayerList[2].AddCard(card3);
            }
            //发底牌 放底牌库
            for (int i = 0; i < 3; i++)
            {
                CardDto card = libraryModel.Deal();
                TableCardList.Add(card);
            }
        }
        /// <summary>
        /// 设置地主身份
        /// </summary>
        public void SetLandlord(int userId)
        {
            foreach (var player in PlayerList)
            {
                if (player.Userid == userId)
                {
                    player.MIdentity = Identity.LANDLORD;
                    //给地主发底牌
                    foreach (var card in TableCardList)
                    {
                        player.AddCard(card);
                    }
                    //给玩家手牌再次排个序
                    this.Sort();
                    //开始回合 地主来开始
                    roundModel.Start(userId);
                }
            }
        }
        /// <summary>
        /// 获取玩家模型
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PlayerDto GetPlayerModel(int userId)
        {
            foreach (var player in PlayerList)
            {
                if (player.Userid == userId)
                {
                    return player;
                }
            }
            throw new Exception("没有这个玩家 获取不到数据");
        }
        /// <summary>
        /// 获取用户的身份
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetPlayerIdentity(int userId)
        {
            return GetPlayerModel(userId).MIdentity;
        }
        /// <summary>
        /// 获取相同身份的用户的id
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public List<int> GetSameIdentiyUIds(int identity)
        {
            List<int> ids = new List<int>();
            foreach (var player in PlayerList)
            {
                if (player.MIdentity == identity)
                {
                    ids.Add(player.Userid);
                }
            }
            return ids;
        }
        /// <summary>
        /// 获取不同身份的玩家id
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public List<int> GetDifferentIdentiyUIds(int identity)
        {
            List<int> ids = new List<int>();
            foreach (var player in PlayerList)
            {
                if (player.MIdentity != identity)
                {
                    ids.Add(player.Userid);
                }
            }
            return ids;
        }
        /// <summary>
        /// 获取房间内第一个玩家的id
        /// </summary>
        /// <returns></returns>
        public int GetFirstUId()
        {
            return PlayerList[0].Userid;
        }

        private void sortCard(List<CardDto> cards,bool asc = true) //asc 升序 des 降序
        {
            cards.Sort(
                (CardDto a, CardDto b) =>
                {
                    if (asc)
                        return a.Weight.CompareTo(b.Weight);
                    else
                        return a.Weight.CompareTo(b.Weight) * -1;
                }
                );
        }
        /// <summary>
        /// 排序 默认是升序
        /// </summary>
        public void Sort(bool asc = true)
        {
            sortCard(PlayerList[0].CardList,asc);
            sortCard(PlayerList[1].CardList,asc);
            sortCard(PlayerList[2].CardList,asc);
            sortCard(TableCardList,asc);
        }
        /// <summary>
        /// 玩家是否掉线
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsOffline(int userId)
        {
            return LeaveUIdList.Contains(userId);
        }
        /// <summary>
        /// 初始化playerUidList
        /// </summary>
        /// <param name="uidList"></param>
        public void Init(List<int> uidList)
        {
            foreach (var userId in uidList)
            {
                PlayerDto player = new PlayerDto(userId);
                this.PlayerList.Add(player);
            }
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Cache.Fight
{
    /// <summary>
    /// 回合管理类
    /// </summary>
    public class RoundModel
    {
        /// <summary>
        /// 当前回合最大的出牌者
        /// </summary>
        public int BiggestUId { get; set; }
        /// <summary>
        /// 当前出牌者的id
        /// </summary>
        public int CurrentUId { get; set; }
        /// <summary>
        /// 上一次出牌的长度
        /// </summary>
        public int LastLength { get; set; }
        /// <summary>
        /// 上一次出牌的权值
        /// </summary>
        public int LastWeight { get; set; }
        /// <summary>
        /// 上一次出牌的类型
        /// </summary>
        public int LastCardType { get; set; }
        public RoundModel()
        {
            this.CurrentUId = -1;
            this.BiggestUId = -1;
            this.LastLength = -1;
            this.LastWeight = -1;
            this.LastCardType = -1;
        }
        public void Init()
        {
            this.CurrentUId = -1;
            this.BiggestUId = -1;
            this.LastLength = -1;
            this.LastWeight = -1;
            this.LastCardType = -1;
        }
        /// <summary>
        /// 开始出牌
        /// </summary>
        /// <param name="userId">要出牌的玩家id</param>
        public void Start(int userId)
        {
            this.CurrentUId = userId;
            this.BiggestUId = userId;
        }
        /// <summary>
        /// 改变出牌者
        /// </summary>
        /// <param name="length"></param>
        /// <param name="type"></param>
        /// <param name="weight"></param>
        /// <param name="userId"></param>
        public void Change(int length,int type,int weight,int userId)
        {
            this.BiggestUId = userId;
            this.LastLength = length;
            this.LastWeight = weight;
            this.LastCardType = type;
        }
        /// <summary>
        /// 转换出牌
        /// </summary>
        public void Turn(int userId)
        {
            this.CurrentUId = userId;
        }
    }
}

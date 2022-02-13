using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 房间数据对应的传输模型
    /// </summary>
    [Serializable]
    public class MatchRoomDto
    {
        /// <summary>
        /// 用户id对应的用户数据的传输模型
        /// </summary>
        public Dictionary<int, UserDto> UIdUserDict;
        /// <summary>
        /// 准备的用户id
        /// </summary>
        public List<int> ReadyUIdList;
        /// <summary>
        /// 按顺序进入的用户id 存储玩家的进入顺序
        /// </summary>
        private List<int> uIdList;

        public MatchRoomDto()
        {
            this.UIdUserDict = new Dictionary<int, UserDto>();
            this.ReadyUIdList = new List<int>();
            //fix bug
            this.uIdList = new List<int>();
        }
        public void AddUser(UserDto newUser)
        {
            UIdUserDict.Add(newUser.Id, newUser);
            uIdList.Add(newUser.Id);
        }
        public void Leave(int userId)
        {
            UIdUserDict.Remove(userId);
            uIdList.Remove(userId);
        }
        public void Ready(int userId)
        {
            ReadyUIdList.Add(userId);
        }

        public int LeftId;
        public int RightId;
        /// <summary>
        /// 重置位置
        ///     在每次玩家进入或离开房间的时候 都要调整一下位置
        /// </summary>
        /// <param name="myUserId"></param>
        public void ResetPostion(int myUserId)
        {
            LeftId = -1;
            RightId = -1;
            if(uIdList.Count <= 1)
            {
                return;
            }else if(uIdList.Count == 2)
            {
                if (uIdList[0] == myUserId)
                    RightId = uIdList[1];
                if (uIdList[1] == myUserId)
                    LeftId = uIdList[0];
            }else if(uIdList.Count == 3)
            {
                // x a b  x是自己自己下面看自己是右边先有那么这个就是右另一个是左   如果x左边有那么这个就是左 另一个就是右
                if (uIdList[0] == myUserId)
                {
                    LeftId = uIdList[2];
                    RightId = uIdList[1];
                }
                // a x b
                if (uIdList[1] == myUserId)
                {
                    LeftId = uIdList[0];
                    RightId = uIdList[2];
                }
                // a b x
                if (uIdList[2] == myUserId)
                {
                    LeftId = uIdList[1];
                    RightId = uIdList[0];
                }
            }
        }
    }
}

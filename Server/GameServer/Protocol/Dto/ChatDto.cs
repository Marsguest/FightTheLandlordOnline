using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 聊天的数据传输模型
    /// </summary>
    [Serializable]
    public class ChatDto
    {
        public int UserId;
        public int ChatType;

        public ChatDto()
        {

        }
        
        public ChatDto(int _userId,int _chatType)
        {
            this.UserId = _userId;
            this.ChatType = _chatType;
        }
    }
}

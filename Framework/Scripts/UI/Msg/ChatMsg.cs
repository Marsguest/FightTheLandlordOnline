using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.UI.Msg
{
    public class ChatMsg
    {
        public int UserId;
        public int ChatType;
        public string Text;

        public ChatMsg()
        {

        }

        public ChatMsg(int _userId,int _chatType,string _txt)
        {
            this.UserId = _userId;
            this.ChatType = _chatType;
            this.Text = _txt;
        }
        public void Change(int _userId, int _chatType, string _txt)
        {
            this.UserId = _userId;
            this.ChatType = _chatType;
            this.Text = _txt;
        }
    }
}

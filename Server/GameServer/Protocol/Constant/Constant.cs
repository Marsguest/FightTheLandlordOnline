using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Constant
{
    public class Constant
    {
        private static Dictionary<int, string> chatTypeTextDict;

        static Constant()
        {
            chatTypeTextDict = new Dictionary<int, string>();

            chatTypeTextDict.Add(1, "大家好，很高兴见到各位");
            chatTypeTextDict.Add(2, "和你合作真实太愉快啦！");
            chatTypeTextDict.Add(3, "快点吧，我等的花儿都谢了");
            chatTypeTextDict.Add(4, "你的牌打的也太好了！");
            chatTypeTextDict.Add(5, "不要吵了，有什么好吵的\n专心玩游戏吧！");
            chatTypeTextDict.Add(6, "不要走，决战到天亮！");
            chatTypeTextDict.Add(7, "再见了，我会想念大家的~");
        }

        public static string GetChatText(int chatType)
        {
            return chatTypeTextDict[chatType];
        }
    }
}

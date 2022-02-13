using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    [Serializable]
    public class UserDto
    {
        public int Id;
        public string Name; //角色名字
        public int Bean; //豆子的数量

        public int WinCount;//胜场
        public int LoseCount;//负场
        public int RunCount;//逃跑场

        public int Lv;//等级
        public int Exp;//经验

        public UserDto()
        {

        }
        public UserDto(int _id, string _name,int _bean,int _winCount,int _lostCount,int _runCount,int _lv,int _Exp)
        {
            this.Id = _id;
            this.Name = _name;
            this.Bean = _bean;
            this.WinCount = _winCount;
            this.LoseCount = _lostCount;
            this.RunCount = _runCount;
            this.Lv = _lv;
            this.Exp = _Exp;
        }
    }
}

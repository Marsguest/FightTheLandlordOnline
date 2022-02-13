using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto.Fight
{
    /// <summary>
    /// 卡牌
    /// </summary>
    [Serializable]
    public class CardDto
    {
        public int Id;
        public string Name;
        public int Color; //红桃
        public int Weight; //9

        public CardDto()
        {

        }

        public CardDto(int _id,string _name,int _color,int _weight)
        {
            this.Id = _id;
            this.Name = _name;
            this.Color = _color;
            this.Weight = _weight;
        }
    }
}

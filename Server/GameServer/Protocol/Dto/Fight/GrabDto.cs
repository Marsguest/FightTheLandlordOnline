using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto.Fight
{
    [Serializable]
    public class GrabDto
    {
        public int UserId;
        public List<CardDto> TableCardList;
        public List<CardDto> PlayerCardList;

        public GrabDto()
        {

        }

        public GrabDto(int userId, List<CardDto> tableCardList, List<CardDto> playerCardList)
        {
            this.UserId = userId;
            this.TableCardList = tableCardList;
            this.PlayerCardList = playerCardList;
        }
    }
}

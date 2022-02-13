using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto.Fight
{
    [Serializable]
    public class OverDto
    {
        public int WinIdentity;
        public List<int> WinUIdList;
        public int BeanCount;
        public OverDto()
        {

        }
    }
}

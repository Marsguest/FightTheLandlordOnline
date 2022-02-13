using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Cache.Fight;
using GameServer.Cache.Match;

namespace GameServer.Cache
{
    public class Caches
    {
        public static AccountCache Account { get; set; }
        public static UserCache User { get; set; }

        public static MatchCache Match { get; set; }

        public static FightCache Fight { get; set; }

        static Caches()
        {
            Account = new AccountCache();
            User = new UserCache();
            Match = new MatchCache();
            Fight = new FightCache();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_EX1_162 : SimTemplate //* 恐狼前锋 Dire Wolf Alpha
	{
		//Adjacent minions have +1_Attack.
		//相邻的随从获得+1攻击力。

        //    benachbarte diener haben +1 angriff.
        // note buff and debuff is handled by playfield (faster)
        // Handled in updateBoards()
        /*
        public override void onAuraStarts(Playfield p, Minion own)
        {
            if (own.own)
            {
                foreach (Minion m in p.ownMinions)
                {
                    if (m.zonepos-1 == own.zonepos || m.zonepos + 1 == own.zonepos)
                    {
                        p.minionGetAdjacentBuff(m, 1, 0);
                    }
                }
            }
            else
            {
                foreach (Minion m in p.enemyMinions)
                {
                    if (m.zonepos-1 == own.zonepos || m.zonepos + 1 == own.zonepos)
                    {
                        p.minionGetAdjacentBuff(m, 1, 0);
                    }
                }
            }

		}


        public override void onAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                foreach (Minion m in p.ownMinions)
                {
                    if (m.zonepos - 1 == own.zonepos || m.zonepos + 1 == own.zonepos)
                    {
                        p.minionGetAdjacentBuff(m, -1, 0);
                    }
                }
            }
            else
            {
                foreach (Minion m in p.enemyMinions)
                {
                    if (m.zonepos - 1 == own.zonepos || m.zonepos + 1 == own.zonepos)
                    {
                        p.minionGetAdjacentBuff(m, -1, 0);
                    }
                }
            }
        }*/

	}
}
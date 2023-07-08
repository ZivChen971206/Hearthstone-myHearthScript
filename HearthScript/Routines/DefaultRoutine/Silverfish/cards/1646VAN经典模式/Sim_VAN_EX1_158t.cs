using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_VAN_EX1_158t : SimTemplate //* 树人 Treant
	{
		//
		//
		public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            p.minionGetDestroyed(triggerEffectMinion);    
        }
	}
}

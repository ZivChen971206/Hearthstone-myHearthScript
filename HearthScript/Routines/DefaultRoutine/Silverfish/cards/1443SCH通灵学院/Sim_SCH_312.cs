using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_SCH_312 : SimTemplate //* 巡游向导 Tour Guide
	{
		//<b>Battlecry:</b> Your next Hero Power costs (0).
		//<b>战吼：</b>你的下一个英雄技能的法力值消耗为（0）点。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (p.ownHeroPowerCostLessOnce <= -99) p.evaluatePenality += 50;
			p.ownHeroPowerCostLessOnce -= 99;
		}		
		
	}
}

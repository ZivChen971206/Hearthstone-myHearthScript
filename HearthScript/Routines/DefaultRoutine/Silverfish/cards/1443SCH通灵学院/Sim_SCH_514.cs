using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_SCH_514 : SimTemplate //* 亡者复生 Raise Dead
	{
		//Deal $3 damage to your hero. Return two friendly minions that died this game to your hand.
		//对你的英雄造成$3点伤害。将两个在本局对战中死亡的友方随从移回你的手牌。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			if (p.ownQuest.maxProgress == 8 && p.ownQuest.questProgress >= 5 && p.owncards.Count >= 9) p.evaluatePenality += 1000;
			int rebornCard = 2;
			p.minionGetDamageOrHeal(ownplay ? p.ownHero : p.enemyHero, 3);
			foreach (KeyValuePair<CardDB.cardIDEnum, int> e in p.ownGraveyard)
            {
                // 死亡随从
                CardDB.Card rebornMob = CardDB.Instance.getCardDataFromID(e.Key);
                // 不是已死亡随从退出
                if( rebornMob.type != CardDB.cardtype.MOB) continue;

                rebornCard -= e.Value;
				if(rebornCard < 0) break;
				p.drawACard(e.Key, ownplay, true);
				if(e.Value > 1){
					p.drawACard(e.Key, ownplay, true);
				}
            }
			if(rebornCard == 2 ) p.evaluatePenality += 1000;
			else if ( rebornCard == 1 ){
				p.evaluatePenality += 20;
				foreach(Minion m in p.ownMinions)
                {
					if (m.Ready) p.evaluatePenality += 50;
                }
			}else
            {
				p.evaluatePenality -= 10;
			}
			if(!p.anzTamsin && p.owncards.Count + 2 - rebornCard > 10) p.evaluatePenality += 1000;
		}
		
	}
}

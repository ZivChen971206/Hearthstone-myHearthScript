using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_NAX15_02 : SimTemplate //* 冰霜冲击 Frost Blast
//<b>Hero Power</b>Deal 2 damage to the enemy hero and <b>Freeze</b> it.
//<b>英雄技能</b>对敌方英雄造成2点伤害，并使其<b>冻结</b>。 
	{
        

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getHeroPowerDamage(2) : p.getEnemyHeroPowerDamage(2);
            p.minionGetDamageOrHeal(ownplay ? p.enemyHero : p.ownHero, dmg);
            p.minionGetFrozen(target);
        }
	}
}
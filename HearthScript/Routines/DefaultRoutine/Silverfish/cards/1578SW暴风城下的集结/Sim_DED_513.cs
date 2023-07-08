using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DED_513 : SimTemplate //* 迪菲亚麻风侏儒 Defias Leper
    {
        //战吼：如果你的手牌中有暗影法术牌，则造成2点伤害。
        //Battlecry: If you're holding a Shadow spell, deal 2 damage.
        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (!target.own)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.SpellSchool == CardDB.SpellSchool.SHADOW && hc.card.type == CardDB.cardtype.SPELL)
                    {
                        p.minionGetDamageOrHeal(target, 2, true);
                    }
                }
            }
        }
    }
}

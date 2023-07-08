using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_REV_841 : SimTemplate //* 匿名线人 anonymousinformant
//[x]<b>Battlecry:</b> The next <b>Secret</b>you play costs (0).
//<b>战吼：</b>你使用的下一个<b>奥秘</b>的法力值消耗为（0）点。 
	{
        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (m.own) p.nextSecretThisTurnCost0 = true;
        }
    }
}

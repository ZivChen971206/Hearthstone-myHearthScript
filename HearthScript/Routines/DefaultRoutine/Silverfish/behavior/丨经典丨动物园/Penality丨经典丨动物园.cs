using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HREngine.Bots
{
    //每个策略的 Penality{策略名}文件里面放 三个函数：打牌评分，随从进攻评分；英雄进攻评分 以及他们需要用到的private函数
    //这三个函数用于单动作评估，如果返回值超过500，则被剪枝，不列入候选动作
    public partial class Behavior丨经典丨动物园 : Behavior
    {
        public override int getPlayCardPenality(CardDB.Card card, Minion target, Playfield p, Handmanager.Handcard nowHandcard)
        {
            switch (card.nameCN)
            {
                case CardDB.cardNameCN.力量的代价:
                    if (target.own && !target.Ready && target.handcard.card.nameCN != CardDB.cardNameCN.麻风侏儒) return 1000;
                    break;
                case CardDB.cardNameCN.黑铁矮人:
                    break;
                case CardDB.cardNameCN.精灵弓箭手:
                    if (target.own) return 1000;
                    break;
                case CardDB.cardNameCN.叫嚣的中士:
                    if (p.ownMinions.Count > 0 && target != null && !target.own) return 1000;
                    break;
                case CardDB.cardNameCN.灵魂之火:
                    if (target.own) return 1000;
                    if (target.isHero && ( (p.ownMaxMana < 4 || target.Hp + target.armor > 20) && p.owncards.Count > 2) ) return 1000;
                    break;
                case CardDB.cardNameCN.死亡缠绕:
                    if (target.own && target.handcard.card.nameCN != CardDB.cardNameCN.麻风侏儒) return 1000;
                    break;
            }
            return getComboPenality(card, target, p, nowHandcard);
        }

        public override int getAttackWithMininonPenality(Minion m, Playfield p, Minion target)
        {
            if (!m.silenced && m.handcard.card.CantAttack || target.untouchable)
                return 1000;
            int pen = -10;

            return pen;
        }

        //英雄攻击惩罚
        public override int getAttackWithHeroPenality(Minion target, Playfield p) // 奥秘法没有英雄带刀进攻
        {
            if (target.untouchable)
                return 1000;
            int pen = 0;
            return pen;
        }

        private int getSecretPenality(Playfield p)  // 牌序和防奥秘的影响
        {
            // 弃牌前是不抽牌的
            int discard = -10;
            for (int i = 0; i < p.playactions.Count; i++)
            {
                Action a = p.playactions[i];
                if (a.actionType == actionEnum.playcard && (a.card.card.nameCN == CardDB.cardNameCN.末日守卫 || a.card.card.nameCN == CardDB.cardNameCN.灵魂之火 )) // 随从攻击敌方英雄
                {
                    discard = i;
                    break;
                }
            }
            if(discard >= 0)
            {
                for (int i = 0; i < discard; i++)
                {
                    Action a = p.playactions[i];
                    if (a.actionType == actionEnum.useHeroPower) 
                    {
                        return -15000;
                    }
                }
            }

            // 飞刀临死前想要再飞一刀
            int knifeDied = 1000;
            for (int i = 0; i < p.playactions.Count; i++)
            {
                Action a = p.playactions[i];
                if (a.actionType == actionEnum.attackWithMinion && a.own.handcard.card.nameCN == CardDB.cardNameCN.飞刀杂耍者 && a.own.Hp <= 0) // 随从攻击敌方英雄
                {
                    knifeDied = i;
                    break;
                }
            }
            for(int i = knifeDied; i < p.playactions.Count; i++)
            {
                Action a = p.playactions[i];
                if (a.actionType == actionEnum.playcard && a.card.card.type == CardDB.cardtype.MOB)
                {
                    return -10000;
                }
            }


            if (p.enemySecretCount == 0)
                return 0;
           
            int pen = 0;

            bool canBe_explosive = false;  //防止是猎人的爆炸陷阱
            foreach (SecretItem si in p.enemySecretList)
            {
                if (si.canBe_explosive) { canBe_explosive = true; break; }
            }
            if (canBe_explosive)
            {
                int first_attack_hero = -2;
                for (int i = 0; i < p.playactions.Count; i++)
                {
                    Action a = p.playactions[i];
                    if ((a.actionType == actionEnum.attackWithMinion || a.actionType == actionEnum.attackWithHero) && a.target.isHero) // 随从攻击敌方英雄
                    {
                        first_attack_hero = i;
                        if (p.ownHero.Hp <= 2)
                            pen -= 500;
                        break;
                    }
                    // Todo: 这里还要考虑法术伤害敌方英雄 待Fix
                }
                if (first_attack_hero >= 0) // 存在随从攻击英雄
                {
                    //如果此前出牌了，扣分，容易被炸
                    for (int i = 0; i < first_attack_hero; i++)
                    {
                        Action a = p.playactions[i];
                        if (a.actionType == actionEnum.playcard)
                        {
                            if (a.card.card.type == CardDB.cardtype.MOB) //出了随从
                                return -15000; // 不可接受，抛弃本牌面以及子牌面
                        }
                    }
                    for (int i = first_attack_hero + 1; i < p.playactions.Count; i++)
                    {
                        Action a = p.playactions[i];
                        if (a.actionType == actionEnum.attackWithMinion && !a.target.isHero && a.own.Hp < 3 && !a.own.divineshild)
                        {
                            return -15000; // 不可接受，抛弃本牌面以及子牌面
                        }
                    }
                    // 尽量少留 2 血以下生物
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Hp < 3 && !m.divineshild)
                        {
                            pen -= m.Angr * 6;
                        }
                    }
                }
            }
            return pen;
        }
    }
}

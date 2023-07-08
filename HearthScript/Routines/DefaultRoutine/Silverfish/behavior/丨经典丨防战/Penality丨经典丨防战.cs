using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HREngine.Bots
{
    //每个策略的 Penality{策略名}文件里面放 三个函数：打牌评分，随从进攻评分；英雄进攻评分 以及他们需要用到的private函数
    //这三个函数用于单动作评估，如果返回值超过500，则被剪枝，不列入候选动作
    public partial class Behavior丨经典丨防战 : Behavior
    {
        public override int getPlayCardPenality(CardDB.Card card, Minion target, Playfield p, Handmanager.Handcard nowHandcard)
        {
            switch (card.nameCN)
            {
                case CardDB.cardNameCN.绝命乱斗:
                    if(p.enemyMinions.Count == 0 )return 1000;
                    break;
                case CardDB.cardNameCN.严酷的监工:
                case CardDB.cardNameCN.怒火中烧:
                    if (target != null && target.own && target.Hp <= 1 + p.spellpower && !target.divineshild) return 1000;
                    if (target != null && target.own && target.handcard.card.nameCN == CardDB.cardNameCN.炎魔之王拉格纳罗斯) return 1000;
                    if (target != null && target.handcard.card.nameCN == CardDB.cardNameCN.末日预言者 && p.ownMinions.Count == 0) return 1000;
                    break;
                case CardDB.cardNameCN.盾牌猛击:
                    if( target != null && target.own && !(target.handcard.card.nameCN == CardDB.cardNameCN.格罗玛什地狱咆哮 && p.ownHero.armor + p.spellpower < 10 && target.Hp == target.maxHp ) )
                        return 1000;
                    if(p.ownHero.armor == 0) return 1000;
                    if (target != null && target.handcard.card.nameCN == CardDB.cardNameCN.末日预言者 && p.ownMinions.Count == 0) return 1000;
                    break;
                case CardDB.cardNameCN.顺劈斩:
                    if (p.enemyMinions.Count < 2) return 1000;
                    break;
                case CardDB.cardNameCN.猛击:
                case CardDB.cardNameCN.斩杀:
                    if( target != null &&target.own && target.handcard.card.nameCN != CardDB.cardNameCN.格罗玛什地狱咆哮)
                        return 1000;
                    if (target != null && target.handcard.card.nameCN == CardDB.cardNameCN.末日预言者 && p.ownMinions.Count == 0) return 1000;
                    break;
                case CardDB.cardNameCN.黑骑士:
                    if (target != null && target.own) return 1000;
                    break;
                case CardDB.cardNameCN.王牌猎人:
                    if (target != null && target.own) return 1000;
                    break;
            }
            return getComboPenality(card, target, p, nowHandcard);
        }

        public override int getAttackWithMininonPenality(Minion m, Playfield p, Minion target)
        {
            if (!m.silenced && m.handcard.card.CantAttack || target.untouchable)
                return 1000;
            // 保留，别送
            int pen = 0;
            if (m.isHero || target.isHero) return pen;
            bool setGeddon = false;
            foreach (Minion mm in p.ownMinions)
            {
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.迦顿男爵)
                {
                    setGeddon = true;
                    break;
                }
            }
            if (setGeddon && target.Hp <= 2 && !target.divineshild)
            {
                return 1000;
            }
            return pen;
        }

        //英雄攻击惩罚
        public override int getAttackWithHeroPenality(Minion target, Playfield p) // 奥秘法没有英雄带刀进攻
        {
            int pen = 0;
            bool setGeddon = false;
            foreach (Minion mm in p.ownMinions)
            {
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.迦顿男爵)
                {
                    setGeddon = true;
                    break;
                }
            }
            if (setGeddon && target.Hp <= 2 && !target.divineshild)
            {
                return 1000;
            }
            // 谁莫名其妙给我设置了 16 点杀怪奖励？
            //if (target.Hp <= p.ownHero.Angr && !target.divineshild)
            //{
            //    pen += 16;
            //}
            //// TMD 不知道在哪里写了个溢出伤害超过2给了20点惩罚，修正
            //if (p.ownHero.Angr >= target.Hp + 2)
            //{
            //    pen -= 20;
            //}
            //pen += target.Angr * 3;
            return pen;
        }

        private int getSecretPenality(Playfield p)  // 牌序和防奥秘的影响
        {
            // 嘉顿男爵在场不应该再解对手的低血怪了

            //int playDuel = -1;
            //// 乱斗第一张出
            //for (int i = 0; i < p.playactions.Count; i++)
            //{
            //    Action a = p.playactions[i];
            //    if (a.actionType == actionEnum.playcard && a.card.card.chnName == CardDB.cardNameCN.绝命乱斗)
            //    {
            //        playDuel = i;
            //    }
            //}
            //if(playDuel > -1)
            //{
            //    // 乱斗之前不攻击随从，不出牌
            //    for (int i = 0; i < playDuel; i++)
            //    {
            //        Action a = p.playactions[i];
            //        if (a.actionType == actionEnum.playcard)
            //        {
            //            return -15000;
            //        }
            //        if ((a.actionType == actionEnum.attackWithHero || a.actionType == actionEnum.attackWithMinion) && (!a.target.isHero && !a.target.taunt))
            //        {
            //            return -15000;
            //        }

            //    }

            //    // 乱斗之后不攻击
            //    for (int i = playDuel; i < p.playactions.Count; i++)
            //    {
            //        Action a = p.playactions[i];
            //        if (a.actionType == actionEnum.attackWithMinion)
            //        {
            //            return -15000;
            //        }

            //    }
            //}
            
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
                int first_attack_hero = -1;
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
                }

            }
            return pen;
        }

    }
}

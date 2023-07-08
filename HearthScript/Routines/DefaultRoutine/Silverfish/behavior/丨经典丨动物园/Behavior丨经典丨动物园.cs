using System.Collections.Generic;
using System;

namespace HREngine.Bots
{
    public partial class Behavior丨经典丨动物园 : Behavior
    {
        private int bonus_enemy = 4;
        private int bonus_mine = 4;

        public override string BehaviorName() { return "丨经典丨动物园"; }
        PenalityManager penman = PenalityManager.Instance;
        // 核心，场面值
        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value > -200000) return p.value;
            float retval = 0;

            // 出牌序列数量
            int count = p.playactions.Count;
            int ownActCount = 0;

            bool foundKnife = false;
            foreach (Minion mm in p.ownMinions)
            {
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.飞刀杂耍者)
                {
                    foundKnife = true;
                }
            }

            // 排序问题！！！！
            for (int i = 0; i < count; i++)
            {
                Action a = p.playactions[i];
                ownActCount++;
                switch (a.actionType)
                {
                    // 英雄攻击
                    case actionEnum.attackWithHero:
                        continue;
                    case actionEnum.useHeroPower:
                    case actionEnum.playcard:
                        break;
                    default:
                        continue;
                }
                if (foundKnife && a.actionType == actionEnum.playcard && a.card.card.type == CardDB.cardtype.MOB)
                {
                    retval += 5 - i;
                }
                switch (a.card.card.nameCN)
                {
                    // 排序优先
                    case CardDB.cardNameCN.死亡缠绕:
                        if(!foundKnife)
                        retval -= i * 7 - 7;
                        break;
                    case CardDB.cardNameCN.生命分流:
                        retval -= i * 3;
                        break;
                    case CardDB.cardNameCN.恐狼前锋:
                        retval -= i;
                        break;
                    case CardDB.cardNameCN.力量的代价:
                    case CardDB.cardNameCN.飞刀杂耍者:
                        retval -= i * 2;
                        break;
                    case CardDB.cardNameCN.末日守卫:
                    case CardDB.cardNameCN.灵魂之火:
                    case CardDB.cardNameCN.阿古斯防御者:
                        retval += i;
                        break;
                }
            }

            retval -= p.enemyWeapon.Angr * bonus_enemy + p.enemyWeapon.Durability * bonus_enemy;
            retval += getGeneralVal(p);
            retval += p.owncards.Count * bonus_mine;
            // 危险血线
            int hpboarder = 7;
            // 抢脸血线
            int aggroboarder = 20;
            retval += getHpValue(p, hpboarder, aggroboarder);

            // 对手基本随从交换模拟
            retval += enemyTurnPen(p);
            retval -= p.lostDamage;

            retval += getSecretPenality(p); // 奥秘的影响
            //p.value = retval;
            return retval;
        }


        // 敌方随从价值 主要等于 （HP + Angr） * 4  
        public override int getEnemyMinionValue(Minion m, Playfield p)
        {
            bool dieNextTurn = false;
            foreach (Minion mm in p.enemyMinions)
            {
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.末日预言者)
                {
                    dieNextTurn = true;
                    break;
                }
            }
            if (m.destroyOnEnemyTurnEnd || m.destroyOnEnemyTurnStart || m.destroyOnOwnTurnEnd || m.destroyOnOwnTurnStart) dieNextTurn = true;
            if (dieNextTurn)
            {
                return -1;
            }
            if (m.Hp <= 0) return 0;
            int retval = 0;
            if (p.ownHero.Hp < 10) retval += bonus_mine * 3;
            if (m.Angr > 0  || p.enemyHeroStartClass == TAG_CLASS.PRIEST)
                retval += m.Hp * bonus_enemy;
            retval += m.spellpower * bonus_enemy * 3 / 2;
            if (!m.frozen && !m.cantAttack )
            {
                retval += m.Angr * bonus_enemy;
                if (m.windfury) retval += m.Angr * bonus_enemy / 2;
            }
            if (m.silenced) return retval;

            if (m.taunt) retval += 2;
            if (m.divineshild) retval += m.Angr * 2;
            if (m.divineshild && m.taunt) retval += 5;
            if (m.stealth) retval += 2;

            // 剧毒价值两点属性
            if (m.poisonous)
            {
                retval += bonus_enemy * 2;
            }
            if (m.lifesteal) retval += m.Angr * bonus_enemy / 2;

            switch (m.handcard.card.nameCN)
            {
                //case CardDB.cardNameCN.凯恩血蹄:
                   //retval += bonus_enemy * 5;
                    //break; 动物园解个锤子牛头人
                // 异能价值每多 1 属性值加权 4 点
                // 异能价值 4 属性值
                case CardDB.cardNameCN.暴乱狂战士:
                case CardDB.cardNameCN.加基森拍卖师:
                case CardDB.cardNameCN.任务达人:
                case CardDB.cardNameCN.狂野炎术师:
                //case CardDB.cardNameCN.苔原犀牛:
                    retval += bonus_enemy * 5;
                    break;
                // 异能价值 3 属性
                case CardDB.cardNameCN.恐狼前锋:
                case CardDB.cardNameCN.迦顿男爵:
                case CardDB.cardNameCN.法力之潮图腾:
                case CardDB.cardNameCN.火舌图腾:
                case CardDB.cardNameCN.雷欧克:
                case CardDB.cardNameCN.北郡牧师:
                    retval += bonus_enemy * 3;
                    break;
                // 异能价值 2 属性
                case CardDB.cardNameCN.巫师学徒:
                case CardDB.cardNameCN.飞刀杂耍者:
                case CardDB.cardNameCN.大法师安东尼达斯:
                case CardDB.cardNameCN.苦痛侍僧:
                case CardDB.cardNameCN.奥金尼灵魂祭司:
                case CardDB.cardNameCN.暴风城勇士:
                case CardDB.cardNameCN.年轻的女祭司:
                case CardDB.cardNameCN.森林狼:
                case CardDB.cardNameCN.食腐土狼:
                case CardDB.cardNameCN.法力浮龙:
                case CardDB.cardNameCN.饥饿的秃鹫:
                case CardDB.cardNameCN.铸甲师:
                case CardDB.cardNameCN.先知维伦:
                    retval += bonus_enemy * 2;
                    break;
                //case CardDB.cardNameCN.炎魔之王拉格纳罗斯:
                //    retval += bonus_enemy * 8;
                //    break;
                case CardDB.cardNameCN.末日预言者:
                    //foreach(Minion mm in p.ownMinions)
                    //{
                    //    retval += mm.Hp + mm.Angr;
                    //}
                    //if(p.ownMinions.Count == 0)
                    //{
                    //    retval = -20;
                    //}
                    break;

                // 不解
                case CardDB.cardNameCN.希尔瓦娜斯风行者:
                case CardDB.cardNameCN.凯恩血蹄:
                case CardDB.cardNameCN.炎魔之王拉格纳罗斯:
                        retval -= 10;
                    break;
            }

            //对面术士专用敌方随从价值 打内战解就完事了
            if (p.enemyHeroStartClass == TAG_CLASS.WARLOCK)
            {
                switch (m.handcard.card.nameCN){
                // 异能价值每多 1 属性值加权 4 点
                // 异能价值 4 属性值 最高优先
                case CardDB.cardNameCN.飞刀杂耍者:
                case CardDB.cardNameCN.恐狼前锋:
                    retval += bonus_enemy * 4;
                    break;
                // 异能价值 2 属性  带特效有威胁中等优先
                case CardDB.cardNameCN.年轻的女祭司:
                case CardDB.cardNameCN.银色侍从:
                case CardDB.cardNameCN.持盾卫士:
                case CardDB.cardNameCN.阿曼尼狂战士:
                case CardDB.cardNameCN.团队领袖:
                case CardDB.cardNameCN.血色十字军战士:
                case CardDB.cardNameCN.麦田傀儡:
                case CardDB.cardNameCN.银色指挥官:
                        retval += bonus_enemy * 2;
                    break;
                // 当作白板给个基础值
                case CardDB.cardNameCN.虚空行者:
                case CardDB.cardNameCN.烈焰小鬼:
                case CardDB.cardNameCN.末日守卫:
                case CardDB.cardNameCN.精灵弓箭手:
                case CardDB.cardNameCN.叫嚣的中士:
                case CardDB.cardNameCN.狼人渗透者:
                case CardDB.cardNameCN.麻风侏儒:
                case CardDB.cardNameCN.铁喙猫头鹰:
                case CardDB.cardNameCN.破碎残阳祭司:
                case CardDB.cardNameCN.黑铁矮人:
                case CardDB.cardNameCN.破法者:
                case CardDB.cardNameCN.阿古斯防御者:
                retval += bonus_enemy;
                    break;
                }
            }
            return retval;


        }

        // 我方随从价值 主要等于 （HP + Angr） * 4  + 5
        public override int getMyMinionValue(Minion m, Playfield p)
        {
            bool dieNextTurn = false;
            foreach (Minion mm in p.enemyMinions)
            {
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.末日预言者)
                {
                    dieNextTurn = true;
                    break;
                }
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.迦顿男爵 && m.Hp <= 2 && !m.divineshild)
                {
                    dieNextTurn = true;
                    break;
                }
            }
            if (m.destroyOnEnemyTurnEnd || m.destroyOnEnemyTurnStart || m.destroyOnOwnTurnEnd || m.destroyOnOwnTurnStart) dieNextTurn = true;
            if (dieNextTurn)
            {
                return -1;
            }
            int retval = 5;
            if (m.Hp <= 0) return 0;
            retval += m.Hp * bonus_mine;
            retval += m.Angr * bonus_mine;
            if (m.Hp <= 1 && !m.divineshild) retval -= (m.Angr - 1) * (bonus_mine - 1);
            // 高攻低血是垃圾
            if (m.Angr > m.Hp + 4) retval -= (m.Angr - m.Hp) * (bonus_mine - 1);
            // 风怒价值
            if ((!m.playedThisTurn || m.rush == 1 || m.charge == 1) && m.windfury) retval += m.Angr;
            // 圣盾价值
            if (m.divineshild) retval += m.Angr * 3;
            // 潜行价值
            if (m.stealth) retval += m.Angr / 2 + 1;
            // 吸血
            if (m.lifesteal) retval += m.Angr / 2 + 1;
            // 圣盾嘲讽
            if (m.divineshild && m.taunt) retval += 4;


            //经典动物园专用自己的随从价值
            switch (m.handcard.card.nameCN)
            {
                // 异能价值 4 属性  最高优先
                case CardDB.cardNameCN.飞刀杂耍者:
                case CardDB.cardNameCN.恐狼前锋:
                    retval += bonus_mine * 4;
                    break;
                // 异能价值 3 属性  卡组唯一白板带价值的
                case CardDB.cardNameCN.阿曼尼狂战士:
                    retval += bonus_mine * 3;
                    break;
                // 马云和傀儡降低权重
                case CardDB.cardNameCN.麦田傀儡:
                case CardDB.cardNameCN.麻风侏儒:
                    retval += bonus_mine / 2;
                    break;
               //自己场上大于1个怪才提高2倍价值 
                case CardDB.cardNameCN.年轻的女祭司:
                    if(p.ownMinions.Count > 1)
                        retval += bonus_mine * 2;
                    break;
            }
            return retval;
        }

        public override int getSirFinleyPriority(List<Handmanager.Handcard> discoverCards)
        {
            
            return -1; //comment out or remove this to set manual priority
            int sirFinleyChoice = -1;
            int tmp = int.MinValue;
            for (int i = 0; i < discoverCards.Count; i++)
            {
                CardDB.cardNameEN name = discoverCards[i].card.nameEN;
                if (SirFinleyPriorityList.ContainsKey(name) && SirFinleyPriorityList[name] > tmp)
                {
                    tmp = SirFinleyPriorityList[name];
                    sirFinleyChoice = i;
                }
            }
            return sirFinleyChoice;
        }
        public override int getSirFinleyPriority(CardDB.Card card)
        {
            return SirFinleyPriorityList[card.nameEN];
        }

        private Dictionary<CardDB.cardNameEN, int> SirFinleyPriorityList = new Dictionary<CardDB.cardNameEN, int>
        {
            //{HeroPowerName, Priority}, where 0-9 = manual priority
            { CardDB.cardNameEN.lesserheal, 0 }, 
            { CardDB.cardNameEN.shapeshift, 6 },
            { CardDB.cardNameEN.fireblast, 7 },
            { CardDB.cardNameEN.totemiccall, 1 },
            { CardDB.cardNameEN.lifetap, 9 },
            { CardDB.cardNameEN.daggermastery, 5 },
            { CardDB.cardNameEN.reinforce, 4 },
            { CardDB.cardNameEN.armorup, 2 },
            { CardDB.cardNameEN.steadyshot, 8 }
        };

        public override int getComboPenality(CardDB.Card card, Minion target, Playfield p, Handmanager.Handcard nowHandcard)
        {
            // 无法选中
            if (target != null && target.untouchable)
            {
                return 100000;
            }
            // 初始惩罚值
            int pen = 0;
            pen += penman.getSilencePenality(card.nameEN, target, p);
            switch (card.nameCN)
            {
                case CardDB.cardNameCN.幸运币:
                    break;
                case CardDB.cardNameCN.麦田傀儡:
                    break;
                case CardDB.cardNameCN.死亡缠绕:
                    pen += bonus_enemy * 2;
                    break;
                case CardDB.cardNameCN.年轻的女祭司:
                    if( p.ownMinions.Count > 0 ) pen -= 20;
                    if( p.ownMinions.Count < 1 && p.ownMaxMana < 2) pen += 2;
                    break;
                case CardDB.cardNameCN.血色十字军战士:
                    break;
                case CardDB.cardNameCN.破碎残阳祭司:
                    //if (target != null) pen -= 5;
                    break;
                case CardDB.cardNameCN.飞刀杂耍者:   
                    if( p.ownMaxMana < 3 && p.enemyMinions.Count == 0 ) pen -= 50;
                    // 白送
                    //else if(p.ownMinions.Count == 0 && p.enemyMinions.Count > 0 && p.mana == 2) pen += 10;
                    break;
                case CardDB.cardNameCN.末日守卫:
                    if (p.owncards.Count > 2)
                        pen += bonus_mine * 6;
                    else if (p.owncards.Count > 1)
                        pen += bonus_mine * 2;
                    break; 
                case CardDB.cardNameCN.灵魂之火:
                    if (p.enemyHero.Hp <= 4 && !target.isHero) return 1000;
                    int cnt = 0;
                    foreach(Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.card.nameCN == CardDB.cardNameCN.灵魂之火) cnt++;
                    }
                    if (p.owncards.Count > 1 && cnt < 2)
                        pen += bonus_mine * 2;
                    else if (cnt > 1)
                        pen -= bonus_mine * 6;
                    else if (p.owncards.Count <= 1)
                        pen -= bonus_mine * 2;
                    break; 
                case CardDB.cardNameCN.阿古斯防御者:
                    //if( p.ownMinions.Count > 1) pen -= 5;
                    //if (p.ownMinions.Count > 2) pen -= 2;
                    break;
                case CardDB.cardNameCN.叫嚣的中士:
                    if (target == null || !target.Ready || !target.own) pen += bonus_enemy * 1;
                    break;
                case CardDB.cardNameCN.黑铁矮人:
                    if (target == null || !target.Ready || !target.own) pen += bonus_enemy * 1;
                    break;
                case CardDB.cardNameCN.生命分流:
                    //pen -= bonus_mine * 1;
                    //pen += 8;
                    //if (p.enemyHero.Hp <= 5) pen -= 50;
                    //if (p.owncards.Count < 3 && p.mana == p.ownMaxMana) pen -= 10;
                    break;
                case CardDB.cardNameCN.持盾卫士:
                    break;
                case CardDB.cardNameCN.银色侍从:
                    break;
                case CardDB.cardNameCN.力量的代价:
                    break;
                case CardDB.cardNameCN.麻风侏儒:
                    break;
                case CardDB.cardNameCN.烈焰小鬼:
                    break;
                    // 亡语补正
            }
            return pen;
        }

        public override int getHpValue(Playfield p, int hpboarder, int aggroboarder)
        {
            bool canAttackHero = false;
            if (p.anzEnemyTaunt == 0 && !p.enemyHero.untouchable)
            {
                canAttackHero = true;
            }
            int offsetMana = 0;
            if (p.anzEnemyTaunt == 1)
            {
                int tauntHp = 0;
                foreach (Minion m in p.enemyMinions)
                {
                    if (m.taunt) tauntHp += m.Hp;
                }
                foreach (Minion m in p.ownMinions)
                {
                    if (m.Ready && m.playedThisTurn && m.handcard.card.Rush) tauntHp -= m.Angr;
                }
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.Rush && tauntHp > 0 && p.mana + offsetMana >= hc.manacost)
                    {
                        offsetMana -= hc.manacost;
                        tauntHp -= hc.addattack + hc.card.Attack;
                    }
                }
                if (tauntHp <= 0) canAttackHero = true;
            }
            int offset_enemy = 0;
            if (canAttackHero && p.mana + offsetMana >= 0)
            {
                foreach (Minion m in p.ownMinions)
                {
                    if (m.Ready && !m.cantAttackHeroes && !m.frozen && !(m.handcard.card.Rush && m.playedThisTurn))
                    {
                        offset_enemy -= m.Angr;
                    }
                }
                offset_enemy -= p.calDirectDmg(p.mana + offsetMana, true);
            }

            int lifesteal = 0;
            foreach (Minion m in p.enemyMinions)
            {
                if (m.lifesteal)
                {
                    lifesteal += m.Angr;
                }
            }

            int retval = 0;
            // 血线安全
//            if (p.ownHero.Hp + p.ownHero.armor > hpboarder)
 //           {
////                retval += (5 + p.ownHero.Hp + p.ownHero.armor - hpboarder);
////            }
//            // 快死了
//            else
//            {
 //               //if (p.nextTurnWin()) retval -= (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
//                retval -= 5 * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor) * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
//            }
//            if (p.ownHero.Hp + p.ownHero.armor < 10 && p.ownHero.Hp + p.ownHero.armor > 0)
//            {
//                retval -= 200 / (p.ownHero.Hp + p.ownHero.armor);
//            }
//            // 对手血线安全
//            if (p.enemyHero.Hp + p.enemyHero.armor + lifesteal >= aggroboarder)
 //           {
//                retval += 2 * (aggroboarder - p.enemyHero.Hp - p.enemyHero.armor - lifesteal);
//            }
            // 开始打脸
//            else
//            {
//                retval += 6 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor - lifesteal);
//            }
            // 进入斩杀线 / 拖到后期，不考虑随从仅打脸
//            if (p.enemyHero.Hp + p.enemyHero.armor + lifesteal <= 5 && p.enemyHero.Hp + p.enemyHero.armor + lifesteal > 0 )
//            {
//                retval += 1000 / (p.enemyHero.Hp + p.enemyHero.armor + lifesteal);
//            }

            // 预计完成斩杀
//            if (p.enemyHero.Hp + p.enemyHero.armor + offset_enemy <= 0)
//            {
 //               retval += 2000;
 //           }
            return retval;
        }


        /// <summary>
        /// 攻击触发的奥秘惩罚
        /// </summary>
        /// <param name="si"></param>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        public override int getSecretPen_CharIsAttacked(Playfield p, SecretItem si, Minion attacker, Minion defender)
        {
            if (attacker.isHero) return 0;
            int pen = 0;
            // 攻击的基本惩罚
            if (si.canBe_explosive && !defender.isHero)
            {
                pen -= 20;
                //pen += (attacker.Hp + attacker.Angr);
                foreach (SecretItem sii in p.enemySecretList)
                {
                    sii.canBe_explosive = false;
                }
            }
            return pen;
        }
    }
}
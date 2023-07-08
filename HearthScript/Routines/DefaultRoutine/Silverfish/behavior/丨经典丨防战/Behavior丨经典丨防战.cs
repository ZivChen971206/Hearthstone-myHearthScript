using System.Collections.Generic;
using System;

namespace HREngine.Bots
{
    public partial class Behavior丨经典丨防战 : Behavior
    {
        public override string BehaviorName() { return "丨经典丨防战"; }
        PenalityManager penman = PenalityManager.Instance;
        // 核心，场面值
        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value > -20000) return p.value;
            float retval = 0;
            retval += getGeneralVal(p);
            //抽牌价值
            if (p.owncarddraw + p.owncards.Count <= 10)
            {
                if (p.owncards.Count <= 4)
                    retval += p.owncarddraw * 7;
                else retval += p.owncarddraw * 5;
            }
            else
            {
                //爆的牌按 1 - 1 白板算 价值为12
                retval -= (p.owncarddraw + p.owncards.Count - 10) * 12;
            }
            //考虑疲劳
            if (p.prozis.ownDeckSize < 8 && p.prozis.enemyDeckSize > p.prozis.ownDeckSize)
                retval -= p.owncarddraw * 6;
            if (p.prozis.ownDeckSize == 0 || p.prozis.ownHeroFatigue > 0)
                retval -= p.owncarddraw * p.prozis.ownHeroFatigue * 12;
            //手牌价值
            retval += p.owncards.Count;
            //下回合爆牌
            if (p.owncards.Count >= 10) retval -= 12;

            // 护甲
            retval += p.ownHero.armor;
            bool hasSlam = false;
            foreach (Handmanager.Handcard card in p.owncards)
            {
                if (card.card.nameCN == CardDB.cardNameCN.盾牌猛击)
                {
                    hasSlam = true;
                    break;
                }
            }
            if (hasSlam) retval += (int)p.ownHero.armor / 2;
            // 危险血线
            int hpboarder = 12;
            // 抢脸血线
            int aggroboarder = 12;
            retval += getHpValue(p, hpboarder, aggroboarder);
            // 对手基本随从交换模拟
            retval += enemyTurnPen(p);
            // 出牌序列数量
            int count = p.playactions.Count;
            int ownActCount = 0;
            // 排序问题！！！！
            for (int i = 0; i < count; i++)
            {
                Action a = p.playactions[i];
                ownActCount++;
                if (a.actionType == actionEnum.attackWithMinion)
                {
                    switch (a.own.handcard.card.nameCN)
                    {
                        case CardDB.cardNameCN.苦痛侍僧:
                            retval -= i;
                            break;
                    }
                }
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
                switch (a.card.card.nameCN)
                {
                    case CardDB.cardNameCN.顺劈斩:
                        retval -= i ;
                        break;
                    case CardDB.cardNameCN.严酷的监工:
                    case CardDB.cardNameCN.怒火中烧:
                        if(a.target != null && a.target.handcard.card.nameCN == CardDB.cardNameCN.苦痛侍僧)
                        {
                            retval += 6 - i * 3;
                        }
                        retval += i;
                        break;
                    //case CardDB.cardNameCN.全副武装:
                    //    retval -= i;
                    //    break;
                    // 排序优先
                    case CardDB.cardNameCN.猛击:
                    case CardDB.cardNameCN.盾牌格挡:
                        retval -= i * 2;
                        break;
                    //case CardDB.cardNameCN.绝命乱斗:
                    //    if (i != 0) retval -= 50;
                    //    break;
                    case CardDB.cardNameCN.盾牌猛击:
                        retval += i;
                        break;
                }
            }
            retval -= p.lostDamage;
            retval += getSecretPenality(p); // 奥秘的影响
            //p.value = retval;

            retval -= p.enemyWeapon.Angr * 3 + p.enemyWeapon.Durability * 3;
            
            // 有刀不 A 的惩罚
            if (p.ownWeapon != null && !p.ownHero.allreadyAttacked && p.ownWeapon.Durability > 0 )
            {
                foreach (Minion m in p.enemyMinions)
                {
                    if (m.Hp <= p.ownWeapon.Angr && !m.divineshild)
                    {
                        retval -= 50;
                        break;
                    }
                }
            }
            return retval;
        }


        // 敌方随从价值 主要等于 （HP + Angr） * 4 + 10 
        public override int getEnemyMinionValue(Minion m, Playfield p)
        {
            bool setGeddon = false;
            foreach (Minion mm in p.enemyMinions)
            {
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.末日预言者)
                {
                    setGeddon = true;
                    break;
                }
            }
            if (setGeddon)
            {
                return -1;
            }

            if (m.Hp <= 0) return 0;
            if (m.destroyOnEnemyTurnStart || m.destroyOnOwnTurnStart) return 0;
            //bool setGeddon = false;
            //foreach (Minion mm in p.ownMinions)
            //{
            //    if (mm.handcard.card.chnName == CardDB.cardNameCN.迦顿男爵)
            //    {
            //        setGeddon = true;
            //        break;
            //    }
            //}
            //if(setGeddon && m.Hp < 3 && !m.divineshild)
            //{
            //    return 0;
            //}

            int retval = 10;
            if (m.handcard.card.nameCN == CardDB.cardNameCN.烈焰小鬼)
            {
                retval = 10;
            }

            retval += m.Hp * 4;
            if (m.Angr <= 1 && p.enemyHeroStartClass != TAG_CLASS.PRIEST) retval -= m.Hp * 2;
            if (!m.frozen)
            {
                retval += m.Angr * 4;
                if (m.windfury) retval += m.Angr * 2;
                if (m.Angr > 5) retval += (m.Angr - 4) * 10;
            }else
            {
                retval += m.Angr * 2;
            }

            if (m.taunt) retval += 1;
            if (m.divineshild) retval += m.Angr;
            if (m.divineshild && m.taunt) retval += 5;
            if (m.stealth) retval += 1;

            if (m.poisonous)
            {
                retval += 4;
            }

            // 亡语怪不优先解
            if (m.handcard.card.deathrattle)
            {
                retval -= 3;
            }

            int bonus = 4;
            switch (m.handcard.card.nameCN){
                case CardDB.cardNameCN.凯恩血蹄:
                    retval += bonus * 5;
                    break;
                // 异能价值每多 1 属性值加权 4 点
                // 异能价值 4 属性值
                case CardDB.cardNameCN.加基森拍卖师:
                case CardDB.cardNameCN.任务达人:
                case CardDB.cardNameCN.苔原犀牛:
                    retval += bonus * 4;
                    break;
                // 异能价值 3 属性
                case CardDB.cardNameCN.迦顿男爵:
                case CardDB.cardNameCN.法力之潮图腾:
                case CardDB.cardNameCN.火舌图腾:
                case CardDB.cardNameCN.雷欧克:
                    retval += bonus * 3;
                    break;
                // 异能价值 2 属性
                case CardDB.cardNameCN.巫师学徒:
                case CardDB.cardNameCN.飞刀杂耍者:
                case CardDB.cardNameCN.大法师安东尼达斯:
                case CardDB.cardNameCN.苦痛侍僧:
                case CardDB.cardNameCN.北郡牧师:
                case CardDB.cardNameCN.狂野炎术师:
                case CardDB.cardNameCN.奥金尼灵魂祭司:
                case CardDB.cardNameCN.暴风城勇士:
                case CardDB.cardNameCN.年轻的女祭司:
                case CardDB.cardNameCN.恐狼前锋:
                case CardDB.cardNameCN.森林狼:
                case CardDB.cardNameCN.食腐土狼:
                case CardDB.cardNameCN.法力浮龙:
                case CardDB.cardNameCN.饥饿的秃鹫:
                    retval += bonus * 2;
                    break;
                // 异能价值 1 属性
                case CardDB.cardNameCN.铸甲师:
                case CardDB.cardNameCN.先知维伦:
                    retval += bonus * 2;
                    break;
                case CardDB.cardNameCN.炎魔之王拉格纳罗斯:
                    retval += bonus * 8;
                    break;
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
                // 大哥，求别解
                case CardDB.cardNameCN.希尔瓦娜斯风行者:
                    if (p.ownMinions.Count > 1)
                        retval -= 10;
                    else retval += 10;
                    break;                
            }

            if (p.ownHero.Hp < 15) retval += 10;
            return retval;
        }

        // 我方随从价值 主要等于 （HP + Angr） * 3 + 5 
        public override int getMyMinionValue(Minion m, Playfield p)
        {
            bool setGeddon = false;
            foreach (Minion mm in p.enemyMinions)
            {
                if (mm.handcard.card.nameCN == CardDB.cardNameCN.末日预言者)
                {
                    setGeddon = true;
                    break;
                }
            }
            if (setGeddon)
            {
                return -1;
            }
            if (m.Hp <= 0) return 0;

            //bool setGeddon = false;
            //foreach (Minion mm in p.ownMinions)
            //{
            //    if (mm.handcard.card.chnName == CardDB.cardNameCN.迦顿男爵)
            //    {
            //        setGeddon = true;
            //        break;
            //    }
            //}
            //if (setGeddon && m.Hp < 3 && !m.divineshild)
            //{
            //    return 0;
            //}

            int retval = 5;
            retval += m.Hp * 3;
            retval += m.Angr * 3;
            if (m.Hp <= 1) retval -= (m.Angr - 1) * 2;
            // 鱼人种族价值
            //if (m.handcard.card.race == CardDB.Race.MURLOC)
            //{
            //    retval += m.Hp + m.Angr + 2;
            //}
            // 风怒价值
            if ((!m.playedThisTurn || m.rush == 1 || m.charge == 1) && m.windfury) retval += m.Angr;
            // 圣盾价值
            if (m.divineshild ) retval += m.Angr / 2 + 1;
            // 潜行价值
            if (m.stealth) retval += m.Angr / 3 + 1;
            // 吸血
            if (m.lifesteal) retval += m.Angr / 3 + 1;
            // 圣盾嘲讽
            if (m.divineshild && m.taunt) retval += 4;
            // retval += m.synergy;
            // retval += getSpeicalCardVal(m.handcard.card);

            int bonus = 3;
            switch (m.handcard.card.nameCN)
            {
                // 异能价值 4 属性
                case CardDB.cardNameCN.炎魔之王拉格纳罗斯:
                case CardDB.cardNameCN.伊瑟拉:
                    retval += bonus * 4;
                    break;
                case CardDB.cardNameCN.凯恩血蹄:
                    retval += bonus * 5;
                    break; 
                // 异能价值 2 属性
                case CardDB.cardNameCN.迦顿男爵:
                case CardDB.cardNameCN.苦痛侍僧:
                case CardDB.cardNameCN.战利品贮藏者:
                case CardDB.cardNameCN.麦田傀儡:
                    retval += bonus * 2;
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
            bool found_aoe = false;
            foreach(Minion m in p.ownMinions)
            {
                if (m.handcard.card.nameCN == CardDB.cardNameCN.迦顿男爵) found_aoe = true;
            }
            // 初始惩罚值
            int pen = 0;
            pen += penman.getSilencePenality(card.nameEN, target, p);
            switch (card.nameCN)
            {
                case CardDB.cardNameCN.幸运币:
                    pen += 5;
                    break;
                case CardDB.cardNameCN.斩杀:
                    pen += 50;
                    if (target != null && target.Angr > 1)
                        pen -= (target.Angr + target.Hp) * 4;
                    break;
                case CardDB.cardNameCN.旋风斩:
                    pen += 20;
                    break;
                case CardDB.cardNameCN.狂奔科多兽:
                    break;
                case CardDB.cardNameCN.绝命乱斗:
                    pen += 40;
                    break;
                case CardDB.cardNameCN.猛击:
                    pen += 18;
                    if (found_aoe && target != null && target.Hp <= 2) pen += 1000;
                    break;
                case CardDB.cardNameCN.严酷的监工:
                case CardDB.cardNameCN.怒火中烧:
                    pen += 17;
                    if (target != null && target.own && target.Hp > 2 && !target.Ready)
                    {
                        pen += 1000;
                    }
                    break;
                case CardDB.cardNameCN.迦顿男爵:
                    foreach(Minion m in p.enemyMinions)
                    {
                        if (m.Hp <= 2) pen -= 6;
                    }
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Hp <= 2) pen += 6;
                    }
                    break;
                case CardDB.cardNameCN.希尔瓦娜斯风行者:
                    if (p.enemyMinions.Count > 2) pen -= 20;
                    break;
                case CardDB.cardNameCN.格罗玛什地狱咆哮:
                    pen += 15;
                    break;
                case CardDB.cardNameCN.顺劈斩:
                    pen += 15;
                    break;
                case CardDB.cardNameCN.阿古斯防御者:
                    break;
                case CardDB.cardNameCN.黑骑士:
                    break;
                case CardDB.cardNameCN.凯恩血蹄:
                    break;
                case CardDB.cardNameCN.炎魔之王拉格纳罗斯:
                    if (p.ownHero.Hp < 10 && p.enemyMinions.Count > 1) pen += 20;
                    break;
                case CardDB.cardNameCN.盾牌猛击:
                    pen += 25;
                    if(target.Hp > p.ownHero.armor + p.spellpower)
                    {
                        pen += 15;
                    }
                    break;
                case CardDB.cardNameCN.盾牌格挡:
                    pen += 8;
                    break;
                case CardDB.cardNameCN.阿莱克丝塔萨:
                    pen += 20;
                    break;
                //-----------------------------英雄技能-----------------------------------
                case CardDB.cardNameCN.全副武装:
                    pen += 4;
                    break;
            }
            return pen;
        }

        public override int getHpValue(Playfield p, int hpboarder, int aggroboarder)
        {
            int offset = 0;
            foreach(Minion m in p.ownMinions)
            {
                if(m.handcard.card.nameCN == CardDB.cardNameCN.迦顿男爵)
                {
                    offset -= 2;
                }
            }
            foreach (Minion m in p.enemyMinions)
            {
                if (m.handcard.card.nameCN == CardDB.cardNameCN.炎魔之王拉格纳罗斯)
                {
                    offset -= 8;
                }
                if (m.handcard.card.nameCN == CardDB.cardNameCN.迦顿男爵)
                {
                    offset -= 2;
                }
                if (m.handcard.card.nameCN == CardDB.cardNameCN.末日预言者)
                {
                    offset = 0;
                    break;
                }
            }

            int retval = 0;
            // 血线安全
            if (p.ownHero.Hp + p.ownHero.armor + offset > hpboarder)
            {
                retval += 2 * (5 + p.ownHero.Hp + p.ownHero.armor + offset - hpboarder) ;
            }
            // 快死了
            else
            {
                if (p.nextTurnWin()) retval -= 2 * (hpboarder + 1 - p.ownHero.Hp - offset - p.ownHero.armor);
                else retval -= 5 * (hpboarder + 1 - offset - p.ownHero.Hp - p.ownHero.armor) * (hpboarder + 1 - offset - p.ownHero.Hp - p.ownHero.armor);
            }
            if(p.ownHero.Hp + p.ownHero.armor + offset < 10 && p.ownHero.Hp + p.ownHero.armor + offset > 0)
            {
                retval -= 200 / (p.ownHero.Hp + p.ownHero.armor + offset);
            }
            if(p.ownHero.Hp + offset <= 7 && (p.enemyHeroStartClass == TAG_CLASS.HUNTER || p.enemyHeroStartClass == TAG_CLASS.MAGE || p.enemyHeroStartClass == TAG_CLASS.WARLOCK) )
            {
                retval -= 200;
            }
            // 对手血线安全,反正都是红龙一口的事情，完全不急
            if (p.enemyHero.Hp + p.enemyHero.armor >= aggroboarder)
            {
                retval += aggroboarder - p.enemyHero.Hp - p.enemyHero.armor ;
            }
            // 开始打脸
            else if (p.enemyHero.Hp + p.enemyHero.armor <= aggroboarder && p.enemyHero.Hp + p.enemyHero.armor > 3)
            {
                retval += 2 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor);
            }
            // 一个小斧子就斩了的事情
            else
            {
                retval += 6 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor);
            }
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
            int pen = 0;
            // 攻击的基本惩罚
            // 对手 T7 低血量不攻击
            if (si.canBe_explosive && defender.isHero && p.ownHero.Hp <= 10)
            {
                pen += 50;
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
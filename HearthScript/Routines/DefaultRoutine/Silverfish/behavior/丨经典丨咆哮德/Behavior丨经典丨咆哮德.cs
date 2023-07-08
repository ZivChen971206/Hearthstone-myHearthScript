using System.Collections.Generic;
using System;

namespace HREngine.Bots
{
    public partial class Behavior丨经典丨咆哮德 : Behavior
    {
        private int bonus_enemy = 4;
        private int bonus_mine = 4;

        public override string BehaviorName() { return "丨经典丨咆哮德"; }
        PenalityManager penman = PenalityManager.Instance;

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

            if (p.ownMaxMana == 1 && (nowHandcard.getManaCost(p) >= 4 && card.type == CardDB.cardtype.MOB) ) pen -= 40;
            
            switch (card.nameCN)
            {
                case CardDB.cardNameCN.大地之环先知:
                    if (!target.own) pen += 1000;
                    if (target.Hp == target.maxHp) pen += bonus_mine * 2;
                    if (p.ownMaxMana == 1) pen += bonus_mine * 5;
                    break;
                case CardDB.cardNameCN.野蛮咆哮:
                    pen += 30;
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.card.nameCN == CardDB.cardNameCN.自然之力) pen += 30;
                    }
                    foreach(Minion m in p.ownMinions)
                    {
                        if (m.Ready) pen -= 10;
                    }
                    break;
                case CardDB.cardNameCN.自然之力:
                    pen += 20;
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.card.nameCN == CardDB.cardNameCN.野蛮咆哮) pen += 20;
                        if (hc.card.nameCN == CardDB.cardNameCN.自然之力) pen -= 30;
                    }
                    break;
                case CardDB.cardNameCN.幸运币:
                case CardDB.cardNameCN.激活:
                    pen += 20;
                    if (p.enemyHeroStartClass != TAG_CLASS.HUNTER && p.enemyHeroStartClass != TAG_CLASS.WARLOCK && p.enemyHeroStartClass != TAG_CLASS.SHAMAN)
                    {
                        foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if (hc.card.nameCN == CardDB.cardNameCN.自然之力) pen += 20;
                        }
                    }
                    break;
                case CardDB.cardNameCN.愤怒:
                    pen += 10;
                    break;
                case CardDB.cardNameCN.横扫:
                    pen += 15;
                    break;
                case CardDB.cardNameCN.野性成长:
                    if (p.ownMaxMana == 9 || p.ownMaxMana == 8) pen += 1000;
                    break;
                case CardDB.cardNameCN.王牌猎人:
                    if(p.enemyHeroStartClass == TAG_CLASS.WARRIOR && target == null && p.ownMaxMana > 5)
                    {
                        pen += bonus_mine * 4;
                    }
                    break;
                case CardDB.cardNameCN.塞纳留斯:
                case CardDB.cardNameCN.凯恩血蹄:
                case CardDB.cardNameCN.希尔瓦娜斯风行者:
                case CardDB.cardNameCN.黑骑士:
                case CardDB.cardNameCN.冰风雪人:
                case CardDB.cardNameCN.森金持盾卫士:
                case CardDB.cardNameCN.银色侍从:
                case CardDB.cardNameCN.麦田傀儡:
                case CardDB.cardNameCN.野性之力:
                case CardDB.cardNameCN.战利品贮藏者:
                case CardDB.cardNameCN.丛林守护者:
                case CardDB.cardNameCN.利爪德鲁伊:
                case CardDB.cardNameCN.紫罗兰教师:
                case CardDB.cardNameCN.血法师萨尔诺斯:
                case CardDB.cardNameCN.碧蓝幼龙:
                case CardDB.cardNameCN.知识古树:
                    break;
            }
            return pen;
        }

        // 核心，场面值
        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value > -200000) return p.value;
            float retval = 0;
            retval += getGeneralVal(p);
            retval += p.owncarddraw * 12;


            if (p.ownMaxMana <= 4)
            {
                retval += 25 * p.ownMaxMana;
            }
            else if (p.ownMaxMana <= 6)
            {
                retval += 25 * 4 + (p.ownMaxMana - 4) * 10;
            }
            else if (p.ownMaxMana <= 8)
            {
                retval += 25 * 4 + 10 * 2 + (p.ownMaxMana - 6) * 5;
            }
            else
            {
                retval += 25 * 4 + 10 * 2 + 5 * 2;
            }

            // 危险血线
            int hpboarder = 5;
            // 不考虑法强了
            if (p.enemyHeroName == HeroEnum.mage) retval += 2 * p.enemyspellpower;
            // 抢脸血线
            int aggroboarder = 20;
            retval += getHpValue(p, hpboarder, aggroboarder);
            // 出牌序列数量
            int count = p.playactions.Count;
            int ownActCount = 0;
            bool useCoin = false;
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
                switch (a.card.card.nameCN)
                {
                    case CardDB.cardNameCN.自然之怒:
                    case CardDB.cardNameCN.知识古树:
                    case CardDB.cardNameCN.碧蓝幼龙:
                    case CardDB.cardNameCN.法力过剩:
                        retval -= 2 * i;
                        break;
                    // 排序优先
                    case CardDB.cardNameCN.激活:
                    case CardDB.cardNameCN.幸运币:
                        useCoin = true;
                        retval -= i;
                        break;
                }
            }
            if (useCoin && p.manaTurnEnd > 0) retval -= 30;
            // 对手基本随从交换模拟
            retval += enemyTurnPen(p);
            retval -= p.lostDamage;
            retval += getSecretPenality(p); // 奥秘的影响
            retval -= p.enemyWeapon.Angr * 3 + p.enemyWeapon.Durability * 3;
            return retval;
        }

        // 敌方随从价值 主要等于（HP + Angr） * 4 + 4
        public override int getEnemyMinionValue(Minion m, Playfield p)
        {
            bool dieNextTurn = false;
            foreach (Minion mm in p.enemyMinions)
            {
                if (!mm.silenced && mm.handcard.card.nameCN == CardDB.cardNameCN.末日预言者)
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
            int retval = 4;
            if (m.Angr > 0 || m.taunt || m.handcard.card.race == CardDB.Race.TOTEM || p.enemyHeroStartClass == TAG_CLASS.PALADIN || p.enemyHeroStartClass == TAG_CLASS.PRIEST)
                retval += m.Hp * 4;
            retval += m.spellpower * 4;
            if (!m.frozen && (!m.cantAttack || m.handcard.card.nameCN == CardDB.cardNameCN.邪刃豹))
            {
                retval += m.Angr * 4;
                if (m.Angr > 5) retval += 10;
                if (m.windfury) retval += m.Angr * 2;
            }
            if (m.silenced) return retval;

            if (m.taunt) retval += 2;
            if (m.divineshild) retval += m.Angr * 2;
            if (m.divineshild && m.taunt) retval += 5;
            if (m.stealth) retval += 2;

            // 鱼人
            if (m.handcard.card.race == CardDB.Race.MURLOC) retval += bonus_enemy * 4;

            // 剧毒价值两点属性
            if (m.poisonous)
            {
                retval += 8;
            }
            if (m.lifesteal) retval += m.Angr * bonus_enemy * 4;

            int bonus = 4;
            switch (m.handcard.card.nameCN)
            {
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
            return retval;
        }

        // 我方随从价值，大致等于主要等于 （HP + Angr） * 4 + 4
        public override int getMyMinionValue(Minion m, Playfield p)
        {
            bool dieNextTurn = false;
            foreach (Minion mm in p.enemyMinions)
            {
                if (!mm.silenced && mm.handcard.card.nameCN == CardDB.cardNameCN.末日预言者)
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
            int retval = 4;
            retval += m.Hp * 4;
            retval += m.Angr * 4;
            retval += m.spellpower * 4;
            if (m.Hp <= 1) retval -= (m.Angr - 1) * 3;
            if (m.Angr <= 1)
            {
                if (m.Hp > 3)
                {
                    retval -= (m.Hp - 3) * 4;
                }
            }
            // 高攻低血是垃圾
            if (m.Angr > m.Hp + 4) retval -= (m.Angr - m.Hp) * 3;

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

            int bonus = 4;
            switch (m.handcard.card.nameCN)
            {
                case CardDB.cardNameCN.凯恩血蹄:
                    retval += bonus_mine * 5;
                    break;
                // 优先选择熊德吧
                case CardDB.cardNameCN.利爪德鲁伊:
                    if (m.charge > 0 && p.enemyHero.Hp >= 15) retval -= bonus_mine * 5;
                    if (m.taunt ) retval += bonus_mine * 2;
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


        public override int getHpValue(Playfield p, int hpboarder, int aggroboarder)
        {
            int offset_enemy = 0;
            int offset_mine = p.calEnemyTotalAngr() + Hrtprozis.Instance.enemyDirectDmg;

            int retval = 0;
            if(p.enemyGuessDeck == "经典T7猎" && p.ownHero.Hp + p.ownHero.armor <= 15)
            {
                retval += 4 * (p.ownHero.Hp + p.ownHero.armor) - 45;
            }
            // 血线安全
            if (p.ownHero.Hp + p.ownHero.armor - offset_mine > hpboarder)
            {
                retval += 5 * (5 + p.ownHero.Hp + p.ownHero.armor - offset_mine - hpboarder) / 2 ;
            }
            // 快死了
            else if (p.ownHero.Hp + p.ownHero.armor - offset_mine > 0)
            {
                //if (p.nextTurnWin()) retval -= (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
                retval -= 5 * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor + offset_mine) * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor + offset_mine);
            }
            else
            {
                retval -= 3 * (hpboarder + 1) * (hpboarder + 1) + 100;
            }
            if (p.ownHero.Hp + p.ownHero.armor - offset_mine < 6 && p.ownHero.Hp + p.ownHero.armor - offset_mine > 0)
            {
                retval -= 80 / (p.ownHero.Hp + p.ownHero.armor - offset_mine);
            }
            // 对手血线安全
            if (p.enemyHero.Hp + p.enemyHero.armor + offset_enemy >= aggroboarder)
            {
                retval += (aggroboarder - p.enemyHero.Hp - p.enemyHero.armor - offset_enemy);
            }
            // 开始打脸
            else
            {
                retval += 3 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor - offset_enemy);
            }
            // 进入斩杀线
            // if (p.enemyHero.Hp + p.enemyHero.armor + offset_enemy <= 5 && p.enemyHero.Hp + p.enemyHero.armor + offset_enemy > 0)
            // {
            //     retval += 300 / (p.enemyHero.Hp + p.enemyHero.armor - offset_enemy);
            // }
            // 场攻+直伤大于对方生命，预计完成斩杀
            if (p.anzEnemyTaunt == 0 && p.calTotalAngr() + p.calDirectDmg(p.mana, false) >= p.enemyHero.Hp + p.enemyHero.armor)
            {
                retval += 2000;
            }
            // 下回合斩杀本回合打脸
            //if (p.calDirectDmg(p.ownMaxMana + 1, false, true) >= p.enemyHero.Hp + p.enemyHero.armor)
            //{
            //    retval += 100;
            //}
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
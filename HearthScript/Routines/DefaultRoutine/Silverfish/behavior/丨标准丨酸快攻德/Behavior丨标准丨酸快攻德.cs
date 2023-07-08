using System.Collections.Generic;
using System;
using System.Linq;

namespace HREngine.Bots
{
    public partial class Behavior丨标准丨酸快攻德 : Behavior
    {
        private int bonus_enemy = 4;
        private int bonus_mine = 4;

        public override string BehaviorName() { return "丨标准丨酸快攻德"; }
        PenalityManager penman = PenalityManager.Instance;

        public override int getComboPenality(CardDB.Card card, Minion target, Playfield p, Handmanager.Handcard nowHandcard)
        {
            // 无法选中
            if (target != null && target.untouchable)
            {
                return 100000;
            }

            // 初始惩罚值
            int pen = penman.getSilencePenality(card.nameEN, target, p);
            int hp = p.enemyHero.Hp + p.enemyHero.armor;

            int ifUseNature = 0;            //////////////////////判断自然使徒是否能够buff///////////////////



            //foreach (Minion elm in this.ownMinions) 
            //{
            //    if ((TAG_RACE)elm.handcard.card.race == TAG_RACE.ELEMENTAL)
            //    {
            //        this.anzOwnElementalsThisTurn = 1;
            //        this.anzOwnElementalsLastTurn = this.anzOwnElementalsThisTurn;
            //        break;
            //    }
            //}

            ////////////////////判断结束///////////////////////////////////////
            switch (card.nameCN)
            {
                case CardDB.cardNameCN.农夫:
                    pen -= bonus_mine;
                    if (p.ownMaxMana <= 3) pen -= bonus_mine;
                    if (p.owncards.Count <= 3) pen -= bonus_mine;
                    switch (p.enemyHeroStartClass)
                    {
                        case TAG_CLASS.MAGE:
                            pen += bonus_mine;
                            break;
                        case TAG_CLASS.DRUID:
                        case TAG_CLASS.DEMONHUNTER:
                            if (p.ownMinions.Exists(x => x.taunt)) pen -= bonus_mine;
                            else pen += bonus_mine;
                            break;
                        case TAG_CLASS.PRIEST:
                            if (p.enemyHeroAblility.card.nameCN == CardDB.cardNameCN.心灵尖刺)
                            {
                                pen += bonus_mine;
                            }
                            else
                            {
                                pen -= bonus_mine;
                            }
                            break;
                        case TAG_CLASS.HUNTER:
                        case TAG_CLASS.WARRIOR:
                        case TAG_CLASS.WARLOCK:
                        case TAG_CLASS.ROGUE:
                        case TAG_CLASS.SHAMAN:
                            pen -= bonus_mine;
                            break;
                        default:
                            pen -= bonus_mine;
                            break;
                    }
                    break;
                case CardDB.cardNameCN.深铁穴居人:
                    pen -= bonus_mine * 2;
                    if (p.ownMaxMana <= 3) pen -= bonus_mine;
                    if (p.ownMinions.Exists(x => x.nameCN == CardDB.cardNameCN.深铁穴居人)) pen += bonus_mine * 3;
                    //if (p.useNature != 1) return 500;
                    //return -500;
                    break;
                case CardDB.cardNameCN.水栖形态:
                    if (p.mana < 2) return 500;
                    break;
                case CardDB.cardNameCN.安插证据:
                    if (p.mana < 3) return 500;
                    break;
                case CardDB.cardNameCN.荆棘护卫:
                    //if (p.useNature != 1) return 500;
                    //return -500;
                    break;
                case CardDB.cardNameCN.暗礁德鲁伊:
                    break;
                case CardDB.cardNameCN.防护改装师:
                case CardDB.cardNameCN.劳累的驮骡:
                case CardDB.cardNameCN.吵吵机器人:
                    break;
                case CardDB.cardNameCN.萌物来袭:
                case CardDB.cardNameCN.尖壳印记:
                case CardDB.cardNameCN.野性印记:
                    break;
                case CardDB.cardNameCN.噬骨殴斗者:
                    pen -= bonus_mine;
                    break;
                case CardDB.cardNameCN.堆肥:
                    int sum = 0;
                    foreach (var x in p.ownMinions)
                    {
                        sum += x.infest;
                    }
                    if (sum + p.owncards.Count + p.ownMinions.Count > 15) pen += bonus_mine * 7 * (sum + p.owncards.Count + p.ownMinions.Count - 13);
                    if (p.ownMinions.Count <= 1) pen += bonus_mine * 5;
                    else
                    {
                        pen -= bonus_mine * 2 * (p.ownMinions.Count);
                    }
                    if (p.owncards.Count <= 3) pen -= bonus_mine * 3;
                    break;
                case CardDB.cardNameCN.怒爪精锐:
                    foreach (var m in p.ownMinions) m.updateReadyness();
                    int cnt = p.ownMinions.Count(x => x.Ready);
                    if (p.owncards.Count(x => x.card.type == CardDB.cardtype.MOB)
                        - p.owncards.Count(x => x.card.nameCN == CardDB.cardNameCN.怒爪精锐) >= cnt && cnt < 5) pen += bonus_mine * 5;
                    else pen -= bonus_mine * cnt;
                    break;
                case CardDB.cardNameCN.钢鬃卫兵:
                    pen += bonus_mine;
                    break;
                case CardDB.cardNameCN.邪恶的厨师:
                    pen -= bonus_mine * 4 * (p.ownMinions.Count - 2);
                    break;
                case CardDB.cardNameCN.艾露恩神谕者:
                    if (p.owncards.Exists(x => x.getManaCost(p) <= p.mana - nowHandcard.getManaCost(p))) pen -= bonus_mine * 4;
                    break;
                case CardDB.cardNameCN.德雷克塔尔:
                    if (p.ownMinions.Count < 5) pen -= bonus_mine * 9;
                    if (p.ownMinions.Count == 5) pen -= bonus_mine * 5;
                    break;
                case CardDB.cardNameCN.树木生长:
                    pen -= bonus_mine * (3 * p.ownMinions.Count + 7 * p.ownMinions.Count <= 5 ? 2 : 7 - p.ownMinions.Count);
                    break;
                case CardDB.cardNameCN.狮群之怒:
                    pen -= bonus_mine * 6 * (p.ownMinions.Count - 2);
                    break;
                case CardDB.cardNameCN.自然使徒:
                    if (p.useNature != 1) return 500;
                    //return -500;
                    pen -= bonus_mine * 7 * (p.ownMinions.Count - 1);
                    //pen -= bonus_mine * 4 * (p.ownMinions.Count - 2);
                    break;
                case CardDB.cardNameCN.肥料滋养:
                    pen -= bonus_mine * 5 * (p.ownMinions.Count - 2);
                    //pen -= bonus_mine * 4 * (p.ownMinions.Count - 2);
                    break;
                //-----------------------------英雄技能-----------------------------------
                case CardDB.cardNameCN.变形:
                    pen += bonus_mine * 2;
                    break;
                case CardDB.cardNameCN.未知:
                    pen = 50;
                    break;
            }
            return pen;
        }

        // 核心，场面值
        public override float getPlayfieldValue(Playfield p)
        {
            /*投降特判
            if (((p.owncards.Count == 0 && p.ownMinions.Count == 1 && p.ownMinions[0].Hp + p.ownMinions[0].Angr < 8)
                || (p.ownMinions.Count == 0 && p.owncards.Count <= 1))
                && p.enemyHero.Hp + p.enemyHero.armor > 10 && p.enemyCardsCountStarted >= 5) return -20000;
            */

            if (p.value > -200000) return p.value;
            float retval = 0;
            retval += getGeneralVal(p);
            retval += p.owncarddraw * 5;
            // 危险血线
            int hpboarder = 7;
            // 考虑法强
            retval -= 5 * p.enemyspellpower;
            // 抢脸血线
            int aggroboarder = 18;
            retval += getHpValue(p, hpboarder, aggroboarder);
            // 出牌序列数量
            int count = p.playactions.Count;
            int ownActCount = 0;

            /*
             * 贪
             if (p.owncards.Exists(x => x.card.nameCN == CardDB.cardNameCN.树木生长 || x.card.nameCN == CardDB.cardNameCN.怒爪精锐
            || x.card.nameCN == CardDB.cardNameCN.堆肥 || x.card.nameCN == CardDB.cardNameCN.狮群之怒))
            {
                retval += p.ownMinions.Count;
            }
            */

            // 排序问题！！！
            bool attack = false;
            for (int i = 0; i < count; i++)
            {
                Action a = p.playactions[i];
                ownActCount++;
                switch (a.actionType)
                {
                    // 英雄攻击
                    case actionEnum.attackWithHero:
                    case actionEnum.attackWithMinion:
                        attack = true;
                        continue;
                    case actionEnum.useHeroPower:
                    case actionEnum.playcard:
                        break;
                    default:
                        continue;
                }
                switch (a.card.card.nameCN)
                {
                    // 排序优先
                    case CardDB.cardNameCN.萌物来袭:
                    case CardDB.cardNameCN.堆肥:
                    case CardDB.cardNameCN.野性印记:
                    case CardDB.cardNameCN.怒爪精锐:
                    case CardDB.cardNameCN.狮群之怒:
                    case CardDB.cardNameCN.树木生长:
                    case CardDB.cardNameCN.自然使徒:
                    case CardDB.cardNameCN.播种施肥:
                    case CardDB.cardNameCN.肥料滋养:
                        retval += i * 2;
                        break;
                    case CardDB.cardNameCN.水栖形态:
                    //case CardDB.cardNameCN.安插证据:
                        retval -= i * 7 - 7;
                        break;
                    case CardDB.cardNameCN.荆棘护卫:
                    case CardDB.cardNameCN.邪恶的厨师:
                    case CardDB.cardNameCN.凶恶的滑矛纳迦:
                    case CardDB.cardNameCN.暗礁德鲁伊:
                    case CardDB.cardNameCN.活体根须:
                        retval -= i;
                        break;
                    case CardDB.cardNameCN.德雷克塔尔:
                        retval -= i * 3 - 1;
                        break;
                    case CardDB.cardNameCN.艾露恩神谕者:
                        retval -= i * 3;
                        break;
                    case CardDB.cardNameCN.钢鬃卫兵:
                    case CardDB.cardNameCN.应急木工:
                        retval -= i * 2;
                        break;
                    case CardDB.cardNameCN.尖壳印记:
                        retval -= i;
                        break;
                    case CardDB.cardNameCN.劳累的驮骡:
                    case CardDB.cardNameCN.吵吵机器人:
                    case CardDB.cardNameCN.噬骨殴斗者:
                        if (p.ownMinions.Exists(x => x.nameCN == CardDB.cardNameCN.钢鬃卫兵)) retval -= i;
                        break;

                }
            }
            // 对手基本随从交换模拟
            retval += enemyTurnPen(p);
            retval -= p.lostDamage;
            retval += getSecretPenality(p); // 奥秘的影响
            retval -= p.enemyWeapon.Angr * 3 + p.enemyWeapon.Durability * 3;

            // 特殊：优势防亵渎
            if (retval > 50 && (p.enemyHeroStartClass == TAG_CLASS.WARLOCK || (p.enemyMaxMana >= 4 && p.enemyMobsCountStarted >= 5)) && p.enemyMinions.Count == 0 && p.ownMinions.Count > 2)
            {
                bool found = false;
                // 从 2 开始防，术士自带一堆 1
                for (int i = 1; i <= 10; i++)
                {
                    found = false;
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.Hp == i)
                        {
                            retval -= 2 * (i - 1);
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        if (i == 1) retval += 10;
                        if (i == 2) retval += 10;
                        break;
                    }
                }
            }

            return retval;
        }

        // 发现卡的价值
        public override int getDiscoverVal(CardDB.Card card, Playfield p)
        {
            int aimcard = 0;
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if (hc.card.nameCN == CardDB.cardNameCN.自然使徒)
                {
                    aimcard = 1;
                }
                else aimcard = 0;
            }
            int disvalue = 5;
            int offset = p.ownAbilityReady ? 2 : 0;
            switch (card.nameCN)
            {
                case CardDB.cardNameCN.活体根须:
                    if (p.useNature == 0&& aimcard == 1) return 10;
                    if (p.ownMinions.Count < 2) return 8;
                    return 5;
                case CardDB.cardNameCN.水栖形态:
                    return 6;
                case CardDB.cardNameCN.深铁穴居人:
                    if (p.mana == 1&& p.ownMinions.Count < 2) return 10;
                    return 5;
                case CardDB.cardNameCN.欢快的同伴:
                    if (p.ownMinions.Count > 1) return 6;
                    return 5;
                case CardDB.cardNameCN.暗礁德鲁伊:
                    foreach (Minion mi in p.enemyMinions)
                    {
                        //对方随从生命值小于 3
                        if (mi.Hp <= 3)
                        {
                            if (mi.name != CardDB.cardNameEN.madscientist)
                            {
                                if (mi.Angr >= 6) return 8;
                            }
                        }
                    }
                    return 5;

                case CardDB.cardNameCN.播种施肥:
                    if (p.mana >= 1 && p.ownMinions.Count > 2) return 15;
                    if (p.mana >= 1 && p.ownMinions.Count == 2) return 8;
                    return 5;
                case CardDB.cardNameCN.安插证据:
                    if (p.useNature == 0 && aimcard == 1) return 10;
                    return 5;
                case CardDB.cardNameCN.凶恶的滑矛纳迦:
                    if (p.mana == 1 && p.ownMinions.Count < 2) return 10;
                    return 5;
                case CardDB.cardNameCN.农夫:
                    if (p.mana == 1 && p.enemyMinions.Count < 1) return 9;
                    return 5;
                case CardDB.cardNameCN.野性印记:
                    if (p.mana > 1&& p.ownMinions.Count > 0 && p.useNature == 0) return 9;
                    return 5;
                case CardDB.cardNameCN.邪恶的厨师:
                    if (p.ownMinions.Count > 2 && p.mana > 1) return 20;
                    else return 0;
                case CardDB.cardNameCN.荆棘护卫:
                    if (p.mana > 1&& p.useNature == 0 && aimcard == 1) return 15;
                    if (p.mana > 1 && p.ownMinions.Count < 2) return 15;
                    return 6;
                case CardDB.cardNameCN.应急木工:
                    if ( p.mana > 1&& p.owncards.Count <= 3) return 10;
                    return 0;
                case CardDB.cardNameCN.自然使徒:
                    if (p.ownMinions.Count > 2 && p.mana > 3) return 20;
                    return 0;
                case CardDB.cardNameCN.狮群之怒:
                    if (p.ownMinions.Count > 2 && p.mana > 3) return 20;
                    return 0;
                ////////////////////////以下为安插证据的发现//////////////////////////////
                case CardDB.cardNameCN.自然之力:
                    if (p.ownMinions.Count < 2 && p.mana > 3) return 10;
                    return 5;
                case CardDB.cardNameCN.滋养:
                    if (p.owncards.Count < 2 && p.mana > 3) return 10;
                    return 5;
                case CardDB.cardNameCN.丛林之魂:
                    if (p.ownMinions.Count > 2 && p.mana > 2) return 9;
                    return 5;
                case CardDB.cardNameCN.野性之怒:
                    if ( p.mana > 1) return 5;
                    return 5;
                case CardDB.cardNameCN.野性成长:
                    return 1;
                case CardDB.cardNameCN.大地之鳞:
                    return 1;
                case CardDB.cardNameCN.愤怒:
                    if ( p.mana <= 1) return 6;
                    return 1;
                case CardDB.cardNameCN.激活:
                    return 1;
                case CardDB.cardNameCN.尖壳印记:
                    if (p.mana <= 1 && p.ownMinions.Count > 0) return 9;
                    return 5;
                case CardDB.cardNameCN.堆肥:
                    if (p.ownMinions.Count > 2) return 20;
                    return 1;
                //case CardDB.cardNameCN.奇迹生长:
                //    if (p.mana >= 6) return 20;
                //    return 1;
                case CardDB.cardNameCN.自然死亡:
                    return 6;
                case CardDB.cardNameCN.野性之力:
                    if (p.ownMinions.Count > 2 ) return 8;
                    return 5;
                case CardDB.cardNameCN.飞扑:
                    return 4;
                case CardDB.cardNameCN.科多兽坐骑:
                    if (p.mana > 2 && p.ownMinions.Count > 0) return 11;
                    return 5;
                case CardDB.cardNameCN.月光指引:
                    if (p.ownMinions.Count < 2 ) return 10;
                    return 5;
                case CardDB.cardNameCN.占领冷齿矿洞:
                    return 1;
            }
            return 0;
        }

        // 敌方随从价值 主要等于（HP + Angr） * 4  + 5
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
            int retval = 1;
            if (m.Angr > 0 || p.enemyHeroStartClass == TAG_CLASS.PALADIN || p.enemyHeroStartClass == TAG_CLASS.PRIEST)
                retval += m.Hp * 4;
            retval += m.spellpower * 2;
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

            // 剧毒价值两点属性
            if (m.poisonous)
            {
                retval += 8;
            }
            if (m.lifesteal) retval += m.Angr * bonus_enemy * 4;

            //攻击时免疫
            if (m.immuneWhileAttacking) retval += m.Angr * 3;

            int bonus = 4;
            switch (m.handcard.card.nameCN)
            {
                case CardDB.cardNameCN.血骨傀儡:
                    return 100;
                // 异能价值每多 1 属性值加权 4 点
                // 异能价值 4 属性值
                case CardDB.cardNameCN.原野联络人:
                case CardDB.cardNameCN.奥:
                case CardDB.cardNameCN.奥的灰烬:
                case CardDB.cardNameCN.科卡尔驯犬者:
                case CardDB.cardNameCN.巡游领队:
                case CardDB.cardNameCN.团伙核心:
                case CardDB.cardNameCN.前沿哨所:
                case CardDB.cardNameCN.莫尔杉哨所:
                case CardDB.cardNameCN.塔姆辛罗姆:
                case CardDB.cardNameCN.导师火心:
                case CardDB.cardNameCN.伊纳拉碎雷:
                case CardDB.cardNameCN.暗影珠宝师汉纳尔:
                case CardDB.cardNameCN.伦萨克大王:
                case CardDB.cardNameCN.洛卡拉:
                    retval += bonus * 4;
                    break;
                case CardDB.cardNameCN.虚触侍从:
                    // 谢谢啊
                    retval -= bonus * 4;
                    break;
                // 异能价值 2 属性值
                case CardDB.cardNameCN.萨特监工:
                case CardDB.cardNameCN.食人魔巫术师:
                case CardDB.cardNameCN.甩笔侏儒:
                case CardDB.cardNameCN.精英牛头人酋长金属之神:
                case CardDB.cardNameCN.克欧雷:
                case CardDB.cardNameCN.雷欧克:
                case CardDB.cardNameCN.纳兹曼尼织血者:
                case CardDB.cardNameCN.塞泰克织巢者:
                    retval += bonus * 3;
                    break;
                // 异能价值 2 属性值
                case CardDB.cardNameCN.新生刺头:
                    if (m.Spellburst) retval += bonus * 1;
                    break;
                case CardDB.cardNameCN.对空奥术法师:
                case CardDB.cardNameCN.火焰术士弗洛格尔:
                case CardDB.cardNameCN.黑眼:
                    retval += bonus_enemy * 20;
                    break;
                // 解不掉游戏结束
                case CardDB.cardNameCN.洛萨:
                case CardDB.cardNameCN.考内留斯罗姆:
                case CardDB.cardNameCN.战场军官:
                case CardDB.cardNameCN.大领主弗塔根:
                case CardDB.cardNameCN.圣殿蜡烛商:
                case CardDB.cardNameCN.伯尔纳锤喙:
                case CardDB.cardNameCN.魅影歹徒:
                case CardDB.cardNameCN.灵魂窃贼:
                case CardDB.cardNameCN.紫罗兰法师:
                case CardDB.cardNameCN.甜水鱼人斥候:
                case CardDB.cardNameCN.狂欢报幕员:
                case CardDB.cardNameCN.巫师学徒:
                case CardDB.cardNameCN.布莱恩铜须:
                case CardDB.cardNameCN.观星者露娜:
                case CardDB.cardNameCN.大法师瓦格斯:
                case CardDB.cardNameCN.火妖:
                case CardDB.cardNameCN.下水道渔人:
                case CardDB.cardNameCN.空中炮艇:
                case CardDB.cardNameCN.船载火炮:
                case CardDB.cardNameCN.火舌图腾:
                case CardDB.cardNameCN.农夫:
                case CardDB.cardNameCN.艾露恩神谕者:
                    retval += bonus_enemy * 8;
                    break;
                case CardDB.cardNameCN.末日预言者:
                    break;
                // 不解巨大劣势
                case CardDB.cardNameCN.拍卖师亚克森:
                case CardDB.cardNameCN.法师猎手:
                case CardDB.cardNameCN.凯瑞尔罗姆:
                case CardDB.cardNameCN.鱼人领军:
                case CardDB.cardNameCN.南海船长:
                case CardDB.cardNameCN.坎雷萨德埃伯洛克:
                case CardDB.cardNameCN.人偶大师多里安:
                case CardDB.cardNameCN.暗鳞先知:
                case CardDB.cardNameCN.灭龙弩炮:
                case CardDB.cardNameCN.神秘女猎手:
                case CardDB.cardNameCN.鲨鳍后援:
                case CardDB.cardNameCN.怪盗图腾:
                case CardDB.cardNameCN.矮人神射手:
                case CardDB.cardNameCN.任务达人:
                case CardDB.cardNameCN.贪婪的书虫:
                case CardDB.cardNameCN.战马训练师:
                case CardDB.cardNameCN.相位追猎者:
                case CardDB.cardNameCN.鱼人宝宝车队:
                case CardDB.cardNameCN.科多兽骑手:
                case CardDB.cardNameCN.奥秘守护者:
                case CardDB.cardNameCN.获救的流民:
                case CardDB.cardNameCN.低阶侍从:
                case CardDB.cardNameCN.深铁穴居人:
                    retval += bonus_enemy * 3;
                    break;
                case CardDB.cardNameCN.白银之手新兵:
                    if (p.enemyHeroAblility.card.nameCN == CardDB.cardNameCN.白银之手)
                        retval += bonus_enemy * 3;
                    break;
                // 算有点用
                case CardDB.cardNameCN.幽灵狼前锋:
                case CardDB.cardNameCN.战斗邪犬:
                case CardDB.cardNameCN.饥饿的秃鹫:
                case CardDB.cardNameCN.法力浮龙:
                case CardDB.cardNameCN.加基森拍卖师:
                case CardDB.cardNameCN.飞刀杂耍者:
                case CardDB.cardNameCN.锈水海盗:
                case CardDB.cardNameCN.大法师安东尼达斯:
                    retval += bonus_enemy * 2;
                    break;
                case CardDB.cardNameCN.疯狂的科学家:
                    retval -= bonus_enemy * 4;
                    break;

                case CardDB.cardNameCN.月牙:
                    //case "明月之牙":
                    retval += bonus_enemy * m.Hp;
                    break;
            }
            return retval;
        }

        // 我方随从价值，大致等于主要等于 （HP + Angr） * 4  + 5
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
            }
            if (m.destroyOnEnemyTurnEnd || m.destroyOnEnemyTurnStart || m.destroyOnOwnTurnEnd || m.destroyOnOwnTurnStart) dieNextTurn = true;
            if (dieNextTurn)
            {
                return -1;
            }
            if (m.Hp <= 0) return 0;
            int retval = 1;
            retval += m.Hp * 4;
            retval += m.Angr * 4;

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
            if (m.divineshild && m.taunt) retval += 8;
            //攻击时免疫
            if (m.immuneWhileAttacking) retval += m.Angr * 3;

            //堆肥送怪
            if (m.infest > 0 && p.owncards.Count <= 3 && m.Angr + m.Hp <= 2
                && p.enemyMinions.Count > 0 && p.enemyHero.Hp + p.enemyHero.armor >= 10) retval -= 4;


            switch (m.nameCN)
            {
                case CardDB.cardNameCN.深铁穴居人:
                    retval += 8;
                    if (p.ownMaxMana <= 4) retval += 4;
                    break;
                case CardDB.cardNameCN.农夫:
                    retval += 8;
                    if (p.owncards.Count <= 5) retval += 4;
                    break;
                case CardDB.cardNameCN.邪恶的厨师:
                    retval += 8;
                    if (p.owncards.Count <= 5) retval += 4;
                    break;
                case CardDB.cardNameCN.艾露恩神谕者:
                    if (p.owncards.Exists(x => x.getManaCost(p) <= 2)) retval += 16;
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
            if (p.ownHero.Hp + p.ownHero.armor > hpboarder)
            {
                retval += (5 + p.ownHero.Hp + p.ownHero.armor - hpboarder);
            }
            // 快死了
            else
            {
                //if (p.nextTurnWin()) retval -= (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
                retval -= 5 * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor) * (hpboarder + 1 - p.ownHero.Hp - p.ownHero.armor);
            }
            if (p.ownHero.Hp + p.ownHero.armor < 10 && p.ownHero.Hp + p.ownHero.armor > 0)
            {
                retval -= 200 / (p.ownHero.Hp + p.ownHero.armor);
            }
            // 对手血线安全
            if (p.enemyHero.Hp + p.enemyHero.armor + lifesteal >= aggroboarder)
            {
                retval += 2 * (aggroboarder - p.enemyHero.Hp - p.enemyHero.armor - lifesteal);
            }
            // 开始打脸
            else
            {
                retval += 7 * (aggroboarder + 1 - p.enemyHero.Hp - p.enemyHero.armor - lifesteal);
            }
            // 进入斩杀线 / 拖到后期，不考虑随从仅打脸
            //if (p.enemyHero.Hp + p.enemyHero.armor + lifesteal <= 5 && p.enemyHero.Hp + p.enemyHero.armor + lifesteal > 0)
            //{
            //    retval += 1000 / (p.enemyHero.Hp + p.enemyHero.armor + lifesteal);
            //}

            // 预计完成斩杀
            if (p.enemyHero.Hp + p.enemyHero.armor + offset_enemy <= 0)
            {
                retval += 2000;
            }

            // 下回合斩杀
            if (p.calDirectDmg(p.ownMaxMana + 1 > 10 ? 10 : p.ownMaxMana + 1, false) >= lifesteal + p.enemyHero.Hp)
            {
                retval += 1000;
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
            if (attacker.isHero) return 0;
            int pen = 0;
            // 攻击的基本惩罚
            if (si.canBe_explosive)
            {
                if (defender.isHero)
                {
                    if (p.enemyMinions.Count > 0) pen += 10 * p.ownMinions.Count(x => x.Hp <= 2);
                }
                else
                {
                    pen -= 20;
                    //pen += (attacker.Hp + attacker.Angr);
                    foreach (SecretItem sii in p.enemySecretList)
                    {
                        sii.canBe_explosive = false;
                    }

                }
            }
            return pen;
        }

    }
}
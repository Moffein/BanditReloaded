using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BanditReloaded
{
    public class DisplaySetup
    {
        public static void DisplayRules(GameObject BanditBody)
        {
            ItemDisplayRuleSet idrsCommando = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;
            ItemDisplayRuleSet.NamedRuleGroup[] ersCommando = idrsCommando.GetFieldValue<ItemDisplayRuleSet.NamedRuleGroup[]>("namedEquipmentRuleGroups");
            GameObject ggPrefab = null;
            GameObject bsPrefab = null;
            GameObject vbPrefab = null;
            GameObject sawPrefab = null;
            GameObject proohPrefab = null;
            GameObject recyclerPrefab = null;
            GameObject eggPrefab = null;
            GameObject vasePrefab = null;
            GameObject radarPrefab = null;
            GameObject meteorPrefab = null;
            GameObject helPrefab = null;
            GameObject tonicPrefab = null;
            GameObject effPrefab = null;
            GameObject malPrefab = null;
            GameObject spectPrefab = null;
            GameObject leechPrefab = null;
            GameObject opusPrefab = null;
            GameObject forgivePrefab = null;
            for (int i = 0; i < ersCommando.Length; i++)
            {
                if (ersCommando[i].name == "GoldGat")
                {
                    ggPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "Cleanse")
                {
                    bsPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "QuestVolatileBattery")
                {
                    vbPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "GainArmor")
                {
                    proohPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "Recycle")
                {
                    recyclerPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "Saw")
                {
                    sawPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "FireBallDash")
                {
                    eggPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "Gateway")
                {
                    vasePrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "Scanner")
                {
                    radarPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "Meteor")
                {
                    meteorPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "BurnNearby")
                {
                    helPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "Tonic")
                {
                    tonicPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "CrippleWard")
                {
                    effPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "AffixPoison")
                {
                    malPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "AffixHaunted")
                {
                    spectPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "LifestealOnHit")
                {
                    leechPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "TeamWarCry")
                {
                    opusPrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (ersCommando[i].name == "DeathProjectile")
                {
                    forgivePrefab = ersCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                if (false)
                {
                    Debug.Log("\nChildname: " + ersCommando[i].displayRuleGroup.rules[0].childName + "\nScale: " + ersCommando[i].displayRuleGroup.rules[0].localScale + "\nAngles: " + ersCommando[i].displayRuleGroup.rules[0].localAngles + "\nPos: " + ersCommando[i].displayRuleGroup.rules[0].localPos + "\n\n");
                }
            }

            if (false)
            {
                Transform[] bT = BanditBody.GetComponent<ModelLocator>().GetComponentsInChildren<Transform>();
                for (int i = 0; i < bT.Length; i++)
                {
                    Debug.Log(bT[i].name);
                }
            }

            GameObject fcPrefab = null;
            GameObject dmPrefab = null;
            GameObject ghorPrefab = null;
            GameObject quailPrefab = null;
            GameObject genesisPrefab = null;
            GameObject happiestPrefab = null;
            GameObject backupmagPrefab = null;
            GameObject broochPrefab = null;
            GameObject dioPrefab = null;
            GameObject brainPrefab = null;
            GameObject apPrefab = null;
            GameObject visionsPrefab = null;
            GameObject wakePrefab = null;
            GameObject hardlightPrefab = null;
            GameObject nrgPrefab = null;
            GameObject stickyPrefab = null;
            GameObject rustyPrefab = null;
            GameObject rapPrefab = null;
            GameObject meatPrefab = null;
            GameObject squidPrefab = null;
            GameObject rejuvPrefab = null;
            GameObject aegisPrefab = null;
            GameObject shatterPrefab = null;
            GameObject deskPrefab = null;
            GameObject convPrefab = null;
            GameObject beadPrefab = null;
            GameObject rosePrefab = null;
            GameObject iceringPrefab = null;
            GameObject fireringPrefab = null;
            GameObject chronoPrefab = null;
            GameObject guilPrefab = null;
            GameObject hornPrefab = null;
            GameObject daisyPrefab = null;
            GameObject razorPrefab = null;
            GameObject pearlPrefab = null;
            GameObject ipearlPrefab = null;
            GameObject halcPrefab = null;
            GameObject disciplePrefab = null;
            GameObject stridesPrefab = null;
            GameObject gesturePrefab = null;
            GameObject cbPrefab = null;
            GameObject gougePrefab = null;
            GameObject rachisPrefab = null;
            GameObject perfPrefab = null;
            GameObject spleenPrefab = null;
            GameObject urnPrefab = null;
            GameObject incubPrefab = null;
            ItemDisplayRuleSet.NamedRuleGroup[] irsCommando = idrsCommando.GetFieldValue<ItemDisplayRuleSet.NamedRuleGroup[]>("namedItemRuleGroups");
            for (int i = 0; i < irsCommando.Length; i++)
            {
                if (irsCommando[i].name == "SprintBonus")
                {
                    nrgPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "StickyBomb")
                {
                    stickyPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "TreasureCache")
                {
                    rustyPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "NearbyDamageBonus")
                {
                    fcPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "DeathMark")
                {
                    dmPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "BonusGoldPackOnKill")
                {
                    ghorPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "JumpBoost")
                {
                    quailPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "NovaOnLowHealth")
                {
                    genesisPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "GhostOnKill")
                {
                    happiestPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "SecondarySkillMagazine")
                {
                    backupmagPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "BarrierOnKill")
                {
                    broochPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "ExtraLife")
                {
                    dioPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "KillEliteFrenzy")
                {
                    brainPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "BossDamageBonus")
                {
                    apPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "LunarPrimaryReplacement")
                {
                    visionsPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "HeadHunter")
                {
                    wakePrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "UtilitySkillMagazine")
                {
                    hardlightPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "RegenOnKill")
                {
                    meatPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "ArmorPlate")
                {
                    rapPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "Squid")
                {
                    squidPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "IncreaseHealing")
                {
                    rejuvPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "BarrierOnOverHeal")
                {
                    aegisPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "ArmorReductionOnHit")
                {
                    shatterPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "Plant")
                {
                    deskPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "FocusConvergence")
                {
                    convPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "LunarTrinket")
                {
                    beadPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "SprintArmor")
                {
                    rosePrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "IceRing")
                {
                    iceringPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "FireRing")
                {
                    fireringPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "SlowOnHit")
                {
                    chronoPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "ExecuteLowHealthElite")
                {
                    guilPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "EnergizedOnEquipmentUse")
                {
                    hornPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "TPHealingNova")
                {
                    daisyPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "Thorns")
                {
                    razorPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "Pearl")
                {
                    pearlPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "ShinyPearl")
                {
                    ipearlPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "TitanGoldDuringTP")
                {
                    halcPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "SprintWisp")
                {
                    disciplePrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "LunarUtilityReplacement")
                {
                    stridesPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "AutoCastEquipment")
                {
                    gesturePrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "AutoCastEquipment")
                {
                    gesturePrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "RepeatHeal")
                {
                    cbPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "MonstersOnShrineUse")
                {
                    gougePrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "RandomDamageZone")
                {
                    rachisPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "FireballsOnHit")
                {
                    perfPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "BleedOnHitAndExplode")
                {
                    spleenPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "SiphonOnLowHealth")
                {
                    urnPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                else if (irsCommando[i].name == "Incubator")
                {
                    incubPrefab = irsCommando[i].displayRuleGroup.rules[0].followerPrefab;
                }
                if (false)
                {
                    Debug.Log("\nChildname: " + irsCommando[i].displayRuleGroup.rules[0].childName + "\nScale: " + irsCommando[i].displayRuleGroup.rules[0].localScale + "\nAngles: " + irsCommando[i].displayRuleGroup.rules[0].localAngles + "\nPos: " + irsCommando[i].displayRuleGroup.rules[0].localPos + "\n\n");
                }
            }
            ItemDisplayRuleSet idrsBandit = BanditBody.GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;
            List<ItemDisplayRuleSet.NamedRuleGroup> equipmentList = new List<ItemDisplayRuleSet.NamedRuleGroup>();
            List<ItemDisplayRuleSet.NamedRuleGroup> itemList = new List<ItemDisplayRuleSet.NamedRuleGroup>();

            for (int i = 0; i < idrsBandit.namedItemRuleGroups.Length; i++)
            {
                if (idrsBandit.namedItemRuleGroups[i].name != "GhostOnKill")
                {
                    itemList.Add(idrsBandit.namedItemRuleGroups[i]);
                }
            }

            for (int i = 0; i < idrsBandit.namedEquipmentRuleGroups.Length; i++)
            {
                equipmentList.Add(idrsBandit.namedItemRuleGroups[i]);
            }

            #region completed
            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "IceRing",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = iceringPrefab,
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(0.0f, 0.02f, -0.06f),
                            localAngles = new Vector3(0f, 0f, 0f),
                            localScale = 0.45f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "FireRing",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = fireringPrefab,
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(0.0f, 0.02f, -0.14f),
                            localAngles = new Vector3(0f, 0f, 0f),
                            localScale = 0.45f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "LifestealOnHit",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = leechPrefab,
                            childName = "Head",
                            localPos = new Vector3(0.25f, 0.22f, 0f),
                            localAngles = new Vector3(20.8f, 255.8f, 68.7f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "TeamWarCry",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = opusPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.07f, -0.25f),
                            localAngles = new Vector3(20f, 180f, 0f),
                            localScale = 0.08f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "DeathProjectile",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = forgivePrefab,
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(0f, 0f, 0f),
                            localAngles = new Vector3(0f, 180f, 0f),
                            localScale = 0.15f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Incubator",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = incubPrefab,
                            childName = "LowerArmR",
                            localPos = new Vector3(0f, 0.2f, -0.05f),
                            localAngles = new Vector3(270.0f, 328.4f, 0.0f),
                            localScale = 0.02f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "SiphonOnLowHealth",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = urnPrefab,
                            childName = "ThighL",
                            localPos = new Vector3(0f, 0.2f, 0.2f),
                            localAngles = new Vector3(358.0f, 358.7f, 172.4f),
                            localScale = 0.07f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "BleedOnHitAndExplode",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = spleenPrefab,
                            childName = "ThighR",
                            localPos = new Vector3(0f, 0.1f, 0.15f),
                            localAngles = new Vector3(14.1f, 42.4f, 176.0f),
                            localScale = 0.05f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "FireballsOnHit",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = perfPrefab,
                            childName = "LowerArmR",
                            localPos = new Vector3(0f, 0.2f, -0.1f),
                            localAngles = new Vector3(273.9f, 148.4f, 180.0f),
                            localScale = 0.05f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "RandomDamageZone",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = rachisPrefab,
                            childName = "LowerArmR",
                            localPos = new Vector3(0f, 0.2f, -0.1f),
                            localAngles = new Vector3(2.6f, 13.8f, 273.2f),
                            localScale = 0.03f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "MonsterOnShrineUse",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = gougePrefab,
                            childName = "ThighL",
                            localPos = new Vector3(0f, 0.3f, 0.1f),
                            localAngles = new Vector3(21.8f, 309.1f, 27.9f),
                            localScale = 0.06f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "AffixPoison",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = malPrefab,
                            childName = "Head",
                            localPos = new Vector3(0f, 0.3f, -0.1f),
                            localAngles = new Vector3(255f, 0f, 0f),
                            localScale = 0.05f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "AffixHaunted",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = spectPrefab,
                            childName = "Head",
                            localPos = new Vector3(0f, 0.35f, -0.1f),
                            localAngles = new Vector3(245f, 0f, 0f),
                            localScale = 0.06f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "RepeatHeal",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = cbPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.2f, 0.18f),
                            localAngles = new Vector3(90f, 180f, 180f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "AutoCastEquipment",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = gesturePrefab,
                            childName = "Stomach",
                            localPos = new Vector3(-0.1f, 0f, 0.21f),
                            localAngles = new Vector3(0f, 75f, 0f),
                            localScale = 0.6f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "LunarUtilityReplacement",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = stridesPrefab,
                            childName = "Head",
                            localPos = new Vector3(0f, 0.35f, -0.23f),
                            localAngles = new Vector3(1.1f, 267.8f, 337.9f),
                            localScale = 0.5f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "SprintWisp",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = disciplePrefab,
                            childName = "Chest",
                            localPos = new Vector3(0.3f, 0.35f, -0.1f),
                            localAngles = new Vector3(332.4f, 105.9f, 310.1f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "TitanGoldDuringTP",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = halcPrefab,
                            childName = "Stomach",
                            localPos = new Vector3(0.1f, 0f, 0.2f),
                            localAngles = new Vector3(2.2f, 331.8f, 312.8f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Pearl",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                   {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = pearlPrefab,
                            childName = "LowerArmL",
                            localPos = new Vector3(0, 0.2f, 0),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                   }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ShinyPearl",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ipearlPrefab,
                            childName = "LowerArmR",
                            localPos = new Vector3(0, 0.2f, 0),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Thorns",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = razorPrefab,
                            childName = "UpperArmR",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "TPHealingNova",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = daisyPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0.08f, 0.25f, 0.15f),
                            localAngles = new Vector3(0f, 0f, 0f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "EnergizedOnEquipmentUse",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = hornPrefab,
                            childName = "Stomach",
                            localPos = new Vector3(0.3f, 0f, 0f),
                            localAngles = new Vector3(3.2f, 270.0f, 92.3f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ExecuteLowHealthElite",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = guilPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0.0f, 0.18f, -0.25f),
                            localAngles = new Vector3(-90f, 0f, 0f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "SlowOnHit",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = chronoPrefab,
                            childName = "ThighL",
                            localPos = new Vector3(-0.15f, 0.5f, 0f),
                            localAngles = new Vector3(11.4f, 39.7f, 137.1f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "SprintArmor",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = rosePrefab,
                            childName = "LowerArmL",
                            localPos = new Vector3(0.0f, 0.2f, 0.0f),
                            localAngles = new Vector3(356.3f, 192.8f, 91.2f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "LunarTrinket",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = beadPrefab,
                            childName = "HandR",
                            localPos = new Vector3(0.0f, 0.1f, 0.0f),
                            localAngles = new Vector3(15.0f, 270.0f, 270.0f),
                            localScale = 0.7f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "FocusConvergence",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = convPrefab,
                            childName = "Base",
                            localPos = new Vector3(0.3f, -0.2f, -1.3f),
                            localAngles = new Vector3(270.0f, 0.3f, 0.0f),
                            localScale = 0.07f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "CrippleWard",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = effPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0.3f, -0.2f, -0.3f),
                            localAngles = new Vector3(0f, 20.0f, 90.0f),
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Tonic",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = tonicPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.1f, -0.2f),
                            localAngles = new Vector3(-10.0f, 0.0f, 0.0f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Meteor",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = meteorPrefab,
                            childName = "Chest",
                            localPos = new Vector3(-0.6f, 0.0f, -1.0f),
                            localAngles = new Vector3(270.0f, 0.3f, 0.0f),
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "BurnNearby",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = helPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.2f, -0.2f),
                            localAngles = new Vector3(0.0f, 0.0f, 0.0f),
                            localScale = 0.05f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Scanner",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = radarPrefab,
                            childName = "Chest",
                            localPos = new Vector3(-0.35f, 0.25f, -0.05f),
                            localAngles = new Vector3(285.1f, 224.5f, 136.5f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Gateway",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = vasePrefab,
                            childName = "Chest",
                            localPos = new Vector3(0.0f, 0.3f, -0.2f),
                            localAngles = new Vector3(297.8f, 2.7f, 2.5f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "FireBallDash",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = eggPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.2f, -0.2f),
                            localAngles = new Vector3(180f + 70f, 268.9f, 268.8f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Recycle",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = recyclerPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.2f, -0.2f),
                            localAngles = new Vector3(0f, 90f, 0f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "GainArmor",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = proohPrefab,
                            childName = "CalfL",
                            localPos = new Vector3(-0.15f, 0.3f, 0f),
                            localAngles = new Vector3(73.1f, 332.0f, 63.1f),
                            localScale = 0.8f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Plant",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = deskPrefab,
                            childName = "ThighR",
                            localPos = new Vector3(-0.1f, 0.3f, 0.1f),
                            localAngles = new Vector3(67.4f, 329.0f, 164.1f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ArmorReductionOnHit",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = shatterPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0.0f, 0.25f, -0.22f),
                            localAngles = new Vector3(276.7f, 0.0f, 0.0f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "BarrierOnOverHeal",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = aegisPrefab,
                            childName = "LowerArmL",
                            localPos = new Vector3(0f, 0.06f, -0.06f),
                            localAngles = new Vector3(86.2f, 210.9f, 213.0f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "IncreaseHealing",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = rejuvPrefab,
                            childName = "Head",
                            localPos = new Vector3(0.1f, 0.3f, 0f),
                            localAngles = new Vector3(358.0f, 93.2f, 1.4f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = rejuvPrefab,
                            childName = "Head",
                            localPos = new Vector3(-0.1f, 0.3f, 0f),
                            localAngles = new Vector3(358.0f, -93.2f, 1.4f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Squid",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = squidPrefab,
                            childName = "ThighR",
                            localPos = new Vector3(0.1f, 0.3f, 0.1f),
                            localAngles = new Vector3(8.3f, 127.6f, 103.5f),
                            localScale = 0.07f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "RegenOnKill",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = meatPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.1f, 0.2f),
                            localAngles = new Vector3(-30f, 0f, 0f),
                            localScale = 0.12f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ArmorPlate",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = rapPrefab,
                            childName = "CalfR",
                            localPos = new Vector3(0f, 0.2f, 0f),
                            localAngles = new Vector3(85.4f, 155.5f, 151.7f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "TreasureCache",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = rustyPrefab,
                            childName = "ThighL",
                            localPos = new Vector3(-0.09f, 0.1f, 0.09f),
                            localAngles = new Vector3(0f, 60f, 264.8f),
                            localScale = 1.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "StickyBomb",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = stickyPrefab,
                            childName = "ThighL",
                            localPos = new Vector3(0f, 0.4f, 0.1f),
                            localAngles = new Vector3(7.9f, 180.8f, 7.6f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "SprintBonus",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = nrgPrefab,
                            childName = "ThighR",
                            localPos = new Vector3(0f, 0.2f, 0.15f),
                            localAngles = new Vector3(84.6f, 97.5f, 249.1f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "UtilitySkillMagazine",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = hardlightPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0.14f, 0.25f, -0.07f),
                            localAngles = new Vector3(-110f, 180f, 0f),
                            localScale = 0.8f * Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = hardlightPrefab,
                            childName = "Chest",
                            localPos = new Vector3(-0.14f, 0.25f, -0.07f),
                            localAngles = new Vector3(-110f, 180f, 0f),
                            localScale = 0.8f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "HeadHunter",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = wakePrefab,
                            childName = "Head",
                            localPos = new Vector3(0f, 0.31f, -0.04f),
                            localAngles = new Vector3(-30f, 0f, 0f),
                            localScale = new Vector3(0.38f, 0.13f, 0.13f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "LunarPrimaryReplacement",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = visionsPrefab,
                            childName = "Head",
                            localPos = new Vector3(0f, 0.21f, 0.08f),
                            localAngles = new Vector3(270f, 0f, 0f),
                            localScale = 0.18f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "BossDamageBonus",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = apPrefab,
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(-0.05f, -0.1f, -0.7f),
                            localAngles = new Vector3(90f, 90f, 0f),
                            localScale = 0.7f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "KillEliteFrenzy",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = brainPrefab,
                            childName = "Head",
                            localPos = new Vector3(0f, 0.2f, -0.05f),
                            localAngles = Vector3.zero,
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "ExtraLife",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = dioPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.25f, -0.25f),
                            localAngles = new Vector3(0f, 180f, 0f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Saw",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = sawPrefab,
                            childName = "Chest",
                            localPos = new Vector3(-0.6f, 0.9f, 0.8f),
                            localAngles = new Vector3(84.9f, 179.6f, 180f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "BarrierOnKill",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = broochPrefab,
                            childName = "Chest",
                            localPos = new Vector3(-0.05f, 0.2f, 0.18f),
                            localAngles = new Vector3(90f, 0f, 0f),
                            localScale = 0.7f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "GoldGat",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ggPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0.37f, 0.63f, -0.32f),
                            localAngles = new Vector3(30f, 130f, -15f),
                            localScale = new Vector3(0.2f, 0.2f, 0.2f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "QuestVolatileBattery",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = vbPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.1f, -0.25f),
                            localAngles = new Vector3(15f, 180f, 0f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });


            equipmentList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "Cleanse",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = bsPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.15f, -0.15f),
                            localAngles = new Vector3(7f, 180f, 0f),
                            localScale = new Vector3(0.2f, 0.2f, 0.2f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "NearbyDamageBonus",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = fcPrefab,
                            childName = "HandL",
                            localPos = new Vector3(0.1f, 0f, 0f),
                            localAngles = Vector3.zero,
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "BonusGoldPackOnKill",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ghorPrefab,
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.1f, -0.25f),
                            localAngles = new Vector3(15f, 180f, 0f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "JumpBoost",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = quailPrefab,
                            childName = "Head",
                            localPos = new Vector3(0f, -0.25f, -0.3f),
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "NovaOnLowHealth",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = genesisPrefab,
                            childName = "Head",
                            localPos = new Vector3(-0.1f, 0.25f, -0.1f),
                            localAngles = new Vector3(-90f, 0f, 0f),
                            localScale = 0.18f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "GhostOnKill",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = happiestPrefab,
                            childName = "Head",
                            localPos = new Vector3(0f, 0.2f, 0.05f),
                            localAngles = new Vector3(0f, 0f, 0f),
                            localScale = 0.6f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "SecondarySkillMagazine",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = backupmagPrefab,
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(0.02f, -0.3f, -0.6f),
                            localAngles = Vector3.zero,
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemList.Add(new ItemDisplayRuleSet.NamedRuleGroup
            {
                name = "DeathMark",
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = dmPrefab,
                            childName = "Head",
                            localPos = new Vector3(0f, 0.22f, -0.02f),
                            localAngles = new Vector3(280f, 180f, 180f),
                            localScale = 0.075f * Vector3.one,
                            limbMask = LimbFlags.Head
                        }
                    }
                }
            });
            #endregion

            ItemDisplayRuleSet.NamedRuleGroup[] equipmentArray = equipmentList.ToArray();
            ItemDisplayRuleSet.NamedRuleGroup[] itemArray = itemList.ToArray();
            idrsBandit.namedEquipmentRuleGroups = equipmentArray;
            idrsBandit.namedItemRuleGroups = itemArray;
        }
    }
}

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
            PopulateDisplays();
            
            CharacterModel characterModel = BanditBody.GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>();
            ItemDisplayRuleSet idrsBandit = characterModel.itemDisplayRuleSet;

            ItemDisplayRuleSet itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            for (int i = 0; i < idrsBandit.keyAssetRuleGroups.Length; i++)
            {
                if (idrsBandit.keyAssetRuleGroups[i].keyAsset != RoR2Content.Items.GhostOnKill)
                {
                    itemDisplayRules.Add(idrsBandit.keyAssetRuleGroups[i]);
                }
            }

            #region completed
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IceRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayIceRing"),
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(0.0f, 0.02f, -0.06f),
                            localAngles = new Vector3(0f, 0f, 0f),
                            localScale = 0.45f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayFireRing"),
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(0.0f, 0.02f, -0.14f),
                            localAngles = new Vector3(0f, 0f, 0f),
                            localScale = 0.45f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.LifestealOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab =  LoadDisplay("DisplayLifestealOnHit"),
                            childName = "Head",
                            localPos = new Vector3(0.25f, 0.22f, 0f),
                            localAngles = new Vector3(20.8f, 255.8f, 68.7f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.TeamWarCry,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayTeamWarCry"),
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.07f, -0.25f),
                            localAngles = new Vector3(20f, 180f, 0f),
                            localScale = 0.08f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.DeathProjectile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayDeathProjectile"),
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(0f, 0f, 0f),
                            localAngles = new Vector3(0f, 180f, 0f),
                            localScale = 0.15f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup{
                keyAsset = RoR2Content.Items.Incubator,displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayAncestralIncubator"),
                            childName = "LowerArmR",
                            localPos = new Vector3(0f, 0.2f, -0.05f),
                            localAngles = new Vector3(270.0f, 328.4f, 0.0f),
                            localScale = 0.02f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SiphonOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplaySiphonOnLowHealth"),
                            childName = "ThighL",
                            localPos = new Vector3(0f, 0.2f, 0.2f),
                            localAngles = new Vector3(358.0f, 358.7f, 172.4f),
                            localScale = 0.07f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHitAndExplode,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBleedOnHitAndExplode"),
                            childName = "ThighR",
                            localPos = new Vector3(0f, 0.1f, 0.15f),
                            localAngles = new Vector3(14.1f, 42.4f, 176.0f),
                            localScale = 0.05f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireballsOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayFireballsOnHit"),
                            childName = "LowerArmR",
                            localPos = new Vector3(0f, 0.2f, -0.1f),
                            localAngles = new Vector3(273.9f, 148.4f, 180.0f),
                            localScale = 0.05f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RandomDamageZone,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayRandomDamageZone"),
                            childName = "LowerArmR",
                            localPos = new Vector3(0f, 0.2f, -0.1f),
                            localAngles = new Vector3(2.6f, 13.8f, 273.2f),
                            localScale = 0.03f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.MonstersOnShrineUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayMonstersOnShrineUse"),
                            childName = "ThighL",
                            localPos = new Vector3(0f, 0.3f, 0.1f),
                            localAngles = new Vector3(21.8f, 309.1f, 27.9f),
                            localScale = 0.06f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixPoison,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayEliteUrchinCrown"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.3f, -0.1f),
                            localAngles = new Vector3(255f, 0f, 0f),
                            localScale = 0.05f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixHaunted,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayEliteStealthCrown"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.35f, -0.1f),
                            localAngles = new Vector3(245f, 0f, 0f),
                            localScale = 0.06f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RepeatHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayCorpseFlower"),
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.2f, 0.18f),
                            localAngles = new Vector3(90f, 180f, 180f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AutoCastEquipment,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayFossil"),
                            childName = "Stomach",
                            localPos = new Vector3(-0.1f, 0f, 0.21f),
                            localAngles = new Vector3(0f, 75f, 0f),
                            localScale = 0.6f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarUtilityReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBirdFoot"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.35f, -0.23f),
                            localAngles = new Vector3(1.1f, 267.8f, 337.9f),
                            localScale = 0.5f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintWisp,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBrokenMask"),
                            childName = "Chest",
                            localPos = new Vector3(0.3f, 0.35f, -0.1f),
                            localAngles = new Vector3(332.4f, 105.9f, 310.1f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TitanGoldDuringTP,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayGoldHeart"),
                            childName = "Stomach",
                            localPos = new Vector3(0.1f, 0f, 0.2f),
                            localAngles = new Vector3(2.2f, 331.8f, 312.8f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Pearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayPearl"),
                            childName = "LowerArmL",
                            localPos = new Vector3(0, 0.2f, 0),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                   }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShinyPearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayShinyPearl"),
                            childName = "LowerArmR",
                            localPos = new Vector3(0, 0.2f, 0),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Thorns,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayRazorwireLeft"),
                            childName = "UpperArmR",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TPHealingNova,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayGlowFlower"),
                            childName = "Chest",
                            localPos = new Vector3(0.08f, 0.25f, 0.15f),
                            localAngles = new Vector3(0f, 0f, 0f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EnergizedOnEquipmentUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayWarHorn"),
                            childName = "Stomach",
                            localPos = new Vector3(0.3f, 0f, 0f),
                            localAngles = new Vector3(3.2f, 270.0f, 92.3f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayGuillotine"),
                            childName = "Chest",
                            localPos = new Vector3(0.0f, 0.18f, -0.25f),
                            localAngles = new Vector3(-90f, 0f, 0f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SlowOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBauble"),
                            childName = "ThighL",
                            localPos = new Vector3(-0.15f, 0.5f, 0f),
                            localAngles = new Vector3(11.4f, 39.7f, 137.1f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBuckler"),
                            childName = "LowerArmL",
                            localPos = new Vector3(0.0f, 0.2f, 0.0f),
                            localAngles = new Vector3(356.3f, 192.8f, 91.2f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarTrinket,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBeads"),
                            childName = "HandR",
                            localPos = new Vector3(0.0f, 0.1f, 0.0f),
                            localAngles = new Vector3(15.0f, 270.0f, 270.0f),
                            localScale = 0.7f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FocusConvergence,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayFocusedConvergence"),
                            childName = "Base",
                            localPos = new Vector3(0.3f, -0.2f, -1.3f),
                            localAngles = new Vector3(270.0f, 0.3f, 0.0f),
                            localScale = 0.07f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CrippleWard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayEffigy"),
                            childName = "Chest",
                            localPos = new Vector3(0.3f, -0.2f, -0.3f),
                            localAngles = new Vector3(0f, 20.0f, 90.0f),
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Tonic,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayTonic"),
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.1f, -0.2f),
                            localAngles = new Vector3(-10.0f, 0.0f, 0.0f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Meteor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayMeteor"),
                            childName = "Chest",
                            localPos = new Vector3(-0.6f, 0.0f, -1.0f),
                            localAngles = new Vector3(270.0f, 0.3f, 0.0f),
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.BurnNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayPotion"),
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.2f, -0.2f),
                            localAngles = new Vector3(0.0f, 0.0f, 0.0f),
                            localScale = 0.05f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Scanner,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayScanner"),
                            childName = "Chest",
                            localPos = new Vector3(-0.35f, 0.25f, -0.05f),
                            localAngles = new Vector3(285.1f, 224.5f, 136.5f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Gateway,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayVase"),
                            childName = "Chest",
                            localPos = new Vector3(0.0f, 0.3f, -0.2f),
                            localAngles = new Vector3(297.8f, 2.7f, 2.5f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.FireBallDash,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayEgg"),
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.2f, -0.2f),
                            localAngles = new Vector3(180f + 70f, 268.9f, 268.8f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Recycle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayRecycler"),
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.2f, -0.2f),
                            localAngles = new Vector3(0f, 90f, 0f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GainArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayElephantFigure"),
                            childName = "CalfL",
                            localPos = new Vector3(-0.15f, 0.3f, 0f),
                            localAngles = new Vector3(73.1f, 332.0f, 63.1f),
                            localScale = 0.8f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Plant,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayInterstellarDeskPlant"),
                            childName = "ThighR",
                            localPos = new Vector3(-0.1f, 0.3f, 0.1f),
                            localAngles = new Vector3(67.4f, 329.0f, 164.1f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorReductionOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayWarhammer"),
                            childName = "Chest",
                            localPos = new Vector3(0.0f, 0.25f, -0.22f),
                            localAngles = new Vector3(276.7f, 0.0f, 0.0f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnOverHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayAegis"),
                            childName = "LowerArmL",
                            localPos = new Vector3(0f, 0.06f, -0.06f),
                            localAngles = new Vector3(86.2f, 210.9f, 213.0f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IncreaseHealing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayAntler"),
                            childName = "Head",
                            localPos = new Vector3(0.1f, 0.3f, 0f),
                            localAngles = new Vector3(358.0f, 93.2f, 1.4f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayAntler"),
                            childName = "Head",
                            localPos = new Vector3(-0.1f, 0.3f, 0f),
                            localAngles = new Vector3(358.0f, -93.2f, 1.4f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Squid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplaySquidTurret"),
                            childName = "ThighR",
                            localPos = new Vector3(0.1f, 0.3f, 0.1f),
                            localAngles = new Vector3(8.3f, 127.6f, 103.5f),
                            localScale = 0.07f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FlatHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplaySteakCurved"),
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.1f, 0.2f),
                            localAngles = new Vector3(-30f, 0f, 0f),
                            localScale = 0.12f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorPlate,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayRepulsionArmorPlate"),
                            childName = "CalfR",
                            localPos = new Vector3(0f, 0.2f, 0f),
                            localAngles = new Vector3(85.4f, 155.5f, 151.7f),
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TreasureCache,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayKey"),
                            childName = "ThighL",
                            localPos = new Vector3(-0.09f, 0.1f, 0.09f),
                            localAngles = new Vector3(0f, 60f, 264.8f),
                            localScale = 1.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StickyBomb,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayStickyBomb"),
                            childName = "ThighL",
                            localPos = new Vector3(0f, 0.4f, 0.1f),
                            localAngles = new Vector3(7.9f, 180.8f, 7.6f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplaySoda"),
                            childName = "ThighR",
                            localPos = new Vector3(0f, 0.2f, 0.15f),
                            localAngles = new Vector3(84.6f, 97.5f, 249.1f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.UtilitySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayAfterburnerShoulderRing"),
                            childName = "Chest",
                            localPos = new Vector3(0.14f, 0.25f, -0.07f),
                            localAngles = new Vector3(-110f, 180f, 0f),
                            localScale = 0.8f * Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = LoadDisplay("DisplayAfterburnerShoulderRing"),
                            childName = "Chest",
                            localPos = new Vector3(-0.14f, 0.25f, -0.07f),
                            localAngles = new Vector3(-110f, 180f, 0f),
                            localScale = 0.8f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HeadHunter,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplaySkullCrown"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.31f, -0.04f),
                            localAngles = new Vector3(-30f, 0f, 0f),
                            localScale = new Vector3(0.38f, 0.13f, 0.13f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBirdEye"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.21f, 0.08f),
                            localAngles = new Vector3(270f, 0f, 0f),
                            localScale = 0.18f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BossDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayAPRound"),
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(-0.05f, -0.1f, -0.7f),
                            localAngles = new Vector3(90f, 90f, 0f),
                            localScale = 0.7f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.KillEliteFrenzy,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBrainstalk"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.2f, -0.05f),
                            localAngles = Vector3.zero,
                            localScale = 0.3f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExtraLife,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayHippo"),
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.25f, -0.25f),
                            localAngles = new Vector3(0f, 180f, 0f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Saw,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplaySawmerang"),
                            childName = "Chest",
                            localPos = new Vector3(-0.6f, 0.9f, 0.8f),
                            localAngles = new Vector3(84.9f, 179.6f, 180f),
                            localScale = 0.2f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBrooch"),
                            childName = "Chest",
                            localPos = new Vector3(-0.05f, 0.2f, 0.18f),
                            localAngles = new Vector3(90f, 0f, 0f),
                            localScale = 0.7f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GoldGat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayGoldGat"),
                            childName = "Chest",
                            localPos = new Vector3(0.37f, 0.63f, -0.32f),
                            localAngles = new Vector3(30f, 130f, -15f),
                            localScale = new Vector3(0.2f, 0.2f, 0.2f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.QuestVolatileBattery,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayBatteryArray"),
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.1f, -0.25f),
                            localAngles = new Vector3(15f, 180f, 0f),
                            localScale = 0.4f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });


            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Cleanse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayWaterPack"),
                            childName = "Chest",
                            localPos = new Vector3(0f, -0.15f, -0.15f),
                            localAngles = new Vector3(7f, 180f, 0f),
                            localScale = new Vector3(0.2f, 0.2f, 0.2f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NearbyDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayDiamond"),
                            childName = "HandL",
                            localPos = new Vector3(0.1f, 0f, 0f),
                            localAngles = Vector3.zero,
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BonusGoldPackOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayTome"),
                            childName = "Chest",
                            localPos = new Vector3(0f, 0.1f, -0.25f),
                            localAngles = new Vector3(15f, 180f, 0f),
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.JumpBoost,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayWaxBird"),
                            childName = "Head",
                            localPos = new Vector3(0f, -0.25f, -0.3f),
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayJellyGuts"),
                            childName = "Head",
                            localPos = new Vector3(-0.1f, 0.25f, -0.1f),
                            localAngles = new Vector3(-90f, 0f, 0f),
                            localScale = 0.18f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GhostOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayMask"),
                            childName = "Head",
                            localPos = new Vector3(0f, 0.2f, 0.05f),
                            localAngles = new Vector3(0f, 0f, 0f),
                            localScale = 0.6f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SecondarySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayDoubleMag"),
                            childName = "MuzzleShotgun",
                            localPos = new Vector3(0.02f, -0.3f, -0.6f),
                            localAngles = Vector3.zero,
                            localScale = 0.1f * Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.DeathMark,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]{new ItemDisplayRule{ruleType = ItemDisplayRuleType.ParentedPrefab,followerPrefab = LoadDisplay("DisplayDeathMark"),
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

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
            characterModel.itemDisplayRuleSet.GenerateRuntimeValues();
        }

        internal static void PopulateDisplays()
        {
            ItemDisplayRuleSet itemDisplayRuleSet = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string name = followerPrefab.name;
                        string key = (name != null) ? name.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        public static GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
            {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }
            Debug.Log("NULL: " + name);
            return null;
        }

        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();
    }
}

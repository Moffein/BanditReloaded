using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.BanditReloaded;
using EntityStates.Engi.EngiWeapon;
using MonoMod;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace BanditReloaded
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.Moffein.BanditReloaded_v2", "Bandit Reloaded v2", "2.3.0")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(SurvivorAPI), nameof(LoadoutAPI), nameof(PrefabAPI), nameof(BuffAPI), nameof(ResourcesAPI), nameof(LanguageAPI), nameof(DotAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    //[NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    class BanditReloaded : BaseUnityPlugin
    {
        #region cfg
        /*private static ConfigEntry<bool> useBodyClone;
        private static ConfigEntry<bool> useBodyClone2;
        private static ConfigEntry<bool> useBodyClone3;
        private static ConfigEntry<bool> useBodyClone4;*/
        bool useBodyClone = true;

        public static SurvivorDef item;

        private static ConfigEntry<bool> usePassive;
        private static ConfigEntry<bool> cloakAnim;
        private static ConfigEntry<bool> vanillaCloak;
        private static ConfigEntry<bool> useAltCrosshair;

        private static ConfigEntry<bool> classicOutro;

        private static ConfigEntry<float> blastDamage;
        private static ConfigEntry<float> blastRange;
        private static ConfigEntry<bool> blastDryEnabled;
        private static ConfigEntry<float> blastDryDuration;
        private static ConfigEntry<float> blastMaxDuration;
        private static ConfigEntry<float> blastMinDuration;
        private static ConfigEntry<float> blastForce;
        private static ConfigEntry<float> blastSpread;
        private static ConfigEntry<int> blastStock;
        private static ConfigEntry<float> blastRechargeInterval;
        //private static ConfigEntry<bool> blastIndividualReload;
        //private static ConfigEntry<float> blastInitialRechargeInterval;
        private static ConfigEntry<bool> blastVanillaBrainstalks;
        private static ConfigEntry<bool> blastPenetrate;
        private static ConfigEntry<bool> blastFalloff;
        private static ConfigEntry<float> blastRadius;
        private static ConfigEntry<float> blastMashSpread;

        private static ConfigEntry<float> thermiteDamage;
        private static ConfigEntry<float> thermiteBurnDuration;
        private static ConfigEntry<float> thermiteRadius;
        private static ConfigEntry<float> thermiteFireRate;
        private static ConfigEntry<float> thermiteProcCoefficient;
        private static ConfigEntry<float> thermiteVelocity;
        private static ConfigEntry<float> thermiteLifetime;
        private static ConfigEntry<float> thermiteCooldown;
        private static ConfigEntry<float> thermiteBurnDamageMult;
        private static ConfigEntry<int> thermiteStock;
        private static ConfigEntry<bool> thermiteGravity;

        private static ConfigEntry<float> cloakDamage;
        private static ConfigEntry<float> cloakDuration;
        private static ConfigEntry<float> cloakMinDuration;
        private static ConfigEntry<float> cloakRadius;
        private static ConfigEntry<float> cloakCooldown;
        private static ConfigEntry<int> cloakStock;

        private static ConfigEntry<float> loDamage;
        private static ConfigEntry<float> loBuffDamage;
        private static ConfigEntry<float> loGracePeriodMin;
        private static ConfigEntry<float> loGracePeriodMax;
        private static ConfigEntry<float> loForce;
        private static ConfigEntry<float> loFireRate;
        private static ConfigEntry<float> loEndLag;
        private static ConfigEntry<float> loCooldown;
        private static ConfigEntry<float> loExecuteThreshold;
        private static ConfigEntry<bool> loExecuteBosses;
        private static ConfigEntry<int> loStock;

        private static ConfigEntry<float> scatterDamage;
        private static ConfigEntry<uint> scatterPellets;
        private static ConfigEntry<float> scatterForce;
        private static ConfigEntry<float> scatterSpread;
        private static ConfigEntry<float> scatterProcCoefficient;
        private static ConfigEntry<float> scatterMaxDuration;
        private static ConfigEntry<float> scatterMinDuration;
        private static ConfigEntry<bool> scatterDryEnabled;
        private static ConfigEntry<float> scatterDryDuration;
        private static ConfigEntry<int> scatterStock;
        private static ConfigEntry<float> scatterRechargeInterval;
        //private static ConfigEntry<bool> scatterIndividualReload;
        //private static ConfigEntry<float> scatterInitialRechargeInterval;
        private static ConfigEntry<bool> scatterVanillaBrainstalks;
        private static ConfigEntry<bool> scatterPenetrate;
        private static ConfigEntry<float> scatterRange;
        private static ConfigEntry<float> scatterRadius;

        private static ConfigEntry<float> acidDamage;
        private static ConfigEntry<float> acidWeakDuration;
        private static ConfigEntry<float> acidRadius;
        private static ConfigEntry<float> acidFireRate;
        private static ConfigEntry<float> acidProcCoefficient;
        private static ConfigEntry<float> acidVelocity;
        private static ConfigEntry<float> acidLifetime;
        private static ConfigEntry<int> acidStock;
        private static ConfigEntry<float> acidCooldown;
        private static ConfigEntry<bool> acidGravity;

        private static ConfigEntry<float> asMinDamage;
        private static ConfigEntry<float> asMaxDamage;
        private static ConfigEntry<float> asMinRadius;
        private static ConfigEntry<float> asMaxRadius;
        private static ConfigEntry<float> asChargeDuration;
        private static ConfigEntry<float> asMinForce;
        private static ConfigEntry<float> asMaxForce;
        private static ConfigEntry<float> asSelfForceMin;
        private static ConfigEntry<float> asSelfForceMax;
        private static ConfigEntry<float> asEndLag;
        private static ConfigEntry<float> asCooldown;
        private static ConfigEntry<float> asZoom;
        private static ConfigEntry<int> asStock;
        private static ConfigEntry<bool> asEnabled;
        private static ConfigEntry<float> asBarrierPercent;

        private static ConfigEntry<float> cbDamage;
        private static ConfigEntry<float> cbRadius;
        private static ConfigEntry<float> cbProcCoefficient;
        private static ConfigEntry<int> cbBombletCount;
        private static ConfigEntry<float> cbBombletDamage;
        private static ConfigEntry<float> cbBombletRadius;
        private static ConfigEntry<float> cbBombletProcCoefficient;
        private static ConfigEntry<float> cbFireRate;
        private static ConfigEntry<float> cbVelocity;
        private static ConfigEntry<float> cbCooldown;
        private static ConfigEntry<int> cbStock;

        private static ConfigEntry<float> reuDamage;
        private static ConfigEntry<float> reuDebuffDamage;
        private static ConfigEntry<int> reuBullets;
        private static ConfigEntry<float> reuGracePeriodMin;
        private static ConfigEntry<float> reuGracePeriodMax;
        private static ConfigEntry<float> reuForce;
        private static ConfigEntry<float> reuDraw;
        private static ConfigEntry<float> reuFireRate;
        private static ConfigEntry<float> reuEndLag;
        private static ConfigEntry<float> reuCooldown;
        private static ConfigEntry<int> reuStock;
        private static ConfigEntry<float> reuExecuteThreshold;
        private static ConfigEntry<bool> reuExecuteBosses;
        private static ConfigEntry<float> reuSpread;
        private static ConfigEntry<float> reuRange;

        

        #endregion

        public static string outroTextSelected = "";
        public static string outroEasterEgg = "..and so he left, seeking warmth.";

        SkillDef primaryBlastDef, primaryScatterDef, utilityDefA, utilityDefB, utilityAltDef, thermiteDef, acidBombDef, specialLightsOutDef, clusterBombDef, specialBarrageDef, specialBarrageScepterDef, specialLightsOutScepterDef;

        public static GameObject BanditBody = null;
        GameObject BanditMonsterMaster = null;

        GameObject AcidBombObject = null;
        GameObject ThermiteObject = null;
        GameObject ClusterBombObject = null;
        GameObject ClusterBombletObject = null;

        GameObject AcidBombGhostObject = null;
        GameObject ThermiteGhostObject = null;
        GameObject ClusterBombGhostObject = null;
        GameObject ClusterBombletGhostObject = null;

        Color BanditColor = new Color(0.8039216f, 0.482352942f, 0.843137264f);
        String BanditBodyName = "";
        String BanditDesc = "";
        public static BuffIndex lightsOutBuff;
        public static BuffIndex thermiteBuff;
        public static BuffIndex fakeStun;
        public static DotController.DotIndex thermiteDoT;

        Sprite iconSkill1 = null;
        Sprite iconSkill1a = null;
        Sprite iconSkill2 = null;
        Sprite iconSkill2a = null;
        Sprite iconSkill3 = null;
        Sprite iconSkill3a = null;
        Sprite iconSkill4 = null;
        Sprite iconPassive = null;
        Sprite iconClusterBomb = null;
        const String assetPrefix = "@MoffeinBanditReloaded";

        DisplayRuleGroup drg_focuscrystal, drg_deathmark, drg_ghor, drg_quail, drg_genesis, drg_goldgat, drg_blastshower, drg_fuelarray, drg_happiest, drg_backupmag, drg_brooch, drg_dio, drg_sawmerang, drg_brain, drg_ap,
            drg_visions, drg_wake, drg_hardlight, drg_nrg, drg_sticky, drg_key, drg_rap, drg_meat, drg_squid, drg_rejuv, drg_aegis, drg_shatter, drg_desk, drg_prooh, drg_recycler, drg_egg, drg_vase, drg_radar, drg_meteor,
            drg_hel, drg_tonic, drg_effigy, drg_conv, drg_beads, drg_rose, drg_icering, drg_firering, drg_chrono, drg_guil, drg_horn, drg_daisy, drg_razor, drg_pearl, drg_ipearl, drg_halc, drg_disciple, drg_strides,
            drg_gesture, drg_corpsebloom, drg_malachite, drg_celestine, drg_gouge, drg_rachis, drg_perf, drg_spleen, drg_urn, drg_incub, drg_leech, drg_opus, drg_forgive;

        private bool started = false;
        public void Start()
        {
            InitSkills();
        }
        public void Awake()
        {
            SetBanditBody();
            SetIcons();
            SetAttributes();
            InitSkills();
            AssignSkills();
            CreateMaster();
            DisplayRules();
            AddSkin();
            BanditBody.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/survivorpod");
            GameObject banditDisplay = BanditBody.GetComponent<ModelLocator>().modelTransform.gameObject;
            banditDisplay.AddComponent<MenuAnimComponent>();

            BanditDesc += "The Bandit is a hit-and-run survivor who uses dirty tricks to assassinate his targets.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Space out your skill usage to keep firing Blast, or dump them all at once for massive damage!" + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Use grenades to apply debuffs to enemies, boosting the damage of Lights Out." + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Use Smokebomb to either run away or to stun many enemies at once." + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Dealing a killing blow with Lights Out allows you to chain many skills together, allowing for maximum damage AND safety." + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Lights Out deals extra damage for every different debuff on a target. It also gains additional damage if a target has multiple stacks of Bandit's Thermite burn.</color>" + Environment.NewLine + Environment.NewLine;

            SkillCatalog.getAdditionalSkillDefs += delegate (List<SkillDef> list)
            {
                list.Add(primaryBlastDef);
                list.Add(primaryScatterDef);
                list.Add(thermiteDef);
                list.Add(acidBombDef);
                list.Add(utilityDefA);
                list.Add(utilityDefB);
                list.Add(specialBarrageDef);
                list.Add(clusterBombDef);
                //list.Add(utilityAltDef);
                list.Add(specialLightsOutDef);
            };

            string outroNormal = "..and so he left, empty-handed.";
            string outroClassic = "..and so he left, with his pyrrhic plunder.";
            outroTextSelected = classicOutro.Value ? outroClassic : outroNormal;
            LanguageAPI.Add("BANDITRELOADED_OUTRO_FLAVOR", outroTextSelected);
            LanguageAPI.Add("BANDITRELOADED_OUTRO_EASTEREGG_FLAVOR", outroEasterEgg);

            LanguageAPI.Add("KEYWORD_BANDITRELOADED_STACKABLE", "<style=cKeywordName>Stackable</style><style=cSub>The <style=cIsDamage>burn</style> from this ability can be stacked to boost the damage of your <style=cIsUtility>Special</style> multiple times.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_RESET", "<style=cKeywordName>Marked</style><style=cSub>Kill Marked enemies to <style=cIsUtility>reset all skill cooldowns to 0</style>. Enemies with <style=cIsHealth>low HP</style> are Marked longer.</style>");

            LanguageAPI.Add("KEYWORD_BANDITRELOADED_INVIS", "<style=cKeywordName>Invisible</style><style=cSub>Enemies are unable to target you while using this ability.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_EXECUTE", "<style=cKeywordName>Executing</style><style=cSub>The ability <style=cIsHealth>instantly kills</style> enemies with <style=cIsHealth>low HP</style>.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_RAPIDFIRE", "<style=cKeywordName>Rapid-Fire</style><style=cSub>The skill fires faster if you click faster.</style>");

            item = new SurvivorDef
            {
                name = useBodyClone ? "BanditReloaded" : "BanditReloadedVanilla",
                bodyPrefab = BanditBody,
                descriptionToken = BanditDesc,
                displayPrefab = banditDisplay,
                primaryColor = BanditColor,
                unlockableName = "",
                //displayNameToken = BanditBody.GetComponent<CharacterBody>().baseNameToken,
                outroFlavorToken = "BANDITRELOADED_OUTRO_FLAVOR"
            };
            SurvivorAPI.AddSurvivor(item);

            On.RoR2.CameraRigController.OnEnable += (orig, self) =>
            {
                if (RoR2.SceneCatalog.GetSceneDefForCurrentScene().baseSceneName.Equals("lobby"))
                {
                    self.enableFading = false;
                }
                orig(self);
            };

            On.RoR2.BuffCatalog.Init += (orig) =>
            {
                CreateLightsOutBuff();
                orig();
            };

            On.RoR2.ItemDisplayRuleSet.GetEquipmentDisplayRuleGroup += (orig, self, equipmentIndex) =>
            {
                DisplayRuleGroup toReturn = orig(self, equipmentIndex);
                if (self.name == "idrsBandit")
                {
                    switch (equipmentIndex)
                    {
                        case EquipmentIndex.GoldGat:
                            toReturn = drg_goldgat;
                            break;
                        case EquipmentIndex.Cleanse:
                            toReturn = drg_blastshower;
                            break;
                        case EquipmentIndex.QuestVolatileBattery:
                            toReturn = drg_fuelarray;
                            break;
                        case EquipmentIndex.GainArmor:
                            toReturn = drg_prooh;
                            break;
                        case EquipmentIndex.Recycle:
                            toReturn = drg_recycler;
                            break;
                        case EquipmentIndex.Saw:
                            toReturn = drg_sawmerang;
                            break;
                        case EquipmentIndex.FireBallDash:
                            toReturn = drg_egg;
                            break;
                        case EquipmentIndex.Gateway:
                            toReturn = drg_vase;
                            break;
                        case EquipmentIndex.Scanner:
                            toReturn = drg_radar;
                            break;
                        case EquipmentIndex.Meteor:
                            toReturn = drg_meteor;
                            break;
                        case EquipmentIndex.BurnNearby:
                            toReturn = drg_hel;
                            break;
                        case EquipmentIndex.Tonic:
                            toReturn = drg_tonic;
                            break;
                        case EquipmentIndex.CrippleWard:
                            toReturn = drg_effigy;
                            break;
                        case EquipmentIndex.AffixPoison:
                            toReturn = drg_malachite;
                            break;
                        case EquipmentIndex.AffixHaunted:
                            toReturn = drg_celestine;
                            break;
                        case EquipmentIndex.LifestealOnHit:
                            toReturn = drg_leech;
                            break;
                        case EquipmentIndex.TeamWarCry:
                            toReturn = drg_opus;
                            break;
                        case EquipmentIndex.DeathProjectile:
                            toReturn = drg_forgive;
                            break;
                        default:
                            break;
                    }
                }
                return toReturn;
            };

            On.RoR2.ItemDisplayRuleSet.GetItemDisplayRuleGroup += (orig, self, itemIndex) =>
            {
                DisplayRuleGroup toReturn = orig(self, itemIndex);
                if (self.name == "idrsBandit")
                {
                    switch (itemIndex)
                    {
                        case ItemIndex.SprintBonus:
                            toReturn = drg_nrg;
                            break;
                        case ItemIndex.SecondarySkillMagazine:
                            toReturn = drg_backupmag;
                            break;
                        case ItemIndex.StickyBomb:
                            toReturn = drg_sticky;
                            break;
                        case ItemIndex.TreasureCache:
                            toReturn = drg_key;
                            break;
                        case ItemIndex.BossDamageBonus:
                            toReturn = drg_ap;
                            break;
                        case ItemIndex.RegenOnKill:
                            toReturn = drg_meat;
                            break;
                        case ItemIndex.ArmorPlate:
                            toReturn = drg_rap;
                            break;
                        case ItemIndex.SprintArmor:
                            toReturn = drg_rose;
                            break;
                        case ItemIndex.IceRing:
                            toReturn = drg_icering;
                            break;
                        case ItemIndex.FireRing:
                            toReturn = drg_firering;
                            break;
                        case ItemIndex.SlowOnHit:
                            toReturn = drg_chrono;
                            break;
                        case ItemIndex.GhostOnKill:
                            toReturn = drg_happiest;
                            break;
                        case ItemIndex.IncreaseHealing:
                            toReturn = drg_rejuv;
                            break;
                        case ItemIndex.KillEliteFrenzy:
                            toReturn = drg_brain;
                            break;
                        case ItemIndex.NearbyDamageBonus:
                            toReturn = drg_focuscrystal;
                            break;
                        case ItemIndex.BarrierOnKill:
                            toReturn = drg_brooch;
                            break;
                        case ItemIndex.DeathMark:
                            toReturn = drg_deathmark;
                            break;
                        case ItemIndex.BonusGoldPackOnKill:
                            toReturn = drg_ghor;
                            break;
                        case ItemIndex.JumpBoost:
                            toReturn = drg_quail;
                            break;
                        case ItemIndex.ExecuteLowHealthElite:
                            toReturn = drg_guil;
                            break;
                        case ItemIndex.EnergizedOnEquipmentUse:
                            toReturn = drg_horn;
                            break;
                        case ItemIndex.TPHealingNova:
                            toReturn = drg_daisy;
                            break;
                        case ItemIndex.Thorns:
                            toReturn = drg_razor;
                            break;
                        case ItemIndex.Squid:
                            toReturn = drg_squid;
                            break;
                        case ItemIndex.NovaOnLowHealth:
                            toReturn = drg_genesis;
                            break;
                        case ItemIndex.ExtraLife:
                            toReturn = drg_dio;
                            break;
                        case ItemIndex.UtilitySkillMagazine:
                            toReturn = drg_hardlight;
                            break;
                        case ItemIndex.HeadHunter:
                            toReturn = drg_wake;
                            break;
                        case ItemIndex.BarrierOnOverHeal:
                            toReturn = drg_aegis;
                            break;
                        case ItemIndex.ArmorReductionOnHit:
                            toReturn = drg_shatter;
                            break;
                        case ItemIndex.Plant:
                            toReturn = drg_desk;
                            break;
                        case ItemIndex.LunarPrimaryReplacement:
                            toReturn = drg_visions;
                            break;
                        case ItemIndex.LunarUtilityReplacement:
                            toReturn = drg_strides;
                            break;
                        case ItemIndex.FocusConvergence:
                            toReturn = drg_conv;
                            break;
                        case ItemIndex.LunarTrinket:
                            toReturn = drg_beads;
                            break;
                        case ItemIndex.AutoCastEquipment:
                            toReturn = drg_gesture;
                            break;
                        case ItemIndex.RepeatHeal:
                            toReturn = drg_corpsebloom;
                            break;
                        case ItemIndex.Pearl:
                            toReturn = drg_pearl;
                            break;
                        case ItemIndex.ShinyPearl:
                            toReturn = drg_ipearl;
                            break;
                        case ItemIndex.TitanGoldDuringTP:
                            toReturn = drg_halc;
                            break;
                        case ItemIndex.SprintWisp:
                            toReturn = drg_disciple;
                            break;
                        case ItemIndex.MonstersOnShrineUse:
                            toReturn = drg_gouge;
                            break;
                        case ItemIndex.RandomDamageZone:
                            toReturn = drg_rachis;
                            break;
                        case ItemIndex.FireballsOnHit:
                            toReturn = drg_perf;
                            break;
                        case ItemIndex.BleedOnHitAndExplode:
                            toReturn = drg_spleen;
                            break;
                        case ItemIndex.SiphonOnLowHealth:
                            toReturn = drg_urn;
                            break;
                        case ItemIndex.Incubator:
                            toReturn = drg_incub;
                            break;
                        default:
                            break;
                    }
                }
                return toReturn;
            };

            On.RoR2.CharacterModel.UpdateItemDisplay += (orig, self, inv) =>
            {
                orig(self, inv);
                if (self.name == "mdlBandit")
                {
                    if (inv.GetItemCount(ItemIndex.DeathMark) > 0 && inv.GetItemCount(ItemIndex.Talisman) > 0 && inv.GetItemCount(ItemIndex.HealOnCrit) > 0 && inv.GetItemCount(ItemIndex.LunarUtilityReplacement) > 0)
                    {
                        item.outroFlavorToken = "BANDITRELOADED_OUTRO_EASTEREGG_FLAVOR";
                    }
                    else
                    {
                        item.outroFlavorToken = "BANDITRELOADED_OUTRO_FLAVOR";
                    }
                }
            };

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                if (self.HasBuff(fakeStun))
                {
                    self.SetPropertyValue<float>("moveSpeed", self.moveSpeed * 0.5f);
                }
                if (self.HasBuff(thermiteBuff))
                {
                    int tCount = self.GetBuffCount(thermiteBuff);
                    self.SetPropertyValue<float>("moveSpeed", self.moveSpeed * Mathf.Pow(0.9f,tCount));
                }
                int loCount = self.GetBuffCount(lightsOutBuff);
                self.SetPropertyValue<float>("moveSpeed", self.moveSpeed * Mathf.Pow(0.9f, loCount));
            };


            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                bool aliveBeforeHit = self.alive;
                bool applyWeaken = false;
                bool resetCooldownsOnKill = false;
                //float chargePercent = 0f;
                bool isBarrage = false;
                bool isScepterLO = false;

                bool banditAttacker = damageInfo.attacker != null && damageInfo.attacker.name == (BanditBodyName + "(Clone)");

                if (banditAttacker)
                {
                    if ((damageInfo.damageType & DamageType.ResetCooldownsOnKill) > 0)
                    {
                        if (damageInfo.damageType == (DamageType.ResetCooldownsOnKill | DamageType.SlowOnHit))
                        {
                            damageInfo.damageType = DamageType.ResetCooldownsOnKill;
                            isBarrage = true;
                        }
                        else if (damageInfo.damageType == (DamageType.ResetCooldownsOnKill | DamageType.ClayGoo))
                        {
                            damageInfo.damageType = DamageType.ResetCooldownsOnKill;
                            isScepterLO = true;
                        }
                        damageInfo.damageType &= ~DamageType.ResetCooldownsOnKill;

                        resetCooldownsOnKill = true;
                        int debuffCount = 0;
                        BuffDef b;
                        bool isDot;
                        bool isThermite;
                        int loCount = 0;
                        int thermCount = 0;

                        DotController d = DotController.FindDotController(self.gameObject);
                        for (int i = 0; i < BuffCatalog.buffCount; i++)
                        {
                            isDot = false;
                            isThermite = false;
                            b = BuffCatalog.GetBuffDef((BuffIndex)i);

                            switch ((BuffIndex)i)   //hardcoding is bad
                            {
                                case (BuffIndex.OnFire):
                                case (BuffIndex.Bleeding):
                                case (BuffIndex.Poisoned):
                                case (BuffIndex.Blight):
                                    isDot = true;
                                    break;
                                default:
                                    break;
                            };

                            if (b.buffIndex == thermiteBuff)
                            {
                                isThermite = true;
                            }

                            if (b.isDebuff || isDot || isThermite)
                            {
                                if (self.body.HasBuff(b.buffIndex))
                                {
                                    if (b.isDebuff)
                                    {
                                        if (b.buffIndex == lightsOutBuff)
                                        {
                                            loCount = self.body.GetBuffCount(lightsOutBuff);
                                        }
                                        else
                                        {
                                            debuffCount++;
                                        }
                                    }
                                    else if (isThermite)
                                    {
                                        thermCount = self.body.GetBuffCount(thermiteBuff);
                                    }
                                    else
                                    {
                                        debuffCount++;
                                    }

                                }
                            }
                            else if (b.buffIndex == fakeStun && self.body.HasBuff(fakeStun))
                            {
                                debuffCount++;
                            }
                        }

                        SetStateOnHurt ss = self.gameObject.GetComponent<SetStateOnHurt>();

                        if (self.isInFrozenState)
                        {
                            debuffCount++;
                        }
                        else if (ss)
                        {
                            Type state = ss.targetStateMachine.state.GetType();
                            if (state == typeof(EntityStates.StunState) || state == typeof(EntityStates.ShockState))
                            {
                                debuffCount++;
                            }
                        }

                        float buffDamage = 0f;
                        float buffBaseDamage;
                        CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                        if (isBarrage)
                        {
                            buffBaseDamage = attackerBody.damage * FireBarrage.buffDamageCoefficient;
                            
                        }
                        else if (isScepterLO)
                        {
                            buffBaseDamage = attackerBody.damage * FireLightsOutScepter.buffDamageCoefficient;
                        }
                        else
                        {
                            buffBaseDamage = attackerBody.damage * FireLightsOut.buffDamageCoefficient;
                        }
                        buffDamage += buffBaseDamage * debuffCount;
                        buffDamage += buffBaseDamage * thermCount;
                        buffDamage += buffBaseDamage * loCount;
                        damageInfo.damage += buffDamage;

                        BanditTimerComponent btc = self.gameObject.GetComponent<BanditTimerComponent>();
                        if (!btc)
                        {
                            btc = self.gameObject.AddComponent<BanditTimerComponent>();
                        }
                        float loDuration;
                        if (!isBarrage)
                        {
                            loDuration = Mathf.Lerp(FireLightsOut.gracePeriodMin, FireLightsOut.gracePeriodMax, 1f - self.combinedHealthFraction);
                        }
                        else
                        {
                            loDuration = Mathf.Lerp(FireBarrage.gracePeriodMin, FireBarrage.gracePeriodMax, 1f - self.combinedHealthFraction);
                        }
                        if (loDuration > 0f)
                        {
                            self.body.AddTimedBuff(BanditReloaded.lightsOutBuff, loDuration);
                            btc.AddTimer(damageInfo.attacker.GetComponent<SkillLocator>(), loDuration);
                        }
                    }
                    else if (damageInfo.damageType == (DamageType.IgniteOnHit | DamageType.AOE) && damageInfo.damageColorIndex != DamageColorIndex.Item)
                    {
                        damageInfo.damageType = DamageType.AOE;
                        DotController.InflictDot(self.gameObject, damageInfo.attacker, thermiteDoT, ThermiteBomb.debuffDuration, ThermiteBomb.burnDamageMult);
                        self.gameObject.AddComponent<BanditThermiteComponent>();
                    }
                    else if (damageInfo.damageType == (DamageType.WeakOnHit | DamageType.AOE))
                    {
                        damageInfo.damageType = DamageType.AOE;
                        applyWeaken = true;
                    }
                    else if ((damageInfo.damageType & DamageType.Stun1s) > 0)
                    {
                        SetStateOnHurt ss = self.gameObject.GetComponent<SetStateOnHurt>();
                        if (!ss || !ss.canBeStunned)
                        {
                            self.body.AddTimedBuff(fakeStun, 1.5f);
                        }
                        if (asEnabled.Value && damageInfo.damageType == (DamageType.Stun1s))
                        {
                            HealthComponent hc = damageInfo.attacker.GetComponent<HealthComponent>();
                            hc.AddBarrier(damageInfo.procCoefficient * FireChargeShot.barrierPercent * hc.combinedHealth);
                            damageInfo.procCoefficient = 1f;
                        }
                    }
                }

                orig(self, damageInfo);

                if (!self.alive && aliveBeforeHit)
                {
                    BanditTimerComponent btc = self.gameObject.GetComponent<BanditTimerComponent>();
                    if (btc)
                    {
                        btc.ResetCooldowns();
                    }
                }

                if (banditAttacker)
                {
                    if (self.alive)
                    {
                        if (applyWeaken)
                        {
                            self.body.AddTimedBuff(BuffIndex.Weak, GrenadeToss.debuffDuration);
                        }
                        if (resetCooldownsOnKill)
                        {
                            if ((!isBarrage && FireLightsOut.executeThreshold > 0f) || (isBarrage && FireBarrage.executeThreshold > 0f))
                            {
                                if (((self.body.bodyFlags & CharacterBody.BodyFlags.ImmuneToExecutes) == 0 && !self.body.isChampion) || ((!isBarrage && FireLightsOut.executeBosses) || (isBarrage && FireBarrage.executeBosses)))
                                {
                                    float executeThreshold;
                                    if (!isBarrage)
                                    {
                                        executeThreshold = FireLightsOut.executeThreshold;
                                    }
                                    else
                                    {
                                        executeThreshold = FireBarrage.executeThreshold;
                                    }
                                    if (self.body.isElite)
                                    {
                                        executeThreshold += damageInfo.inflictor.GetComponent<CharacterBody>().executeEliteHealthFraction;
                                    }

                                    if (self.alive && (self.combinedHealthFraction < executeThreshold))
                                    {
                                        damageInfo.damage = self.health;
                                        damageInfo.damageType = (DamageType.ResetCooldownsOnKill | DamageType.BypassArmor);
                                        damageInfo.procCoefficient = 0f;
                                        damageInfo.crit = false;
                                        damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                                        orig(self, damageInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            };
            base.StartCoroutine(this.FixIce());
        }

        //Credits to MisterName for the ice explosion fix
        private IEnumerator FixIce()
        {
            for (; ; )
            {
                if (BanditBody != null && BanditBody.GetComponent<SetStateOnHurt>() != null && BanditBody.GetComponent<SetStateOnHurt>().idleStateMachine != null && BanditBody.GetComponent<SetStateOnHurt>().idleStateMachine.Length != 0)
                {
                    BanditBody.GetComponent<SetStateOnHurt>().idleStateMachine[0] = BanditBody.GetComponent<SetStateOnHurt>().idleStateMachine[1];
                    yield return null;
                }
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        private void DisplayRules()
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

            #region completed
            drg_icering = new DisplayRuleGroup();
            ItemDisplayRule banditIceRingRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = iceringPrefab,
                childName = "MuzzleShotgun",
                localPos = new Vector3(0.0f, 0.02f, -0.06f),
                localAngles = new Vector3(0f, 0f, 0f),
                localScale = 0.45f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_icering.AddDisplayRule(banditIceRingRule);
            idrsBandit.SetItemDisplayRuleGroup("IceRing", drg_icering);

            drg_firering = new DisplayRuleGroup();
            ItemDisplayRule banditFireRingRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireringPrefab,
                childName = "MuzzleShotgun",
                localPos = new Vector3(0.0f, 0.02f, -0.14f),
                localAngles = new Vector3(0f, 0f, 0f),
                localScale = 0.45f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_firering.AddDisplayRule(banditFireRingRule);
            idrsBandit.SetItemDisplayRuleGroup("FireRing", drg_firering);

            drg_leech = new DisplayRuleGroup();
            ItemDisplayRule banditLeechRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = leechPrefab,
                childName = "Head",
                localPos = new Vector3(0.25f, 0.22f, 0f),
                localAngles = new Vector3(20.8f, 255.8f, 68.7f),
                localScale = 0.1f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_leech.AddDisplayRule(banditLeechRule);
            idrsBandit.SetItemDisplayRuleGroup("LifestealOnHit", drg_leech);

            drg_opus = new DisplayRuleGroup();
            ItemDisplayRule banditOpusRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = opusPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, -0.07f, -0.25f),
                localAngles = new Vector3(20f, 180f, 0f),
                localScale = 0.08f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_opus.AddDisplayRule(banditOpusRule);
            idrsBandit.SetItemDisplayRuleGroup("TeamWarCry", drg_opus);

            drg_forgive = new DisplayRuleGroup();
            ItemDisplayRule banditForgiveRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = forgivePrefab,
                childName = "MuzzleShotgun",
                localPos = new Vector3(0f, 0f, 0f),
                localAngles = new Vector3(0f, 180f, 0f),
                localScale = 0.15f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_forgive.AddDisplayRule(banditForgiveRule);
            idrsBandit.SetItemDisplayRuleGroup("DeathProjectile", drg_forgive);

            drg_incub = new DisplayRuleGroup();
            ItemDisplayRule banditIncubRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = incubPrefab,
                childName = "LowerArmR",
                localPos = new Vector3(0f, 0.2f, -0.05f),
                localAngles = new Vector3(270.0f, 328.4f, 0.0f),
                localScale = 0.02f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_incub.AddDisplayRule(banditIncubRule);
            idrsBandit.SetItemDisplayRuleGroup("Incubator", drg_incub);

            drg_urn = new DisplayRuleGroup();
            ItemDisplayRule banditUrnRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = urnPrefab,
                childName = "ThighL",
                localPos = new Vector3(0f, 0.2f, 0.2f),
                localAngles = new Vector3(358.0f, 358.7f, 172.4f),
                localScale = 0.07f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_urn.AddDisplayRule(banditUrnRule);
            idrsBandit.SetItemDisplayRuleGroup("SiphonOnLowHealth", drg_urn);

            drg_spleen = new DisplayRuleGroup();
            ItemDisplayRule banditSpleenRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = spleenPrefab,
                childName = "ThighR",
                localPos = new Vector3(0f, 0.1f, 0.15f),
                localAngles = new Vector3(14.1f, 42.4f, 176.0f),
                localScale = 0.05f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_spleen.AddDisplayRule(banditSpleenRule);
            idrsBandit.SetItemDisplayRuleGroup("BleedOnHitAndExplode", drg_spleen);

            drg_perf = new DisplayRuleGroup();
            ItemDisplayRule banditPerfRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = perfPrefab,
                childName = "LowerArmR",
                localPos = new Vector3(0f, 0.2f, -0.1f),
                localAngles = new Vector3(273.9f, 148.4f, 180.0f),
                localScale = 0.05f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_perf.AddDisplayRule(banditPerfRule);
            idrsBandit.SetItemDisplayRuleGroup("FireballsOnHit", drg_perf);

            drg_rachis = new DisplayRuleGroup();
            ItemDisplayRule banditRachisRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = rachisPrefab,
                childName = "LowerArmR",
                localPos = new Vector3(0f, 0.2f, -0.1f),
                localAngles = new Vector3(2.6f, 13.8f, 273.2f),
                localScale = 0.03f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_rachis.AddDisplayRule(banditRachisRule);
            idrsBandit.SetItemDisplayRuleGroup("RandomDamageZone", drg_rachis);

            drg_gouge = new DisplayRuleGroup();
            ItemDisplayRule banditGougeRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = gougePrefab,
                childName = "ThighL",
                localPos = new Vector3(0f, 0.3f, 0.1f),
                localAngles = new Vector3(21.8f, 309.1f, 27.9f),
                localScale = 0.06f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_gouge.AddDisplayRule(banditGougeRule);
            idrsBandit.SetItemDisplayRuleGroup("MonsterOnShrineUse", drg_gouge);

            drg_malachite = new DisplayRuleGroup();
            ItemDisplayRule banditMalRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = malPrefab,
                childName = "Head",
                localPos = new Vector3(0f, 0.3f, -0.1f),
                localAngles = new Vector3(255f, 0f, 0f),
                localScale = 0.05f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_malachite.AddDisplayRule(banditMalRule);
            idrsBandit.SetItemDisplayRuleGroup("AffixPoison", drg_malachite);

            drg_celestine = new DisplayRuleGroup();
            ItemDisplayRule banditCelRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = spectPrefab,
                childName = "Head",
                localPos = new Vector3(0f, 0.35f, -0.1f),
                localAngles = new Vector3(245f, 0f, 0f),
                localScale = 0.06f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_celestine.AddDisplayRule(banditCelRule);
            idrsBandit.SetItemDisplayRuleGroup("AffixHaunted", drg_celestine);

            drg_corpsebloom = new DisplayRuleGroup();
            ItemDisplayRule banditCBRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = cbPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, 0.2f, 0.18f),
                localAngles = new Vector3(90f, 180f, 180f),
                localScale = 0.4f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_corpsebloom.AddDisplayRule(banditCBRule);
            idrsBandit.SetItemDisplayRuleGroup("RepeatHeal", drg_corpsebloom);

            drg_gesture = new DisplayRuleGroup();
            ItemDisplayRule banditGestureRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = gesturePrefab,
                childName = "Stomach",
                localPos = new Vector3(-0.1f, 0f, 0.21f),
                localAngles = new Vector3(0f, 75f, 0f),
                localScale = 0.6f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_gesture.AddDisplayRule(banditGestureRule);
            idrsBandit.SetItemDisplayRuleGroup("AutoCastEquipment", drg_gesture);

            drg_strides = new DisplayRuleGroup();
            ItemDisplayRule banditStridesRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = stridesPrefab,
                childName = "Head",
                localPos = new Vector3(0f, 0.35f, -0.23f),
                localAngles = new Vector3(1.1f, 267.8f, 337.9f),
                localScale = 0.5f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_strides.AddDisplayRule(banditStridesRule);
            idrsBandit.SetItemDisplayRuleGroup("LunarUtilityReplacement", drg_strides);

            drg_disciple = new DisplayRuleGroup();
            ItemDisplayRule banditDiscipleRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = disciplePrefab,
                childName = "Chest",
                localPos = new Vector3(0.3f, 0.35f, -0.1f),
                localAngles = new Vector3(332.4f, 105.9f, 310.1f),
                localScale = 0.2f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_disciple.AddDisplayRule(banditDiscipleRule);
            idrsBandit.SetItemDisplayRuleGroup("SprintWisp", drg_disciple);

            drg_halc = new DisplayRuleGroup();
            ItemDisplayRule banditHalcRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = halcPrefab,
                childName = "Stomach",
                localPos = new Vector3(0.1f, 0f, 0.2f),
                localAngles = new Vector3(2.2f, 331.8f, 312.8f),
                localScale = 0.2f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_halc.AddDisplayRule(banditHalcRule);
            idrsBandit.SetItemDisplayRuleGroup("TitanGoldDuringTP", drg_halc);

            drg_pearl = new DisplayRuleGroup();
            ItemDisplayRule banditPearlRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = pearlPrefab,
                childName = "LowerArmL",
                localPos = new Vector3(0, 0.2f, 0),
                localAngles = new Vector3(270, 0, 0),
                localScale = 0.1f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_pearl.AddDisplayRule(banditPearlRule);
            idrsBandit.SetItemDisplayRuleGroup("Pearl", drg_pearl);

            drg_ipearl = new DisplayRuleGroup();
            ItemDisplayRule banditiPearlRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ipearlPrefab,
                childName = "LowerArmR",
                localPos = new Vector3(0, 0.2f, 0),
                localAngles = new Vector3(270, 0, 0),
                localScale = 0.1f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_ipearl.AddDisplayRule(banditiPearlRule);
            idrsBandit.SetItemDisplayRuleGroup("ShinyPearl", drg_ipearl);

            drg_razor = new DisplayRuleGroup();
            ItemDisplayRule banditRazorRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = razorPrefab,
                childName = "UpperArmR",
                localPos = Vector3.zero,
                localAngles = Vector3.zero,
                localScale = Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_razor.AddDisplayRule(banditRazorRule);
            idrsBandit.SetItemDisplayRuleGroup("Thorns", drg_razor);

            drg_daisy = new DisplayRuleGroup();
            ItemDisplayRule banditDaisyRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = daisyPrefab,
                childName = "Chest",
                localPos = new Vector3(0.08f, 0.25f, 0.15f),
                localAngles = new Vector3(0f, 0f, 0f),
                localScale = 0.2f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_daisy.AddDisplayRule(banditDaisyRule);
            idrsBandit.SetItemDisplayRuleGroup("TPHealingNova", drg_daisy);

            drg_horn = new DisplayRuleGroup();
            ItemDisplayRule banditHornRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = hornPrefab,
                childName = "Stomach",
                localPos = new Vector3(0.3f, 0f, 0f),
                localAngles = new Vector3(3.2f, 270.0f, 92.3f),
                localScale = 0.4f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_horn.AddDisplayRule(banditHornRule);
            idrsBandit.SetItemDisplayRuleGroup("EnergizedOnEquipmentUse", drg_horn);

            drg_guil = new DisplayRuleGroup();
            ItemDisplayRule banditGuilRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = guilPrefab,
                childName = "Chest",
                localPos = new Vector3(0.0f, 0.18f, -0.25f),
                localAngles = new Vector3(-90f, 0f, 0f),
                localScale = 0.2f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_guil.AddDisplayRule(banditGuilRule);
            idrsBandit.SetItemDisplayRuleGroup("ExecuteLowHealthElite", drg_guil);

            drg_chrono = new DisplayRuleGroup();
            ItemDisplayRule banditChronoRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = chronoPrefab,
                childName = "ThighL",
                localPos = new Vector3(-0.15f, 0.5f, 0f),
                localAngles = new Vector3(11.4f, 39.7f, 137.1f),
                localScale = 0.3f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_chrono.AddDisplayRule(banditChronoRule);
            idrsBandit.SetItemDisplayRuleGroup("SlowOnHit", drg_chrono);

            drg_rose = new DisplayRuleGroup();
            ItemDisplayRule banditRoseRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = rosePrefab,
                childName = "LowerArmL",
                localPos = new Vector3(0.0f, 0.2f, 0.0f),
                localAngles = new Vector3(356.3f, 192.8f, 91.2f),
                localScale = 0.3f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_rose.AddDisplayRule(banditRoseRule);
            idrsBandit.SetItemDisplayRuleGroup("SprintArmor", drg_rose);

            drg_beads = new DisplayRuleGroup();
            ItemDisplayRule banditBeadsRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = beadPrefab,
                childName = "HandR",
                localPos = new Vector3(0.0f, 0.1f, 0.0f),
                localAngles = new Vector3(15.0f, 270.0f, 270.0f),
                localScale = 0.7f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_beads.AddDisplayRule(banditBeadsRule);
            idrsBandit.SetItemDisplayRuleGroup("LunarTrinket", drg_beads);

            drg_conv = new DisplayRuleGroup();
            ItemDisplayRule banditConvRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = convPrefab,
                childName = "Base",
                localPos = new Vector3(0.3f, -0.2f, -1.3f),
                localAngles = new Vector3(270.0f, 0.3f, 0.0f),
                localScale = 0.07f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_conv.AddDisplayRule(banditConvRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("FocusConvergence", drg_conv);

            drg_effigy = new DisplayRuleGroup();
            ItemDisplayRule banditEffRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = effPrefab,
                childName = "Chest",
                localPos = new Vector3(0.3f, -0.2f, -0.3f),
                localAngles = new Vector3(0f, 20.0f, 90.0f),
                localScale = Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_effigy.AddDisplayRule(banditEffRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("CrippleWard", drg_effigy);

            drg_tonic = new DisplayRuleGroup();
            ItemDisplayRule banditTonicRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = tonicPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, -0.1f, -0.2f),
                localAngles = new Vector3(-10.0f, 0.0f, 0.0f),
                localScale = 0.3f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_tonic.AddDisplayRule(banditTonicRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("Tonic", drg_tonic);

            drg_meteor = new DisplayRuleGroup();
            ItemDisplayRule banditMeteorRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = meteorPrefab,
                childName = "Chest",
                localPos = new Vector3(-0.6f, 0.0f, -1.0f),
                localAngles = new Vector3(270.0f, 0.3f, 0.0f),
                localScale = Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_meteor.AddDisplayRule(banditMeteorRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("Meteor", drg_meteor);

            drg_hel = new DisplayRuleGroup();
            ItemDisplayRule banditHelRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = helPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, -0.2f, -0.2f),
                localAngles = new Vector3(0.0f, 0.0f, 0.0f),
                localScale = 0.05f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_hel.AddDisplayRule(banditHelRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("BurnNearby", drg_hel);

            drg_radar = new DisplayRuleGroup();
            ItemDisplayRule banditRadarRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = radarPrefab,
                childName = "Chest",
                localPos = new Vector3(-0.35f, 0.25f, -0.05f),
                localAngles = new Vector3(285.1f, 224.5f, 136.5f),
                localScale = 0.3f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_radar.AddDisplayRule(banditRadarRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("Scanner", drg_radar);

            drg_vase = new DisplayRuleGroup();
            ItemDisplayRule banditVaseRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = vasePrefab,
                childName = "Chest",
                localPos = new Vector3(0.0f, 0.3f, -0.2f),
                localAngles = new Vector3(297.8f, 2.7f, 2.5f),
                localScale = 0.3f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_vase.AddDisplayRule(banditVaseRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("Gateway", drg_vase);

            drg_egg = new DisplayRuleGroup();
            ItemDisplayRule banditEggRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = eggPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, -0.2f, -0.2f),
                localAngles = new Vector3(180f + 70f, 268.9f, 268.8f),
                localScale = 0.3f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_egg.AddDisplayRule(banditEggRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("FireBallDash", drg_egg);

            drg_recycler = new DisplayRuleGroup();
            ItemDisplayRule banditRecyclerRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = recyclerPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, -0.2f, -0.2f),
                localAngles = new Vector3(0f, 90f, 0f),
                localScale = 0.1f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_recycler.AddDisplayRule(banditRecyclerRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("Recycle", drg_recycler);

            drg_prooh = new DisplayRuleGroup();
            ItemDisplayRule banditProohRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = proohPrefab,
                childName = "CalfL",
                localPos = new Vector3(-0.15f, 0.3f, 0f),
                localAngles = new Vector3(73.1f, 332.0f, 63.1f),
                localScale = 0.8f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_prooh.AddDisplayRule(banditProohRule);
            idrsBandit.SetItemDisplayRuleGroup("GainArmor", drg_prooh);

            drg_desk = new DisplayRuleGroup();
            ItemDisplayRule banditDeskRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = deskPrefab,
                childName = "ThighR",
                localPos = new Vector3(-0.1f, 0.3f, 0.1f),
                localAngles = new Vector3(67.4f, 329.0f, 164.1f),
                localScale = 0.1f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_desk.AddDisplayRule(banditDeskRule);
            idrsBandit.SetItemDisplayRuleGroup("Plant", drg_desk);

            drg_shatter = new DisplayRuleGroup();
            ItemDisplayRule banditShatterRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = shatterPrefab,
                childName = "Chest",
                localPos = new Vector3(0.0f, 0.25f, -0.22f),
                localAngles = new Vector3(276.7f, 0.0f, 0.0f),
                localScale = 0.2f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_shatter.AddDisplayRule(banditShatterRule);
            idrsBandit.SetItemDisplayRuleGroup("ArmorReductionOnHit", drg_shatter);

            drg_aegis = new DisplayRuleGroup();
            ItemDisplayRule banditAegisRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = aegisPrefab,
                childName = "LowerArmL",
                localPos = new Vector3(0f, 0.06f, -0.06f),
                localAngles = new Vector3(86.2f, 210.9f, 213.0f),
                localScale = 0.3f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_aegis.AddDisplayRule(banditAegisRule);
            idrsBandit.SetItemDisplayRuleGroup("BarrierOnOverHeal", drg_aegis);

            drg_rejuv = new DisplayRuleGroup();
            ItemDisplayRule banditRejuvRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = rejuvPrefab,
                childName = "Head",
                localPos = new Vector3(0.1f, 0.3f, 0f),
                localAngles = new Vector3(358.0f, 93.2f, 1.4f),
                localScale = 0.4f * Vector3.one,
                limbMask = LimbFlags.None
            };
            ItemDisplayRule banditRejuvRule2 = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = rejuvPrefab,
                childName = "Head",
                localPos = new Vector3(-0.1f, 0.3f, 0f),
                localAngles = new Vector3(358.0f, -93.2f, 1.4f),
                localScale = 0.4f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_rejuv.AddDisplayRule(banditRejuvRule);
            drg_rejuv.AddDisplayRule(banditRejuvRule2);
            idrsBandit.SetItemDisplayRuleGroup("IncreaseHealing", drg_rejuv);

            drg_squid = new DisplayRuleGroup();
            ItemDisplayRule banditSquidRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = squidPrefab,
                childName = "ThighR",
                localPos = new Vector3(0.1f, 0.3f, 0.1f),
                localAngles = new Vector3(8.3f, 127.6f, 103.5f),
                localScale = 0.07f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_squid.AddDisplayRule(banditSquidRule);
            idrsBandit.SetItemDisplayRuleGroup("Squid", drg_squid);

            drg_meat = new DisplayRuleGroup();
            ItemDisplayRule banditMeatRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = meatPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, 0.1f, 0.2f),
                localAngles = new Vector3(-30f, 0f, 0f),
                localScale = 0.12f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_meat.AddDisplayRule(banditMeatRule);
            idrsBandit.SetItemDisplayRuleGroup("RegenOnKill", drg_meat);

            drg_rap = new DisplayRuleGroup();
            ItemDisplayRule banditRAPRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = rapPrefab,
                childName = "CalfR",
                localPos = new Vector3(0f, 0.2f, 0f),
                localAngles = new Vector3(85.4f, 155.5f, 151.7f),
                localScale = 0.3f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_rap.AddDisplayRule(banditRAPRule);
            idrsBandit.SetItemDisplayRuleGroup("ArmorPlate", drg_rap);

            drg_key = new DisplayRuleGroup();
            ItemDisplayRule banditKeyRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = rustyPrefab,
                childName = "ThighL",
                localPos = new Vector3(-0.09f, 0.1f, 0.09f),
                localAngles = new Vector3(0f, 60f, 264.8f),
                localScale = 1.2f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_key.AddDisplayRule(banditKeyRule);
            idrsBandit.SetItemDisplayRuleGroup("TreasureCache", drg_key);

            drg_sticky = new DisplayRuleGroup();
            ItemDisplayRule banditSBRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = stickyPrefab,
                childName = "ThighL",
                localPos = new Vector3(0f, 0.4f, 0.1f),
                localAngles = new Vector3(7.9f, 180.8f, 7.6f),
                localScale = 0.4f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_sticky.AddDisplayRule(banditSBRule);
            idrsBandit.SetItemDisplayRuleGroup("StickyBomb", drg_sticky);

            drg_nrg = new DisplayRuleGroup();
            ItemDisplayRule banditNRGRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = nrgPrefab,
                childName = "ThighR",
                localPos = new Vector3(0f, 0.2f, 0.15f),
                localAngles = new Vector3(84.6f, 97.5f, 249.1f),
                localScale = 0.4f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_nrg.AddDisplayRule(banditNRGRule);
            idrsBandit.SetItemDisplayRuleGroup("SprintBonus", drg_nrg);

            drg_hardlight = new DisplayRuleGroup();
            ItemDisplayRule banditHLRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = hardlightPrefab,
                childName = "Chest",
                localPos = new Vector3(0.14f, 0.25f, -0.07f),
                localAngles = new Vector3(-110f, 180f, 0f),
                localScale = 0.8f * Vector3.one,
                limbMask = LimbFlags.None
            };
            ItemDisplayRule banditHLRule2 = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = hardlightPrefab,
                childName = "Chest",
                localPos = new Vector3(-0.14f, 0.25f, -0.07f),
                localAngles = new Vector3(-110f, 180f, 0f),
                localScale = 0.8f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_hardlight.AddDisplayRule(banditHLRule);
            drg_hardlight.AddDisplayRule(banditHLRule2);
            idrsBandit.SetItemDisplayRuleGroup("UtilitySkillMagazine", drg_hardlight);

            drg_wake = new DisplayRuleGroup();
            ItemDisplayRule banditWakeRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = wakePrefab,
                childName = "Head",
                localPos = new Vector3(0f, 0.31f, -0.04f),
                localAngles = new Vector3(-30f, 0f, 0f),
                localScale = new Vector3(0.38f, 0.13f, 0.13f),
                limbMask = LimbFlags.None
            };
            drg_wake.AddDisplayRule(banditWakeRule);
            idrsBandit.SetItemDisplayRuleGroup("HeadHunter", drg_wake);

            drg_visions = new DisplayRuleGroup();
            ItemDisplayRule banditVisionsRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = visionsPrefab,
                childName = "Head",
                localPos = new Vector3(0f, 0.21f, 0.08f),
                localAngles = new Vector3(270f, 0f, 0f),
                localScale = 0.18f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_visions.AddDisplayRule(banditVisionsRule);
            idrsBandit.SetItemDisplayRuleGroup("LunarPrimaryReplacement", drg_visions);

            drg_ap = new DisplayRuleGroup();
            ItemDisplayRule banditAPRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = apPrefab,
                childName = "MuzzleShotgun",
                localPos = new Vector3(-0.05f, -0.1f, -0.7f),
                localAngles = new Vector3(90f, 90f, 0f),
                localScale = 0.7f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_ap.AddDisplayRule(banditAPRule);
            idrsBandit.SetItemDisplayRuleGroup("BossDamageBonus", drg_ap);

            drg_brain = new DisplayRuleGroup();
            ItemDisplayRule banditBrainRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = brainPrefab,
                childName = "Head",
                localPos = new Vector3(0f, 0.2f, -0.05f),
                localAngles = Vector3.zero,
                localScale = 0.3f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_brain.AddDisplayRule(banditBrainRule);
            idrsBandit.SetItemDisplayRuleGroup("KillEliteFrenzy", drg_brain);

            drg_dio = new DisplayRuleGroup();
            ItemDisplayRule banditDioRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = dioPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, 0.25f, -0.25f),
                localAngles = new Vector3(0f, 180f, 0f),
                localScale = 0.4f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_dio.AddDisplayRule(banditDioRule);
            idrsBandit.SetItemDisplayRuleGroup("ExtraLife", drg_dio);

            drg_sawmerang = new DisplayRuleGroup();
            ItemDisplayRule banditSawRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = sawPrefab,
                childName = "Chest",
                localPos = new Vector3(-0.6f, 0.9f, 0.8f),
                localAngles = new Vector3(84.9f, 179.6f, 180f),
                localScale = 0.2f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_sawmerang.AddDisplayRule(banditSawRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("Saw", drg_sawmerang);

            drg_brooch = new DisplayRuleGroup();
            ItemDisplayRule banditBroochRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = broochPrefab,
                childName = "Chest",
                localPos = new Vector3(-0.05f, 0.2f, 0.18f),
                localAngles = new Vector3(90f, 0f, 0f),
                localScale = 0.7f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_brooch.AddDisplayRule(banditBroochRule);
            idrsBandit.SetItemDisplayRuleGroup("BarrierOnKill", drg_brooch);

            drg_goldgat = new DisplayRuleGroup();
            ItemDisplayRule banditGGRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ggPrefab,
                childName = "Chest",
                localPos = new Vector3(0.37f, 0.63f, -0.32f),
                localAngles = new Vector3(30f, 130f, -15f),
                localScale = new Vector3(0.2f, 0.2f, 0.2f),
                limbMask = LimbFlags.None
            };
            drg_goldgat.AddDisplayRule(banditGGRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("GoldGat", drg_goldgat);

            drg_fuelarray = new DisplayRuleGroup();
            ItemDisplayRule banditVBRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = vbPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, 0.1f, -0.25f),
                localAngles = new Vector3(15f, 180f, 0f),
                localScale = 0.4f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_fuelarray.AddDisplayRule(banditVBRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("QuestVolatileBattery", drg_fuelarray);

            drg_blastshower = new DisplayRuleGroup();
            ItemDisplayRule banditBSRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = bsPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, -0.15f, -0.15f),
                localAngles = new Vector3(7f, 180f, 0f),
                localScale = new Vector3(0.2f, 0.2f, 0.2f),
                limbMask = LimbFlags.None
            };
            drg_blastshower.AddDisplayRule(banditBSRule);
            idrsBandit.SetEquipmentDisplayRuleGroup("Cleanse", drg_blastshower);

            drg_focuscrystal = new DisplayRuleGroup();
            ItemDisplayRule banditFCRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fcPrefab,
                childName = "HandL",
                localPos = new Vector3(0.1f, 0f, 0f),
                localAngles = Vector3.zero,
                localScale = 0.1f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_focuscrystal.AddDisplayRule(banditFCRule);
            idrsBandit.SetItemDisplayRuleGroup("NearbyDamageBonus", drg_focuscrystal);

            drg_ghor = new DisplayRuleGroup();
            ItemDisplayRule banditGhorRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ghorPrefab,
                childName = "Chest",
                localPos = new Vector3(0f, 0.1f, -0.25f),
                localAngles = new Vector3(15f, 180f, 0f),
                localScale = 0.1f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_ghor.AddDisplayRule(banditGhorRule);
            idrsBandit.SetItemDisplayRuleGroup("BonusGoldPackOnKill", drg_ghor);

            drg_quail = new DisplayRuleGroup();
            ItemDisplayRule banditQuailRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = quailPrefab,
                childName = "Head",
                localPos = new Vector3(0f, -0.25f, -0.3f),
                localAngles = Vector3.zero,
                localScale = Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_quail.AddDisplayRule(banditQuailRule);
            idrsBandit.SetItemDisplayRuleGroup("JumpBoost", drg_quail);

            drg_genesis = new DisplayRuleGroup();
            ItemDisplayRule banditGenesisRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = genesisPrefab,
                childName = "Head",
                localPos = new Vector3(-0.1f, 0.25f, -0.1f),
                localAngles = new Vector3(-90f, 0f, 0f),
                localScale = 0.18f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_genesis.AddDisplayRule(banditGenesisRule);
            idrsBandit.SetItemDisplayRuleGroup("NovaOnLowHealth", drg_genesis);

            drg_happiest = new DisplayRuleGroup();
            ItemDisplayRule banditHappiestRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = happiestPrefab,
                childName = "Head",
                localPos = new Vector3(0f, 0.2f, 0.05f),
                localAngles = new Vector3(0f, 0f, 0f),
                localScale = 0.6f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_happiest.AddDisplayRule(banditHappiestRule);
            idrsBandit.SetItemDisplayRuleGroup("GhostOnKill", drg_happiest);

            drg_backupmag = new DisplayRuleGroup();
            ItemDisplayRule banditBMRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = backupmagPrefab,
                childName = "MuzzleShotgun",
                localPos = new Vector3(0.02f, -0.3f, -0.6f),
                localAngles = Vector3.zero,
                localScale = 0.1f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_backupmag.AddDisplayRule(banditBMRule);
            idrsBandit.SetItemDisplayRuleGroup("SecondarySkillMagazine", drg_backupmag);
            #endregion

            /*drg_deathmark = new DisplayRuleGroup();
            ItemDisplayRule banditDMRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = dmPrefab,
                childName = "HandR",
                localPos = new Vector3(0.1f, 0f, 0f),
                localAngles = new Vector3(-90f, 0f, 90f),
                localScale = 0.025f * Vector3.one,
                limbMask = LimbFlags.None
            };
            drg_deathmark.AddDisplayRule(banditDMRule);
            idrsBandit.SetItemDisplayRuleGroup("DeathMark", drg_deathmark);*/

            drg_deathmark = new DisplayRuleGroup();
            ItemDisplayRule banditDMRule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = dmPrefab,
                childName = "Head",
                localPos = new Vector3(0f, 0.22f, -0.02f),
                localAngles = new Vector3(280f, 180f, 180f),
                localScale = 0.075f * Vector3.one,
                limbMask = LimbFlags.Head
            };
            drg_deathmark.AddDisplayRule(banditDMRule);
            idrsBandit.SetItemDisplayRuleGroup("DeathMark", drg_deathmark);
        }

        private void ReadConfig()
        {
            usePassive = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Enable Passive"), true, new ConfigDescription("Makes Bandit auto-reload his primary when using other skills."));
            cloakAnim = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use Cloak Anim"), false, new ConfigDescription("Enables the unused Smokebomb entry animation."));
            vanillaCloak = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use Vanilla Cloak"), false, new ConfigDescription("Use the unused cloak skill in the game files. This makes Bandit able to use Smokebomb when playing with a host that doesn't have the mod, but may interfere with mods that rely on the unused cloak skill."));
            useAltCrosshair = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use Alt Crosshair"), true, new ConfigDescription("Uses the unused Bandit-specific crosshair."));
            classicOutro = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use RoR1 Outro"), false, new ConfigDescription("Uses Bandit's RoR1 ending."));
            asEnabled = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Enable unused Assassinate utility*"), false, new ConfigDescription("Enables the Assassinate Utility skill. This skill was disabled due to being poorly coded and not fitting Bandit's kit, but it's left in in case you want to use it. This skill can only be used if Assassinate is enabled on the host."));

            blastDamage = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Damage"), 2.3f, new ConfigDescription("How much damage Blast deals."));
            blastMaxDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Fire Rate"), 0.3f, new ConfigDescription("Time between shots."));
            blastMinDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Min Duration"), 0.2f, new ConfigDescription("How soon you can fire another shot if you mash."));
            blastDryEnabled = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Enable Dryfire"), true, new ConfigDescription("Allow Blast to be fired on an empty mag."));
            blastDryDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Dryfire Duration"), 0.5f, new ConfigDescription("Time between shots on an empty mag."));
            blastPenetrate = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Penetrate Enemies"), true, new ConfigDescription("Shots pierce enemies."));
            blastRadius = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Shot Radius"), 0.4f, new ConfigDescription("How wide Blast's shots are."));
            blastForce = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Force"), 600f, new ConfigDescription("Push force per shot."));
            blastSpread = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Spread"), 0f, new ConfigDescription("Amount of spread with added each shot."));
            blastMashSpread = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Mash Spread"), 0.4f, new ConfigDescription("Amount of spread with added each shot when mashing."));
            blastRange = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Range"), 250f, new ConfigDescription("How far Blast can reach."));
            blastFalloff = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Use Falloff"), true, new ConfigDescription("Shots deal less damage over range."));
            blastStock = base.Config.Bind<int>(new ConfigDefinition("10 - Primary - Blast", "Stock"), 8, new ConfigDescription("How many shots can be fired before reloading."));
            blastRechargeInterval = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Reload Time"), 2f, new ConfigDescription("How long it takes to reload. Set to 0 to disable reloading."));
            //blastIndividualReload = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Reload Individually"), false, new ConfigDescription("Reload each shot individually."));
            //blastInitialRechargeInterval = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Initial Reload Time"), 1f, new ConfigDescription("How much time it takes to reload the first shot. Does nothing unless Reload Individually is enabled."));
            blastVanillaBrainstalks = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Use Vanilla Brainstalks Behavior"), true, new ConfigDescription("Disables infinite ammo when Brainstalks is active."));

            scatterDamage = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Damage"), 0.8f, new ConfigDescription("How much damage each pellet of Scatter deals."));
            scatterPellets = base.Config.Bind<uint>(new ConfigDefinition("11 - Primary - Scatter", "Pellets"), 6, new ConfigDescription("How many pellets Scatter shoots."));
            scatterProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Proc Coefficient"), 0.7f, new ConfigDescription("Affects the chance and power of each pellet's procs."));
            scatterMaxDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Fire Rate"), 0.625f, new ConfigDescription("Time between shots."));
            scatterMinDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Min Duration"), 0.625f, new ConfigDescription("How soon you can fire another shot if you mash."));
            scatterDryEnabled = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Enable Dryfire"), true, new ConfigDescription("Allow Scatter to be fired on an empty mag."));
            scatterDryDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Dryfire Duration"), 1f, new ConfigDescription("Time between shots on an empty mag."));
            scatterPenetrate = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Penetrate Enemies"), true, new ConfigDescription("Shots pierce enemies."));
            scatterRadius = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Shot Radius"), 0.4f, new ConfigDescription("How wide Scatter's pellets are."));
            scatterForce = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Force"), 200f, new ConfigDescription("Push force per pellet."));
            scatterSpread = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Spread"), 2.5f, new ConfigDescription("Size of the pellet spread."));
            scatterRange = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Range"), 120f, new ConfigDescription("How far Scatter can reach."));
            scatterStock = base.Config.Bind<int>(new ConfigDefinition("11 - Primary - Scatter", "Stock"), 5, new ConfigDescription("How many shots Scatter can hold."));
            scatterRechargeInterval = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Reload Time"), 2.5f, new ConfigDescription("How much time it takes to reload. Set to 0 to disable reloading."));
            //scatterIndividualReload = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Reload Individually"), true, new ConfigDescription("Reload each shot individually."));
            //scatterInitialRechargeInterval = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Initial Reload Time"), 1f, new ConfigDescription("How much time it takes to reload the first shot. Does nothing unless Reload Individually is enabled."));
            scatterVanillaBrainstalks = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Use Vanilla Brainstalks Behavior"), true, new ConfigDescription("Disables infinite ammo when Brainstalks is active."));

            thermiteDamage = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Damage"), 2.4f, new ConfigDescription("How much damage Thermite Bomb deals."));
            thermiteBurnDamageMult = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Burn Damage*"), 1f, new ConfigDescription("Multiplier for Thermite burn damage."));
            thermiteBurnDuration = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Burn Duration*"), 10f, new ConfigDescription("How long the burn lasts for."));
            thermiteRadius = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Radius*"), 8f, new ConfigDescription("How large the explosion is."));
            thermiteProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Proc Coefficient*"), 1f, new ConfigDescription("Affects the chance and power of Thermite Bomb's procs."));
            thermiteVelocity = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Velocity*"), 60f, new ConfigDescription("How fast the projectile travels."));
            thermiteLifetime = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Lifetime*"), 20f, new ConfigDescription("Duration the projectile can fly for before exploding."));
            thermiteGravity = base.Config.Bind<bool>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Use Gravity*"), true, new ConfigDescription("Makes Thermite Bombs have an arc."));
            thermiteFireRate = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Fire Rate"), 0.4f, new ConfigDescription("How long it takes to throw a Thermite Bomb."));
            thermiteCooldown = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Cooldown"), 7f, new ConfigDescription("How long Thermite Bomb takes to recharge."));
            thermiteStock = base.Config.Bind<int>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Stock"), 1, new ConfigDescription("How many Thermite Bombs you start with."));

            acidDamage = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Damage"), 2.4f, new ConfigDescription("How much damage Acid Bomb deals."));
            acidWeakDuration = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Weaken Duration*"), 5f, new ConfigDescription("How long the Cripple debuff lasts for."));
            acidRadius = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Radius*"), 8f, new ConfigDescription("How large the explosion is."));
            acidProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Proc Coefficient*"), 1f, new ConfigDescription("Affects the chance and power of Acid Bomb's procs."));
            acidVelocity = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Velocity*"), 60f, new ConfigDescription("How fast the projectile travels."));
            acidLifetime = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Lifetime*"), 20f, new ConfigDescription("Duration the projectile can fly for before exploding."));
            acidGravity = base.Config.Bind<bool>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Use Gravity*"), true, new ConfigDescription("Makes Acid Bombs have an arc."));
            acidFireRate = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Fire Rate"), 0.4f, new ConfigDescription("How long it takes to throw a Acid Bomb."));
            acidCooldown = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Cooldown"), 7f, new ConfigDescription("How long Acid Bomb takes to recharge."));
            acidStock = base.Config.Bind<int>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Stock"), 1, new ConfigDescription("How many Acid Bombs you start with."));

            cbDamage = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Damage*"), 4f, new ConfigDescription("How much damage Cluster Bomb deals."));
            cbRadius = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Radius*"), 8f, new ConfigDescription("How large the explosion is."));
            cbProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Proc Coefficient*"), 1f, new ConfigDescription("Affects the chance and power of Cluster Bomb's procs."));
            cbBombletCount = base.Config.Bind<int>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Bomblet Count*"), 6, new ConfigDescription("How many mini bombs Cluster Bomb releases."));
            cbBombletDamage = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Bomblet Damage*"), 1.2f, new ConfigDescription("How much damage Cluster Bomb Bomblets deals."));
            cbBombletRadius = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Bomblet Radius*"), 6f, new ConfigDescription("How large the mini explosions are."));
            cbBombletProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Bomblet Proc Coefficient*"), 0.6f, new ConfigDescription("Affects the chance and power of Cluster Bomb Bomblets' procs."));
            cbVelocity = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Velocity*"), 60f, new ConfigDescription("How fast the projectile travels."));
            cbFireRate = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Fire Rate"), 0.4f, new ConfigDescription("How long it takes to throw a Cluster Bomb."));
            cbCooldown = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Cooldown"), 7f, new ConfigDescription("How long it takes for Cluster Bomb to recharge."));
            cbStock = base.Config.Bind<int>(new ConfigDefinition("22 - Secondary - Cluster Bomb", "Stock"), 1, new ConfigDescription("How many Cluster Bombs you start with."));

            cloakDamage = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Damage*"), 1.5f, new ConfigDescription("How much damage Smokebomb deals."));
            cloakRadius = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Radius*"), 10f, new ConfigDescription("Size of the stun radius."));
            cloakDuration = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Duration*"), 3f, new ConfigDescription("How long Smokebomb lasts."));
            cloakMinDuration = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Minimum Duration"), 0.5f, new ConfigDescription("Minimum amount of time Smokebomb lasts for."));
            cloakCooldown = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Cooldown"), 8f, new ConfigDescription("How long Smokebomb takes to recharge."));
            cloakStock = base.Config.Bind<int>(new ConfigDefinition("30 - Utility - Smokebomb", "Stock"), 1, new ConfigDescription("How many charges Smokebomb has."));

            loDamage = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Damage"), 4.2f, new ConfigDescription("How much damage Lights Out deals."));
            loBuffDamage = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Debuff Bonus Damage*"), 4.2f, new ConfigDescription("How much extra damage Lights Out deals for each debuff the target has."));
            loGracePeriodMin = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Grace Period Minimum Duration*"), 1.5f, new ConfigDescription("Lower bound of Lights Out's Grace Period duration."));
            loGracePeriodMax = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Grace Period Maximum Duration*"), 4.5f, new ConfigDescription("Upper bound of Lights Out's Grace Period duration."));
            loForce = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Force"), 2400f, new ConfigDescription("Push force per shot."));
            loFireRate = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Fire Rate"), 0.6f, new ConfigDescription("How long it takes to aim the skill."));
            loEndLag = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "End Lag"), 0.2f, new ConfigDescription("Delay after firing."));
            loCooldown = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Cooldown"), 7f, new ConfigDescription("How long Lights Out takes to recharge."));
            loStock = base.Config.Bind<int>(new ConfigDefinition("40 - Special - Lights Out", "Stock"), 1, new ConfigDescription("How many charges Lights Out has."));
            loExecuteThreshold = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Execute Threshold*"), 0f, new ConfigDescription("Instakill enemies that fall below this HP percentage."));
            loExecuteBosses = base.Config.Bind<bool>(new ConfigDefinition("40 - Special - Lights Out", "Execute Bosses*"), false, new ConfigDescription("Allow bosses to be executed by Lights Out."));

            reuDamage = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Damage"), 0.7f, new ConfigDescription("How much damage Rack em Up deals."));
            reuDebuffDamage = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Debuff Bonus Damage*"), 0.7f, new ConfigDescription("How much extra damage Rack em Up deals for each debuff the target has."));
            reuGracePeriodMin = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Grace Period Minimum Duration*"), 1f, new ConfigDescription("Lower bound of Rack em Up's Grace Period duration."));
            reuGracePeriodMax = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Grace Period Maximum Duration*"), 4f, new ConfigDescription("Upper bound of Rack em Up's Grace Period duration."));
            reuBullets = base.Config.Bind<int>(new ConfigDefinition("41 - Special - Rack em Up", "Total Shots"), 6, new ConfigDescription("How many shots are fired."));
            reuForce = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Force"), 100f, new ConfigDescription("Push force per shot."));
            reuDraw = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Draw Time"), 0.32f, new ConfigDescription("How long it takes to prepare Rack em Up."));
            reuFireRate = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Fire Rate"), 0.13f, new ConfigDescription("Time it takes for Rack em Up to fire a single shot."));
            reuEndLag = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "End Lag"), 0.4f, new ConfigDescription("Delay after firing all shots."));
            reuSpread = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Spread"), 2.5f, new ConfigDescription("Size of the cone of fire."));
            reuRange = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Range"), 120f, new ConfigDescription("How far shots reach."));
            reuCooldown = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Cooldown"), 7f, new ConfigDescription("How long Rack em Up takes to recharge."));
            reuStock = base.Config.Bind<int>(new ConfigDefinition("41 - Special - Rack em Up", "Stock"), 1, new ConfigDescription("How many charges Rack em Up has."));
            reuExecuteThreshold = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Execute Threshold*"), 0f, new ConfigDescription("Instakill enemies that fall below this HP percentage."));
            reuExecuteBosses = base.Config.Bind<bool>(new ConfigDefinition("40 - Special - Lights Out", "Execute Bosses*"), false, new ConfigDescription("Allow bosses to be executed by Rack em Up."));

            
            asMinDamage = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Minimum Damage"), 2.3f, new ConfigDescription("How much damage Assassinate deals at no charge."));
            asMaxDamage = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Maximum Damage"), 17f, new ConfigDescription("How much damage Assassinate deals at max charge."));
            asMinRadius = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Minimum Radius"), 0.4f, new ConfigDescription("How large Assassinate's shot radius is at no charge."));
            asMaxRadius = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Maximum Radius"), 2.4f, new ConfigDescription("How large Assassinate's shot radius is at max charge."));
            asMinForce = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Minimum Force"), 600f, new ConfigDescription("Push force at no charge."));
            asMaxForce = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Maximum Force"), 2400f, new ConfigDescription("Push force at max charge."));
            asSelfForceMin = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Minimum Self Force"), 4500f, new ConfigDescription("How far back you are launched when firing at no charge."));
            asSelfForceMax = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Maximum Self Force"), 4500f, new ConfigDescription("How far back you are launched when firing at max charge."));
            asChargeDuration = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Charge Duration"), 1.5f, new ConfigDescription("How long it takes to fully charge Assassinate."));
            asEndLag = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "End Lag"), 0.5f, new ConfigDescription("Delay after firing."));
            asZoom = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Zoom FOV"), -1f, new ConfigDescription("Zoom-in FOV when charging Assassinate. -1 disables."));
            asCooldown = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Cooldown"), 5f, new ConfigDescription("How long it takes Assassinate to recharge"));
            asStock = base.Config.Bind<int>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Stock"), 1, new ConfigDescription("How many charges Assassinate has."));
            asBarrierPercent = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Barrier Percent"), 0.07f, new ConfigDescription("Max barrier on hit percent at full charge."));
        }

        //Fill out fields in EntityStates
        private void InitSkills()
        {
            BanditHelpers.enablePassive = usePassive.Value;

            Blast.force = blastForce.Value;
            Blast.maxDistance = blastRange.Value;
            Blast.damageCoefficient = blastDamage.Value;
            Blast.baseMaxDuration = blastMaxDuration.Value;
            Blast.baseMinDuration = blastMinDuration.Value;
            Blast.spreadBloomValue = blastSpread.Value;
            //Blast.individualReload = blastIndividualReload.Value;
            Blast.recoilAmplitude = 1.4f;
            Blast.bulletRadius = 0.4f;
            Blast.attackSoundString = "Play_bandit_M1_shot";
            Blast.effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
            Blast.hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
            Blast.tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");
            Blast.vanillaBrainstalks = blastVanillaBrainstalks.Value;
            //Blast.initialReloadDuration = blastInitialRechargeInterval.Value;
            Blast.penetrateEnemies = blastPenetrate.Value;
            Blast.useFalloff = blastFalloff.Value;
            Blast.mashSpread = blastMashSpread.Value;
            Blast.dryFireDuration = blastDryDuration.Value;
            //Blast.emptyCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/badcrosshair");

            if (vanillaCloak.Value)
            {
                if (cloakAnim.Value)
                {
                    EntityStates.Commando.CommandoWeapon.CastSmokescreen.damageCoefficient = cloakDamage.Value;
                    EntityStates.Commando.CommandoWeapon.CastSmokescreen.radius = cloakRadius.Value;
                    EntityStates.Commando.CommandoWeapon.CastSmokescreen.stealthDuration = cloakDuration.Value + 1f;
                }
                else
                {
                    EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.damageCoefficient = cloakDamage.Value;
                    EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.radius = cloakRadius.Value;
                    EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.duration = cloakDuration.Value;
                    EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.minimumStateDuration = cloakMinDuration.Value;
                }
            }

            CastSmokescreen.stealthDuration = cloakDuration.Value + 1f;
            CastSmokescreen.damageCoefficient = cloakDamage.Value;
            CastSmokescreen.radius = cloakRadius.Value;
            CastSmokescreen.baseDuration = EntityStates.Commando.CommandoWeapon.CastSmokescreen.baseDuration;
            CastSmokescreen.forceMagnitude = 0f;
            CastSmokescreen.initialEffectPrefab = EntityStates.Commando.CommandoWeapon.CastSmokescreen.initialEffectPrefab;
            CastSmokescreen.jumpSoundString = EntityStates.Commando.CommandoWeapon.CastSmokescreen.jumpSoundString;
            CastSmokescreen.smokescreenEffectPrefab = EntityStates.Commando.CommandoWeapon.CastSmokescreen.smokescreenEffectPrefab;
            CastSmokescreen.startCloakSoundString = EntityStates.Commando.CommandoWeapon.CastSmokescreen.startCloakSoundString;
            CastSmokescreen.stopCloakSoundString = EntityStates.Commando.CommandoWeapon.CastSmokescreen.stopCloakSoundString;

            CastSmokescreenNoDelay.duration = cloakDuration.Value;
            CastSmokescreenNoDelay.damageCoefficient = cloakDamage.Value;
            CastSmokescreenNoDelay.radius = cloakRadius.Value;
            CastSmokescreenNoDelay.minimumStateDuration = cloakMinDuration.Value;
            CastSmokescreenNoDelay.forceMagnitude = 0f;
            CastSmokescreenNoDelay.smokescreenEffectPrefab = EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.smokescreenEffectPrefab;
            CastSmokescreenNoDelay.startCloakSoundString = EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.startCloakSoundString;
            CastSmokescreenNoDelay.stopCloakSoundString = EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.stopCloakSoundString;
            CastSmokescreenNoDelay.destealthMaterial = EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.destealthMaterial;

            Assassinate.minimumStateDuration = 0f;
            Assassinate.baseChargeDuration = asChargeDuration.Value;
            Assassinate.beginChargeSoundString = "Play_MULT_m1_snipe_charge";
            Assassinate.chargeSoundString = "Play_item_proc_crit_cooldown";
            Assassinate.holdChargeVfxPrefab = EntityStates.Toolbot.ChargeSpear.holdChargeVfxPrefab;
            Assassinate.chargeupVfxPrefab = EntityStates.Toolbot.ChargeSpear.chargeupVfxPrefab;
            Assassinate.muzzleName = "MuzzleShotgun";
            Assassinate.zoomFOV = asZoom.Value;
            Assassinate.perfectChargeDuration = 0f;

            FireChargeShot.minForce = asMinForce.Value;
            FireChargeShot.maxForce = asMaxForce.Value;
            FireChargeShot.selfForceMax = asSelfForceMax.Value;
            FireChargeShot.selfForceMin = asSelfForceMin.Value;
            FireChargeShot.maxDamageCoefficient = asMaxDamage.Value;
            FireChargeShot.minDamageCoefficient = asMinDamage.Value;
            FireChargeShot.attackSoundString = "Play_bandit_M2_shot";
            FireChargeShot.fullChargeSoundString = "Play_item_use_lighningArm";
            FireChargeShot.hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
            FireChargeShot.tracerEffectPrefab = EntityStates.Sniper.SniperWeapon.FireRifle.tracerEffectPrefab;
            FireChargeShot.perfectTracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracertoolbotrebar");
            FireChargeShot.perfectChargeBonus = 2f;
            FireChargeShot.baseDuration = asEndLag.Value;
            FireChargeShot.recoilAmplitude = 4f;
            FireChargeShot.effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
            FireChargeShot.minRadius = asMinRadius.Value;
            FireChargeShot.maxRadius = asMaxRadius.Value;
            FireChargeShot.barrierPercent = asBarrierPercent.Value;

            PrepLightsOut.baseDuration = loFireRate.Value;
            PrepLightsOut.prepSoundString = "Play_bandit_M2_load";
            PrepLightsOut.specialCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/banditcrosshairrevolver");

            FireLightsOut.damageCoefficient = loDamage.Value;
            FireLightsOut.force = loForce.Value;
            FireLightsOut.baseDuration = loEndLag.Value;
            FireLightsOut.gracePeriodMin = loGracePeriodMin.Value;
            FireLightsOut.gracePeriodMax = loGracePeriodMax.Value;
            FireLightsOut.executeThreshold = loExecuteThreshold.Value;
            FireLightsOut.executeBosses = loExecuteBosses.Value;
            FireLightsOut.recoilAmplitude = 4f;
            FireLightsOut.effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditpistol");
            FireLightsOut.hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/hitsparkbanditpistol");
            FireLightsOut.tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditpistol");
            FireLightsOut.attackSoundString = "Play_bandit_M2_shot";
            FireLightsOut.buffDamageCoefficient = loBuffDamage.Value;

            GrenadeToss.projectilePrefab = AcidBombObject;
            GrenadeToss.damageCoefficient = acidDamage.Value;
            GrenadeToss.debuffDuration = acidWeakDuration.Value;
            GrenadeToss.baseDuration = acidFireRate.Value;
            GrenadeToss.force = 0f;
            GrenadeToss.selfForce = 0f;
            if (AcidBombObject != null)
            {
                AcidBombObject.GetComponent<ProjectileSimple>().velocity = acidVelocity.Value;
                ProjectileImpactExplosion abPIE = AcidBombObject.GetComponent<ProjectileImpactExplosion>();
                abPIE.blastRadius = acidRadius.Value;
                AcidBombObject.GetComponent<ProjectileDamage>().damageType = DamageType.WeakOnHit;
                abPIE.blastProcCoefficient = acidProcCoefficient.Value;
                abPIE.falloffModel = BlastAttack.FalloffModel.None;
                abPIE.lifetime = acidLifetime.Value;
                abPIE.impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion");
                abPIE.explosionSoundString = "Play_commando_M2_grenade_explo";
                abPIE.timerAfterImpact = true;
                abPIE.lifetimeAfterImpact = 0.25f;
                abPIE.destroyOnWorld = false;
                abPIE.destroyOnEnemy = false;
                AcidBombObject.GetComponent<ProjectileStickOnImpact>().ignoreWorld = false;
                if (acidGravity.Value)
                {
                    AcidBombObject.GetComponent<Rigidbody>().useGravity = acidGravity.Value;
                }
            }

            ThermiteBomb.projectilePrefab = ThermiteObject;
            ThermiteBomb.damageCoefficient = thermiteDamage.Value;
            ThermiteBomb.baseDuration = thermiteFireRate.Value;
            ThermiteBomb.force = 0f;
            ThermiteBomb.selfForce = 0f;
            ThermiteBomb.debuffDuration = thermiteBurnDuration.Value;
            ThermiteBomb.burnDamageMult = thermiteBurnDamageMult.Value;
            if (ThermiteObject != null)
            {
                ProjectileImpactExplosion tPIE = ThermiteObject.GetComponent<ProjectileImpactExplosion>();
                tPIE.blastRadius = thermiteRadius.Value;
                tPIE.blastProcCoefficient = thermiteProcCoefficient.Value;
                tPIE.blastDamageCoefficient = 1f;
                tPIE.falloffModel = BlastAttack.FalloffModel.None;
                tPIE.timerAfterImpact = false;
                tPIE.lifetimeAfterImpact = 0.25f;
                tPIE.lifetime = thermiteLifetime.Value;
                tPIE.impactEffect = Resources.Load<GameObject>("prefabs/effects/omnieffect/omniexplosionvfx");
                tPIE.explosionSoundString = "Play_clayboss_M1_explo";
                tPIE.destroyOnEnemy = true;
                tPIE.destroyOnWorld = true;
                ThermiteObject.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
                ThermiteObject.GetComponent<ProjectileSimple>().velocity = thermiteVelocity.Value;
                ThermiteObject.GetComponent<ProjectileDamage>().damage = 1f;
                SphereCollider tS = ThermiteObject.GetComponent<SphereCollider>();
                if (tS == null)
                {
                    tS = ThermiteObject.AddComponent<SphereCollider>();
                }
                tS.contactOffset = AcidBombObject.GetComponent<SphereCollider>().contactOffset;
                tS.radius = AcidBombObject.GetComponent<SphereCollider>().radius;
                tS.contactOffset = AcidBombObject.GetComponent<SphereCollider>().contactOffset;
                ProjectileController tPC = ThermiteObject.GetComponent<ProjectileController>();
                if (tPC == null)
                {
                    tPC = ThermiteObject.AddComponent<ProjectileController>();
                }
                tPC.procCoefficient = 1f;
                ThermiteObject.GetComponent<ProjectileIntervalOverlapAttack>().damageCoefficient = 0f;
                ThermiteBomb.projectilePrefab = ThermiteObject;
                tPC.ghostPrefab = Resources.Load<GameObject>("prefabs/projectileghosts/thermiteghost");
                if (thermiteGravity.Value)
                {
                    ThermiteObject.GetComponent<Rigidbody>().useGravity = thermiteGravity.Value;
                }
            }

            Scatter.procCoefficient = scatterProcCoefficient.Value;
            Scatter.pelletCount = scatterPellets.Value;
            Scatter.damageCoefficient = scatterDamage.Value;
            Scatter.force = scatterForce.Value;
            Scatter.baseMaxDuration = scatterMaxDuration.Value;
            Scatter.baseMinDuration = scatterMinDuration.Value;
            Scatter.spreadBloomValue = scatterSpread.Value;
            Scatter.recoilAmplitude = 2.6f;
            Scatter.bulletRadius = 0.4f;
            Scatter.attackSoundString = "Play_bandit_M2_shot";
            Scatter.effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
            Scatter.hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/hitspark1");
            Scatter.tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");
            //Scatter.individualReload = scatterIndividualReload.Value;
            Scatter.vanillaBrainstalks = scatterVanillaBrainstalks.Value;
            //Scatter.initialReloadDuration = scatterInitialRechargeInterval.Value;
            Scatter.dryFireDuration = scatterDryDuration.Value;
            Scatter.penetrateEnemies = scatterPenetrate.Value;
            Scatter.range = scatterRange.Value;
            //Scatter.emptyCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/badcrosshair");

            ClusterBomb.baseDuration = cbFireRate.Value;
            ClusterBomb.damageCoefficient = cbDamage.Value;
            ClusterBomb.bombletCount = cbBombletCount.Value;
            ClusterBomb.force = 0f;
            ClusterBomb.selfForce = 0f;
            ClusterBomb.projectilePrefab = ClusterBombObject;
            ClusterBomb.bombletDamageCoefficient = cbBombletDamage.Value;
            float trueBombletDamage = ClusterBomb.bombletDamageCoefficient / ClusterBomb.damageCoefficient;
            if (ClusterBombObject != null)
            {
                ClusterBombObject.AddComponent<SphereCollider>();
                //ClusterBombGhostObject.AddComponent<SphereCollider>();

                ProjectileImpactExplosion pie = ClusterBombObject.GetComponent<ProjectileImpactExplosion>();
                pie.blastRadius = cbRadius.Value;
                pie.falloffModel = BlastAttack.FalloffModel.None;
                pie.lifetime = 25f;
                pie.lifetimeAfterImpact = 0.75f;
                pie.destroyOnEnemy = false;
                pie.destroyOnWorld = false;
                pie.childrenCount = ClusterBomb.bombletCount;
                pie.childrenDamageCoefficient = trueBombletDamage;
                pie.blastProcCoefficient = cbProcCoefficient.Value;
                pie.impactEffect = Resources.Load<GameObject>("prefabs/effects/omnieffect/omniexplosionvfx");
                pie.explosionSoundString = "Play_MULT_m2_main_explode";
                ClusterBombObject.GetComponent<ProjectileStickOnImpact>().ignoreWorld = true;
                pie.childrenProjectilePrefab = ClusterBombletObject;

                ProjectileSimple ps = ClusterBombObject.GetComponent<ProjectileSimple>();
                ps.velocity = cbVelocity.Value;

                ClusterBombObject.GetComponent<Rigidbody>().useGravity = true;

                ProjectileDamage pd = ClusterBombObject.GetComponent<ProjectileDamage>();
                pd.damageType = DamageType.Stun1s;
            }
            if (ClusterBombletObject != null)
            {
                ClusterBombletObject.AddComponent<SphereCollider>();
                //ClusterBombletGhostObject.AddComponent<SphereCollider>();

                ProjectileImpactExplosion pie = ClusterBombletObject.GetComponent<ProjectileImpactExplosion>();
                pie.blastRadius = cbBombletRadius.Value;
                pie.falloffModel = BlastAttack.FalloffModel.None;
                pie.destroyOnEnemy = false;
                pie.destroyOnWorld = false;
                pie.lifetime = 1.5f;
                pie.timerAfterImpact = false;
                pie.blastProcCoefficient = cbBombletProcCoefficient.Value;
                //pie.impactEffect = Resources.Load<GameObject>("prefabs/effects/igniteexplosionvfx");
                //Destroy(ClusterBombletObject.GetComponent<ProjectileStickOnImpact>());
                ClusterBombletObject.GetComponent<ProjectileStickOnImpact>().ignoreWorld = true;

                ProjectileSimple ps = ClusterBombletObject.GetComponent<ProjectileSimple>();
                ps.velocity = 12f;

                ProjectileDamage pd = ClusterBombletObject.GetComponent<ProjectileDamage>();
                pd.damageType = DamageType.Stun1s;
            }

            PrepBarrage.baseDuration = reuDraw.Value;
            PrepBarrage.prepSoundString = "Play_bandit_M2_load";

            FireBarrage.maxBullets = reuBullets.Value;
            FireBarrage.damageCoefficient = reuDamage.Value;
            FireBarrage.force = reuForce.Value;
            FireBarrage.baseDuration = reuFireRate.Value;
            FireBarrage.gracePeriodMin = reuGracePeriodMin.Value;
            FireBarrage.gracePeriodMax = reuGracePeriodMax.Value;
            FireBarrage.executeThreshold = reuExecuteThreshold.Value;
            FireBarrage.executeBosses = reuExecuteBosses.Value;
            FireBarrage.recoilAmplitude = 2.2f;
            FireBarrage.spread = reuSpread.Value;
            FireBarrage.effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
            FireBarrage.hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
            FireBarrage.tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");
            FireBarrage.attackSoundString = "Play_bandit_M2_shot";
            FireBarrage.buffDamageCoefficient = reuDebuffDamage.Value;
            FireBarrage.endLag = reuEndLag.Value;

            PrepBarrageScepter.baseDuration = reuDraw.Value;
            PrepBarrageScepter.prepSoundString = "Play_bandit_M2_load";

            FireBarrageScepter.maxBullets = reuBullets.Value * 2;
            FireBarrageScepter.damageCoefficient = reuDamage.Value;
            FireBarrageScepter.force = reuForce.Value;
            FireBarrageScepter.baseDuration = reuFireRate.Value;
            FireBarrageScepter.gracePeriodMin = reuGracePeriodMin.Value;
            FireBarrageScepter.gracePeriodMax = reuGracePeriodMax.Value;
            FireBarrageScepter.executeThreshold = reuExecuteThreshold.Value;
            FireBarrageScepter.executeBosses = reuExecuteBosses.Value;
            FireBarrageScepter.recoilAmplitude = 2.2f;
            FireBarrageScepter.spread = reuSpread.Value;
            FireBarrageScepter.effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
            FireBarrageScepter.hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
            FireBarrageScepter.tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");
            FireBarrageScepter.attackSoundString = "Play_bandit_M2_shot";
            FireBarrageScepter.buffDamageCoefficient = reuDebuffDamage.Value;
            FireBarrageScepter.endLag = reuEndLag.Value;

            PrepLightsOutScepter.baseDuration = loFireRate.Value;
            PrepLightsOutScepter.prepSoundString = "Play_bandit_M2_load";
            PrepLightsOutScepter.specialCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/banditcrosshairrevolver");

            FireLightsOutScepter.damageCoefficient = loDamage.Value * 2f;
            FireLightsOutScepter.force = loForce.Value;
            FireLightsOutScepter.baseDuration = loEndLag.Value;
            FireLightsOutScepter.gracePeriodMin = loGracePeriodMin.Value;
            FireLightsOutScepter.gracePeriodMax = loGracePeriodMax.Value;
            FireLightsOutScepter.executeThreshold = loExecuteThreshold.Value;
            FireLightsOutScepter.executeBosses = loExecuteBosses.Value;
            FireLightsOutScepter.recoilAmplitude = 4f;
            FireLightsOutScepter.effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditpistol");
            FireLightsOutScepter.hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/hitsparkbanditpistol");
            FireLightsOutScepter.tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditpistol");
            FireLightsOutScepter.attackSoundString = "Play_bandit_M2_shot";
            FireLightsOutScepter.buffDamageCoefficient = loBuffDamage.Value * 2f;

            if (!started)
            {
                started = true;
                LoadoutAPI.AddSkill(typeof(Blast));
                LoadoutAPI.AddSkill(typeof(CastSmokescreenNoDelay));
                LoadoutAPI.AddSkill(typeof(CastSmokescreen));
                //LoadoutAPI.AddSkill(typeof(Assassinate));
                //LoadoutAPI.AddSkill(typeof(FireChargeShot));
                LoadoutAPI.AddSkill(typeof(PrepLightsOut));
                LoadoutAPI.AddSkill(typeof(FireLightsOut));
                LoadoutAPI.AddSkill(typeof(GrenadeToss));
                LoadoutAPI.AddSkill(typeof(ThermiteBomb));
                LoadoutAPI.AddSkill(typeof(Scatter));
                LoadoutAPI.AddSkill(typeof(ClusterBomb));
                LoadoutAPI.AddSkill(typeof(PrepBarrage));
                LoadoutAPI.AddSkill(typeof(FireBarrage));
            }
        }

        //Assign skills to the loadout
        private void AssignSkills()
        {
            SkillFamily primarySkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            primarySkillFamily.defaultVariantIndex = 0u;
            primarySkillFamily.variants = new SkillFamily.Variant[1];

            SkillFamily secondarySkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            secondarySkillFamily.defaultVariantIndex = 0u;
            secondarySkillFamily.variants = new SkillFamily.Variant[1];

            SkillFamily utilitySkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            utilitySkillFamily.defaultVariantIndex = 0u;
            utilitySkillFamily.variants = new SkillFamily.Variant[1];

            SkillFamily specialSkillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            specialSkillFamily.defaultVariantIndex = 0u;
            specialSkillFamily.variants = new SkillFamily.Variant[1];

            SkillLocator skillComponent = BanditBody.GetComponent<SkillLocator>();
            Reflection.SetFieldValue<SkillFamily>(skillComponent.primary, "_skillFamily", primarySkillFamily);
            Reflection.SetFieldValue<SkillFamily>(skillComponent.secondary, "_skillFamily", secondarySkillFamily);
            Reflection.SetFieldValue<SkillFamily>(skillComponent.utility, "_skillFamily", utilitySkillFamily);
            Reflection.SetFieldValue<SkillFamily>(skillComponent.special, "_skillFamily", specialSkillFamily);

            LanguageAPI.Add("BANDITRELOADED_PASSIVE_NAME", "Quickdraw");
            LanguageAPI.Add("BANDITRELOADED_PASSIVE_DESCRIPTION", "The Bandit <style=cIsUtility>automatically reloads</style> his primary when using other skills.");
            if (usePassive.Value)
            {
                skillComponent.passiveSkill.enabled = true;
                skillComponent.passiveSkill.skillNameToken = "BANDITRELOADED_PASSIVE_NAME";
                skillComponent.passiveSkill.skillDescriptionToken = "BANDITRELOADED_PASSIVE_DESCRIPTION";
                skillComponent.passiveSkill.icon = iconPassive;
            }

            #region Blast
            primaryBlastDef = SkillDef.CreateInstance<SkillDef>();
            primaryBlastDef.activationState = new SerializableEntityStateType(typeof(Blast));

            primaryBlastDef.baseRechargeInterval = blastRechargeInterval.Value;
            if (primaryBlastDef.baseRechargeInterval > 0f)
            {
                primaryBlastDef.baseMaxStock = blastStock.Value;
                // if (!blastIndividualReload.Value)
                //{
                primaryBlastDef.rechargeStock = primaryBlastDef.baseMaxStock;
                primaryBlastDef.isBullets = true;
                /*}
                else
                {
                    primaryBlastDef.rechargeStock = 1;
                    primaryBlastDef.isBullets = false;
                }*/
                Blast.noReload = false;
            }
            else
            {
                primaryBlastDef.isBullets = true;
                primaryBlastDef.baseRechargeInterval = 0f;
                primaryBlastDef.baseMaxStock = 1;
                primaryBlastDef.rechargeStock = 1;
                Blast.noReload = true;
            }
            primaryBlastDef.skillDescriptionToken = "";
            primaryBlastDef.skillDescriptionToken += "Fire a powerful slug " + (Blast.penetrateEnemies ? "that pierces enemies " : "") + "for <style=cIsDamage>" + Blast.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            if (primaryBlastDef.baseRechargeInterval > 0f)
            {
                //primaryBlastDef.skillDescriptionToken += (blastIndividualReload.Value ? " Hold up to " : " Reload every ") + primaryBlastDef.baseMaxStock + " shots.";
                primaryBlastDef.skillDescriptionToken += " Reload every " + primaryBlastDef.baseMaxStock + " shots.";
            }
            primaryBlastDef.skillDescriptionToken += Environment.NewLine;

            primaryBlastDef.skillName = "FireSlug";
            primaryBlastDef.skillNameToken = "Blast";
            primaryBlastDef.activationStateMachineName = "Weapon";
            primaryBlastDef.shootDelay = 0;
            primaryBlastDef.beginSkillCooldownOnSkillEnd = false;
            primaryBlastDef.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryBlastDef.isCombatSkill = true;
            primaryBlastDef.noSprint = true;
            primaryBlastDef.canceledFromSprinting = false;
            primaryBlastDef.mustKeyPress = false;
            primaryBlastDef.icon = iconSkill1;

            primaryBlastDef.requiredStock = blastDryEnabled.Value ? 0 : 1;
            primaryBlastDef.stockToConsume = 1;

            LoadoutAPI.AddSkillDef(primaryBlastDef);
            primarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primaryBlastDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(primaryBlastDef.skillNameToken, false)
            };
            #endregion

            #region scatter
            primaryScatterDef = SkillDef.CreateInstance<SkillDef>();
            primaryScatterDef.activationState = new SerializableEntityStateType(typeof(Scatter));

            primaryScatterDef.baseRechargeInterval = scatterRechargeInterval.Value;
            if (primaryScatterDef.baseRechargeInterval > 0f)
            {
                primaryScatterDef.baseMaxStock = scatterStock.Value;
                //if (!scatterIndividualReload.Value)
                //{
                primaryScatterDef.rechargeStock = primaryScatterDef.baseMaxStock;
                primaryScatterDef.isBullets = true;
                /*}
                else
                {
                    primaryScatterDef.rechargeStock = 1;
                    primaryScatterDef.isBullets = false;
                
                }*/
                Scatter.noReload = false;
            }
            else
            {
                primaryScatterDef.isBullets = true;
                primaryScatterDef.baseRechargeInterval = 0f;
                primaryScatterDef.baseMaxStock = 1;
                primaryScatterDef.rechargeStock = 1;
                Scatter.noReload = true;
            }

            primaryScatterDef.skillName = "FireScatter";
            primaryScatterDef.skillNameToken = "Scatter";
            primaryScatterDef.skillDescriptionToken = "";
            primaryScatterDef.skillDescriptionToken += "Fire a volley of " + (Scatter.penetrateEnemies ? "piercing flechettes " : "buckshot ") + "for <style=cIsDamage>" + Scatter.pelletCount + "x" + Scatter.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            if (primaryScatterDef.baseRechargeInterval > 0f)
            {
                primaryScatterDef.skillDescriptionToken += " Reload every " + primaryScatterDef.baseMaxStock + " shots.";
            }
            primaryScatterDef.skillDescriptionToken += Environment.NewLine;
            primaryScatterDef.activationStateMachineName = "Weapon";
            primaryScatterDef.shootDelay = 0;
            primaryScatterDef.beginSkillCooldownOnSkillEnd = false;
            primaryScatterDef.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryScatterDef.isCombatSkill = true;
            primaryScatterDef.noSprint = true;
            primaryScatterDef.canceledFromSprinting = false;
            primaryScatterDef.mustKeyPress = false;
            primaryScatterDef.icon = iconSkill1a;
            primaryScatterDef.requiredStock = scatterDryEnabled.Value ? 0 : 1;
            primaryScatterDef.stockToConsume = 1;

            LoadoutAPI.AddSkillDef(primaryScatterDef);
            #endregion

            #region CastSmokescreen
            SkillDef utilityDefVA = SkillDef.CreateInstance<SkillDef>();
            utilityDefVA.activationState = new SerializableEntityStateType(typeof(EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay));
            utilityDefVA.baseRechargeInterval = cloakCooldown.Value;
            utilityDefVA.skillName = "Cloak";
            utilityDefVA.skillNameToken = "Smokebomb";
            utilityDefVA.skillDescriptionToken = "<color=#95CDE5>Turn invisible</color>. After " + EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.duration.ToString("N1") + " seconds or after using another ability, surprise and <style=cIsDamage>stun enemies for " + EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            utilityDefVA.skillDescriptionToken += Environment.NewLine;
            utilityDefVA.baseMaxStock = cloakStock.Value;
            utilityDefVA.rechargeStock = 1;
            utilityDefVA.isBullets = false;
            utilityDefVA.shootDelay = 0f;
            utilityDefVA.beginSkillCooldownOnSkillEnd = true;
            utilityDefVA.activationStateMachineName = "Weapon";
            utilityDefVA.interruptPriority = EntityStates.InterruptPriority.Skill;
            utilityDefVA.isCombatSkill = false;
            utilityDefVA.noSprint = false;
            utilityDefVA.canceledFromSprinting = false;
            utilityDefVA.mustKeyPress = false;
            utilityDefVA.icon = iconSkill3;
            utilityDefVA.requiredStock = 1;
            utilityDefVA.stockToConsume = 1;
            utilityDefVA.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_INVIS", "KEYWORD_STUNNING" };
            LoadoutAPI.AddSkillDef(utilityDefVA);

            SkillDef utilityDefVB = SkillDef.CreateInstance<SkillDef>();
            utilityDefVB.activationState = new SerializableEntityStateType(typeof(EntityStates.Commando.CommandoWeapon.CastSmokescreen));
            utilityDefVB.baseRechargeInterval = cloakCooldown.Value;
            utilityDefVB.skillName = "CloakAnim";
            utilityDefVB.skillNameToken = "Smokebomb";
            utilityDefVB.skillDescriptionToken = "<color=#95CDE5>Turn invisible</color>. After " + (EntityStates.Commando.CommandoWeapon.CastSmokescreen.stealthDuration - 1f).ToString("N1") + " seconds or after using another ability, surprise and <style=cIsDamage>stun enemies for " + EntityStates.Commando.CommandoWeapon.CastSmokescreen.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            utilityDefVB.skillDescriptionToken += Environment.NewLine;
            utilityDefVB.baseMaxStock = cloakStock.Value;
            utilityDefVB.rechargeStock = 1;
            utilityDefVB.isBullets = false;
            utilityDefVB.shootDelay = 0f;
            utilityDefVB.beginSkillCooldownOnSkillEnd = true;
            utilityDefVB.activationStateMachineName = "Weapon";
            utilityDefVB.interruptPriority = EntityStates.InterruptPriority.Skill;
            utilityDefVB.isCombatSkill = false;
            utilityDefVB.noSprint = false;
            utilityDefVB.canceledFromSprinting = false;
            utilityDefVB.mustKeyPress = false;
            utilityDefVB.icon = iconSkill3;
            utilityDefVB.requiredStock = 1;
            utilityDefVB.stockToConsume = 1;
            utilityDefVB.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_INVIS", "KEYWORD_STUNNING" };
            LoadoutAPI.AddSkillDef(utilityDefVB);

            utilityDefA = SkillDef.CreateInstance<SkillDef>();
            utilityDefA.activationState = new SerializableEntityStateType(typeof(CastSmokescreenNoDelay));
            utilityDefA.baseRechargeInterval = cloakCooldown.Value;
            utilityDefA.skillName = "CloakBanditReloaded";
            utilityDefA.skillNameToken = "Smokebomb";
            utilityDefA.skillDescriptionToken = "<color=#95CDE5>Turn invisible</color>. After " + CastSmokescreenNoDelay.duration.ToString("N1") + " seconds or after using another ability, surprise and <style=cIsDamage>stun enemies for " + CastSmokescreenNoDelay.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            utilityDefA.skillDescriptionToken += Environment.NewLine;
            utilityDefA.baseMaxStock = cloakStock.Value;
            utilityDefA.rechargeStock = 1;
            utilityDefA.isBullets = false;
            utilityDefA.shootDelay = 0f;
            utilityDefA.beginSkillCooldownOnSkillEnd = true;
            utilityDefA.activationStateMachineName = "Weapon";
            utilityDefA.interruptPriority = EntityStates.InterruptPriority.Skill;
            utilityDefA.isCombatSkill = false;
            utilityDefA.noSprint = false;
            utilityDefA.canceledFromSprinting = false;
            utilityDefA.mustKeyPress = false;
            utilityDefA.icon = iconSkill3;
            utilityDefA.requiredStock = 1;
            utilityDefA.stockToConsume = 1;
            utilityDefA.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_INVIS", "KEYWORD_STUNNING" };
            LoadoutAPI.AddSkillDef(utilityDefA);

            utilityDefB = SkillDef.CreateInstance<SkillDef>();
            utilityDefB.activationState = new SerializableEntityStateType(typeof(CastSmokescreen));
            utilityDefB.baseRechargeInterval = cloakCooldown.Value;
            utilityDefB.skillName = "CloakAnimBanditReloaded";
            utilityDefB.skillNameToken = "Smokebomb";
            utilityDefB.skillDescriptionToken = "<color=#95CDE5>Turn invisible</color>. After " + (CastSmokescreen.stealthDuration - 1f).ToString("N1") + " seconds or after using another ability, surprise and <style=cIsDamage>stun enemies for " + CastSmokescreen.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            utilityDefB.skillDescriptionToken += Environment.NewLine;
            utilityDefB.baseMaxStock = cloakStock.Value;
            utilityDefB.rechargeStock = 1;
            utilityDefB.isBullets = false;
            utilityDefB.shootDelay = 0f;
            utilityDefB.beginSkillCooldownOnSkillEnd = true;
            utilityDefB.activationStateMachineName = "Weapon";
            utilityDefB.interruptPriority = EntityStates.InterruptPriority.Skill;
            utilityDefB.isCombatSkill = false;
            utilityDefB.noSprint = false;
            utilityDefB.canceledFromSprinting = false;
            utilityDefB.mustKeyPress = false;
            utilityDefB.icon = iconSkill3;
            utilityDefB.requiredStock = 1;
            utilityDefB.stockToConsume = 1;
            utilityDefB.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_INVIS", "KEYWORD_STUNNING" };
            LoadoutAPI.AddSkillDef(utilityDefB);
            #endregion
            #region Assassinate
            utilityAltDef = SkillDef.CreateInstance<SkillDef>();
            utilityAltDef.activationState = new SerializableEntityStateType(typeof(Assassinate));

            utilityAltDef.baseRechargeInterval = asCooldown.Value;
            utilityAltDef.skillName = "Assassinate";
            utilityAltDef.skillNameToken = "Assassinate";
            utilityAltDef.skillDescriptionToken = "Charge up your gun and fire a high caliber shot that pierces enemies for <style=cIsDamage>" + FireChargeShot.minDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + "-" + FireChargeShot.maxDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>";
            utilityAltDef.skillDescriptionToken += ".";
            if (asSelfForceMax.Value != 0 && asSelfForceMin.Value != 0)
            {
                utilityAltDef.skillDescriptionToken += " <style=cIsUtility>Pushes you backwards</style> if you are airborn.";
            }
            utilityAltDef.skillDescriptionToken += Environment.NewLine;
            utilityAltDef.baseMaxStock = asStock.Value;
            utilityAltDef.rechargeStock = 1;
            utilityAltDef.isBullets = false;
            utilityAltDef.shootDelay = 0f;
            utilityAltDef.beginSkillCooldownOnSkillEnd = true;
            utilityAltDef.activationStateMachineName = "Weapon";
            utilityAltDef.interruptPriority = EntityStates.InterruptPriority.Skill;//should be .Skill if utility
            utilityAltDef.isCombatSkill = true;
            utilityAltDef.noSprint = true;
            utilityAltDef.canceledFromSprinting = false;
            utilityAltDef.mustKeyPress = false;
            utilityAltDef.icon = iconSkill3a;
            utilityAltDef.requiredStock = 1;
            utilityAltDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(utilityAltDef);
            #endregion

            #region LightsOut
            specialLightsOutDef = SkillDef.CreateInstance<SkillDef>();
            specialLightsOutDef.activationState = new SerializableEntityStateType(typeof(PrepLightsOut));
            //todo: add keywords

            specialLightsOutDef.skillDescriptionToken = "";
            if (loExecuteThreshold.Value > 0f)
            {
                specialLightsOutDef.skillDescriptionToken += "<style=cIsDamage>Executing</style>. ";
                specialLightsOutDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_EXECUTE", "KEYWORD_BANDITRELOADED_DEBUFFBOOST1", "KEYWORD_BANDITRELOADED_RESET" };
            }
            else
            {
                specialLightsOutDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_DEBUFFBOOST1", "KEYWORD_BANDITRELOADED_RESET" };
            }
            specialLightsOutDef.skillDescriptionToken += "<style=cIsDamage>Compromising</style>. Take aim with your Persuader, <style=cIsDamage>dealing " + FireLightsOut.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialLightsOutDef.skillDescriptionToken += " Enemies hit by this ability are <style=cIsUtility>Marked</style>.";
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST1", "<style=cKeywordName>Compromising</style><style=cSub>The skill deals <style=cIsDamage>+" + FireLightsOut.buffDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> for each debuff on the enemy including <style=cIsDamage>stun</style>, <style=cIsDamage>shock</style>, and <style=cIsUtility>freeze</style>. The damage bonus is only applied <style=cIsHealth>once</style> per debuff stack.</style>");
            specialLightsOutDef.skillDescriptionToken += Environment.NewLine;
            specialLightsOutDef.baseRechargeInterval = loCooldown.Value;
            specialLightsOutDef.skillNameToken = "Lights Out";
            specialLightsOutDef.skillName = "LightsOut";
            specialLightsOutDef.baseMaxStock = loStock.Value;
            specialLightsOutDef.rechargeStock = 1;
            specialLightsOutDef.isBullets = false;
            specialLightsOutDef.shootDelay = 0f;
            specialLightsOutDef.activationStateMachineName = "Weapon";
            specialLightsOutDef.icon = iconSkill4;
            specialLightsOutDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            specialLightsOutDef.beginSkillCooldownOnSkillEnd = true;
            specialLightsOutDef.isCombatSkill = true;
            specialLightsOutDef.canceledFromSprinting = false;
            specialLightsOutDef.noSprint = true;
            specialLightsOutDef.mustKeyPress = false;
            specialLightsOutDef.requiredStock = 1;
            specialLightsOutDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(specialLightsOutDef);
            #endregion

            #region Thermite Bomb
            thermiteDef = SkillDef.CreateInstance<SkillDef>();
            thermiteDef.activationState = new SerializableEntityStateType(typeof(ThermiteBomb));
            thermiteDef.baseRechargeInterval = thermiteCooldown.Value;
            thermiteDef.skillNameToken = "Thermite Bomb";
            thermiteDef.skillDescriptionToken = "<style=cIsUtility>Stackable</style>. Toss a fiery bomb that <style=cIsDamage>ignites</style> and <style=cIsUtility>slows</style> enemies for <style=cIsDamage>" + (ThermiteBomb.damageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            //thermiteDef.skillDescriptionToken += " The <style=cIsDamage>afterburn</style> deals up to <style=cIsDamage>3x</style> damage to low health enemies.";
            thermiteDef.skillDescriptionToken += Environment.NewLine;
            thermiteDef.skillName = "Thermite";
            thermiteDef.icon = iconSkill2;
            thermiteDef.shootDelay = 0.3f;
            thermiteDef.baseMaxStock = thermiteStock.Value;
            thermiteDef.rechargeStock = 1;
            thermiteDef.isBullets = false;
            thermiteDef.beginSkillCooldownOnSkillEnd = false;
            thermiteDef.activationStateMachineName = "Weapon";
            thermiteDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            thermiteDef.isCombatSkill = true;
            thermiteDef.noSprint = false;
            thermiteDef.canceledFromSprinting = false;
            thermiteDef.mustKeyPress = false;
            thermiteDef.requiredStock = 1;
            thermiteDef.stockToConsume = 1;
            thermiteDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_STACKABLE" };
            LoadoutAPI.AddSkillDef(thermiteDef);
            #endregion

            #region Acid Bomb
            acidBombDef = SkillDef.CreateInstance<SkillDef>();
            acidBombDef.activationState = new SerializableEntityStateType(typeof(GrenadeToss));
            acidBombDef.baseRechargeInterval = acidCooldown.Value;
            acidBombDef.skillNameToken = "Acid Bomb";
            acidBombDef.skillDescriptionToken = "Toss a caustic grenade that <style=cIsHealing>Weakens</style> enemies for <style=cIsDamage>" + (GrenadeToss.damageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            acidBombDef.skillDescriptionToken += Environment.NewLine;
            acidBombDef.skillName = "Grenade";
            acidBombDef.icon = iconSkill2a;
            acidBombDef.shootDelay = 0.3f;
            acidBombDef.baseMaxStock = acidStock.Value;
            acidBombDef.rechargeStock = 1;
            acidBombDef.isBullets = false;
            acidBombDef.beginSkillCooldownOnSkillEnd = false;
            acidBombDef.activationStateMachineName = "Weapon";
            acidBombDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            acidBombDef.isCombatSkill = true;
            acidBombDef.noSprint = false;
            acidBombDef.canceledFromSprinting = false;
            acidBombDef.mustKeyPress = false;
            acidBombDef.requiredStock = 1;
            acidBombDef.stockToConsume = 1;
            acidBombDef.keywordTokens = new string[] { "KEYWORD_WEAK" };
            LoadoutAPI.AddSkillDef(acidBombDef);
            #endregion

            #region Cluster Bomb
            clusterBombDef = SkillDef.CreateInstance<SkillDef>();
            clusterBombDef.activationState = new SerializableEntityStateType(typeof(ClusterBomb));
            clusterBombDef.baseRechargeInterval = cbCooldown.Value;
            clusterBombDef.skillNameToken = "Cluster Bomb";
            clusterBombDef.skillDescriptionToken = "<style=cIsDamage>Stunning</style>. Toss a MIRV grenade that explodes for <style=cIsDamage>" + (ClusterBomb.damageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>, then splits into <style=cIsDamage>" + ClusterBomb.bombletCount + "</style> bomblets that deal <style=cIsDamage>" + (ClusterBomb.bombletDamageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            clusterBombDef.skillDescriptionToken += Environment.NewLine;
            clusterBombDef.skillName = "ClusterGrenade";
            clusterBombDef.icon = iconClusterBomb;
            clusterBombDef.shootDelay = 0.3f;
            clusterBombDef.baseMaxStock = cbStock.Value;
            clusterBombDef.rechargeStock = 1;
            clusterBombDef.isBullets = false;
            clusterBombDef.beginSkillCooldownOnSkillEnd = false;
            clusterBombDef.activationStateMachineName = "Weapon";
            clusterBombDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            clusterBombDef.isCombatSkill = true;
            clusterBombDef.noSprint = false;
            clusterBombDef.canceledFromSprinting = false;
            clusterBombDef.mustKeyPress = false;
            clusterBombDef.requiredStock = 1;
            clusterBombDef.stockToConsume = 1;
            clusterBombDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            LoadoutAPI.AddSkillDef(clusterBombDef);
            #endregion

            #region barrage
            specialBarrageDef = SkillDef.CreateInstance<SkillDef>();
            specialBarrageDef.activationState = new SerializableEntityStateType(typeof(PrepBarrage));
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST2", "<style=cKeywordName>Compromising</style><style=cSub>Each shot deals <style=cIsDamage>+" + FireBarrage.buffDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> for each debuff on the enemy including <style=cIsDamage>stun</style>, <style=cIsDamage>shock</style>, and <style=cIsUtility>freeze</style>. The damage bonus is only applied <style=cIsHealth>once</style> per debuff stack.</style>");
            if (reuExecuteThreshold.Value > 0f)
            {
                specialBarrageDef.skillDescriptionToken = " <style=cIsDamage>Executing</style>. ";
                specialBarrageDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_EXECUTE", "KEYWORD_BANDITRELOADED_DEBUFFBOOST2", "KEYWORD_BANDITRELOADED_RESET" };
            }
            else
            {
                specialBarrageDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_DEBUFFBOOST2", "KEYWORD_BANDITRELOADED_RESET" };
            }

            specialBarrageDef.skillDescriptionToken += "<style=cIsDamage>Compromising</style>. Rapidly fire your Persuader, dealing <style=cIsDamage>" + FireBarrage.maxBullets + "x" + FireBarrage.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialBarrageDef.skillDescriptionToken += " Consecutive hits deal <style=cIsDamage>+" + FireBarrage.buffDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> each.";
            specialBarrageDef.skillDescriptionToken += " Enemies hit by this ability are <style=cIsUtility>Marked</style>.";
            //specialBarrageDef.skillDescriptionToken += " Deals an additional <style=cIsDamage>+" + FireBarrage.buffDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> for <style=cIsDamage>each debuff</style> present on the enemy.";
            specialBarrageDef.skillDescriptionToken += Environment.NewLine;
            specialBarrageDef.baseRechargeInterval = reuCooldown.Value;
            specialBarrageDef.skillNameToken = "Rack em Up";
            specialBarrageDef.skillName = "BanditBarrage";
            specialBarrageDef.baseMaxStock = reuStock.Value;
            specialBarrageDef.rechargeStock = 1;
            specialBarrageDef.isBullets = false;
            specialBarrageDef.shootDelay = 0f;
            specialBarrageDef.activationStateMachineName = "Weapon";
            specialBarrageDef.icon = iconSkill3a;
            specialBarrageDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            specialBarrageDef.beginSkillCooldownOnSkillEnd = true;
            specialBarrageDef.isCombatSkill = true;
            specialBarrageDef.canceledFromSprinting = false;
            specialBarrageDef.noSprint = true;
            specialBarrageDef.mustKeyPress = false;
            specialBarrageDef.requiredStock = 1;
            specialBarrageDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(specialBarrageDef);

            specialBarrageScepterDef = SkillDef.CreateInstance<SkillDef>();
            specialBarrageScepterDef.activationState = new SerializableEntityStateType(typeof(PrepBarrageScepter));
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST2", "<style=cKeywordName>Compromising</style><style=cSub>Each shot deals <style=cIsDamage>+" + FireBarrageScepter.buffDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> for each debuff on the enemy including <style=cIsDamage>stun</style>, <style=cIsDamage>shock</style>, and <style=cIsUtility>freeze</style>. The damage bonus is only applied <style=cIsHealth>once</style> per debuff stack.</style>");
            if (reuExecuteThreshold.Value > 0f)
            {
                specialBarrageScepterDef.skillDescriptionToken = " <style=cIsDamage>Executing</style>. ";
                specialBarrageScepterDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_EXECUTE", "KEYWORD_BANDITRELOADED_DEBUFFBOOST2", "KEYWORD_BANDITRELOADED_RESET" };
            }
            else
            {
                specialBarrageScepterDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_DEBUFFBOOST2", "KEYWORD_BANDITRELOADED_RESET" };
            }

            specialBarrageScepterDef.skillDescriptionToken += "<style=cIsDamage>Compromising</style>. Rapidly fire your Persuader, dealing <style=cIsDamage>" + FireBarrageScepter.maxBullets + "x" + FireBarrageScepter.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialBarrageScepterDef.skillDescriptionToken += " Consecutive hits deal <style=cIsDamage>+" + FireBarrageScepter.buffDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> each.";
            specialBarrageScepterDef.skillDescriptionToken += " Enemies hit by this ability are <style=cIsUtility>Marked</style>.";
            //specialBarrageScepterDef.skillDescriptionToken += " Deals an additional <style=cIsDamage>+" + FireBarrageScepter.buffDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> for <style=cIsDamage>each debuff</style> present on the enemy.";
            specialBarrageScepterDef.skillDescriptionToken += Environment.NewLine;
            specialBarrageScepterDef.baseRechargeInterval = reuCooldown.Value;
            specialBarrageScepterDef.skillNameToken = "Fistful of Lead";
            specialBarrageScepterDef.skillName = "BanditBarrageScepter";
            specialBarrageScepterDef.baseMaxStock = reuStock.Value;
            specialBarrageScepterDef.rechargeStock = 1;
            specialBarrageScepterDef.isBullets = false;
            specialBarrageScepterDef.shootDelay = 0f;
            specialBarrageScepterDef.activationStateMachineName = "Weapon";
            specialBarrageScepterDef.icon = iconSkill3a;
            specialBarrageScepterDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            specialBarrageScepterDef.beginSkillCooldownOnSkillEnd = true;
            specialBarrageScepterDef.isCombatSkill = true;
            specialBarrageScepterDef.canceledFromSprinting = false;
            specialBarrageScepterDef.noSprint = true;
            specialBarrageScepterDef.mustKeyPress = false;
            specialBarrageScepterDef.requiredStock = 1;
            specialBarrageScepterDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(specialBarrageScepterDef);

            specialLightsOutScepterDef = SkillDef.CreateInstance<SkillDef>();
            specialLightsOutScepterDef.activationState = new SerializableEntityStateType(typeof(PrepLightsOutScepter));

            specialLightsOutScepterDef.skillDescriptionToken = "";
            if (loExecuteThreshold.Value > 0f)
            {
                specialLightsOutScepterDef.skillDescriptionToken += "<style=cIsDamage>Executing</style>. ";
                specialLightsOutScepterDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_EXECUTE", "KEYWORD_BANDITRELOADED_DEBUFFBOOST1", "KEYWORD_BANDITRELOADED_RESET" };
            }
            else
            {
                specialLightsOutScepterDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_DEBUFFBOOST1", "KEYWORD_BANDITRELOADED_RESET" };
            }
            specialLightsOutScepterDef.skillDescriptionToken += "<style=cIsDamage>Compromising</style>. Take aim with your Persuader, <style=cIsDamage>dealing " + FireLightsOutScepter.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialLightsOutScepterDef.skillDescriptionToken += " Enemies hit by this ability are <style=cIsUtility>Marked</style>.";
            
            specialLightsOutScepterDef.skillDescriptionToken += Environment.NewLine;
            specialLightsOutScepterDef.baseRechargeInterval = loCooldown.Value;
            specialLightsOutScepterDef.skillNameToken = "Decapitate";
            specialLightsOutScepterDef.skillName = "LightsOutScepter";
            specialLightsOutScepterDef.baseMaxStock = loStock.Value;
            specialLightsOutScepterDef.rechargeStock = 1;
            specialLightsOutScepterDef.isBullets = false;
            specialLightsOutScepterDef.shootDelay = 0f;
            specialLightsOutScepterDef.activationStateMachineName = "Weapon";
            specialLightsOutScepterDef.icon = iconSkill4;
            specialLightsOutScepterDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            specialLightsOutScepterDef.beginSkillCooldownOnSkillEnd = true;
            specialLightsOutScepterDef.isCombatSkill = true;
            specialLightsOutScepterDef.canceledFromSprinting = false;
            specialLightsOutScepterDef.noSprint = true;
            specialLightsOutScepterDef.mustKeyPress = false;
            specialLightsOutScepterDef.requiredStock = 1;
            specialLightsOutScepterDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(specialLightsOutScepterDef);
            #endregion

            Array.Resize(ref primarySkillFamily.variants, primarySkillFamily.variants.Length + 1);
            primarySkillFamily.variants[primarySkillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = primaryScatterDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(primaryScatterDef.skillNameToken, false)
            };

            Array.Resize(ref secondarySkillFamily.variants, secondarySkillFamily.variants.Length + 1);
            secondarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = thermiteDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(thermiteDef.skillNameToken, false)
            };
            secondarySkillFamily.variants[secondarySkillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = acidBombDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(acidBombDef.skillNameToken, false)
            };
            Array.Resize(ref secondarySkillFamily.variants, secondarySkillFamily.variants.Length + 1);
            secondarySkillFamily.variants[secondarySkillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = clusterBombDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(clusterBombDef.skillNameToken, false)
            };


            SkillDef toInsert = utilityDefA;
            if (vanillaCloak.Value)
            {
                if (cloakAnim.Value)
                {
                    toInsert = utilityDefVB;
                }
                else
                {
                    toInsert = utilityDefVA;
                }
            }
            else
            {
                if (cloakAnim.Value)
                {
                    toInsert = utilityDefB;
                }
                else
                {
                    toInsert = utilityDefA;
                }
            }

            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = toInsert,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(toInsert.skillNameToken, false)
            };
            if (asEnabled.Value)
            {
                Array.Resize(ref utilitySkillFamily.variants, utilitySkillFamily.variants.Length + 1);
                //Array.Resize(ref primarySkillFamily.variants, primarySkillFamily.variants.Length + 1);//remove if undoing assassinate change
                utilitySkillFamily.variants[utilitySkillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = utilityAltDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(utilityAltDef.skillNameToken, false)
                };
            }



            specialSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialLightsOutDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(specialLightsOutDef.skillNameToken, false)
            };
            Array.Resize(ref specialSkillFamily.variants, specialSkillFamily.variants.Length + 1);
            specialSkillFamily.variants[specialSkillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = specialBarrageDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(specialBarrageDef.skillNameToken, false)
            };

            LoadoutAPI.AddSkillFamily(primarySkillFamily);
            LoadoutAPI.AddSkillFamily(secondarySkillFamily);
            LoadoutAPI.AddSkillFamily(utilitySkillFamily);
            LoadoutAPI.AddSkillFamily(specialSkillFamily);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems"))
            {
                SetupScepter();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            ThinkInvisible.ClassicItems.Scepter_V2.instance.RegisterScepterSkill(specialLightsOutScepterDef, BanditBodyName, SkillSlot.Special, 0);
            ThinkInvisible.ClassicItems.Scepter_V2.instance.RegisterScepterSkill(specialBarrageScepterDef, BanditBodyName, SkillSlot.Special, 1);
        }

        private void SetIcons()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BanditReloaded.icons"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new R2API.AssetBundleResourcesProvider(assetPrefix, bundle);
                R2API.ResourcesAPI.AddProvider(provider);
            }
            iconSkill1 = Resources.Load<Sprite>(assetPrefix + ":skill1.png");
            iconSkill1a = Resources.Load<Sprite>(assetPrefix + ":skill1a.png");
            iconSkill2 = Resources.Load<Sprite>(assetPrefix + ":skill2.png");
            iconSkill2a = Resources.Load<Sprite>(assetPrefix + ":skill2a.png");
            iconSkill3 = Resources.Load<Sprite>(assetPrefix + ":skill3.png");
            iconSkill3a = Resources.Load<Sprite>(assetPrefix + ":skill3a.png");
            iconSkill4 = Resources.Load<Sprite>(assetPrefix + ":skill4.png");
            iconPassive = Resources.Load<Sprite>(assetPrefix + ":quickdraw.png");
            iconClusterBomb = Resources.Load<Sprite>(assetPrefix + ":clusterbomb.png");
        }

        private void SetAttributes()
        {
            BanditBody.tag = "Player";
            CharacterBody cb = BanditBody.GetComponent<CharacterBody>();
            cb.subtitleNameToken = "BANDITRELOADED_BODY_SUBTITLE";
            LanguageAPI.Add("BANDITRELOADED_BODY_SUBTITLE", "Wanted Dead or Alive");
            //cb.baseMaxHealth = maxHealth.Value;
            cb.baseMaxHealth = 100f;
            cb.baseRegen = 1f;
            cb.baseMaxShield = 0f;
            cb.baseMoveSpeed = 7f;
            cb.baseAcceleration = 80f;
            cb.baseJumpPower = 15f;
            cb.baseDamage = 12f;
            cb.baseAttackSpeed = 1f;
            cb.baseCrit = 1f;
            cb.baseArmor = 0f;
            cb.baseJumpCount = 1;

            cb.autoCalculateLevelStats = true;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.levelRegen = cb.baseRegen * 0.2f;
            cb.levelMaxShield = 0f;
            cb.levelMoveSpeed = 0f;
            cb.levelJumpPower = 0f;
            cb.levelDamage = cb.baseDamage * 0.2f;
            cb.levelAttackSpeed = 0f;
            cb.levelCrit = 0f;
            cb.levelArmor = 0f;

            cb.hideCrosshair = false;
            if (useAltCrosshair.Value)
            {
                cb.crosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/banditcrosshair");
            }
            BanditBody.AddComponent<BanditCrosshairComponent>();
        }

        private void CreateMaster()
        {
            BanditMonsterMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/charactermasters/commandomonstermaster"), "BanditReloadedMonsterMaster", true);
            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(BanditMonsterMaster);
            };

            CharacterMaster cm = BanditMonsterMaster.GetComponent<CharacterMaster>();
            cm.bodyPrefab = BanditBody;

            Component[] toDelete = BanditMonsterMaster.GetComponents<AISkillDriver>();
            foreach (AISkillDriver asd in toDelete)
            {
                Destroy(asd);
            }

            AISkillDriver ass = BanditMonsterMaster.AddComponent<AISkillDriver>();
            ass.skillSlot = SkillSlot.Utility;
            ass.requiredSkill = utilityAltDef;
            ass.requireSkillReady = true;
            ass.requireEquipmentReady = false;
            ass.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            ass.minDistance = 0f;
            ass.maxDistance = float.PositiveInfinity;
            ass.selectionRequiresTargetLoS = true;
            ass.activationRequiresTargetLoS = true;
            ass.activationRequiresAimConfirmation = true;
            ass.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            ass.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            ass.ignoreNodeGraph = false;
            ass.driverUpdateTimerOverride = 2f;
            ass.noRepeat = false;
            ass.shouldSprint = false;
            ass.shouldFireEquipment = false;
            ass.shouldTapButton = false;
            AISkillDriver reu = BanditMonsterMaster.AddComponent<AISkillDriver>();
            reu.skillSlot = SkillSlot.Special;
            reu.requiredSkill = specialBarrageDef;
            reu.requireSkillReady = true;
            reu.requireEquipmentReady = false;
            reu.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            reu.minDistance = 0f;
            reu.maxDistance = 30f;
            reu.selectionRequiresTargetLoS = true;
            reu.activationRequiresTargetLoS = true;
            reu.activationRequiresAimConfirmation = true;
            reu.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            reu.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            reu.ignoreNodeGraph = false;
            reu.driverUpdateTimerOverride = 0.6f;
            reu.noRepeat = false;
            reu.shouldSprint = false;
            reu.shouldFireEquipment = false;
            reu.shouldTapButton = false;

            AISkillDriver reposition = BanditMonsterMaster.AddComponent<AISkillDriver>();
            reposition.skillSlot = SkillSlot.Utility;
            reposition.requireSkillReady = true;
            reposition.requireEquipmentReady = false;
            reposition.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            reposition.minDistance = 0f;
            reposition.maxDistance = 10f;
            reposition.selectionRequiresTargetLoS = false;
            reposition.activationRequiresTargetLoS = false;
            reposition.activationRequiresAimConfirmation = false;
            reposition.movementType = AISkillDriver.MovementType.FleeMoveTarget;
            reposition.aimType = AISkillDriver.AimType.None;
            reposition.ignoreNodeGraph = false;
            reposition.driverUpdateTimerOverride = 3f;
            reposition.noRepeat = true;
            reposition.shouldSprint = true;
            reposition.shouldFireEquipment = false;
            reposition.shouldTapButton = true;

            AISkillDriver cloakchase = BanditMonsterMaster.AddComponent<AISkillDriver>();
            cloakchase.skillSlot = SkillSlot.Utility;
            cloakchase.requireSkillReady = true;
            cloakchase.requireEquipmentReady = false;
            cloakchase.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            cloakchase.minDistance = 0f;
            cloakchase.maxDistance = float.PositiveInfinity;
            cloakchase.selectionRequiresTargetLoS = false;
            cloakchase.activationRequiresTargetLoS = false;
            cloakchase.activationRequiresAimConfirmation = false;
            cloakchase.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            cloakchase.aimType = AISkillDriver.AimType.None;
            cloakchase.ignoreNodeGraph = false;
            cloakchase.driverUpdateTimerOverride = 3f;
            cloakchase.noRepeat = true;
            cloakchase.shouldSprint = true;
            cloakchase.shouldFireEquipment = false;
            cloakchase.shouldTapButton = true;

            AISkillDriver lo = BanditMonsterMaster.AddComponent<AISkillDriver>();
            lo.skillSlot = SkillSlot.Special;
            lo.requiredSkill = specialLightsOutDef;
            lo.requireSkillReady = true;
            lo.requireEquipmentReady = false;
            lo.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            lo.minDistance = 0f;
            lo.maxDistance = 50f;
            lo.selectionRequiresTargetLoS = true;
            lo.activationRequiresTargetLoS = true;
            lo.activationRequiresAimConfirmation = true;
            lo.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            lo.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            lo.ignoreNodeGraph = false;
            lo.driverUpdateTimerOverride = 0.6f;
            lo.noRepeat = false;
            lo.shouldSprint = false;
            lo.shouldFireEquipment = false;
            lo.shouldTapButton = false;

            AISkillDriver grenade = BanditMonsterMaster.AddComponent<AISkillDriver>();
            grenade.skillSlot = SkillSlot.Secondary;
            grenade.requireSkillReady = true;
            grenade.requireEquipmentReady = false;
            grenade.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            grenade.minDistance = 0f;
            grenade.maxDistance = 50f;
            grenade.selectionRequiresTargetLoS = true;
            grenade.activationRequiresTargetLoS = false;
            grenade.activationRequiresAimConfirmation = false;
            grenade.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            grenade.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            grenade.ignoreNodeGraph = false;
            grenade.driverUpdateTimerOverride = 0.5f;
            grenade.noRepeat = false;
            grenade.shouldSprint = false;
            grenade.shouldFireEquipment = false;
            grenade.shouldTapButton = false;

            AISkillDriver scatterAggressive = BanditMonsterMaster.AddComponent<AISkillDriver>();
            scatterAggressive.skillSlot = SkillSlot.Primary;
            scatterAggressive.requiredSkill = primaryScatterDef;
            scatterAggressive.requireSkillReady = true;
            scatterAggressive.requireEquipmentReady = false;
            scatterAggressive.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            scatterAggressive.minDistance = 0f;
            scatterAggressive.maxDistance = 20f;
            scatterAggressive.selectionRequiresTargetLoS = true;
            scatterAggressive.activationRequiresTargetLoS = false;
            scatterAggressive.activationRequiresAimConfirmation = false;
            scatterAggressive.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            scatterAggressive.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            scatterAggressive.ignoreNodeGraph = false;
            scatterAggressive.driverUpdateTimerOverride = 0.7f;
            scatterAggressive.noRepeat = false;
            scatterAggressive.shouldSprint = false;
            scatterAggressive.shouldFireEquipment = false;
            scatterAggressive.shouldTapButton = false;
            scatterAggressive.minUserHealthFraction = 0.4f;

            AISkillDriver scatter = BanditMonsterMaster.AddComponent<AISkillDriver>();
            scatter.skillSlot = SkillSlot.Primary;
            scatter.requiredSkill = primaryScatterDef;
            scatter.requireSkillReady = true;
            scatter.requireEquipmentReady = false;
            scatter.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            scatter.minDistance = 0f;
            scatter.maxDistance = 20f;
            scatter.selectionRequiresTargetLoS = true;
            scatter.activationRequiresTargetLoS = false;
            scatter.activationRequiresAimConfirmation = false;
            scatter.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            scatter.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            scatter.ignoreNodeGraph = false;
            scatter.driverUpdateTimerOverride = 0.7f;
            scatter.noRepeat = false;
            scatter.shouldSprint = false;
            scatter.shouldFireEquipment = false;
            scatter.shouldTapButton = false;

            AISkillDriver blast = BanditMonsterMaster.AddComponent<AISkillDriver>();
            blast.skillSlot = SkillSlot.Primary;
            blast.requireSkillReady = true;
            blast.requireEquipmentReady = false;
            blast.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            blast.minDistance = 8f;
            blast.maxDistance = 30f;
            blast.selectionRequiresTargetLoS = true;
            blast.activationRequiresTargetLoS = false;
            blast.activationRequiresAimConfirmation = false;
            blast.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            blast.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            blast.ignoreNodeGraph = false;
            blast.driverUpdateTimerOverride = 0.6f;
            blast.noRepeat = false;
            blast.shouldSprint = false;
            blast.shouldFireEquipment = false;
            blast.shouldTapButton = false;

            AISkillDriver chase = BanditMonsterMaster.AddComponent<AISkillDriver>();
            chase.skillSlot = SkillSlot.None;
            chase.requireSkillReady = false;
            chase.requireEquipmentReady = false;
            chase.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chase.minDistance = 0f;
            chase.maxDistance = float.PositiveInfinity;
            chase.selectionRequiresTargetLoS = false;
            chase.activationRequiresTargetLoS = false;
            chase.activationRequiresAimConfirmation = false;
            chase.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chase.aimType = AISkillDriver.AimType.AtMoveTarget;
            chase.ignoreNodeGraph = false;
            chase.driverUpdateTimerOverride = -1f;
            chase.noRepeat = false;
            chase.shouldSprint = true;
            chase.shouldFireEquipment = false;
            chase.shouldTapButton = false;

            AISkillDriver afk = BanditMonsterMaster.AddComponent<AISkillDriver>();
            afk.skillSlot = SkillSlot.None;
            afk.requireSkillReady = false;
            afk.requireEquipmentReady = false;
            afk.moveTargetType = AISkillDriver.TargetType.NearestFriendlyInSkillRange;
            afk.minDistance = 0f;
            afk.maxDistance = float.PositiveInfinity;
            afk.selectionRequiresTargetLoS = false;
            afk.activationRequiresTargetLoS = false;
            afk.activationRequiresAimConfirmation = false;
            afk.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            afk.aimType = AISkillDriver.AimType.MoveDirection;
            afk.ignoreNodeGraph = false;
            afk.driverUpdateTimerOverride = -1f;
            afk.noRepeat = false;
            afk.shouldSprint = true;
            afk.shouldFireEquipment = false;
            afk.shouldTapButton = false;
        }

        private void SetBanditBody()
        {
            ReadConfig();

            if (BanditBody == null)
            {
                if (useBodyClone)
                {
                    BanditBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/banditbody"), "BanditReloadedBody", true);
                    BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                    {
                        list.Add(BanditBody);
                    };
                }
                else
                {
                    BanditBody = Resources.Load<GameObject>("prefabs/characterbodies/banditbody");
                }
                BanditBodyName = BanditBody.name;
            }
            if (AcidBombObject == null)
            {
                if (useBodyClone)
                {
                    AcidBombObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile"), "BanditReloadedAcidBomb", true);
                    AcidBombGhostObject = PrefabAPI.InstantiateClone(AcidBombObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedAcidBombGhost", false);
                    ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                    {
                        list.Add(AcidBombObject);
                    };
                    AcidBombObject.GetComponent<ProjectileController>().ghostPrefab = AcidBombGhostObject;
                }
                else
                {
                    AcidBombObject = Resources.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile");
                    AcidBombGhostObject = Resources.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile").GetComponent<ProjectileController>().ghostPrefab;
                }
            }
            if (ThermiteObject == null)
            {
                if (useBodyClone)
                {
                    ThermiteObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/thermite"), "BanditReloadedThermite", true);
                    ThermiteGhostObject = PrefabAPI.InstantiateClone(ThermiteObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedThermiteGhost", false);
                    ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                    {
                        list.Add(ThermiteObject);
                    };
                    ThermiteObject.GetComponent<ProjectileController>().ghostPrefab = ThermiteGhostObject;
                }
                else
                {
                    ThermiteObject = Resources.Load<GameObject>("prefabs/projectiles/thermite");
                    ThermiteGhostObject = Resources.Load<GameObject>("prefabs/projectiles/thermite").GetComponent<ProjectileController>().ghostPrefab;
                }
            }
            if (ClusterBombObject == null)
            {
                if (useBodyClone)
                {
                    ClusterBombObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/BanditClusterBombSeed"), "BanditReloadedClusterBomb", true);
                    ClusterBombGhostObject = PrefabAPI.InstantiateClone(ClusterBombObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedClusterBombGhost", false);
                    ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                    {
                        list.Add(ClusterBombObject);
                    };
                    ClusterBombObject.GetComponent<ProjectileController>().ghostPrefab = ClusterBombGhostObject;
                }
                else
                {
                    ClusterBombObject = Resources.Load<GameObject>("prefabs/projectiles/BanditClusterBombSeed");
                    ClusterBombGhostObject = Resources.Load<GameObject>("prefabs/projectiles/BanditClusterBombSeed").GetComponent<ProjectileController>().ghostPrefab;
                }
            }
            if (ClusterBombletObject == null)
            {
                if (useBodyClone)
                {
                    ClusterBombletObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/BanditClusterGrenadeProjectile"), "BanditReloadedClusterBomblet", true);
                    ClusterBombletGhostObject = PrefabAPI.InstantiateClone(ClusterBombletObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedClusterBombletGhost", false);
                    ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
                    {
                        list.Add(ClusterBombletObject);
                    };
                    ClusterBombletObject.GetComponent<ProjectileController>().ghostPrefab = ClusterBombletGhostObject;
                }
                else
                {
                    ClusterBombletObject = Resources.Load<GameObject>("prefabs/projectiles/BanditClusterGrenadeProjectile");
                    ClusterBombletGhostObject = Resources.Load<GameObject>("prefabs/projectiles/BanditClusterGrenadeProjectile").GetComponent<ProjectileController>().ghostPrefab;
                }
            }
        }

        private void CreateLightsOutBuff()
        {
            BuffDef LightsOutBuffDef = new BuffDef
            {
                buffColor = BanditColor,
                buffIndex = BuffIndex.Count,
                canStack = true,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texBuffFullCritIcon",
                isDebuff = true,
                name = "BanditReloadedMarkedForDeath"
            };
            BanditReloaded.lightsOutBuff = BuffAPI.Add(new CustomBuff(LightsOutBuffDef));

            BuffDef SlowBuffDef = new BuffDef
            {
                buffColor = BanditColor,
                buffIndex = BuffIndex.Count,
                canStack = true,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texBuffOnFireIcon",
                isDebuff = false,
                name = "BanditReloadedThermite"
            };
            BanditReloaded.thermiteBuff = BuffAPI.Add(new CustomBuff(SlowBuffDef));
            thermiteDoT = DotAPI.RegisterDotDef(0.5f, 0.25f, DamageColorIndex.Item, thermiteBuff);

            BuffDef FakeStunDef = new BuffDef
            {
                buffColor = Color.white,
                buffIndex = BuffIndex.Count,
                canStack = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texBuffSlow50Icon",
                isDebuff = false,
                name = "BanditReloadedFakeStun"
            };
            BanditReloaded.fakeStun = BuffAPI.Add(new CustomBuff(FakeStunDef));
        }

        public static void AddSkin()    //credits to rob
        {
            GameObject bodyPrefab = BanditBody;
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = null;
            if (model.GetComponent<ModelSkinController>())
                skinController = model.GetComponent<ModelSkinController>();
            else
                skinController = model.AddComponent<ModelSkinController>();

            SkinnedMeshRenderer mainRenderer = Reflection.GetFieldValue<SkinnedMeshRenderer>(characterModel, "mainSkinnedMeshRenderer");
            if (mainRenderer == null)
            {
                CharacterModel.RendererInfo[] bRI = Reflection.GetFieldValue<CharacterModel.RendererInfo[]>(characterModel, "baseRendererInfos");
                if (bRI != null)
                {
                    foreach (CharacterModel.RendererInfo rendererInfo in bRI)
                    {
                        if (rendererInfo.renderer is SkinnedMeshRenderer)
                        {
                            mainRenderer = (SkinnedMeshRenderer)rendererInfo.renderer;
                            break;
                        }
                    }
                    if (mainRenderer != null)
                    {
                        characterModel.SetFieldValue<SkinnedMeshRenderer>("mainSkinnedMeshRenderer", mainRenderer);
                    }
                }
            }

            LanguageAPI.Add("BANDITRELOADEDBODY_DEFAULT_SKIN_NAME", "Default");

            LoadoutAPI.SkinDefInfo skinDefInfo = default(LoadoutAPI.SkinDefInfo);
            skinDefInfo.BaseSkins = Array.Empty<SkinDef>();
            skinDefInfo.GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>();
            skinDefInfo.Icon = LoadoutAPI.CreateSkinIcon(new Color(143f / 255f, 132f / 255f, 106f / 255f), Color.cyan, new Color(92f / 255f, 136f / 255f, 167f / 255f), new Color(25f / 255f, 50f / 255f, 57f / 255f));
            skinDefInfo.MeshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    renderer = mainRenderer,
                    mesh = mainRenderer.sharedMesh
                }
            };
            skinDefInfo.Name = "BANDITRELOADEDBODY_DEFAULT_SKIN_NAME";
            skinDefInfo.NameToken = "BANDITRELOADEDBODY_DEFAULT_SKIN_NAME";
            skinDefInfo.RendererInfos = characterModel.baseRendererInfos;
            skinDefInfo.RootObject = model;
            skinDefInfo.UnlockableName = "";
            skinDefInfo.MinionSkinReplacements = new SkinDef.MinionSkinReplacement[0];
            skinDefInfo.ProjectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[0];

            SkinDef defaultSkin = LoadoutAPI.CreateNewSkinDef(skinDefInfo);

            skinController.skins = new SkinDef[1]
            {
                defaultSkin,
            };
        }

        private class MenuAnimComponent : MonoBehaviour
        {
            internal void OnEnable()
            {
                if (base.gameObject && base.transform.parent && base.gameObject.transform.parent.gameObject && base.gameObject.transform.parent.gameObject.name == "CharacterPad")
                {
                    base.StartCoroutine(this.RevolverAnim());
                }
            }

            private IEnumerator RevolverAnim()
            {
                Animator animator = base.gameObject.GetComponent<Animator>();
                EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/smokescreeneffect"), new EffectData
                {
                    origin = base.gameObject.transform.position
                }, false);
                Util.PlaySound("play_bandit_shift_end", base.gameObject);
                this.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", 1f, animator);
                this.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", 1f, animator);
                yield return new WaitForSeconds(0.48f);
                Util.PlaySound("play_bandit_m1_pump", base.gameObject);
                yield return new WaitForSeconds(0.4f);
                this.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", 0.62f, animator);
                this.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", 0.62f, animator);
                Util.PlaySound("play_bandit_m2_load", base.gameObject);
                yield break;
            }

            private void PlayAnimation(string layerName, string animationStateName, string playbackRateParam, float duration, Animator animator)
            {
                int layerIndex = animator.GetLayerIndex(layerName);
                animator.SetFloat(playbackRateParam, 1f);
                animator.PlayInFixedTime(animationStateName, layerIndex, 0f);
                animator.Update(0f);
                float length = animator.GetCurrentAnimatorStateInfo(layerIndex).length;
                animator.SetFloat(playbackRateParam, length / duration);
            }
        }
    }

    public class BanditCrosshairComponent : MonoBehaviour
    {
        private void Start()
        {
            cb = base.GetComponent<CharacterBody>();
            skills = cb.skillLocator;
            defaultCrosshairPrefab = cb.crosshairPrefab;
        }
        private void Update()
        {
            if (skills.primary.skillDef.skillName == "FireSlug" || skills.primary.skillDef.skillName == "FireScatter")
            {
                if ((!Blast.noReload && skills.primary.skillDef.skillName == "FireSlug") || (!Scatter.noReload && skills.primary.skillDef.skillName == "FireScatter"))
                {
                    if (skills.primary.maxStock > 1 && skills.primary.stock > 0)
                    {
                        if (cb.crosshairPrefab == emptyCrosshairPrefab)
                        {
                            cb.crosshairPrefab = defaultCrosshairPrefab;
                        }
                    }
                    else
                    {
                        if (cb.crosshairPrefab == defaultCrosshairPrefab)
                        {
                            cb.crosshairPrefab = emptyCrosshairPrefab;
                        }
                    }
                }
            }
        }
        private CharacterBody cb;
        private SkillLocator skills;
        private static GameObject emptyCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/badcrosshair");
        private GameObject defaultCrosshairPrefab;
    }

    public class BanditTimerComponent : MonoBehaviour
    {
        private void Update()
        {
            del.Clear();
            foreach (BanditTimer b in hitList)
            {
                if (b.owner)
                {
                    b.length -= Time.fixedDeltaTime;
                    if (b.length <= 0f)
                    {
                        del.Add(b);
                    }
                }
            }
            foreach (BanditTimer b in del)
            {
                hitList.Remove(b);
            }
            del.Clear();
        }
        public void ResetCooldowns()
        {
            List<SkillLocator> resetList = new List<SkillLocator>();
            foreach (BanditTimer b in hitList)
            {
                bool repeat = false;
                foreach (SkillLocator s in resetList)
                {
                    if (s == b.owner)
                    {
                        repeat = true;
                        break;
                    }
                }
                if (!repeat)
                {
                    if (b.owner && b.owner.isActiveAndEnabled)
                    {
                        resetList.Add(b.owner);
                        b.owner.ResetSkills();
                    }
                }
            }
            hitList.Clear();
        }
        public void AddTimer(SkillLocator owner, float length)
        {
            BanditTimer b = new BanditTimer(owner, length);
            hitList.Add(b);
        }
        private class BanditTimer
        {
            public SkillLocator owner;
            public float length;
            public BanditTimer(SkillLocator o, float l)
            {
                owner = o;
                length = l;
            }
        }
        private List<BanditTimer> hitList = new List<BanditTimer>();
        List<BanditTimer> del = new List<BanditTimer>();
    }

    public class BanditThermiteComponent : NetworkBehaviour
    {
        private void Awake()
        {
            ModelLocator ml = base.GetComponent<ModelLocator>();
            cb = base.GetComponent<CharacterBody>();
            if (cb && ml && ml.modelTransform && ml.modelTransform.gameObject)
            {
                burnEffectController = base.gameObject.AddComponent<BurnEffectController>();
                burnEffectController.effectType = BurnEffectController.normalEffect;
                burnEffectController.target = ml.modelTransform.gameObject;
            }
            else
            {
                DestroySelf();
            }
        }
        private void Update()
        {
            if(!cb || cb.GetBuffCount(BanditReloaded.thermiteBuff) <= 0)
            {
                DestroySelf();
            }
        }

        [Server]
        private void DestroySelf()
        {
            if (burnEffectController)
            {
                Destroy(burnEffectController);
            }
            Destroy(this);
        }

        private BurnEffectController burnEffectController;
        private CharacterBody cb;
    }
}

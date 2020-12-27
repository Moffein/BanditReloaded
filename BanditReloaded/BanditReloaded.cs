using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.BanditReloadedSkills;
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
    [R2API.Utils.R2APISubmoduleDependency(nameof(SurvivorAPI), nameof(LoadoutAPI), nameof(PrefabAPI), nameof(BuffAPI), nameof(ResourcesAPI), nameof(LanguageAPI), nameof(DotAPI), nameof(SoundAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    //[NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    class BanditReloaded : BaseUnityPlugin
    {
        #region cfg
        ConfigEntry<bool> usePassive, cloakAnim, useAltCrosshair, classicOutro;

        ConfigEntry<float> blastDamage, blastRange, blastDryDuration, blastMaxDuration, blastMinDuration, blastForce, blastSpread, blastRadius, blastMashSpread, blastRecoil, blastRechargeInterval;
        ConfigEntry<int> blastStock;
        ConfigEntry<bool> blastDryEnabled, blastVanillaBrainstalks, blastPenetrate, blastFalloff;

        ConfigEntry<float> thermiteDamage, thermiteBurnDuration, thermiteRadius, thermiteFireRate, thermiteProcCoefficient, thermiteVelocity, thermiteLifetime, thermiteCooldown, thermiteBurnDamageMult;
        ConfigEntry<int> thermiteStock;
        ConfigEntry<bool> thermiteGravity;

        ConfigEntry<float> cloakDamage, cloakDuration, cloakMinDuration, cloakRadius, cloakCooldown;
        ConfigEntry<int> cloakStock;

        ConfigEntry<float> loDamage, loDebuffDamage, loGracePeriodMin, loGracePeriodMax, loForce, loFireRate, loEndLag, loCooldown, loExecuteThreshold;
        ConfigEntry<bool> loExecuteBosses;
        ConfigEntry<int> loStock;

        ConfigEntry<float> scatterDamage, scatterForce, scatterSpread, scatterProcCoefficient, scatterMaxDuration, scatterMinDuration, scatterDryDuration, scatterRechargeInterval, scatterRange, scatterRadius, scatterRecoil;
        ConfigEntry<uint> scatterPellets;
        ConfigEntry<bool> scatterDryEnabled, scatterVanillaBrainstalks, scatterPenetrate;
        ConfigEntry<int> scatterStock;

        ConfigEntry<float> acidDamage, acidWeakDuration, acidRadius, acidFireRate, acidProcCoefficient, acidVelocity, acidLifetime, acidCooldown;
        ConfigEntry<int> acidStock;
        ConfigEntry<bool> acidGravity;

        ConfigEntry<float> asMinDamage, asMaxDamage, asMinRadius, asMaxRadius, asChargeDuration, asMinForce, asMaxForce, asSelfForceMin, asSelfForceMax, asEndLag, asCooldown, asZoom, asBarrierPercent;
        ConfigEntry<int> asStock;
        ConfigEntry<bool> asEnabled;

        ConfigEntry<float> cbDamage, cbRadius, cbProcCoefficient, cbBombletDamage, cbBombletRadius, cbBombletProcCoefficient, cbFireRate, cbVelocity, cbCooldown;
        ConfigEntry<int> cbBombletCount;
        ConfigEntry<int> cbStock;

        ConfigEntry<float> reuDamage, reuDebuffDamage, reuGracePeriodMin, reuGracePeriodMax, reuForce, reuDraw, reuFireRate, reuEndLag, reuCooldown, reuExecuteThreshold, reuSpread, reuRange;
        ConfigEntry<int> reuBullets, reuStock;
        ConfigEntry<bool> reuExecuteBosses;
        #endregion
        public static SurvivorDef item;
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
        public static BuffIndex cloakDamageBuff;
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

        public void Awake()
        {
            LoadSounds();
            ReadConfig();
            SetupBanditBody();
            SetupProjectiles();
            SetIcons();
            SetAttributes();
            InitSkills();
            AssignSkills();
            CreateMaster();
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

            LanguageAPI.Add("BANDITRELOADED_BODY_NAME", "Bandit");
            LanguageAPI.Add("BANDITRELOADED_BODY_DESC", BanditDesc);

            LanguageAPI.Add("KEYWORD_BANDITRELOADED_STACKABLE", "<style=cKeywordName>Stackable</style><style=cSub>The <style=cIsDamage>burn</style> from this ability can be stacked to boost the damage of your <style=cIsUtility>Special</style> multiple times.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_RESET", "<style=cKeywordName>Marked</style><style=cSub>Kill Marked enemies to <style=cIsUtility>reset all skill cooldowns to 0</style>. Enemies with <style=cIsHealth>low HP</style> are Marked longer.</style>");

            LanguageAPI.Add("KEYWORD_BANDITRELOADED_INVIS", "<style=cKeywordName>Invisible</style><style=cSub>Enemies are unable to target you.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_EXECUTE", "<style=cKeywordName>Executing</style><style=cSub>The ability <style=cIsHealth>instantly kills</style> enemies with <style=cIsHealth>low HP</style>.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_RAPIDFIRE", "<style=cKeywordName>Rapid-Fire</style><style=cSub>The skill fires faster if you click faster.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_FASTLOAD", "<style=cKeywordName>Fast-Load</style><style=cSub>The skill can be fired slowly while reloading.</style>");

            item = new SurvivorDef
            {
                name = "BanditReloaded",
                bodyPrefab = BanditBody,
                descriptionToken = "BANDITRELOADED_BODY_dESC",
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

            On.RoR2.SkillLocator.ResetSkills += (orig, self) =>
            {
                orig(self);
                if (self.gameObject.GetComponentInParent<CharacterBody>().baseNameToken == "BANDITRELOADED_BODY_NAME")
                {
                    Util.PlaySound("Play_BanditReloaded_reset", self.gameObject);
                }
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
                if (self.HasBuff(thermiteBuff))
                {
                    int tCount = self.GetBuffCount(thermiteBuff);
                    self.SetPropertyValue<float>("moveSpeed", self.moveSpeed * Mathf.Pow(0.9f,tCount));
                }
                int loCount = self.GetBuffCount(lightsOutBuff);
                self.SetPropertyValue<float>("moveSpeed", self.moveSpeed * Mathf.Pow(0.9f, loCount));
            };

            On.EntityStates.BaseState.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.HasBuff(cloakDamageBuff))
                {
                    self.damageStat *= 1.5f;
                }
            };


            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                bool aliveBeforeHit = self.alive;
                bool applyWeaken = false;
                bool resetCooldownsOnKill = false;
                bool isBarrage = false;
                bool isScepterLO = false;

                bool applyThermite = false;

                bool banditAttacker = damageInfo.attacker != null;
                CharacterBody attackerCB = null;
                if (banditAttacker)
                {
                    attackerCB = damageInfo.attacker.GetComponent<CharacterBody>();
                    banditAttacker = attackerCB.baseNameToken == "BANDITRELOADED_BODY_NAME";
                }

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
                        }

                        float buffDamage = 0f;
                        float buffBaseDamage;
                        if (isBarrage)
                        {
                            buffBaseDamage = damageInfo.damage / FireBarrage.damageCoefficient * FireBarrage.buffDamageCoefficient;
                            
                        }
                        else if (isScepterLO)
                        {
                            buffBaseDamage = damageInfo.damage / FireLightsOutScepter.damageCoefficient * FireLightsOutScepter.buffDamageCoefficient;
                        }
                        else
                        {
                            buffBaseDamage = damageInfo.damage / FireLightsOut.damageCoefficient * FireLightsOut.buffDamageCoefficient;
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
                        applyThermite = true;
                    }
                    else if (damageInfo.damageType == (DamageType.WeakOnHit | DamageType.AOE))
                    {
                        damageInfo.damageType = DamageType.AOE;
                        applyWeaken = true;
                    }
                    else if (asEnabled.Value && damageInfo.damageType == (DamageType.Stun1s))
                    {
                        HealthComponent hc = damageInfo.attacker.GetComponent<HealthComponent>();
                        hc.AddBarrier(damageInfo.procCoefficient * FireChargeShot.barrierPercent * hc.combinedHealth);
                        damageInfo.procCoefficient = 1f;
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
                    if (self.alive && !damageInfo.rejected)
                    {
                        if (applyThermite)
                        {
                            DotController.InflictDot(self.gameObject, damageInfo.attacker, thermiteDoT, ThermiteBomb.debuffDuration, ThermiteBomb.burnDamageMult);
                            self.gameObject.AddComponent<BanditThermiteComponent>();
                        }
                        if (applyWeaken)
                        {
                            self.body.AddTimedBuff(BuffIndex.Weak, AcidBomb.debuffDuration);
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

        private void ReadConfig()
        {
            usePassive = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Enable Passive"), true, new ConfigDescription("Makes Bandit auto-reload his primary when using other skills."));
            cloakAnim = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use Cloak Anim"), false, new ConfigDescription("Enables the unused Smokebomb entry animation."));
            useAltCrosshair = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use Alt Crosshair"), true, new ConfigDescription("Uses the unused Bandit-specific crosshair."));
            classicOutro = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use RoR1 Outro"), false, new ConfigDescription("Uses Bandit's RoR1 ending."));
            asEnabled = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Enable unused Assassinate utility*"), false, new ConfigDescription("Enables the Assassinate Utility skill. This skill was disabled due to being poorly coded and not fitting Bandit's kit, but it's left in in case you want to use it. This skill can only be used if Assassinate is enabled on the host."));

            blastDamage = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Damage"), 2.5f, new ConfigDescription("How much damage Blast deals."));
            blastMaxDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Fire Rate"), 0.3f, new ConfigDescription("Time between shots."));
            blastMinDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Min Duration"), 0.2f, new ConfigDescription("How soon you can fire another shot if you mash."));
            blastDryEnabled = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Allow Fire While Empty"), false, new ConfigDescription("Allow Blast to be fired on an empty mag."));
            blastDryDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Fire While Empty Duration"), 0.5f, new ConfigDescription("Time between shots on an empty mag."));
            blastPenetrate = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Penetrate Enemies"), true, new ConfigDescription("Shots pierce enemies."));
            blastRadius = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Shot Radius"), 0.4f, new ConfigDescription("How wide Blast's shots are."));
            blastForce = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Force"), 600f, new ConfigDescription("Push force per shot."));
            blastSpread = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Spread"), 0f, new ConfigDescription("Amount of spread with added each shot."));
            blastMashSpread = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Mash Spread"), 0.4f, new ConfigDescription("Amount of spread with added each shot when mashing."));
            blastRecoil = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Recoil"), 1.4f, new ConfigDescription("How hard the gun kicks when shooting."));
            blastRange = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Range"), 250f, new ConfigDescription("How far Blast can reach."));
            blastFalloff = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Use Falloff"), true, new ConfigDescription("Shots deal less damage over range."));
            blastStock = base.Config.Bind<int>(new ConfigDefinition("10 - Primary - Blast", "Stock"), 8, new ConfigDescription("How many shots can be fired before reloading."));
            blastRechargeInterval = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Reload Time"), 2f, new ConfigDescription("How long it takes to reload. Set to 0 to disable reloading."));
             blastVanillaBrainstalks = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Use Vanilla Brainstalks Behavior"), true, new ConfigDescription("Disables infinite ammo when Brainstalks is active."));

            scatterDamage = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Damage"), 0.8f, new ConfigDescription("How much damage each pellet of Scatter deals."));
            scatterPellets = base.Config.Bind<uint>(new ConfigDefinition("11 - Primary - Scatter", "Pellets"), 6, new ConfigDescription("How many pellets Scatter shoots."));
            scatterProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Proc Coefficient"), 0.7f, new ConfigDescription("Affects the chance and power of each pellet's procs."));
            scatterMaxDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Fire Rate"), 0.625f, new ConfigDescription("Time between shots."));
            scatterMinDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Min Duration"), 0.625f, new ConfigDescription("How soon you can fire another shot if you mash."));
            scatterDryEnabled = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Allow Fire While Empty"), true, new ConfigDescription("Allow Scatter to be fired on an empty mag."));
            scatterDryDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Fire While Empty Duration"), 1f, new ConfigDescription("Time between shots on an empty mag."));
            scatterPenetrate = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Penetrate Enemies"), true, new ConfigDescription("Shots pierce enemies."));
            scatterRadius = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Shot Radius"), 0.4f, new ConfigDescription("How wide Scatter's pellets are."));
            scatterForce = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Force"), 200f, new ConfigDescription("Push force per pellet."));
            scatterSpread = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Spread"), 2.5f, new ConfigDescription("Size of the pellet spread."));
            scatterRecoil = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Recoil"), 2.6f, new ConfigDescription("How hard the gun kicks when shooting."));
            scatterRange = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Range"), 120f, new ConfigDescription("How far Scatter can reach."));
            scatterStock = base.Config.Bind<int>(new ConfigDefinition("11 - Primary - Scatter", "Stock"), 5, new ConfigDescription("How many shots Scatter can hold."));
            scatterRechargeInterval = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Reload Time"), 2.5f, new ConfigDescription("How much time it takes to reload. Set to 0 to disable reloading."));
            scatterVanillaBrainstalks = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Use Vanilla Brainstalks Behavior"), true, new ConfigDescription("Disables infinite ammo when Brainstalks is active."));

            thermiteDamage = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Thermite Bomb", "Damage"), 2.7f, new ConfigDescription("How much damage Thermite Bomb deals."));
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

            acidDamage = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Damage"), 2.7f, new ConfigDescription("How much damage Acid Bomb deals."));
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

            loDamage = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Damage"), 6f, new ConfigDescription("How much damage Lights Out deals."));
            loDebuffDamage = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Debuff Bonus Damage*"), 3f, new ConfigDescription("How much extra damage Lights Out deals for each debuff the target has."));
            loGracePeriodMin = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Grace Period Minimum Duration*"), 1f, new ConfigDescription("Lower bound of Lights Out's Grace Period duration."));
            loGracePeriodMax = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Grace Period Maximum Duration*"), 3.6f, new ConfigDescription("Upper bound of Lights Out's Grace Period duration."));
            loForce = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Force"), 2400f, new ConfigDescription("Push force per shot."));
            loFireRate = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Draw Time"), 0.4f, new ConfigDescription("How long it takes to prepare Lights Out."));
            loEndLag = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "End Lag"), 0.2f, new ConfigDescription("Delay after firing."));
            loCooldown = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Cooldown"), 7f, new ConfigDescription("How long Lights Out takes to recharge."));
            loStock = base.Config.Bind<int>(new ConfigDefinition("40 - Special - Lights Out", "Stock"), 1, new ConfigDescription("How many charges Lights Out has."));
            loExecuteThreshold = base.Config.Bind<float>(new ConfigDefinition("40 - Special - Lights Out", "Execute Threshold*"), 0f, new ConfigDescription("Instakill enemies that fall below this HP percentage. 0 = 0%, 1 = 100%"));
            loExecuteBosses = base.Config.Bind<bool>(new ConfigDefinition("40 - Special - Lights Out", "Execute Bosses*"), false, new ConfigDescription("Allow bosses to be executed by Lights Out."));

            reuDamage = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Damage"), 1f, new ConfigDescription("How much damage Rack em Up deals."));
            reuDebuffDamage = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Debuff Bonus Damage*"), 0.5f, new ConfigDescription("How much extra damage Rack em Up deals for each debuff the target has."));
            reuGracePeriodMin = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Grace Period Minimum Duration*"), 1f, new ConfigDescription("Lower bound of Rack em Up's Grace Period duration."));
            reuGracePeriodMax = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Grace Period Maximum Duration*"), 3.6f, new ConfigDescription("Upper bound of Rack em Up's Grace Period duration."));
            reuBullets = base.Config.Bind<int>(new ConfigDefinition("41 - Special - Rack em Up", "Total Shots"), 6, new ConfigDescription("How many shots are fired."));
            reuForce = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Force"), 100f, new ConfigDescription("Push force per shot."));
            reuDraw = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Draw Time"), 0.32f, new ConfigDescription("How long it takes to prepare Rack em Up."));
            reuFireRate = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Fire Rate"), 0.13f, new ConfigDescription("Time it takes for Rack em Up to fire a single shot."));
            reuEndLag = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "End Lag"), 0.4f, new ConfigDescription("Delay after firing all shots."));
            reuSpread = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Spread"), 2.5f, new ConfigDescription("Size of the cone of fire."));
            reuRange = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Range"), 120f, new ConfigDescription("How far shots reach."));
            reuCooldown = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Cooldown"), 7f, new ConfigDescription("How long Rack em Up takes to recharge."));
            reuStock = base.Config.Bind<int>(new ConfigDefinition("41 - Special - Rack em Up", "Stock"), 1, new ConfigDescription("How many charges Rack em Up has."));
            reuExecuteThreshold = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Rack em Up", "Execute Threshold*"), 0f, new ConfigDescription("Instakill enemies that fall below this HP percentage. 0 = 0%, 1 = 100%"));
            reuExecuteBosses = base.Config.Bind<bool>(new ConfigDefinition("40 - Special - Lights Out", "Execute Bosses*"), false, new ConfigDescription("Allow bosses to be executed by Rack em Up."));
            
            asMinDamage = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Minimum Damage"), 2.5f, new ConfigDescription("How much damage Assassinate deals at no charge."));
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
            primaryBlastDef.isBullets = false;
            primaryBlastDef.baseRechargeInterval = blastRechargeInterval.Value;
            if (primaryBlastDef.baseRechargeInterval > 0f)
            {
                primaryBlastDef.baseMaxStock = blastStock.Value;
                primaryBlastDef.rechargeStock = primaryBlastDef.baseMaxStock;
                Blast.noReload = false;
            }
            else
            {
                primaryBlastDef.baseRechargeInterval = 0f;
                primaryBlastDef.baseMaxStock = 1;
                primaryBlastDef.rechargeStock = 1;
                Blast.noReload = true;
            }
            primaryBlastDef.skillDescriptionToken = "";
            primaryBlastDef.skillDescriptionToken += "Fire a powerful slug " + (Blast.penetrateEnemies ? "that pierces enemies " : "") + "for <style=cIsDamage>" + Blast.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            if (primaryBlastDef.baseRechargeInterval > 0f)
            {
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
            #endregion

            #region scatter
            primaryScatterDef = SkillDef.CreateInstance<SkillDef>();
            primaryScatterDef.activationState = new SerializableEntityStateType(typeof(Scatter));
            primaryScatterDef.isBullets = false;
            primaryScatterDef.baseRechargeInterval = scatterRechargeInterval.Value;
            if (primaryScatterDef.baseRechargeInterval > 0f)
            {
                primaryScatterDef.baseMaxStock = scatterStock.Value;
                primaryScatterDef.rechargeStock = primaryScatterDef.baseMaxStock;
                Scatter.noReload = false;
            }
            else
            {
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
            utilityDefA = SkillDef.CreateInstance<SkillDef>();
            utilityDefA.activationState = new SerializableEntityStateType(typeof(CastSmokescreenNoDelay));
            utilityDefA.baseRechargeInterval = cloakCooldown.Value;
            utilityDefA.skillName = "CloakBanditReloaded";
            utilityDefA.skillNameToken = "Smokebomb";
            utilityDefA.skillDescriptionToken = "<color=#95CDE5>Turn invisible</color>. After " + CastSmokescreenNoDelay.duration.ToString("N1")
                + " seconds or after using another ability, surprise and <style=cIsDamage>stun enemies for " + CastSmokescreenNoDelay.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>."
                + " Your first attack after using this skill will deal <style=cIsDamage>1.5x bonus damage</style>.";
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
            utilityDefB.skillDescriptionToken = "<color=#95CDE5>Turn invisible</color>. After " + (CastSmokescreen.stealthDuration - 1f).ToString("N1")
                + " seconds or after using another ability, surprise and <style=cIsDamage>stun enemies for " + CastSmokescreen.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>."
                + " Your first attack after using this skill will deal <style=cIsDamage>1.5x bonus damage</style>."; ;
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

            specialLightsOutDef.skillDescriptionToken = "";
            List<String> kwlLO = new List<String>();
            if (loExecuteThreshold.Value > 0f)
            {
                kwlLO.Add("KEYWORD_BANDITRELOADED_EXECUTE");
                specialLightsOutDef.skillDescriptionToken += "<style=cIsDamage>Executing</style>. ";
            }
            if (loDebuffDamage.Value > 0f)
            {
                kwlLO.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST1");
                specialLightsOutDef.skillDescriptionToken += "<style=cIsDamage>Compromising</style>. ";
            }
            kwlLO.Add("KEYWORD_BANDITRELOADED_RESET");
            specialLightsOutDef.keywordTokens = kwlLO.ToArray();

            specialLightsOutDef.skillDescriptionToken += "Take aim with your Persuader, <style=cIsDamage>dealing " + FireLightsOut.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialLightsOutDef.skillDescriptionToken += " Enemies hit by this ability are <style=cIsUtility>Marked</style>.";
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST1", "<style=cKeywordName>Compromising</style><style=cSub>The skill deals <style=cIsDamage>+" + FireLightsOut.buffDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> for each debuff on the enemy. The damage bonus is only applied <style=cIsHealth>once</style> per debuff stack.</style>");
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
            acidBombDef.activationState = new SerializableEntityStateType(typeof(AcidBomb));
            acidBombDef.baseRechargeInterval = acidCooldown.Value;
            acidBombDef.skillNameToken = "Acid Bomb";
            acidBombDef.skillDescriptionToken = "Toss a caustic grenade that <style=cIsHealing>Weakens</style> enemies for <style=cIsDamage>" + (AcidBomb.damageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
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
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST2", "<style=cKeywordName>Compromising</style><style=cSub>Each shot deals <style=cIsDamage>+" + FireBarrage.buffDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> for each debuff on the enemy. The damage bonus is only applied <style=cIsHealth>once</style> per debuff stack.</style>");
            List<string> kwlBarrage = new List<string>();
            kwlBarrage.Add("KEYWORD_BANDITRELOADED_RESET");
            if (reuExecuteThreshold.Value > 0f)
            {
                kwlBarrage.Add("KEYWORD_BANDITRELOADED_EXECUTE");
            }
            if (reuDebuffDamage.Value > 0f)
            {
                kwlBarrage.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST2");
            }
            specialBarrageDef.keywordTokens = kwlBarrage.ToArray();

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


            primarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = primaryBlastDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(primaryBlastDef.skillNameToken, false)
            };
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
            if (cloakAnim.Value)
            {
                toInsert = utilityDefB;
            }
            else
            {
                toInsert = utilityDefA;
            }

            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = toInsert,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(toInsert.skillNameToken, false)
            };
            Debug.Log("added smokebomb to family");
            if (asEnabled.Value)
            {
                Array.Resize(ref utilitySkillFamily.variants, utilitySkillFamily.variants.Length + 1);
                utilitySkillFamily.variants[utilitySkillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = utilityAltDef,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node(utilityAltDef.skillNameToken, false)
                };
            }
            Debug.Log("added assassinate to family");

            specialSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = specialLightsOutDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(specialLightsOutDef.skillNameToken, false)
            };

            Debug.Log("added lo to family");
            Array.Resize(ref specialSkillFamily.variants, specialSkillFamily.variants.Length + 1);
            specialSkillFamily.variants[specialSkillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = specialBarrageDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(specialBarrageDef.skillNameToken, false)
            };
            Debug.Log("added reu to family");

            LoadoutAPI.AddSkillFamily(primarySkillFamily);
            LoadoutAPI.AddSkillFamily(secondarySkillFamily);
            LoadoutAPI.AddSkillFamily(utilitySkillFamily);
            LoadoutAPI.AddSkillFamily(specialSkillFamily);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems"))
            {
               // SetupScepter();
            }
        }

        private void ModifyAcidBomb(GameObject ab)
        {
            ab.GetComponent<ProjectileSimple>().velocity = acidVelocity.Value;
            ProjectileImpactExplosion abPIE = ab.GetComponent<ProjectileImpactExplosion>();
            abPIE.blastRadius = acidRadius.Value;
            ab.GetComponent<ProjectileDamage>().damageType = DamageType.WeakOnHit;
            abPIE.blastProcCoefficient = acidProcCoefficient.Value;
            abPIE.falloffModel = BlastAttack.FalloffModel.None;
            abPIE.lifetime = acidLifetime.Value;
            abPIE.impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion");
            abPIE.explosionSoundString = "Play_commando_M2_grenade_explo";
            abPIE.timerAfterImpact = true;
            abPIE.lifetimeAfterImpact = 0.25f;
            abPIE.destroyOnWorld = false;
            abPIE.destroyOnEnemy = false;
            ab.GetComponent<ProjectileStickOnImpact>().ignoreWorld = false;
            if (acidGravity.Value)
            {
                ab.GetComponent<Rigidbody>().useGravity = acidGravity.Value;
            }
        }

        private void ModifyThermite(GameObject tm)
        {
            ProjectileImpactExplosion tPIE = tm.GetComponent<ProjectileImpactExplosion>();
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
            tm.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            tm.GetComponent<ProjectileSimple>().velocity = thermiteVelocity.Value;
            tm.GetComponent<ProjectileDamage>().damage = 1f;
            SphereCollider thermiteSphere = tm.GetComponent<SphereCollider>();
            if (thermiteSphere == null)
            {
                thermiteSphere = tm.AddComponent<SphereCollider>();
            }
            thermiteSphere.contactOffset = AcidBombObject.GetComponent<SphereCollider>().contactOffset;
            thermiteSphere.radius = AcidBombObject.GetComponent<SphereCollider>().radius;
            thermiteSphere.contactOffset = AcidBombObject.GetComponent<SphereCollider>().contactOffset;
            ProjectileController tPC = tm.GetComponent<ProjectileController>();
            if (tPC == null)
            {
                tPC = tm.AddComponent<ProjectileController>();
            }
            tPC.procCoefficient = 1f;
            tm.GetComponent<ProjectileIntervalOverlapAttack>().damageCoefficient = 0f;
            ThermiteBomb.projectilePrefab = tm;
            tPC.ghostPrefab = Resources.Load<GameObject>("prefabs/projectileghosts/thermiteghost");
            if (thermiteGravity.Value)
            {
                tm.GetComponent<Rigidbody>().useGravity = thermiteGravity.Value;
            }
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
            Blast.recoilAmplitude = blastRecoil.Value;
            Blast.vanillaBrainstalks = blastVanillaBrainstalks.Value;
            Blast.penetrateEnemies = blastPenetrate.Value;
            Blast.useFalloff = blastFalloff.Value;
            Blast.mashSpread = blastMashSpread.Value;
            Blast.dryFireDuration = blastDryDuration.Value;

            CastSmokescreen.stealthDuration = cloakDuration.Value + 1f;
            CastSmokescreen.damageCoefficient = cloakDamage.Value;
            CastSmokescreen.radius = cloakRadius.Value;

            CastSmokescreenNoDelay.duration = cloakDuration.Value;
            CastSmokescreenNoDelay.damageCoefficient = cloakDamage.Value;
            CastSmokescreenNoDelay.radius = cloakRadius.Value;
            CastSmokescreenNoDelay.minimumStateDuration = cloakMinDuration.Value;

            Assassinate.baseChargeDuration = asChargeDuration.Value;
            Assassinate.zoomFOV = asZoom.Value;

            FireChargeShot.minForce = asMinForce.Value;
            FireChargeShot.maxForce = asMaxForce.Value;
            FireChargeShot.selfForceMax = asSelfForceMax.Value;
            FireChargeShot.selfForceMin = asSelfForceMin.Value;
            FireChargeShot.maxDamageCoefficient = asMaxDamage.Value;
            FireChargeShot.minDamageCoefficient = asMinDamage.Value;
            FireChargeShot.baseDuration = asEndLag.Value;
            FireChargeShot.minRadius = asMinRadius.Value;
            FireChargeShot.maxRadius = asMaxRadius.Value;
            FireChargeShot.barrierPercent = asBarrierPercent.Value;

            PrepLightsOut.baseDuration = loFireRate.Value;

            FireLightsOut.damageCoefficient = loDamage.Value;
            FireLightsOut.force = loForce.Value;
            FireLightsOut.baseDuration = loEndLag.Value;
            FireLightsOut.gracePeriodMin = loGracePeriodMin.Value;
            FireLightsOut.gracePeriodMax = loGracePeriodMax.Value;
            FireLightsOut.executeThreshold = loExecuteThreshold.Value;
            FireLightsOut.executeBosses = loExecuteBosses.Value;
            FireLightsOut.buffDamageCoefficient = loDebuffDamage.Value;

            AcidBomb.projectilePrefab = AcidBombObject;
            AcidBomb.damageCoefficient = acidDamage.Value;
            AcidBomb.debuffDuration = acidWeakDuration.Value;
            AcidBomb.baseDuration = acidFireRate.Value;

            ThermiteBomb.projectilePrefab = ThermiteObject;
            ThermiteBomb.damageCoefficient = thermiteDamage.Value;
            ThermiteBomb.baseDuration = thermiteFireRate.Value;
            ThermiteBomb.debuffDuration = thermiteBurnDuration.Value;
            ThermiteBomb.burnDamageMult = thermiteBurnDamageMult.Value;

            Scatter.procCoefficient = scatterProcCoefficient.Value;
            Scatter.pelletCount = scatterPellets.Value;
            Scatter.damageCoefficient = scatterDamage.Value;
            Scatter.force = scatterForce.Value;
            Scatter.baseMaxDuration = scatterMaxDuration.Value;
            Scatter.baseMinDuration = scatterMinDuration.Value;
            Scatter.spreadBloomValue = scatterSpread.Value;
            Scatter.recoilAmplitude = scatterRecoil.Value;
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
            ClusterBomb.projectilePrefab = ClusterBombObject;
            ClusterBomb.bombletDamageCoefficient = cbBombletDamage.Value;

            PrepBarrage.baseDuration = reuDraw.Value;

            FireBarrage.maxBullets = reuBullets.Value;
            FireBarrage.damageCoefficient = reuDamage.Value;
            FireBarrage.force = reuForce.Value;
            FireBarrage.baseDuration = reuFireRate.Value;
            FireBarrage.gracePeriodMin = reuGracePeriodMin.Value;
            FireBarrage.gracePeriodMax = reuGracePeriodMax.Value;
            FireBarrage.executeThreshold = reuExecuteThreshold.Value;
            FireBarrage.executeBosses = reuExecuteBosses.Value;
            FireBarrage.spread = reuSpread.Value;
            FireBarrage.buffDamageCoefficient = reuDebuffDamage.Value;
            FireBarrage.endLag = reuEndLag.Value;

            PrepBarrageScepter.baseDuration = reuDraw.Value;

            FireBarrageScepter.maxBullets = reuBullets.Value * 2;
            FireBarrageScepter.damageCoefficient = reuDamage.Value;
            FireBarrageScepter.force = reuForce.Value;
            FireBarrageScepter.baseDuration = reuFireRate.Value;
            FireBarrageScepter.gracePeriodMin = reuGracePeriodMin.Value;
            FireBarrageScepter.gracePeriodMax = reuGracePeriodMax.Value;
            FireBarrageScepter.executeThreshold = reuExecuteThreshold.Value;
            FireBarrageScepter.executeBosses = reuExecuteBosses.Value;
            FireBarrageScepter.spread = reuSpread.Value;
            FireBarrageScepter.buffDamageCoefficient = reuDebuffDamage.Value;
            FireBarrageScepter.endLag = reuEndLag.Value;

            PrepLightsOutScepter.baseDuration = loFireRate.Value;

            FireLightsOutScepter.damageCoefficient = loDamage.Value * 2f;
            FireLightsOutScepter.force = loForce.Value;
            FireLightsOutScepter.baseDuration = loEndLag.Value;
            FireLightsOutScepter.gracePeriodMin = loGracePeriodMin.Value;
            FireLightsOutScepter.gracePeriodMax = loGracePeriodMax.Value;
            FireLightsOutScepter.executeThreshold = loExecuteThreshold.Value;
            FireLightsOutScepter.executeBosses = loExecuteBosses.Value;
            FireLightsOutScepter.buffDamageCoefficient = loDebuffDamage.Value * 2f;

            LoadoutAPI.AddSkill(typeof(Blast));
            LoadoutAPI.AddSkill(typeof(CastSmokescreenNoDelay));
            LoadoutAPI.AddSkill(typeof(CastSmokescreen));
            LoadoutAPI.AddSkill(typeof(Assassinate));
            LoadoutAPI.AddSkill(typeof(FireChargeShot));
            LoadoutAPI.AddSkill(typeof(PrepLightsOut));
            LoadoutAPI.AddSkill(typeof(FireLightsOut));
            LoadoutAPI.AddSkill(typeof(AcidBomb));
            LoadoutAPI.AddSkill(typeof(ThermiteBomb));
            LoadoutAPI.AddSkill(typeof(Scatter));
            LoadoutAPI.AddSkill(typeof(ClusterBomb));
            LoadoutAPI.AddSkill(typeof(PrepBarrage));
            LoadoutAPI.AddSkill(typeof(FireBarrage));
            LoadoutAPI.AddSkill(typeof(FireBarrageScepter));
            LoadoutAPI.AddSkill(typeof(FireLightsOutScepter));
            LoadoutAPI.AddSkill(typeof(PrepBarrageScepter));
            LoadoutAPI.AddSkill(typeof(PrepLightsOutScepter));
        }

        private void ModifyClusterBomb(GameObject cbm)
        {
            float trueBombletDamage = cbBombletDamage.Value / cbDamage.Value;
            cbm.AddComponent<SphereCollider>();

            ProjectileImpactExplosion pie = cbm.GetComponent<ProjectileImpactExplosion>();
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
            cbm.GetComponent<ProjectileStickOnImpact>().ignoreWorld = true;
            pie.childrenProjectilePrefab = ClusterBombletObject;

            ProjectileSimple ps = cbm.GetComponent<ProjectileSimple>();
            ps.velocity = cbVelocity.Value;

            cbm.GetComponent<Rigidbody>().useGravity = true;

            ProjectileDamage pd = cbm.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Stun1s;
        }

        private void ModifyClusterBomblet(GameObject cbl)
        {
            cbl.AddComponent<SphereCollider>();

            ProjectileImpactExplosion pie = cbl.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = cbBombletRadius.Value;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.destroyOnEnemy = false;
            pie.destroyOnWorld = false;
            pie.lifetime = 1.5f;
            pie.timerAfterImpact = false;
            pie.blastProcCoefficient = cbBombletProcCoefficient.Value;
            cbl.GetComponent<ProjectileStickOnImpact>().ignoreWorld = true;

            ProjectileSimple ps = cbl.GetComponent<ProjectileSimple>();
            ps.velocity = 12f;

            ProjectileDamage pd = cbl.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Stun1s;
        }

        /*[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(specialLightsOutScepterDef, BanditBodyName, SkillSlot.Special, 0);
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(specialBarrageScepterDef, BanditBodyName, SkillSlot.Special, 1);
        }*/

        private void LoadSounds()
        {
            using (var bankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BanditReloaded.BanditReloaded.bnk"))
            {
                var bytes = new byte[bankStream.Length];
                bankStream.Read(bytes, 0, bytes.Length);
                SoundAPI.SoundBanks.Add(bytes);
            }
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
            cb.baseNameToken = "BANDITRELOADED_BODY_NAME";
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

        private void SetupProjectiles()
        {
            AcidBombObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile"), "BanditReloadedAcidBomb", true);
            AcidBombGhostObject = PrefabAPI.InstantiateClone(AcidBombObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedAcidBombGhost", false);
            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(AcidBombObject);
            };
            AcidBombObject.GetComponent<ProjectileController>().ghostPrefab = AcidBombGhostObject;
            ModifyAcidBomb(AcidBombObject);

            ThermiteObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/thermite"), "BanditReloadedThermite", true);
            ThermiteGhostObject = PrefabAPI.InstantiateClone(ThermiteObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedThermiteGhost", false);
            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(ThermiteObject);
            };
            ThermiteObject.GetComponent<ProjectileController>().ghostPrefab = ThermiteGhostObject;
            ModifyThermite(ThermiteObject);

            ClusterBombObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/BanditClusterBombSeed"), "BanditReloadedClusterBomb", true);
            ClusterBombGhostObject = PrefabAPI.InstantiateClone(ClusterBombObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedClusterBombGhost", false);
            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(ClusterBombObject);
            };
            ClusterBombObject.GetComponent<ProjectileController>().ghostPrefab = ClusterBombGhostObject;
            ModifyClusterBomb(ClusterBombObject);

            ClusterBombletObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/BanditClusterGrenadeProjectile"), "BanditReloadedClusterBomblet", true);
            ClusterBombletGhostObject = PrefabAPI.InstantiateClone(ClusterBombletObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedClusterBombletGhost", false);
            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(ClusterBombletObject);
            };
            ClusterBombletObject.GetComponent<ProjectileController>().ghostPrefab = ClusterBombletGhostObject;
            ModifyClusterBomblet(ClusterBombletObject);
        }

        private void SetupBanditBody()
        {
            BanditBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/banditbody"), "BanditReloadedBody", true);
            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(BanditBody);
            };
            BanditBodyName = BanditBody.name;

            DisplaySetup.DisplayRules(BanditBody);
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
                isDebuff = true,
                name = "BanditReloadedThermite"
            };
            BanditReloaded.thermiteBuff = BuffAPI.Add(new CustomBuff(SlowBuffDef));
            thermiteDoT = DotAPI.RegisterDotDef(0.5f, 0.25f, DamageColorIndex.Item, thermiteBuff);

            BuffDef cloakDamageBuffDef = new BuffDef
            {
                buffColor = BanditColor,
                buffIndex = BuffIndex.Count,
                canStack = false,
                eliteIndex = EliteIndex.None,
                iconPath = "Textures/BuffIcons/texBuffFullCritIcon",
                isDebuff = false,
                name = "BanditReloadedCloakDamage"
            };
            BanditReloaded.cloakDamageBuff = BuffAPI.Add(new CustomBuff(cloakDamageBuffDef));
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
    }
}

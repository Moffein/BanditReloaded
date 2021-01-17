using BanditReloaded.Components;
using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.BanditReloadedSkills;
using EntityStates.Engi.EngiWeapon;
using MonoMod;
using MonoMod.Utils;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Audio;
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
    [BepInPlugin("com.Moffein.BanditReloaded_v3", "Bandit Reloaded v3", "3.0.1")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(SurvivorAPI), nameof(LoadoutAPI), nameof(PrefabAPI), nameof(BuffAPI), nameof(ResourcesAPI), nameof(LanguageAPI), nameof(SoundAPI), nameof(EffectAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    class BanditReloaded : BaseUnityPlugin
    {
        #region cfg
        ConfigEntry<bool> usePassive, cloakAnim, useAltCrosshair, classicOutro, classicBlastSound;

        ConfigEntry<float> specialExecuteThreshold, loGracePeriodMin, loGracePeriodMax, specialDebuffBonus;
        ConfigEntry<bool> specialExecuteBosses;

        ConfigEntry<float> blastDamage, blastRange, blastMaxDuration, blastMinDuration, blastForce, blastSpread, blastRadius, blastMashSpread, blastRecoil, blastRechargeInterval;
        ConfigEntry<int> blastStock;
        ConfigEntry<bool> blastPenetrate, blastFalloff;

        ConfigEntry<float> thermiteDamage, thermiteBurnDuration, thermiteRadius, thermiteFireRate, thermiteProcCoefficient, thermiteCooldown, thermiteBurnDamageMult;
        ConfigEntry<int> thermiteStock;

        ConfigEntry<float> cloakDamage, cloakDuration, cloakMinDuration, cloakRadius, cloakCooldown, cloakProcCoefficient;
        ConfigEntry<int> cloakStock;
        ConfigEntry<bool> cloakNonLethal;

        ConfigEntry<float> loDamage, loForce, loFireRate, loEndLag, loCooldown;
        ConfigEntry<int> loStock;

        ConfigEntry<float> scatterDamage, scatterForce, scatterSpread, scatterProcCoefficient, scatterMaxDuration, scatterMinDuration, scatterRechargeInterval, scatterRange, scatterRadius, scatterRecoil;
        ConfigEntry<uint> scatterPellets;
        ConfigEntry<bool> scatterPenetrate;
        ConfigEntry<int> scatterStock;

        ConfigEntry<float> acidPoolDamage, acidDamage, acidRadius, acidFireRate, acidProcCoefficient, acidCooldown;
        ConfigEntry<int> acidStock;

        ConfigEntry<float> asMinDamage, asMaxDamage, asMinRadius, asMaxRadius, asChargeDuration, asMinForce, asMaxForce, asSelfForceMin, asSelfForceMax, asEndLag, asCooldown, asZoom;
        ConfigEntry<int> asStock;
        ConfigEntry<bool> asEnabled;

        ConfigEntry<float> cbDamage, cbRadius, cbBombletDamage, cbBombletRadius, cbBombletProcCoefficient, cbFireRate, cbCooldown;
        ConfigEntry<int> cbBombletCount;
        ConfigEntry<int> cbStock;

        ConfigEntry<float> reuDamage, reuForce, reuDraw, reuFireRate, reuEndLag, reuCooldown, reuSpread, reuRange, reuGracePeriodMin, reuGracePeriodMax;
        ConfigEntry<int> reuBullets, reuStock;

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

        GameObject loEffectMid = null;
        GameObject loEffectHigh = null;

        Color BanditColor = new Color(0.8039216f, 0.482352942f, 0.843137264f);
        String BanditBodyName = "";
        public static BuffIndex lightsOutBuff;
        public static BuffIndex thermiteBuff;
        public static BuffIndex cloakDamageBuff;

        Sprite iconSkill1 = null;
        Sprite iconSkill1a = null;
        Sprite iconSkill2 = null;
        Sprite iconSkill2a = null;
        Sprite iconSkill3 = null;
        Sprite iconSkill3a = null;
        Sprite iconSkill4 = null;
        Sprite iconPassive = null;
        Sprite iconClusterBomb = null;
        Sprite iconLOScepter = null;
        Sprite iconREUScepter = null;
        const String assetPrefix = "@MoffeinBanditReloaded";

        Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/hgstandard");

        public void RegisterLanguageTokens()
        {
            LanguageAPI.Add("BANDITRELOADED_PASSIVE_NAME", "Quickdraw");
            LanguageAPI.Add("BANDITRELOADED_PASSIVE_DESCRIPTION", "The Bandit <style=cIsUtility>instantly reloads</style> his primary when using other skills.");


            string outroNormal = "..and so he left, empty-handed.";
            string outroClassic = "..and so he left, with his pyrrhic plunder.";
            outroTextSelected = classicOutro.Value ? outroClassic : outroNormal;
            LanguageAPI.Add("BANDITRELOADED_OUTRO_FLAVOR", outroTextSelected);
            LanguageAPI.Add("BANDITRELOADED_OUTRO_EASTEREGG_FLAVOR", outroEasterEgg);

            LanguageAPI.Add("BANDITRELOADED_BODY_NAME", "Bandit");
            LanguageAPI.Add("BANDITRELOADED_BODY_SUBTITLE", "Wanted Dead or Alive");

            string BanditDesc = "The Bandit is a hit-and-run survivor who uses dirty tricks to assassinate his targets.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Space out your skill usage to keep firing Blast, or dump them all at once for massive damage!" + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Use grenades to apply debuffs to enemies, boosting the damage of Lights Out." + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Use Smokebomb to either run away or to stun many enemies at once." + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Dealing a killing blow with Lights Out allows you to chain many skills together, allowing for maximum damage AND safety." + Environment.NewLine + Environment.NewLine;
            LanguageAPI.Add("BANDITRELOADED_BODY_DESC", BanditDesc);

            LanguageAPI.Add("KEYWORD_BANDITRELOADED_RESET", "<style=cKeywordName>Resets Cooldowns</style><style=cSub>Killing an enemy with this ability <style=cIsUtility>resets all skill cooldowns to 0</style>. Enemies with <style=cIsHealth>low HP</style> have a longer cooldown reset time window.</style>");

            LanguageAPI.Add("KEYWORD_BANDITRELOADED_INVIS", "<style=cKeywordName>Invisible</style><style=cSub>Enemies are unable to target you.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_EXECUTE", "<style=cKeywordName>Executing</style><style=cSub>The ability <style=cIsHealth>instantly kills</style> enemies below <style=cIsHealth>"+specialExecuteThreshold.Value.ToString("P0").Replace(" ", "").Replace(",", "") + " HP</style>.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_RAPIDFIRE", "<style=cKeywordName>Rapid-Fire</style><style=cSub>The skill fires faster if you click faster.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_THERMITE", "<style=cKeywordName>Thermite</style><style=cSub>Reduce movement speed by <style=cIsDamage>15%</style> per stack. Reduce armor by <style=cIsDamage>2.5</style> per stack.</style>");

            LanguageAPI.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST", "<style=cKeywordName>Debuff Boosted</style><style=cSub>Gain <style=cIsDamage>+" + specialDebuffBonus.Value.ToString("P0").Replace(" ", "").Replace(",", "") + " TOTAL damage</style> for each unique debuff on the enemy.");

            LanguageAPI.Add("BANDITRELOADED_PRIMARY_NAME", "Blast");
            LanguageAPI.Add("BANDITRELOADED_PRIMARY_ALT_NAME", "Scatter");
            //LanguageAPI.Add("BANDITRELOADED_PRIMARY_ALT2_NAME", "Assassinate");

            LanguageAPI.Add("BANDITRELOADED_SECONDARY_NAME", "Dynamite Toss");
            LanguageAPI.Add("BANDITRELOADED_SECONDARY_ALT_NAME", "Thermite Flare");
            LanguageAPI.Add("BANDITRELOADED_SECONDARY_ALT2_NAME", "Acid Bomb");

            LanguageAPI.Add("BANDITRELOADED_UTILITY_NAME", "Smokebomb");

            LanguageAPI.Add("BANDITRELOADED_SPECIAL_NAME", "Lights Out");
            LanguageAPI.Add("BANDITRELOADED_SPECIAL_ALT_NAME", "Rack em Up");

            LanguageAPI.Add("BANDITRELOADED_SPECIAL_SCEPTER_NAME", "Decapitate");
            LanguageAPI.Add("BANDITRELOADED_SPECIAL_ALT_SCEPTER_NAME", "Fistful of Lead");
        }

        public void Start()
        {
            CastSmokescreenNoDelay.destealthMaterial = EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.destealthMaterial;
        }

        public void Awake()
        {
            LoadResources();
            ReadConfig();
            SetupBanditBody();
            SetupProjectiles();
            SetupLOEffect();
            SetIcons();
            SetAttributes();
            InitSkills();
            AssignSkills();
            CreateMaster();
            AddSkin();

            RegisterLanguageTokens();

            BanditBody.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/survivorpod");
            GameObject banditDisplay = BanditBody.GetComponent<ModelLocator>().modelTransform.gameObject;
            banditDisplay.AddComponent<MenuAnimComponent>();

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

            item = new SurvivorDef
            {
                name = "BanditReloaded",
                bodyPrefab = BanditBody,
                descriptionToken = "BANDITRELOADED_BODY_DESC",
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
                    self.moveSpeed = self.moveSpeed * Mathf.Pow(0.85f, tCount);
                    self.armor = self.armor - 2.5f * tCount;
                }
                int loCount = self.GetBuffCount(lightsOutBuff);
                self.moveSpeed = self.moveSpeed * Mathf.Pow(0.85f, loCount);
            };

            On.EntityStates.BaseState.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.HasBuff(cloakDamageBuff))
                {
                    self.damageStat *= 1.3f;
                }
            };

            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                bool aliveBeforeHit = self.alive;
                bool resetCooldownsOnKill = false;
                bool isBarrage = false;
                bool isDynamiteBundle = false;
                BanditNetworkCommands bnc = null;
                bool banditAttacker = false;
                AssignDynamiteTeamFilter ad = self.gameObject.GetComponent<AssignDynamiteTeamFilter>();
                if (ad)
                {
                    isDynamiteBundle = true;
                }


                CharacterBody attackerCB = null;
                if (damageInfo.attacker)
                {
                    attackerCB = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerCB)
                    {
                        banditAttacker = attackerCB.baseNameToken == "BANDITRELOADED_BODY_NAME";
                    }
                }

                if (banditAttacker)
                {
                    bnc = damageInfo.attacker.GetComponent<BanditNetworkCommands>();
                    if ((damageInfo.damageType & DamageType.ResetCooldownsOnKill) > 0)
                    {
                        if (damageInfo.damageType == (DamageType.ResetCooldownsOnKill | DamageType.SlowOnHit))
                        {
                            damageInfo.damageType = DamageType.ResetCooldownsOnKill;
                            isBarrage = true;
                        }

                        damageInfo.damageType = DamageType.Generic;

                        resetCooldownsOnKill = true;
                        int debuffCount = 0;
                        BuffDef b;
                        bool isDot;

                        DotController d = DotController.FindDotController(self.gameObject);
                        for (int i = 0; i < BuffCatalog.buffCount; i++)
                        {
                            isDot = false;
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
                            }

                            if (b.isDebuff || isDot)
                            {
                                if (self.body.HasBuff(b.buffIndex))
                                {
                                    if (b.buffIndex == lightsOutBuff)
                                    {
                                        debuffCount += self.body.GetBuffCount(lightsOutBuff);
                                    }
                                    else
                                    {
                                        debuffCount++;
                                    }
                                }
                            }
                        }

                        float buffDamage = 0f;
                        float buffBaseDamage = damageInfo.damage * specialDebuffBonus.Value;
                        buffDamage = buffBaseDamage * debuffCount;
                        damageInfo.damage += buffDamage;

                        BanditTimerComponent btc = self.gameObject.GetComponent<BanditTimerComponent>();
                        if (!btc)
                        {
                            btc = self.gameObject.AddComponent<BanditTimerComponent>();
                        }

                        bool lightWeight = false;
                        if (self.body)
                        {
                            Rigidbody rb = self.body.rigidbody;
                            if (rb)
                            {
                                if (rb.mass < 50f)
                                {
                                    lightWeight = true;
                                }
                            }
                        }

                        float resetDuration = 3.6f;
                        if (!lightWeight)
                        {
                            if (!isBarrage)
                            {
                                resetDuration = Mathf.Lerp(loGracePeriodMax.Value, loGracePeriodMin.Value, self.combinedHealthFraction);
                            }
                            else
                            {
                                resetDuration = Mathf.Lerp(reuGracePeriodMax.Value, reuGracePeriodMin.Value, self.combinedHealthFraction);
                            }
                        }
                        if (resetDuration > 0f)
                        {
                            self.body.AddTimedBuff(BanditReloaded.lightsOutBuff, resetDuration);
                            btc.AddTimer(damageInfo.attacker.GetComponent<SkillLocator>(), resetDuration);
                        }
                    }
                }

                if (isDynamiteBundle)
                {
                    if (!ad.fired && banditAttacker && (damageInfo.damageType & DamageType.AOE) == 0 && damageInfo.procCoefficient > 0f)
                    {
                        ad.fired = true;
                        damageInfo.crit = true;
                        damageInfo.procCoefficient = 0f;
                        ProjectileImpactExplosion pie = self.gameObject.GetComponent<ProjectileImpactExplosion>();
                        if (pie)
                        {
                            pie.blastRadius *= 2f;
                        }

                        ProjectileDamage pd = self.gameObject.GetComponent<ProjectileDamage>();
                        if (pd)
                        {
                            if (resetCooldownsOnKill)
                            {
                                pd.damage *= 2f;
                            }
                            else
                            {
                                pd.damage *= 1.5f;
                            }
                        }
                    }
                    else
                    {
                        damageInfo.rejected = true;
                    }
                }

                orig(self, damageInfo);

                if (!self.alive && aliveBeforeHit)
                {
                    if (self.globalDeathEventChanceCoefficient > 0f)
                    {
                        BanditTimerComponent btc = self.gameObject.GetComponent<BanditTimerComponent>();
                        if (btc)
                        {
                            btc.ResetCooldowns();
                        }
                    }
                }

                if (!damageInfo.rejected)
                {
                    if (banditAttacker)
                    {
                        if (resetCooldownsOnKill)
                        {
                            float damageCoefficient = damageInfo.damage / attackerCB.damage;
                            if ((!isBarrage && damageCoefficient > loDamage.Value * (1f + 2f * specialDebuffBonus.Value)) || (isBarrage && damageCoefficient > reuDamage.Value * (1f + 7f * specialDebuffBonus.Value)))
                            {
                                if ((!isBarrage && damageCoefficient > loDamage.Value * (1f + 4f * specialDebuffBonus.Value)) || (isBarrage && damageCoefficient > reuDamage.Value * (1f + 9f * specialDebuffBonus.Value)))
                                {
                                    EffectManager.SpawnEffect(loEffectHigh, new EffectData
                                    {
                                        color = BanditColor,
                                        origin = damageInfo.position,
                                        scale = 6f
                                    }, true);
                                    if (bnc)
                                    {
                                        bnc.RpcPlayLOHigh();
                                    }
                                }
                                else
                                {
                                    if (bnc)
                                    {
                                        bnc.RpcPlayLOMid();
                                    }
                                }
                                EffectManager.SpawnEffect(loEffectMid, new EffectData
                                {
                                    color = BanditColor,
                                    origin = damageInfo.position,
                                    scale = 6f
                                }, true);
                            }

                            if (self.alive)
                            {
                                if (specialExecuteThreshold.Value > 0f)
                                {
                                    if (((self.body.bodyFlags & CharacterBody.BodyFlags.ImmuneToExecutes) == 0 && !self.body.isChampion) || specialExecuteBosses.Value)
                                    {
                                        float executeThreshold = specialExecuteThreshold.Value;
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

                        if (isDynamiteBundle && resetCooldownsOnKill)
                        {
                            if (bnc)
                            {
                                bnc.RpcResetSpecialCooldown();
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
            usePassive = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Enable Passive"), true, new ConfigDescription("Makes Bandit insta-reload his primary when using other skills."));
            cloakAnim = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use Cloak Anim"), false, new ConfigDescription("Enables the unused Smokebomb entry animation."));
            useAltCrosshair = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use Alt Crosshair"), true, new ConfigDescription("Uses the unused Bandit-specific crosshair."));
            classicOutro = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use RoR1 Outro"), false, new ConfigDescription("Uses Bandit's RoR1 ending."));
            asEnabled = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Enable unused Assassinate utility*"), false, new ConfigDescription("Enables the Assassinate Utility skill. This skill was disabled due to being poorly coded and not fitting Bandit's kit, but it's left in in case you want to use it. This skill can only be used if Assassinate is enabled on the host."));

            classicBlastSound = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Classic Sound"), false, new ConfigDescription("Use Bandit's RoR1 sound for Blast."));
            blastDamage = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Damage"), 2.3f, new ConfigDescription("How much damage Blast deals."));
            blastMaxDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Fire Rate"), 0.3f, new ConfigDescription("Time between shots."));
            blastMinDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Min Duration"), 0.2f, new ConfigDescription("How soon you can fire another shot if you mash."));
            blastPenetrate = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Penetrate Enemies"), true, new ConfigDescription("Shots pierce enemies."));
            blastRadius = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Shot Radius"), 0.4f, new ConfigDescription("How wide Blast's shots are."));
            blastForce = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Force"), 600f, new ConfigDescription("Push force per shot."));
            blastSpread = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Spread"), 0f, new ConfigDescription("Amount of spread with added each shot."));
            blastMashSpread = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Mash Spread"), 0.4f, new ConfigDescription("Amount of spread with added each shot when mashing."));
            blastRecoil = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Recoil"), 1.4f, new ConfigDescription("How hard the gun kicks when shooting."));
            blastRange = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Range"), 300f, new ConfigDescription("How far Blast can reach."));
            blastFalloff = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Use Falloff"), true, new ConfigDescription("Shots deal less damage over range."));
            blastStock = base.Config.Bind<int>(new ConfigDefinition("10 - Primary - Blast", "Stock"), 10, new ConfigDescription("How many shots can be fired before reloading."));
            blastRechargeInterval = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Reload Time"), 2.0f, new ConfigDescription("How long it takes to reload. Set to 0 to disable reloading."));
            
            scatterDamage = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Damage"), 0.65f, new ConfigDescription("How much damage each pellet of Scatter deals."));
            scatterPellets = base.Config.Bind<uint>(new ConfigDefinition("11 - Primary - Scatter", "Pellets"), 8, new ConfigDescription("How many pellets Scatter shoots."));
            scatterProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Proc Coefficient"), 0.75f, new ConfigDescription("Affects the chance and power of each pellet's procs."));
            scatterMaxDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Fire Rate"), 0.625f, new ConfigDescription("Time between shots."));
            scatterMinDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Min Duration"), 0.625f, new ConfigDescription("How soon you can fire another shot if you mash."));
            scatterPenetrate = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Penetrate Enemies"), true, new ConfigDescription("Shots pierce enemies."));
            scatterRadius = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Shot Radius"), 0.4f, new ConfigDescription("How wide Scatter's pellets are."));
            scatterForce = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Force"), 200f, new ConfigDescription("Push force per pellet."));
            scatterSpread = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Spread"), 2.5f, new ConfigDescription("Size of the pellet spread."));
            scatterRecoil = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Recoil"), 2.6f, new ConfigDescription("How hard the gun kicks when shooting."));
            scatterRange = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Range"), 200f, new ConfigDescription("How far Scatter can reach."));
            scatterStock = base.Config.Bind<int>(new ConfigDefinition("11 - Primary - Scatter", "Stock"), 6, new ConfigDescription("How many shots Scatter can hold."));
            scatterRechargeInterval = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Reload Time"), 2f, new ConfigDescription("How much time it takes to reload. Set to 0 to disable reloading."));
            
            cbDamage = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Damage*"), 3.9f, new ConfigDescription("How much damage Dynamite Toss deals."));
            cbRadius = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Radius*"), 8f, new ConfigDescription("How large the explosion is. Radius is doubled when shot out of the air."));
            cbBombletCount = base.Config.Bind<int>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Bomblet Count*"), 6, new ConfigDescription("How many mini bombs Dynamite Toss releases."));
            cbBombletDamage = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Bomblet Damage*"), 1.2f, new ConfigDescription("How much damage Dynamite Toss Bomblets deals."));
            cbBombletRadius = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Bomblet Radius*"), 8f, new ConfigDescription("How large the mini explosions are."));
            cbBombletProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Bomblet Proc Coefficient*"), 0.6f, new ConfigDescription("Affects the chance and power of Dynamite Toss Bomblet procs."));
            cbFireRate = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Throw Duration"), 0.4f, new ConfigDescription("How long it takes to throw a Dynamite Toss."));
            cbCooldown = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Cooldown"), 7f, new ConfigDescription("How long it takes for Dynamite Toss to recharge."));
            cbStock = base.Config.Bind<int>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Stock"), 1, new ConfigDescription("How much Dynamite you start with."));

            acidDamage = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Damage"), 2.7f, new ConfigDescription("How much damage Acid Bomb deals."));
            acidPoolDamage = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Acid Pool Damage"), 0.4f, new ConfigDescription("How much damage Acid Bomb's acid pool deals per second."));
            acidRadius = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Radius*"), 8f, new ConfigDescription("How large the explosion is."));
            acidProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Acid Proc Coefficient*"), 0.2f, new ConfigDescription("Affects the chance and power of Acid Bomb's procs."));
            acidFireRate = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Throw Duration"), 0.4f, new ConfigDescription("How long it takes to throw a Acid Bomb."));
            acidCooldown = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Cooldown"), 7f, new ConfigDescription("How long Acid Bomb takes to recharge."));
            acidStock = base.Config.Bind<int>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Stock"), 1, new ConfigDescription("How many Acid Bombs you start with."));

            thermiteDamage = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Damage"), 4.8f, new ConfigDescription("How much damage Thermite Flare deals."));
            thermiteBurnDamageMult = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Burn Damage*"), 0.6f, new ConfigDescription("How much damage Thermite Flare deals per second."));
            thermiteBurnDuration = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Burn Duration*"), 7f, new ConfigDescription("How long the burn lasts for."));
            thermiteProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Burn Proc Coefficient*"), 0.2f, new ConfigDescription("Affects the chance and power of Thermite Flare's procs."));
            thermiteRadius = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Radius*"), 10f, new ConfigDescription("How large the explosion is. Radius is halved if it doesn't stick to a target."));
            thermiteFireRate = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Throw Duration"), 0.4f, new ConfigDescription("How long it takes to throw a Thermite Flare."));
            thermiteCooldown = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Cooldown"), 7f, new ConfigDescription("How long Thermite Flare takes to recharge."));
            thermiteStock = base.Config.Bind<int>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Stock"), 1, new ConfigDescription("How many Thermite Flares you start with."));

            cloakDamage = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Damage*"), 1.5f, new ConfigDescription("How much damage Smokebomb deals."));
            cloakRadius = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Radius*"), 10f, new ConfigDescription("Size of the stun radius."));
            cloakDuration = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Duration*"), 3f, new ConfigDescription("How long Smokebomb lasts."));
            cloakMinDuration = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Minimum Duration"), 0.3f, new ConfigDescription("Minimum amount of time Smokebomb lasts for."));
            cloakNonLethal = base.Config.Bind<bool>(new ConfigDefinition("30 - Utility - Smokebomb", "Nonlethal"), true, new ConfigDescription("Prevents Smokebomb from landing the killing blow on enemies."));
            cloakProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Proc Coefficient"), 0.5f, new ConfigDescription("Affects the chance and power of Smokebomb's procs."));
            cloakCooldown = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Cooldown"), 8f, new ConfigDescription("How long Smokebomb takes to recharge."));
            cloakStock = base.Config.Bind<int>(new ConfigDefinition("30 - Utility - Smokebomb", "Stock"), 1, new ConfigDescription("How many charges Smokebomb has."));

            specialDebuffBonus = base.Config.Bind<float>(new ConfigDefinition("40 - Special Settings", "Special Debuff Bonus Multiplier*"), 0.5f, new ConfigDescription("Multiplier for how big the debuff damage bonus should be for Bandit's specials."));
            specialExecuteThreshold = base.Config.Bind<float>(new ConfigDefinition("40 - Special Settings", "Special Execute Threshold*"), 0f, new ConfigDescription("Bandit's Specials instakill enemies that fall below this HP percentage. 0 = 0%, 1 = 100%"));
            specialExecuteBosses = base.Config.Bind<bool>(new ConfigDefinition("40 - Special Settings", "Special Execute Bosses*"), false, new ConfigDescription("Allow bosses to be executed by Bandit's Specials if Execute is enabled."));

            loDamage = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Damage"), 6f, new ConfigDescription("How much damage Lights Out deals."));
            loForce = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Force"), 2400f, new ConfigDescription("Push force per shot."));
            loFireRate = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Draw Time"), 0.6f, new ConfigDescription("How long it takes to prepare Lights Out."));
            loEndLag = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "End Lag"), 0.2f, new ConfigDescription("Delay after firing."));
            loCooldown = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Cooldown"), 7f, new ConfigDescription("How long Lights Out takes to recharge."));
            loStock = base.Config.Bind<int>(new ConfigDefinition("41 - Special - Lights Out", "Stock"), 1, new ConfigDescription("How many charges Lights Out has."));
            loGracePeriodMin = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Cooldown Reset Grace Period Min*"), 0.5f, new ConfigDescription("Minimum duration for Lights Out's cooldown reset window."));
            loGracePeriodMax = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Cooldown Reset Grace Period Max*"), 3.6f, new ConfigDescription("Maximum duration forLights Out's cooldown reset window."));

            reuDamage = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Damage"), 1f, new ConfigDescription("How much damage Rack em Up deals."));
            reuBullets = base.Config.Bind<int>(new ConfigDefinition("42 - Special - Rack em Up", "Total Shots"), 6, new ConfigDescription("How many shots are fired."));
            reuForce = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Force"), 100f, new ConfigDescription("Push force per shot."));
            reuDraw = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Draw Time"), 0.32f, new ConfigDescription("How long it takes to prepare Rack em Up."));
            reuFireRate = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Fire Rate"), 0.13f, new ConfigDescription("Time it takes for Rack em Up to fire a single shot."));
            reuEndLag = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "End Lag"), 0.4f, new ConfigDescription("Delay after firing all shots."));
            reuSpread = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Spread"), 2.5f, new ConfigDescription("Size of the cone of fire."));
            reuRange = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Range"), 120f, new ConfigDescription("How far shots reach."));
            reuCooldown = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Cooldown"), 7f, new ConfigDescription("How long Rack em Up takes to recharge."));
            reuStock = base.Config.Bind<int>(new ConfigDefinition("42 - Special - Rack em Up", "Stock"), 1, new ConfigDescription("How many charges Rack em Up has."));
            reuGracePeriodMin = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Cooldown Reset Grace Period Min*"), 0.8f, new ConfigDescription("Minimum duration for Rack em Up's cooldown reset window."));
            reuGracePeriodMax = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Cooldown Reset Grace Period Max*"), 1.6f, new ConfigDescription("Maximum duration for Rack em Up's Special cooldown reset window."));


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
            primaryBlastDef.skillDescriptionToken += "Fire a powerful slug " + (Blast.penetrateEnemies ? "that pierces " : "") + "for <style=cIsDamage>" + Blast.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            if (primaryBlastDef.baseRechargeInterval > 0f)
            {
                primaryBlastDef.skillDescriptionToken += " Reloads every " + primaryBlastDef.baseMaxStock + " shots.";
            }
            primaryBlastDef.skillDescriptionToken += Environment.NewLine;

            primaryBlastDef.skillName = "FireSlug";
            primaryBlastDef.skillNameToken = "BANDITRELOADED_PRIMARY_NAME";
            primaryBlastDef.activationStateMachineName = "Weapon";
            primaryBlastDef.shootDelay = 0;
            primaryBlastDef.beginSkillCooldownOnSkillEnd = false;
            primaryBlastDef.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryBlastDef.isCombatSkill = true;
            primaryBlastDef.noSprint = true;
            primaryBlastDef.canceledFromSprinting = false;
            primaryBlastDef.mustKeyPress = false;
            primaryBlastDef.icon = iconSkill1;

            primaryBlastDef.requiredStock = 1;
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
            primaryScatterDef.skillNameToken = "BANDITRELOADED_PRIMARY_ALT_NAME";
            primaryScatterDef.skillDescriptionToken = "";
            primaryScatterDef.skillDescriptionToken += "Fire a volley of " + (Scatter.penetrateEnemies ? "piercing flechettes " : "buckshot ") + "for <style=cIsDamage>" + Scatter.pelletCount + "x" + Scatter.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            if (primaryScatterDef.baseRechargeInterval > 0f)
            {
                primaryScatterDef.skillDescriptionToken += " Reloads every " + primaryScatterDef.baseMaxStock + " shots.";
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
            primaryScatterDef.requiredStock = 1;
            primaryScatterDef.stockToConsume = 1;

            LoadoutAPI.AddSkillDef(primaryScatterDef);
            #endregion

            #region CastSmokescreen
            utilityDefA = SkillDef.CreateInstance<SkillDef>();
            utilityDefA.activationState = new SerializableEntityStateType(typeof(CastSmokescreenNoDelay));
            utilityDefA.baseRechargeInterval = cloakCooldown.Value;
            utilityDefA.skillName = "CloakBanditReloaded";
            utilityDefA.skillNameToken = "BANDITRELOADED_UTILITY_NAME";
            utilityDefA.skillDescriptionToken = "<style=cIsUtility>Turn invisible</style>. After " + CastSmokescreenNoDelay.duration.ToString("N1")
                + " seconds, <style=cIsDamage>stun enemies for " + CastSmokescreenNoDelay.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>,"
                + " and your next attack gains <style=cIsDamage>+30%</style> TOTAL damage.";
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
            utilityDefA.forceSprintDuringState = false;
            utilityDefA.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_INVIS", "KEYWORD_STUNNING" };
            LoadoutAPI.AddSkillDef(utilityDefA);

            utilityDefB = SkillDef.CreateInstance<SkillDef>();
            utilityDefB.activationState = new SerializableEntityStateType(typeof(CastSmokescreen));
            utilityDefB.baseRechargeInterval = cloakCooldown.Value;
            utilityDefB.skillName = "CloakAnimBanditReloaded";
            utilityDefB.skillNameToken = "BANDITRELOADED_UTILITY_NAME";
            utilityDefB.skillDescriptionToken = "<style=cIsUtility>Turn invisible</style>. After " + (CastSmokescreen.stealthDuration - 0.8f).ToString("N1")
                + " seconds, <style=cIsDamage>stun enemies for " + CastSmokescreen.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>,"
                + " and your next attack gains <style=cIsDamage>+30% TOTAL damage</style>.";
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
            utilityDefB.forceSprintDuringState = false;
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
            if (specialExecuteThreshold.Value > 0f)
            {
                kwlLO.Add("KEYWORD_BANDITRELOADED_EXECUTE");
                specialLightsOutDef.skillDescriptionToken += "<style=cIsDamage>Executing</style>. ";
            }
            if (specialDebuffBonus.Value > 0f)
            {
                kwlLO.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST");
                specialLightsOutDef.skillDescriptionToken += "<style=cIsDamage>Debuff Boosted</style>. ";
            }
            kwlLO.Add("KEYWORD_BANDITRELOADED_RESET");
            specialLightsOutDef.keywordTokens = kwlLO.ToArray();

            specialLightsOutDef.skillDescriptionToken += "Take aim with your Persuader, <style=cIsDamage>dealing " + FireLightsOut.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialLightsOutDef.skillDescriptionToken += " <style=cIsUtility>Resets skill cooldowns on kill</style>.";
            specialLightsOutDef.skillDescriptionToken += Environment.NewLine;
            specialLightsOutDef.baseRechargeInterval = loCooldown.Value;
            specialLightsOutDef.skillNameToken = "BANDITRELOADED_SPECIAL_NAME";
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
            thermiteDef.skillNameToken = "BANDITRELOADED_SECONDARY_ALT_NAME";
            thermiteDef.skillDescriptionToken = "Fire a flare that coats enemies in <color=#cd7bd7>Thermite</color>, dealing <style=cIsDamage>" + thermiteBurnDamageMult.Value.ToString("P0").Replace(" ", "").Replace(",", "") + " damage per second</style>."
                + " Explodes for <style=cIsDamage>" + thermiteDamage.Value.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>. New flares <style=cIsUtility>reset the burn timer</style>.";
            thermiteDef.skillDescriptionToken += Environment.NewLine;
            thermiteDef.skillName = "Thermite";
            thermiteDef.icon = iconSkill2;
            thermiteDef.shootDelay = 0f;
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
            thermiteDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_THERMITE" };
            LoadoutAPI.AddSkillDef(thermiteDef);
            #endregion

            #region Acid Bomb

            acidBombDef = SkillDef.CreateInstance<SkillDef>();
            acidBombDef.activationState = new SerializableEntityStateType(typeof(AcidBomb));
            acidBombDef.baseRechargeInterval = acidCooldown.Value;
            acidBombDef.skillNameToken = "BANDITRELOADED_SECONDARY_ALT2_NAME";
            acidBombDef.skillDescriptionToken = "Toss a grenade that <style=cIsHealing>Weakens</style> for <style=cIsDamage>" + (AcidBomb.damageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>."
                + " Leaves acid that deals <style=cIsDamage>"+acidPoolDamage.Value.ToString("P0").Replace(" ", "").Replace(",", "" )+ " damage per second</style>.";
            acidBombDef.skillDescriptionToken += Environment.NewLine;
            acidBombDef.skillName = "AcidGrenade";
            acidBombDef.icon = iconSkill2a;
            acidBombDef.shootDelay = 0f;
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
            clusterBombDef.skillNameToken = "BANDITRELOADED_SECONDARY_NAME";
            clusterBombDef.skillDescriptionToken = "Toss a bomb that <style=cIsDamage>ignites</style> for <style=cIsDamage>" + (ClusterBomb.damageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>."
                +" Drops bomblets for <style=cIsDamage>" + cbBombletCount.Value + "x" + (ClusterBomb.bombletDamageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>."
                + " Can be shot midair for <style=cIsDamage>bonus damage</style>.";
            clusterBombDef.skillDescriptionToken += Environment.NewLine;
            clusterBombDef.skillName = "Dynamite";
            clusterBombDef.icon = iconClusterBomb;
            clusterBombDef.shootDelay = 0f;
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
            clusterBombDef.keywordTokens = new string[] {};
            LoadoutAPI.AddSkillDef(clusterBombDef);
            #endregion
            
            #region barrage
            specialBarrageDef = SkillDef.CreateInstance<SkillDef>();
            specialBarrageDef.activationState = new SerializableEntityStateType(typeof(PrepBarrage));
            List<string> kwlBarrage = new List<string>();
            if (specialExecuteThreshold.Value > 0f)
            {
                kwlBarrage.Add("KEYWORD_BANDITRELOADED_EXECUTE");
            }
            if (specialDebuffBonus.Value > 0f)
            {
                kwlBarrage.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST");
            }
            kwlBarrage.Add("KEYWORD_BANDITRELOADED_RESET");
            specialBarrageDef.keywordTokens = kwlBarrage.ToArray();

            float barrageBonusDamage = FireBarrage.damageCoefficient * specialDebuffBonus.Value;
            specialBarrageDef.skillDescriptionToken += "<style=cIsDamage>Debuff Boosted</style>. Rapidly fire bullets, dealing <style=cIsDamage>" + FireBarrage.maxBullets + "x" + FireBarrage.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialBarrageDef.skillDescriptionToken += " Consecutive hits deal <style=cIsDamage>+" + barrageBonusDamage.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> each.";
            specialBarrageDef.skillDescriptionToken += " <style=cIsUtility>Resets skill cooldowns on kill</style>.";
            specialBarrageDef.skillDescriptionToken += Environment.NewLine;
            specialBarrageDef.baseRechargeInterval = reuCooldown.Value;
            specialBarrageDef.skillNameToken = "BANDITRELOADED_SPECIAL_ALT_NAME";
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
            if (specialExecuteThreshold.Value > 0f)
            {
                specialBarrageScepterDef.skillDescriptionToken = " <style=cIsDamage>Executing</style>. ";
                specialBarrageScepterDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_EXECUTE", "KEYWORD_BANDITRELOADED_DEBUFFBOOST2", "KEYWORD_BANDITRELOADED_RESET" };
            }
            else
            {
                specialBarrageScepterDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_DEBUFFBOOST", "KEYWORD_BANDITRELOADED_RESET" };
            }
            float barrageScepterBonusDamage = FireBarrageScepter.damageCoefficient * specialDebuffBonus.Value;
            specialBarrageScepterDef.skillDescriptionToken += "<style=cIsDamage>Debuff Boosted</style>. Rapidly fire bullets, dealing <style=cIsDamage>" + FireBarrageScepter.maxBullets + "x" + FireBarrageScepter.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialBarrageScepterDef.skillDescriptionToken += " Consecutive hits deal <style=cIsDamage>+" + barrageScepterBonusDamage.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> each.";
            specialBarrageScepterDef.skillDescriptionToken += " <style=cIsUtility>Resets skill cooldowns on kill</style>.";
            specialBarrageScepterDef.skillDescriptionToken += Environment.NewLine;
            specialBarrageScepterDef.baseRechargeInterval = reuCooldown.Value;
            specialBarrageScepterDef.skillNameToken = "BANDITRELOADED_SPECIAL_ALT_SCEPTER_NAME";
            specialBarrageScepterDef.skillName = "BanditBarrageScepter";
            specialBarrageScepterDef.baseMaxStock = reuStock.Value;
            specialBarrageScepterDef.rechargeStock = 1;
            specialBarrageScepterDef.isBullets = false;
            specialBarrageScepterDef.shootDelay = 0f;
            specialBarrageScepterDef.activationStateMachineName = "Weapon";
            specialBarrageScepterDef.icon = iconREUScepter;
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
            if (specialExecuteThreshold.Value > 0f)
            {
                specialLightsOutScepterDef.skillDescriptionToken += "<style=cIsDamage>Executing</style>. ";
                specialLightsOutScepterDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_EXECUTE", "KEYWORD_BANDITRELOADED_DEBUFFBOOST", "KEYWORD_BANDITRELOADED_RESET" };
            }
            else
            {
                specialLightsOutScepterDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_DEBUFFBOOST", "KEYWORD_BANDITRELOADED_RESET" };
            }
            specialLightsOutScepterDef.skillDescriptionToken += "<style=cIsDamage>Debuff Boosted</style>. Take aim with your Persuader, <style=cIsDamage>dealing " + FireLightsOutScepter.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialLightsOutScepterDef.skillDescriptionToken += " <style=cIsUtility>Resets skill cooldowns on kill</style>.";

            specialLightsOutScepterDef.skillDescriptionToken += Environment.NewLine;
            specialLightsOutScepterDef.baseRechargeInterval = loCooldown.Value;
            specialLightsOutScepterDef.skillNameToken = "BANDITRELOADED_SPECIAL_SCEPTER_NAME";
            specialLightsOutScepterDef.skillName = "LightsOutScepter";
            specialLightsOutScepterDef.baseMaxStock = loStock.Value;
            specialLightsOutScepterDef.rechargeStock = 1;
            specialLightsOutScepterDef.isBullets = false;
            specialLightsOutScepterDef.shootDelay = 0f;
            specialLightsOutScepterDef.activationStateMachineName = "Weapon";
            specialLightsOutScepterDef.icon = iconLOScepter;
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

            secondarySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = clusterBombDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(clusterBombDef.skillNameToken, false)
            };
            Array.Resize(ref secondarySkillFamily.variants, secondarySkillFamily.variants.Length + 1);
            secondarySkillFamily.variants[secondarySkillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = thermiteDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(thermiteDef.skillNameToken, false)
            };
            Array.Resize(ref secondarySkillFamily.variants, secondarySkillFamily.variants.Length + 1);
            secondarySkillFamily.variants[secondarySkillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = acidBombDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(acidBombDef.skillNameToken, false)
            };


            SkillDef toInsert;
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

        private void SetupAcidBomb()
        {
            AcidBombObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile"), "BanditReloadedAcidBomb", true);
            AcidBombGhostObject = PrefabAPI.InstantiateClone(AcidBombObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedAcidBombGhost", false);
            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(AcidBombObject);
            };
            AcidBombObject.GetComponent<ProjectileController>().ghostPrefab = AcidBombGhostObject;

            GameObject puddleObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/crocoleapacid"), "BanditReloadedAcidBombPuddle", true);
            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(puddleObject);
            };
            ProjectileDamage puddleDamage = puddleObject.GetComponent<ProjectileDamage>();
            puddleDamage.damageType = DamageType.WeakOnHit;
            ProjectileDotZone pdz = puddleObject.GetComponent<ProjectileDotZone>();
            pdz.attackerFiltering = AttackerFiltering.Default;
            pdz.overlapProcCoefficient = acidProcCoefficient.Value;
            pdz.lifetime = 5f;
            pdz.damageCoefficient = acidPoolDamage.Value / acidDamage.Value;

            GameObject abImpact = Resources.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion").InstantiateClone("BanditReloadedAcidEffect", false);
            EffectComponent ec = abImpact.GetComponent<EffectComponent>();
            //ec.applyScale = true;
            //ec.disregardZScale = false;
            ec.soundName = "Play_acrid_shift_land";
            EffectAPI.AddEffect(abImpact);

            AcidBombObject.GetComponent<ProjectileSimple>().velocity = 60f;
            ProjectileImpactExplosion abPIE = AcidBombObject.GetComponent<ProjectileImpactExplosion>();
            abPIE.blastRadius = acidRadius.Value;
            AcidBombObject.GetComponent<ProjectileDamage>().damageType = DamageType.WeakOnHit;
            abPIE.blastProcCoefficient = 1f;
            abPIE.falloffModel = BlastAttack.FalloffModel.None;
            abPIE.lifetime = 20f;
            abPIE.impactEffect = abImpact;
            abPIE.explosionSoundString = "";
            abPIE.timerAfterImpact = false;
            abPIE.lifetimeAfterImpact = 0f;
            abPIE.destroyOnWorld = true;
            abPIE.destroyOnEnemy = true;
            abPIE.fireChildren = true;
            abPIE.childrenCount = 1;
            abPIE.childrenDamageCoefficient = 1f;
            abPIE.childrenProjectilePrefab = puddleObject;
            Destroy(AcidBombObject.GetComponent<ProjectileStickOnImpact>());
            AcidBombObject.GetComponent<Rigidbody>().useGravity = true;
        }

        private void SetupThermite()
        {
            ThermiteObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/thermite"), "BanditReloadedThermite", true);
            ThermiteGhostObject = PrefabAPI.InstantiateClone(ThermiteObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedThermiteGhost", false);
            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(ThermiteObject);
            };
            ThermiteObject.GetComponent<ProjectileController>().ghostPrefab = ThermiteGhostObject;

            ProjectileImpactExplosion tPIE = ThermiteObject.GetComponent<ProjectileImpactExplosion>();
            tPIE.blastRadius = thermiteRadius.Value/2f;
            tPIE.blastProcCoefficient = 1f;
            tPIE.blastDamageCoefficient = 1f;
            tPIE.falloffModel = BlastAttack.FalloffModel.None;
            tPIE.timerAfterImpact = false;
            tPIE.lifetime = 20f;
            tPIE.destroyOnEnemy = false;
            tPIE.destroyOnWorld = true;
            ThermiteObject.GetComponent<ProjectileDamage>().damageType = DamageType.Stun1s;

            ProjectileSimple ps = ThermiteObject.GetComponent<ProjectileSimple>();
            ps.velocity = 100f;
            ps.lifetime = 20f;

            ProjectileDamage pd = ThermiteObject.GetComponent<ProjectileDamage>();
            pd.damage = 1f;
            pd.damageColorIndex = DamageColorIndex.Default;

            SphereCollider thermiteSphere = ThermiteObject.GetComponent<SphereCollider>();
            if (thermiteSphere == null)
            {
                thermiteSphere = ThermiteObject.AddComponent<SphereCollider>();
            }
            thermiteSphere.radius = 0.6f; //old radius: 0.25f

            ProjectileController tPC = ThermiteObject.GetComponent<ProjectileController>();
            if (tPC == null)
            {
                tPC = ThermiteObject.AddComponent<ProjectileController>();
            }
            tPC.procCoefficient = 1f;

            Destroy(ThermiteObject.GetComponent<ProjectileIntervalOverlapAttack>());

            BootlegThermiteOverlapAttack bootlegPOA = ThermiteObject.AddComponent<BootlegThermiteOverlapAttack>();
            bootlegPOA.damageCoefficient = 0.5f * thermiteBurnDamageMult.Value / thermiteDamage.Value;
            bootlegPOA.procCoefficient = thermiteProcCoefficient.Value;
            bootlegPOA.damageInterval = 0.5f;
            bootlegPOA.lifetimeAfterImpact = thermiteBurnDuration.Value;

            ThermiteBomb.projectilePrefab = ThermiteObject;

            GameObject thermiteBurnEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/missileexplosionvfx").InstantiateClone("BanditReloadedThermiteBurnEffect", false);
            thermiteBurnEffect.GetComponent<EffectComponent>().soundName = "Play_BanditReloaded_burn";
            EffectAPI.AddEffect(thermiteBurnEffect);
            BootlegThermiteOverlapAttack.burnEffectPrefab = thermiteBurnEffect;
        }

        private void SetupClusterBomb()
        {
            ClusterBombObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/BanditClusterBombSeed"), "BanditReloadedClusterBomb", true);
            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(ClusterBombObject);
            };

            ClusterBombGhostObject = Resources.Load<GameObject>(assetPrefix + ":DynamiteBundle.prefab").InstantiateClone("BanditReloadedClusterBombGhost", true);
            ClusterBombGhostObject.GetComponentInChildren<MeshRenderer>().material.shader = hotpoo;
            ClusterBombGhostObject.AddComponent<ProjectileGhostController>();

            ClusterBombObject.AddComponent<DynamiteRotation>();

            ClusterBombObject.GetComponent<ProjectileController>().ghostPrefab = ClusterBombGhostObject;


            float trueBombletDamage = cbBombletDamage.Value / cbDamage.Value;
            SphereCollider sc = ClusterBombObject.AddComponent<SphereCollider>();
            sc.radius = 0.6f;
            sc.contactOffset = 0.01f;

            TeamComponent tc = ClusterBombObject.AddComponent<TeamComponent>();
            tc.hideAllyCardDisplay = false;
            ClusterBombObject.AddComponent<SkillLocator>();

            CharacterBody cb = ClusterBombObject.AddComponent<CharacterBody>();
            cb.rootMotionInMainState = false;
            cb.bodyFlags = CharacterBody.BodyFlags.Masterless;
            cb.baseMaxHealth = 1f;
            cb.baseCrit = 0f;
            cb.baseAcceleration = 0f;
            cb.baseArmor = 0f;
            cb.baseAttackSpeed = 0f;
            cb.baseDamage = 0f;
            cb.baseJumpCount = 0;
            cb.baseJumpPower = 0f;
            cb.baseMoveSpeed = 0f;
            cb.baseMaxShield = 0f;
            cb.baseRegen = 0f;
            cb.autoCalculateLevelStats = true;
            cb.levelArmor = 0f;
            cb.levelAttackSpeed = 0f;
            cb.levelCrit = 0f;
            cb.levelDamage = 0f;
            cb.levelJumpPower = 0f;
            cb.levelMaxHealth = 0f;
            cb.levelMaxShield = 0f;
            cb.levelMoveSpeed = 0f;
            cb.levelRegen = 0f;
            cb.hullClassification = HullClassification.Human;

            HealthComponent hc = ClusterBombObject.AddComponent<HealthComponent>();
            hc.globalDeathEventChanceCoefficient = 0f;
            hc.body = cb;

            ClusterBombObject.AddComponent<AssignDynamiteTeamFilter>();

            ProjectileImpactExplosion pie = ClusterBombObject.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = cbRadius.Value;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.lifetime = 25f;
            pie.lifetimeAfterImpact = 1.5f;
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = false;
            pie.childrenCount = cbBombletCount.Value;
            pie.childrenDamageCoefficient = trueBombletDamage;
            pie.blastProcCoefficient = 1f;
            pie.impactEffect = SetupDynamiteExplosion();

            pie.explosionSoundString = "";
            pie.lifetimeExpiredSound = null;
            pie.projectileHealthComponent = hc;
            pie.transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            Destroy(ClusterBombObject.GetComponent<ProjectileStickOnImpact>());

            ProjectileSimple ps = ClusterBombObject.GetComponent<ProjectileSimple>();
            ps.velocity = 60f;
            ps.lifetime = 25f;

            ClusterBombObject.GetComponent<Rigidbody>().useGravity = true;

            ProjectileDamage pd = ClusterBombObject.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.IgniteOnHit;


            AddDynamiteHurtbox(ClusterBombObject);
        }

        private void AddDynamiteHurtbox(GameObject go)
        {
            GameObject hbObject = new GameObject();
            hbObject.transform.parent = go.transform;
            //GameObject hbObject = go;

            hbObject.layer = LayerIndex.entityPrecise.intVal;
            SphereCollider goCollider = hbObject.AddComponent<SphereCollider>();
            goCollider.radius = 0.9f;

            HurtBoxGroup goHurtBoxGroup = hbObject.AddComponent<HurtBoxGroup>();
            HurtBox goHurtBox = hbObject.AddComponent<HurtBox>();
            goHurtBox.isBullseye = false;
            goHurtBox.healthComponent = go.GetComponent<HealthComponent>();
            goHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            goHurtBox.hurtBoxGroup = goHurtBoxGroup;
            goHurtBox.indexInGroup = 0;

            HurtBox[] goHurtBoxArray = new HurtBox[]
            {
                goHurtBox
            };

            goHurtBoxGroup.bullseyeCount = 0;
            goHurtBoxGroup.hurtBoxes = goHurtBoxArray;
            goHurtBoxGroup.mainHurtBox = goHurtBox;

            DisableCollisionsBetweenColliders dc = go.AddComponent<DisableCollisionsBetweenColliders>();
            dc.collidersA = go.GetComponents<Collider>();
            dc.collidersB = hbObject.GetComponents<Collider>();
        }

        private GameObject SetupDynamiteExplosion()
        {
            GameObject dynamiteExplosion = Resources.Load<GameObject>("prefabs/effects/omnieffect/omniexplosionvfx").InstantiateClone("BanditReloadedDynamiteExplosion", false);
            ShakeEmitter se = dynamiteExplosion.AddComponent<ShakeEmitter>();
            se.shakeOnStart = true;
            se.duration = 0.5f;
            se.scaleShakeRadiusWithLocalScale = false;
            se.radius = 75f;
            se.wave = new Wave()
            {
                amplitude = 1f,
                cycleOffset = 0f,
                frequency = 40f
            };

            EffectComponent ec = dynamiteExplosion.GetComponent<EffectComponent>();
            ec.soundName = "Play_BanditReloaded_dynamite";

            EffectAPI.AddEffect(dynamiteExplosion);
            return dynamiteExplosion;
        }

        private void SetupClusterBomblet()
        {
            ClusterBombletObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/BanditClusterGrenadeProjectile"), "BanditReloadedClusterBomblet", true);
            ProjectileCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(ClusterBombletObject);
            };

            ClusterBombletGhostObject = Resources.Load<GameObject>(assetPrefix + ":DynamiteStick.prefab").InstantiateClone("BanditReloadedClusterBombletGhost", true);
            ClusterBombletGhostObject.GetComponentInChildren<MeshRenderer>().material.shader = hotpoo;
            ClusterBombletGhostObject.AddComponent<ProjectileGhostController>();

            ClusterBombObject.GetComponent<ProjectileImpactExplosion>().childrenProjectilePrefab = ClusterBombletObject;

            ClusterBombletObject.AddComponent<SphereCollider>();
            ClusterBombletObject.GetComponent<ProjectileController>().ghostPrefab = ClusterBombletGhostObject;

            ProjectileImpactExplosion pie = ClusterBombletObject.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = cbBombletRadius.Value;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.destroyOnEnemy = false;
            pie.destroyOnWorld = false;
            pie.lifetime = 1.5f;
            pie.timerAfterImpact = false;
            pie.blastProcCoefficient = cbBombletProcCoefficient.Value;
            pie.explosionSoundString = "";
            pie.impactEffect = SetupDynamiteBombletExplosion();

            Destroy(ClusterBombletObject.GetComponent<ProjectileStickOnImpact>());

            ProjectileSimple ps = ClusterBombletObject.GetComponent<ProjectileSimple>();
            ps.velocity = 12f;

            ProjectileDamage pd = ClusterBombletObject.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.IgniteOnHit;
        }

        private GameObject SetupDynamiteBombletExplosion()
        {
            GameObject dynamiteExplosion = Resources.Load<GameObject>("prefabs/effects/impacteffects/explosionvfx").InstantiateClone("BanditReloadedDynamiteBombletExplosion", false);

            EffectComponent ec = dynamiteExplosion.GetComponent<EffectComponent>();
            ec.soundName = "Play_engi_M2_explo";

            EffectAPI.AddEffect(dynamiteExplosion);
            return dynamiteExplosion;
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
            Blast.penetrateEnemies = blastPenetrate.Value;
            Blast.useFalloff = blastFalloff.Value;
            Blast.mashSpread = blastMashSpread.Value;
            Blast.useClassicSound = classicBlastSound.Value;

            CastSmokescreen.stealthDuration = cloakDuration.Value + 0.8f;
            CastSmokescreen.damageCoefficient = cloakDamage.Value;
            CastSmokescreen.radius = cloakRadius.Value;
            CastSmokescreen.procCoefficient = cloakProcCoefficient.Value;
            CastSmokescreen.nonLethal = cloakNonLethal.Value;

            CastSmokescreenNoDelay.duration = cloakDuration.Value;
            CastSmokescreenNoDelay.damageCoefficient = cloakDamage.Value;
            CastSmokescreenNoDelay.radius = cloakRadius.Value;
            CastSmokescreenNoDelay.minimumStateDuration = cloakMinDuration.Value;
            CastSmokescreenNoDelay.procCoefficient = cloakProcCoefficient.Value;
            CastSmokescreenNoDelay.nonLethal = cloakNonLethal.Value;

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

            PrepLightsOut.baseDuration = loFireRate.Value;

            FireLightsOut.damageCoefficient = loDamage.Value;
            FireLightsOut.force = loForce.Value;
            FireLightsOut.baseDuration = loEndLag.Value;

            AcidBomb.projectilePrefab = AcidBombObject;
            AcidBomb.damageCoefficient = acidDamage.Value;
            AcidBomb.baseDuration = acidFireRate.Value;

            ThermiteBomb.projectilePrefab = ThermiteObject;
            ThermiteBomb.damageCoefficient = thermiteDamage.Value;
            ThermiteBomb.baseDuration = thermiteFireRate.Value;
            ThermiteBomb.burnDamageMult = thermiteBurnDamageMult.Value;

            Scatter.procCoefficient = scatterProcCoefficient.Value;
            Scatter.pelletCount = scatterPellets.Value;
            Scatter.damageCoefficient = scatterDamage.Value;
            Scatter.force = scatterForce.Value;
            Scatter.baseMaxDuration = scatterMaxDuration.Value;
            Scatter.baseMinDuration = scatterMinDuration.Value;
            Scatter.spreadBloomValue = scatterSpread.Value;
            Scatter.recoilAmplitude = scatterRecoil.Value;
            Scatter.penetrateEnemies = scatterPenetrate.Value;
            Scatter.range = scatterRange.Value;

            ClusterBomb.baseDuration = cbFireRate.Value;
            ClusterBomb.damageCoefficient = cbDamage.Value;
            ClusterBomb.projectilePrefab = ClusterBombObject;
            ClusterBomb.bombletDamageCoefficient = cbBombletDamage.Value;

            PrepBarrage.baseDuration = reuDraw.Value;

            FireBarrage.maxBullets = reuBullets.Value;
            FireBarrage.damageCoefficient = reuDamage.Value;
            FireBarrage.force = reuForce.Value;
            FireBarrage.baseDuration = reuFireRate.Value;
            FireBarrage.spread = reuSpread.Value;
            FireBarrage.endLag = reuEndLag.Value;

            PrepBarrageScepter.baseDuration = reuDraw.Value;

            FireBarrageScepter.maxBullets = reuBullets.Value * 2;
            FireBarrageScepter.damageCoefficient = reuDamage.Value;
            FireBarrageScepter.force = reuForce.Value;
            FireBarrageScepter.baseDuration = reuFireRate.Value;
            FireBarrageScepter.spread = reuSpread.Value;
            FireBarrageScepter.endLag = reuEndLag.Value;

            PrepLightsOutScepter.baseDuration = loFireRate.Value;

            FireLightsOutScepter.damageCoefficient = loDamage.Value * 2f;
            FireLightsOutScepter.force = loForce.Value;
            FireLightsOutScepter.baseDuration = loEndLag.Value;

            LoadoutAPI.AddSkill(typeof(Blast));
            LoadoutAPI.AddSkill(typeof(CastSmokescreenNoDelay));
            LoadoutAPI.AddSkill(typeof(CastSmokescreen));
            LoadoutAPI.AddSkill(typeof(Assassinate));
            LoadoutAPI.AddSkill(typeof(FireChargeShot));
            LoadoutAPI.AddSkill(typeof(PrepLightsOut));
            LoadoutAPI.AddSkill(typeof(FireLightsOut));
            LoadoutAPI.AddSkill(typeof(AcidBomb));
            LoadoutAPI.AddSkill(typeof(PrepFlare));
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

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            ThinkInvisible.ClassicItems.Scepter_V2.instance.RegisterScepterSkill(specialLightsOutScepterDef, BanditBodyName, SkillSlot.Special, 0);
            ThinkInvisible.ClassicItems.Scepter_V2.instance.RegisterScepterSkill(specialBarrageScepterDef, BanditBodyName, SkillSlot.Special, 1);
        }

        private void LoadResources()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BanditReloaded.banditbundle"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new R2API.AssetBundleResourcesProvider(assetPrefix, bundle);
                R2API.ResourcesAPI.AddProvider(provider);
            }

            using (var bankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BanditReloaded.BanditReloaded.bnk"))
            {
                var bytes = new byte[bankStream.Length];
                bankStream.Read(bytes, 0, bytes.Length);
                SoundAPI.SoundBanks.Add(bytes);
            }
        }

        private void SetIcons()
        {
            iconSkill1 = Resources.Load<Sprite>(assetPrefix + ":skill1.png");
            iconSkill1a = Resources.Load<Sprite>(assetPrefix + ":skill1a.png");
            iconSkill2 = Resources.Load<Sprite>(assetPrefix + ":skill2.png");
            iconSkill2a = Resources.Load<Sprite>(assetPrefix + ":skill2a.png");
            iconSkill3 = Resources.Load<Sprite>(assetPrefix + ":skill3.png");
            iconSkill3a = Resources.Load<Sprite>(assetPrefix + ":skill3a.png");
            iconSkill4 = Resources.Load<Sprite>(assetPrefix + ":skill4.png");
            iconPassive = Resources.Load<Sprite>(assetPrefix + ":quickdraw.png");
            iconClusterBomb = Resources.Load<Sprite>(assetPrefix + ":dynamite_red.png");
            iconLOScepter = Resources.Load<Sprite>(assetPrefix + ":lo_scepter.png");
            iconREUScepter = Resources.Load<Sprite>(assetPrefix + ":reu_scepter.png");
        }

        private void SetAttributes()
        {
            BanditBody.tag = "Player";
            CharacterBody cb = BanditBody.GetComponent<CharacterBody>();
            cb.subtitleNameToken = "BANDITRELOADED_BODY_SUBTITLE";
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
            BanditBody.AddComponent<BanditNetworkCommands>();
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
            reposition.maxDistance = 30f;
            reposition.selectionRequiresTargetLoS = false;
            reposition.activationRequiresTargetLoS = false;
            reposition.activationRequiresAimConfirmation = false;
            reposition.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            reposition.aimType = AISkillDriver.AimType.None;
            reposition.ignoreNodeGraph = false;
            reposition.driverUpdateTimerOverride = 1.6f;
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
            SetupThermite();
            SetupAcidBomb();
            SetupClusterBomb();
            SetupClusterBomblet();
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

        private void SetupLOEffect()
        {
            loEffectMid = Resources.Load<GameObject>("prefabs/effects/BleedOnHitAndExplode_Explosion").InstantiateClone("BanditReloadedLOMidEffect", false);
            EffectComponent ecMid = loEffectMid.GetComponent<EffectComponent>();
            ecMid.soundName = "Play_item_proc_armorReduction_hit";
            ecMid.applyScale = false;
            EffectAPI.AddEffect(loEffectMid);

            loEffectHigh = Resources.Load<GameObject>("prefabs/effects/AncientWispEnrage").InstantiateClone("BanditReloadedLOHighEffect", false);
            Transform effectTransform = loEffectHigh.transform.Find("SwingTrail");
            var effectRenderer = effectTransform.GetComponent<Renderer>();
            if (effectRenderer)
            {
                effectRenderer.material = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun").transform.Find("Fire").GetComponent<Renderer>().material;
            }
            EffectComponent ecHigh = loEffectHigh.GetComponent<EffectComponent>();
            ecHigh.soundName = "Play_item_proc_armorReduction_shatter";
            ecHigh.applyScale = false;
            VFXAttributes vfxHigh = loEffectHigh.GetComponent<VFXAttributes>();
            vfxHigh.vfxPriority = VFXAttributes.VFXPriority.Medium;

            Destroy(loEffectHigh.GetComponent<ShakeEmitter>());

            EffectAPI.AddEffect(loEffectHigh);
        }
    }
}

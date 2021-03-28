using BanditReloaded.Components;
using BanditReloaded.Hooks;
using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.Bandit2.Weapon;
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
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.Moffein.BanditReloaded_v4", "Bandit Reloaded v4 beta 1", "4.0.0")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(PrefabAPI), nameof(ResourcesAPI), nameof(LanguageAPI), nameof(SoundAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    class BanditReloaded : BaseUnityPlugin
    {
        #region cfg
        bool useAltCrosshair;

        int blastStock;

        float thermiteBurnDuration, thermiteRadius, thermiteProcCoefficient, thermiteCooldown;
        int thermiteStock;

        float cloakCooldown;
        int cloakStock;

        float loCooldown;
        int loStock;

        int scatterStock;

        float acidRadius, acidProcCoefficient, acidCooldown;
        int acidStock;

        float asCooldown;
        int asStock;
        bool asEnabled;

        float cbRadius, cbBombletRadius, cbBombletProcCoefficient, cbCooldown;
        int cbStock, cbBombletCount;

        float reuCooldown;
        int reuStock;

        #endregion
        public static SurvivorDef item;

        public ReloadSkillDef primaryBlastDef, primaryScatterDef;
        public SkillDef utilityDefA, utilityAltDef, thermiteDef, acidBombDef, specialLightsOutDef, clusterBombDef, specialBarrageDef, specialBarrageScepterDef, specialLightsOutScepterDef;

        public static GameObject BanditBody = null;
        public GameObject BanditMonsterMaster = null;

        public GameObject AcidBombObject = null;
        public GameObject ThermiteObject = null;
        public GameObject ClusterBombObject = null;
        public GameObject ClusterBombletObject = null;

        public GameObject AcidBombGhostObject = null;
        public GameObject ThermiteGhostObject = null;
        public GameObject ClusterBombGhostObject = null;
        public GameObject ClusterBombletGhostObject = null;

        public Color BanditColor = new Color(0.8039216f, 0.482352942f, 0.843137264f);
        String BanditBodyName = "";

        const string assetPrefix = "@MoffeinBanditReloaded";

        private readonly Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/hgstandard");

        public void RegisterLanguageTokens()
        {
            LanguageAPI.Add("BANDITRELOADED_PASSIVE_NAME", "Quickdraw");
            LanguageAPI.Add("BANDITRELOADED_PASSIVE_DESCRIPTION", "The Bandit <style=cIsUtility>instantly reloads</style> his primary when using other skills.");

            LanguageAPI.Add("BANDITRELOADED_OUTRO_FLAVOR", "..and so he left, with his pyrrhic plunder.");
            LanguageAPI.Add("BANDITRELOADED_OUTRO_EASTEREGG_FLAVOR", "..and so he left, seeking warmth.");

            LanguageAPI.Add("BANDITRELOADED_BODY_NAME", "Classic Bandit");
            LanguageAPI.Add("BANDITRELOADED_BODY_SUBTITLE", "Wanted Dead or Alive");

            string BanditDesc = "The Bandit is a hit-and-run survivor who uses dirty tricks to assassinate his targets.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Space out your skill usage to keep firing Blast, or dump them all at once for massive damage!" + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Use grenades to apply debuffs to enemies, boosting the damage of Lights Out." + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Use Smokebomb to either run away or to stun many enemies at once." + Environment.NewLine + Environment.NewLine;
            BanditDesc += "< ! > Dealing a killing blow with Lights Out allows you to chain many skills together, allowing for maximum damage AND safety." + Environment.NewLine + Environment.NewLine;
            LanguageAPI.Add("BANDITRELOADED_BODY_DESC", BanditDesc);

            LanguageAPI.Add("KEYWORD_BANDITRELOADED_EXECUTE", "<style=cKeywordName>Executing</style><style=cSub>The ability <style=cIsHealth>instantly kills</style> enemies below <style=cIsHealth>"+TakeDamage.specialExecuteThreshold.ToString("P0").Replace(" ", "").Replace(",", "") + " HP</style>.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_RAPIDFIRE", "<style=cKeywordName>Rapid-Fire</style><style=cSub>The skill fires faster if you click faster.</style>");
            LanguageAPI.Add("KEYWORD_BANDITRELOADED_THERMITE", "<style=cKeywordName>Thermite</style><style=cSub>Reduce movement speed by <style=cIsDamage>15%</style> per stack. Reduce armor by <style=cIsDamage>2.5</style> per stack.</style>");

            LanguageAPI.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST", "<style=cKeywordName>Debuff Boosted</style><style=cSub>Gain <style=cIsDamage>+" + TakeDamage.specialDebuffBonus.ToString("P0").Replace(" ", "").Replace(",", "") + " TOTAL damage</style> for each unique debuff on the enemy.");

            LanguageAPI.Add("BANDITRELOADED_PRIMARY_NAME", "Blast");
            LanguageAPI.Add("BANDITRELOADED_PRIMARY_ALT_NAME", "Scatter");

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
            Assassinate.chargeupVfxPrefab = EntityStates.Toolbot.ChargeSpear.chargeupVfxPrefab;
            Assassinate.holdChargeVfxPrefab = EntityStates.Toolbot.ChargeSpear.holdChargeVfxPrefab;
        }

        public void Awake()
        {
            LoadResources();
            ReadConfig();
            SetupBanditBody();
            SetupProjectiles();
            SetAttributes();
            AssignSkills();
            CreateMaster();
            CreateLightsOutBuff();

            RegisterLanguageTokens();

            BanditBody.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/survivorpod");
            GameObject banditDisplay = Resources.Load<GameObject>("Prefabs/CharacterDisplays/Bandit2Display");
            item = SurvivorDef.CreateInstance<SurvivorDef>();
            item.cachedName = "BanditReloaded";
            item.bodyPrefab = BanditBody;
            item.descriptionToken = "BANDITRELOADED_BODY_DESC";
            item.displayPrefab = banditDisplay;
            item.primaryColor = BanditColor;
            item.unlockableName = "";
            item.outroFlavorToken = "BANDITRELOADED_OUTRO_FLAVOR";
            ModContentPack.survivorDefs.Add(item);

            /*On.RoR2.SkillLocator.ResetSkills += (orig, self) =>
            {
                orig(self);
                if (self.gameObject.GetComponentInParent<CharacterBody>().baseNameToken == "BANDITRELOADED_BODY_NAME")
                {
                    Util.PlaySound("Play_BanditReloaded_reset", self.gameObject);
                }
            };*/

            TakeDamage.AddHook();
            RecalculateStats.AddHook();
            CloakDamage.AddHook();

            ModContentPack.CreateContentPack();
        }

        private void ReadConfig()
        {
            useAltCrosshair = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Use Alt Crosshair"), true, new ConfigDescription("Uses the unused Bandit-specific crosshair.")).Value;
            asEnabled = base.Config.Bind<bool>(new ConfigDefinition("01 - General Settings", "Enable unused Assassinate utility*"), false, new ConfigDescription("Enables the Assassinate Utility skill. This skill was disabled due to being poorly coded and not fitting Bandit's kit, but it's left in in case you want to use it. This skill can only be used if Assassinate is enabled on the host.")).Value;

            string blastSound = base.Config.Bind<string>(new ConfigDefinition("10 - Primary - Blast", "Firing Sound"), "vanilla", new ConfigDescription("Which sound Blast plays when firing. Accepted values are 'new', 'classic', and 'vanilla'.")).Value;
            switch (blastSound.ToLower())
            {
                case "classic":
                    Blast.attackSoundString = "Play_BanditReloaded_blast_classic";
                    break;
                case "new":
                    Blast.attackSoundString = "Play_BanditReloaded_blast";
                    break;
                default:
                    Blast.attackSoundString = "Play_bandit2_m1_rifle";
                    break;
            }
            Blast.damageCoefficient = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Damage"), 2.6f, new ConfigDescription("How much damage Blast deals.")).Value;
            Blast.baseMaxDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Fire Rate"), 0.3f, new ConfigDescription("Time between shots.")).Value;
            Blast.baseMinDuration = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Min Duration"), 0f, new ConfigDescription("How soon you can fire another shot if you mash.")).Value;
            Blast.penetrateEnemies = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Penetrate Enemies"), true, new ConfigDescription("Shots pierce enemies.")).Value;
            Blast.bulletRadius = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Shot Radius"), 0.4f, new ConfigDescription("How wide Blast's shots are.")).Value;
            Blast.force = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Force"), 600f, new ConfigDescription("Push force per shot.")).Value;
            Blast.spreadBloomValue = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Spread"), 0.5f, new ConfigDescription("Amount of spread with added when mashing.")).Value;
            Blast.recoilAmplitude = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Recoil"), 1.4f, new ConfigDescription("How hard the gun kicks when shooting.")).Value;
            Blast.maxDistance = base.Config.Bind<float>(new ConfigDefinition("10 - Primary - Blast", "Range"), 300f, new ConfigDescription("How far Blast can reach.")).Value;
            Blast.useFalloff = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Use Falloff"), false, new ConfigDescription("Shots deal less damage over range.")).Value;
            blastStock = base.Config.Bind<int>(new ConfigDefinition("10 - Primary - Blast", "Stock"), 8, new ConfigDescription("How many shots can be fired before reloading.")).Value;
            Blast.noReload = base.Config.Bind<bool>(new ConfigDefinition("10 - Primary - Blast", "Disable Reload"), false, new ConfigDescription("Makes Blast never need to reload.")).Value;

            Scatter.damageCoefficient = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Damage"), 0.7f, new ConfigDescription("How much damage each pellet of Scatter deals.")).Value;
            Scatter.pelletCount = base.Config.Bind<uint>(new ConfigDefinition("11 - Primary - Scatter", "Pellets"), 8, new ConfigDescription("How many pellets Scatter shoots.")).Value;
            Scatter.procCoefficient = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Proc Coefficient"), 0.75f, new ConfigDescription("Affects the chance and power of each pellet's procs.")).Value;
            Scatter.baseMaxDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Fire Rate"), 0.625f, new ConfigDescription("Time between shots.")).Value;
            Scatter.baseMinDuration = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Min Duration"), 0.625f, new ConfigDescription("How soon you can fire another shot if you mash.")).Value;
            Scatter.penetrateEnemies = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Penetrate Enemies"), true, new ConfigDescription("Shots pierce enemies.")).Value;
            Scatter.bulletRadius = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Shot Radius"), 0.4f, new ConfigDescription("How wide Scatter's pellets are.")).Value;
            Scatter.force = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Force"), 200f, new ConfigDescription("Push force per pellet.")).Value;
            Scatter.spreadBloomValue = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Spread"), 2.5f, new ConfigDescription("Size of the pellet spread.")).Value;
            Scatter.recoilAmplitude = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Recoil"), 2.6f, new ConfigDescription("How hard the gun kicks when shooting.")).Value;
            Scatter.range = base.Config.Bind<float>(new ConfigDefinition("11 - Primary - Scatter", "Range"), 200f, new ConfigDescription("How far Scatter can reach.")).Value;
            scatterStock = base.Config.Bind<int>(new ConfigDefinition("11 - Primary - Scatter", "Stock"), 6, new ConfigDescription("How many shots Scatter can hold.")).Value;
            Scatter.noReload = base.Config.Bind<bool>(new ConfigDefinition("11 - Primary - Scatter", "Disable Reload"), false, new ConfigDescription("Makes Scatter never need to reload.")).Value;

            ClusterBomb.damageCoefficient = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Damage*"), 3.9f, new ConfigDescription("How much damage Dynamite Toss deals.")).Value;
            cbRadius = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Radius*"), 8f, new ConfigDescription("How large the explosion is. Radius is doubled when shot out of the air.")).Value;
            cbBombletCount = base.Config.Bind<int>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Bomblet Count*"), 6, new ConfigDescription("How many mini bombs Dynamite Toss releases.")).Value;
            ClusterBomb.bombletDamageCoefficient = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Bomblet Damage*"), 1.2f, new ConfigDescription("How much damage Dynamite Toss Bomblets deals.")).Value;
            cbBombletRadius = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Bomblet Radius*"), 8f, new ConfigDescription("How large the mini explosions are.")).Value;
            cbBombletProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Bomblet Proc Coefficient*"), 0.6f, new ConfigDescription("Affects the chance and power of Dynamite Toss Bomblet procs.")).Value;
            ClusterBomb.baseDuration = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Throw Duration"), 0.4f, new ConfigDescription("How long it takes to throw a Dynamite Bundle.")).Value;
            cbCooldown = base.Config.Bind<float>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Cooldown"), 6f, new ConfigDescription("How long it takes for Dynamite Toss to recharge.")).Value;
            cbStock = base.Config.Bind<int>(new ConfigDefinition("20 - Secondary - Dynamite Toss", "Stock"), 1, new ConfigDescription("How much Dynamite you start with.")).Value;

            AcidBomb.damageCoefficient = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Damage"), 2.7f, new ConfigDescription("How much damage Acid Bomb deals.")).Value;
            AcidBomb.acidDamageCoefficient = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Acid Pool Damage"), 0.4f, new ConfigDescription("How much damage Acid Bomb's acid pool deals per second.")).Value;
            acidRadius = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Radius*"), 8f, new ConfigDescription("How large the explosion is.")).Value;
            acidProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Acid Proc Coefficient*"), 0.2f, new ConfigDescription("Affects the chance and power of Acid Bomb's procs.")).Value;
            AcidBomb.baseDuration = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Throw Duration"), 0.4f, new ConfigDescription("How long it takes to throw a Acid Bomb.")).Value;
            acidCooldown = base.Config.Bind<float>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Cooldown"), 6f, new ConfigDescription("How long Acid Bomb takes to recharge.")).Value;
            acidStock = base.Config.Bind<int>(new ConfigDefinition("21 - Secondary - Acid Bomb", "Stock"), 1, new ConfigDescription("How many Acid Bombs you start with.")).Value;

            ThermiteBomb.damageCoefficient = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Damage"), 4.8f, new ConfigDescription("How much damage Thermite Flare deals.")).Value;
            ThermiteBomb.burnDamageMult = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Burn Damage*"), 0.6f, new ConfigDescription("How much damage Thermite Flare deals per second.")).Value;
            thermiteBurnDuration = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Burn Duration*"), 7f, new ConfigDescription("How long the burn lasts for.")).Value;
            thermiteProcCoefficient = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Burn Proc Coefficient*"), 0.4f, new ConfigDescription("Affects the chance and power of Thermite Flare's procs.")).Value;
            thermiteRadius = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Radius*"), 10f, new ConfigDescription("How large the explosion is. Radius is halved if it doesn't stick to a target.")).Value;
            ThermiteBomb.baseDuration = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Throw Duration"), 0.4f, new ConfigDescription("How long it takes to shoot a Thermite Flare.")).Value;
            thermiteCooldown = base.Config.Bind<float>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Cooldown"), 6f, new ConfigDescription("How long Thermite Flare takes to recharge.")).Value;
            thermiteStock = base.Config.Bind<int>(new ConfigDefinition("22 - Secondary - Thermite Flare", "Stock"), 1, new ConfigDescription("How many Thermite Flares you start with.")).Value;

            CastSmokescreenNoDelay.damageCoefficient = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Damage*"), 2f, new ConfigDescription("How much damage Smokebomb deals.")).Value;
            CastSmokescreenNoDelay.radius = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Radius*"), 12f, new ConfigDescription("Size of the stun radius.")).Value;
            CastSmokescreenNoDelay.duration = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Duration*"), 3f, new ConfigDescription("How long Smokebomb lasts.")).Value;
            CastSmokescreenNoDelay.minimumStateDuration = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Minimum Duration"), 0.3f, new ConfigDescription("Minimum amount of time Smokebomb lasts for.")).Value;
            CastSmokescreenNoDelay.nonLethal = base.Config.Bind<bool>(new ConfigDefinition("30 - Utility - Smokebomb", "Nonlethal"), true, new ConfigDescription("Prevents Smokebomb from landing the killing blow on enemies.")).Value;
            CastSmokescreenNoDelay.procCoefficient = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Proc Coefficient"), 0.5f, new ConfigDescription("Affects the chance and power of Smokebomb's procs.")).Value;
            cloakCooldown = base.Config.Bind<float>(new ConfigDefinition("30 - Utility - Smokebomb", "Cooldown"), 8f, new ConfigDescription("How long Smokebomb takes to recharge.")).Value;
            cloakStock = base.Config.Bind<int>(new ConfigDefinition("30 - Utility - Smokebomb", "Stock"), 1, new ConfigDescription("How many charges Smokebomb has.")).Value;

            TakeDamage.specialDebuffBonus = base.Config.Bind<float>(new ConfigDefinition("40 - Special Settings", "Special Debuff Bonus Multiplier*"), 0.5f, new ConfigDescription("Multiplier for how big the debuff damage bonus should be for Bandit's specials.")).Value;

            TakeDamage.specialExecuteThreshold = base.Config.Bind<float>(new ConfigDefinition("40 - Special Settings", "Special Execute Threshold*"), 0.15f, new ConfigDescription("Bandit's Specials instakill enemies that fall below this HP percentage. 0 = 0%, 1 = 100%")).Value;
            TakeDamage.specialExecuteBosses = base.Config.Bind<bool>(new ConfigDefinition("40 - Special Settings", "Special Execute Bosses*"), true, new ConfigDescription("Allow bosses to be executed by Bandit's Specials if Execute is enabled.")).Value;

            FireLightsOut.damageCoefficient = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Damage"), 6f, new ConfigDescription("How much damage Lights Out deals.")).Value;
            FireLightsOut.force = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Force"), 2400f, new ConfigDescription("Push force per shot.")).Value;
            PrepLightsOut.baseDuration = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Draw Time"), 0.6f, new ConfigDescription("How long it takes to prepare Lights Out.")).Value;
            FireLightsOut.baseDuration = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "End Lag"), 0.2f, new ConfigDescription("Delay after firing.")).Value;
            loCooldown = base.Config.Bind<float>(new ConfigDefinition("41 - Special - Lights Out", "Cooldown"), 7f, new ConfigDescription("How long Lights Out takes to recharge.")).Value;
            loStock = base.Config.Bind<int>(new ConfigDefinition("41 - Special - Lights Out", "Stock"), 1, new ConfigDescription("How many charges Lights Out has.")).Value;

            PrepLightsOutScepter.baseDuration = PrepLightsOut.baseDuration;
            FireLightsOutScepter.damageCoefficient = FireLightsOut.damageCoefficient * 2f;
            FireLightsOutScepter.force = FireLightsOut.force;
            FireLightsOutScepter.baseDuration = FireLightsOut.baseDuration;


            FireBarrage.damageCoefficient = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Damage"), 1f, new ConfigDescription("How much damage Rack em Up deals.")).Value;
            FireBarrage.maxBullets = base.Config.Bind<int>(new ConfigDefinition("42 - Special - Rack em Up", "Total Shots"), 6, new ConfigDescription("How many shots are fired.")).Value;
            FireBarrage.force = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Force"), 100f, new ConfigDescription("Push force per shot.")).Value;
            PrepBarrage.baseDuration = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Draw Time"), 0.32f, new ConfigDescription("How long it takes to prepare Rack em Up.")).Value;
            FireBarrage.baseDuration = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Fire Rate"), 0.13f, new ConfigDescription("Time it takes for Rack em Up to fire a single shot.")).Value;
            FireBarrage.endLag = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "End Lag"), 0.4f, new ConfigDescription("Delay after firing all shots.")).Value;
            FireBarrage.spread = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Spread"), 2.5f, new ConfigDescription("Size of the cone of fire.")).Value;
            FireBarrage.maxDistance = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Range"), 200f, new ConfigDescription("How far shots reach.")).Value;
            reuCooldown = base.Config.Bind<float>(new ConfigDefinition("42 - Special - Rack em Up", "Cooldown"), 7f, new ConfigDescription("How long Rack em Up takes to recharge.")).Value;
            reuStock = base.Config.Bind<int>(new ConfigDefinition("42 - Special - Rack em Up", "Stock"), 1, new ConfigDescription("How many charges Rack em Up has.")).Value;

            PrepBarrageScepter.baseDuration = PrepBarrage.baseDuration;

            FireBarrageScepter.maxBullets = FireBarrage.maxBullets * 2;
            FireBarrageScepter.damageCoefficient = FireBarrage.damageCoefficient;
            FireBarrageScepter.force = FireBarrage.force;
            FireBarrageScepter.baseDuration = FireBarrage.baseDuration;
            FireBarrageScepter.spread = FireBarrage.spread;
            FireBarrageScepter.endLag = FireBarrage.endLag;
            FireBarrageScepter.maxDistance = FireBarrage.maxDistance;

            FireChargeShot.minDamageCoefficient = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Minimum Damage"), 2.5f, new ConfigDescription("How much damage Assassinate deals at no charge.")).Value;
            FireChargeShot.maxDamageCoefficient = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Maximum Damage"), 17f, new ConfigDescription("How much damage Assassinate deals at max charge.")).Value;
            FireChargeShot.minRadius = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Minimum Radius"), 0.4f, new ConfigDescription("How large Assassinate's shot radius is at no charge.")).Value;
            FireChargeShot.maxRadius = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Maximum Radius"), 2.4f, new ConfigDescription("How large Assassinate's shot radius is at max charge.")).Value;
            FireChargeShot.minForce = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Minimum Force"), 600f, new ConfigDescription("Push force at no charge.")).Value;
            FireChargeShot.maxForce = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Maximum Force"), 2400f, new ConfigDescription("Push force at max charge.")).Value;
            FireChargeShot.selfForceMin = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Minimum Self Force"), 4500f, new ConfigDescription("How far back you are launched when firing at no charge.")).Value;
            FireChargeShot.selfForceMax = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Maximum Self Force"), 4500f, new ConfigDescription("How far back you are launched when firing at max charge.")).Value;
            Assassinate.baseChargeDuration = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Charge Duration"), 1.5f, new ConfigDescription("How long it takes to fully charge Assassinate.")).Value;
            FireChargeShot.baseDuration = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "End Lag"), 0.5f, new ConfigDescription("Delay after firing.")).Value;
            Assassinate.zoomFOV = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Zoom FOV"), -1f, new ConfigDescription("Zoom-in FOV when charging Assassinate. -1 disables.")).Value;
            asCooldown = base.Config.Bind<float>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Cooldown"), 5f, new ConfigDescription("How long it takes Assassinate to recharge")).Value;
            asStock = base.Config.Bind<int>(new ConfigDefinition("99 -  Deprecated - Assassinate", "Stock"), 1, new ConfigDescription("How many charges Assassinate has.")).Value;
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

            skillComponent.passiveSkill.enabled = true;
            skillComponent.passiveSkill.skillNameToken = "BANDITRELOADED_PASSIVE_NAME";
            skillComponent.passiveSkill.skillDescriptionToken = "BANDITRELOADED_PASSIVE_DESCRIPTION";
            skillComponent.passiveSkill.icon = Resources.Load<Sprite>(assetPrefix + ":quickdraw.png");

            #region Blast
            primaryBlastDef = ReloadSkillDef.CreateInstance<ReloadSkillDef>();
            primaryBlastDef.activationState = new SerializableEntityStateType(typeof(Blast));
            primaryBlastDef.baseRechargeInterval = 0f;
            if (!Blast.noReload)
            {
                primaryBlastDef.baseMaxStock = blastStock;
                primaryBlastDef.rechargeStock = 0;
            }
            else
            {
                primaryBlastDef.baseRechargeInterval = 0f;
                primaryBlastDef.baseMaxStock = 1;
                primaryBlastDef.rechargeStock = 1;
            }
            primaryBlastDef.skillDescriptionToken = "";
            primaryBlastDef.skillDescriptionToken += "Fire a slug " + (Blast.penetrateEnemies ? "that pierces " : "") + "for <style=cIsDamage>" + Blast.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            if (primaryBlastDef.baseRechargeInterval > 0f)
            {
                primaryBlastDef.skillDescriptionToken += " Can hold up to " + primaryBlastDef.baseMaxStock + " bullets.";
            }
            primaryBlastDef.skillDescriptionToken += Environment.NewLine;

            primaryBlastDef.skillName = "FireSlug";
            primaryBlastDef.skillNameToken = "BANDITRELOADED_PRIMARY_NAME";
            primaryBlastDef.activationStateMachineName = "Weapon";
            primaryBlastDef.beginSkillCooldownOnSkillEnd = false;
            primaryBlastDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            primaryBlastDef.isCombatSkill = true;
            primaryBlastDef.cancelSprintingOnActivation = true;
            primaryBlastDef.canceledFromSprinting = false;
            primaryBlastDef.mustKeyPress = false;
            primaryBlastDef.icon = Resources.Load<Sprite>(assetPrefix + ":skill1.png");

            primaryBlastDef.requiredStock = 1;
            primaryBlastDef.stockToConsume = 1;

            primaryBlastDef.reloadInterruptPriority = InterruptPriority.Any;
            primaryBlastDef.reloadState = new SerializableEntityStateType(typeof(EntityStates.Bandit2.Weapon.EnterReload));
            primaryBlastDef.graceDuration = 0.5f;

            ModContentPack.skillDefs.Add(primaryBlastDef);
            #endregion

            #region scatter
            primaryScatterDef = ReloadSkillDef.CreateInstance<ReloadSkillDef>();
            primaryScatterDef.activationState = new SerializableEntityStateType(typeof(Scatter));
            primaryScatterDef.baseRechargeInterval = 0f;
            if (!Scatter.noReload)
            {
                primaryScatterDef.baseMaxStock = scatterStock;
                primaryScatterDef.rechargeStock = 0;
            }
            else
            {
                primaryScatterDef.baseRechargeInterval = 0f;
                primaryScatterDef.baseMaxStock = 1;
                primaryScatterDef.rechargeStock = 1;
            }

            primaryScatterDef.skillName = "FireScatter";
            primaryScatterDef.skillNameToken = "BANDITRELOADED_PRIMARY_ALT_NAME";
            primaryScatterDef.skillDescriptionToken = "";
            primaryScatterDef.skillDescriptionToken += "Fire a volley of " + (Scatter.penetrateEnemies ? "piercing flechettes " : "buckshot ") + "for <style=cIsDamage>" + Scatter.pelletCount + "x" + Scatter.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            if (primaryScatterDef.baseRechargeInterval > 0f)
            {
                primaryScatterDef.skillDescriptionToken += " Can hold up to " + primaryScatterDef.baseMaxStock + " shells.";
            }
            primaryScatterDef.skillDescriptionToken += Environment.NewLine;
            primaryScatterDef.activationStateMachineName = "Weapon";
            primaryScatterDef.beginSkillCooldownOnSkillEnd = false;
            primaryScatterDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            primaryScatterDef.isCombatSkill = true;
            primaryScatterDef.cancelSprintingOnActivation = true;
            primaryScatterDef.canceledFromSprinting = false;
            primaryScatterDef.mustKeyPress = false;
            primaryScatterDef.icon = Resources.Load<Sprite>(assetPrefix + ":skill1a.png");
            primaryScatterDef.requiredStock = 1;
            primaryScatterDef.stockToConsume = 1;

            primaryScatterDef.reloadInterruptPriority = InterruptPriority.Any;
            primaryScatterDef.reloadState = new SerializableEntityStateType(typeof(EntityStates.Bandit2.Weapon.EnterReload));
            primaryScatterDef.graceDuration = 0.5f;

            ModContentPack.skillDefs.Add(primaryScatterDef);
            #endregion

            #region CastSmokescreen
            utilityDefA = SkillDef.CreateInstance<SkillDef>();
            utilityDefA.activationState = new SerializableEntityStateType(typeof(CastSmokescreenNoDelay));
            utilityDefA.baseRechargeInterval = cloakCooldown;
            utilityDefA.skillName = "CloakBanditReloaded";
            utilityDefA.skillNameToken = "BANDITRELOADED_UTILITY_NAME";
            utilityDefA.skillDescriptionToken = "<style=cIsDamage>Stunning</style>. Deal <style=cIsDamage>" + CastSmokescreenNoDelay.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>, become"
                   + " <style=cIsUtility>invisible</style>, then deal <style=cIsDamage>" + CastSmokescreenNoDelay.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> again.";
            utilityDefA.skillDescriptionToken += " Your next attack gains <style=cIsDamage>+50% TOTAL damage</style>.";
            utilityDefA.skillDescriptionToken += Environment.NewLine;
            utilityDefA.baseMaxStock = cloakStock;
            utilityDefA.rechargeStock = 1;
            utilityDefA.beginSkillCooldownOnSkillEnd = false;
            utilityDefA.activationStateMachineName = "Weapon";
            utilityDefA.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            utilityDefA.isCombatSkill = false;
            utilityDefA.cancelSprintingOnActivation = false;
            utilityDefA.canceledFromSprinting = false;
            utilityDefA.mustKeyPress = false;
            utilityDefA.icon = Resources.Load<Sprite>(assetPrefix + ":skill3.png");
            utilityDefA.requiredStock = 1;
            utilityDefA.stockToConsume = 1;
            utilityDefA.forceSprintDuringState = false;
            utilityDefA.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            ModContentPack.skillDefs.Add(utilityDefA);
            #endregion

            #region Assassinate
            utilityAltDef = SkillDef.CreateInstance<SkillDef>();
            utilityAltDef.activationState = new SerializableEntityStateType(typeof(Assassinate));

            utilityAltDef.baseRechargeInterval = asCooldown;
            utilityAltDef.skillName = "Assassinate";
            utilityAltDef.skillNameToken = "Assassinate";
            utilityAltDef.skillDescriptionToken = "Charge up your gun and fire a high caliber shot that pierces enemies for <style=cIsDamage>" + FireChargeShot.minDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + "-" + FireChargeShot.maxDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>";
            utilityAltDef.skillDescriptionToken += ".";
            if (FireChargeShot.selfForceMax != 0 && FireChargeShot.selfForceMin != 0)
            {
                utilityAltDef.skillDescriptionToken += " <style=cIsUtility>Pushes you backwards</style> if you are airborn.";
            }
            utilityAltDef.skillDescriptionToken += Environment.NewLine;
            utilityAltDef.baseMaxStock = asStock;
            utilityAltDef.rechargeStock = 1;
            utilityAltDef.beginSkillCooldownOnSkillEnd = true;
            utilityAltDef.activationStateMachineName = "Weapon";
            utilityAltDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;//should be .Skill if utility
            utilityAltDef.isCombatSkill = true;
            utilityAltDef.cancelSprintingOnActivation = true;
            utilityAltDef.canceledFromSprinting = false;
            utilityAltDef.mustKeyPress = false;
            utilityAltDef.icon = Resources.Load<Sprite>(assetPrefix + ":skill3a.png");
            utilityAltDef.requiredStock = 1;
            utilityAltDef.stockToConsume = 1;
            ModContentPack.skillDefs.Add(utilityAltDef);
            #endregion

            #region LightsOut
            specialLightsOutDef = SkillDef.CreateInstance<SkillDef>();
            specialLightsOutDef.activationState = new SerializableEntityStateType(typeof(PrepLightsOut));

            specialLightsOutDef.skillDescriptionToken = "";
            List<String> kwlLO = new List<String>();
            if (TakeDamage.specialExecuteThreshold > 0f)
            {
                kwlLO.Add("KEYWORD_BANDITRELOADED_EXECUTE");
                specialLightsOutDef.skillDescriptionToken += "<style=cIsDamage>Executing</style>. ";
            }
            if (TakeDamage.specialDebuffBonus > 0f)
            {
                kwlLO.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST");
                specialLightsOutDef.skillDescriptionToken += "<style=cIsDamage>Debuff Boosted</style>. ";
            }
            specialLightsOutDef.keywordTokens = kwlLO.ToArray();

            specialLightsOutDef.skillDescriptionToken += "Fire a persuader shot for <style=cIsDamage>" + FireLightsOut.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialLightsOutDef.skillDescriptionToken += " Kills <style=cIsUtility>reset all your cooldowns</style>.";
            specialLightsOutDef.skillDescriptionToken += Environment.NewLine;
            specialLightsOutDef.baseRechargeInterval = loCooldown;
            specialLightsOutDef.skillNameToken = "BANDITRELOADED_SPECIAL_NAME";
            specialLightsOutDef.skillName = "LightsOut";
            specialLightsOutDef.baseMaxStock = loStock;
            specialLightsOutDef.rechargeStock = 1;

            specialLightsOutDef.activationStateMachineName = "Weapon";
            specialLightsOutDef.icon = Resources.Load<Sprite>(assetPrefix + ":skill4.png");
            specialLightsOutDef.interruptPriority = EntityStates.InterruptPriority.Pain;
            specialLightsOutDef.beginSkillCooldownOnSkillEnd = true;
            specialLightsOutDef.isCombatSkill = true;
            specialLightsOutDef.canceledFromSprinting = false;
            specialLightsOutDef.cancelSprintingOnActivation = true;
            specialLightsOutDef.mustKeyPress = false;
            specialLightsOutDef.requiredStock = 1;
            specialLightsOutDef.stockToConsume = 1;
            ModContentPack.skillDefs.Add(specialLightsOutDef);
            #endregion

            #region Thermite Bomb
            thermiteDef = SkillDef.CreateInstance<SkillDef>();
            thermiteDef.activationState = new SerializableEntityStateType(typeof(ThermiteBomb));
            thermiteDef.baseRechargeInterval = thermiteCooldown;
            thermiteDef.skillNameToken = "BANDITRELOADED_SECONDARY_ALT_NAME";
            thermiteDef.skillDescriptionToken = "Fire a flare that coats enemies in <color=#cd7bd7>Thermite</color>, dealing <style=cIsDamage>" + ThermiteBomb.burnDamageMult.ToString("P0").Replace(" ", "").Replace(",", "") + " damage per second</style>."
                + " Explodes for <style=cIsDamage>" + ThermiteBomb.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>. New flares <style=cIsUtility>reset the burn timer</style>.";
            thermiteDef.skillDescriptionToken += Environment.NewLine;
            thermiteDef.skillName = "Thermite";
            thermiteDef.icon = Resources.Load<Sprite>(assetPrefix + ":skill2.png");
            thermiteDef.baseMaxStock = thermiteStock;
            thermiteDef.rechargeStock = 1;
            thermiteDef.beginSkillCooldownOnSkillEnd = false;
            thermiteDef.activationStateMachineName = "Weapon";
            thermiteDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            thermiteDef.isCombatSkill = true;
            thermiteDef.cancelSprintingOnActivation = false;
            thermiteDef.canceledFromSprinting = false;
            thermiteDef.mustKeyPress = false;
            thermiteDef.requiredStock = 1;
            thermiteDef.stockToConsume = 1;
            thermiteDef.keywordTokens = new string[] { "KEYWORD_BANDITRELOADED_THERMITE" };
            ModContentPack.skillDefs.Add(thermiteDef);
            #endregion

            #region Acid Bomb

            acidBombDef = SkillDef.CreateInstance<SkillDef>();
            acidBombDef.activationState = new SerializableEntityStateType(typeof(AcidBomb));
            acidBombDef.baseRechargeInterval = acidCooldown;
            acidBombDef.skillNameToken = "BANDITRELOADED_SECONDARY_ALT2_NAME";
            acidBombDef.skillDescriptionToken = "Toss a grenade that <style=cIsHealing>Weakens</style> for <style=cIsDamage>" + (AcidBomb.damageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>."
                + " Leaves acid that deals <style=cIsDamage>"+AcidBomb.acidDamageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "" )+ " damage per second</style>.";
            acidBombDef.skillDescriptionToken += Environment.NewLine;
            acidBombDef.skillName = "AcidGrenade";
            acidBombDef.icon = Resources.Load<Sprite>(assetPrefix + ":skill2a.png");
            acidBombDef.baseMaxStock = acidStock;
            acidBombDef.rechargeStock = 1;
            acidBombDef.beginSkillCooldownOnSkillEnd = false;
            acidBombDef.activationStateMachineName = "Weapon";
            acidBombDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            acidBombDef.isCombatSkill = true;
            acidBombDef.cancelSprintingOnActivation = false;
            acidBombDef.canceledFromSprinting = false;
            acidBombDef.mustKeyPress = false;
            acidBombDef.requiredStock = 1;
            acidBombDef.stockToConsume = 1;
            acidBombDef.keywordTokens = new string[] { "KEYWORD_WEAK" };
            ModContentPack.skillDefs.Add(acidBombDef);
            #endregion
            
            #region Cluster Bomb
            clusterBombDef = SkillDef.CreateInstance<SkillDef>();
            clusterBombDef.activationState = new SerializableEntityStateType(typeof(ClusterBomb));
            clusterBombDef.baseRechargeInterval = cbCooldown;
            clusterBombDef.skillNameToken = "BANDITRELOADED_SECONDARY_NAME";
            clusterBombDef.skillDescriptionToken = "Toss a bomb that <style=cIsDamage>ignites</style> for <style=cIsDamage>" + (ClusterBomb.damageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>."
                +" Drops bomblets for <style=cIsDamage>" + cbBombletCount + "x" + (ClusterBomb.bombletDamageCoefficient).ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>."
                + " Can be shot midair for <style=cIsDamage>bonus damage</style>.";
            clusterBombDef.skillDescriptionToken += Environment.NewLine;
            clusterBombDef.skillName = "Dynamite";
            clusterBombDef.icon = Resources.Load<Sprite>(assetPrefix + ":dynamite_red.png");
            clusterBombDef.baseMaxStock = cbStock;
            clusterBombDef.rechargeStock = 1;
            clusterBombDef.beginSkillCooldownOnSkillEnd = false;
            clusterBombDef.activationStateMachineName = "Weapon";
            clusterBombDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            clusterBombDef.isCombatSkill = true;
            clusterBombDef.cancelSprintingOnActivation = false;
            clusterBombDef.canceledFromSprinting = false;
            clusterBombDef.mustKeyPress = false;
            clusterBombDef.requiredStock = 1;
            clusterBombDef.stockToConsume = 1;
            clusterBombDef.keywordTokens = new string[] {};
            ModContentPack.skillDefs.Add(clusterBombDef);
            #endregion
            
            #region barrage
            specialBarrageDef = SkillDef.CreateInstance<SkillDef>();
            specialBarrageDef.activationState = new SerializableEntityStateType(typeof(PrepBarrage));
            specialBarrageDef.skillDescriptionToken = "";
            List<string> kwlBarrage = new List<string>();
            if (TakeDamage.specialExecuteThreshold > 0f)
            {
                kwlBarrage.Add("KEYWORD_BANDITRELOADED_EXECUTE");
                specialBarrageDef.skillDescriptionToken += "<style=cIsDamage>Executing</style>. ";
            }
            if (TakeDamage.specialDebuffBonus > 0f)
            {
                kwlBarrage.Add("KEYWORD_BANDITRELOADED_DEBUFFBOOST");
                specialBarrageDef.skillDescriptionToken += "<style=cIsDamage>Debuff Boosted</style>. ";
            }
            specialBarrageDef.keywordTokens = kwlBarrage.ToArray();

            float barrageBonusDamage = FireBarrage.damageCoefficient * TakeDamage.specialDebuffBonus;
            specialBarrageDef.skillDescriptionToken += "Rapidly fire shots for <style=cIsDamage>" + FireBarrage.maxBullets + "x" + FireBarrage.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialBarrageDef.skillDescriptionToken += " Repeated hits deal <style=cIsDamage>+" + barrageBonusDamage.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> each.";
            specialBarrageDef.skillDescriptionToken += " Kills <style=cIsUtility>reset all your cooldowns</style>.";
            specialBarrageDef.skillDescriptionToken += Environment.NewLine;
            specialBarrageDef.baseRechargeInterval = reuCooldown;
            specialBarrageDef.skillNameToken = "BANDITRELOADED_SPECIAL_ALT_NAME";
            specialBarrageDef.skillName = "BanditBarrage";
            specialBarrageDef.baseMaxStock = reuStock;
            specialBarrageDef.rechargeStock = 1;
            specialBarrageDef.activationStateMachineName = "Weapon";
            specialBarrageDef.icon = Resources.Load<Sprite>(assetPrefix + ":skill3a.png");
            specialBarrageDef.interruptPriority = EntityStates.InterruptPriority.Pain;
            specialBarrageDef.beginSkillCooldownOnSkillEnd = true;
            specialBarrageDef.isCombatSkill = true;
            specialBarrageDef.canceledFromSprinting = false;
            specialBarrageDef.cancelSprintingOnActivation = true;
            specialBarrageDef.mustKeyPress = false;
            specialBarrageDef.requiredStock = 1;
            specialBarrageDef.stockToConsume = 1;
            ModContentPack.skillDefs.Add(specialBarrageDef);

            specialBarrageScepterDef = SkillDef.CreateInstance<SkillDef>();
            specialBarrageScepterDef.activationState = new SerializableEntityStateType(typeof(PrepBarrageScepter));

            specialBarrageScepterDef.skillDescriptionToken = "";
            if (TakeDamage.specialExecuteThreshold > 0f)
            {
                specialBarrageScepterDef.skillDescriptionToken += "<style=cIsDamage>Executing</style>. ";
            }
            if (TakeDamage.specialDebuffBonus > 0f)
            {
                specialBarrageScepterDef.skillDescriptionToken += "<style=cIsDamage>Debuff Boosted</style>. ";
            }
            specialBarrageDef.keywordTokens = kwlBarrage.ToArray();
            float barrageScepterBonusDamage = FireBarrageScepter.damageCoefficient * TakeDamage.specialDebuffBonus;
            specialBarrageScepterDef.skillDescriptionToken += "Rapidly fire shots for <style=cIsDamage>" + FireBarrageScepter.maxBullets + "x" + FireBarrageScepter.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialBarrageScepterDef.skillDescriptionToken += " Repeated hits deal <style=cIsDamage>+" + barrageScepterBonusDamage.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style> each.";
            specialBarrageScepterDef.skillDescriptionToken += " Kills <style=cIsUtility>reset all your cooldowns</style>.";
            specialBarrageScepterDef.skillDescriptionToken += Environment.NewLine;
            specialBarrageScepterDef.baseRechargeInterval = reuCooldown;
            specialBarrageScepterDef.skillNameToken = "BANDITRELOADED_SPECIAL_ALT_SCEPTER_NAME";
            specialBarrageScepterDef.skillName = "BanditBarrageScepter";
            specialBarrageScepterDef.baseMaxStock = reuStock;
            specialBarrageScepterDef.rechargeStock = 1;
            specialBarrageScepterDef.activationStateMachineName = "Weapon";
            specialBarrageScepterDef.icon = Resources.Load<Sprite>(assetPrefix + ":reu_scepter.png");
            specialBarrageScepterDef.interruptPriority = EntityStates.InterruptPriority.Pain;
            specialBarrageScepterDef.beginSkillCooldownOnSkillEnd = true;
            specialBarrageScepterDef.isCombatSkill = true;
            specialBarrageScepterDef.canceledFromSprinting = false;
            specialBarrageScepterDef.cancelSprintingOnActivation = true;
            specialBarrageScepterDef.mustKeyPress = false;
            specialBarrageScepterDef.requiredStock = 1;
            specialBarrageScepterDef.stockToConsume = 1;
            ModContentPack.skillDefs.Add(specialBarrageScepterDef);

            specialLightsOutScepterDef = SkillDef.CreateInstance<SkillDef>();
            specialLightsOutScepterDef.activationState = new SerializableEntityStateType(typeof(PrepLightsOutScepter));

            specialLightsOutScepterDef.skillDescriptionToken = "";
            if (TakeDamage.specialExecuteThreshold > 0f)
            {
                specialLightsOutScepterDef.skillDescriptionToken += "<style=cIsDamage>Executing</style>. ";
            }
            if (TakeDamage.specialDebuffBonus > 0f)
            {
                specialLightsOutScepterDef.skillDescriptionToken += "<style=cIsDamage>Debuff Boosted</style>. ";
            }
            specialLightsOutScepterDef.keywordTokens = kwlBarrage.ToArray();

            specialLightsOutScepterDef.skillDescriptionToken += "Fire a persuader shot for <style=cIsDamage>" + FireLightsOutScepter.damageCoefficient.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            specialLightsOutScepterDef.skillDescriptionToken += " Kills <style=cIsUtility>reset all your cooldowns</style>.";

            specialLightsOutScepterDef.skillDescriptionToken += Environment.NewLine;
            specialLightsOutScepterDef.baseRechargeInterval = loCooldown;
            specialLightsOutScepterDef.skillNameToken = "BANDITRELOADED_SPECIAL_SCEPTER_NAME";
            specialLightsOutScepterDef.skillName = "LightsOutScepter";
            specialLightsOutScepterDef.baseMaxStock = loStock;
            specialLightsOutScepterDef.rechargeStock = 1;
            specialLightsOutScepterDef.activationStateMachineName = "Weapon";
            specialLightsOutScepterDef.icon = Resources.Load<Sprite>(assetPrefix + ":lo_scepter.png");
            specialLightsOutScepterDef.interruptPriority = EntityStates.InterruptPriority.Pain;
            specialLightsOutScepterDef.beginSkillCooldownOnSkillEnd = true;
            specialLightsOutScepterDef.isCombatSkill = true;
            specialLightsOutScepterDef.canceledFromSprinting = false;
            specialLightsOutScepterDef.cancelSprintingOnActivation = true;
            specialLightsOutScepterDef.mustKeyPress = false;
            specialLightsOutScepterDef.requiredStock = 1;
            specialLightsOutScepterDef.stockToConsume = 1;
            ModContentPack.skillDefs.Add(specialLightsOutScepterDef);
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

            utilitySkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = utilityDefA,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(utilityDefA.skillNameToken, false)
            };
            if (asEnabled)
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

            ModContentPack.skillFamilies.Add(primarySkillFamily);
            ModContentPack.skillFamilies.Add(secondarySkillFamily);
            ModContentPack.skillFamilies.Add(utilitySkillFamily);
            ModContentPack.skillFamilies.Add(specialSkillFamily);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter"))
            {
               SetupScepter();
            }

            ModContentPack.entityStates.Add(typeof(Blast));
            ModContentPack.entityStates.Add(typeof(CastSmokescreenNoDelay));
            ModContentPack.entityStates.Add(typeof(Assassinate));
            ModContentPack.entityStates.Add(typeof(FireChargeShot));
            ModContentPack.entityStates.Add(typeof(PrepLightsOut));
            ModContentPack.entityStates.Add(typeof(FireLightsOut));
            ModContentPack.entityStates.Add(typeof(AcidBomb));
            ModContentPack.entityStates.Add(typeof(ThermiteBomb));
            ModContentPack.entityStates.Add(typeof(Scatter));
            ModContentPack.entityStates.Add(typeof(ClusterBomb));
            ModContentPack.entityStates.Add(typeof(PrepBarrage));
            ModContentPack.entityStates.Add(typeof(FireBarrage));
            ModContentPack.entityStates.Add(typeof(FireBarrageScepter));
            ModContentPack.entityStates.Add(typeof(FireLightsOutScepter));
            ModContentPack.entityStates.Add(typeof(PrepBarrageScepter));
            ModContentPack.entityStates.Add(typeof(PrepLightsOutScepter));
            ModContentPack.entityStates.Add(typeof(ExitRevolver));
        }

        private void SetupAcidBomb()
        {
            AcidBombObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/banditgrenadeprojectile"), "BanditReloadedAcidBomb", true);
            AcidBombGhostObject = PrefabAPI.InstantiateClone(AcidBombObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedAcidBombGhost", false);
            ModContentPack.projectilePrefabs.Add(AcidBombObject);
            AcidBombObject.GetComponent<ProjectileController>().ghostPrefab = AcidBombGhostObject;

            GameObject puddleObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/crocoleapacid"), "BanditReloadedAcidBombPuddle", true);
            ModContentPack.projectilePrefabs.Add(puddleObject);
            ProjectileDamage puddleDamage = puddleObject.GetComponent<ProjectileDamage>();
            puddleDamage.damageType = DamageType.WeakOnHit;
            ProjectileDotZone pdz = puddleObject.GetComponent<ProjectileDotZone>();
            pdz.attackerFiltering = AttackerFiltering.Default;
            pdz.overlapProcCoefficient = acidProcCoefficient;
            pdz.lifetime = 5f;
            pdz.damageCoefficient = AcidBomb.acidDamageCoefficient / AcidBomb.damageCoefficient;

            GameObject abImpact = Resources.Load<GameObject>("prefabs/effects/impacteffects/engimineexplosion").InstantiateClone("BanditReloadedAcidEffect", false);
            EffectComponent ec = abImpact.GetComponent<EffectComponent>();
            //ec.applyScale = true;
            //ec.disregardZScale = false;
            ec.soundName = "Play_acrid_shift_land";
            ModContentPack.effectDefs.Add(new EffectDef(abImpact));

            AcidBombObject.GetComponent<ProjectileSimple>().velocity = 60f;
            ProjectileImpactExplosion abPIE = AcidBombObject.GetComponent<ProjectileImpactExplosion>();
            abPIE.blastRadius = acidRadius;
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

            AcidBomb.projectilePrefab = AcidBombObject;
        }

        private void SetupThermite()
        {
            ThermiteObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/thermite"), "BanditReloadedThermite", true);
            ThermiteGhostObject = PrefabAPI.InstantiateClone(ThermiteObject.GetComponent<ProjectileController>().ghostPrefab, "BanditReloadedThermiteGhost", false);
            ModContentPack.projectilePrefabs.Add(ThermiteObject);
            ThermiteObject.GetComponent<ProjectileController>().ghostPrefab = ThermiteGhostObject;

            ProjectileImpactExplosion tPIE = ThermiteObject.GetComponent<ProjectileImpactExplosion>();
            tPIE.blastRadius = thermiteRadius/2f;
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
            bootlegPOA.damageCoefficient = 0.5f * ThermiteBomb.burnDamageMult / ThermiteBomb.damageCoefficient;
            bootlegPOA.procCoefficient = thermiteProcCoefficient;
            bootlegPOA.damageInterval = 0.5f;
            bootlegPOA.lifetimeAfterImpact = thermiteBurnDuration;

            ThermiteBomb.projectilePrefab = ThermiteObject;

            GameObject thermiteBurnEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/missileexplosionvfx").InstantiateClone("BanditReloadedThermiteBurnEffect", false);
            thermiteBurnEffect.GetComponent<EffectComponent>().soundName = "Play_BanditReloaded_burn";
            ModContentPack.effectDefs.Add(new EffectDef(thermiteBurnEffect));
            BootlegThermiteOverlapAttack.burnEffectPrefab = thermiteBurnEffect;
            ThermiteBomb.projectilePrefab = ThermiteObject;
        }

        private void SetupClusterBomb()
        {
            ClusterBombObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/BanditClusterBombSeed"), "BanditReloadedClusterBomb", true);
            ModContentPack.projectilePrefabs.Add(ClusterBombObject);

            ClusterBombGhostObject = Resources.Load<GameObject>(assetPrefix + ":DynamiteBundle.prefab").InstantiateClone("BanditReloadedClusterBombGhost", true);
            ClusterBombGhostObject.GetComponentInChildren<MeshRenderer>().material.shader = hotpoo;
            ClusterBombGhostObject.AddComponent<ProjectileGhostController>();

            ClusterBombObject.AddComponent<DynamiteRotation>();

            ClusterBombObject.GetComponent<ProjectileController>().ghostPrefab = ClusterBombGhostObject;


            float trueBombletDamage = ClusterBomb.bombletDamageCoefficient / ClusterBomb.damageCoefficient;
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
            pie.blastRadius = cbRadius;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.lifetime = 25f;
            pie.lifetimeAfterImpact = 1.5f;
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = false;
            pie.childrenCount = cbBombletCount;
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

            ClusterBomb.projectilePrefab = ClusterBombObject;
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

            ModContentPack.effectDefs.Add(new EffectDef(dynamiteExplosion));
            return dynamiteExplosion;
        }

        private void SetupClusterBomblet()
        {
            ClusterBombletObject = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/BanditClusterGrenadeProjectile"), "BanditReloadedClusterBomblet", true);
            ModContentPack.projectilePrefabs.Add(ClusterBombletObject);

            ClusterBombletGhostObject = Resources.Load<GameObject>(assetPrefix + ":DynamiteStick.prefab").InstantiateClone("BanditReloadedClusterBombletGhost", true);
            ClusterBombletGhostObject.GetComponentInChildren<MeshRenderer>().material.shader = hotpoo;
            ClusterBombletGhostObject.AddComponent<ProjectileGhostController>();

            ClusterBombObject.GetComponent<ProjectileImpactExplosion>().childrenProjectilePrefab = ClusterBombletObject;

            ClusterBombletObject.AddComponent<SphereCollider>();
            ClusterBombletObject.GetComponent<ProjectileController>().ghostPrefab = ClusterBombletGhostObject;

            ProjectileImpactExplosion pie = ClusterBombletObject.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = cbBombletRadius;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.destroyOnEnemy = false;
            pie.destroyOnWorld = false;
            pie.lifetime = 1.5f;
            pie.timerAfterImpact = false;
            pie.blastProcCoefficient = cbBombletProcCoefficient;
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

            ModContentPack.effectDefs.Add(new EffectDef(dynamiteExplosion));
            return dynamiteExplosion;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void SetupScepter()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(specialLightsOutScepterDef, BanditBodyName, SkillSlot.Special, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(specialBarrageScepterDef, BanditBodyName, SkillSlot.Special, 1);
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

        private void SetAttributes()
        {
            BanditBody.tag = "Player";
            CharacterBody cb = BanditBody.GetComponent<CharacterBody>();
            cb.subtitleNameToken = "BANDITRELOADED_BODY_SUBTITLE";
            cb.baseNameToken = "BANDITRELOADED_BODY_NAME";
            cb.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
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
            if (useAltCrosshair)
            {
                cb.crosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/banditcrosshair");
            }
            BanditBody.AddComponent<BanditCrosshairComponent>();
            BanditBody.AddComponent<BanditNetworkCommands>();
        }

        private void CreateMaster()
        {
            BanditMonsterMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/charactermasters/commandomonstermaster"), "BanditReloadedMonsterMaster", true);
            ModContentPack.masterPrefabs.Add(BanditMonsterMaster);

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
            BanditBody = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/bandit2body"), "BanditReloadedBody", true);
            BanditBodyName = BanditBody.name;

            ModContentPack.bodyPrefabs.Add(BanditBody);
        }

        private void CreateLightsOutBuff()
        {
            BuffDef LightsOutBuffDef = BuffDef.CreateInstance<BuffDef>();
            LightsOutBuffDef.buffColor = BanditColor;
            LightsOutBuffDef.canStack = false;
            LightsOutBuffDef.isDebuff = true;
            LightsOutBuffDef.iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffFullCritIcon");
            LightsOutBuffDef.name = "BanditReloadedMarkedForDeath";
            ModContentPack.buffDefs.Add(LightsOutBuffDef);
            ModContentPack.lightsOutBuff = LightsOutBuffDef;

            BuffDef ThermiteBuffDef = BuffDef.CreateInstance<BuffDef>();
            ThermiteBuffDef.buffColor = BanditColor;
            ThermiteBuffDef.canStack = true;
            ThermiteBuffDef.iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffOnFireIcon");
            ThermiteBuffDef.isDebuff = true;
            ThermiteBuffDef.name = "BanditReloadedThermite";
            ModContentPack.buffDefs.Add(ThermiteBuffDef);
            ModContentPack.thermiteBuff = ThermiteBuffDef;

            BuffDef cloakDamageBuffDef = BuffDef.CreateInstance<BuffDef>();
            cloakDamageBuffDef.buffColor = BanditColor;
            cloakDamageBuffDef.canStack = false;
            cloakDamageBuffDef.iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffFullCritIcon");
            cloakDamageBuffDef.name = "BanditReloadedCloakDamage";
            cloakDamageBuffDef.isDebuff = false;
            ModContentPack.buffDefs.Add(cloakDamageBuffDef);
            ModContentPack.cloakDamageBuff = cloakDamageBuffDef;

            BuffDef skullBuffDef = BuffDef.CreateInstance<BuffDef>();
            skullBuffDef.buffColor = BanditColor;
            skullBuffDef.canStack = true;
            skullBuffDef.iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffBanditSkullIcon");
            skullBuffDef.isDebuff = true;
            skullBuffDef.name = "BanditReloadedSkull";
            ModContentPack.buffDefs.Add(skullBuffDef);
            ModContentPack.skullBuff = skullBuffDef;

            BuffDef cloakSpeedBuffDef = BuffDef.CreateInstance<BuffDef>();
            cloakSpeedBuffDef.buffColor = new Color(0.376f, 0.843f, 0.898f);
            cloakSpeedBuffDef.canStack = false;
            cloakSpeedBuffDef.iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texMovespeedBuffIcon");
            cloakSpeedBuffDef.isDebuff = false;
            cloakSpeedBuffDef.name = "BanditReloadedCloakSpeed";
            ModContentPack.buffDefs.Add(cloakSpeedBuffDef);
            ModContentPack.cloakSpeedBuff = cloakSpeedBuffDef;
        }
    }
}

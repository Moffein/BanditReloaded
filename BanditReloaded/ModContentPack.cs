using RoR2.Skills;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;

namespace BanditReloaded
{
    public class ModContentPack
    {
        internal static ContentPack contentPack;

        public static List<GameObject> bodyPrefabs = new List<GameObject>();
        public static List<BuffDef> buffDefs = new List<BuffDef>();
        public static List<EffectDef> effectDefs= new List<EffectDef>();
        public static List<Type> entityStates = new List<Type>();
        public static List<GameObject> masterPrefabs = new List<GameObject>();
        public static List<GameObject> projectilePrefabs = new List<GameObject>();
        public static List<SkillDef> skillDefs = new List<SkillDef>();
        public static List<SkillFamily> skillFamilies = new List<SkillFamily>();
        public static List<SurvivorDef> survivorDefs = new List<SurvivorDef>();
        public static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();


        public static BuffDef lightsOutBuff;
        public static BuffDef thermiteBuff;
        public static BuffDef cloakDamageBuff;
        public static BuffDef skullBuff;

        public static AssetBundle assets;

        public static UnlockableDef masteryUnlock;

        public static SurvivorDef banditReloadedSurvivor;

        public static void CreateContentPack()
        {
            contentPack = new ContentPack()
            {
                bodyPrefabs = bodyPrefabs.ToArray(),
                buffDefs = buffDefs.ToArray(),
                effectDefs = effectDefs.ToArray(),
                entityStateTypes = entityStates.ToArray(),
                masterPrefabs = masterPrefabs.ToArray(),
                projectilePrefabs = projectilePrefabs.ToArray(),
                skillDefs = skillDefs.ToArray(),
                skillFamilies = skillFamilies.ToArray(),
                survivorDefs = survivorDefs.ToArray(),
                unlockableDefs = unlockableDefs.ToArray()
            };

            On.RoR2.ContentManager.SetContentPacks += AddContent;
        }

        private static void AddContent(On.RoR2.ContentManager.orig_SetContentPacks orig, List<ContentPack> newContentPacks)
        {
            newContentPacks.Add(contentPack);
            orig(newContentPacks);
        }

        public static void LoadResources()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BanditReloaded.banditbundle"))
            {
                assets = AssetBundle.LoadFromStream(stream);
            }

            using (var bankStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BanditReloaded.BanditReloaded.bnk"))
            {
                var bytes = new byte[bankStream.Length];
                bankStream.Read(bytes, 0, bytes.Length);
                EnigmaticThunder.Modules.Sounds.SoundBanks.Add(bytes);
            }
        }
    }
}

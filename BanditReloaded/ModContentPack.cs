using RoR2.Skills;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;
using RoR2.ContentManagement;
using System.Collections;

namespace BanditReloaded
{
    public class ModContentPack : IContentPackProvider
    {
        internal static ContentPack contentPack = new ContentPack();

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

        public string identifier => "BanditReloaded.content";

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
                R2API.SoundAPI.SoundBanks.Add(bytes);
            }
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            contentPack.bodyPrefabs.Add(bodyPrefabs.ToArray());
            contentPack.buffDefs.Add(buffDefs.ToArray());
            contentPack.effectDefs.Add(effectDefs.ToArray());
            contentPack.entityStateTypes.Add(entityStates.ToArray());
            contentPack.masterPrefabs.Add(masterPrefabs.ToArray());
            contentPack.projectilePrefabs.Add(projectilePrefabs.ToArray());
            contentPack.skillDefs.Add(skillDefs.ToArray());
            contentPack.skillFamilies.Add(skillFamilies.ToArray());
            contentPack.survivorDefs.Add(survivorDefs.ToArray());
            contentPack.unlockableDefs.Add(unlockableDefs.ToArray());
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}

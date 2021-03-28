using RoR2.Skills;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
            };

            On.RoR2.ContentManager.SetContentPacks += AddContent;
        }

        private static void AddContent(On.RoR2.ContentManager.orig_SetContentPacks orig, List<ContentPack> newContentPacks)
        {
            newContentPacks.Add(contentPack);
            orig(newContentPacks);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BanditReloaded.Hooks
{
    public class RecalculateStats
    {
        public static void AddHook()
        {
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                if (self.HasBuff(ModContentPack.thermiteBuff))
                {
                    int tCount = self.GetBuffCount(ModContentPack.thermiteBuff);
                    self.moveSpeed *= Mathf.Pow(0.85f, tCount);
                    self.armor -= 2.5f * tCount;
                }
                if (self.HasBuff(ModContentPack.cloakSpeedBuff))
                {
                    self.moveSpeed *= 1.5f;
                }
            };
        }
    }
}

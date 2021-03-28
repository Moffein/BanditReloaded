using System;
using System.Collections.Generic;
using System.Text;

namespace BanditReloaded.Hooks
{
    public class CloakDamage
    {
        public static void AddHook()
        {
            On.EntityStates.BaseState.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.HasBuff(ModContentPack.cloakDamageBuff))
                {
                    self.damageStat *= 1.5f;
                }
            };
        }
    }
}

using BanditReloaded;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EntityStates.BanditReloadedSkills
{
    public class BanditHelpers
    {
        public static void TriggerQuickdraw(SkillLocator skills)
        {
            skills.primary.stock = skills.primary.maxStock;
            skills.primary.rechargeStopwatch = 0f;
        }

        public static void ConsumeCloakDamageBuff(CharacterBody cb)
        {
            if (NetworkServer.active && cb)
            {
                cb.ClearTimedBuffs(ModContentPack.cloakDamageBuff);
            }
        }

        public static void PlayCloakDamageSound(CharacterBody cb)
        {
            if (cb && cb.HasBuff(ModContentPack.cloakDamageBuff))
            {
                Util.PlaySound("Play_BanditReloaded_cloakdamage", cb.gameObject);
            }
        }
    }
}

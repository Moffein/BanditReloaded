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
            if (enablePassive)
            {
                skills.primary.stock = skills.primary.maxStock;
                skills.primary.rechargeStopwatch = 0f;
            }
        }

        public static void ConsumeCloakDamageBuff(CharacterBody cb)
        {
            if (NetworkServer.active && cb)
            {
                cb.ClearTimedBuffs(BanditReloaded.BanditReloaded.cloakDamageBuff);
            }
        }

        public static void PlayCloakDamageSound(CharacterBody cb)
        {
            if (cb && cb.HasBuff(BanditReloaded.BanditReloaded.cloakDamageBuff))
            {
                Util.PlaySound("Play_BanditReloaded_cloakdamage", cb.gameObject);
            }
        }

        public static bool enablePassive;
    }
}

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
                if (skills.primary.rechargeStock < skills.primary.maxStock
                && skills.primary.stock + skills.primary.rechargeStock <= skills.primary.maxStock)
                {
                    skills.primary.stock += skills.primary.rechargeStock;
                    if (skills.primary.rechargeStopwatch < 0f)
                    {
                        skills.primary.rechargeStopwatch = 0f;
                    }
                }
                else
                {
                    skills.primary.rechargeStopwatch += skills.primary.CalculateFinalRechargeInterval();
                }
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
                Util.PlaySound("Play_BanditReloaded_reset", cb.gameObject);
            }
        }

        public static bool enablePassive;
    }
}

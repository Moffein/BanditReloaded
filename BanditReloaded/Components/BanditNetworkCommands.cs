using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;

namespace BanditReloaded.Components
{
    public class BanditNetworkCommands : NetworkBehaviour
    {
        [ClientRpc]
        public void RpcResetSpecialCooldown()
        {
            if (this.hasAuthority)
            {
                skillLocator.special.stock = skillLocator.special.maxStock;
                skillLocator.special.rechargeStopwatch = 0f;
            }
        }

        [ClientRpc]
        public void RpcPlayLOMid()
        {
            Util.PlaySound("Play_item_proc_armorReduction_hit", this.gameObject);
        }

        [ClientRpc]
        public void RpcPlayLOHigh()
        {
            Util.PlaySound("Play_item_proc_armorReduction_shatter", this.gameObject);
        }

        private void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            skillLocator = base.GetComponent<SkillLocator>();
        }

        private SkillLocator skillLocator;
        private CharacterBody characterBody;
    }
}

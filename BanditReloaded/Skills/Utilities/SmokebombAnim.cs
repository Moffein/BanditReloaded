using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.BanditReloadedSkills
{
    public class CastSmokescreen : BaseState
    {
        private void CastSmoke()
        {
            if (!this.hasCastSmoke)
            {
                Util.PlaySound(CastSmokescreen.startCloakSoundString, base.gameObject);
            }
            EffectManager.SpawnEffect(CastSmokescreen.smokescreenEffectPrefab, new EffectData
            {
                origin = base.transform.position
            }, false);
            int layerIndex = this.animator.GetLayerIndex("Impact");
            if (layerIndex >= 0)
            {
                this.animator.SetLayerWeight(layerIndex, 2f);
                this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
            }
            if (NetworkServer.active)
            {
                new BlastAttack
                {
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                    baseDamage = this.damageStat * CastSmokescreen.damageCoefficient,
                    baseForce = CastSmokescreen.forceMagnitude,
                    position = base.transform.position,
                    radius = CastSmokescreen.radius,
                    falloffModel = BlastAttack.FalloffModel.None,
                    damageType = DamageType.Stun1s,
                    crit = base.RollCrit(),
                    attackerFiltering = AttackerFiltering.NeverHit
                }.Fire();
            }
        }
        public override void OnEnter()
        {
            this.duration = CastSmokescreen.baseDuration / this.attackSpeedStat;
            this.totalDuration = CastSmokescreen.stealthDuration + this.totalDuration;
            base.PlayCrossfade("Gesture, Smokescreen", "CastSmokescreen", "CastSmokescreen.playbackRate", this.duration, 0.2f);
            this.animator = base.GetModelAnimator();
            Util.PlaySound(CastSmokescreen.jumpSoundString, base.gameObject);
            EffectManager.SpawnEffect(CastSmokescreen.initialEffectPrefab, new EffectData
            {
                origin = base.transform.position
            }, true);
            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    characterBody.AddBuff(BuffIndex.CloakSpeed);
                    base.characterBody.AddTimedBuff(BanditReloaded.BanditReloaded.cloakDamageBuff, CastSmokescreen.stealthDuration + 0.5f);
                }
                if (base.isAuthority)
                {
                    base.OnEnter();
                }
                BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
            }
        }

        public override void OnExit()
        {
            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    if (base.characterBody.HasBuff(BuffIndex.Cloak))
                    {
                        base.characterBody.RemoveBuff(BuffIndex.Cloak);
                    }
                    if (base.characterBody.HasBuff(BuffIndex.CloakSpeed))
                    {
                        base.characterBody.RemoveBuff(BuffIndex.CloakSpeed);
                    }
                    base.characterBody.AddTimedBuff(BanditReloaded.BanditReloaded.cloakDamageBuff, 0.5f);
                }
            }
            if (!this.outer.destroying)
            {
                this.CastSmoke();
            }
            Util.PlaySound(CastSmokescreen.stopCloakSoundString, base.gameObject);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && !this.hasCastSmoke)
            {
                this.CastSmoke();
                if (base.characterBody && NetworkServer.active)
                {
                    base.characterBody.AddBuff(BuffIndex.Cloak);
                }

                this.hasCastSmoke = true;
            }
            if (base.fixedAge >= this.totalDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (!this.hasCastSmoke)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Any;
        }

        public static float baseDuration = EntityStates.Commando.CommandoWeapon.CastSmokescreen.baseDuration;
        public static float stealthDuration = 3f;
        public static string jumpSoundString = "Play_bandit_shift_jump";
        public static string startCloakSoundString = "Play_bandit_shift_land";
        public static string stopCloakSoundString = "Play_bandit_shift_end";
        public static GameObject initialEffectPrefab = EntityStates.Commando.CommandoWeapon.CastSmokescreen.initialEffectPrefab;
        public static GameObject smokescreenEffectPrefab = EntityStates.Commando.CommandoWeapon.CastSmokescreen.smokescreenEffectPrefab;
        public static float damageCoefficient = 1.3f;
        public static float radius = 4f;
        public static float forceMagnitude = 0f;

        private float duration;
        private float totalDuration;
        private bool hasCastSmoke;
        private Animator animator;
    }
}

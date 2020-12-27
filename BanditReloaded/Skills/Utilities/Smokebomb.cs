using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BanditReloadedSkills
{
    public class CastSmokescreenNoDelay : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.CastSmoke();
            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    base.characterBody.AddBuff(BuffIndex.Cloak);
                    base.characterBody.AddBuff(BuffIndex.CloakSpeed);
                }
                BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
                if (NetworkServer.active)
                {
                    base.characterBody.AddTimedBuff(BanditReloaded.BanditReloaded.cloakDamageBuff, CastSmokescreenNoDelay.duration + 0.5f);
                }
            }
        }

        public override void OnExit()
        {
            if (base.characterBody && NetworkServer.active)
            {
                if (base.characterBody.HasBuff(BuffIndex.Cloak))
                {
                    base.characterBody.RemoveBuff(BuffIndex.Cloak);
                }
                if (base.characterBody.HasBuff(BuffIndex.CloakSpeed))
                {
                    base.characterBody.RemoveBuff(BuffIndex.CloakSpeed);
                }
            }
            if (!this.outer.destroying)
            {
                this.CastSmoke();
            }
            if (CastSmokescreenNoDelay.destealthMaterial)
            {
                TemporaryOverlay temporaryOverlay = this.animator.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1f;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = CastSmokescreenNoDelay.destealthMaterial;
                temporaryOverlay.inspectorCharacterModel = this.animator.gameObject.GetComponent<CharacterModel>();
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.animateShaderAlpha = true;
            }
            Util.PlaySound(CastSmokescreenNoDelay.stopCloakSoundString, base.gameObject);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= CastSmokescreenNoDelay.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void CastSmoke()
        {
            if (!this.hasCastSmoke)
            {
                Util.PlaySound(CastSmokescreenNoDelay.startCloakSoundString, base.gameObject);
                this.hasCastSmoke = true;
            }
            EffectManager.SpawnEffect(CastSmokescreenNoDelay.smokescreenEffectPrefab, new EffectData
            {
                origin = base.transform.position
            }, false);
            int layerIndex = this.animator.GetLayerIndex("Impact");
            if (layerIndex >= 0)
            {
                this.animator.SetLayerWeight(layerIndex, 1f);
                this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
            }

            new BlastAttack
            {
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                baseDamage = this.damageStat * CastSmokescreenNoDelay.damageCoefficient,
                baseForce = CastSmokescreenNoDelay.forceMagnitude,
                position = base.transform.position,
                radius = CastSmokescreenNoDelay.radius,
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = DamageType.Stun1s,
                crit = base.RollCrit(),
                attackerFiltering = AttackerFiltering.NeverHit
            }.Fire();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.stopwatch <= CastSmokescreenNoDelay.minimumStateDuration)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Any;
        }

        public static float duration;
        public static float minimumStateDuration;
        public static string startCloakSoundString = "Play_bandit_shift_land";
        public static string stopCloakSoundString = "Play_bandit_shift_end";
        public static GameObject smokescreenEffectPrefab = EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.smokescreenEffectPrefab;
        public static Material destealthMaterial = EntityStates.Commando.CommandoWeapon.CastSmokescreenNoDelay.destealthMaterial;
        public static float damageCoefficient;
        public static float radius;
        public static float forceMagnitude = 0f;

        private float stopwatch;
        private bool hasCastSmoke;
        private Animator animator;
    }
}

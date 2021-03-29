using System;
using System.Collections.Generic;
using System.Text;
using BanditReloaded;
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
                if (base.isAuthority)
                {
                    base.characterBody.isSprinting = true;
                }
                if (NetworkServer.active)
                {
                    base.characterBody.AddBuff(RoR2Content.Buffs.Cloak);
                    base.characterBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
                    base.characterBody.AddTimedBuff(ModContentPack.cloakDamageBuff, CastSmokescreenNoDelay.duration + 0.5f);
                }
                BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
            }

            if (base.characterMotor && !base.characterMotor.isGrounded)
            {
                base.PlayAnimation("Gesture, Additive", "ThrowSmokebomb", "ThrowSmokebomb.playbackRate", 0.2f);
            }
        }

        public override void OnExit()
        {
            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    if (base.characterBody.HasBuff(RoR2Content.Buffs.Cloak))
                    {
                        base.characterBody.RemoveBuff(RoR2Content.Buffs.Cloak);
                    }
                    if (base.characterBody.HasBuff(RoR2Content.Buffs.CloakSpeed))
                    {
                        base.characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
                    }
                }
                BanditHelpers.PlayCloakDamageSound(base.characterBody);
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
            if (this.animator)
            {
                this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, StealthWeapon"), 0f);
            }

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

            if (NetworkServer.active)
            {
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
                    damageType = CastSmokescreenNoDelay.nonLethal ? (DamageType.Stun1s | DamageType.NonLethal) : DamageType.Stun1s,
                    procCoefficient = CastSmokescreenNoDelay.procCoefficient,
                    crit = base.RollCrit(),
                    attackerFiltering = AttackerFiltering.NeverHit
                }.Fire();
            }

            if (base.isAuthority && base.characterMotor && !base.characterMotor.isGrounded)
            {
                base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, 17f, base.characterMotor.velocity.z);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.stopwatch <= CastSmokescreenNoDelay.minimumStateDuration)
            {
                return InterruptPriority.Pain;
            }
            return InterruptPriority.Any;
        }

        public static float duration;
        public static float minimumStateDuration;
        public static string startCloakSoundString = "Play_bandit_shift_land";
        public static string stopCloakSoundString = "Play_bandit_shift_end";
        public static GameObject smokescreenEffectPrefab = Resources.Load<GameObject>("prefabs/effects/smokescreeneffect");
        public static Material destealthMaterial;
        public static float damageCoefficient;
        public static float procCoefficient;
        public static bool nonLethal;
        public static float radius;
        public static float forceMagnitude = 0f;

        private float stopwatch;
        private bool hasCastSmoke;
        private Animator animator;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using BanditReloaded;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BanditReloadedSkills
{
    public class ThermiteBomb : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ThermiteBomb.baseDuration / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            Util.PlaySound("Play_Bandit_m2_fire", base.gameObject);

            this.animator = base.GetModelAnimator();
            if (!BanditReloaded.BanditReloaded.useOldModel)
            {
                if (this.animator)
                {
                    this.bodySideWeaponLayerIndex = this.animator.GetLayerIndex("Body, SideWeapon");
                    this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 1f);
                }
                base.PlayAnimation("Gesture, Additive", "MainToSide", "MainToSide.playbackRate", this.duration * 0.5f);
            }
            else
            {
                base.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", this.duration * 0.5f);
                base.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", this.duration * 0.5f);
            }

            if (ThermiteBomb.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(ThermiteBomb.effectPrefab, base.gameObject, "MuzzlePistol", false);
            }
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(ThermiteBomb.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * ThermiteBomb.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
            if (base.characterMotor && !base.characterMotor.isGrounded)
            {
                Vector3 vector = -aimRay.direction * ThermiteBomb.selfForce;
                vector.y *= 0.5f;
                base.characterMotor.ApplyForce(vector, true, false);
            }
            base.characterBody.AddSpreadBloom(2.4f);
            base.AddRecoil(-1f * 6f, -2f * 6f, -0.5f * 6f, 0.5f * 6f);
            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
        }
        public override void OnExit()
        {
            BanditHelpers.ConsumeCloakDamageBuff(base.characterBody);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!playedAnim && base.fixedAge > this.duration*0.5f)
            {
                playedAnim = true;
                if (!BanditReloaded.BanditReloaded.useOldModel)
                {
                    base.PlayAnimation("Gesture, Additive", "FireSideWeapon", "FireSideWeapon.playbackRate", this.duration);
                }
                else
                {
                    base.PlayAnimation("Gesture", "FireRevolver", "FireRevolver.playbackRate", this.duration);
                }
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                if (this.animator)
                {
                    this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 0f);
                }
                Transform transform = base.FindModelChild("SpinningPistolFX");
                if (transform)
                {
                    transform.gameObject.SetActive(false);
                }
                this.outer.SetNextState(new ExitRevolver());
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public static GameObject projectilePrefab;
        public static float damageCoefficient;
        public static float force = 0f;
        public static float selfForce = 0f;
        public static float baseDuration;
        public static float burnDamageMult;
        public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditpistol");
        private float duration;
        private bool playedAnim = false;
        private Animator animator;
        private int bodySideWeaponLayerIndex;
    }
}

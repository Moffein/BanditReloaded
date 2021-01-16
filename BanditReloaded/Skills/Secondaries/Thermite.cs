using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BanditReloadedSkills
{
    public class PrepFlare : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = PrepFlare.baseDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            Util.PlaySound(PrepFlare.prepSoundString, base.gameObject);
            this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
            base.characterBody.crosshairPrefab = PrepFlare.specialCrosshairPrefab;

            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);

            if (base.characterBody)
            {
                if (base.characterBody.HasBuff(BanditReloaded.BanditReloaded.cloakDamageBuff))
                {
                    base.characterBody.ClearTimedBuffs(BanditReloaded.BanditReloaded.cloakDamageBuff);
                    base.characterBody.AddTimedBuff(BanditReloaded.BanditReloaded.cloakDamageBuff, 1.2f);
                }
                base.characterBody.SetAimTimer(this.duration);
                if (base.characterBody.HasBuff(BanditReloaded.BanditReloaded.cloakDamageBuff))
                {
                    base.characterBody.ClearTimedBuffs(BanditReloaded.BanditReloaded.cloakDamageBuff);
                    base.characterBody.AddTimedBuff(BanditReloaded.BanditReloaded.cloakDamageBuff, 1.2f);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration);
            }
            if (base.fixedAge >= this.duration && base.isAuthority && (!base.inputBank || !base.inputBank.skill2.down))
            {
                this.outer.SetNextState(new ThermiteBomb());
                return;
            }
        }

        public override void OnExit()
        {
            base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
        public static float baseDuration = 0.4f;
        public static string prepSoundString = "Play_bandit_M2_load";
        private float duration;
        private ChildLocator childLocator;
        public static GameObject specialCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/banditcrosshairrevolver");
        private GameObject defaultCrosshairPrefab;
    }
    public class ThermiteBomb : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ThermiteBomb.baseDuration / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            //base.PlayAnimation("Gesture", "FireRevolver", "FireRevolver.playbackRate", this.duration);
            Util.PlaySound("Play_BanditReloaded_flare", base.gameObject);
            if (ThermiteBomb.effectPrefab)
            {
                /*base.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", this.duration * 2f);
                base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", this.duration * 2f);*/

                base.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", this.duration * 0.5f);
                base.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", this.duration * 0.5f);

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
                base.PlayAnimation("Gesture", "FireRevolver", "FireRevolver.playbackRate", this.duration);
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
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
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace EntityStates.BanditReloadedSkills
{
    public class PrepLightsOutScepter : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = PrepLightsOutScepter.baseDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            Util.PlaySound(PrepLightsOutScepter.prepSoundString, base.gameObject);
            this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
            base.characterBody.crosshairPrefab = PrepLightsOutScepter.specialCrosshairPrefab;

            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);

            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority && !inputBank.skill4.down)
            {
                this.outer.SetNextState(new FireLightsOutScepter());
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
            return InterruptPriority.Pain;
        }
        public static float baseDuration;
        public static string prepSoundString = "Play_bandit_M2_load";
        private float duration;
        private ChildLocator childLocator;
        public static GameObject specialCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/banditcrosshairrevolver");
        private GameObject defaultCrosshairPrefab;
    }
    public class FireLightsOutScepter : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = FireLightsOutScepter.baseDuration / this.attackSpeedStat;
            base.AddRecoil(-3f * FireLightsOutScepter.recoilAmplitude, -4f * FireLightsOutScepter.recoilAmplitude, -0.5f * FireLightsOutScepter.recoilAmplitude, 0.5f * FireLightsOutScepter.recoilAmplitude);
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            string muzzleName = "MuzzlePistol";
            Util.PlaySound(FireLightsOutScepter.attackSoundString, base.gameObject);

            base.PlayAnimation("Gesture, Additive", "FireRevolver");
            base.PlayAnimation("Gesture, Override", "FireRevolver");
            if (FireLightsOutScepter.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(FireLightsOutScepter.effectPrefab, base.gameObject, muzzleName, false);
            }
            if (base.isAuthority)
            {
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    force = FireLightsOutScepter.force,
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = FireLightsOutScepter.tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = FireLightsOutScepter.hitEffectPrefab,
                    isCrit = base.RollCrit(),
                    HitEffectNormal = true,
                    radius = 0.5f,
                    maxDistance = 2000f,
                    procCoefficient = 1f,
                    damage = FireLightsOutScepter.damageCoefficient * this.damageStat,
                    damageType = DamageType.ResetCooldownsOnKill | DamageType.ClayGoo,
                    smartCollision = true
                }.Fire();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditpistol");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/hitsparkbanditpistol");
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditpistol");
        public static float damageCoefficient;
        public static float force;
        public static float baseDuration;
        public static float gracePeriodMin;
        public static float gracePeriodMax;
        public static float executeThreshold;
        public static float buffDamageCoefficient;
        public static bool executeBosses;
        public static string attackSoundString = "Play_bandit_M2_shot";
        public static float recoilAmplitude = 4f;
        private ChildLocator childLocator;
        private float duration;
    }
}

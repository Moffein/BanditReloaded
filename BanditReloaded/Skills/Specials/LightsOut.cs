using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using BanditReloaded;

namespace EntityStates.BanditReloadedSkills
{
    public class PrepLightsOut : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = PrepLightsOut.baseDuration / this.attackSpeedStat;

            this.animator = base.GetModelAnimator();
            if (this.animator)
            {
                this.bodySideWeaponLayerIndex = this.animator.GetLayerIndex("Body, SideWeapon");
                this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 1f);
            }
            base.PlayAnimation("Gesture, Additive", "MainToSide", "MainToSide.playbackRate", this.duration);

            Util.PlaySound(PrepLightsOut.prepSoundString, base.gameObject);
            this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
            base.characterBody.crosshairPrefab = PrepLightsOut.specialCrosshairPrefab;

            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);

            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration);
                if (base.characterBody.HasBuff(ModContentPack.cloakDamageBuff))
                {
                    base.characterBody.ClearTimedBuffs(ModContentPack.cloakDamageBuff);
                    base.characterBody.AddTimedBuff(ModContentPack.cloakDamageBuff, 1.2f);
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
            if (base.fixedAge >= this.duration && base.isAuthority && !inputBank.skill4.down)
            {
                this.outer.SetNextState(new FireLightsOut());
                return;
            }
        }

        public override void OnExit()
        {
            base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;

            if (this.animator)
            {
                this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 0f);
            }
            Transform transform = base.FindModelChild("SpinningPistolFX");
            if (transform)
            {
                transform.gameObject.SetActive(false);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
        public static float baseDuration;
        public static string prepSoundString = "Play_bandit2_R_load";
        private float duration;
        private ChildLocator childLocator;
        public static GameObject specialCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/Bandit2CrosshairPrepRevolver");
        private GameObject defaultCrosshairPrefab;
        private Animator animator;
        private int bodySideWeaponLayerIndex;
    }
    public class FireLightsOut : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = FireLightsOut.baseDuration / this.attackSpeedStat;
            base.AddRecoil(-3f * FireLightsOut.recoilAmplitude, -4f * FireLightsOut.recoilAmplitude, -0.5f * FireLightsOut.recoilAmplitude, 0.5f * FireLightsOut.recoilAmplitude);
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            string muzzleName = "MuzzlePistol";
            Util.PlaySound(FireLightsOut.attackSoundString, base.gameObject);

            this.animator = base.GetModelAnimator();
            if (this.animator)
            {
                this.bodySideWeaponLayerIndex = this.animator.GetLayerIndex("Body, SideWeapon");
                this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 1f);
            }
            base.PlayAnimation("Gesture, Additive", "FireSideWeapon", "FireSideWeapon.playbackRate", 1f);

            if (FireLightsOut.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(FireLightsOut.effectPrefab, base.gameObject, muzzleName, false);
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
                    force = FireLightsOut.force,
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = FireLightsOut.tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = FireLightsOut.hitEffectPrefab,
                    isCrit = base.RollCrit(),
                    HitEffectNormal = true,
                    radius = 0.5f,
                    maxDistance = 2000f,
                    procCoefficient = 1f,
                    damage = FireLightsOut.damageCoefficient * this.damageStat,
                    damageType = DamageType.ResetCooldownsOnKill,
                    smartCollision = true
                }.Fire();
            }
        }

        public override void OnExit()
        {
            BanditHelpers.ConsumeCloakDamageBuff(base.characterBody);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
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
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditpistol");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/hitsparkbanditpistol");
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditpistol");
        public static float damageCoefficient;
        public static float force;
        public static float baseDuration;
        public static string attackSoundString = "Play_bandit2_R_fire";
        public static float recoilAmplitude = 4f;
        private ChildLocator childLocator;
        private float duration;
        private Animator animator;
        private int bodySideWeaponLayerIndex;
    }
}

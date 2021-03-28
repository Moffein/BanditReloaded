using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using BanditReloaded;

namespace EntityStates.BanditReloadedSkills
{
    public class PrepBarrageScepter : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = PrepBarrageScepter.baseDuration / this.attackSpeedStat;

            this.animator = base.GetModelAnimator();
            if (this.animator)
            {
                this.bodySideWeaponLayerIndex = this.animator.GetLayerIndex("Body, SideWeapon");
                this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 1f);
            }
            base.PlayAnimation("Gesture, Additive", "MainToSide", "MainToSide.playbackRate", this.duration);

            Util.PlaySound(PrepBarrageScepter.prepSoundString, base.gameObject);

            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration);
                BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);

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
                base.characterBody.SetSpreadBloom(FireBarrage.spread * 0.8f, false);
                base.characterBody.SetAimTimer(this.duration);
            }
            if (base.fixedAge >= this.duration && base.isAuthority && !inputBank.skill4.down)
            {
                this.outer.SetNextState(new FireBarrageScepter());
                return;
            }
        }

        public override void OnExit()
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
        private Animator animator;
        private int bodySideWeaponLayerIndex;
    }
    public class FireBarrageScepter : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            bulletCount = FireBarrageScepter.maxBullets;
            prevShot = 0f;
            this.duration = FireBarrageScepter.baseDuration / this.attackSpeedStat;
            this.recoil = FireBarrageScepter.recoilAmplitude / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            muzzleName = "MuzzlePistol";
            isCrit = base.RollCrit();

            this.animator = base.GetModelAnimator();
            if (this.animator)
            {
                this.bodySideWeaponLayerIndex = this.animator.GetLayerIndex("Body, SideWeapon");
                this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 1f);
            }
        }

        public override void OnExit()
        {
            BanditHelpers.ConsumeCloakDamageBuff(base.characterBody);
            base.characterBody.SetSpreadBloom(0f, false);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if ((base.fixedAge - this.prevShot) > this.duration)
            {
                if (bulletCount > 0)
                {
                    this.prevShot = base.fixedAge;
                    bulletCount--;
                    base.AddRecoil(-3f * this.recoil, -4f * this.recoil, -0.5f * this.recoil, 0.5f * this.recoil);
                    Ray aimRay = base.GetAimRay();
                    muzzleName = "MuzzlePistol";
                    Util.PlaySound(FireBarrageScepter.attackSoundString, base.gameObject);
                    base.PlayAnimation("Gesture, Additive", "FireSideWeapon", "FireSideWeapon.playbackRate", 0.5f);
                    if (FireBarrageScepter.effectPrefab)
                    {
                        EffectManager.SimpleMuzzleFlash(FireBarrageScepter.effectPrefab, base.gameObject, muzzleName, false);
                    }
                    float bulletSpread = bulletCount <= 0 ? 0f : FireBarrageScepter.spread;
                    if (base.isAuthority)
                    {
                        new BulletAttack
                        {
                            owner = base.gameObject,
                            weapon = base.gameObject,
                            origin = aimRay.origin,
                            aimVector = aimRay.direction,
                            minSpread = bulletSpread,
                            maxSpread = bulletSpread,
                            force = FireBarrageScepter.force,
                            falloffModel = BulletAttack.FalloffModel.None,
                            tracerEffectPrefab = FireBarrageScepter.tracerEffectPrefab,
                            muzzleName = muzzleName,
                            hitEffectPrefab = FireBarrageScepter.hitEffectPrefab,
                            isCrit = this.isCrit,
                            HitEffectNormal = true,
                            radius = 0.4f,
                            maxDistance = FireBarrageScepter.maxDistance,
                            procCoefficient = 1f,
                            damage = FireBarrageScepter.damageCoefficient * this.damageStat,
                            damageType = DamageType.ResetCooldownsOnKill | DamageType.SlowOnHit,
                            smartCollision = true
                        }.Fire();
                        base.characterBody.SetSpreadBloom(FireBarrage.spread * 0.8f, false);
                    }
                }
                else if (base.fixedAge - prevShot > endLag)
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
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");
        public static float damageCoefficient;
        public static float force;
        public static float baseDuration;
        public static string attackSoundString = "Play_bandit2_R_fire";
        public static float recoilAmplitude = 2.2f;
        public static int maxBullets;
        public static float endLag;
        public static float spread;
        public static float maxDistance;

        private int bulletCount;
        private ChildLocator childLocator;
        private float duration;
        private float prevShot;
        private string muzzleName;
        private bool isCrit;
        private float recoil;
        private Animator animator;
        private int bodySideWeaponLayerIndex;
    }
}

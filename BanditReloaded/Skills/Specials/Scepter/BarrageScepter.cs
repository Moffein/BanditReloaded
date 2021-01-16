using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace EntityStates.BanditReloadedSkills
{
    public class PrepBarrageScepter : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = PrepBarrageScepter.baseDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            Util.PlaySound(PrepBarrageScepter.prepSoundString, base.gameObject);

            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration);
                BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);

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
            if (base.fixedAge >= this.duration && base.isAuthority && !inputBank.skill4.down)
            {
                this.outer.SetNextState(new FireBarrageScepter());
                return;
            }
        }

        public override void OnExit()
        {
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
        }

        public override void OnExit()
        {
            BanditHelpers.ConsumeCloakDamageBuff(base.characterBody);
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
                    Util.PlayScaledSound(FireBarrageScepter.attackSoundString, base.gameObject, 1.2f);
                    base.PlayAnimation("Gesture, Additive", "FireRevolver");
                    base.PlayAnimation("Gesture, Override", "FireRevolver");
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
                            maxDistance = 80f,
                            procCoefficient = 1f,
                            damage = FireBarrageScepter.damageCoefficient * this.damageStat,
                            damageType = DamageType.ResetCooldownsOnKill | DamageType.SlowOnHit,
                            smartCollision = true
                        }.Fire();
                    }

                }
                else if (base.fixedAge - prevShot > endLag)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");
        public static float damageCoefficient;
        public static float force;
        public static float baseDuration;
        public static string attackSoundString = "Play_bandit_M2_shot";
        public static float recoilAmplitude = 2.2f;
        public static int maxBullets;
        public static float endLag;
        public static float spread;

        private int bulletCount;
        private ChildLocator childLocator;
        private float duration;
        private float prevShot;
        private string muzzleName;
        private bool isCrit;
        private float recoil;
    }
}

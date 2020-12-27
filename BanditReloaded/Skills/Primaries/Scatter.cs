using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BanditReloadedSkills
{
    public class Scatter : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            BanditHelpers.PlayCloakDamageSound(base.characterBody);
            base.AddRecoil(-1f * Scatter.recoilAmplitude, -2f * Scatter.recoilAmplitude, -0.5f * Scatter.recoilAmplitude, 0.5f * Scatter.recoilAmplitude);

            if (base.characterBody.skillLocator.primary.stock > 0 || Scatter.noReload)
            {
                this.maxDuration = Scatter.baseMaxDuration / this.attackSpeedStat;
                this.minDuration = Scatter.baseMinDuration / this.attackSpeedStat;
                Util.PlayScaledSound(Scatter.attackSoundString, base.gameObject, 0.8f);
                base.characterBody.skillLocator.primary.rechargeStopwatch = 0f;
            }
            else
            {
                this.maxDuration = Scatter.dryFireDuration;
                this.minDuration = Scatter.dryFireDuration;
                base.characterBody.skillLocator.primary.stock = 0;
                //base.characterBody.crosshairPrefab = Scatter.emptyCrosshairPrefab;
                Util.PlayScaledSound(Scatter.attackSoundString, base.gameObject, 1f);
                Util.PlayScaledSound("Play_commando_M2_grenade_throw", base.gameObject, 1.2f);
                dryFire = true;
                reloadTimer = Mathf.Max(base.characterBody.skillLocator.primary.rechargeStopwatch, Scatter.dryFireDuration);
            }

            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);

            base.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", this.maxDuration * 0.8f);
            base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", this.maxDuration * 0.8f);
            string muzzleName = "MuzzleShotgun";
            if (Scatter.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(Scatter.effectPrefab, base.gameObject, muzzleName, false);
            }
            if (base.isAuthority)
            {
                BulletAttack ba = new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 1,
                    procCoefficient = Scatter.procCoefficient,
                    damage = Scatter.damageCoefficient * this.damageStat,
                    force = Scatter.force,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    tracerEffectPrefab = Scatter.tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = Scatter.hitEffectPrefab,
                    isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
                    HitEffectNormal = false,
                    radius = Scatter.bulletRadius,
                    smartCollision = true,
                    stopperMask = Scatter.penetrateEnemies ? LayerIndex.world.mask : LayerIndex.entityPrecise.mask
                };
                ba.Fire();
                if (Scatter.pelletCount > 1)
                {
                    BulletAttack ba2 = new BulletAttack
                    {
                        owner = base.gameObject,
                        weapon = base.gameObject,
                        origin = aimRay.origin,
                        aimVector = aimRay.direction,
                        minSpread = Scatter.spreadBloomValue / ((float)Scatter.pelletCount - 1f),
                        maxSpread = Scatter.spreadBloomValue,
                        bulletCount = Scatter.pelletCount - 1,
                        procCoefficient = Scatter.procCoefficient,
                        damage = Scatter.damageCoefficient * this.damageStat,
                        force = Scatter.force,
                        falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                        tracerEffectPrefab = Scatter.tracerEffectPrefab,
                        muzzleName = muzzleName,
                        hitEffectPrefab = Scatter.hitEffectPrefab,
                        isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
                        HitEffectNormal = false,
                        radius = Scatter.bulletRadius,
                        smartCollision = true,
                        maxDistance = Scatter.range,
                        stopperMask = Scatter.penetrateEnemies ? LayerIndex.world.mask : LayerIndex.entityPrecise.mask
                    };
                    ba2.Fire();
                }
                base.characterBody.AddSpreadBloom(Scatter.spreadBloomValue);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            //base.characterBody.crosshairPrefab = defaultCrosshairPrefab;
            if (Scatter.vanillaBrainstalks && base.HasBuff(BuffIndex.NoCooldowns))
            {
                base.characterBody.skillLocator.primary.stock = base.characterBody.skillLocator.primary.maxStock;
            }
            BanditHelpers.ConsumeCloakDamageBuff(base.characterBody);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (dryFire)
            {
                base.characterBody.skillLocator.primary.rechargeStopwatch = reloadTimer;
            }
            this.buttonReleased |= !base.inputBank.skill1.down;
            if (base.fixedAge >= this.maxDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.buttonReleased && base.fixedAge >= this.minDuration)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }

        public static GameObject effectPrefab;
        public static GameObject hitEffectPrefab;
        public static GameObject tracerEffectPrefab;
        public static float damageCoefficient;
        public static float force;
        public static float bulletRadius;
        public static float baseMaxDuration;
        public static float baseMinDuration;
        public static string attackSoundString;
        public static float recoilAmplitude;
        public static float spreadBloomValue;
        public static uint pelletCount;
        public static float procCoefficient;
        public static float range;
        public static bool vanillaBrainstalks;
        public static bool penetrateEnemies;
        private float maxDuration;
        private float minDuration;
        private bool buttonReleased;
        public static bool noReload;
        public static float dryFireDuration;

        private bool dryFire = false;
        private float reloadTimer = 0f;
    }
}

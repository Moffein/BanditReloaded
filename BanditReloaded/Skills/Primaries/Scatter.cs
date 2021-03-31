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
            base.AddRecoil(-1f * Scatter.recoilAmplitude, -2f * Scatter.recoilAmplitude, -0.5f * Scatter.recoilAmplitude, 0.5f * Scatter.recoilAmplitude);

            this.maxDuration = Scatter.baseMaxDuration / this.attackSpeedStat;
            this.minDuration = Scatter.baseMinDuration / this.attackSpeedStat;
            Util.PlaySound(Scatter.attackSoundString, base.gameObject);
            base.characterBody.skillLocator.primary.rechargeStopwatch = 0f;
            if (base.characterBody.skillLocator.primary.stock == 0)
            {
                Util.PlaySound("Play_commando_M2_grenade_throw", base.gameObject);
            }

            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);

            base.PlayAnimation("Gesture, Additive", "FireMainWeapon", "FireMainWeapon.playbackRate", this.maxDuration);

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
                    maxSpread = Mathf.Max(Scatter.spreadBloomValue, base.characterBody.spreadBloomAngle),
                    bulletCount = Scatter.pelletCount,
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
                ba.Fire();
                base.characterBody.AddSpreadBloom(Scatter.spreadBloomValue);
            }
        }

        public override void OnExit()
        {
            if (!buttonReleased && base.characterBody)
            {
                base.characterBody.SetSpreadBloom(0f, false);
            }
            BanditHelpers.ConsumeCloakDamageBuff(base.characterBody);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.buttonReleased |= !base.inputBank.skill1.down;
            if (!playedSound && base.fixedAge > this.maxDuration * 0.5f)
            {
                playedSound = true;
                base.PlayCrossfade("Gesture, Additive", "EnterReload", "Reload.playbackRate", this.maxDuration * 0.5f, 0.1f);
                Util.PlaySound("Play_bandit_M1_pump", base.gameObject);
            }
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
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbandit2");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/hitsparkbandit");
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbandit2shotgun");
        public static float damageCoefficient;
        public static float force;
        public static float bulletRadius = 0.3f;
        public static float baseMaxDuration;
        public static float baseMinDuration;
        public static string attackSoundString = "Play_BanditReloaded_shotgun";
        public static float recoilAmplitude;
        public static float spreadBloomValue;
        public static uint pelletCount;
        public static float procCoefficient;
        public static float range;
        public static bool penetrateEnemies;
        private float maxDuration;
        private float minDuration;
        private bool buttonReleased;
        public static bool noReload;

        private bool playedSound = false;
    }
}

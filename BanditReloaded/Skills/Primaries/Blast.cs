using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BanditReloadedSkills
{
    public class Blast : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            base.AddRecoil(-1f * Blast.recoilAmplitude, -2f * Blast.recoilAmplitude, -0.5f * Blast.recoilAmplitude, 0.5f * Blast.recoilAmplitude);

            this.maxDuration = Blast.baseMaxDuration / this.attackSpeedStat;
            this.minDuration = Blast.baseMinDuration / this.attackSpeedStat;
            Util.PlaySound(Blast.useClassicSound ? Blast.classicSoundString : Blast.attackSoundString, base.gameObject);
            base.characterBody.skillLocator.primary.rechargeStopwatch = 0f;
            if (base.characterBody.skillLocator.primary.stock == 0)
            {
                Util.PlayScaledSound("Play_commando_M2_grenade_throw", base.gameObject, 1.2f);
            }

            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);

            if (base.characterBody.skillLocator.primary.baseRechargeInterval <= 0f)
            {
                base.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", 1.1f);
                base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", 1.1f);
            }
            else
            {
                base.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", base.characterBody.skillLocator.primary.CalculateFinalRechargeInterval() * 1.72f);
                base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", base.characterBody.skillLocator.primary.CalculateFinalRechargeInterval() * 1.72f);
            }
            string muzzleName = "MuzzleShotgun";
            if (Blast.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(Blast.effectPrefab, base.gameObject, muzzleName, false);
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
                    maxSpread = base.characterBody.spreadBloomAngle,
                    bulletCount = 1u,
                    procCoefficient = 1f,
                    damage = Blast.damageCoefficient * this.damageStat,
                    force = Blast.force,
                    falloffModel = Blast.useFalloff ? BulletAttack.FalloffModel.DefaultBullet : BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = Blast.tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = Blast.hitEffectPrefab,
                    isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
                    HitEffectNormal = true,
                    radius = Blast.bulletRadius,
                    smartCollision = true,
                    maxDistance = Blast.maxDistance
                };
                if (Blast.penetrateEnemies)
                {
                    ba.stopperMask = LayerIndex.world.mask;
                }
                ba.Fire();
            }
            base.characterBody.AddSpreadBloom(Blast.spreadBloomValue);
        }

        public override void OnExit()
        {
            base.OnExit();
            BanditHelpers.ConsumeCloakDamageBuff(base.characterBody);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
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
                base.characterBody.AddSpreadBloom((Blast.mashSpread - Blast.spreadBloomValue));
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }

        public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");
        public static string attackSoundString = "Play_BanditReloaded_blast";
        public static string classicSoundString = "Play_BanditReloaded_blast";
        public static float maxDistance;
        public static float damageCoefficient;
        public static float force;
        public static float bulletRadius = 0.4f;
        public static float baseMaxDuration;
        public static float baseMinDuration;
        public static float recoilAmplitude;
        public static float spreadBloomValue;
        public static bool individualReload;
        public static bool useFalloff;
        public static bool penetrateEnemies;
        public static bool noReload;
        public static bool useClassicSound;
        public static float mashSpread;
        private float maxDuration;
        private float minDuration;
        private bool buttonReleased;
    }
}

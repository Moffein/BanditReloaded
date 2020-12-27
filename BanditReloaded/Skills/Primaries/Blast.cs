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
            BanditHelpers.PlayCloakDamageSound(base.characterBody);
            base.AddRecoil(-1f * Blast.recoilAmplitude, -2f * Blast.recoilAmplitude, -0.5f * Blast.recoilAmplitude, 0.5f * Blast.recoilAmplitude);
            if (base.characterBody.skillLocator.primary.stock > 0 || Blast.noReload)
            {
                this.maxDuration = Blast.baseMaxDuration / this.attackSpeedStat;
                this.minDuration = Blast.baseMinDuration / this.attackSpeedStat;
                Util.PlayScaledSound(Blast.attackSoundString, base.gameObject, 0.85f);
                base.characterBody.skillLocator.primary.rechargeStopwatch = 0f;
            }
            else
            {
                this.maxDuration = Blast.dryFireDuration;
                this.minDuration = Blast.dryFireDuration;
                base.characterBody.skillLocator.primary.stock = 0;
                Util.PlayScaledSound(Blast.attackSoundString, base.gameObject, 1f);
                Util.PlayScaledSound("Play_commando_M2_grenade_throw", base.gameObject, 1.2f);
                dryFire = true;
                reloadTimer = Mathf.Max(base.characterBody.skillLocator.primary.rechargeStopwatch, Blast.dryFireDuration);
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
                base.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", base.characterBody.skillLocator.primary.CalculateFinalRechargeInterval() * 1.1f);
                base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", base.characterBody.skillLocator.primary.CalculateFinalRechargeInterval() * 1.1f);
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
            base.characterBody.AddSpreadBloom(this.isMash ? 2 * Blast.mashSpread : Blast.spreadBloomValue);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (Blast.vanillaBrainstalks && base.HasBuff(BuffIndex.NoCooldowns))
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
                base.characterBody.AddSpreadBloom((Blast.mashSpread - Blast.spreadBloomValue) / 2f);
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }

        public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerbanditshotgun");
        public static string attackSoundString = "Play_BanditReloaded_blast";
        public static float maxDistance;
        public static float damageCoefficient;
        public static float force;
        public static float bulletRadius = 0.4f;
        public static float dryFireDuration;
        public static float baseMaxDuration;
        public static float baseMinDuration;
        public static float recoilAmplitude;
        public static float spreadBloomValue;
        public static bool individualReload;
        public static bool useFalloff;
        public static bool penetrateEnemies;
        public static bool vanillaBrainstalks;
        public static bool noReload;
        public static float mashSpread;
        private float maxDuration;
        private float minDuration;
        private bool buttonReleased;
        public bool isMash = false;

        private bool dryFire = false;
        private float reloadTimer = 0f;
    }
}

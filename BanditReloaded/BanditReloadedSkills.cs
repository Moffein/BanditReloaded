using EntityStates.Engi.EngiWeapon;
using RoR2;
using RoR2.Networking;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ThinkInvisible.ClassicItems;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BanditReloaded
{
    public class BanditHelpers
    {
        public static void TriggerQuickdraw(SkillLocator skills)
        {
            if (enablePassive)
            {
                if (skills.primary.rechargeStock < skills.primary.maxStock
                && skills.primary.stock + skills.primary.rechargeStock <= skills.primary.maxStock)
                {
                    skills.primary.stock += skills.primary.rechargeStock;
                    if (skills.primary.rechargeStopwatch < 0f)
                    {
                        skills.primary.rechargeStopwatch = 0f;
                    }
                }
                else
                {
                    skills.primary.rechargeStopwatch += skills.primary.CalculateFinalRechargeInterval();
                }
            }
        }

        public static bool enablePassive;
    }

    public class Blast : BaseState
	{
		public override void OnEnter()
		{
            base.OnEnter();
            //this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
            /*if (Blast.individualReload && Blast.initialReloadDuration != base.skillLocator.primary.baseRechargeInterval)
            {
                float cdr = base.skillLocator.primary.CalculateFinalRechargeInterval() / base.skillLocator.primary.baseRechargeInterval;
                base.characterBody.skillLocator.primary.rechargeStopwatch = cdr * (base.skillLocator.primary.baseRechargeInterval - Blast.initialReloadDuration);
            }
            else
            {*/
            base.characterBody.skillLocator.primary.rechargeStopwatch = 0f;
            //}
            base.AddRecoil(-1f * Blast.recoilAmplitude, -2f * Blast.recoilAmplitude, -0.5f * Blast.recoilAmplitude, 0.5f * Blast.recoilAmplitude);
            if (base.characterBody.skillLocator.primary.stock > 0 || Blast.noReload)
            {
                this.maxDuration = Blast.baseMaxDuration / this.attackSpeedStat;
                this.minDuration = Blast.baseMinDuration / this.attackSpeedStat;
                Util.PlayScaledSound(Blast.attackSoundString, base.gameObject, 0.85f);
            }
            else
            {
                this.maxDuration = Blast.dryFireDuration;
                this.minDuration = Blast.dryFireDuration;
                base.characterBody.skillLocator.primary.stock = 0;
                //base.characterBody.crosshairPrefab = Blast.emptyCrosshairPrefab;
                Util.PlayScaledSound(Blast.attackSoundString, base.gameObject, 1f);
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
			base.characterBody.AddSpreadBloom(this.isMash? 2*Blast.mashSpread : Blast.spreadBloomValue);
		}
        
		public override void OnExit()
		{
			base.OnExit();
            //base.characterBody.crosshairPrefab = defaultCrosshairPrefab;
            if (Blast.vanillaBrainstalks && base.HasBuff(BuffIndex.NoCooldowns))
            {
                base.characterBody.skillLocator.primary.stock = base.characterBody.skillLocator.primary.maxStock;
            }
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
                base.characterBody.AddSpreadBloom((Blast.mashSpread - Blast.spreadBloomValue)/2f);
                return InterruptPriority.Any;
			}
			return InterruptPriority.Skill;
		}

		public static GameObject effectPrefab;
		public static GameObject hitEffectPrefab;
		public static GameObject tracerEffectPrefab;
        public static float maxDistance;
		public static float damageCoefficient;
        public static float force;
        public static float bulletRadius;
        public static float dryFireDuration;
        public static float baseMaxDuration;
        public static float baseMinDuration;
		public static string attackSoundString;
		public static float recoilAmplitude;
        public static float spreadBloomValue;
        public static bool individualReload;
        public static bool useFalloff;
        public static bool penetrateEnemies;
        public static bool vanillaBrainstalks;
        //public static float initialReloadDuration;
        public static bool noReload;
        public static float mashSpread;
        private float maxDuration;
		private float minDuration;
		private bool buttonReleased;
        public bool isMash = false;

        //public static GameObject emptyCrosshairPrefab;
        //private GameObject defaultCrosshairPrefab;
    }

    public class PrepLightsOut : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = PrepLightsOut.baseDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            Util.PlaySound(PrepLightsOut.prepSoundString, base.gameObject);
            this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
            base.characterBody.crosshairPrefab = PrepLightsOut.specialCrosshairPrefab;

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
                this.outer.SetNextState(new FireLightsOut());
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
        public static string prepSoundString;
        private float duration;
        private ChildLocator childLocator;
        public static GameObject specialCrosshairPrefab;
        private GameObject defaultCrosshairPrefab;
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

            base.PlayAnimation("Gesture, Additive", "FireRevolver");
            base.PlayAnimation("Gesture, Override", "FireRevolver");
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

        public static GameObject effectPrefab;
        public static GameObject hitEffectPrefab;
        public static GameObject tracerEffectPrefab;
        public static float damageCoefficient;
        public static float force;
        public static float baseDuration;
        public static float gracePeriodMin;
        public static float gracePeriodMax;
        public static float executeThreshold;
        public static float buffDamageCoefficient;
        public static bool executeBosses;
        public static string attackSoundString;
        public static float recoilAmplitude;
        private ChildLocator childLocator;
        private float duration;
    }

    public class GrenadeToss : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = GrenadeToss.baseDuration / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            base.PlayAnimation("Gesture", "FireRevolver", "FireRevolver.playbackRate", this.duration);
            Util.PlaySound("Play_commando_M2_grenade_throw", base.gameObject);
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(GrenadeToss.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * GrenadeToss.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
            if (base.characterMotor && !base.characterMotor.isGrounded)
            {
                Vector3 vector = -aimRay.direction * GrenadeToss.selfForce;
                vector.y *= 0.5f;
                base.characterMotor.ApplyForce(vector, true, false);
            }
            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
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
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject projectilePrefab;
        public static float damageCoefficient;
        public static float debuffDuration;
        public static float force;
        public static float selfForce;
        public static float baseDuration;
        private float duration;
    }

    public class ThermiteBomb : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            firstStock = (base.characterBody.skillLocator.secondary.stock + 1) == base.characterBody.skillLocator.secondary.maxStock;
            this.duration = ThermiteBomb.baseDuration / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            base.PlayAnimation("Gesture", "FireRevolver", "FireRevolver.playbackRate", this.duration);
            Util.PlaySound("Play_commando_M2_grenade_throw", base.gameObject);
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
            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
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
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject projectilePrefab;
        public static float damageCoefficient;
        public static float force;
        public static float selfForce;
        public static float baseDuration;
        public static float debuffDuration;
        public static float burnDamageMult;
        private float duration;
        private bool firstStock;
    }

    public class Scatter : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            //this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
            /*if (Scatter.individualReload && Scatter.initialReloadDuration != base.skillLocator.primary.baseRechargeInterval)
            {
                float cdr = base.skillLocator.primary.CalculateFinalRechargeInterval() / base.skillLocator.primary.baseRechargeInterval;
                base.characterBody.skillLocator.primary.rechargeStopwatch = cdr * (base.skillLocator.primary.baseRechargeInterval - Scatter.initialReloadDuration);
            }
            else
            {*/
            base.characterBody.skillLocator.primary.rechargeStopwatch = 0f;
            //}

            base.AddRecoil(-1f * Scatter.recoilAmplitude, -2f * Scatter.recoilAmplitude, -0.5f * Scatter.recoilAmplitude, 0.5f * Scatter.recoilAmplitude);

            if (base.characterBody.skillLocator.primary.stock > 0 || Scatter.noReload)
            {
                this.maxDuration = Scatter.baseMaxDuration / this.attackSpeedStat;
                this.minDuration = Scatter.baseMinDuration / this.attackSpeedStat;
                Util.PlayScaledSound(Scatter.attackSoundString, base.gameObject, 0.8f);
            }
            else
            {
                this.maxDuration = Scatter.dryFireDuration;
                this.minDuration = Scatter.dryFireDuration;
                base.characterBody.skillLocator.primary.stock = 0;
                //base.characterBody.crosshairPrefab = Scatter.emptyCrosshairPrefab;
                Util.PlayScaledSound(Scatter.attackSoundString, base.gameObject, 1f);
                Util.PlayScaledSound("Play_commando_M2_grenade_throw", base.gameObject,1.2f);
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
        //public static bool individualReload;
        public static bool vanillaBrainstalks;
        public static bool penetrateEnemies;
        private float maxDuration;
        private float minDuration;
        private bool buttonReleased;
        //public static float initialReloadDuration;
        public static bool noReload;
        public static float dryFireDuration;

        //public static GameObject emptyCrosshairPrefab;
        //private GameObject defaultCrosshairPrefab;
    }

    public class Assassinate : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);

            this.animator = base.GetModelAnimator();
            this.chargeDuration = Assassinate.baseChargeDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", this.chargeDuration*1.5f);
            base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", this.chargeDuration*1.5f);
            Transform modelTransform = base.GetModelTransform();
            base.cameraTargetParams.fovOverride = Assassinate.zoomFOV;
            if (modelTransform)
            {
                ChildLocator component = modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    this.muzzleTransform = component.FindChild(Assassinate.muzzleName);
                    if (this.muzzleTransform)
                    {
                        this.chargeupVfxGameObject = UnityEngine.Object.Instantiate<GameObject>(Assassinate.chargeupVfxPrefab, this.muzzleTransform);
                        this.chargeupVfxGameObject.GetComponent<ScaleParticleSystemDuration>().newDuration = this.chargeDuration;
                    }
                }
            }
            Util.PlaySound(Assassinate.beginChargeSoundString, base.gameObject);
            this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
            base.characterBody.crosshairPrefab = specialCrosshairPrefab;
        }

        public override void OnExit()
        {
            base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
            base.cameraTargetParams.fovOverride = -1f;
            if (this.chargeupVfxGameObject)
            {
                EntityState.Destroy(this.chargeupVfxGameObject);
                this.chargeupVfxGameObject = null;
            }
            if (this.holdChargeVfxGameObject)
            {
                EntityState.Destroy(this.holdChargeVfxGameObject);
                this.holdChargeVfxGameObject = null;
            }
            base.OnExit();
            
            if (base.inputBank.skill4.down)
            {
                base.skillLocator.utility.rechargeStopwatch = base.skillLocator.utility.CalculateFinalRechargeInterval() / 6f;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;

            if (this.stopwatch >= this.chargeDuration)
            {
                if (this.stopwatch <= this.chargeDuration + Assassinate.perfectChargeDuration)
                {
                    this.chargeCoefficient = 1.2f;
                    if (FireChargeShot.effectPrefab)
                    {
                        if (this.stopwatch - this.lastMuzzleFlash > 0.1f)
                        {
                            this.lastMuzzleFlash = this.stopwatch;
                            EffectManager.SimpleMuzzleFlash(FireChargeShot.effectPrefab, base.gameObject, muzzleName, false);
                        }
                    }
                    base.characterBody.crosshairPrefab = perfectCrosshairPrefab;
                }
                else
                {
                    if (base.characterBody.crosshairPrefab != specialCrosshairPrefab)
                    {
                        base.characterBody.crosshairPrefab = specialCrosshairPrefab;
                    }
                    this.chargeCoefficient = 1f;
                }
                base.characterBody.SetSpreadBloom(0f, false);

                if (!playedSound)
                {
                    playedSound = true;
                    Util.PlayScaledSound(Assassinate.chargeSoundString, base.gameObject, Assassinate.perfectChargeDuration);
                }
                if (!chargeEffect)
                {
                    chargeEffect = true;
                    if (this.chargeupVfxGameObject)
                    {
                        EntityState.Destroy(this.chargeupVfxGameObject);
                        this.chargeupVfxGameObject = null;
                    }
                    if (!this.holdChargeVfxGameObject && this.muzzleTransform)
                    {
                        this.holdChargeVfxGameObject = UnityEngine.Object.Instantiate<GameObject>(Assassinate.holdChargeVfxPrefab, this.muzzleTransform);
                    }
                }
            }
            else
            {
                this.chargeCoefficient = this.stopwatch / this.chargeDuration;
                base.characterBody.SetSpreadBloom(1.25f * (1f - chargeCoefficient), false);
            }
            if (!base.inputBank || (this.stopwatch >= Assassinate.minimumStateDuration && base.isAuthority))
            {
                if (!base.inputBank.skill3.down)
                {
                    this.outer.SetNextState(new FireChargeShot { chargeCoefficient = this.chargeCoefficient });
                    return;
                }
                else if (!base.inputBank)
                {
                    base.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public static string chargeSoundString;
        public static string beginChargeSoundString;
        public static float baseChargeDuration;
        public static float minimumStateDuration;
        public static float zoomFOV;

        private float stopwatch;
        private Animator animator;
        private float lastMuzzleFlash = 0f;

        private float chargeDuration;

        private bool playedSound = false;
        private float chargeCoefficient = 0f;

        public static GameObject chargeupVfxPrefab;
        public static GameObject holdChargeVfxPrefab;

        private GameObject chargeupVfxGameObject;
        private GameObject holdChargeVfxGameObject;
        private Transform muzzleTransform;
        public static string muzzleName;

        private GameObject defaultCrosshairPrefab;
        private GameObject specialCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/banditcrosshair");
        private GameObject perfectCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/StraightBracketCrosshair");

        private bool chargeEffect = false;

        public static float perfectChargeDuration;
    }

    public class FireChargeShot : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = FireChargeShot.baseDuration / this.attackSpeedStat;
            base.AddRecoil(-3f * FireChargeShot.recoilAmplitude, -4f * FireChargeShot.recoilAmplitude, -0.5f * FireChargeShot.recoilAmplitude, 0.5f * FireChargeShot.recoilAmplitude);
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            string muzzleName = "MuzzleShotgun";
            Util.PlaySound(FireChargeShot.attackSoundString, base.gameObject);
            if (this.chargeCoefficient > 1f)
            {
                perfect = true;
                this.chargeCoefficient = 1f;
                Util.PlaySound(FireChargeShot.fullChargeSoundString, base.gameObject);
            }
            base.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", this.duration * 0.8f);
            base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", this.duration * 0.8f);

            if (FireChargeShot.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(FireChargeShot.effectPrefab, base.gameObject, muzzleName, false);
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
                    damage = Mathf.Lerp(FireChargeShot.minDamageCoefficient, FireChargeShot.maxDamageCoefficient, chargeCoefficient) * this.damageStat + (perfect ? FireChargeShot.perfectChargeBonus * this.damageStat : 0f),
                    force = Mathf.Lerp(FireChargeShot.minForce, FireChargeShot.maxForce, chargeCoefficient),
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = perfect ? FireChargeShot.perfectTracerEffectPrefab : FireChargeShot.tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = FireChargeShot.hitEffectPrefab,
                    isCrit = this.RollCrit(),
                    radius = Mathf.Lerp(FireChargeShot.minRadius, FireChargeShot.maxRadius, chargeCoefficient),
                    maxDistance = 2000f,
                    stopperMask = LayerIndex.world.mask,
                    damageType = DamageType.Stun1s,
                    smartCollision = true,
                    procCoefficient = Mathf.Lerp(0f, 1f, chargeCoefficient)
                    //procCoefficient = 1f
                }.Fire();

                if (base.characterBody && base.characterMotor && !base.characterMotor.isGrounded)
                {
                    Vector3 vector = -aimRay.direction * Mathf.Lerp(FireChargeShot.selfForceMin, FireChargeShot.selfForceMax, chargeCoefficient);
                    vector.y *= 0.5f;
                    base.characterMotor.ApplyForce(vector, true, false);
                }
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

        public static GameObject effectPrefab;
        public static GameObject hitEffectPrefab;
        public static GameObject tracerEffectPrefab;
        public static GameObject perfectTracerEffectPrefab;
        public static float minDamageCoefficient;
        public static float maxDamageCoefficient;
        public static float perfectChargeBonus;
        public static float minForce;
        public static float maxForce;
        public static float baseDuration;
        public static float barrierPercent;
        public static string attackSoundString;
        public static string fullChargeSoundString;
        public static float recoilAmplitude;
        public float chargeCoefficient;
        public static float selfForceMax;
        public static float selfForceMin;

        private ChildLocator childLocator;
        private float duration;
        private bool perfect = false;
        public static float minRadius;
        public static float maxRadius;
        public static int maxTargets;
    }

    public class CastSmokescreen : BaseState
    {
        private void CastSmoke()
        {
            if (!this.hasCastSmoke)
            {
                Util.PlaySound(CastSmokescreen.startCloakSoundString, base.gameObject);
            }
            EffectManager.SpawnEffect(CastSmokescreen.smokescreenEffectPrefab, new EffectData
            {
                origin = base.transform.position
            }, false);
            int layerIndex = this.animator.GetLayerIndex("Impact");
            if (layerIndex >= 0)
            {
                this.animator.SetLayerWeight(layerIndex, 2f);
                this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
            }
            if (NetworkServer.active)
            {
                new BlastAttack
                {
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                    baseDamage = this.damageStat * CastSmokescreen.damageCoefficient,
                    baseForce = CastSmokescreen.forceMagnitude,
                    position = base.transform.position,
                    radius = CastSmokescreen.radius,
                    falloffModel = BlastAttack.FalloffModel.None,
                    damageType = DamageType.Stun1s,
                    crit = base.RollCrit(),
                    attackerFiltering = AttackerFiltering.NeverHit
                }.Fire();
            }
        }
        public override void OnEnter()
        {
            this.duration = CastSmokescreen.baseDuration / this.attackSpeedStat;
            this.totalDuration = CastSmokescreen.stealthDuration + this.totalDuration;
            base.PlayCrossfade("Gesture, Smokescreen", "CastSmokescreen", "CastSmokescreen.playbackRate", this.duration, 0.2f);
            this.animator = base.GetModelAnimator();
            Util.PlaySound(CastSmokescreen.jumpSoundString, base.gameObject);
            EffectManager.SpawnEffect(CastSmokescreen.initialEffectPrefab, new EffectData
            {
                origin = base.transform.position
            }, true);
            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    characterBody.AddBuff(BuffIndex.CloakSpeed);
                }
                if (base.isAuthority)
                {
                    base.OnEnter();
                }
                BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
            }
        }

        public override void OnExit()
        {
            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    if (base.characterBody.HasBuff(BuffIndex.Cloak))
                    {
                        base.characterBody.RemoveBuff(BuffIndex.Cloak);
                    }
                    if (base.characterBody.HasBuff(BuffIndex.CloakSpeed))
                    {
                        base.characterBody.RemoveBuff(BuffIndex.CloakSpeed);
                    }
                }
            }
            /*if (base.characterBody && base.isAuthority)
            {
                DamageInfo damageInfo = new DamageInfo()
                {
                    damage = 1,
                    position = Vector3.zero,
                    force = Vector3.zero,
                    damageColorIndex = DamageColorIndex.Default,
                    crit = false,
                    attacker = null,
                    inflictor = null,
                    damageType = (DamageType.NonLethal | DamageType.BypassArmor),
                    procCoefficient = BanditNetworking.removeCloakAndSpeed,
                    procChainMask = default(ProcChainMask)
                };
                if (NetworkServer.active)
                {
                    //this.healthComponent.TakeDamage(damageInfo);
                }
                (else if (ClientScene.ready)
                {
                    BanditNetworking.write.StartMessage(53);
                    BanditNetworking.write.Write(healthComponent.gameObject);
                    BanditNetworking.WriteDmgInfo(BanditNetworking.write, damageInfo);
                    BanditNetworking.write.Write(healthComponent != null);
                    BanditNetworking.write.FinishMessage();
                    ClientScene.readyConnection.SendWriter(BanditNetworking.write, QosChannelIndex.defaultReliable.intVal);
                }
            }*/
            if (!this.outer.destroying)
            {
                this.CastSmoke();
            }
            Util.PlaySound(CastSmokescreen.stopCloakSoundString, base.gameObject);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && !this.hasCastSmoke)
            {
                this.CastSmoke();
                if (base.characterBody && NetworkServer.active)
                {
                    base.characterBody.AddBuff(BuffIndex.Cloak);
                }
                /*if (base.characterBody)
                {
                    DamageInfo damageInfo = new DamageInfo()
                    {
                        damage = 1,
                        position = Vector3.zero,
                        force = Vector3.zero,
                        damageColorIndex = DamageColorIndex.Default,
                        crit = false,
                        attacker = null,
                        inflictor = null,
                        damageType = (DamageType.NonLethal | DamageType.BypassArmor),
                        procCoefficient = BanditNetworking.addCloak,
                        procChainMask = default(ProcChainMask)
                    };
                    if (NetworkServer.active)
                    {
                        this.healthComponent.TakeDamage(damageInfo);
                    }
                    else if (ClientScene.ready)
                    {
                        BanditNetworking.write.StartMessage(53);
                        BanditNetworking.write.Write(healthComponent.gameObject);
                        BanditNetworking.WriteDmgInfo(BanditNetworking.write, damageInfo);
                        BanditNetworking.write.Write(healthComponent != null);
                        BanditNetworking.write.FinishMessage();
                        ClientScene.readyConnection.SendWriter(BanditNetworking.write, QosChannelIndex.defaultReliable.intVal);
                    }
                }*/
                this.hasCastSmoke = true;
            }
            if (base.fixedAge >= this.totalDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (!this.hasCastSmoke)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Any;
        }

        public static float baseDuration;
        public static float stealthDuration = 3f;
        public static string jumpSoundString;
        public static string startCloakSoundString;
        public static string stopCloakSoundString;
        public static GameObject initialEffectPrefab;
        public static GameObject smokescreenEffectPrefab;
        public static float damageCoefficient = 1.3f;
        public static float radius = 4f;
        public static float forceMagnitude = 0f;

        private float duration;
        private float totalDuration;
        private bool hasCastSmoke;
        private Animator animator;
    }

    public class CastSmokescreenNoDelay : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.CastSmoke();
            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    base.characterBody.AddBuff(BuffIndex.Cloak);
                    base.characterBody.AddBuff(BuffIndex.CloakSpeed);
                }
                BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
            }
        }

        public override void OnExit()
        {
            if (base.characterBody && NetworkServer.active)
            {
                if (base.characterBody.HasBuff(BuffIndex.Cloak))
                {
                    base.characterBody.RemoveBuff(BuffIndex.Cloak);
                }
                if (base.characterBody.HasBuff(BuffIndex.CloakSpeed))
                {
                    base.characterBody.RemoveBuff(BuffIndex.CloakSpeed);
                }
            }
            if (!this.outer.destroying)
            {
                this.CastSmoke();
            }
            if (CastSmokescreenNoDelay.destealthMaterial)
            {
                TemporaryOverlay temporaryOverlay = this.animator.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1f;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = CastSmokescreenNoDelay.destealthMaterial;
                temporaryOverlay.inspectorCharacterModel = this.animator.gameObject.GetComponent<CharacterModel>();
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.animateShaderAlpha = true;
            }
            Util.PlaySound(CastSmokescreenNoDelay.stopCloakSoundString, base.gameObject);

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch >= CastSmokescreenNoDelay.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void CastSmoke()
        {
            if (!this.hasCastSmoke)
            {
                Util.PlaySound(CastSmokescreenNoDelay.startCloakSoundString, base.gameObject);
                this.hasCastSmoke = true;
            }
            EffectManager.SpawnEffect(CastSmokescreenNoDelay.smokescreenEffectPrefab, new EffectData
            {
                origin = base.transform.position
            }, false);
            int layerIndex = this.animator.GetLayerIndex("Impact");
            if (layerIndex >= 0)
            {
                this.animator.SetLayerWeight(layerIndex, 1f);
                this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
            }

            new BlastAttack
            {
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                baseDamage = this.damageStat * CastSmokescreenNoDelay.damageCoefficient,
                baseForce = CastSmokescreenNoDelay.forceMagnitude,
                position = base.transform.position,
                radius = CastSmokescreenNoDelay.radius,
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = DamageType.Stun1s,
                crit = base.RollCrit(),
                attackerFiltering = AttackerFiltering.NeverHit
            }.Fire();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.stopwatch <= CastSmokescreenNoDelay.minimumStateDuration)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Any;
        }

        public static float duration;
        public static float minimumStateDuration;
        public static string startCloakSoundString;
        public static string stopCloakSoundString;
        public static GameObject smokescreenEffectPrefab;
        public static Material destealthMaterial;
        public static float damageCoefficient;
        public static float radius;
        public static float forceMagnitude;

        private float stopwatch;
        private bool hasCastSmoke;
        private Animator animator;
    }

    public class ClusterBomb : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ClusterBomb.baseDuration / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            base.PlayAnimation("Gesture", "FireRevolver", "FireRevolver.playbackRate", this.duration);
            Util.PlaySound("Play_commando_M2_grenade_throw", base.gameObject);
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(ClusterBomb.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * ClusterBomb.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
            if (base.characterMotor && !base.characterMotor.isGrounded)
            {
                Vector3 vector = -aimRay.direction * ClusterBomb.selfForce;
                vector.y *= 0.5f;
                base.characterMotor.ApplyForce(vector, true, false);
            }
            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
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
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject projectilePrefab;
        public static float damageCoefficient;
        public static float force;
        public static float selfForce;
        public static float baseDuration;
        public static int bombletCount;
        public static float bombletDamageCoefficient;
        private float duration;

    }

    public class PrepBarrage : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = PrepBarrage.baseDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", this.duration);
            Util.PlaySound(PrepBarrage.prepSoundString, base.gameObject);
            
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration);
                BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority && !inputBank.skill4.down)
            {
                this.outer.SetNextState(new FireBarrage());
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
        public static string prepSoundString;
        private float duration;
        private ChildLocator childLocator;
    }

    public class FireBarrage : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            bulletCount = FireBarrage.maxBullets;
            prevShot = 0f;
            this.duration = FireBarrage.baseDuration / this.attackSpeedStat;
            this.recoil = FireBarrage.recoilAmplitude / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            muzzleName = "MuzzlePistol";
            isCrit = base.RollCrit();
        }

        public override void OnExit()
        {
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
                        Util.PlayScaledSound(FireBarrage.attackSoundString, base.gameObject, 1.2f);
                        base.PlayAnimation("Gesture, Additive", "FireRevolver");
                        base.PlayAnimation("Gesture, Override", "FireRevolver");
                        if (FireBarrage.effectPrefab)
                        {
                            EffectManager.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, muzzleName, false);
                        }
                        float bulletSpread = bulletCount <= 0 ? 0f : FireBarrage.spread;
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
                                force = FireBarrage.force,
                                falloffModel = BulletAttack.FalloffModel.None,
                                tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
                                muzzleName = muzzleName,
                                hitEffectPrefab = FireBarrage.hitEffectPrefab,
                                isCrit = this.isCrit,
                                HitEffectNormal = true,
                                radius = 0.4f,
                                maxDistance = 80f,
                                procCoefficient = 1f,
                                damage = FireBarrage.damageCoefficient * this.damageStat,
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

        public static GameObject effectPrefab;
        public static GameObject hitEffectPrefab;
        public static GameObject tracerEffectPrefab;
        public static float damageCoefficient;
        public static float force;
        public static float baseDuration;
        public static float gracePeriodMin;
        public static float gracePeriodMax;
        public static float executeThreshold;
        public static float buffDamageCoefficient;
        public static bool executeBosses;
        public static string attackSoundString;
        public static float recoilAmplitude;
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
        public static string prepSoundString;
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

        public static GameObject effectPrefab;
        public static GameObject hitEffectPrefab;
        public static GameObject tracerEffectPrefab;
        public static float damageCoefficient;
        public static float force;
        public static float baseDuration;
        public static float gracePeriodMin;
        public static float gracePeriodMax;
        public static float executeThreshold;
        public static float buffDamageCoefficient;
        public static bool executeBosses;
        public static string attackSoundString;
        public static float recoilAmplitude;
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
        public static string prepSoundString;
        private float duration;
        private ChildLocator childLocator;
        public static GameObject specialCrosshairPrefab;
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

        public static GameObject effectPrefab;
        public static GameObject hitEffectPrefab;
        public static GameObject tracerEffectPrefab;
        public static float damageCoefficient;
        public static float force;
        public static float baseDuration;
        public static float gracePeriodMin;
        public static float gracePeriodMax;
        public static float executeThreshold;
        public static float buffDamageCoefficient;
        public static bool executeBosses;
        public static string attackSoundString;
        public static float recoilAmplitude;
        private ChildLocator childLocator;
        private float duration;
    }
}

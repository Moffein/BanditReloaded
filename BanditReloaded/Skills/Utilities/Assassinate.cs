using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace EntityStates.BanditReloadedSkills
{
    public class Assassinate : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);

            this.animator = base.GetModelAnimator();
            this.chargeDuration = Assassinate.baseChargeDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", this.chargeDuration * 1.5f);
            base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", this.chargeDuration * 1.5f);
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

            if (base.characterBody)
            {
                if (base.characterBody.HasBuff(BanditReloaded.BanditReloaded.cloakDamageBuff))
                {
                    base.characterBody.ClearTimedBuffs(BanditReloaded.BanditReloaded.cloakDamageBuff);
                    base.characterBody.AddTimedBuff(BanditReloaded.BanditReloaded.cloakDamageBuff, this.chargeDuration + 0.5f);
                }
            }
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

        public static string chargeSoundString = "Play_item_proc_crit_cooldown";
        public static string beginChargeSoundString = "Play_MULT_m1_snipe_charge";
        public static float baseChargeDuration;
        public static float minimumStateDuration = 0f;
        public static float zoomFOV;

        private float stopwatch;
        private Animator animator;
        private float lastMuzzleFlash = 0f;

        private float chargeDuration;

        private bool playedSound = false;
        private float chargeCoefficient = 0f;

        public static GameObject chargeupVfxPrefab = EntityStates.Toolbot.ChargeSpear.chargeupVfxPrefab;
        public static GameObject holdChargeVfxPrefab = EntityStates.Toolbot.ChargeSpear.holdChargeVfxPrefab;

        private GameObject chargeupVfxGameObject;
        private GameObject holdChargeVfxGameObject;
        private Transform muzzleTransform;
        public static string muzzleName = "MuzzleShotgun";

        private GameObject defaultCrosshairPrefab;
        private GameObject specialCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/banditcrosshair");
        private GameObject perfectCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/StraightBracketCrosshair");

        private bool chargeEffect = false;

        public static float perfectChargeDuration = 0f;
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
            BanditHelpers.ConsumeCloakDamageBuff(base.characterBody);
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

        public static GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashbanditshotgun");
        public static GameObject tracerEffectPrefab = EntityStates.Sniper.SniperWeapon.FireRifle.tracerEffectPrefab;
        public static GameObject perfectTracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracertoolbotrebar");
        public static float minDamageCoefficient;
        public static float maxDamageCoefficient;
        public static float perfectChargeBonus = 2f;
        public static float minForce;
        public static float maxForce;
        public static float baseDuration;
        public static string attackSoundString = "Play_bandit_M2_shot";
        public static string fullChargeSoundString = "Play_item_use_lighningArm";
        public static float recoilAmplitude = 4f;
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
}

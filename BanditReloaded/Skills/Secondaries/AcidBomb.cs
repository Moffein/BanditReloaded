using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.BanditReloadedSkills
{
    public class AcidBomb : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            BanditHelpers.PlayCloakDamageSound(base.characterBody);
            this.duration = AcidBomb.baseDuration / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            base.PlayAnimation("Gesture", "FireRevolver", "FireRevolver.playbackRate", this.duration);
            Util.PlaySound("Play_commando_M2_grenade_throw", base.gameObject);
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(AcidBomb.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * AcidBomb.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
            if (base.characterMotor && !base.characterMotor.isGrounded)
            {
                Vector3 vector = -aimRay.direction * AcidBomb.selfForce;
                vector.y *= 0.5f;
                base.characterMotor.ApplyForce(vector, true, false);
            }
            BanditHelpers.TriggerQuickdraw(base.characterBody.skillLocator);
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
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject projectilePrefab;
        public static float damageCoefficient;
        public static float debuffDuration;
        public static float force = 0f;
        public static float selfForce = 0f;
        public static float baseDuration;
        private float duration;
    }
}

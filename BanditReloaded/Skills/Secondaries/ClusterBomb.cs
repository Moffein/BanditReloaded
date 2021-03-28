using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace EntityStates.BanditReloadedSkills
{
    public class ClusterBomb : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ClusterBomb.baseDuration / this.attackSpeedStat;
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            base.PlayAnimation("Gesture, Additive", "SlashBlade", "SlashBlade.playbackRate", this.duration);
            Util.PlaySound("Play_BanditReloaded_dynamite_toss", base.gameObject);
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
        public static float force = 2500f;
        public static float selfForce = 0f;
        public static float baseDuration;
        public static float bombletDamageCoefficient;
        private float duration;
    }
}

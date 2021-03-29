using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace BanditReloaded.Components
{
    class BootlegThermiteOverlapAttack : MonoBehaviour
    {
        public void Awake()
        {
            this.damageStopwatch = 0f;
            this.detonationStopwatch = 0f;
            this.cachedThermiteCount = 0;
            projectileStick = this.gameObject.GetComponent<ProjectileStickOnImpact>();
            this.projectileImpactExplosion = this.gameObject.GetComponent<ProjectileImpactExplosion>();
            this.projectileDamage = this.gameObject.GetComponent<ProjectileDamage>();
            this.projectileController = this.gameObject.GetComponent<ProjectileController>();
        }

        public void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!firstHit)
            {
                if (!this.stuckObject || (!this.stuckObjectHealthComponent || !this.stuckObjectHealthComponent.alive))
                {
                    Detonate();
                    return;
                }
            }
            else
            {
                if (projectileStick)
                {
                    this.stuckObject = projectileStick.victim;
                    if (this.stuckObject)
                    {
                        firstHit = false;
                        this.stuckObjectHealthComponent = this.stuckObject.GetComponent<HealthComponent>();
                        if (this.stuckObjectHealthComponent.body)
                        {
                            this.stuckObjectHealthComponent.body.AddBuff(ModContentPack.thermiteBuff);
                            this.cachedThermiteCount = this.stuckObjectHealthComponent.body.GetBuffCount(ModContentPack.thermiteBuff);
                        }

                        if (this.projectileDamage)
                        {
                            this.damage = this.projectileDamage.damage * this.damageCoefficient;
                            this.isCrit = this.projectileDamage.crit;
                        }
                        if (this.projectileController)
                        {
                            this.attacker = this.projectileController.owner;
                            this.procChainMask = this.projectileController.procChainMask;
                        }
                        if (this.attacker)
                        {
                            TeamComponent tc = this.attacker.GetComponent<TeamComponent>();
                            if (tc)
                            {
                                this.teamIndex = tc.teamIndex;
                            }
                        }

                        ResetNearbyThermite();
                    }
                }
            }

            damageStopwatch += Time.fixedDeltaTime;
            if (damageStopwatch > damageInterval)
            {
                damageStopwatch -= damageInterval;

                if (this.stuckObject && this.stuckObjectHealthComponent)
                {
                    FireOverlapAttack();
                }
            }

            detonationStopwatch += Time.fixedDeltaTime;
            if (detonationStopwatch > lifetimeAfterImpact)
            {
                Detonate();
            }
        }

        public void OnDestroy()
        {
            if (this.stuckObject && this.stuckObjectHealthComponent.body && this.stuckObjectHealthComponent.body.HasBuff(ModContentPack.thermiteBuff))
            {
                this.stuckObjectHealthComponent.body.RemoveBuff(ModContentPack.thermiteBuff);
            }
        }

        private void FireOverlapAttack()
        {
            if (FriendlyFireManager.ShouldDirectHitProceed(this.stuckObjectHealthComponent, this.teamIndex))
            {
                this.stuckObjectHealthComponent.TakeDamage(new DamageInfo
                {
                    attacker = this.attacker,
                    inflictor = this.gameObject,
                    damage = this.damage,
                    damageColorIndex = DamageColorIndex.WeakPoint,
                    damageType = this.damageType,
                    crit = this.isCrit,
                    dotIndex = DotController.DotIndex.None,
                    force = Vector3.zero,
                    position = this.transform.position,
                    procChainMask = this.procChainMask,
                    procCoefficient = this.procCoefficient
                });

                GlobalEventManager.instance.OnHitEnemy(new DamageInfo
                {
                    attacker = this.attacker,
                    inflictor = this.gameObject,
                    damage = this.damage,
                    damageColorIndex = DamageColorIndex.WeakPoint,
                    damageType = this.damageType,
                    crit = this.isCrit,
                    dotIndex = DotController.DotIndex.None,
                    force = Vector3.zero,
                    position = this.transform.position,
                    procChainMask = this.procChainMask,
                    procCoefficient = this.procCoefficient
                }, this.stuckObject);

                EffectManager.SpawnEffect(BootlegThermiteOverlapAttack.burnEffectPrefab, new EffectData
                {
                    origin = this.transform.position,
                    scale = 2f
                }, true);
            }
        }

        private void Detonate()
        {
            ProjectileSimple ps = this.gameObject.GetComponent<ProjectileSimple>();
            if (!this.detonated)
            {
                this.detonated = true;
                if (ps)
                {
                    ps.stopwatch = 0f;
                }

                /*if (this.projectileDamage)
                {
                    this.projectileDamage.damage *= this.cachedThermiteCount;
                }*/

                if (projectileImpactExplosion)
                {
                    /*float radius = projectileImpactExplosion.blastRadius;
                    for (int i = 0; i < this.cachedThermiteCount; i++)
                    {
                        radius *= 0.95f;
                        projectileImpactExplosion.blastRadius += radius;
                    }*/
                    //projectileImpactExplosion.blastRadius = projectileImpactExplosion.blastRadius * this.cachedThermiteCount;
                    projectileImpactExplosion.blastRadius *= 2f;
                    projectileImpactExplosion.stopwatch = projectileImpactExplosion.lifetime;
                }
            }
            else
            {
                if (projectileImpactExplosion)
                {
                    projectileImpactExplosion.stopwatch = projectileImpactExplosion.lifetime;
                }
                if (ps)
                {
                    ps.stopwatch = ps.lifetime;
                }
            }
        }

        private void ResetNearbyThermite()
        {
            Collider[] array = Physics.OverlapSphere(this.transform.position, 40f, LayerIndex.projectile.mask);
            for (int i = 0; i < array.Length; i++)
            {
                ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                if (pc && pc.gameObject != this.gameObject)
                {
                    BootlegThermiteOverlapAttack bt = pc.gameObject.GetComponent<BootlegThermiteOverlapAttack>();
                    if (bt && bt.stuckObject == this.stuckObject)
                    {
                        //bt.detonated = true;
                        bt.detonationStopwatch = -0.13f*(i+1);
                        ProjectileSimple ps = pc.gameObject.GetComponent<ProjectileSimple>();
                        if (ps)
                        {
                            ps.stopwatch = 0f;
                        }
                        if (bt.projectileImpactExplosion)
                        {
                            bt.projectileImpactExplosion.stopwatch = 0f;
                        }
                        if (bt.stuckObject && bt.stuckObjectHealthComponent && bt.stuckObjectHealthComponent.body)
                        {
                            bt.cachedThermiteCount = bt.stuckObjectHealthComponent.body.GetBuffCount(ModContentPack.thermiteBuff);
                        }
                    }
                }
            }
        }

        public float damageCoefficient;
        public float procCoefficient;
        public DamageType damageType = DamageType.Generic;
        public float damageInterval;
        public float lifetimeAfterImpact;

        public static GameObject burnEffectPrefab;

        private HealthComponent stuckObjectHealthComponent;
        public GameObject stuckObject = null;
        private ProjectileStickOnImpact projectileStick;
        private ProjectileImpactExplosion projectileImpactExplosion;
        private ProjectileDamage projectileDamage;
        private ProjectileController projectileController;
        private ProcChainMask procChainMask;
        private GameObject attacker;
        private float damage;
        private TeamIndex teamIndex;

        public int cachedThermiteCount;
        private bool firstHit = true;
        private bool isCrit;
        public float damageStopwatch;
        public float detonationStopwatch;
        private bool detonated = false;
    }
}

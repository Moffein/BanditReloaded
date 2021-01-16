using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using R2API.Utils;

namespace BanditReloaded.Components
{
    class DynamiteRotation : MonoBehaviour
    {
        public void Awake()
        {
            projectileImpactExplosion = base.gameObject.GetComponent<ProjectileImpactExplosion>();
        }

        public void FixedUpdate()
        {
            if (projectileImpactExplosion.GetFieldValue<bool>("hasImpact"))
            {
                Destroy(this);
            }
        }

        public void Update()
        {
            base.transform.rotation = Quaternion.AngleAxis(-360f * Time.deltaTime, Vector3.right) * base.transform.rotation;
        }

        private ProjectileImpactExplosion projectileImpactExplosion;
    }
}

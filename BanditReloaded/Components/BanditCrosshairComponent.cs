using System;
using System.Collections.Generic;
using System.Text;
using EntityStates.BanditReloadedSkills;
using RoR2;
using UnityEngine;

namespace BanditReloaded
{
    public class BanditCrosshairComponent : MonoBehaviour
    {
        private void Awake()
        {
            cb = base.GetComponent<CharacterBody>();
            skills = cb.skillLocator;
            defaultCrosshairPrefab = cb.crosshairPrefab;
        }
        private void FixedUpdate()
        {
            if ((skills.primary.skillDef.skillName == "FireSlug" && !Blast.noReload) || (skills.primary.skillDef.skillName == "FireScatter" && !Scatter.noReload))
            {
                if (skills.primary.maxStock > 1 && skills.primary.stock > 0)
                {
                    if (cb.crosshairPrefab == emptyCrosshairPrefab)
                    {
                        cb.crosshairPrefab = defaultCrosshairPrefab;
                    }
                }
                else
                {
                    if (cb.crosshairPrefab == defaultCrosshairPrefab)
                    {
                        cb.crosshairPrefab = emptyCrosshairPrefab;
                    }
                }
            }
        }
        private CharacterBody cb;
        private SkillLocator skills;
        private static GameObject emptyCrosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/badcrosshair");
        private GameObject defaultCrosshairPrefab;
    }
}

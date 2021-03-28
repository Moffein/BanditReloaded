using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace BanditReloaded
{
    public class BanditThermiteComponent : MonoBehaviour
    {
        private void Awake()
        {
            ModelLocator ml = base.GetComponent<ModelLocator>();
            cb = base.GetComponent<CharacterBody>();
            if (cb && ml && ml.modelTransform && ml.modelTransform.gameObject)
            {
                burnEffectController = base.gameObject.AddComponent<BurnEffectController>();
                burnEffectController.effectType = BurnEffectController.normalEffect;
                burnEffectController.target = ml.modelTransform.gameObject;
            }
            else
            {
                DestroySelf();
            }
        }
        private void Update()
        {
            if (!cb || cb.GetBuffCount(ModContentPack.thermiteBuff) <= 0)
            {
                DestroySelf();
            }
        }

        private void DestroySelf()
        {
            if (burnEffectController)
            {
                Destroy(burnEffectController);
            }
            Destroy(this);
        }

        private BurnEffectController burnEffectController;
        private CharacterBody cb;
    }
}

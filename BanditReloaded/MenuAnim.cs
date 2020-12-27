using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;

namespace BanditReloaded
{
    public class MenuAnimComponent : MonoBehaviour
    {
        internal void OnEnable()
        {
            if (base.gameObject && base.transform.parent && base.gameObject.transform.parent.gameObject && base.gameObject.transform.parent.gameObject.name == "CharacterPad")
            {
                base.StartCoroutine(this.RevolverAnim());
            }
        }

        private IEnumerator RevolverAnim()
        {
            Animator animator = base.gameObject.GetComponent<Animator>();
            EffectManager.SpawnEffect(Resources.Load<GameObject>("prefabs/effects/smokescreeneffect"), new EffectData
            {
                origin = base.gameObject.transform.position
            }, false);
            Util.PlaySound("play_bandit_shift_end", base.gameObject);
            this.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", 1f, animator);
            this.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", 1f, animator);
            yield return new WaitForSeconds(0.48f);
            Util.PlaySound("play_bandit_m1_pump", base.gameObject);
            yield return new WaitForSeconds(0.4f);
            this.PlayAnimation("Gesture, Additive", "PrepRevolver", "PrepRevolver.playbackRate", 0.62f, animator);
            this.PlayAnimation("Gesture, Override", "PrepRevolver", "PrepRevolver.playbackRate", 0.62f, animator);
            Util.PlaySound("play_bandit_m2_load", base.gameObject);
            yield break;
        }

        private void PlayAnimation(string layerName, string animationStateName, string playbackRateParam, float duration, Animator animator)
        {
            int layerIndex = animator.GetLayerIndex(layerName);
            animator.SetFloat(playbackRateParam, 1f);
            animator.PlayInFixedTime(animationStateName, layerIndex, 0f);
            animator.Update(0f);
            float length = animator.GetCurrentAnimatorStateInfo(layerIndex).length;
            animator.SetFloat(playbackRateParam, length / duration);
        }
    }
}

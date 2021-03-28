using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.BanditReloadedSkills
{
    class ExitRevolver : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.duration = this.baseDuration / this.attackSpeedStat;
			if (this.animator)
			{
				this.bodySideWeaponLayerIndex = this.animator.GetLayerIndex("Body, SideWeapon");
				this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 1f);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge > this.duration || (base.characterBody && base.characterBody.isSprinting))
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override void OnExit()
		{
			if (this.animator)
			{
				this.animator.SetLayerWeight(this.bodySideWeaponLayerIndex, 0f);
			}
			base.PlayAnimation("Gesture, Additive", "SideToMain");
			Transform transform = base.FindModelChild("SpinningPistolFX");
			if (transform)
			{
				transform.gameObject.SetActive(false);
			}
			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}

		public float baseDuration = 1.5f;

		protected float duration;

		private Animator animator;

		private int bodySideWeaponLayerIndex;
	}
}

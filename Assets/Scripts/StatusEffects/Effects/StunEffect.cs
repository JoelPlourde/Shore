using System.Collections.Generic;
using UnityEngine;

namespace StatusEffectSystem {
	public class StunEffect : StatusEffect<StunEffect>, IStatusEffect {

		private GameObject _particleSystem;

		public override void Apply(Status status) {
			base.Apply(status);
            status.Creature.Stunned = true;

			// Add the stun particle effect
			status.Creature.Animator.SetBool("Stunned", true);

			if (ReferenceEquals(status.StatusEffectData.ParticleEffectPrefab, null)) {
				Debug.LogWarning("StunEffect: No particle effect prefab assigned in StatusEffectData.");
				return;
			}

			_particleSystem = Object.Instantiate(status.StatusEffectData.ParticleEffectPrefab, status.Creature.transform);
			_particleSystem.transform.position = status.Creature.transform.position + new Vector3(0, status.Creature.Height, 0);
		}

		public override void Unapply(Status status) {
			base.Unapply(status);
            status.Creature.Stunned = false;

			// Remove the stun particle effect
			if (!ReferenceEquals(_particleSystem, null)) {
                Object.Destroy(_particleSystem);
			}

			status.Creature.Animator.SetBool("Stunned", false);
		}
	}
}

using UnityEngine;

namespace StatusEffectSystem {
	[RequireComponent(typeof(Collider))]
	public class StatusEffectZone : MonoBehaviour {

		protected float _magnitude = 1.0f;
		protected StatusEffectData _statusEffectData;

		protected Status _status;
		protected Collider _collider;

		private void Awake() {
			_collider = GetComponent<Collider>();
			_collider.isTrigger = true;
		}

		protected virtual void OnTriggerEnter(Collider other) {
			if (ReferenceEquals(_statusEffectData, null)) {
				return;
			}

			Actor actor = other.gameObject.GetComponent<Actor>();
			if (!ReferenceEquals(actor, null)) {
				_status = new Status(actor, _magnitude, _statusEffectData);
				actor.Creature.StatusEffectScheduler.AddStatusEffect(_status);
			}
		}

		protected virtual void OnTriggerExit(Collider other) {
			if (ReferenceEquals(_statusEffectData, null)) {
				return;
			}

			Actor actor = other.gameObject.GetComponent<Actor>();
			if (!ReferenceEquals(actor, null)) {
				actor.Creature.StatusEffectScheduler.RemoveStatusEffect(_status.StatusEffectData.Name);
			}
		}
	}
}

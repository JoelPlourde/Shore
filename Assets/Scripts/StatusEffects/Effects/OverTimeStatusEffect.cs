using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatusEffectSystem {
	public abstract class OverTimeStatusEffect<T> : SingletonBehaviour<T> where T : MonoBehaviour, IStatusEffect {

		public List<Status> Statuses { get; private set; }

		private bool _started = false;

		protected override void Awake() {
			base.Awake();
			Statuses = new List<Status>();
		}

		public virtual void Apply(Status status) {
			if (!status.Actor.Dead) {
				Statuses.Add(status);

				if (!_started) {
					_started = true;
					InvokeRepeating(nameof(Routine), TickRate, TickRate);
				}
			}
		}

		public virtual void Unapply(Status status) {
			Statuses.RemoveAll(x => x.Guid == status.Guid);
			if (Statuses.Count == 0) {
				_started = false;
				CancelInvoke();
			}
		}

		protected virtual void Routine() {
			foreach (Status status in Statuses) {
				if (!status.Actor.Dead) {
					status.Actor.Creature.SufferDamage(status.DamageCategoryType,status.Magnitude);
				}
			}
		}

		public virtual float TickRate {
			get { return 1f; }
		}
	}
}

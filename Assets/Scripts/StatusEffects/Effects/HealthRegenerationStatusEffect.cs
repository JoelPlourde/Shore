using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatusEffectSystem {
	public class HealthRegenerationStatusEffect : OverTimeStatusEffect<HealthRegenerationStatusEffect>, IStatusEffect {

		protected override void Routine() {
			foreach (Status status in Statuses) {
				if (!status.Actor.Dead) {
					status.Actor.Creature.IncreaseHealth(status.Actor.Attributes.HealthRegeneration);
				}
			}
		}

		public override float TickRate {
			get { return 5f; }
		}
	}
}

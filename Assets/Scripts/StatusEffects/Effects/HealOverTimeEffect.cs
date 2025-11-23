using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatusEffectSystem {
	public class HealOverTimeEffect : OverTimeStatusEffect<HealOverTimeEffect>, IStatusEffect {
	
		protected override void Routine() {
			foreach (Status status in Statuses) {
				if (!status.Actor.Dead) {
					status.Actor.Creature.IncreaseHealth(status.Magnitude);
				}
			}
		}
	}
}

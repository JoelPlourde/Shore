using System.Collections;
using System.Collections.Generic;

namespace StatusEffectSystem {
	public class DamageOverTimeEffect : OverTimeStatusEffect<DamageOverTimeEffect>, IStatusEffect {

		protected override void Routine() {
			foreach (Status status in Statuses) {
				if (!status.Actor.Dead) {
					status.Actor.Creature.SufferDamage(status.DamageCategoryType,status.Magnitude);
				}
			}
		}
	}
}

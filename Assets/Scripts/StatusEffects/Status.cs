using System;
using MonsterSystem;

namespace StatusEffectSystem {
	public class Status {

		public Status(Actor actor, float magnitude, StatusEffectData statusEffectData) {
			Guid = Guid.NewGuid();
			Actor = actor;
			Magnitude = magnitude;
			StatusEffectData = statusEffectData;
		}

		public Status(Actor actor, float magnitude, int duration, StatusEffectData statusEffectData) {
			Guid = Guid.NewGuid();
			Actor = actor;
			Magnitude = magnitude;
			Duration = duration;
			MaxDuration = duration;
			StatusEffectData = statusEffectData;
		}

		public void ResetDuration() {
			Duration = MaxDuration;
		} 

		public int IncreaseDuration(int value) {
			Duration += value;
			return Duration;
		}

		public int ReduceDuration() {
			Duration--;
			return Duration;
		}

		public int IncreaseStack() {
			Stack++;
			return Stack;
		}

		public int DecreaseStack() {
			Stack--;
			return Stack;
		}

		public Actor Actor { get; private set; }
		public StatusEffectData StatusEffectData { get; private set; }
		public DamageCategoryType DamageCategoryType {
			get { return StatusEffectData.DamageCategoryType; }
		}
		public float Magnitude { get; private set; }
		public int MaxDuration { get; private set; }
		public int Duration { get; private set; }
		public int Stack { get; private set; }

		public Guid Guid { get; private set; }
	}
}

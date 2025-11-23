using SaveSystem;

namespace SkillSystem {
	public class Level {

		public int Value { get; private set; }

		public float Experience { get; private set; }

		public Level() {
			Value = 1;
			Experience = 0f;
		}

		public void Initialize(LevelDto levelDto) {
			if (ReferenceEquals(levelDto, null)) {
				return;
			}

			Value = levelDto.Value;
			Experience = levelDto.Experience;
		}

		/// <summary>
		/// Increase the experience.
		/// </summary>
		/// <param name="experience">The amount of experience to increase it by.</param>
		/// <returns>The new total amount of experience.</returns>
		public float IncreaseExperience(float experience) {
			Experience += experience;
			return Experience;
		}

		/// <summary>
		/// Increase the level by 1.
		/// </summary>
		/// <returns>The new level.</returns>
		public int IncreaseLevel() {
			Value++;
			return Value;
		}
	}
}
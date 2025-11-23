using SkillSystem;
using System;
using UnityEngine;

namespace SaveSystem {
	[Serializable]
	public class LevelDto {

		[SerializeField]
		public int Value = 1;

		[SerializeField]
		public float Experience = 0f;

		public LevelDto() {}

		public LevelDto(Level level) {
			Value = level.Value;
			Experience = level.Experience;
		}
	}
}

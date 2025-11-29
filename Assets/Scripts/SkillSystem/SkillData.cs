using System;
using AbilitySystem;
using UnityEngine;

namespace SkillSystem {
	[Serializable]
	[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObjects/Skill Data")]
	public class SkillData : ScriptableObject {

		[SerializeField]
		[Tooltip("The type of this skill (this must be unique)")]
		public SkillType SkillType;

		[SerializeField]
		[Tooltip("The color that represents this skill.")]
		public Color Color;

		[SerializeField]
		[Tooltip("The icon that represents this skill.")]
		public Sprite Icon;

		[SerializeField]
		[Tooltip("Competencies that the character unlocks by leveling up this skill.")]
		public Competency[] Competencies;

		[SerializeField]
		[Tooltip("Abilities that are unlocked by leveling up this skill.")]
		public AbilityData[] Abilities;
	}
}

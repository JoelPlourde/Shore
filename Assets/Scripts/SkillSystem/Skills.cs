using Gameplay;
using CombatSystem;
using SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem {
	public class Skills {

		public Action<SkillType, Level> OnLevelUp;
		public Action<SkillType, Level> OnExperienceGain;

		private Actor _actor;
		private Dictionary<SkillType, Level> _skills = new Dictionary<SkillType, Level>();

		/// <summary>
		/// Initialize the Skills from the saved information.
		/// </summary>
		/// <param name="skillsDto">The skills to be loaded.</param>
		public void Initialize(Actor actor, SkillsDto skillsDto) {
			_actor = actor;
			_skills = new Dictionary<SkillType, Level>();

			foreach (SkillType skillType in Enum.GetValues(typeof(SkillType))) {
				_skills.Add(skillType, new Level());
				if (skillsDto.levelDtos.Length <= (int) skillType) {
					Debug.Log("This new skill has been added: " + skillType);
					continue;
				}
				_skills[skillType].Initialize(skillsDto.levelDtos[(int) skillType]);
			}
		}

		/// <summary>
		/// Get the Level by its skill type.
		/// </summary>
		/// <param name="skillType">A Skill Type</param>
		/// <returns>The level for the respective skill.</returns>
		public Level GetLevel(SkillType skillType) {
			if (_skills.TryGetValue(skillType, out Level level)) {
				return level;
			} else {
				throw new UnityException("Please define this Skill Type: " + skillType);
			}
		}

		public void GainExperience(DamageCategoryType damageCategoryType, float experience) {
			if (damageCategoryType == DamageCategoryType.TYPELESS)
			{
				return;
			}

			SkillType skillType = GetSkillTypeFromDamageCategoryType(damageCategoryType);
			GainExperience(skillType, experience);
		}

		/// <summary>
		/// Gain experience in a respective skill.
		/// </summary>
		/// <param name="skillType">The skill</param>
		/// <param name="experience">The amount of experience</param>
		public void GainExperience(SkillType skillType, float experience) {
			
			FeedbackManager.Instance.DisplayExperienceGain(_actor, skillType, (int) experience);

			if (_skills[skillType].IncreaseExperience(experience) >= ExperienceTable.GetExperienceRequiredAt(GetLevel(skillType).Value + 1)) {
				_skills[skillType].IncreaseLevel();
				OnLevelUp?.Invoke(skillType, _skills[skillType]);
			}
			OnExperienceGain?.Invoke(skillType, _skills[skillType]);
		}

		private SkillType GetSkillTypeFromDamageCategoryType(DamageCategoryType damageCategoryType)
        {
            switch (damageCategoryType)
			{
				case DamageCategoryType.MELEE:
					return SkillType.FIGHTING;
				case DamageCategoryType.MAGIC:
					throw new NotImplementedException("Magic skill type is not implemented yet.");
				case DamageCategoryType.RANGED:
					throw new NotImplementedException("Ranged skill type is not implemented yet.");
				default:
					throw new NotImplementedException("Unknown DamageCategoryType: " + damageCategoryType);
			}
        }

		public IReadOnlyDictionary<SkillType, Level> GetSkillLevels => _skills;
	}
}

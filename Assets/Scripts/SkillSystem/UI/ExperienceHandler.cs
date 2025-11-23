using System;
using UI;
using UnityEngine;

namespace SkillSystem {
	namespace UI {
		[RequireComponent(typeof(Canvas))]
		public class ExperienceHandler : MonoBehaviour, IMenu {

			public static ExperienceHandler Instance;

			private SkillComponent[] _skillComponents;

			private void Awake() {
				Instance = this;

				Canvas = GetComponent<Canvas>();
				_skillComponents = GetComponentsInChildren<SkillComponent>();
			}

			public void Open(Actor actor) {
				foreach (SkillType skillType in Enum.GetValues(typeof(SkillType))) {
					_skillComponents[(int) skillType].Initialize(SkillManager.Instance.GetSkillData(skillType), actor.Skills.GetSkillLevels[skillType], delegate { OnClick(skillType, actor); });
				}

				actor.Skills.OnLevelUp += OnLevelUp;
				actor.Skills.OnExperienceGain += OnExperienceGain;

				Canvas.enabled = true;
			}

			public void Close(Actor actor) {
				Canvas.enabled = false;

				actor.Skills.OnLevelUp -= OnLevelUp;
				actor.Skills.OnExperienceGain -= OnExperienceGain;
			}

			private void OnClick(SkillType skillType, Actor actor) {
				SkillHandler.Instance.Open(skillType, actor);
			}

			/// <summary>
			/// Callback whenever an Actor levels up in any of its skills.
			/// </summary>
			/// <param name="skillType">The type of the skill that leveled up.</param>
			/// <param name="level">The updated level information.</param>
			private void OnLevelUp(SkillType skillType, Level level) {
				_skillComponents[(int) skillType].TextComponent.Refresh(level);
			}

			/// <summary>
			/// Callback whenever an actor gain experience in any of its skills.
			/// </summary>
			/// <param name="skillType">The type of the skill that gained experience in.</param>
			/// <param name="level">The update level information.</param>
			private void OnExperienceGain(SkillType skillType, Level level) {
				_skillComponents[(int)skillType].TextComponent.OnGainExperience(level);
				_skillComponents[(int)skillType].ExperienceComponent.OnUpdateExperience(level.Experience);
			}

			public Canvas Canvas { get; set; }
		}
	}
}

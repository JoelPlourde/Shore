using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SkillSystem {
	namespace UI {
		[RequireComponent(typeof(Button))]
		public class SkillComponent : MonoBehaviour {

			private bool _initialized = false;

			private void Awake() {
				IconComponent = transform.GetComponentInChildren<SkillIconComponent>();
				if (ReferenceEquals(IconComponent, null)) {
					throw new UnityException("Please verify that this component has an IconComponent as his children.");
				}

				TextComponent = transform.GetComponentInChildren<SkillTextComponent>();
				if (ReferenceEquals(TextComponent, null)) {
					throw new UnityException("Please verify that this component has an TextComponent as his children.");
				}

				ExperienceComponent = transform.GetComponentInChildren<SkillExperienceComponent>();
				if (ReferenceEquals(ExperienceComponent, null)) {
					throw new UnityException("Please verify that this component has an ExperienceComponent as his children.");
				}
			}

			public void Initialize(SkillData skillData, Level level, UnityAction callback) {
				if (!_initialized) {
					GetComponent<Button>().onClick.AddListener(callback);
					IconComponent.Initialize(skillData);
					TextComponent.Initialize(level);
					ExperienceComponent.Initialize(skillData, level);
					_initialized = true;
				}
				TextComponent.Refresh(level);
			}

			private void Destroy() {
				GetComponent<Button>().onClick.RemoveAllListeners();
			}

			public SkillIconComponent IconComponent { get; private set; }
			public SkillTextComponent TextComponent { get; private set; }
			public SkillExperienceComponent ExperienceComponent { get; set; }
		}
	}
}

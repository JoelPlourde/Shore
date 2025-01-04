using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SkillSystem {
	namespace UI {
		[RequireComponent(typeof(Button))]
		public class CompetencyComponent : MonoBehaviour {

			private Image _icon;
			private Text _requirement;
			private Text _description;
			private Button _button;

			private void Awake() {
				_icon = transform.Find("Icon").GetComponent<Image>();
				_requirement = transform.Find("Requirement").GetComponent<Text>();
				_description = transform.Find("Description").GetComponent<Text>();
				_button = transform.GetComponent<Button>();
			}

			public void Enable(SkillType skillType, Competency competency, Level level, UnityAction<SkillType, CompetencyComponent, Competency> callback) {
				_icon.sprite = competency.Icon;
				_requirement.text = competency.Requirement.ToString();
				_description.text = I18N.GetValue("skills." + EnumExtensions.FormatEnum(skillType.ToString() + ".competencies." + competency.Descriptive + ".name"));
				if (level.Value >= competency.Requirement) {
					_icon.sprite = competency.Icon;
				} else {
					_icon.sprite = IconManager.Instance.GetSprite("lock");
				}
				_button.onClick.RemoveAllListeners();
				_button.onClick.AddListener(delegate { callback(skillType, this, competency); });
			}

			private void Destroy() {
				_button.onClick.RemoveAllListeners();
			}
		}
	}
}
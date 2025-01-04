using UnityEngine;
using UnityEngine.UI;

namespace SkillSystem {
	namespace UI {
		public class CompetencyDescription : MonoBehaviour {
			private Text _text;
			private RectTransform _rectTransform;

			private int _index = -1;
			private Vector2 _sizeDelta;

			private void Awake() {
				_rectTransform = GetComponent<RectTransform>();
				_text = GetComponentInChildren<Text>();
			}

			public void Open(SkillType skillType, CompetencyComponent competencyComponent, Competency competency) {
				// Set the description as the last sibling initially.
				transform.SetAsLastSibling();

				int siblingIndex = competencyComponent.transform.GetSiblingIndex();
				if (_index == siblingIndex) {
					_index = -1;
					Close();
					return;
				}

				transform.SetSiblingIndex(siblingIndex + 1);
				_index = siblingIndex;

				// TODO implement Tag
				_text.text = I18N.GetValue("skills." + EnumExtensions.FormatEnum(skillType.ToString() + ".competencies." + competency.Descriptive + ".description"));

				_sizeDelta = _rectTransform.sizeDelta;
				_sizeDelta.y = _text.preferredHeight + 20f;
				_rectTransform.sizeDelta = _sizeDelta;
			}

			public void Close() {
				gameObject.SetActive(false);
			}
		}
	}
}

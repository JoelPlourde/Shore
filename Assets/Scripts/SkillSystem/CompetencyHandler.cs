using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem {
	namespace UI {
		public class CompetencyHandler : MonoBehaviour {

			private RectTransform _rectTransform;

			private GameObject _template;

			private List<CompetencyComponent> _competencyComponents;
			private CompetencyDescription _competencyDescription;

			private void Awake() {
				_rectTransform = GetComponent<RectTransform>();

				_template = transform.GetChild(0).gameObject;
				_competencyComponents = new List<CompetencyComponent> {
					GetComponentInChildren<CompetencyComponent>()
				};
				_competencyDescription = GetComponentInChildren<CompetencyDescription>(true);
			}

			public void Initialize(SkillData skillData, Skills skills) {
				int diff = (transform.childCount - 1) - skillData.Competencies.Length;
				// Spawn the Competency rows missing, if any.
				while (diff < 0) {
					_competencyComponents.Add(Instantiate(_template, transform).GetComponent<CompetencyComponent>());
					diff++;
				}

				Level level = skills.GetLevel(skillData.SkillType);

				// Enable all the rows you need.
				int i;
				for (i = 0; i < skillData.Competencies.Length; i++) {
					_competencyComponents[i].gameObject.SetActive(true);
					_competencyComponents[i].Enable(skillData.SkillType, skillData.Competencies[i], level, OnClick);
				}

				// Disable those which you do not need.
				for (int j = i; j < _competencyComponents.Count; j++) {
					_competencyComponents[j].gameObject.SetActive(false);
				}

				Vector2 size = _rectTransform.sizeDelta;
				size.y = i * 50f; // 50f is just an arbitrary size.
				_rectTransform.sizeDelta = size;
			}

			public void OnClick(SkillType skillType, CompetencyComponent competencyComponent, Competency competency) {
				_competencyDescription.gameObject.SetActive(true);
				_competencyDescription.Open(skillType, competencyComponent, competency);
			}
		}
	}
}

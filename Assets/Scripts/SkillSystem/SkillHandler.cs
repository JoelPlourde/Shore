using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SkillSystem {
	namespace UI {
		[RequireComponent(typeof(Canvas))]
		public class SkillHandler : MonoBehaviour {

			public static SkillHandler Instance;

			private Image _icon;
			private TextMeshProUGUI _title;
			private Button _exit;
			private Canvas _canvas;

			private CompetencyHandler _compentencyHandler;
			private Scrollbar _scrollBar;

			private void Awake() {
				Instance = this;

				_canvas = GetComponent<Canvas>();
				_icon = transform.Find("Icon/Image").GetComponent<Image>();
				_title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
				_exit = transform.Find("Exit").GetComponent<Button>();
				_compentencyHandler = transform.Find("Scroll View/Viewport/CompetencyHandler").GetComponent<CompetencyHandler>();
				_scrollBar = transform.Find("Scroll View/Scrollbar Vertical").GetComponent<Scrollbar>();

				_exit.onClick.AddListener(Close);
			}

			/// <summary>
			/// On destroy, unsubscribe to callbacks.
			/// </summary>
			private void Destroy() {
				if (!ReferenceEquals(_exit, null)) {
					_exit.onClick.RemoveAllListeners();
				}
			}

			/// <summary>
			/// Open the Skill information menu by providing a skill type and an actor.
			/// </summary>
			/// <param name="skillType">The skill type</param>
			/// <param name="actor">The actor that has opened the skill.</param>
			public void Open(SkillType skillType, Actor actor) {
				SkillData skillData = SkillManager.Instance.GetSkillData(skillType);

				_title.text = I18N.GetValue("skills." + skillType + ".name");
				_icon.sprite = skillData.Icon;

				_compentencyHandler.Initialize(skillData, actor.Skills);

				// Scroll the scrollbar to the last unlocked competency.
				Level level = actor.Skills.GetLevel(skillType);
				int i;
				for (i = 0; i < skillData.Competencies.Length; i++) {
					if (level.Value < skillData.Competencies[i].Requirement) {
						break;
					}
				}

				_scrollBar.numberOfSteps = skillData.Competencies.Length;
				_scrollBar.value = 1 - (i / (float) skillData.Competencies.Length);

				_canvas.enabled = true;
			}

			/// <summary>
			/// Close the Skill UI by clicking on the Exit button.
			/// </summary>
			public void Close() {
				_canvas.enabled = false;
			}
		}
	}
}

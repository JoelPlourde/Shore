using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SkillSystem {
	namespace UI {
		[RequireComponent(typeof(Text))]
		public class SkillTextComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

			private Text _text;

			private string _tooltip = "";

			private void Awake() {
				_text = GetComponent<Text>();

				_text.color = Color.black;
				_text.fontSize = 50;
				_text.alignment = TextAnchor.MiddleCenter;
			}

			/// <summary>
			/// Initialize the component with the initial information of the level.
			/// </summary>
			/// <param name="level">The level.</param>
			public void Initialize(Level level) {
				OnGainExperience(level);
				Refresh(level);
			}

			/// <summary>
			/// Method called whenever the Experience menu receives the callback that the actor level up.
			/// </summary>
			/// <param name="level"></param>
			public void Refresh(Level level) {
				_text.text = level.Value.ToString();
			}

			/// <summary>
			/// Method called whenever the Experience menu receives the callback that the actor gains experience.
			/// </summary>
			/// <param name="level">The updated level information.</param>
			public void OnGainExperience(Level level) {
				string experienceLabel = I18N.GetValue("experience");
				string requiredLabel = I18N.GetValue("required");
				_tooltip = experienceLabel + ": " + level.Experience + "\n" + requiredLabel + ": " + ((level.Value > 100) ? "N/A" : ExperienceTable.GetExperienceRequiredAt(level.Value + 1).ToString());
			}

			public void OnPointerEnter(PointerEventData eventData) {
				Tooltip.Instance.ShowTooltip(_tooltip, Constant.TOOLTIP_DELAY);
			}

			public void OnPointerExit(PointerEventData eventData) {
				Tooltip.Instance.HideTooltip();
			}
		}
	}
}
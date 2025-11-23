using ItemSystem.UI;
using SkillSystem.UI;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
	/// <summary>
	/// The Menu Bar is a bar specific to Actor-related menu.
	/// </summary>
	public class ActorMenuBar : BaseMenuBar {

		public static ActorMenuBar Instance;

		private RectTransform _rectTransform;

		private new void Awake() {
			Instance = this;

			_rectTransform = GetComponent<RectTransform>();

			base.Awake();

			UserInputs.Instance.Subscribe(_menuButtons[(int) MenuType.ABILITIES].GetKeyCode(), delegate { OnClick((int) MenuType.ABILITIES); });
			UserInputs.Instance.Subscribe(_menuButtons[(int) MenuType.INVENTORY].GetKeyCode(), delegate { OnClick((int) MenuType.INVENTORY); });
			UserInputs.Instance.Subscribe(_menuButtons[(int) MenuType.EQUIPMENT].GetKeyCode(), delegate { OnClick((int) MenuType.EQUIPMENT); });
			UserInputs.Instance.Subscribe(_menuButtons[(int) MenuType.EXPERIENCE].GetKeyCode(), delegate { OnClick((int) MenuType.EXPERIENCE); });

			_interfaceStatus = new Dictionary<MenuType, IMenu>() {
				{ MenuType.ABILITIES, AbilityHandler.Instance },
				{ MenuType.INVENTORY, InventoryHandler.Instance},
				{ MenuType.EQUIPMENT, EquipmentHandler.Instance },
				{ MenuType.EXPERIENCE, ExperienceHandler.Instance }
			};
		}

		private void SubscribeToUserInputs() {
			UserInputs.Instance.Subscribe(_menuButtons[(int)MenuType.ABILITIES].GetKeyCode(), delegate { OnClick((int)MenuType.ABILITIES); });
			UserInputs.Instance.Subscribe(_menuButtons[(int)MenuType.INVENTORY].GetKeyCode(), delegate { OnClick((int)MenuType.INVENTORY); });
			UserInputs.Instance.Subscribe(_menuButtons[(int)MenuType.EQUIPMENT].GetKeyCode(), delegate { OnClick((int)MenuType.EQUIPMENT); });
			UserInputs.Instance.Subscribe(_menuButtons[(int)MenuType.EXPERIENCE].GetKeyCode(), delegate { OnClick((int)MenuType.EXPERIENCE); });
		}

		private void UnsubscribeToUserInputs() {
			UserInputs.Instance.Unsubscribe(_menuButtons[(int)MenuType.ABILITIES].GetKeyCode());
			UserInputs.Instance.Unsubscribe(_menuButtons[(int)MenuType.INVENTORY].GetKeyCode());
			UserInputs.Instance.Unsubscribe(_menuButtons[(int)MenuType.EQUIPMENT].GetKeyCode());
			UserInputs.Instance.Unsubscribe(_menuButtons[(int)MenuType.EXPERIENCE].GetKeyCode());
		}

		/// <summary>
		/// Show the Actor menu bar.
		/// </summary>
		public void ShowActorMenuBar() {
			LeanTween.moveY(_rectTransform, 0, 0.25f);
		}

		/// <summary>
		/// Hide the Actor menu bar.
		/// </summary>
		public void HideActorMenuBar() {
			if (CurrentMenu != MenuType.NONE) {
				ToggleInterface(CurrentMenu);

				CurrentMenu = MenuType.NONE;
			}

			LeanTween.moveY(_rectTransform, -_rectTransform.sizeDelta.y, 0.25f);
		}

		protected override void ToggleInterface(MenuType index) {
			if (index == MenuType.NONE) {
				return;
			}

			if (Squad.Any(out Actor actor)) {
				ToggleMenu(_interfaceStatus[index], actor);
			}
		}
	}
}

using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace ItemSystem {
	namespace UI {
		[RequireComponent(typeof(Canvas))]
		[RequireComponent(typeof(GridLayoutGroup))]
		public class InventoryHandler : BaseHandler, IMenu {

			public static InventoryHandler Instance;

			public GameObject SlotTemplate;

			private SlotHandler[] _slots = new SlotHandler[Inventory.MAX_STACK];

			private void Awake() {
				Canvas = GetComponent<Canvas>();
				Instance = this;

				GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
				gridLayoutGroup.cellSize = new Vector2(65, 65);
				gridLayoutGroup.spacing = new Vector2(5, 5);
				gridLayoutGroup.padding.left = 15;
				gridLayoutGroup.padding.top = 15;
		
				RectTransform rectTransform = GetComponent<RectTransform>();
				rectTransform.pivot = new Vector2(1, 0);
				rectTransform.sizeDelta = new Vector2(305, 380);

				for (int i = 0; i < Inventory.MAX_STACK; i++) {
					GameObject slotHandlerObj = Instantiate(SlotTemplate, transform);
					slotHandlerObj.name = "Slot" + i;
					_slots[i] = slotHandlerObj.GetComponent<SlotHandler>();
					_slots[i].Initialize(this, false, i);
					_slots[i].Disable();
				}

				_interactable = true;
			}

			public void Open(Actor actor) {
				Initialize(actor.Inventory);

				actor.Inventory.OnDirtyItemsEvent += OnDirtyItemsEvent;
				actor.Inventory.OnRedrawEvent += RedrawEvent;

				RedrawEvent(actor.Inventory.Items);

				Canvas.enabled = true;
			}

			public void Close(Actor actor) {
				actor.Inventory.OnDirtyItemsEvent -= OnDirtyItemsEvent;
				actor.Inventory.OnRedrawEvent -= RedrawEvent;

				foreach (SlotHandler slot in _slots) {
					slot.Disable();
				}

				Canvas.enabled = false;
			}

			public Canvas Canvas { get; set; }
		}
	}
}

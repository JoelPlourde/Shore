using ItemSystem.EffectSystem;
using ItemSystem.EquipmentSystem;
using PointerSystem;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ItemSystem {
	namespace UI {
		[RequireComponent(typeof(Image))]
		public class SlotHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

			public event Action OnBeginDragEvent;
			public event Action OnEndDragEvent;

			private void Awake() {
				Image = transform.Find("Item").GetComponent<Image>();
				Amount = GetComponentInChildren<Text>();

				Image.rectTransform.sizeDelta = new Vector2(55, 55);
			}

			public void Initialize(IContainer container, bool readOnly, int id) {
				Container = container;
				ReadOnly = readOnly;
				ID = id;
			}

			public void Refresh(Item item) {
				if (ReferenceEquals(item, null)) {
					Disable();
					return;
				}

				Item = item;
				Image.sprite = item.ItemData.Sprite;
				Amount.text = (item.Amount > 1) ? item.Amount.ToString() : "";
				Enable();
			}

			public void Enable() {
				Image.enabled = true;
				Amount.enabled = true;
			}

			public void Disable() {
				Image.enabled = false;
				Amount.enabled = false;

				Item = null;
			}

			#region Pointer
			public void OnPointerEnter(PointerEventData eventData) {
				if (!ReferenceEquals(Item, null)) {
					string tooltip = "";
					string effectName;
					if (Item.ItemData.ItemEffects.Count > 0) {
						if (string.IsNullOrEmpty(Item.ItemData.ItemEffects[0].effectName)) {
							effectName = EnumExtensions.FormatItemEffectType(Item.ItemData.ItemEffects[0].ItemEffectType);
						} else {
							effectName = Item.ItemData.ItemEffects[0].effectName;
						}
						tooltip += string.Format("<color=#BFBFBF><size=20>{0}</size></color> ", I18N.GetValue("effects." + effectName));
					}

					string itemName = I18N.GetValue("items." + Item.ItemData.ID + ".name");
                    string itemType;
					string statistics = "";

                    if (Item.ItemData is EquipmentData equipmentData) {
                        itemType = I18N.GetValue("equipments." + EnumExtensions.FormatEquipmentType(equipmentData));

                        int count = equipmentData.EquipmentStats.Statistics.Count;
                        foreach (Statistic statistic in equipmentData.EquipmentStats.Statistics) {
							if (statistic.StatisticType == StatisticType.NONE) {
								continue;
							}

                            string statisticType = I18N.GetValue("statistics." + EnumExtensions.FormatEnum(statistic.StatisticType.ToString()) + ".name");
                            statistics += string.Format("<size=16><color=#1DFF00>+{0} {1}</color></size>", statistic.Value, statisticType);
                            count--;
                            if (count > 0) {
                                statistics += "\n";
                            }
                        }
                    }
                    else {
                        itemType = I18N.GetValue(EnumExtensions.FormatItemType(Item.ItemData.ItemType));
                    }

                    tooltip += string.Format("<size=22>{0}</size>\n<size=16>{1}</size>", itemName, itemType);
					tooltip += string.IsNullOrEmpty(statistics) ? "" : "\n" + statistics;

					Tooltip.Instance.ShowTooltip(tooltip, Constant.TOOLTIP_DELAY);
				} else {
					Tooltip.Instance.HideTooltip();
				}
			}

			public void OnPointerExit(PointerEventData eventData) {
				if (!ReferenceEquals(Item, null)) {
					Tooltip.Instance.HideTooltip();
				}
			}

			public void OnPointerUp(PointerEventData eventData) {
				PointerManager.Instance.SetPointer(PointerMode.DEFAULT);

				if (_dragging) {
					return;
				}

				if (Input.GetMouseButtonUp(1)) {
					OptionsHandler.Instance.Open(this);
					return;
				}

				if (HasItem) {
					if (Squad.FirstSelected(out Actor actor)) {
						if (ReadOnly) {
							if (InventoryUtils.TransferItem(Container.Inventory, actor.Inventory, Item.ItemData, Item.Amount)) {
								// TODO Give Experience based on the Recipe.

								Tooltip.Instance.HideTooltip();

								OnEndDragEvent?.Invoke();
							}
						} else {
							ItemEffectFactory.Activate(actor, Item);

							if (!HasItem) {
								Tooltip.Instance.HideTooltip();
							}
						}
					}
				}

				ItemSelector.UnselectItem();
			}

			public void OnPointerDown(PointerEventData eventData) {
				PointerManager.Instance.SetPointer(PointerMode.DEFAULT_PRESSED);
			}
			#endregion

			#region Drag
			private bool _dragging = false;

			public void OnBeginDrag(PointerEventData eventData) {
				if (ReadOnly) {
					return;
				}

				if (ReferenceEquals(Item, null)) {
					return;
				}

				_dragging = true;

				Image.transform.SetParent(transform.root);
				Amount.transform.SetParent(Image.transform);

				OnBeginDragEvent?.Invoke();
			}

			public void OnDrag(PointerEventData eventData) {
				if (ReadOnly) {
					return;
				}

				Image.rectTransform.position = Input.mousePosition;
			}

			public void OnEndDrag(PointerEventData eventData) {
				if (ReadOnly) {
					return;
				}

				_dragging = false;

				var raycastResults = new List<RaycastResult>();
				EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = Input.mousePosition }, raycastResults);

				if (raycastResults.Count > 0) {
					bool handle = false;
					foreach (var result in raycastResults) {
						SlotHandler destinationSlot = result.gameObject.GetComponent<SlotHandler>();
						if (!ReferenceEquals(destinationSlot, null)) {

							if (ReferenceEquals(destinationSlot, this)) {
								ReplaceImage(eventData.pointerDrag.transform);
								handle = true;
								break;
							}

							if (ReferenceEquals(Container, destinationSlot.Container)) {
								Container.SwapItems(Item.Index, destinationSlot.ID);
							} else {
								InventoryUtils.SwapItemsBetweenContainers(Item, Container, destinationSlot.Container, Item.Index, destinationSlot.ID);
							}

							ReplaceImage(eventData.pointerDrag.transform);
							handle = true;
							break;
						}
					}

					if (!handle) {
						ReplaceImage(eventData.pointerDrag.transform);

						Ray ray = Camera.main.ScreenPointToRay(eventData.position);
						if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain"))) {
							if (Squad.FirstSelected(out Actor actor)) {
								if (ItemManager.Instance.PlaceItemInWorld(Item, hit.point, Quaternion.identity)) {
									if (!actor.Inventory.RemoveItemFromInventoryAtPosition(Item.Index, Item.Amount)) {
										Debug.Log("Failed to remove item from inventory!");
									}
								} else {
									Debug.Log("Failed to place item in world!");
								}
							} else {
								Debug.Log("No actor selected!");
							}
						} else {
							Debug.Log("Failed to raycast!");
						}
					}

					OnEndDragEvent?.Invoke();
				}
			}
			#endregion

			private void ReplaceImage(Transform parent) {
				Image.transform.SetParent(parent);
				Image.transform.SetAsFirstSibling();
				Image.rectTransform.sizeDelta = new Vector2(45, 45);
				Image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				Image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				Image.rectTransform.localPosition = Vector2.zero;

				Amount.rectTransform.SetParent(parent, true);
			}

			public Image Image { get; private set; }
			public Text Amount { get; private set; }

			public Item Item { get; private set; }
			public IContainer Container { get; private set; }

			public bool ReadOnly { get; private set; }
			public int ID { get; private set; }
			public bool HasItem { get { return !ReferenceEquals(Item, null); } }
		}
	}
}

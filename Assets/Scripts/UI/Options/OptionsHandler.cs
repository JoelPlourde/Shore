using UnityEngine;
using ItemSystem;
using System.Collections.Generic;
using ItemSystem.UI;
using ItemSystem.EffectSystem;
using UnityEngine.Events;
using UnityEngine.UI;
using Gameplay;
using System;
using NodeSystem;
using CombatSystem;

namespace UI
{
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(GridLayoutGroup))]
	public class OptionsHandler : MonoBehaviour, IUpdatable
	{

		public static OptionsHandler Instance;

		public GameObject OptionPrefab;

		[Header("Configuration")]
		[Tooltip("Number of options that should be pre-instantiated at runtime.")]
		public int PrebatchOptionsCount = 10;

		private List<OptionComponent> _options = new List<OptionComponent>();

		private Canvas _canvas;
		private RectTransform _rectTransform;
		private GridLayoutGroup _gridLayoutGroup;

		// Sizing
		private int _childCount;
		private Vector2 _relativePosition;
		private Vector2 _relativeSizeDelta;
		private Vector2 _padding = new Vector2(15, 15);
		private Vector2 _sizeDelta = Vector2.zero;

		private void Awake()
		{
			Instance = this;

			_gridLayoutGroup = GetComponent<GridLayoutGroup>();
			_rectTransform = GetComponent<RectTransform>();
			_canvas = GetComponent<Canvas>();

			_canvas.enabled = false;

			for (int i = 0; i < PrebatchOptionsCount; i++)
			{
				_options.Add(Instantiate(OptionPrefab, transform).GetComponent<OptionComponent>());
				_options[i].Disable();
			}
		}

		/// <summary>
		/// Open the Options menu specific for an item.
		/// </summary>
		/// <param name="slotHandler">The slothandler where the Item is located.</param>
		public void Open(SlotHandler slotHandler)
		{
			if (!slotHandler.HasItem)
			{
				return;
			}

			// Hide the tooltip
			Tooltip.Instance.HideTooltip();

			string translatedItemName = I18N.GetValue("items." + slotHandler.Item.ItemData.ID + ".name");

			_childCount = 0;
			float preferredWidth = 0;
			foreach (var itemEffect in slotHandler.Item.ItemData.ItemEffects)
			{
				string effectName;
				if (string.IsNullOrEmpty(itemEffect.effectName))
				{
					effectName = EnumExtensions.FormatItemEffectType(itemEffect.ItemEffectType);
				}
				else
				{
					effectName = itemEffect.effectName;
				}

				AddOption(I18N.GetValue("effects." + effectName), translatedItemName, ref preferredWidth, delegate { OnItemEffectClicked(slotHandler.Item); });
			}

			// Set default option on items.
			AddOption(I18N.GetValue("select"), translatedItemName, ref preferredWidth, delegate { OnItemSelect(slotHandler); });
			AddOption(I18N.GetValue("drop"), translatedItemName, ref preferredWidth, delegate { OnItemDropped(slotHandler); });
			AddOption(I18N.GetValue("examine"), "", ref preferredWidth, delegate { OnItemExamine(slotHandler.Item.ItemData); });
			AddOption(I18N.GetValue("cancel"), "", ref preferredWidth, Close);

			// Disable all the unused options!
			DisableUnusedOptions();

			// Set the position of the Options menu.
			_rectTransform.position = slotHandler.Image.rectTransform.position;

			ResizeComponent(_childCount, preferredWidth);

			transform.SetAsLastSibling();

			_canvas.enabled = true;

			GameController.Instance.RegisterUpdatable(this);
		}

		/// <summary>
		/// Open the right-click menu specific to a Node.
		/// </summary>
		/// <param name="interactable"></param>
		/// <param name="nodeData"></param>
		/// <param name="callback"></param>
		public void OpenRightClickMenu(IInteractable interactable, NodeData nodeData, Action callback)
		{
			string nodeName = I18N.GetValue("nodes." + nodeData.ID + ".name");
			OpenRightClickMenu(interactable, nodeName, nodeData, callback);
		}

		/// <summary>
		/// Open the right-click menu specific to an Item.
		/// </summary>
		/// <param name="interactable"></param>
		/// <param name="itemData"></param>
		/// <param name="callback"></param>
		public void OpenRightClickMenu(IInteractable interactable, ItemData itemData, Action callback)
		{
			string itemName = I18N.GetValue("items." + itemData.ID + ".name");
			OpenRightClickMenu(interactable, itemName, itemData, callback);
		}

		/// <summary>
		/// Open the right-click menu specific to a Monster.
		/// </summary>
		/// <param name="interactable"></param>
		/// <param name="monsterData"></param>
		/// <param name="callback"></param>
		public void OpenRightClickMenu(IInteractable interactable, MonsterData monsterData, Action callback)
		{
			string monsterName = I18N.GetValue("monsters." + monsterData.ID + ".name");
			OpenRightClickMenu(interactable, monsterName, monsterData, callback);
		}

		/// <summary>
		/// Open the right-click menu with the given parameters.
		/// </summary>
		/// <param name="interactable"></param>
		/// <param name="name"></param>
		/// <param name="data"></param>
		/// <param name="callback"></param>
		private void OpenRightClickMenu(IInteractable interactable, string name, object data, Action callback)
		{
			float preferredWidth = 0;

			string defaultAction = I18N.GetValue(interactable.GetDefaultAction());

			AddOption(defaultAction, name, ref preferredWidth, delegate { callback(); });
			if (data is ItemData itemData)
			{
				AddOption(I18N.GetValue("examine"), "", ref preferredWidth, delegate { OnItemExamine(itemData); });
			}
			else if (data is NodeData nodeData)
			{
				AddOption(I18N.GetValue("examine"), "", ref preferredWidth, delegate { OnNodeExamine(nodeData); });
			}
			else if (data is MonsterData monsterData)
			{
				AddOption(I18N.GetValue("examine"), "", ref preferredWidth, delegate { OnMonsterExamine(monsterData); });
			}
			
			AddOption(I18N.GetValue("cancel"), "", ref preferredWidth, Close);

			// Disable all the unused options!
			DisableUnusedOptions();

			// Convert from world position to screen position:
			_rectTransform.position = Input.mousePosition;

			ResizeComponent(_childCount, preferredWidth);

			transform.SetAsLastSibling();

			_canvas.enabled = true;

			GameController.Instance.RegisterUpdatable(this);
		}

		private void AddOption(string action, string message, ref float preferredWidth, UnityAction unityAction)
		{
			if (_options.Count <= _childCount)
			{
				_options.Add(Instantiate(OptionPrefab, transform).GetComponent<OptionComponent>());
			}
			_options[_childCount++].Enable(action, message, ref preferredWidth, unityAction);
		}

		private void DisableUnusedOptions()
		{
			for (int i = _childCount; i < _options.Count; i++)
			{
				_options[i].Disable();
			}
		}

		void IUpdatable.OnUpdate()
		{
			_relativePosition = Input.mousePosition - _rectTransform.position;
			if (_relativePosition.y >= _padding.y || _relativePosition.y < -_relativeSizeDelta.y)
			{
				Close();
			}
			else if (_relativePosition.x < -_relativeSizeDelta.x || _relativePosition.x >= _relativeSizeDelta.x)
			{
				Close();
			}
		}

		/// <summary>
		/// Close the Options menu, disable the routine, if any.
		/// </summary>
		public void Close()
		{
			GameController.Instance.DeregisterUpdatable(this);
			_canvas.enabled = false;

			// Disable all the options.
			foreach (var option in _options)
			{
				option.Disable();
			}
			_childCount = 0;
		}

		/// <summary>
		/// Event triggered whenever the Item Effect option is activated.
		/// </summary>
		/// <param name="item">The item to activate the item effect on.</param>
		public void OnItemEffectClicked(Item item)
		{
			if (Squad.FirstSelected(out Actor actor))
			{
				ItemEffectFactory.Activate(actor, item);
				Close();
			}
		}

		/// <summary>
		/// Event triggered whenever the Select option is activated.
		/// </summary>
		/// <param name="slotHandler">The SlotHandler to select.</param>
		public void OnItemSelect(SlotHandler slotHandler)
		{
			ItemSelector.SelectItem(slotHandler);
			Close();
		}

		/// <summary>
		/// Event triggered whenever the Drop option is activated.
		/// </summary>
		public void OnItemDropped(SlotHandler slotHandler)
		{
			if (Squad.FirstSelected(out Actor actor))
			{
				if (ItemManager.Instance.PlaceItemInWorld(slotHandler.Item, actor.transform.position + actor.transform.forward, Quaternion.identity))
				{
					actor.Inventory.RemoveItemFromInventoryAtPosition(slotHandler.Item.Index, slotHandler.Item.Amount);
				}
			}
			Close();
		}

		/// <summary>
		/// Method called whenever the option "examine" is clicked on an item.
		/// </summary>
		/// <param name="itemData"></param>
		public void OnItemExamine(ItemData itemData)
		{
			string description = I18N.GetValue("items." + itemData.ID + ".description");
			MessageHandler.Instance.ShowMessage(description);
			Close();
			DisableUnusedOptions();
		}

		/// <summary>
		/// Method called whenever the option "examine" is clicked on a node.
		/// </summary>
		/// <param name="nodeData"></param>
		public void OnNodeExamine(NodeData nodeData)
		{
			string description = I18N.GetValue("nodes." + nodeData.ID + ".description");
			MessageHandler.Instance.ShowMessage(description);
			Close();
			DisableUnusedOptions();
		}

		/// <summary>
		/// Method called whenever the option "examine" is clicked on a monster.
		/// </summary>
		/// <param name="monsterData"></param>
		public void OnMonsterExamine(MonsterData monsterData)
		{
			string description = I18N.GetValue("monsters." + monsterData.ID + ".description");
			MessageHandler.Instance.ShowMessage(description);
			Close();
			DisableUnusedOptions();
		}

		/// <summary>
		/// Resize the component based on the Options listed.
		/// </summary>
		/// <param name="childrenCount">The number of options under the Options menu transform.</param>
		/// <param name="preferredWidth">The preferredWidth of the Option children.</param>
		private void ResizeComponent(int childrenCount, float preferredWidth)
		{
			_sizeDelta.x = preferredWidth + _gridLayoutGroup.padding.left + _gridLayoutGroup.padding.right;
			_sizeDelta.y = (childrenCount * _gridLayoutGroup.cellSize.y) + _gridLayoutGroup.padding.top + _gridLayoutGroup.padding.bottom;
			_rectTransform.sizeDelta = _sizeDelta;
			_relativeSizeDelta = (_rectTransform.sizeDelta * _rectTransform.pivot) + _padding;
		}
	}
}

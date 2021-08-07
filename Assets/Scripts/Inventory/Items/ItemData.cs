﻿using System;
using UnityEngine;

namespace ItemSystem {
	[Serializable]
	[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/Item Data")]
	public class ItemData : ScriptableObject {

		[SerializeField]
		[Tooltip("Image of this item")]
		public Sprite Sprite;

		[SerializeField]
		[Tooltip("Prefab of this item as it appears in the world.")]
		public GameObject Prefab;

		[SerializeField]
		[TextArea(3, 15)]
		[Tooltip("Tooltip to be displayed when hovered on.")]
		public string Tooltip;

		[SerializeField]
		[Tooltip("The type of item")]
		public ItemType ItemType = ItemType.DEFAULT;

		[SerializeField]
		[Tooltip("Determine whether or not the object can be stacked in the inventory or not.")]
		public bool Stackable = true;

		public override bool Equals(object other) {
			if ((other == null) || !GetType().Equals(other.GetType())) {
				return false;
			} else {
				return ID == ((ItemData)other).ID;
			}
		}

		public override int GetHashCode() {
			return ID.GetHashCode();
		}

		public override string ToString() {
			return "Item : { " +
				"ID: " + ID +
				", Name: " + name + 
				" }";
		}

		public string ID { get; set; }
	}
}
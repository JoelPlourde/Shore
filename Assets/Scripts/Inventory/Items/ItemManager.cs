using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using ItemSystem.EquipmentSystem;

namespace ItemSystem {
	[ExecuteInEditMode]
	public class ItemManager : MonoBehaviour {

		public static ItemManager Instance;

		private Dictionary<string, ItemData> _itemDatas;

		private void Awake() {
			Instance = this;

			ItemData[] itemDatas = Resources.LoadAll<ItemData>("Scriptable Objects/Items");
			Array.ForEach(itemDatas, itemData => {
				itemData.ID = itemData.name
					.Replace("D_", "")
					.ToLower()
					.Replace(" ", "_")
					.Replace("(", "")
					.Replace(")", "");
			});
			_itemDatas = itemDatas.ToDictionary(x => x.ID);
		}

		public bool PlaceItemInWorld(Item item, Vector3 position, Quaternion rotation, bool withOffset = true) {
			for (int i = 0; i < item.Amount; i++) {

				// Get the closest point on the NavMesh to the desired position.

				UnityEngine.AI.NavMesh.SamplePosition(position, out UnityEngine.AI.NavMeshHit hit, 0.25f, UnityEngine.AI.NavMesh.AllAreas);
				position = hit.position;

				// Add some jitter to the position to avoid stacking items.
				position.x += UnityEngine.Random.Range(-0.25f, 0.25f);
				position.z += UnityEngine.Random.Range(-0.25f, 0.25f);

				/*
				if (withOffset) {
					position.y += 0.25f;
				}
				*/

				GameObject @object = Instantiate(item.ItemData.Prefab, position, rotation);
				@object.GetComponent<InteractableItem>().RegenerateUUID();
			}
			
			return true;
		}

		public ItemData GetItemData(string name) {
			if (_itemDatas.TryGetValue(name.ToLower().Replace(" ", "_"), out ItemData itemData)) {
				return itemData;
			} else {
				throw new UnityException("The Item couldn't be found by its name. Please define this Item Data: " + name);
			}
		}

		public EquipmentData GetEquipmentData(string name) {
			return (GetItemData(name) as EquipmentData);
		}
	}
}

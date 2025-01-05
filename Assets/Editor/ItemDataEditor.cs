using ItemSystem.EffectSystem;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace ItemSystem {
	[CustomEditor(typeof(ItemData))]
	public class ItemDataEditor : Editor {

		public override void OnInspectorGUI() {
			ItemData itemData = (ItemData) target;

			// Show the ItemData.ID as a label field
			EditorGUILayout.LabelField("ID", itemData.ID.ToString(CultureInfo.InvariantCulture));

			base.OnInspectorGUI();

			if (itemData.Burnable) {
				itemData.Power = EditorGUILayout.IntField("Power", itemData.Power);
			}

			if (itemData.ItemType == ItemType.CONSUMABLE) {
				if (itemData.ItemEffects.Count == 0) {
					itemData.ItemEffects.Add(new ItemEffect {
						ItemEffectType = ItemEffectType.EAT,
						Magnitude = 1
					});
				}
			}

			EditorGUILayout.Space();

			if (GUILayout.Button("Save")) {
				EditorUtility.SetDirty(itemData);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}
	}
}

using ItemSystem.EffectSystem;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace ItemSystem {
	[CustomEditor(typeof(ItemData))]
	public class ItemDataEditor : Editor {

		private static readonly string SPRITE_DIRECTORY = "Assets/Textures/Items";

		public override void OnInspectorGUI() {
			ItemData itemData = (ItemData) target;

			if (itemData.ID == null) {
				itemData.ID = itemData.name
					.Replace("D_", "")
					.ToLower()
					.Replace(" ", "_")
					.Replace("(", "")
					.Replace(")", "");
			}

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

		public void OnEnable()
        {
			ItemData itemData = (ItemData) target;
            // Get the name of this object
			string name = itemData.name;

			// Replace "D_" prefix by "S_"
			if (name.StartsWith("D_"))
			{
				name = name.Substring(2);
			}

			// Add the "S_" prefix to the name to match the ItemData naming convention
			name = "S_" + name;

			// If the Sprite is not assigned, try to find and assign it from the Textures/Items directory
			if (itemData.Sprite == null)
			{
				string folder = getItemTypeToFolder(itemData.ItemType.ToString());
				string spritePath = SPRITE_DIRECTORY + "/" + folder + "/" + name + ".png";
				
				Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

				if (!ReferenceEquals(sprite, null))
				{
					itemData.Sprite = sprite;
					EditorUtility.SetDirty(itemData);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
			}
        }

		private string getItemTypeToFolder(string itemType)
        {
            switch (itemType)
			{
				case "DEFAULT":
					return "Resources";
				case "EQUIPMENT":
					return "Equipments";
				case "CONSUMABLE":
					return "Food";
				default:
					return "Resources";
			}
        }
	}
}

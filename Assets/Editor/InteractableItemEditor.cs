using UnityEngine;
using UnityEditor;
using System.IO;
using ItemSystem.EquipmentSystem;
using System;

namespace ItemSystem
{
    [CustomEditor(typeof(InteractableItem))]
    public class InteractableItemEditor : Editor
    {
        private static readonly string ITEMS_DIRECTORY = "Assets/Resources/Scriptable Objects/Items";
        private static readonly string EQUIPMENT_DIRECTORY = ITEMS_DIRECTORY + "/Equipments";

        public override void OnInspectorGUI() {
            InteractableItem item = (InteractableItem)target;

            base.OnInspectorGUI();

            // Add a space
            GUILayout.Space(10);

            if (ReferenceEquals(item.ItemData, null))
            {
                // Add a button to the Inspector to create an empty ItemData asset
                if (GUILayout.Button("Create ItemData"))
                {
                    ItemData newItemData = ScriptableObject.CreateInstance<ItemData>();
                    CreateItemDataAsset(item, newItemData, ITEMS_DIRECTORY);
                    FindItemData(item);
                }

                if (GUILayout.Button("Create EquipmentData"))
                {
                    EquipmentData newEquipmentData = ScriptableObject.CreateInstance<EquipmentData>();
                    CreateItemDataAsset(item, newEquipmentData, EQUIPMENT_DIRECTORY);
                    FindItemData(item);
                }
            }
        }

        private void OnEnable()
        {
            InteractableItem item = (InteractableItem)target;

            // Ensure the item has the correct tag
            if (item.tag != "Interactable")
            {
                item.tag = "Interactable";
                EditorUtility.SetDirty(item);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // If the ItemData is not assigned, try to find and assign it
            if (item.ItemData == null)
            {
                FindItemData(item);
            }

            Collider collider = item.gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.excludeLayers = LayerMask.GetMask("Default");
                EditorUtility.SetDirty(item);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void CreateItemDataAsset(InteractableItem item, ItemData itemData, string path)
        {
            string assetPath = Path.Combine(path, "D_" + item.gameObject.name + ".asset");
            assetPath = assetPath.Replace("P_", "");
            AssetDatabase.CreateAsset(itemData, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void FindItemData(InteractableItem item)
        {
            // Get the name of this object
            string name = item.gameObject.name;

            // Strip the first two characters if they are "P_" or "I_"
            if (name.StartsWith("P_") || name.StartsWith("I_"))
            {
                name = name.Substring(2);
            }

            // Add the "D_" prefix to the name to match the ItemData naming convention
            name = "D_" + name;

            string[] itemDataFiles = Directory.GetFiles(ITEMS_DIRECTORY, name + ".asset", SearchOption.AllDirectories);

            if (itemDataFiles.Length > 0)
            {
                string relativePath = itemDataFiles[0].Replace(Application.dataPath, "Assets").Replace("\\", "/");
                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(relativePath);
                item.ItemData = itemData;

                // Mark the item as dirty to save changes
                EditorUtility.SetDirty(item);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}

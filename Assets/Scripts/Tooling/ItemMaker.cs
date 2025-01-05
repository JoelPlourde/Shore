using System;
using ItemSystem;
using ItemSystem.EquipmentSystem;
using UnityEditor;
using UnityEngine;

namespace Tools {
    [ExecuteInEditMode]
    public class ItemMaker : MonoBehaviour {

        public void MakeItem(ItemCategory itemCategory) {
            string itemCategoryString = EnumExtensions.FormatEnum(itemCategory.ToString());

            Material material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Toons/Toon.mat");

            gameObject.GetComponent<MeshRenderer>().material = material;
            gameObject.AddComponent<BoxCollider>();
            if (ReferenceEquals(gameObject.GetComponent<InteractableItem>(), null)) {
                gameObject.AddComponent<InteractableItem>();
            }
            gameObject.tag = "Interactable";
            gameObject.name = GetHierarchyName(gameObject.name);

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            GameObject prefab = null;
            if (itemCategory == ItemCategory.EQUIPMENTS) {
                prefab = BuildPrefabObject(gameObject, gameObject.name, "Tools");
            } else {
                prefab = BuildPrefabObject(gameObject, gameObject.name, itemCategoryString);
            }

            if (itemCategory == ItemCategory.EQUIPMENTS) {
                BuildEquipmentData(prefab, gameObject.name, itemCategoryString);
            } else {
                BuildItemData(prefab, gameObject.name, itemCategoryString);
            }

            // At the very end, destroy the ItemMaker script on the prefab
            DestroyImmediate(prefab.GetComponent<ItemMaker>(), true);
        }

        private GameObject BuildPrefabObject(GameObject gameObject, string name, string category) {
            name = GetObjectName(name); // e.g "iron_axe"

            // Add the "P_" prefix to the name
            name = "P_" + name;

            // Make the path if it does not exists:
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Items/" + category)) {
                AssetDatabase.CreateFolder("Assets/Prefabs/Items", category);
            }

            string prefabPath = "Assets/Prefabs/Items/" + category + "/" + name + ".prefab";
            AssetDatabase.DeleteAsset(prefabPath);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath, out bool success);

            if (!success) {
                Debug.LogError("Failed to save prefab at: " + prefabPath);
            }
            return prefab;
        }

        private void BuildEquipmentData(GameObject prefab, string name, string category) {
            // Create a new ScriptableObject of EquipmentData
            EquipmentData equipmentData = ScriptableObject.CreateInstance<EquipmentData>();
            BuildData(prefab, name, category, equipmentData);
        }

        private void BuildItemData(GameObject prefab, string name, string category) {
            // Create a new ScriptableObject of ItemData
            ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
            BuildData(prefab, name, category, itemData);
        }

        private void BuildData(GameObject prefab, string name, string category, ItemData itemData) {
            name = GetHierarchyName(name); // e.g "iron_axe"

            // Add the "D_" prefix to the name
            name = "D_" + name;

            itemData.name = name;
            itemData.Prefab = prefab;

            // Record that itemData in the Resources/Scriptable Objects/Items/ directory
            string path = "Assets/Resources/Scriptable Objects/Items/" + category + "/" + name + ".asset";
            AssetDatabase.DeleteAsset(path);

            // Write to file
            AssetDatabase.CreateAsset(itemData, path);

            // Update the prefab with that itemData
            InteractableItem interactableItem = prefab.GetComponent<InteractableItem>();
            interactableItem.InteractionRadius = 0.5f;
            interactableItem.ItemData = itemData;
            interactableItem.RegenerateUUID();

            // Attempt to find the Texture in 
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/Items/" + category + "/" + name + ".png");
            if (!ReferenceEquals(sprite, null)) {
                itemData.Sprite = sprite;
            }
        }

        /// <summary>
        /// Get the hierarchy name of the object (e.g "iron_axe")
        /// </summary>
        /// <returns></returns>
        private string GetHierarchyName(string name) {
            return name.Replace(" (Object)", "").Replace(" ", "_").ToLower();
        }

        private string GetObjectName(string name) {
            // Start from scratch
            name = name.Replace(" ", "_").Replace("(Object)", "").ToLower();
            return name;
        }
    }

    public enum ItemCategory {
        EQUIPMENTS,
        ESSENCE,
        FOOD,
        RESOURCES,
        RUNE,
        TOOLS
    }
}

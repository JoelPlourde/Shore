using System;
using System.Collections.Generic;
using System.IO;
using CraftingSystem;
using ItemSystem;
using ItemSystem.EffectSystem;
using ItemSystem.EquipmentSystem;
using UnityEditor;
using UnityEngine;

namespace Tools {
    public class AssetTools
    {

        private static readonly string ITEMS_DIRECTORY = "Assets/Resources/Scriptable Objects/Items";
        private static readonly string RECIPES_DIRECTORY = "Assets/Resources/Scriptable Objects/Recipes";
        private static readonly string EQUIPMENT_DIRECTORY = "Assets/Resources/Scriptable Objects/Items/Equipments";
        private static readonly string SPRITE_DIRECTORY = "Assets/Textures/Items";
        private static readonly string ITEM_PREFAB_DIRECTORY = "Assets/Prefabs/Items";
        private static readonly string EQUIPMENT_PREFAB_DIRECTORY = "Assets/Prefabs/Items/Equipments";

        [MenuItem("Tools/Asset Tools/Find Missing Sprites", false, 1)]
        public static void FindMissingSprites()
        {
            // Load all ItemDatas from the Items directory
            string[] itemDataFiles = Directory.GetFiles(ITEMS_DIRECTORY, "*.asset", SearchOption.AllDirectories);

            // Load all of the Sprite files from the Sprites directory
            string[] spriteFiles = Directory.GetFiles(SPRITE_DIRECTORY, "*.png", SearchOption.AllDirectories);

            // For each spriteFile, get the filename without extension
            for (int i = 0; i < spriteFiles.Length; i++)
            {
                spriteFiles[i] = Path.GetRelativePath(SPRITE_DIRECTORY, spriteFiles[i]);
            }

            // Convert the spriteFiles to a hashset for faster lookups
            HashSet<string> spriteFileSet = new HashSet<string>(spriteFiles);

            // Iterate over all of the ItemDatas and check if the sprite exists
            foreach (string itemDataFile in itemDataFiles)
            {
                string fileName = Path.GetRelativePath(ITEMS_DIRECTORY, itemDataFile);

                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(itemDataFile);

                if (itemData.Sprite == null)
                {
                    // Check if the sprite exists in the spriteFiles
                    string spriteName = fileName.Replace("D_", "S_").Replace(".asset", ".png");

                    if (!spriteFileSet.Contains(spriteName))
                    {
                        Debug.LogError($"Missing Sprite for Item Data: {fileName}");
                    }
                    else
                    {
                        // Load the sprite and assign it to the ItemData since we've found it!
                        string path = $"{SPRITE_DIRECTORY}/{spriteName}";
                        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        itemData.Sprite = sprite;
                    }
                }
            }

            Debug.Log("Finished checking for missing sprites");
        }

        [MenuItem("Tools/Asset Tools/Find Missing Prefabs", false, 2)]
        public static void FindMissingPrefabs()
        {
            // Load all ItemDatas from the Items directory
            string[] itemDataFiles = Directory.GetFiles(ITEMS_DIRECTORY, "*.asset", SearchOption.AllDirectories);

            // Load all of the Prefab files from the Prefabs directory
            string[] prefabFiles = Directory.GetFiles(ITEM_PREFAB_DIRECTORY, "*.prefab", SearchOption.AllDirectories);

            // For each prefabFile, get the filename without extension
            for (int i = 0; i < prefabFiles.Length; i++)
            {
                prefabFiles[i] = Path.GetRelativePath(ITEM_PREFAB_DIRECTORY, prefabFiles[i]);
            }

            // TODO Figure out how to handle Equipment Object vs. Equipment Armature.

            // Convert the prefabFiles to a hashset for faster lookups
            HashSet<string> prefabFileSet = new HashSet<string>(prefabFiles);

            // Iterate over all of the ItemDatas and check if the prefab exists
            foreach (string itemDataFile in itemDataFiles)
            {
                string fileName = Path.GetRelativePath(ITEMS_DIRECTORY, itemDataFile);

                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(itemDataFile);

                if (itemData.Prefab == null)
                {
                    // Check if the prefab exists in the prefabFiles
                    string prefabName = fileName.Replace("D_", "P_").Replace(".asset", ".prefab");

                    if (!prefabFileSet.Contains(prefabName))
                    {
                        Debug.LogError($"Missing Prefab for Item Data: {fileName}");
                    }
                    else
                    {
                        // Load the prefab and assign it to the ItemData since we've found it!
                        string path = $"{ITEM_PREFAB_DIRECTORY}/{prefabName}";
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        itemData.Prefab = prefab;
                    }
                }
            }

            Debug.Log("Finished checking for missing prefabs");
        }

        /**
        * Finds all EquipmentDatas that have missing EquipmentPrefabs.
        */
        [MenuItem("Tools/Asset Tools/Find Missing Equipment Prefabs", false, 2)]
        public static void FindMissingEquipmentPrefabs()
        {
            // Load all EquipmentDatas from the Items directory
            string[] equipmentDataFiles = Directory.GetFiles(EQUIPMENT_DIRECTORY, "*.asset", SearchOption.AllDirectories);

            // Load all of the Prefab files from the Prefabs directory
            string[] prefabFiles = Directory.GetFiles(EQUIPMENT_PREFAB_DIRECTORY, "*.prefab", SearchOption.AllDirectories);

            // For each prefabFile, get the filename without extension
            for (int i = 0; i < prefabFiles.Length; i++)
            {
                prefabFiles[i] = Path.GetRelativePath(EQUIPMENT_PREFAB_DIRECTORY, prefabFiles[i]);
            }

            // Convert the prefabFiles to a hashset for faster lookups
            HashSet<string> prefabFileSet = new HashSet<string>(prefabFiles);

            // Iterate over all of the EquipmentDatas and check if the prefab exists
            foreach (string equipmentDataFile in equipmentDataFiles)
            {
                string fileName = Path.GetRelativePath(EQUIPMENT_DIRECTORY, equipmentDataFile);

                EquipmentData equipmentData = AssetDatabase.LoadAssetAtPath<EquipmentData>(equipmentDataFile);

                if (equipmentData.EquipmentPrefab == null)
                {
                    // Check if the prefab exists in the prefabFiles
                    string prefabName = fileName.Replace("D_", "P_").Replace(".asset", ".prefab");

                    if (!prefabFileSet.Contains(prefabName))
                    {
                        Debug.LogError($"Missing Prefab for Item Data: {fileName}");
                    }
                    else
                    {
                        // Load the prefab and assign it to the ItemData since we've found it!
                        string path = $"{EQUIPMENT_PREFAB_DIRECTORY}/{prefabName}";
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        equipmentData.EquipmentPrefab = prefab;
                    }
                }
            }

            Debug.Log("Finished checking for missing equipment prefabs");
        }

        /**
        * Finds all ItemDatas that have missing translations in all languages.
        */
        [MenuItem("Tools/Asset Tools/Find Missing Translations", false, 2)]
        public static void FindMissingTranslations()
        {
            // Load all ItemDatas from the Items directory
            string[] itemDataFiles = Directory.GetFiles(ITEMS_DIRECTORY, "*.asset", SearchOption.AllDirectories);

            // Load the file content of the translations directory
            string[] translationFiles = Directory.GetFiles("Assets/Resources/Translations", "*.json", SearchOption.AllDirectories);

            // For each translationFile, get the filename without extension
            for (int i = 0; i < translationFiles.Length; i++)
            {
                translationFiles[i] = Path.GetFileNameWithoutExtension(translationFiles[i]);

                TextAsset languageFile = Resources.Load<TextAsset>("Translations/" + translationFiles[i]);

                if (languageFile != null)
                {
                    Dictionary<string, string> translations = I18N.FlattenJson(languageFile.text);

                    foreach (string itemDataFile in itemDataFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(itemDataFile);
                        fileName = fileName.Replace("D_", "").Replace(".asset", "");
                        string jsonPath = "items." + fileName + ".name";

                        ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(itemDataFile);

                        if (!translations.ContainsKey(jsonPath))
                        {
                            Debug.LogError($"Missing Translation for Item Data: {jsonPath}");
                        }
                    }
                }
            }

            Debug.Log("Finished checking for missing translations");
        }

        /**
        * Finds all RecipeDatas that have missing inputs or outputs.
        */
        [MenuItem("Tools/Asset Tools/Find Missing Recipes", false, 2)]
        public static void FindMissingRecipes()
        {
            // Load all RecipeDatas from the Recipes directory
            string[] recipeDataFiles = Directory.GetFiles(RECIPES_DIRECTORY, "*.asset", SearchOption.AllDirectories);

            Debug.Log($"Found {recipeDataFiles.Length} recipes to check.");

            // Iterate over all of the RecipeDatas and check if the prefab exists
            foreach (string recipeDataFile in recipeDataFiles)
            {
                string fileName = Path.GetRelativePath(RECIPES_DIRECTORY, recipeDataFile);

                RecipeData recipeData = AssetDatabase.LoadAssetAtPath<RecipeData>(recipeDataFile);

                if (recipeData.Outputs.Count == 0)
                {
                    Debug.LogError($"Recipe has no outputs: {fileName}");
                }
                else
                {
                    foreach (var output in recipeData.Outputs)
                    {
                        if (output.ItemData == null)
                        {
                            Debug.LogError($"Recipe has an output with no ItemData: {fileName}");
                        }
                    }
                }

                if (recipeData.Inputs.Count == 0)
                {
                    Debug.LogError($"Recipe has no inputs: {fileName}");
                }
                else
                {
                    foreach (var input in recipeData.Inputs)
                    {
                        if (input.ItemData == null)
                        {
                            Debug.LogError($"Recipe has an input with no ItemData: {fileName}");
                        }
                    }
                }
            }

            Debug.Log("Finished checking for missing recipes");
        }

        /**
        * Finds all ItemDatas that have ItemEffects of type PROCESS but have no RecipeDatas assigned.
        */
        [MenuItem("Tools/Asset Tools/Find Missing Item Recipes", false, 2)]
        public static void FindMissingItemRecipes()
        {
            // Load all ItemDatas from the Items directory
            string[] itemDataFiles = Directory.GetFiles(ITEMS_DIRECTORY, "*.asset", SearchOption.AllDirectories);

            // Iterate over all of the ItemDatas and check if the prefab exists
            foreach (string itemDataFile in itemDataFiles)
            {
                string fileName = Path.GetRelativePath(ITEMS_DIRECTORY, itemDataFile);

                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(itemDataFile);

                // If the ItemData has some Item Effects of type Process, check if it has at least one RecipeData assigned
                foreach (var itemEffect in itemData.ItemEffects)
                {
                    if (itemEffect.ItemEffectType == ItemEffectType.PROCESS)
                    {
                        // Check that each RecipeData is not null
                        foreach (var recipeData in itemEffect.RecipeDatas)
                        {
                            if (recipeData == null)
                            {
                                Debug.LogError($"Item Data {fileName} has a PROCESS ItemEffect with a null RecipeData.");
                            }
                        }
                    }
                }
            }

            Debug.Log("Finished checking for missing item recipes");
        }
    }
}

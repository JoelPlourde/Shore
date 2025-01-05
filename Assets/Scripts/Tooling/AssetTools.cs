using System.Collections.Generic;
using System.IO;
using ItemSystem;
using UnityEditor;
using UnityEngine;

namespace Tools {
    public class AssetTools {

        private static readonly string ITEMS_DIRECTORY = "Assets/Resources/Scriptable Objects/Items";
        private static readonly string SPRITE_DIRECTORY = "Assets/Textures/Items";
        private static readonly string PREFAB_DIRECTORY = "Assets/Prefabs/Items";

        [MenuItem("Tools/Asset Tools/Find Missing Sprites", false, 1)]
        public static void FindMissingSprites() {
            // Load all ItemDatas from the Items directory
            string[] itemDataFiles = Directory.GetFiles(ITEMS_DIRECTORY, "*.asset", SearchOption.AllDirectories);

            // Load all of the Sprite files from the Sprites directory
            string[] spriteFiles = Directory.GetFiles(SPRITE_DIRECTORY, "*.png", SearchOption.AllDirectories);

            // For each spriteFile, get the filename without extension
            for (int i = 0; i < spriteFiles.Length; i++) {
                spriteFiles[i] = Path.GetRelativePath(SPRITE_DIRECTORY, spriteFiles[i]);
            }

            // Convert the spriteFiles to a hashset for faster lookups
            HashSet<string> spriteFileSet = new HashSet<string>(spriteFiles); 

            // Iterate over all of the ItemDatas and check if the sprite exists
            foreach (string itemDataFile in itemDataFiles) {
                string fileName = Path.GetRelativePath(ITEMS_DIRECTORY, itemDataFile);

                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(itemDataFile);

                if (itemData.Sprite == null) {
                    // Check if the sprite exists in the spriteFiles
                    string spriteName = fileName.Replace("D_", "S_").Replace(".asset", ".png");

                    if (!spriteFileSet.Contains(spriteName)) {
                        Debug.LogError($"Missing Sprite for Item Data: {fileName}");
                    } else {
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
        public static void FindMissingPrefabs() {
            // Load all ItemDatas from the Items directory
            string[] itemDataFiles = Directory.GetFiles(ITEMS_DIRECTORY, "*.asset", SearchOption.AllDirectories);

            // Load all of the Prefab files from the Prefabs directory
            string[] prefabFiles = Directory.GetFiles(PREFAB_DIRECTORY, "*.prefab", SearchOption.AllDirectories);

            // For each prefabFile, get the filename without extension
            for (int i = 0; i < prefabFiles.Length; i++) {
                prefabFiles[i] = Path.GetRelativePath(PREFAB_DIRECTORY, prefabFiles[i]);
            }

            // Convert the prefabFiles to a hashset for faster lookups
            HashSet<string> prefabFileSet = new HashSet<string>(prefabFiles);

            // Iterate over all of the ItemDatas and check if the prefab exists
            foreach (string itemDataFile in itemDataFiles) {
                string fileName = Path.GetRelativePath(ITEMS_DIRECTORY, itemDataFile);

                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(itemDataFile);

                if (itemData.Prefab == null) {
                    // Check if the prefab exists in the prefabFiles
                    string prefabName = fileName.Replace("D_", "P_").Replace(".asset", ".prefab");

                    if (!prefabFileSet.Contains(prefabName)) {
                        Debug.LogError($"Missing Prefab for Item Data: {fileName}");
                    } else {
                        // Load the prefab and assign it to the ItemData since we've found it!
                        string path = $"{PREFAB_DIRECTORY}/{prefabName}";
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        itemData.Prefab = prefab;
                    }
                }
            }

            Debug.Log("Finished checking for missing prefabs");
        }

        [MenuItem("Tools/Asset Tools/Find Missing Translations", false, 2)]
        public static void FindMissingTranslations() {
            // Load all ItemDatas from the Items directory
            string[] itemDataFiles = Directory.GetFiles(ITEMS_DIRECTORY, "*.asset", SearchOption.AllDirectories);

            // Load the file content of the translations directory
            string[] translationFiles = Directory.GetFiles("Assets/Resources/Translations", "*.json", SearchOption.AllDirectories);

            // For each translationFile, get the filename without extension
            for (int i = 0; i < translationFiles.Length; i++) {
                translationFiles[i] = Path.GetFileNameWithoutExtension(translationFiles[i]);

                TextAsset languageFile = Resources.Load<TextAsset>("Translations/" + translationFiles[i]);

                if (languageFile != null) {
                    Dictionary<string, string> translations = I18N.FlattenJson(languageFile.text);

                    foreach (string itemDataFile in itemDataFiles) {
                        string fileName = Path.GetFileNameWithoutExtension(itemDataFile);
                        fileName = fileName.Replace("D_", "").Replace(".asset", "");
                        string jsonPath = "items." + fileName + ".name";

                        ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(itemDataFile);

                        if (!translations.ContainsKey(jsonPath)) {
                            Debug.LogError($"Missing Translation for Item Data: {jsonPath}");
                        }
                    }
                }
            }

            Debug.Log("Finished checking for missing translations");
        }
    }
}

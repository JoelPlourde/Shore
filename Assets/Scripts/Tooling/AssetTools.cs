using System.Collections.Generic;
using System.IO;
using ItemSystem;
using UnityEditor;
using UnityEngine;

namespace Tools {
    public class AssetTools {

        private static readonly string ITEMS_DIRECTORY = "Assets/Resources/Scriptable Objects/Items";
        private static readonly string SPRITE_DIRECTORY = "Assets/Textures/Items";

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
                        Debug.LogError($"Missing sprite for {fileName}");
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

        public static void ImportGeneratedIcons() {
            // Move the generated icons from Assets/GeneratedIcons/128x128/Black to Assets/Textures/Items

            // Configure the import settings for the generated icons

            //TextureImporter textureImporter = AssetImporter.GetAtPath(absolutePath) as TextureImporter;
        }
    }
}

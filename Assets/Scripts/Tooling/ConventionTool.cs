using System.IO;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;

namespace Tools {
    public static class ConventionTools {

        private static readonly string DATA_PREFIX = "D_";
        private static readonly string PREFAB_PREFIX = "P_";

        private static readonly string ASSET_EXTENSION = "*.asset";
        private static readonly string PREFAB_EXTENSION = "*.prefab";

        // Scriptable Objects Suffixes:
        private static readonly string RECIPE_SUFFIX = "_recipe";
        private static readonly string NODE_SUFFIX = "_node";
        private static readonly string DROP_TABLE_SUFFIX = "_drop_table";

        // Scriptable Objects Directories:
        private static readonly string ITEMS_DIRECTORY = "Assets/Resources/Scriptable Objects/Items";
        private static readonly string RECIPE_DIRECTORY = "Assets/Resources/Scriptable Objects/Recipes";
        private static readonly string NODE_DIRECTORY = "Assets/Resources/Scriptable Objects/Nodes";
        private static readonly string DROP_TABLE_DIRECTORY = "Assets/Resources/Scriptable Objects/Drop Tables";

        // Prefab Directories:
        private static readonly string ITEM_PREFAB_DIRECTORY = "Assets/Prefabs/Items";

        #region Scriptable Objects
        [MenuItem("Tools/Naming Convention/Scriptable Objects/Rename Items", false, 1)]
        public static void RenameScriptableObjectItems() {
            RenameFiles(ITEMS_DIRECTORY, ASSET_EXTENSION, DATA_PREFIX);
            Debug.Log("Rename Items complete!");
        }

        [MenuItem("Tools/Naming Convention/Scriptable Objects/Rename Recipes", false, 2)]
        public static void RenameScriptableObjectRecipes() {
            RenameFiles(RECIPE_DIRECTORY, ASSET_EXTENSION, DATA_PREFIX, RECIPE_SUFFIX);
            Debug.Log("Rename Recipes complete!");
        }

        [MenuItem("Tools/Naming Convention/Scriptable Objects/Rename Nodes", false, 3)]
        public static void RenameScriptableObjectNodes() {
            RenameFiles(NODE_DIRECTORY, ASSET_EXTENSION, DATA_PREFIX, NODE_SUFFIX);
            Debug.Log("Rename Nodes complete!");
        }

        [MenuItem("Tools/Naming Convention/Scriptable Objects/Rename Drop Tables", false, 4)]
        public static void RenameScriptableObjectDropTables() {
            RenameFiles(DROP_TABLE_DIRECTORY, ASSET_EXTENSION, DATA_PREFIX, DROP_TABLE_SUFFIX);
            Debug.Log("Rename Drop Tables complete!");
        }
        #endregion

        #region Prefabs
        [MenuItem("Tools/Naming Convention/Prefabs/Rename Items", false, 1)]
        public static void RenamePrefabItems() {
            RenameFiles(ITEM_PREFAB_DIRECTORY, PREFAB_EXTENSION, PREFAB_PREFIX);
            Debug.Log("Rename Items complete!");
        }
        #endregion

        private static void RenameFiles(string directory, string extension, string prefix, string suffix = "") {
            string[] files = Directory.GetFiles(directory, extension, SearchOption.AllDirectories);
            foreach (string file in files) {
                string absolutePath = file.Replace("\\", "/");
                string fileName = Path.GetFileNameWithoutExtension(file);
                string formattedName = FormatName(fileName, prefix, suffix);
                string result = AssetDatabase.RenameAsset(absolutePath, formattedName);
                if (result == "") {
                    Debug.Log(fileName + " has been renamed to " + formattedName);
                }
            }
        }

        private static string FormatName(string name, string prefix, string suffix) {
            name = name
                .Replace(prefix, "")
                .Replace(" (Object)", "")
                .Replace(" ", "_")
                .Replace("(", "")
                .Replace(")", "")
                .ToLower();

            if (suffix != "") {
                name = name.Replace(suffix, "");
            }

            if (!name.StartsWith(prefix)) {
                // Add the prefix to the name:
                name = prefix + name;
            }

            if (!name.EndsWith(suffix)) {
                // Add the suffix to the name:
                name += suffix;
            }

            return name;
        }
    }
}
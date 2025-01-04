using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools {
    public class ImportTools {

        private static readonly string MODEL_DIRECTORY = "Assets/Models/Items";

        private static readonly string EQUIPMENT_DIRECTORY = "Assets/Models/Items/Equipment";
    
        [MenuItem("Tools/Import Tools/Configure Import Settings", false, 1)]
        public static void ConfigureImportSettings() {
            string[] files = Directory.GetFiles(MODEL_DIRECTORY, "*.fbx", SearchOption.AllDirectories);

            foreach (string file in files) {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string absolutePath = file.Replace("\\", "/");
                if (absolutePath.Contains(EQUIPMENT_DIRECTORY)) {
                    Debug.Log("" + fileName + " is an equipment model, it has a different import setting.");
                    continue;
                }

                Debug.Log("Configuring import settings for " + fileName);

                ModelImporter modelImporter = AssetImporter.GetAtPath(absolutePath) as ModelImporter;

                // Model Tab
                modelImporter.useFileScale = false;
                modelImporter.globalScale = 1f;
                modelImporter.importAnimation = false;
                modelImporter.importBlendShapes = false;
                modelImporter.importCameras = false;
                modelImporter.importLights = false;
                modelImporter.useFileUnits = true;

                // Rig Tab
                modelImporter.animationType = ModelImporterAnimationType.None;

                // Animation Tab
                modelImporter.importAnimation = false;
                modelImporter.importConstraints = false;

                // Materials Tab
                modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;

                modelImporter.SaveAndReimport();
            }
        }
    }
}
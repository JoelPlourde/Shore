using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools {
    public class ImportTools {

        private static readonly string MODEL_DIRECTORY = "Assets/Models/Items";

        private static readonly string EQUIPMENT_DIRECTORY = "Assets/Models/Items/Equipment";

        private static readonly string SPRITE_DIRECTORY = "Assets/Textures/Items";

        private static readonly string GENERATED_ICONS_DIRECTORY = "Assets/GeneratedIcons/128x128/Black";
    
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

            Debug.Log("Finished configuring import settings");
        }

        [MenuItem("Tools/Import Tools/Import Generated Icons", false, 2)]
        public static void ImportGeneratedIcons() {
            // Move the generated icons from Assets/GeneratedIcons/128x128/Black to Assets/Textures/Items
            string[] generatedIconFiles = Directory.GetFiles(GENERATED_ICONS_DIRECTORY, "*.png", SearchOption.AllDirectories);

            // For each generated icon, move it to the Items directory
            foreach (string generatedIconFile in generatedIconFiles) {
                string fileName = Path.GetFileName(generatedIconFile).Replace("P_", "S_");
                string destination = $"{SPRITE_DIRECTORY}/{fileName}";

                AssetDatabase.MoveAsset(generatedIconFile, destination);

                TextureImporter textureImporter = AssetImporter.GetAtPath(destination) as TextureImporter;
                if (textureImporter == null) {
                    Debug.LogError("Texture Importer is null");
                    continue;
                }

                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;

                textureImporter.SaveAndReimport();
            }

            Debug.Log("Finished importing generated icons");
        }
    }
}
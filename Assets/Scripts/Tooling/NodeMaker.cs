using DropSystem;
using NodeSystem;
using UnityEditor;
using UnityEngine;

namespace Tools {
    public class NodeMaker : MonoBehaviour {

        public void MakeNode(string nodeKey, NodeCategory nodeCategory) {
            string nodeCategoryString = EnumExtensions.FormatEnum(nodeCategory.ToString());
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider == null) {
                collider = gameObject.AddComponent<BoxCollider>();
            }

            collider.isTrigger = !isObstacle(nodeCategory);

            AddNodeBehaviour(nodeCategory);

            gameObject.tag = "Interactable";

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            nodeKey = GetObjectName(nodeKey);

            GameObject prefab = BuildPrefabObject(gameObject, gameObject.name, nodeCategoryString);

            NodeData nodeData = BuildNodeData(prefab, nodeKey, nodeCategory);

            ConfigureNodeData(nodeCategory, nodeData);

            // At the very end, destroy the ItemMaker script on the prefab
            DestroyImmediate(prefab.GetComponent<NodeMaker>(), true);

            Debug.Log("Node created successfully");
        }

        private NodeBehaviour AddNodeBehaviour(NodeCategory nodeCategory) {
            NodeBehaviour nodeBehaviour = null;
            switch (nodeCategory) {
                case NodeCategory.ORES:
                case NodeCategory.ROCKS:
                case NodeCategory.PLANTS:
                    nodeBehaviour = gameObject.GetComponent<NodeBehaviour>();
                    if (ReferenceEquals(nodeBehaviour, null)) {
                        nodeBehaviour = gameObject.AddComponent<NodeBehaviour>();
                    }
                    break;
                case NodeCategory.TREES:
                    nodeBehaviour = gameObject.GetComponent<TreeBehaviour>();
                    if (ReferenceEquals(nodeBehaviour, null)) {
                        nodeBehaviour = gameObject.AddComponent<TreeBehaviour>();
                    }
                    break;
                default:
                    Debug.LogError("This NodeBehaviour is not implemented yet");
                    break;  
            }
            return nodeBehaviour;
        }

        private GameObject BuildPrefabObject(GameObject gameObject, string name, string category) {
            name = GetObjectName(name); // e.g "stone_node"

            // Add the "P_" prefix to the name
            name = "P_" + name;

            // Create the Path of the nodeCategoryString if not exists
            string nodeCategoryPath = "Assets/Prefabs/Nodes/" + category;    
            if (!AssetDatabase.IsValidFolder(nodeCategoryPath)) {
                AssetDatabase.CreateFolder("Assets/Prefabs/Nodes", category);
            }

            string prefabPath = "Assets/Prefabs/Nodes/" + category + "/" + name + ".prefab";
            AssetDatabase.DeleteAsset(prefabPath);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath, out bool success);
            if (!success) {
                Debug.LogError("Failed to save prefab at: " + prefabPath);
            }

            return prefab;
        }

        private NodeData BuildNodeData(GameObject prefab, string nodeKey, NodeCategory nodeCategory) {
            string nodeCategoryString = EnumExtensions.FormatEnum(nodeCategory.ToString());

            string dataName = "D_" + GetObjectName(nodeKey);

            // Create the Path of the nodeCategoryString if not exists
            string nodeCategoryPath = "Assets/Resources/Scriptable Objects/Nodes/" + nodeCategoryString;    
            if (!AssetDatabase.IsValidFolder(nodeCategoryPath)) {
                AssetDatabase.CreateFolder("Assets/Resources/Scriptable Objects/Nodes", nodeCategoryString);
            }

            string nodeDataPath = "Assets/Resources/Scriptable Objects/Nodes/" + nodeCategoryString + "/" + dataName + ".asset";

            NodeData nodeData = AssetDatabase.LoadAssetAtPath<NodeData>(nodeDataPath);
            if (ReferenceEquals(nodeData, null)) {
                nodeData = BuildNodeData(nodeCategory);
                AssetDatabase.CreateAsset(nodeData, nodeDataPath);
            }

            // Create the Path of the nodeCategoryString for the drop table if not exists
            string nodeDropTablePath = "Assets/Resources/Scriptable Objects/Drop Tables/" + nodeCategoryString;
            if (!AssetDatabase.IsValidFolder(nodeDropTablePath)) {
                AssetDatabase.CreateFolder("Assets/Resources/Scriptable Objects/Drop Tables", nodeCategoryString);
            }

            string dropTableName = dataName + "_drop_table";
            string dropTablePath = "Assets/Resources/Scriptable Objects/Drop Tables/" + nodeCategoryString + "/" + dropTableName + ".asset";

            DropTable dropTable = AssetDatabase.LoadAssetAtPath<DropTable>(dropTablePath);
            if (ReferenceEquals(dropTable, null)) {
                dropTable = ScriptableObject.CreateInstance<DropTable>();
                AssetDatabase.CreateAsset(dropTable, dropTablePath);
            }

            nodeData.DropTable = dropTable;

            // Update the prefab with that nodeData
            NodeBehaviour nodeBehaviour = prefab.GetComponent<NodeBehaviour>();
            nodeBehaviour.InteractionRadius = 1.0f;
            nodeBehaviour.NodeData = nodeData;

            return nodeData;
        }

        private NodeData BuildNodeData(NodeCategory nodeCategory) {
            NodeData nodeData = null;
            switch (nodeCategory) {
                case NodeCategory.ORES:
                case NodeCategory.ROCKS:
                case NodeCategory.PLANTS:
                    nodeData = ScriptableObject.CreateInstance<NodeData>();
                    break;
                case NodeCategory.TREES:
                    nodeData = ScriptableObject.CreateInstance<TreeData>();
                    break;
                default:
                    Debug.LogError("This NodeData is not implemented yet");
                    break;
            }
            return nodeData;
        }

        private void ConfigureNodeData(NodeCategory nodeCategory, NodeData nodeData) {
            switch (nodeCategory) {
                case NodeCategory.ORES:
                case NodeCategory.ROCKS:
                    nodeData.Action = "mine";
                    nodeData.SkillType = SkillSystem.SkillType.MINING;
                    nodeData.WeaponType = ItemSystem.EquipmentSystem.WeaponType.PICKAXE;
                    nodeData.DamageStatistic = ItemSystem.EquipmentSystem.StatisticType.MINING_STRENGTH;
                    nodeData.SpeedStatistic = ItemSystem.EquipmentSystem.StatisticType.MINING_SPEED;
                    nodeData.OnHit.ParticleSystem = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Particle Systems/PS_Mining_hit.prefab");
                    nodeData.OnHit.Sound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/FX/rock_mining_hit.mp3");
                    break;
                case NodeCategory.TREES:
                    TreeData treeData = (TreeData)nodeData;
                    treeData.Action = "chop";
                    treeData.SkillType = SkillSystem.SkillType.WOODWORKING;
                    treeData.WeaponType = ItemSystem.EquipmentSystem.WeaponType.AXE;
                    treeData.DamageStatistic = ItemSystem.EquipmentSystem.StatisticType.WOODWORKING_STRENGTH;
                    treeData.SpeedStatistic = ItemSystem.EquipmentSystem.StatisticType.WOODWORKING_SPEED;
                    treeData.OnHit.ParticleSystem = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Particle Systems/PS_Chopping_hit.prefab");
                    treeData.OnHit.Sound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/FX/tree_chopping_hit.mp3");
                    treeData.OnResponse.ParticleSystem = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Particle Systems/PS_Falling_Leaves.prefab");
                    break;
                case NodeCategory.PLANTS:
                    nodeData.Action = "pick";
                    nodeData.SkillType = SkillSystem.SkillType.FARMING;
                    nodeData.WeaponType  = ItemSystem.EquipmentSystem.WeaponType.NONE;
                    nodeData.DamageStatistic = ItemSystem.EquipmentSystem.StatisticType.NONE;
                    nodeData.SpeedStatistic = ItemSystem.EquipmentSystem.StatisticType.NONE;
                    break;
                default:
                    Debug.LogError("This NodeCategory is not implemented yet");
                    break;
            }
        }

        private string GetObjectName(string name) {
            return name.Replace(" ", "_").ToLower();

        }

        private bool isObstacle(NodeCategory nodeCategory) {
            switch (nodeCategory) {
                case NodeCategory.ORES:
                case NodeCategory.ROCKS:
                    return true;
                case NodeCategory.TREES:
                    return true;
                case NodeCategory.PLANTS:
                    return false;
                default:
                    return true;
            }
        }

        public enum NodeCategory {
            ORES,
            ROCKS,
            TREES,
            PLANTS
        }
    }
}
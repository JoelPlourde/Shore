using UnityEditor;
using UnityEngine;

namespace NodeSystem {
    [CustomEditor(typeof(NodeData))]
    public class NodeDataEditor : Editor {

        public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			NodeData nodeData = (NodeData) target;

            // Get the name of the file.
            string path = AssetDatabase.GetAssetPath(nodeData.GetInstanceID());

            // Get the string after the last /
            int index = path.LastIndexOf('/');
            if (index != -1) {
                path = path.Substring(index + 1);
            }

            // Generate the ID from the path.
            nodeData.ID = path.Replace("D_", "").ToLower().Replace(".asset", "").Replace(" ", "_");
		}
    }
}

using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace AbilitySystem
{
    [CustomEditor(typeof(AbilityData))]
    public class AbilityDataEditor : Editor {

		private static readonly string ABILITIES_DIRECTORY = "Assets/Resources/Scriptable Objects/Abilities/Behaviours";

        public override void OnInspectorGUI() {
			AbilityData abilityData = (AbilityData) target;

			if (string.IsNullOrEmpty(abilityData.ID)) {
				abilityData.ID = abilityData.name
					.Replace("A_", "")
                    .Replace("a_", "")
					.ToLower()
					.Replace(" ", "_")
					.Replace("(", "")
					.Replace(")", "");
			}

            // Show the AbilityData.ID as a label field
			EditorGUILayout.LabelField("ID", abilityData.ID.ToString(CultureInfo.InvariantCulture));

			base.OnInspectorGUI();

			EditorGUILayout.Space();

			// Save
			if (GUILayout.Button("Save")) {
				EditorUtility.SetDirty(abilityData);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
        }
	}
}
using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace MonsterSystem
{
    [CustomEditor(typeof(MonsterData))]
    public class MonsterDataEditor : Editor {

        public override void OnInspectorGUI() {
            MonsterData monsterData = (MonsterData)target;

            if (string.IsNullOrEmpty(monsterData.ID)) {
				monsterData.ID = monsterData.name
                    .Replace("m_", "")
					.Replace("M_", "")
					.ToLower()
					.Replace(" ", "_")
					.Replace("(", "")
					.Replace(")", "");
			}

			// Show the MonsterData.ID as a label field
			EditorGUILayout.LabelField("ID", monsterData.ID.ToString(CultureInfo.InvariantCulture));

            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Wandering Settings", EditorStyles.boldLabel);
            monsterData.Wanders = EditorGUILayout.Toggle("Wanders", monsterData.Wanders);
            // Only show WanderingRadius if Wanders is true
            if (monsterData.Wanders) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("WanderingRadius"));
                // If the MonsterData's WanderingRadius is 0, set it to a default value of 5
                if (monsterData.WanderingRadius == 0)
                {
                    monsterData.WanderingRadius = 5.0f;
                }
            }
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Combat Settings", EditorStyles.boldLabel);
            monsterData.Attackable = EditorGUILayout.Toggle("Attackable", monsterData.Attackable);

            if (monsterData.Attackable) {
                EditorGUILayout.HelpBox("This Monster can be attacked by the player.", MessageType.Info);
            }
        }
    }
}

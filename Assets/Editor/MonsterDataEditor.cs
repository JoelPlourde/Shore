using System;
using System.Globalization;
using UnityEditor;
using UnityEditor.ShaderKeywordFilter;
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
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("ID", monsterData.ID.ToString(CultureInfo.InvariantCulture));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("WalkingSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TimeBetweenActions"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Physical Attributes", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(serializedObject.FindProperty("ForwardOffset"), -180, 180);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Size"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Height"));

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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Combat Settings", EditorStyles.boldLabel);
            monsterData.Attackable = EditorGUILayout.Toggle("Attackable", monsterData.Attackable);
            if (monsterData.Attackable) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Damage"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DamageCategoryType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Health"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AttackRange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AttackSpeed"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Loot Settings", EditorStyles.boldLabel);
            monsterData.Lootable = EditorGUILayout.Toggle("Lootable", monsterData.Lootable);
            if (monsterData.Lootable)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DropTable"));
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Experience Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Experience"));
        }
    }
}

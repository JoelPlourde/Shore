﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SkillSystem {
	[CustomEditor(typeof(SkillData))]
	public class SkillDataEditor : Editor {

		public override void OnInspectorGUI() {
			GUILayout.Space(20);
			GUILayout.Label("General");
			base.OnInspectorGUI();

			SkillData skillData = (SkillData)target;

			GUILayout.Space(20);
			GUILayout.Label("Advanced");

			if (GUILayout.Button("Sort Competencies")) {
				skillData.Competencies = skillData.Competencies.OrderBy(x => x.Requirement).ToArray();
			}

			GUILayout.Space(20);
			GUILayout.Label("Export/Import");

			string filename = skillData.SkillType.ToString().ToLower();

			if (GUILayout.Button("Export")) {
				string assetPath = AssetDatabase.GetAssetPath(skillData.GetInstanceID());
				AssetDatabase.RenameAsset(assetPath, filename);
				AssetDatabase.SaveAssets();
				Directory.CreateDirectory(Application.dataPath + "/Skills");
				File.WriteAllText(Application.dataPath + "/Skills/" + filename + ".json", JsonUtility.ToJson(skillData, true));
			}

			if (GUILayout.Button("Import")) {
				StreamReader reader = new StreamReader(EditorUtility.OpenFilePanel("Select File to import", "", "json"));
				JsonUtility.FromJsonOverwrite(reader.ReadToEnd(), skillData);
				reader.Close();
			}

			string filepath = Application.dataPath + "/Skills/" + filename + ".json";
			if (File.Exists(filepath)) {
				EditorGUILayout.HelpBox("This skill has been last modified on: " + File.GetLastWriteTime(filepath), MessageType.Info);
			}
		}
	}
}

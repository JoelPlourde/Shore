using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace ItemSystem {
	namespace EquipmentSystem {
		[CustomEditor(typeof(EquipmentData))]
		public class EquipmentDataEditor : Editor {

			public override void OnInspectorGUI() {
				base.OnInspectorGUI();

				EquipmentData equipmentData = (EquipmentData)target;

				equipmentData.ItemType = ItemType.EQUIPMENT;

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Conditional Properties", EditorStyles.boldLabel);

				using (new EditorGUI.DisabledScope(equipmentData.SlotType != SlotType.BODY)) {
					equipmentData.HideArms = EditorGUILayout.Toggle("Hide Arms", equipmentData.HideArms);
				}

				using (new EditorGUI.DisabledScope(equipmentData.SlotType != SlotType.HEAD)) {
					equipmentData.HideHair = EditorGUILayout.Toggle("Hide Hair", equipmentData.HideHair);
				}

				if (equipmentData.SlotType == SlotType.WEAPON || equipmentData.SlotType == SlotType.SHIELD) {
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Weapon-Specific Properties", EditorStyles.boldLabel);
					equipmentData.WeaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", equipmentData.WeaponType);
					equipmentData.WeaponDamageType = (WeaponDamageType)EditorGUILayout.EnumPopup("Weapon Damage Type", equipmentData.WeaponDamageType);
				} else {
					equipmentData.WeaponType = WeaponType.NONE;
				}

				if (equipmentData.SlotType != SlotType.BODY) {
					equipmentData.HideArms = false;
				}

				if (equipmentData.SlotType != SlotType.HEAD) {
					equipmentData.HideHair = false;
				}

				if (equipmentData.Stackable) {
					EditorGUILayout.HelpBox("This equipment is set as Stackable, is this ok ?", MessageType.Warning);
				}

				if (equipmentData.EquipmentStats.Statistics.Count == 0) {
					equipmentData.EquipmentStats.Statistics.Add(new Statistic {
						StatisticType = StatisticType.ARMOR,
						Value = 1
					});
				}

				if (equipmentData.ItemEffects.Count == 0) {
					equipmentData.ItemEffects.Add(new EffectSystem.ItemEffect {
						ItemEffectType = EffectSystem.ItemEffectType.EQUIP,
						Magnitude = 0
					});
				}

				EditorGUILayout.Space();

				if (GUILayout.Button("Save")) {
					EditorUtility.SetDirty(equipmentData);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
			}
		}
	}
}
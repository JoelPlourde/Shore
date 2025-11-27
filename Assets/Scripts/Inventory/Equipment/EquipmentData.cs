using System;
using CombatSystem;
using UnityEngine;

namespace ItemSystem {
	namespace EquipmentSystem {
		[Serializable]
		[CreateAssetMenu(fileName = "EquipmentData", menuName = "ScriptableObjects/Equipment Data")]
		public class EquipmentData : ItemData {

			[SerializeField]
			[Tooltip("The object with its armature to be attached to the model.")]
			public GameObject EquipmentPrefab;

			[SerializeField]
			[Tooltip("The slot where this equipment should be equipped.")]
			public SlotType SlotType;

			[SerializeField]
			[HideInInspector]
			public WeaponType WeaponType;
			
			[SerializeField]
			[HideInInspector]
			public WeaponDamageType WeaponDamageType;

			[SerializeField]
			[Tooltip("The damage category type of the equipment (if applicable).")]
			public DamageCategoryType DamageCategoryType = DamageCategoryType.MELEE;

			[SerializeField]
			[Tooltip("If true, the body part will be hidden to avoid clipping when this object is equipped.")]
			public bool HideBodyPart = true;

			[HideInInspector]
			public bool HideArms;

			[HideInInspector]
			public bool HideHair;

			[SerializeField]
			[Tooltip("The statistics of this equiment.")]
			[Space(10, order = 100)]
			public EquipmentStats EquipmentStats;
		}
	}
}

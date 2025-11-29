using System;
using System.Collections.Generic;
using CombatSystem;
using UnityEngine;

namespace StatusEffectSystem {
	[Serializable]
	[CreateAssetMenu(fileName = "StatusEffectData", menuName = "ScriptableObjects/Status Effect Data")]
	public class StatusEffectData : ScriptableObject {

		[SerializeField]
		[Tooltip("The name of the status effect")]
		public string Name;

		[SerializeField]
		[Tooltip("The image of the status effect")]
		public Sprite Sprite;

		[SerializeField]
		[Tooltip("If the same status effect is re-apply, if true; reset the duration to its original. if false; combine new duration and remaining duration.")]
		public bool Reset = true;

		[SerializeField]
		[Tooltip("Whether or not the status effect goes after a period of time. If true; the status effect will last for duration. If false; the status effect has to be manually cleared by an action.")]
		public bool Temporary = false;

		[SerializeField]
		[Tooltip("Whether or not the status effect can stack. If true; add a stack. Else do nothing.")]
		public bool CanStack = false;

		[SerializeField]
		[Tooltip("Whether or not the status effect will show in the UI. If true; hide the status. Else do show it.")]
		public bool Hidden = false;

		[SerializeField]
		[Tooltip("Determine how the status effect is displayed.")]
		public StatusEffectCategory StatusEffectCategory;

		[SerializeField]
		[Tooltip("Determine what is the effect of the status.")]
		public List<StatusEffectType> statusEffectTypes;

		[SerializeField]
		[Tooltip("The damage category type of the status effect (if applicable).")]
		public DamageCategoryType DamageCategoryType = DamageCategoryType.TYPELESS;

		[SerializeField]
		[Tooltip("The particle effect prefab to instantiate when the status effect is applied.")]
		public GameObject ParticleEffectPrefab;
	}
}

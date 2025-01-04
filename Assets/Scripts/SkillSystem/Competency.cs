using System;
using UnityEngine;

namespace SkillSystem {
	[Serializable]
	public class Competency {

		[SerializeField]
		[Tooltip("A I18N tag to identify this competency.")]
		public string Descriptive;

		[SerializeField]
		[Tooltip("The icon that represents this competency.")]
		public Sprite Icon;

		[SerializeField]
		[Range(1, 100)]
		[Tooltip("What is the required level to unlock this competency")]
		public int Requirement = 1;
	}
}

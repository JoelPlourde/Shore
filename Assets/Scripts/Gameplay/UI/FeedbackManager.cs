using System.Collections;
using System.Collections.Generic;
using SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay {
	public class FeedbackManager : MonoBehaviour {
		public static FeedbackManager Instance;

		private static readonly int CAPACITY = 10;

		public GameObject prefab;
		private CircularBuffer<FeedbackComponent> _circularBuffer = new CircularBuffer<FeedbackComponent>(CAPACITY);

		private void Awake() {
			Instance = this;

			for (int i = 0; i < CAPACITY; i++) {
				_circularBuffer.Insert(Instantiate(prefab, transform).GetComponent<FeedbackComponent>()).Disable();
			}
		}

		public void DisplayExperienceGain(Actor actor, SkillType skillType, int experience) {
			SkillData skillData = SkillManager.Instance.GetSkillData(skillType);
			string colorHex = ColorUtility.ToHtmlStringRGB(skillData.Color);
			DisplayMessage(actor, "<color=#" + colorHex + ">+" + experience + "</color>");
		}

		public void DisplayError(Actor actor, string error) {
			DisplayMessage(actor, "<color=#ff000000>+" + error + "</color>");
		}

		public void DisplayMessage(Actor actor, string message) {
			_circularBuffer.Next().Enable(actor, message, 3f);
		}
	}
}

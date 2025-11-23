using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class FeedbackComponent : MonoBehaviour {

		private readonly int MIN_FONT_SIZE = 40;
		private readonly int MAX_FONT_SIZE = 65;

		private RectTransform _rectTransform;
		private TextMeshProUGUI _text;

		private Actor _actor;

		/// <summary>
		/// Enable the Feedback component with a message
		/// </summary>
		/// <param name="actor">The actor to display the message on top of.</param>
		/// <param name="message">The message to display.</param>
		public void Enable(Actor actor, string message, float time = 1f) {

			if (ReferenceEquals(_text, null)) {
				_text = GetComponent<TextMeshProUGUI>();
			}

			if (ReferenceEquals(_rectTransform, null)) {
				_rectTransform = GetComponent<RectTransform>();
			}

			_actor = actor;
			_text.text = message;
			_text.fontSize = Mathf.Clamp((int)(-CameraSystem.CameraController.Instance.Distance + CameraSystem.CameraController.Instance.MaxZoom), MIN_FONT_SIZE, MAX_FONT_SIZE);

			gameObject.SetActive(true);
			_rectTransform.position = Camera.main.WorldToScreenPoint(_actor.transform.position + new Vector3(0, 2f, 0));

            // Animate the Hitsplat to fly diagonally upwards
            LeanTween.moveY(gameObject, _rectTransform.localPosition.y + 25f, 0.25f).setEase(LeanTweenType.easeOutCubic);

			LeanTween.alphaText(_rectTransform, 0f, time).setOnComplete(() => {
				Disable();
			});
		}

		/// <summary>
		/// Disable the Feedback component since its not needed anymore.
		/// </summary>
		public void Disable() {
			gameObject.SetActive(false);
		}
	}
}
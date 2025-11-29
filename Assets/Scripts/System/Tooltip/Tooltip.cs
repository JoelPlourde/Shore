using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	[RequireComponent(typeof(Canvas))]
	public class Tooltip : MonoBehaviour {

		private RectTransform _rectTransform;
		private RectTransform _childrenRectTransform;
		private ContentSizeFitter _contentSizeFitter;
		private Canvas _canvas;
		private Text _text;

		private Vector2 _padding = new Vector2(20, 10);
		private Vector2 _position = Vector2.zero;
		private Vector3 _offset = new Vector3(0, -30, 0);

		private Coroutine _delay;

		private void Awake() {
			if (ReferenceEquals(Instance, null)) {
				Instance = this;
			}

			_canvas = GetComponent<Canvas>();
			if (ReferenceEquals(_canvas, null)) {
				throw new UnityException("Canvas component is null.");
			}

			_text = GetComponentInChildren<Text>();
			if (ReferenceEquals(_text, null)) {
				throw new UnityException("Text component is null.");
			}

			_rectTransform = GetComponent<RectTransform>();
			_childrenRectTransform = transform.GetChild(0).GetComponent<RectTransform>();

			_canvas.enabled = false;
		}

		public void ShowTooltip(string text, float delay = 0f) {
			transform.SetAsLastSibling();
			_text.text = text;

			_delay = StartCoroutine(WaitForDelay(delay));
		}

		public void HideTooltip() {
			if (!ReferenceEquals(_delay, null)) {
				StopCoroutine(_delay);
				_delay = null;
			}
			CancelInvoke();
			_canvas.enabled = false;
		}

		private void FollowMouse() {
			_position = Input.mousePosition + _offset;
			if (_position.x + _rectTransform.sizeDelta.x >= Screen.width) {
				_position.x = Screen.width - _rectTransform.sizeDelta.x;
			} else if (_rectTransform.position.x <= 0) {
				_position.x = 0;
			}

			if (_position.y >= Screen.height) {
				_position.y = Screen.height;
			} else if (_position.y <= _rectTransform.sizeDelta.y) {
				_position.y = _rectTransform.sizeDelta.y;
			}

			_rectTransform.position = _position;
		}

		private IEnumerator WaitForDelay(float delay) {
			yield return new WaitForSecondsRealtime(delay);
			StartCoroutine(UpdateContentSize());
		}

		private IEnumerator UpdateContentSize() {
			yield return new WaitForEndOfFrame();
			_rectTransform.position = Input.mousePosition;
			_rectTransform.sizeDelta = _childrenRectTransform.sizeDelta + _padding;
			InvokeRepeating(nameof(FollowMouse), 0f, Constant.DEFAULT_REFRESH_RATE);
			_canvas.enabled = true;
		}

		public static Tooltip Instance { get; private set; }
	}
}
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
	[RequireComponent(typeof(Button))]
	public class InteractiveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

		private static readonly Vector3 SCALE_UP = new Vector3(1.2f, 1.2f, 1.2f);

		private static readonly Vector3 SCALE_DOWN = new Vector3(1.15f, 1.15f, 1.15f);

		[SerializeField]
		[Tooltip("The tooltip text to display when hovering over this button. Use I18N keys.")]
		protected string _tooltip = "";

		public void Initialize(string tooltip) {
			_tooltip = tooltip;
		}

		#region OnPointerEvent
		public virtual void OnPointerDown(PointerEventData eventData) {
			LeanTween.scale(gameObject, SCALE_DOWN, 0.05f);
		}

		public virtual void OnPointerUp(PointerEventData eventData) {
			LeanTween.scale(gameObject, SCALE_UP, 0.05f);
		}

		public virtual void OnPointerEnter(PointerEventData pointerEventData) {
			if (GetTooltip() != "") {
				Tooltip.Instance.ShowTooltip(GetTooltip(), Constant.TOOLTIP_DELAY);
			}

			LeanTween.scale(gameObject, SCALE_UP, 0.25f);

			SoundSystem.Instance.PlaySound(SoundManager.Instance.GetAudioClip("hover"), 0.2f);
		}

		public virtual void OnPointerExit(PointerEventData pointerEventData) {
			if (GetTooltip() != "") {
				Tooltip.Instance.HideTooltip();
			}

			if (!ReferenceEquals(gameObject, null)) {
				LeanTween.cancel(gameObject);
				LeanTween.scale(gameObject, Vector3.one, 0.1f);
			}
		}
		#endregion

		public virtual string GetTooltip() {
			return I18N.GetValue(_tooltip);
		}
	}
}

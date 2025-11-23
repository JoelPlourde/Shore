using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
	namespace Portrait {
		public class HealthBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
			private Slider _slider;
			private TextMeshProUGUI _text;

			private void Awake() {
				_slider = GetComponent<Slider>();
				_text = GetComponentInChildren<TextMeshProUGUI>();
				_text.gameObject.SetActive(false);
			}

			public void Initialize(Actor actor) {
				UpdateHealth(actor.Creature.Health / actor.Creature.MaxHealth);
				actor.Creature.OnUpdateHealthEvent += UpdateHealth;
			}

			public void OnDelete(Actor actor) {
				actor.Creature.OnUpdateHealthEvent -= UpdateHealth;
			}

			public void UpdateHealth(float percentage) {
				_slider.value = percentage;
				if (_text.gameObject.activeSelf) {
					_text.text = (_slider.value * 100f) + "%";
				}
			}

			public void OnPointerEnter(PointerEventData eventData) {
				_text.text = (_slider.value * 100f) + "%";
				_text.gameObject.SetActive(true);
			}

			public void OnPointerExit(PointerEventData eventData) {
				_text.gameObject.SetActive(false);
			}
		}
	}
}

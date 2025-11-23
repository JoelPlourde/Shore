using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI {
    public class Hitsplat : MonoBehaviour
    {
        private static readonly float HITSPLIT_DIMENSION = 75f;
        private static readonly float MOVE_Y_DISTANCE = 20f;

        private static readonly float FADE_SPEED = 1f;
        private static readonly float MOVE_SPEED = 0.75f;

        private TextMeshProUGUI _hitsplatText;
        private Image _backgroundImage;

        private void Awake()
        {
            _hitsplatText = GetComponentInChildren<TextMeshProUGUI>();
            _backgroundImage = GetComponentInChildren<Image>();

            Configure();
        }

        /// <summary>
        /// Unity has a tendency to mess with UI element configurations when instantiating prefabs.
        /// This method ensures that the Hitsplat UI elements maintain their intended configuration.
        /// </summary>
        private void Configure()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(HITSPLIT_DIMENSION, HITSPLIT_DIMENSION);

            _hitsplatText.rectTransform.anchorMin = new Vector2(0, 0);
            _hitsplatText.rectTransform.anchorMax = new Vector2(1, 1);
            _hitsplatText.rectTransform.offsetMin = new Vector2(0, 0);
            _hitsplatText.rectTransform.offsetMax = new Vector2(0, 0);

            _hitsplatText.alignment = TextAlignmentOptions.Center;
            _hitsplatText.autoSizeTextContainer = true;
            _hitsplatText.enableAutoSizing = true;
            _hitsplatText.fontSizeMax = 72;
            _hitsplatText.fontSize = 24;
            _hitsplatText.color = Color.white;

            _backgroundImage.rectTransform.anchorMin = new Vector2(0, 0);
            _backgroundImage.rectTransform.anchorMax = new Vector2(1, 1);
            _backgroundImage.rectTransform.offsetMin = new Vector2(0, 0);
            _backgroundImage.rectTransform.offsetMax = new Vector2(0, 0);
        }

        /// <summary>
        /// Show damage on the hitsplat.
        /// </summary>
        /// <param name="damage"></param>
        public void ShowDamage(int damage)
        {
            _hitsplatText.text = damage.ToString();
            _hitsplatText.enabled = true;
            _backgroundImage.enabled = true;

            // Animate the Hitsplat to fly diagonally upwards
            LeanTween.moveY(gameObject, transform.position.y + MOVE_Y_DISTANCE, MOVE_SPEED).setEase(LeanTweenType.easeOutCubic);

            // hitsplat: fade out over 1 second
            LeanTween.value(gameObject, 1f, 0f, FADE_SPEED).setOnUpdate(UpdateAlpha).setOnComplete(HideHitsplat);
        }

        private void UpdateAlpha(float alpha)
        {
            Color textColor = _hitsplatText.color;
            textColor.a = alpha;
            _hitsplatText.color = textColor;

            Color bgColor = _backgroundImage.color;
            bgColor.a = alpha;
            _backgroundImage.color = bgColor;
        }

        private void HideHitsplat()
        {
            _hitsplatText.enabled = false;
            _backgroundImage.enabled = false;
        }
    }
}

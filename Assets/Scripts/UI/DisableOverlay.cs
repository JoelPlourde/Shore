using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Component responsible for showing/hiding a tooltip when a disabled element is hovered.
    /// </summary>
    public class DisableOverlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private string _tooltipText;

        private void Awake()
        {
            Image image = GetComponent<Image>();
            if (image != null)
            {
                Color color = image.color;
                color.a = 0.5f; // Set alpha
                image.color = color;
            }
        }

        /// <summary>
        /// Initializes the DisableOverlay with the given tooltip text.
        /// </summary>
        /// <param name="tooltipText"></param>
        public void Initialize(string tooltipText)
        {
            _tooltipText = tooltipText;
        }

        #region Pointers
        /// <summary>
        /// Shows the tooltip when the pointer enters the overlay.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            Tooltip.Instance.ShowTooltip(_tooltipText);
        }

        /// <summary>
        /// Hides the tooltip when the pointer exits the overlay.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.Instance.HideTooltip();
        }
        #endregion
    }
}
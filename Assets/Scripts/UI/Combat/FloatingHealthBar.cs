using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class FloatingHealthBar : MonoBehaviour
    {
        private Slider _slider;
        private Transform _target;
        private float _heightOffset;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            Debug.Assert(_slider != null, "FloatingHealthBar: Slider component not found in children.");
            _slider.value = 1f;
        }

        /// <summary>
        /// Initialize the health bar with the target transform and initial health percentage.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="percentage"></param>
        public void Initialize(Transform target, float percentage = 1f, float heightOffset = 2.0f)
        {
            _target = target;
            _heightOffset = heightOffset;
            SetHealthPercentage(percentage);
        }

        /// <summary>
        /// Refresh the health bar with the new health percentage.
        /// </summary>
        /// <param name="percentage"></param>
        public void Refresh(float percentage)
        {
            SetHealthPercentage(percentage);
        }

        /// <summary>
        /// Set the health percentage on the slider.
        /// </summary>
        /// <param name="percentage"></param>
        private void SetHealthPercentage(float percentage)
        {
            if (ReferenceEquals(_slider, null))
            {
                _slider = GetComponent<Slider>();
            }
            _slider.value = Mathf.Clamp01(percentage);
        }

        public Transform Target => _target;

        public float HeightOffset => _heightOffset;
    }
}

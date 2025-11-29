using System.Collections.Generic;
using UI;
using UnityEngine;

namespace UI
{
    public class FloatingHealthBarHandler : MonoBehaviour
    {
        public static FloatingHealthBarHandler Instance;

        // Template for health bars
        private GameObject _healthBarTemplate;

        // List of Active Health Bars
        private List<FloatingHealthBar> _activeHealthBars = new List<FloatingHealthBar>();

        private void Awake()
        {
            Instance = this;

            _healthBarTemplate = GetComponentInChildren<FloatingHealthBar>(true).gameObject;
            _healthBarTemplate.SetActive(false);
        }

        public FloatingHealthBar InitializeHealthBar(Transform target, float healthPercentage, float heightOffset = 2.5f)
        {
            // Instantiate a new health bar from the template
            FloatingHealthBar healthBar = Instantiate(_healthBarTemplate, transform).GetComponent<FloatingHealthBar>();
            _activeHealthBars.Add(healthBar);

            healthBar.gameObject.SetActive(true);
            healthBar.transform.position = CalculatePosition(target.position, heightOffset);
            healthBar.Initialize(target, healthPercentage, heightOffset);

            // If the Routine isn't started, start it
            if (!IsInvoking("Routine"))
            {
                InvokeRepeating("Routine", 0f, 0.016f);
            }

            return healthBar;
        }

        public void HideHealthBar(FloatingHealthBar healthBar)
        {
            // Remove from active list
            _activeHealthBars.Remove(healthBar);

            // If no active health bars remain, stop the Routine
            if (_activeHealthBars.Count == 0)
            {
                CancelInvoke("Routine");
            }

            healthBar.gameObject.SetActive(false);

            // Destroy the health bar GameObject
            Destroy(healthBar.gameObject);
        }

        public void Routine()
        {
            // Iterate through active health bars and update their positions in reverse
            for (int i = _activeHealthBars.Count - 1; i >= 0; i--)
            {
                FloatingHealthBar healthBar = _activeHealthBars[i];

                if (ReferenceEquals(healthBar, null))
                {
                    HideHealthBar(healthBar);
                    continue;
                }

                if (healthBar.Target == null || ReferenceEquals(healthBar.Target, null)) {
                    HideHealthBar(healthBar);
                    continue;
                }

                // Update position
                healthBar.transform.position = CalculatePosition(healthBar.Target.position, healthBar.HeightOffset);
            }
        }

        private Vector3 CalculatePosition(Vector3 targetPosition, float heightOffset = 2.0f)
        {
            return Camera.main.WorldToScreenPoint(targetPosition + Vector3.up * heightOffset);
        }
    }
}
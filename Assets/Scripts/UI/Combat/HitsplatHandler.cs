using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Handles the display and management of hitsplats in the UI.
    /// </summary>
    public class HitsplatHandler : MonoBehaviour
    {
        private static readonly float DELAYED_CALL_TIMEOUT = 1.5f;

        public static HitsplatHandler Instance;

        // Template hitsplat for instantiation
        private Hitsplat _templateHitsplat;

        // Pool of hitsplats
        private Queue<Hitsplat> _hitsplatPool = new Queue<Hitsplat>();

        private void Awake()
        {
            Instance = this;

            // Get the first object of type Hitsplat in children as the template
            _templateHitsplat = GetComponentInChildren<Hitsplat>(true);
            _templateHitsplat.gameObject.SetActive(false);

            // Pre-instantiate a pool of hitsplats
            for (int i = 0; i < 10; i++)
            {
                Hitsplat hitsplat = Instantiate(_templateHitsplat, transform);
                hitsplat.transform.localPosition = Vector3.zero;
                hitsplat.transform.localRotation = Quaternion.identity;
                hitsplat.transform.localScale = Vector3.one;
                hitsplat.gameObject.SetActive(false);
                _hitsplatPool.Enqueue(hitsplat);
            }
        }

        /// <summary>
        /// Show a hitsplat for the given object with the specified damage.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="damage"></param>
        public void ShowHitsplat(Transform @object, int damage, float heightOffset = 2.0f)
        {
            heightOffset -= 0.5f; // Adjust for better positioning

            Vector3 position = Camera.main.WorldToScreenPoint(@object.position + new Vector3(0, heightOffset, 0));

            Hitsplat hitsplat = _hitsplatPool.Dequeue();
            
            // Activating the hitsplat before setting its position to ensure proper rendering
            hitsplat.gameObject.SetActive(true);
            hitsplat.transform.position = position;
            hitsplat.ShowDamage(damage);

            LeanTween.delayedCall(DELAYED_CALL_TIMEOUT, () =>
            {
                hitsplat.gameObject.SetActive(false);
                _hitsplatPool.Enqueue(hitsplat);
            });
        }
    }
}

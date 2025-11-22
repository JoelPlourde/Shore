using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class HitsplatHandler : MonoBehaviour
    {
        public static HitsplatHandler Instance;

        private int index;
        private Hitsplat[] _queuedHitsplats;

        public GameObject crab;
        public GameObject player;

        private void Awake()
        {
            Instance = this;

            // Get the first object of type Hitsplat in children as the template
            _queuedHitsplats = GetComponentsInChildren<Hitsplat>(true);
            index = _queuedHitsplats.Length - 1;

            // For each queued hitsplat, deactivate it and set its transform to default
            foreach (var hitsplat in _queuedHitsplats)
            {
                hitsplat.gameObject.SetActive(false);
                hitsplat.transform.localPosition = Vector3.zero;
                hitsplat.transform.localRotation = Quaternion.identity;
                hitsplat.transform.localScale = Vector3.one;
            }
        }

        public void ShowHitsplat(Transform @object, int damage)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(@object.position);
            Vector3 randomPosition = screenPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            ShowHitsplat(randomPosition, damage);
        }

        /// <summary>
        /// Show a hitsplat at the given position with the given damage.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="damage"></param>
        private void ShowHitsplat(Vector3 position, int damage)
        {
            Hitsplat hitsplat = _queuedHitsplats[index];
            index = (index - 1 + _queuedHitsplats.Length) % _queuedHitsplats.Length;
            
            hitsplat.GetComponent<RectTransform>().position = position;
            hitsplat.gameObject.SetActive(true);
            hitsplat.ShowDamage(damage);

            // Return the hitsplat to the pool after a delay
            StartCoroutine(ReturnHitsplatToPoolAfterDelay(hitsplat, 1.5f));
        }

        private System.Collections.IEnumerator ReturnHitsplatToPoolAfterDelay(Hitsplat hitsplat, float delay)
        {
            yield return new WaitForSeconds(delay);
            hitsplat.gameObject.SetActive(false);
        }
    }
}

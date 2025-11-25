using AbilitySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    namespace AbilitySystem
    {
        /// <summary>
        /// Placeholder class for AbilityEntry.
        /// </summary>
        public class AbilityEntry : MonoBehaviour
        {
            private AbilitySlotHandler _abilitySlotHandler;
            private TextMeshProUGUI _title;
            private TextMeshProUGUI _subTitle;

            private void Awake()
            {
                _abilitySlotHandler = GetComponentInChildren<AbilitySlotHandler>(true);
                _title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
                _subTitle = transform.Find("SubTitle").GetComponent<TextMeshProUGUI>();
            }

            public void Initialize(AbilityData abilityData)
            {
                // Initialize the AbilitySlotHandler as a read-only slot
                _abilitySlotHandler.Initialize(abilityData, true);

                _title.text = I18N.GetValue("abilities." + abilityData.name + ".name");
                _subTitle.enabled = false;

                if (abilityData.Passive)
                {
                    _subTitle.text = I18N.GetValue("passive");
                    _subTitle.enabled = true;
                }
            }
        }
    }
}

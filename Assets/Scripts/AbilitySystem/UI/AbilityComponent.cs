using AbilitySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    namespace AbilitySystem
    {
        public class AbilityComponent: MonoBehaviour
        {
            private Image _icon;
            private TextMeshProUGUI _title;
            private TextMeshProUGUI _subTitle;

            private void Awake()
            {
                _icon = transform.Find("Icon").GetComponent<Image>();
                _title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
                _subTitle = transform.Find("SubTitle").GetComponent<TextMeshProUGUI>();

                _subTitle.enabled = false; // Disable subtitle for now

                gameObject.SetActive(false);
            }

            public void UpdateAbility(AbilityData abilityData)
            {
                _icon.sprite = abilityData.Sprite;
                _title.text = I18N.GetValue("abilities." + abilityData.name + ".name");

                // Do nothing for now.
                // _subTitle.text = LocalizationManager.Instance.GetLocalizedText("abilities." + abilityData.name + ".description");
            }
        }
    }
}

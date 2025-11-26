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
            private DisableOverlay _disabledOverlay;

            private void Awake()
            {
                _abilitySlotHandler = GetComponentInChildren<AbilitySlotHandler>(true);
                _title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
                _subTitle = transform.Find("SubTitle").GetComponent<TextMeshProUGUI>();
                Transform disableOverlay = transform.Find("Disabled");
                if (ReferenceEquals(disableOverlay, null))
                {
                    return;
                }
                _disabledOverlay = disableOverlay.GetComponent<DisableOverlay>();
            }

            public void Initialize(Actor actor, AbilityData abilityData)
            {
                bool hasRequirements = true;
                if (!CheckIfActorHasSkillLevel(actor, abilityData, out string tooltip))
                {
                    hasRequirements = false;
                    _disabledOverlay.Initialize(tooltip);
                } else {
                    _disabledOverlay.gameObject.SetActive(false);
                }

                // Initialize the AbilitySlotHandler as a read-only slot
                _abilitySlotHandler.Initialize(abilityData, true, !hasRequirements);

                _title.text = I18N.GetValue("abilities." + abilityData.ID + ".name");
                _subTitle.enabled = false;

                if (abilityData.Passive)
                {
                    _subTitle.text = I18N.GetValue("passive");
                    _subTitle.enabled = true;
                }
            }

            /// <summary>
            /// Checks if the given actor meets the skill level requirements for the ability.
            /// </summary>
            /// <param name="actor"></param>
            /// <param name="abilityData"></param>
            /// <param name="tooltip"></param>
            /// <returns></returns>
            private bool CheckIfActorHasSkillLevel(Actor actor, AbilityData abilityData, out string tooltip)
            {
                bool hasRequirements = actor.Skills.GetLevel(abilityData.SkillType).Value >= abilityData.RequiredLevel;

                tooltip = "";
                if (!hasRequirements)
                {
                    tooltip = I18N.GetValue("required_level", abilityData.RequiredLevel);
                }

                return hasRequirements;
            }
        }
    }
}

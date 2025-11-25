using System.Collections.Generic;
using AbilitySystem;
using UnityEngine;

namespace UI
{
    namespace AbilitySystem
    {
        /// <summary>
        /// Component responsible for managing the ability bar UI element.
        /// </summary>
        public class AbilityBar: MonoBehaviour
        {
            public static AbilityBar Instance;

            private AbilitySlotHandler[] _abilitySlots;

            private Dictionary<int, AbilityData> _assignedAbilities = new Dictionary<int, AbilityData>();

            private void Awake()
            {
                Instance = this;

                _abilitySlots = GetComponentsInChildren<AbilitySlotHandler>(true);

                // For each AbilitySlotHandler, subscribe to its OnAbilityAssigned event
                foreach (AbilitySlotHandler slot in _abilitySlots)
                {
                    slot.OnAbilityAssigned += HandleAbilityAssigned;
                }
            }

            // Subscribe to ability assignment events for the given actor
            public void Subscribe(Actor actor)
            {
                actor.Creature.AbilityStateMachine.OnAbilityTriggered += TriggerGlobalCooldown;
            }

            public void Unsubscribe(Actor actor)
            {
                actor.Creature.AbilityStateMachine.OnAbilityTriggered -= TriggerGlobalCooldown;
            }

            public void TriggerGlobalCooldown(int slotIndex, float cooldownDuration, float globalCooldownDuration)
            {
                Debug.Log("Triggering global cooldown for ability slot: " + slotIndex);
                Debug.Log(_abilitySlots.Length);

                // For every ability slot, if the index DOES NOT match, trigger the cooldown UI
                foreach (AbilitySlotHandler slot in _abilitySlots)
                {
                    if (slot.IsEmpty)
                    {
                        continue;
                    }

                    int index = slot.transform.GetSiblingIndex();
                    if (index != slotIndex)
                    {
                        LeanTween.value(globalCooldownDuration, 0f, globalCooldownDuration).setOnUpdate(val =>
                        {
                            slot.UpdateCooldown(val);
                        });
                    }
                    else
                    {
                        LeanTween.value(cooldownDuration, 0f, globalCooldownDuration).setOnUpdate(val =>
                        {
                            slot.UpdateCooldown(val);
                        });
                    }
                }
            }

            private void OnDestroy()
            {
                // Unsubscribe from events to prevent memory leaks
                foreach (AbilitySlotHandler slot in _abilitySlots)
                {
                    slot.OnAbilityAssigned -= HandleAbilityAssigned;
                }
            }

            /// <summary>
            /// Handles the assignment of an ability to a slot.
            /// </summary>
            /// <param name="slotIndex"></param>
            /// <param name="abilityData"></param>
            public void HandleAbilityAssigned(int slotIndex, AbilityData abilityData)
            {
                // TODO: With the current selected actor, switch the ability assigned to the slot index
                _assignedAbilities[slotIndex] = abilityData;

                // Or else, the cooldown will be shown below.
                _abilitySlots[slotIndex].SetAbilityAsFirstSibling();
            }

            public void Update()
            {
                if (Input.GetKeyUp(KeyCode.Alpha1))
                {
                    TriggerAbility(0);
                }

                if (Input.GetKeyUp(KeyCode.Alpha2))
                {
                    TriggerAbility(1);
                }
                
                if (Input.GetKeyUp(KeyCode.Alpha3))
                {
                    TriggerAbility(2);
                }
                
                if (Input.GetKeyUp(KeyCode.Alpha4))
                {
                    TriggerAbility(3);
                }
                
                if (Input.GetKeyUp(KeyCode.Alpha5))
                {
                    TriggerAbility(4);
                }
            }

            /// <summary>
            /// Triggers the ability assigned to the given slot index.
            /// </summary>
            /// <param name="slotIndex"></param>
            public void TriggerAbility(int slotIndex)
            {
                if (_assignedAbilities.ContainsKey(slotIndex))
                {
                    AbilityData abilityData = _assignedAbilities[slotIndex];
                    Debug.Log("Using ability: " + abilityData.name + " from slot index: " + slotIndex);
                    // Here you would add the logic to actually use the ability
                }
                else
                {
                    Debug.Log("No ability assigned to slot index: " + slotIndex);
                }
            }
        }
    }
}

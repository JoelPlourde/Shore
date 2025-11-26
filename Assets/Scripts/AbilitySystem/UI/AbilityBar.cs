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

            private Actor _currentActor;

            private void Awake()
            {
                Instance = this;

                _abilitySlots = GetComponentsInChildren<AbilitySlotHandler>(true);

                // For each AbilitySlotHandler, subscribe to its OnAbilityAssigned event
                foreach (AbilitySlotHandler slot in _abilitySlots)
                {
                    slot.OnAbilityAssigned += HandleAbilityAssigned;
                }

                UserInputs.Instance.Subscribe(KeyCode.Alpha1, () => TriggerAbility(0));
                UserInputs.Instance.Subscribe(KeyCode.Alpha2, () => TriggerAbility(1));
                UserInputs.Instance.Subscribe(KeyCode.Alpha3, () => TriggerAbility(2));
                UserInputs.Instance.Subscribe(KeyCode.Alpha4, () => TriggerAbility(3));
                UserInputs.Instance.Subscribe(KeyCode.Alpha5, () => TriggerAbility(4));
            }

            // Subscribe to ability assignment events for the given actor
            public void Subscribe(Actor actor)
            {
                actor.Creature.AbilityStateMachine.OnAbilityTriggered += TriggerGlobalCooldown;
                _currentActor = actor;
            }

            public void Unsubscribe(Actor actor)
            {
                actor.Creature.AbilityStateMachine.OnAbilityTriggered -= TriggerGlobalCooldown;
                _currentActor = null;
            }

            /// <summary>
            /// Triggers the global cooldown UI for all ability slots.
            /// </summary>
            /// <param name="slotIndex"></param>
            /// <param name="cooldownDuration"></param>
            /// <param name="globalCooldownDuration"></param>
            public void TriggerGlobalCooldown(int slotIndex, float cooldownDuration, float globalCooldownDuration)
            {
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
                        if (slot.CooldownTween != null)
                        {
                            Debug.Log("Cooldown already in progress for slot index: " + index);
                            continue;
                        }

                        LeanTween.value(1f, 0f, globalCooldownDuration).setOnUpdate(val => {
                            slot.UpdateCooldown(val);
                        });
                    }
                    else
                    {
                        slot.CooldownTween = LeanTween.value(1f, 0f, cooldownDuration)
                        .setOnUpdate(val => {
                            slot.UpdateCooldown(val);
                        }).setOnComplete(() => {
                            slot.CooldownTween = null;
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

                UserInputs.Instance.Unsubscribe(KeyCode.Alpha1);
                UserInputs.Instance.Unsubscribe(KeyCode.Alpha2);
                UserInputs.Instance.Unsubscribe(KeyCode.Alpha3);
                UserInputs.Instance.Unsubscribe(KeyCode.Alpha4);
                UserInputs.Instance.Unsubscribe(KeyCode.Alpha5);
            }

            /// <summary>
            /// Handles the assignment of an ability to a slot.
            /// </summary>
            /// <param name="slotIndex"></param>
            /// <param name="abilityData"></param>
            public void HandleAbilityAssigned(int slotIndex, AbilityData abilityData)
            {
                if (_currentActor != null)
                {
                    _currentActor.Creature.AbilityStateMachine.AssignAbilityToSlot(slotIndex, abilityData);
                }

                _assignedAbilities[slotIndex] = abilityData;

                // Or else, the cooldown will be shown below.
                _abilitySlots[slotIndex].SetAbilityAsFirstSibling();
            }

            /// <summary>
            /// Triggers the ability assigned to the given slot index.
            /// </summary>
            /// <param name="slotIndex"></param>
            public void TriggerAbility(int slotIndex)
            {
                if (_assignedAbilities.ContainsKey(slotIndex))
                {
                    _currentActor.Creature.AbilityStateMachine.TriggerAbility(slotIndex);
                }
            }
        }
    }
}

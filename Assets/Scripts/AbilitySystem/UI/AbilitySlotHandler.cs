using System;
using AbilitySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    namespace AbilitySystem
    {
        [RequireComponent(typeof(GraphicRaycaster))]
        public class AbilitySlotHandler: MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
        {
            private bool _readOnly = false;

            // The AbilityComponent assigned to this slot.
            private AbilityComponent _ability;

            // The AbilityComponent being dragged.
            private AbilityComponent _draggedAbility;

            public Action<int, AbilityData> OnAbilityAssigned; 

            private void Awake() {
                _ability = transform.GetComponentInChildren<AbilityComponent>(true);
            }

            /// <summary>
            /// Initializes this AbilitySlotHandler with the given AbilityData.
            /// </summary>
            /// <param name="abilityData"></param>
            /// <param name="readOnly"></param>
            public void Initialize(AbilityData abilityData, bool readOnly = true)
            {
                _readOnly = readOnly;
                _ability.Initialize(abilityData);
            }

            /// <summary>
            /// Assigns the given AbilityComponent to this slot.
            /// </summary>
            /// <param name="abilityComponent"></param>
            public void AssignAbility(AbilityComponent abilityComponent)
            {
                _ability = abilityComponent;
                _ability.transform.SetParent(transform);
                _ability.transform.localPosition = Vector3.zero;

                OnAbilityAssigned?.Invoke(transform.GetSiblingIndex(), _ability.AbilityData);
            }

            #region Drag & Drop Handlers
            public void OnBeginDrag(PointerEventData eventData)
            {
                if (_ability.AbilityData.Passive)
                {
                    // Cannot drag passive abilities
                    return;
                }

                if (_readOnly)
                {
                    // Create a copy of the Ability component to be dragged
                    AbilityComponent abilityComponent = Instantiate(_ability, transform);
                    abilityComponent.Initialize(_ability.AbilityData);
                    abilityComponent.name = "Ability";
                    _draggedAbility = abilityComponent;
                } else {
                    // Drag the existing ability
                    _draggedAbility = _ability;
                }

                _draggedAbility.transform.SetParent(transform.root); // Move to top-level canvas to avoid being masked
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                if (ReferenceEquals(_draggedAbility, null))
                {
                    return;
                }

                // Raycast to check if dropped over a valid target but only on the Layer: AbilitySlot
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = eventData.position
                };

                var results = new System.Collections.Generic.List<RaycastResult>();

                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var result in results)
                {
                    AbilitySlotHandler destinationSlot = result.gameObject.GetComponent<AbilitySlotHandler>();
                    if (!ReferenceEquals(destinationSlot, null))
                    {
                        if (destinationSlot.IsReadOnly)
                        {
                            // Delete the ability if not dropped on a valid slot and in ReadOnly mode
                            Destroy(_draggedAbility.gameObject);
                            return;
                        }

                        if (destinationSlot.IsEmpty)
                        {
                            destinationSlot.AssignAbility(_draggedAbility);
                        } else
                        {
                            AbilityComponent existingAbility = destinationSlot.AbilityComponent;
                            destinationSlot.AssignAbility(_draggedAbility);
                            AssignAbility(existingAbility);
                        }
                        return;
                    }
                }

                // Delete the ability if not dropped on a valid slot and in ReadOnly mode
                Destroy(_draggedAbility.gameObject);
            }

            public void OnDrag(PointerEventData eventData)
            {
                if (ReferenceEquals(_draggedAbility, null))
                {
                    return;
                }
                _draggedAbility.transform.position = eventData.position;
            }
            #endregion

            /// <summary>
            /// Indicates whether this slot is empty (has no Ability component).
            /// </summary>
            public bool IsEmpty {
                get
                {
                    return ReferenceEquals(transform.Find("Ability"), null);
                }
            }

            /// <summary>
            /// Indicates whether this slot is read-only.
            /// </summary>
            public bool IsReadOnly {
                get { return _readOnly; }
            }

            // Gets the AbilityComponent in this slot.
            public AbilityComponent AbilityComponent {
                get { return _ability; }
            }
        }
    }
}

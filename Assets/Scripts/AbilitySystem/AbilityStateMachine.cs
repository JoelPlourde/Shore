using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem;
using SaveSystem;
using UI.AbilitySystem;
using UnityEngine;

namespace CombatSystem
{
    /// <summary>
    /// A class to manage combat abilities.
    /// 
    /// This class is a state machine that looks at what ability the creature is using
    /// and executes the corresponding logic for that ability.
    /// 
    /// Between ability, there is a "global" cooldown to prevent spamming abilities.
    /// </summary>
    public class AbilityStateMachine
    {
        private List<Ability> _abilities;

        private Creature _creature;

        private float _globalCooldownDuration = 1.0f;

        private bool _basicAttackCooldown = false;
        private bool _globalCooldown = false;

        // Event triggered when an ability is used. Parameters: slot index, cooldown duration
        public Action<int, float, float> OnAbilityTriggered;

        // For monsters/NPCs to use abilities automatically
        public void Initialize(Creature creature)
        {
            _creature = creature;
            _abilities = new List<Ability>
            {
                null,
                null,
                null,
                null,
                null
            };
            _globalCooldownDuration = Mathf.Min(1f, creature.AttackSpeed / 2f);
        }

        // For loading saved creatures with abilities
        public void Initialize(Creature creature, CreatureDto creatureDto)
        {
            Initialize(creature);
            
            for (int i = 0; i < creatureDto.AbilityDtos.Count; i++)
            {
                var abilityDto = creatureDto.AbilityDtos[i];
                if (abilityDto == null || string.IsNullOrEmpty(abilityDto.ID))
                {
                    continue;
                }

                try
                {
                    AbilityData abilityData = AbilityManager.Instance.GetAbilityData(abilityDto.ID);
                    Debug.Log("Loading ability for creature: " + abilityData.ID);
                    AssignAbilityToSlot(i, abilityData);
                }
                catch (UnityException ex)
                {
                    Debug.LogError("Error loading ability for creature: " + ex.Message);
                }
            }

            // If the creature is selected, update the Ability Bar UI
            //if (creature.GetComponent<Actor>().Selected)
            //{
                AbilityBar.Instance.Initialize(creature.GetComponent<Actor>());
            //}
        }

        /// <summary>
        /// Triggers a basic attack if not on cooldown.
        /// </summary>
        public void TriggerBasicAttack() {
            if (_basicAttackCooldown)
            {
                return;
            }

            // Raise the flag to indicate basic attack is on cooldown
            _basicAttackCooldown = true;

            // Trigger the attack animation
            _creature.Animator.SetTrigger("Attack");
            
            // Start the basic attack cooldown timer
            LeanTween.delayedCall(_creature.AttackSpeed, () =>
            {
                _basicAttackCooldown = false;
            });
        }

        /// <summary>
        /// Triggers the ability at the given index.
        /// </summary>
        /// <param name="index"></param>
        public void TriggerAbility(int index)
        {
            Creature target = _creature.CombatTargets.FirstOrDefault();
            if (ReferenceEquals(target, null) && ReferenceEquals(_creature, null))
            {
                Debug.Log("No target nor creature available for ability.");
                return;
            }

            if (_globalCooldown)
            {
                Debug.Log("Global cooldown active. Cannot use ability.");
                return;
            }

            if (_abilities[index] == null)
            {
                Debug.Log("No ability assigned to this slot.");
                return;
            }

            if (_abilities[index].OnCooldown)
            {
                Debug.Log("Ability " + _abilities[index].AbilityData.ID + " is on cooldown.");
                return;
            }

            // Set the flag for the global cooldown
            _globalCooldown = true;
            _abilities[index].OnCooldown = true;

            LeanTween.delayedCall(_abilities[index].AbilityData.Cooldown, () =>
            {
                _abilities[index].OnCooldown = false;
            });

            LeanTween.delayedCall(_globalCooldownDuration, () =>
            {
                _globalCooldown = false;
            });

            // Notify listeners about the ability usage
            OnAbilityTriggered?.Invoke(index, _abilities[index].AbilityData.Cooldown, _globalCooldownDuration);

            // Finally trigger the Ability
             _abilities[index].AbilityData.Execute(_creature, target);
        }

        public void AssignAbilityToSlot(int slotIndex, AbilityData abilityData)
        {
            _abilities[slotIndex] = new Ability { AbilityData = abilityData, OnCooldown = false };
        }

        public bool GlobalCooldown
        {
            get => _globalCooldown;
        }

        public List<Ability> GetAbilities()
        {
            return _abilities;
        }
    }
}

using System;
using System.Collections.Generic;
using AbilitySystem;
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
    public class Abilities
    {
        private List<Ability> _abilities;

        private Creature _creature;

        private float _globalCooldownDuration = 1.0f;
        
        private bool _globalCooldown = false;

        public void Initialize(Creature creature)
        {
            _creature = creature;
            _abilities = new List<Ability>();
            _globalCooldownDuration = creature.AttackSpeed;
        }

        public void TriggerNextAbility(Action OnAbilityCompleted)
        {
            Debug.Log("Attempting to trigger ability...");

            if (_globalCooldown)
            {
                return;
            }

            // Set global cooldown
            _globalCooldown = true;

            bool abilityFound = false;

            // Iterate through abilities and find an ability that is not on cooldown
            foreach (var ability in _abilities)
            {
                if (ability.OnCooldown)
                {
                    continue;
                }
                
                // Trigger the ability
                ability.OnCooldown = true;

                // Here you would add the logic to execute the ability's effect
                // For example, dealing damage, applying buffs/debuffs, etc.

                // Start the cooldown timer for the ability
                LeanTween.delayedCall(ability.AbilityData.Cooldown, () =>
                {
                    ability.OnCooldown = false;
                });

                // TODO: Update the UI

                abilityFound = true;

                break; // Exit after triggering one ability
            }

            if (!abilityFound)
            {
                // Fallback on basic attack
                _creature.Animator.SetTrigger("Attack");
            }

            Debug.Log("Global Cooldown Triggered for " + _globalCooldownDuration + " seconds.");

            LeanTween.delayedCall(_globalCooldownDuration, () => {
                Debug.Log("Global Cooldown Ended.");
                _globalCooldown = false;

                OnAbilityCompleted?.Invoke();
            });
        }

        public bool GlobalCooldown
        {
            get => _globalCooldown;
        }
    }

    public class Ability
    {
        public bool OnCooldown = false;
        public AbilityData AbilityData;
    }
}

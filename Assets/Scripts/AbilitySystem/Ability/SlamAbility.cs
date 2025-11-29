using StatusEffectSystem;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "SlamAbility", menuName = "ScriptableObjects/Abilities/Slam Ability")]
    public class SlamAbility : AbilityData
    {
        /// <summary>
        /// Executes the slam ability, triggering the slam animation on the creature.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="target"></param>
        public override void Execute(Creature creature, Creature target)
        {
            if (ReferenceEquals(target, null))
            {
                Debug.Log("No target available for slam ability.");
                return;
            }

            // Trigger the slam animation
            creature.Animator.SetTrigger("Slam");
        }

        /// <summary>
        /// Called when the slam animation ends to apply the stun effect to the target.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="target"></param>
        public override void OnAnimationEnded(Creature creature, Creature target)
        {
            StatusEffectData statusEffectData = StatusEffectManager.GetStatusEffectData(Constant.STUNNED);
            StatusEffectSystem.Status status = new StatusEffectSystem.Status(target, 1f, 2, statusEffectData);
		    target.StatusEffectScheduler.AddStatusEffect(status);
        }

        /// <summary>
        /// Gets the unique ID for the slam ability.
        /// </summary>
        /// <returns></returns>
        public override string GetID()
        {
            return "slam";
        }
    }
}
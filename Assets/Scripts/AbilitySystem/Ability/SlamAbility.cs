using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "SlamAbility", menuName = "ScriptableObjects/Abilities/Slam Ability")]
    public class SlamAbility : AbilityBehaviour
    {
        public override void Execute(Creature creature, Creature target)
        {
            if (ReferenceEquals(target, null))
            {
                Debug.Log("No target available for slam ability.");
                return;
            }

            creature.Animator.SetTrigger("Slam");

            // Implementation of the slam ability
            Debug.Log($"{creature.name} slams {target.name}!");
        }

        public void OnHit()
        {
            // Implementation of what happens when the slam hits
            Debug.Log("Slam hit!");
        }
    }
}

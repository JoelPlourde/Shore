using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "BoomAbility", menuName = "ScriptableObjects/Abilities/Boom Ability")]
    public class BoomAbility : AbilityBehaviour
    {
        public override void Execute(Creature creature, Creature target)
        {
            if (ReferenceEquals(target, null))
            {
                Debug.Log("No target available for slam ability.");
                return;
            }

            // Implementation of the boom ability
            Debug.Log($"{creature.name} booms {target.name}!");
        }
    }
}

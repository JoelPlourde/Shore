using StatusEffectSystem;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "ReflectAbility", menuName = "ScriptableObjects/Abilities/Reflect Ability")]
    public class ReflectAbility : AbilityData
    {
        public override void Execute(Creature creature, Creature target)
        {
            if (ReferenceEquals(creature, null))
            {
                Debug.LogError("Creature is null in ReflectAbility.Execute");
                return;
            }

            Actor actor = creature.GetComponent<Actor>();
            if (actor == null)
            {
                Debug.LogError("Actor component not found on creature in ReflectAbility.Execute");
                return;
            }

            StatusEffectScheduler.Instance(actor.Guid).AddStatusEffect(new StatusEffectSystem.Status(
                actor,
                1f,
                10, // 10 seconds duration
                StatusEffectManager.GetStatusEffectData(Constant.REFLECT))
            );
        }

        public override string GetID()
        {
            return "reflect";
        }
    }
}

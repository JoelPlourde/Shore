using UnityEngine;

namespace AbilitySystem
{
    /// <summary>
    /// Base class for all ability behaviours.
    /// </summary>
    public abstract class AbilityBehaviour : ScriptableObject
    {
        /// <summary>
        /// Executes the ability on the given creature and target.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="target"></param>
        public abstract void Execute(Creature creature, Creature target);
    }
}

using SkillSystem;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "ScriptableObjects/AbilityData")]
    public class AbilityData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The icon representing the ability.")]
        public Sprite Sprite;

        [SerializeField]
        [Tooltip("The background image for the ability.")]
        public Sprite Background;

        [SerializeField]
        [Tooltip("The skill type associated with this ability.")]
        public SkillType SkillType;

        [SerializeField]
        [Tooltip("The level required to unlock this ability.")]
        public int SkillLevel;

        [SerializeField]
        [Tooltip("Indicates whether this ability is passive.")]
        public bool Passive;

        [SerializeField]
        [Tooltip("The cooldown time for this ability in seconds.")]
        public float Cooldown;
    }
}

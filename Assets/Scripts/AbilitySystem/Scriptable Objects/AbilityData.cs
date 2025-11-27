using ItemSystem.EquipmentSystem;
using SkillSystem;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "ScriptableObjects/Ability Data")]
    public class AbilityData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The icon representing the ability.")]
        public Sprite Sprite;

        [SerializeField]
        [Tooltip("(Optional) The background image for the ability.")]
        public Sprite Background;

        [SerializeField]
        [Tooltip("The skill type associated with this ability.")]
        public SkillType SkillType;

        [SerializeField]
        [Tooltip("The level required to unlock this ability.")]
        public int RequiredLevel;

        [SerializeField]
        [Tooltip("The weapon damage type required to use this ability.")]
        public WeaponDamageType RequiredWeaponDamageType;

        [SerializeField]
        [Tooltip("Indicates whether this ability requires a shield to be equipped.")]
        public bool RequiresShield;

        [SerializeField]
        [Tooltip("Indicates whether this ability is passive.")]
        public bool Passive;

        [SerializeField]
        [Tooltip("The cooldown time for this ability in seconds.")]
        public float Cooldown;

        [SerializeField]
        [Tooltip("The behaviour script that defines the ability's actions.")]
        public AbilityBehaviour AbilityBehaviour;

        public string ID { get; set; }
    }
}

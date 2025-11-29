using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DropSystem;
using SkillSystem;

namespace CombatSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObjects/Monster Data")]
    public class MonsterData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The speed at which the NPC walks.")]
        public float WalkingSpeed = 1.0f;

        [SerializeField]
        [Tooltip("The time between each actions.")]
        public float TimeBetweenActions = 2.0f;

        [SerializeField]
        [HideInInspector]
        [Tooltip("The forward direction of the NPC.")]
        public int ForwardOffset = 0;

        [SerializeField]
        [HideInInspector]
        [Tooltip("Determine whether or not the NPC wanders around.")]
        public bool Wanders = true;

        [SerializeField]
        [HideInInspector]
        [Tooltip("The radius within which the NPC wanders from its spawn point.")]
        public float WanderingRadius;

        [SerializeField]
        [HideInInspector]
        [Tooltip("Determine whether or not the NPC can be attacked.")]
        public bool Attackable = false;

        [SerializeField]
        [Tooltip("The size of the monster for spawning and placement purposes.")]
        public float Size = 1.0f;

        [SerializeField]
        [Tooltip("The height of the monster for spawning and placement purposes.")]
        public float Height = 2.0f;

        [SerializeField]
        [Tooltip("The health of the monster.")]
        public float Health = 100.0f;

        [SerializeField]
        [Tooltip("The damage the monster can inflict.")]
        public float Damage = 1.0f;

        [SerializeField]
        [Tooltip("The damage category type of the monster.")]
        public DamageCategoryType DamageCategoryType = DamageCategoryType.TYPELESS;

        [SerializeField]
        [Tooltip("The attack range of the monster.")]
        public float AttackRange = 1.0f;

        [SerializeField]
        [Tooltip("The attack speed of the monster.")]
        public float AttackSpeed = 1.0f;

        [SerializeField]
        [Tooltip("Determine whether or not the monster can be looted.")]
        public bool Lootable = true;

        [SerializeField]
        [Tooltip("The drop table associated with this monster.")]
        public DropTable DropTable;

        [SerializeField]
        [Tooltip("The experience gains granted by this monster upon defeat.")]
        public int Experience;

        public string ID { get; set; }
    }
}
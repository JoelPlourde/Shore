using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MonsterSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObjects/Monster Data")]
    public class MonsterData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Prefab of this item as it appears in the world.")]
        public GameObject Prefab;

        [SerializeField]
        [Tooltip("The speed at which the NPC walks.")]
        public float WalkingSpeed = 1.0f;

        [SerializeField]
        [Tooltip("The time between each actions.")]
        public float TimeBetweenActions = 2.0f;

        [SerializeField]
        [HideInInspector]
        [Tooltip("Determine whether or not the NPC wanders around.")]
        public Boolean Wanders = true;

        [SerializeField]
        [HideInInspector]
        [Tooltip("The radius within which the NPC wanders from its spawn point.")]
        public float WanderingRadius;

        [SerializeField]
        [HideInInspector]
        [Tooltip("Determine whether or not the NPC can be attacked.")]
        public Boolean Attackable = false;

        public string ID { get; set; }
    }
}
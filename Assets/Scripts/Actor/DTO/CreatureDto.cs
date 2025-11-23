using UnityEngine;
using System;

namespace SaveSystem
{
    [Serializable]
    public class CreatureDto
    {
		[SerializeField]
		public float MaxHealth;

		[SerializeField]
		public float Health;

        [SerializeField]
        public float Damage;

        public CreatureDto() {
            MaxHealth = Constant.DEFAULT_HEALTH;
            Health = Constant.DEFAULT_HEALTH;
            Damage = Constant.DEFAULT_DAMAGE;
        }

        public CreatureDto(Creature creature)
        {
            this.MaxHealth = creature.MaxHealth;
            this.Health = creature.Health;
            this.Damage = creature.Damage;
        }
    }
}
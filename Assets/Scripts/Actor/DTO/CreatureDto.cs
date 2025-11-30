using UnityEngine;
using System;
using System.Collections.Generic;
using CombatSystem;

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

        [SerializeField]
        public List<AbilityDto> AbilityDtos;

        public CreatureDto() {
            MaxHealth = Constant.DEFAULT_HEALTH;
            Health = Constant.DEFAULT_HEALTH;
            Damage = Constant.DEFAULT_DAMAGE;
            AbilityDtos = new List<AbilityDto>();
        }

        public CreatureDto(Creature creature)
        {
            this.MaxHealth = creature.MaxHealth;
            this.Health = creature.Health;
            this.Damage = creature.Damage;
            this.AbilityDtos = new List<AbilityDto>();

            foreach (var ability in creature.AbilityStateMachine.GetAbilities())
            {
                if (ability == null)
                {
                    AbilityDtos.Add(null);
                } else
                {
                    AbilityDtos.Add(new AbilityDto { ID = ability.AbilityData.GetID() });
                }
            }
        }
    }
}
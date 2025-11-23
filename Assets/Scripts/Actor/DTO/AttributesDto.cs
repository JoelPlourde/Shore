using System;
using UnityEngine;

namespace SaveSystem {
	[Serializable]
	public class AttributesDto {

		[SerializeField]
		public float HealthRegeneration;

		[SerializeField]
		public float Speed;

		[SerializeField]
		public float Damage;

		[SerializeField]
		public float HungerRate;

		[SerializeField]
		public float Food;

		[SerializeField]
		public float Temperature;

		public AttributesDto() {
			HealthRegeneration = Constant.DEFAULT_HEALTH_REGENERATION;
			Speed = Constant.DEFAULT_SPEED;
			Damage = Constant.DEFAULT_DAMAGE;
			HungerRate = Constant.DEFAULT_HUNGER_RATE;
			Food = Constant.DEFAULT_FOOD;
			Temperature = Constant.DEFAULT_TEMPERATURE;
		}

		public AttributesDto(Attributes attributes) {
			if (ReferenceEquals(attributes, null)) {
				attributes = new Attributes();
			}

			HealthRegeneration = attributes.HealthRegeneration;
			Speed = attributes.Speed;
			HungerRate = attributes.HungerRate;
			Food = attributes.Food;
			Temperature = attributes.Temperature;
		}
	}
}

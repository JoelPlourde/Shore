using UnityEngine;
using FactionSystem;
using System;

namespace SaveSystem {
	[Serializable]
	public class ActorDto {

		[SerializeField]
		public Vector3 Position;

		[SerializeField]
		public Vector3 Rotation;

		[SerializeField]
		public string Guid;

		[SerializeField]
		public bool Playable;

		[SerializeField]
		public FactionType FactionType;

		[SerializeField]
		public CreatureDto CreatureDto;

		[SerializeField]
		public AttributesDto AttributesDto;

		[SerializeField]
		public StatusDto StatusDto;

		[SerializeField]
		public FeaturesDto FeaturesDto;

		[SerializeField]
		public InventoryDto InventoryDto;

		[SerializeField]
		public ArmoryDto ArmoryDto;

		[SerializeField]
		public SkillsDto SkillsDto;

		public ActorDto() {
			Position = Vector3.zero;
			Rotation = Vector3.zero;
			Guid = System.Guid.NewGuid().ToString();
			Playable = true;
			FactionType = FactionType.FACTIONLESS;
			CreatureDto = new CreatureDto();
			AttributesDto = new AttributesDto();
			StatusDto = new StatusDto();
			FeaturesDto = new FeaturesDto();
			InventoryDto = new InventoryDto();
			ArmoryDto = new ArmoryDto();
			SkillsDto = new SkillsDto();
		}

		public ActorDto(Actor actor) {
			Position = actor.transform.position;
			Rotation = actor.transform.eulerAngles;
			Guid = actor.Guid.ToString();
			Playable = actor.Playable;
			FactionType = actor.FactionType;
			CreatureDto = new CreatureDto(actor.Creature);
			AttributesDto = new AttributesDto(actor.Attributes);
			StatusDto = new StatusDto(actor.Status);
			FeaturesDto = new FeaturesDto(actor.Body, actor.Face);
			InventoryDto = new InventoryDto(actor.Inventory);
			ArmoryDto = new ArmoryDto(actor.Armory);
			SkillsDto = new SkillsDto(actor.Skills);
		}
	}
}

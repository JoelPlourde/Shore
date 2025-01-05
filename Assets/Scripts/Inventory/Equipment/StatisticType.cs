namespace ItemSystem {
	namespace EquipmentSystem {
		public enum StatisticType {
			NONE = 0, 				 	// This value is used to indicate that the item does not have any statistic.
			ARMOR = 1,                  // This value correlates with the damage reduction.
			CONSTITUTION = 2,           // This value indicates the amount of health, health regeneration
			STRENGTH = 3,               // This value is linked to melee-based attacks
			INTELLIGENCE = 4,           // This value is linked to magic-based attacks
			DEXTERITY = 5,              // This value is linked to ranged-based attacks.
			FAITH = 6,                  // This value is linked to protection-based spells
			COLD_RESISTANCE = 7,        // This value correlates with the ability to adapt to cold environment
			HEAT_RESISTANCE = 8,        // This value correlates with the ability to adapt to hot environment
			MINING_SPEED = 9,           // This value correlates with the speed at which the character strike a mining node.
			MINING_STRENGTH = 10,        // This value correlates with the damage inflicted to the mining node.
			LUCK = 11,                   // This value correlates with the ability to critically strike a node.
			WOODWORKING_SPEED = 12,		// This value correlates with the speed at which the character strike a lumber node.
			WOODWORKING_STRENGTH = 13	// This value correlates with the damage inflicted to the lumber node.
		}
	}
}

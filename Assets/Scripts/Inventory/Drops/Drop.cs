using UnityEngine;
using ItemSystem;

namespace DropSystem {
	[System.Serializable]
	public class Drop {

		[Tooltip("The item to roll.")]
		[SerializeField]
		public ItemData ItemData;

		[Tooltip("The relative weight of this drop.")]
		[Range(0, 100)]
		[SerializeField]
		public uint Weight;

		[Tooltip("The quantity of item to return on roll.")]
		[SerializeField]
		public Quantity Quantity;

		[Tooltip("Whether or not this drop is guaranteed.")]
		[SerializeField]
		public bool Guaranteed = false;

		private int _amount = 0;

		/// <summary>
		/// Get the drop amount based on the minimum quantity and the maximum quantity.
		/// </summary>
		/// <returns></returns>
		public int Amount { get { return (_amount == 0) ? Random.Range((int)Quantity.Min, (int)(Quantity.Max + 1)) : _amount; } }

		public override string ToString() {
			return ItemData.name + "x" + Amount;
		}

		public Item ToItem() {
			return new Item(ItemData, Amount);
		}
	}
}

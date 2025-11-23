using UnityEngine;

namespace DropSystem {
	[System.Serializable]
	[CreateAssetMenu(fileName = "DropTable", menuName = "ScriptableObjects/Drop Table")]
	public class DropTable : ScriptableObject {

		[Tooltip("The drops that defines this drop table.")]
		[SerializeField]
		public Drop[] Drops = new Drop[0];

		private int _sum;

		private void Awake() {
			ID = name.ToLower().Replace(" ", "_");
			_sum = 0;
			foreach (Drop drop in Drops) {
				_sum += (int)drop.Weight;
			}
		}

		/// <summary>
		/// Get a random drop based on the defined drop table.
		/// </summary>
		/// <returns>Return a single drop.</returns>
		public Drop GetRandomDrop() {
			int weight = 0;
			foreach (Drop drop in Drops) {
				weight += (int) drop.Weight;
				if (weight > Random.Range(0, _sum)) {
					return drop;
				}
			}
			return Drops[Drops.Length - 1];
		}

		public void GetRandomDrops(ref System.Collections.Generic.List<Drop> drops) {
			// Check if there are any guaranteed drops first
			foreach (Drop drop in Drops) {
				if (drop.Guaranteed) {
					drops.Add(drop);
				}
			}
			// Then get a random drop
			drops.Add(GetRandomDrop());
		}

		public string ID { get; set; }
	}
}

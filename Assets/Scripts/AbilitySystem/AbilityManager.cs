using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AbilitySystem
{
	[ExecuteInEditMode]
	public class AbilityManager : MonoBehaviour {

		public static AbilityManager Instance;

		private Dictionary<string, AbilityData> _abilityDatas;
        
		private void Awake() {
			Instance = this;

			AbilityData[] abilityDatas = Resources.LoadAll<AbilityData>("Scriptable Objects/Abilities");
			_abilityDatas = abilityDatas.ToDictionary(x => x.GetID());
		}

        public AbilityData GetAbilityData(string name) {
			if (_abilityDatas.TryGetValue(name.ToLower().Replace(" ", "_"), out AbilityData abilityData)) {
				return abilityData;
			} else {
				throw new UnityException("The Ability couldn't be found by its name. Please define this Ability Data: " + name);
			}
		}
    }
}
﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConstructionSystem {

	/// <summary>
	/// This Scriptable Object is a way to organize ConstructionData in its respective categories.
	/// </summary>
	[Serializable]
	[CreateAssetMenu(fileName = "ArchitectData", menuName = "ScriptableObjects/Construction/Architect Data")]
	public class Architect : ScriptableObject {

		[SerializeField]
		public List<Category> Categories;
	}

	[Serializable]
	public class Category {

		[SerializeField]
		[Tooltip("Name of the Category")]
		public string Name;

		[SerializeField]
		[Tooltip("Describe what the category is")]
		[TextArea(3, 15)]
		public string Tooltip;

		[SerializeField]
		[Tooltip("Image of the Category")]
		public Sprite Sprite;

		[SerializeField]
		public List<ConstructionData> ConstructionDatas;
	}
}

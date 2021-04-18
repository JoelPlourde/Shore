﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem {
	[Serializable]
	public class StatusDto {

		[SerializeField]
		public bool Dead;

		[SerializeField]
		public bool Stunned;

		[SerializeField]
		public bool Fleeing;

		[SerializeField]
		public bool Sheltered;

		public StatusDto(Status status) {
			if (ReferenceEquals(status, null)) {
				status = new Status();
			}

			Dead = status.Dead;
			Stunned = status.Stunned;
			Fleeing = status.Fleeing;
			Sheltered = status.Sheltered;
		}
	}
}

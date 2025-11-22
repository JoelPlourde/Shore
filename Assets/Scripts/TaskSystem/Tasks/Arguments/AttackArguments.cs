using System.Collections;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;

namespace TaskSystem {
	public class AttackArguments : ITaskArguments {

		public Creature Target { get; private set; }

		public AttackArguments(Creature target) {
			Target = target;
		}

		TaskType ITaskArguments.GetTaskType() {
			return TaskType.ATTACK;
		}
	}
}

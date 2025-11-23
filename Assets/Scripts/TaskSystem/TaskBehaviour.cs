using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaskSystem {
	public abstract class TaskBehaviour : MonoBehaviour {

		protected Actor actor;
		protected Creature creature;

		public TaskPriority TaskPriority { get; private set; }
		public ITaskArguments TaskArguments { get; private set; }

		public Guid guid = Guid.NewGuid();
		public event Action<TaskBehaviour> OnEndCallback;

		private void Awake() {
			actor = GetComponent<Actor>();
			creature = GetComponent<Creature>();
		}

		public virtual void Initialize(ITaskArguments taskArguments, TaskPriority taskPriority) {
			TaskArguments = taskArguments;
			TaskPriority = taskPriority;
		}

		public virtual void OnEnd() {
			OnEndCallback?.Invoke(this);
		}

		// Define the code to run when the task should execute
		public abstract void Execute();

		// Define the behaviour when two task of the same type are combined.
		public abstract void Combine(ITaskArguments taskArguments);
	}
}

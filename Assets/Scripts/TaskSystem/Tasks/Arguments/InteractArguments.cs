using UnityEngine;

namespace TaskSystem {
	public class InteractArguments : ITaskArguments {

		public IInteractable Interactable;
		public Transform Transform;

		public InteractArguments(Transform transform, IInteractable interactable) {
			Transform = transform;
			Interactable = interactable;
		}

		public TaskType GetTaskType() {
			return TaskType.INTERACT;
		}
	}
}

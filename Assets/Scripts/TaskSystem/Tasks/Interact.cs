using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static TriggerManager;

namespace TaskSystem {
	public class Interact : TaskBehaviour {

		private InteractArguments _interactArguments;
		private Trigger trigger;

		public override void Execute() {
			// Validate the arguments you've received is of the correct type.
			_interactArguments = TaskArguments as InteractArguments;

			TriggerOptions options = new TriggerOptions {
				Radius = _interactArguments.Interactable.GetInteractionRadius(),
				Color =  InteractionManager.Instance.GetOutlineMaterial(_interactArguments.Interactable.GetOutlineType()).GetColor("_OutlineColor")
			};

			trigger = TriggerManager.CreateTrigger(_interactArguments.Transform, _interactArguments.Interactable.GetInteractionRadius(), OnTriggerEnterCondition, AtDestination, options);

			if (Vector3.Distance(actor.transform.position, _interactArguments.Transform.position) > _interactArguments.Interactable.GetInteractionRadius()) {
				actor.NavMeshAgent.SetDestination(_interactArguments.Transform.position);
				actor.NavMeshAgent.isStopped = false;

				actor.Animator.SetFloat("Speed", actor.NavMeshAgent.speed);
				actor.Animator.SetBool("Move", true);
			}
		}

		public override void Combine(ITaskArguments taskArguments) {
			base.Initialize(taskArguments, TaskPriority);
			if (trigger) {
				trigger.Destroy();
			}
			Execute();
		}

		private void AtDestination() {
			Vector3 targetDirection = _interactArguments.Transform.position - actor.transform.position;
			Vector3 currentDirection = actor.transform.forward;

			// Calculate the angle between the two vectors using the Cross Product
			float angle = Vector3.SignedAngle(currentDirection, targetDirection, Vector3.up);

			// Add the angle to the current rotation
			float y = actor.transform.rotation.eulerAngles.y + angle;
			LeanTween.rotateY(actor.gameObject, y, 0.5f);

			_interactArguments.Interactable.OnInteractEnter(actor);
			actor.NavMeshAgent.isStopped = true;
			actor.Animator.SetBool("Move", false);
			if (_interactArguments.Interactable.IsPickup()) {
				actor.Animator.SetTrigger("Pickup");
			}
		}

		public override void OnEnd() {
			_interactArguments.Interactable.OnInteractExit(actor);
			base.OnEnd();
			if (trigger) {
				trigger.Destroy();
			}
			if (actor.NavMeshAgent.isOnNavMesh) {
				actor.NavMeshAgent.isStopped = true;
			}
			actor.Animator.SetBool("Move", false);
		}

		public void OnDestroy() {
			if (trigger) {
				trigger.OnTriggerEnterEvent -= AtDestination;
			}
		}

		private bool OnTriggerEnterCondition(Collider collider) {
			if (this) {
				return collider.gameObject.name == name;
			} else {
				return false;
			}
		}
	}
}

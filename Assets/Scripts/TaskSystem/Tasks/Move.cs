using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TaskSystem {
	public class Move : TaskBehaviour {

		private NavMeshAgent navMeshAgent;
		private Trigger trigger;

		public override void Execute() {
			// Validate the arguments you've received is of the correct type.
			MoveArguments moveArguments = TaskArguments as MoveArguments;

			if (trigger == null) {
				// Create a trigger at destination.
				trigger = TriggerManager.CreateTrigger(moveArguments.Position, moveArguments.Radius, OnTriggerEnterCondition, OnEnd);
			} else {
				// Re-use the same trigger, simply move the position of it.
				trigger.transform.position = moveArguments.Position;
			}

			// Start the NavMeshAgent.
			creature.NavMeshAgent.SetDestination(moveArguments.Position);
			creature.NavMeshAgent.isStopped = false;

			float angle = Vector3.Angle(transform.forward, (moveArguments.Position - transform.position));
			if (angle > 160) {
				creature.Animator.SetTrigger("Turn");
			}

			// Send the animation.
			creature.Animator.SetFloat("Speed", creature.NavMeshAgent.speed);
			creature.Animator.SetBool("Move", true);
		}

		public override void Combine(ITaskArguments taskArguments) {
			base.Initialize(taskArguments, TaskPriority);
			Execute();
		}

		public override void OnEnd() {
			base.OnEnd();
			trigger?.Destroy();
			creature.NavMeshAgent.isStopped = true;
			creature.Animator.SetBool("Move", false);
		}

		public void OnDestroy() {
			// Unsubscribe to the event.
			if (trigger) {
				trigger.OnTriggerEnterEvent -= OnEnd;
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

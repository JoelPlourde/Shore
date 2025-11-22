using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TaskSystem {
	public class Flee : TaskBehaviour {

		private NavMeshAgent navMeshAgent;
		private FleeArguments fleeArguments;

		public override void Combine(ITaskArguments taskArguments) {
			base.Initialize(taskArguments, TaskPriority);
			Execute();
		}

		public override void Execute() {
			// Validate the arguments you've received is of the correct type.
			FleeArguments fleeArguments = TaskArguments as FleeArguments;

			Debug.Log(name + " is fleeing !!");

			// Start the NavMeshAgent.
			navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
			navMeshAgent.SetDestination(fleeArguments.Direction);
			navMeshAgent.isStopped = false;

			creature.Animator.SetBool("Move", true);
		}

		public override void OnEnd() {
			base.OnEnd();
			navMeshAgent.isStopped = true;
			creature.Animator.SetBool("Move", false);
		}
	}
}

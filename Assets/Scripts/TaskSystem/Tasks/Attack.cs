using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;
using MonsterSystem;

namespace TaskSystem {

	public class Attack : TaskBehaviour {

		NavMeshAgent navMeshAgent;
		AttackArguments attackArguments;

		private bool _canAttack = true;

		private Trigger _trigger;

		public override void Combine(ITaskArguments taskArguments) {
			base.Initialize(taskArguments, TaskPriority);
			Execute();
		}

		public override void Execute() {
			creature = gameObject.GetComponent<Creature>();

			// Validate the arguments you've received is of the correct type.
			attackArguments = TaskArguments as AttackArguments;

			// First of all, verify if you or the target is dead.
			if (attackArguments.Target.Dead || creature.Dead) {
				OnEnd();
				return;
			}

			navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

			Routine();
		}

		private void Routine() {
			Debug.Log(creature.name + " is executing Attack task against " + attackArguments.Target.name + ".");
			if (!this) {
				return;
			}

			if (creature.Dead || attackArguments.Target.Dead) {
				OnEnd();
				return;
			}

			if (CheckIfCloseToTarget()) {
				CancelInvoke(nameof(MoveRoutine));

				AttackState();
			} else {
				MoveState();
			}
		}

		private void MoveState() {
			if (creature.NavMeshAgent.isPathStale) {
				OnEnd();
				return;
			}

			if (ReferenceEquals(_trigger, null))
            {
                _trigger = TriggerManager.CreateTrigger(attackArguments.Target.transform, creature.AttackRange, OnTriggerEnterCondition, Routine);
				_trigger.name = name + "_AttackTrigger";
			}

			if (!IsInvoking(nameof(MoveRoutine)))
            {
				InvokeRepeating(nameof(MoveRoutine), 0f, 0.5f);
            }
		}

		private void MoveRoutine()
        {
            navMeshAgent.destination = attackArguments.Target.transform.position;
			navMeshAgent.isStopped = false;
			creature.Animator.SetBool("Move", true);
        }

		private bool OnTriggerEnterCondition(Collider other)
        {
			if (ReferenceEquals(other, null))
			{
				return false;
			}
            return other == GetComponent<Collider>();
        }

		private void AttackState() {
			creature.Animator.SetBool("Move", false);
			navMeshAgent.isStopped = true;
			if (_canAttack) {
				_canAttack = false;
				AttackNow();
			}
		}

		private void AttackNow() {
			creature.EnterCombat(attackArguments.Target);
			
			LookAtTarget();

			if (attackArguments.Target.Dead || creature.Dead) {
				// Exit combat if either the attacker or the target is dead.
				creature.ExitCombat(attackArguments.Target);
				attackArguments.Target.ExitCombat(creature);

				OnEnd();
				return;
			}

			attackArguments.Target.SufferDamage(creature.Damage, creature);
			creature.Animator.SetTrigger("Attack");

			// Set a cooldown before the next attack can occur.
			TimerManager.Instance.Enqueue(new DelayedAction(ResetAttackCooldown, creature.AttackSpeed));
		}

		private void LookAtTarget() {
			// Rotate to face the target offset by the MonsterData forward vector.
			Vector3 directionToTarget = attackArguments.Target.transform.position - transform.position;
			directionToTarget.y = 0f;
			directionToTarget.Normalize();

			// Determine left/right
			float signedAngle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);

			// Apply offset based on side
			float offset = creature.ForwardOffset;
			float signedOffset = offset * Mathf.Sign(signedAngle);

			// Final rotation
			Quaternion lookRot = Quaternion.LookRotation(directionToTarget, Vector3.up);
			Quaternion finalRot = lookRot * Quaternion.Euler(0f, signedOffset, 0f);

			LeanTween.rotate(gameObject, finalRot.eulerAngles, 0.2f).setEase(LeanTweenType.easeOutQuad);
		}

		private void ResetAttackCooldown() {
			// Allow the creature to attack again.
			_canAttack = true;

			Debug.Log(creature.name + " attack cooldown reset.");

			// Continue the attack sequence if still in range.
			Routine();
		}

		public override void OnEnd() {
			// Huh ?!?!
			// CancelInvoke(nameof(MoveRoutine));

			//_trigger?.Destroy();
			//_trigger = null;
			
			if (navMeshAgent.isActiveAndEnabled) {
				navMeshAgent.isStopped = true;
			}
			base.OnEnd();
		}

		private bool CheckIfCloseToTarget() {
			float distanceToTarget = Vector3.Distance(attackArguments.Target.transform.position, transform.position);
			distanceToTarget -= attackArguments.Target.Size;
			distanceToTarget -= creature.AttackRange;
			return distanceToTarget <= attackArguments.Target.Size;
		}
	}
}

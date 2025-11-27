using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.AI;
using CombatSystem;

namespace TaskSystem {

	public class Attack : TaskBehaviour {

		NavMeshAgent navMeshAgent;
		AttackArguments attackArguments;

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

			// Start the routine to handle the attack logic.
			InvokeRepeating(nameof(Routine), 0f, creature.AttackSpeed);
		}

		private void Routine() {
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
			if (!creature.AbilityStateMachine.GlobalCooldown)
            {
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

			creature.AbilityStateMachine.TriggerBasicAttack();
		}

		/// <summary>
        /// Callback invoked by an animation event when the attack hits.
        /// </summary>
		private void OnHit()
        {
			float calculatedDamage = Mathf.Round(creature.Damage * UnityEngine.Random.Range(1 - Constant.DAMAGE_JITTER_FACTOR, 1 + Constant.DAMAGE_JITTER_FACTOR));

			attackArguments.Target.SufferDamage(creature.DamageCategoryType, calculatedDamage, creature);
        }

		private void LookAtTarget()
        {
			Vector3 dir = attackArguments.Target.transform.position - transform.position;
			Quaternion lookRot = Quaternion.LookRotation(dir);
			Quaternion finalRot = lookRot * Quaternion.Euler(0f, creature.ForwardOffset, 0f);
			LeanTween.rotate(gameObject, finalRot.eulerAngles, 0.2f).setEase(LeanTweenType.easeOutQuad);
        }

		public override void OnEnd() {
			CancelInvoke(nameof(Routine));
			CancelInvoke(nameof(MoveRoutine));

			_trigger?.Destroy();
			_trigger = null;
			
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

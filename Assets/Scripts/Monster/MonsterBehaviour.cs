using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using PointerSystem;
using UI;

namespace MonsterSystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class MonsterBehaviour : InteractableBehavior, IInteractable
    {
        private const float ACTION_JITTER = 0.25f;

        [SerializeField]
        public MonsterData MonsterData;

        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private DelayedAction _action;
        private Vector3 _spawnPosition;

        private void Start()
        {
            if (MonsterData == null)
            {
                Debug.LogError("MonsterData is not assigned on " + gameObject.name);
                return;
            }

            // Remember the spawn position
            _spawnPosition = transform.position;

            _animator = GetComponent<Animator>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = MonsterData.WalkingSpeed;
            _navMeshAgent.autoBraking = false;

            // Add some jitter to the time between actions
            TimerManager.Instance.Enqueue(BuildNewAction(MonsterData.TimeBetweenActions));
        }

        /// <summary>
		/// On Mouse Over, opens the right click menu.
		/// </summary>
		private void OnMouseOver() {
            if (Input.GetMouseButtonDown(1)) {
                OptionsHandler.Instance.OpenRightClickMenu(this, MonsterData, AttackCallback);
            }
        }

        /// <summary>
        /// Callback to create the Attack task if the user has selected the 'Attack' option
        /// </summary>
        private void AttackCallback() {
            /*
			InteractArguments interactArguments = new InteractArguments(transform.position, this);
			if (Squad.FirstSelected(out Actor actor)) {
				actor.TaskScheduler.CreateTask<Interact>(interactArguments);
			}
            */
		}

        private void CheckState()
        {
            if (!this)
            {
                return;
            }

            // Calculate the next state
            MoveState();

            // Re-enqueue the action
            TimerManager.Instance.Enqueue(BuildNewAction(MonsterData.TimeBetweenActions));
        }

        private void MoveState()
        {
            if (_navMeshAgent.isPathStale)
            {
                OnEnd();
                return;
            }

            // Calculate a random point to move to within a certain radius
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * MonsterData.WanderingRadius;

            // Offset it from the spawn position
            randomDirection += _spawnPosition;

            // Find a valid position on the NavMesh close to the random direction
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, MonsterData.WanderingRadius, -1);

            // Set the destination to the random point
            _navMeshAgent.destination = navHit.position;
            _navMeshAgent.isStopped = false;
            _animator.SetBool("Move", true);

            // Create a Target at destination:
            CreateTargetAtDestination();
        }

        private void OnEnd()
        {
            if (_navMeshAgent.isActiveAndEnabled)
            {   
                _animator.SetBool("Move", false);
                _navMeshAgent.isStopped = true;
            }
        }

        private DelayedAction BuildNewAction(float seconds)
        {
            float time = seconds + UnityEngine.Random.Range(-seconds * ACTION_JITTER, seconds * ACTION_JITTER);

            return new DelayedAction(CheckState, time);
        }

        private void CreateTargetAtDestination()
        {
            GameObject targetObject = new GameObject("MonsterTarget - " + gameObject.name);
            MonsterTarget monsterTarget = targetObject.AddComponent<MonsterTarget>();
            SphereCollider sphereCollider = targetObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 1f;

            monsterTarget.SubscribeToTriggerEvents(GetComponent<Collider>(), OnEnd);
            targetObject.transform.position = _navMeshAgent.destination;
        }

        #region Mouse Event
        protected override PointerMode GetPointerMode() {
            return PointerMode.ATTACK;
        }
		#endregion

        #region IInteractable Implementation
        protected override OutlineType GetOutlineType() {
            return OutlineType.ENEMY;
        }

        // Override the GetDefaultAction from IInteractable
        public string GetDefaultAction()
        {
            return "Attack";
        }

        public void OnInteractEnter(Actor actor)
        {
            Debug.Log("Interacted with monster: " + gameObject.name);
        }

        public void OnInteractExit(Actor actor)
        {
            Debug.Log("Stopped interacting with monster: " + gameObject.name);
        }

        public float GetInteractionRadius()
        {
            // Depends on what the monster is
            if (Squad.FirstSelected(out Actor actor))
            {
                return actor.Attributes.AttackRange;
            } else
            {
                return 1f;
            }
        }
        #endregion
    }
}
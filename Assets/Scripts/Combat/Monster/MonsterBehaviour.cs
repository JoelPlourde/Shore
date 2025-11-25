using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using PointerSystem;
using TaskSystem;
using UI;
using DropSystem;
using ItemSystem;
using SkillSystem;

namespace CombatSystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Creature))]
    public class MonsterBehaviour : InteractableBehavior, IInteractable
    {
        private const float ACTION_JITTER = 0.25f;

        [SerializeField]
        public MonsterData MonsterData;

        private Animator _animator;
        private NavMeshAgent _navMeshAgent;
        private Creature _creature;
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
            _creature = GetComponent<Creature>();  
            _navMeshAgent.speed = MonsterData.WalkingSpeed;
            _navMeshAgent.autoBraking = false;

            // Initialize the creature with the MonsterData
            _creature.Initialize(MonsterData, OnDeathCallback);

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
			AttackArguments attackArguments = new AttackArguments(_creature);
			if (Squad.FirstSelected(out Actor actor)) {
				actor.TaskScheduler.CreateTask<Attack>(attackArguments);
			}
		}

        /// <summary>
        /// Callback invoked when the monster dies.
        /// </summary>
        private void OnDeathCallback()
        {
            // Remove the Collider
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            if (MonsterData.Lootable && ReferenceEquals(MonsterData.DropTable, null) == false)
            {
                List<Drop> drops = new List<Drop>();
                MonsterData.DropTable.GetRandomDrops(ref drops);

                foreach (Drop drop in drops)
                {
                    Item item = drop.ToItem();
                    ItemManager.Instance.PlaceItemInWorld(item, transform.position, transform.rotation, false);
                }

                // TODO POOF effect

                // Make the monster disappear after a delay

                LeanTween.delayedCall(2f, () =>
                {
                    Destroy(gameObject);
                });
            }

            if (Squad.FirstSelected(out Actor actor))
            {
                // Calculate the total damage dealt to the monster across all damage categories
                float totalDamage = 0f;
                foreach (var totalEntry in _creature.AccumulatedDamage)
                {
                    totalDamage += totalEntry.Value;
                }

                // Iterate over the Accumulated Damage and award experience based on damage dealt as a proportion of total damage
                foreach (var damageEntry in _creature.AccumulatedDamage)
                {
                    DamageCategoryType damageCategory = damageEntry.Key;
                    float damageDealt = damageEntry.Value;

                    if (damageDealt > 0f && totalDamage > 0f)
                    {
                        float experienceGained = Mathf.Round((damageDealt / totalDamage) * MonsterData.Experience);
                        actor.Skills.GainExperience(damageCategory, experienceGained);
                    }
                }
            }
        }

        private void CheckState()
        {
            if (!this) {
                return;
            }

            if (_creature.InCombat) {
                if (_creature.Dead) {
                    return;
                }

                // Re-enqueue the action
                TimerManager.Instance.Enqueue(BuildNewAction(MonsterData.TimeBetweenActions));
                return;
            }

            // Calculate the next state
            Wander();

            // Re-enqueue the action
            TimerManager.Instance.Enqueue(BuildNewAction(MonsterData.TimeBetweenActions));
        }

        private void Wander()
        {
            // Calculate a random point to move to within a certain radius
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * MonsterData.WanderingRadius;

            // Offset it from the spawn position
            randomDirection += _spawnPosition;

            // Find a valid position on the NavMesh close to the random direction
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, MonsterData.WanderingRadius, -1);

            // Set the destination to the random point
            _navMeshAgent.destination = navHit.position;
            MoveArguments moveArguments = new MoveArguments(navHit.position);
            _creature.TaskScheduler.CreateTask<Move>(moveArguments);
        }

        private DelayedAction BuildNewAction(float seconds)
        {
            float time = seconds + UnityEngine.Random.Range(-seconds * ACTION_JITTER, seconds * ACTION_JITTER);

            return new DelayedAction(CheckState, time);
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

        protected override string GetActionLabel()
        {
            return I18N.GetValue("attack");
        }

        protected override string GetEntityLabel()
        {
            return I18N.GetValue("monsters." + MonsterData.ID + ".name");
        }

        // Override the GetDefaultAction from IInteractable
        public string GetDefaultAction()
        {
            return "Attack";
        }

        public void OnInteractEnter(Actor actor)
        {
            AttackCallback();
        }

        public void OnInteractExit(Actor actor)
        {
        }

        public float GetInteractionRadius()
        {
            // Depends on what the monster is
            if (Squad.FirstSelected(out Actor actor))
            {
                return _creature.AttackRange;
            } else
            {
                return 1f;
            }
        }
        #endregion
    }
}
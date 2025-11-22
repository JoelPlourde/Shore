using UnityEngine;
using System.Collections.Generic;
using System;
using SaveSystem;
using MonsterSystem;
using UnityEngine.AI;
using TaskSystem;

[RequireComponent(typeof(TaskScheduler))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Creature : MonoBehaviour
{
    private static readonly float OUT_OF_COMBAT_THRESHOLD = 15f; // seconds

    private static readonly float COMBAT_CHECK_INTERVAL = 1f; // seconds

    [HideInInspector]
    public float MaxHealth = 100f;

    [HideInInspector]
    public float Health = 100f;

    [HideInInspector]
    public float Damage = 1f;

    [HideInInspector]
    public float AttackRange = 1.0f;

    [HideInInspector]
    public float AttackSpeed = 2.0f;

    [HideInInspector]
    [Tooltip("The forward direction of the creature.")]
    public int ForwardOffset = 0;

    // A flag indicating whether the creature is currently in combat
    private bool _inCombat = false;

    // Duration the creature has been inactive in combat
    private float _timeoutDuration = 0f;

    // A flag indicating that combat-related activities are ongoing
    private bool _combatActivity = false;

    // List of current combat targets
    private List<Creature> _combatTargets = new List<Creature>();

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private TaskScheduler _taskScheduler;

    // Event triggered when health is updated
    public event Action<float> OnUpdateHealthEvent;
    public event Action OnDeathEvent;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _taskScheduler = GetComponent<TaskScheduler>();
    }

    #region Initialization
    public void Initialize(CreatureDto creatureDto)
    {
        MaxHealth = creatureDto.MaxHealth;
        Health = creatureDto.Health;
        Damage = creatureDto.Damage;
    }

    /// <summary>
    /// Initializes the creature with data from MonsterData.
    /// </summary>
    /// <param name="monsterData"></param>
    public void Initialize(MonsterData monsterData, Action OnDeathCallback = null)
    {
        MaxHealth = monsterData.Health;
        Health = monsterData.Health;
        Damage = monsterData.Damage;
        AttackRange = monsterData.AttackRange;
        AttackSpeed = monsterData.AttackSpeed;
        Size = monsterData.Size;
        ForwardOffset = monsterData.ForwardOffset;
        if (OnDeathCallback != null)
        {
            OnDeathEvent += OnDeathCallback;
        }
    }
    #endregion

    #region Combat Management
    /// <summary>
    /// Enters combat mode with a specified target.
    /// </summary>
    /// <param name="target"></param>
    public void EnterCombat(Creature target)
    {
        if (!_inCombat)
        {
            // Set the in-combat flag
            _inCombat = true;
            target.EnterCombat(this); // Ensure mutual combat engagement

            target.TaskScheduler.CancelTask();
            
            // Create an Attack task if not already attacking
		    target.TaskScheduler.CreateTask<Attack>(new AttackArguments(this));
        }
        _animator.SetBool("Combat", true);

        // Register the target if not already present
        if (!_combatTargets.Contains(target) && target != this)
        {
            _combatTargets.Add(target);
        }

        // Mark that there is combat activity
        _combatActivity = true;

        // Start a Coroutine every 5 seconds
        // Check if the Coroutine is already running
        if (IsInvoking(nameof(CombatMonitor)))
        {
            return; // Already running
        }

        InvokeRepeating(nameof(CombatMonitor), 0f, COMBAT_CHECK_INTERVAL);
    }

    /// <summary>
    /// Exits combat mode, clearing targets and resetting flags.
    /// </summary>
    public void ExitCombat(Creature target = null)
    {
        if (target != null)
        {
            _combatTargets.Remove(target);
            if (_combatTargets.Count > 0)
            {
                // Still have other targets, do not exit combat
                return;
            }
        }

        Debug.Log(gameObject.name + " is exiting combat.");

        // Stop the combat monitoring Coroutine
        if (IsInvoking(nameof(CombatMonitor)))
        {
            CancelInvoke(nameof(CombatMonitor));
        }

        // Reset animator and flags
        _animator.SetBool("Combat", false);

        _inCombat = false;
        _combatActivity = false;
        _timeoutDuration = 0f;

        _combatTargets.Clear();
    }

    /// <summary>
    /// Monitors combat activity and exits combat if inactive for a threshold duration.
    /// </summary>
    private void CombatMonitor()
    {
        if (!_combatActivity)
        {
            _timeoutDuration += COMBAT_CHECK_INTERVAL;
            if (_timeoutDuration >= OUT_OF_COMBAT_THRESHOLD)
            {
                ExitCombat();
                CancelInvoke(nameof(CombatMonitor));
            }
        }
        else
        {
            // Reset the timeout duration and combat activity flag
            _timeoutDuration = 0f;
            _combatActivity = false;
        }
    }
    #endregion

    #region Health
    /// <summary>
    /// Suffer X amount of damage. If health is empty, call the virtual OnDeath event.
    /// </summary>
    /// <param name="damage">Amount of damage to suffer.</param>
    public void SufferDamage(float damage)
    {
        Health -= damage;

        OnUpdateHealthEvent?.Invoke(Health / MaxHealth);

        if (Dead)
        {
            OnDeath();
        }
    }

    /// <summary>
    /// Suffer X amount of damage and enter combat with the attacker.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attacker"></param>
    public void SufferDamage(float damage, Creature attacker)
    {
        SufferDamage(damage);

        EnterCombat(attacker);
    }

    /// <summary>
    /// Increase the health value by X. The health value is capped at maximum health of the actor.
    /// </summary>
    /// <param name="value">Amount of health to increase.</param>
    public void IncreaseHealth(float value)
    {
        if (!Dead)
        {
            Health += value;

            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }

            OnUpdateHealthEvent?.Invoke(Health / MaxHealth);
        }
    }
    #endregion

    #region Death
    private void OnDeath()
    {
        Animator.SetBool("Death", true);
        NavMeshAgent.isStopped = true;

        OnDeathEvent?.Invoke();
    }
    #endregion

    public bool Dead { get => Health <= 0; }
    public float Size { get; private set; } = 1.0f;
    public bool InCombat { get => _inCombat; }
    public float TimeoutDuration { get => _timeoutDuration; }
    public List<Creature> CombatTargets { get => _combatTargets; }

    public TaskScheduler TaskScheduler { get => _taskScheduler; }
    public Animator Animator { get => _animator; }
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; }
}

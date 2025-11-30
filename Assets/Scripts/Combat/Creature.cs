using UnityEngine;
using System.Collections.Generic;
using System;
using SaveSystem;
using CombatSystem;
using UnityEngine.AI;
using TaskSystem;
using UI;
using ItemSystem.EquipmentSystem;
using StatusEffectSystem;
using System.Linq;

[RequireComponent(typeof(StatusEffectScheduler))]
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

    [HideInInspector]
    [Tooltip("The damage category type of the creature.")]
    public DamageCategoryType DamageCategoryType = DamageCategoryType.TYPELESS;

    // A flag indicating whether the creature is currently in combat
    private bool _inCombat = false;

    // Duration the creature has been inactive in combat
    private float _timeoutDuration = 0f;

    // A flag indicating that combat-related activities are ongoing
    private bool _combatActivity = false;

    // List of current combat targets
    private List<Creature> _combatTargets = new List<Creature>();

    private Actor _actor;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private TaskScheduler _taskScheduler;
    private StatusEffectScheduler _statusEffectScheduler;

    // Event triggered when health is updated
    public event Action<float> OnUpdateHealthEvent;
    public event Action OnDeathEvent;

    private FloatingHealthBar _floatingHealthBar;

    private AbilityStateMachine _abilityStateMachine = new AbilityStateMachine();

    // Accumulated damage by category
    private Dictionary<DamageCategoryType, float> _accumulatedDamage = new Dictionary<DamageCategoryType, float>()
    {
        { DamageCategoryType.MELEE, 0f },
        { DamageCategoryType.RANGED, 0f },
        { DamageCategoryType.MAGIC, 0f }
    };

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _taskScheduler = GetComponent<TaskScheduler>();
        _statusEffectScheduler = GetComponent<StatusEffectScheduler>();
    }

    #region Initialization
    public void Initialize(Actor actor, CreatureDto creatureDto)
    {
        MaxHealth = creatureDto.MaxHealth;
        Health = creatureDto.Health;
        _actor = actor;

        // TODO, pass the list of Abilities from the Ability Bar
        _abilityStateMachine.Initialize(this, creatureDto);

        actor.Statistics.OnUpdateStatisticEvent += (statisticType, value) =>
        {
            if (statisticType == StatisticType.STRENGTH)
            {
                Damage += value;
            }
        };
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
        DamageCategoryType = monsterData.DamageCategoryType;
        AttackRange = monsterData.AttackRange;
        AttackSpeed = monsterData.AttackSpeed;
        Size = monsterData.Size;
        Height = monsterData.Height;
        ForwardOffset = monsterData.ForwardOffset;
        _abilityStateMachine.Initialize(this);
        if (OnDeathCallback != null)
        {
            OnDeathEvent += OnDeathCallback;
        }
    }
    #endregion

    #region Animation Callbacks
    /// <summary>
    /// Called by the Animator to set up IK (inverse kinematics) each frame.
    /// </summary>
    /// <param name="layerIndex"></param>
    void OnAnimatorIK(int layerIndex)
    {
        if (!_animator) return;

        if (_combatTargets.Count == 0) return;

        Creature target = _combatTargets.First();
        if (ReferenceEquals(target, null)) return;
        if (target == null) return;

        _animator.SetLookAtWeight(0.5f);
        _animator.SetLookAtPosition(target.transform.position);
    }

    /// <summary>
    /// Callback invoked by an animation event when the hit animation is completed
    /// </summary>
    private void OnAnimationHit(string eventData)
    {
        Creature target = _combatTargets.FirstOrDefault();
        if (ReferenceEquals(target, null)) return;

        if (!string.IsNullOrEmpty(eventData))
        {
            _abilityStateMachine.ExecutePendingAbility(eventData);
            return;
        }

        float calculatedDamage = Mathf.Round(Damage * UnityEngine.Random.Range(1 - Constant.DAMAGE_JITTER_FACTOR, 1 + Constant.DAMAGE_JITTER_FACTOR));

        target.SufferDamage(DamageCategoryType, calculatedDamage, this);
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

        // Reset the Accumulated Damage
        _accumulatedDamage[DamageCategoryType.MELEE] = 0f;
        _accumulatedDamage[DamageCategoryType.RANGED] = 0f;
        _accumulatedDamage[DamageCategoryType.MAGIC] = 0f;

        // Show floating health bar
        if (ReferenceEquals(_floatingHealthBar, null))
        {
            // Initialize floating health bar
            _floatingHealthBar = FloatingHealthBarHandler.Instance.InitializeHealthBar(transform, Health / MaxHealth, Height);

            // Subscribe to health update events to refresh the health bar
            OnUpdateHealthEvent += _floatingHealthBar.Refresh;
        }

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

        if (!ReferenceEquals(_floatingHealthBar, null))
        {
            FloatingHealthBarHandler.Instance.HideHealthBar(_floatingHealthBar);
            OnUpdateHealthEvent -= _floatingHealthBar.Refresh;
            _floatingHealthBar = null;
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
    public void SufferDamage(DamageCategoryType damageCategory, float damage)
    {
        if (!ReferenceEquals(_actor, null))
        {
            if (Reflect(damageCategory, damage))
            {
                // Damage was reflected, exit early
                return;
            }

            int armor = _actor.Statistics.GetStatistic(StatisticType.ARMOR);

            // Rounded to the nearest whole number
            float damageReduction = Mathf.Round(armor * Constant.ARMOR_DAMAGE_REDUCTION_FACTOR);

            damage -= damageReduction;

            if (damage < 0)
            {
                damage = 0;
            }
        } else
        {
            if (damageCategory == DamageCategoryType.TYPELESS)
            {
                damageCategory = DamageCategoryType.MELEE;
            }
            _accumulatedDamage[damageCategory] += damage;
        }

        // Compare the damage to current health, only deduct up to current health
        if (damage > Health)
        {
            damage = Health;
        }

        Health -= damage;

        OnUpdateHealthEvent?.Invoke(Health / MaxHealth);

        HitsplatType hitsplatType = (damage > 0) ? HitsplatType.DAMAGE : HitsplatType.BLOCK;

        HitsplatHandler.Instance.ShowHitsplat(transform, hitsplatType, (int)damage, Height / 2f);

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
    public void SufferDamage(DamageCategoryType damageCategory, float damage, Creature attacker)
    {
        SufferDamage(damageCategory, damage);

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

        if (!ReferenceEquals(_floatingHealthBar, null))
        {
            FloatingHealthBarHandler.Instance.HideHealthBar(_floatingHealthBar);
            _floatingHealthBar = null;
        }

        // Find the Rigidbody component and disable it
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (!ReferenceEquals(rigidbody, null))
        {
            rigidbody.isKinematic = true;
        }

        // Disable the NavMeshAgent
        NavMeshAgent.enabled = false;

        OnDeathEvent?.Invoke();
    }
    #endregion

    #region Abilities
    public bool Reflect(DamageCategoryType damageCategory, float damage)
    {
        // Check if you have the "Reflect" status effect
        if (_actor.Creature.StatusEffectScheduler.CheckIfHasStatusEffect(Constant.REFLECT))
        {
            // Find the attacker from combat targets
            Creature attacker = _combatTargets.First();
            if (!ReferenceEquals(attacker, null))
            {
                Animator.SetTrigger("Block");

                attacker.SufferDamage(damageCategory, damage);

                // Remove the Reflect status effect after reflecting
                _actor.Creature.StatusEffectScheduler.RemoveStatusEffect(Constant.REFLECT);
                return true;
            }
        }
        return false;
    }
    

    #endregion

    public bool Stunned { get; set; } = false;
    public bool Dead { get => Health <= 0; }
    public float Size { get; private set; } = 1.0f;
    public float Height { get; private set; } = 2.0f;
    public bool InCombat { get => _inCombat; }
    public float TimeoutDuration { get => _timeoutDuration; }
    public List<Creature> CombatTargets { get => _combatTargets; }
    public Dictionary<DamageCategoryType, float> AccumulatedDamage { get => _accumulatedDamage; }

    public TaskScheduler TaskScheduler { get => _taskScheduler; }
    public Animator Animator { get => _animator; }
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; }
    public StatusEffectScheduler StatusEffectScheduler { get => _statusEffectScheduler; }

    public AbilityStateMachine AbilityStateMachine { get => _abilityStateMachine; }
}

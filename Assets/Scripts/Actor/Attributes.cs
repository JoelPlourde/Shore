using SaveSystem;
using StatusEffectSystem;
using System;
using ItemSystem.EquipmentSystem;
using UnityEngine;

[Serializable]
public class Attributes {
	public float MaxHealth = 100f;
	public float Health = 100f;
	public float HealthRegeneration = 1f;		// Value per 5 seconds
	public float Speed = 1f;
	public float Damage = 2f;
	public float HungerRate = 0.05f;            // Value per 5 seconds
	public float Food = 20f;
	public float Temperature = 20f;
	public float AttackRange = 1f;

	public event Action<float> OnUpdateHealthEvent;
	public event Action<float> OnUpdateFoodEvent;
	public event Action<float> OnUpdateTemperatureEvent;

	private Actor _actor;

	private LTDescr _temperatureLerp;

	public void Initialize(Actor actor, AttributesDto attributesDto) {
		MaxHealth = attributesDto.MaxHealth;
		Health = attributesDto.Health;
		HealthRegeneration = attributesDto.HealthRegeneration;
		Speed = attributesDto.Speed;
		Damage = attributesDto.Damage;
		HungerRate = attributesDto.HungerRate;
		Food = attributesDto.Food;
		Temperature = attributesDto.Temperature;

		_actor = actor;
		StatusEffectScheduler.Instance(_actor.Guid).AddStatusEffect(new StatusEffectSystem.Status(_actor, 1f, StatusEffectManager.GetStatusEffectData(Constant.HUNGRY)));
	}

	#region Health
	/// <summary>
	/// Suffer X amount of damage. If health is empty, call the virtual OnDeath event.
	/// </summary>
	/// <param name="damage">Amount of damage to suffer.</param>
	public void SufferDamage(float damage) {
		Health -= damage;

		OnUpdateHealthEvent?.Invoke(Health / MaxHealth);

		if (Health <= 0) {
			StatusEffectScheduler.Instance(_actor.Guid).AddStatusEffect(new StatusEffectSystem.Status(_actor, 1f, StatusEffectManager.GetStatusEffectData(Constant.DEATH)));
		}
	}

	/// <summary>
	/// Increase the health value by X. The health value is capped at maximum health of the actor.
	/// </summary>
	/// <param name="value">Amount of health to increase.</param>
	public void IncreaseHealth(float value) {
		if (!_actor.Dead) {
			Health += value;

			if (Health > MaxHealth) {
				Health = MaxHealth;
			}

			OnUpdateHealthEvent?.Invoke(Health / MaxHealth);
		}
	}
	#endregion

	#region Speed
	/// <summary>
	/// Reduce speed by a percentage of the base speed.
	/// </summary>
	/// <param name="value">A value in %</param>
	public void ReduceSpeed(float value) {
		Speed = Constant.ACTOR_BASE_SPEED - (Constant.ACTOR_BASE_SPEED * value);
		_actor.NavMeshAgent.speed = Speed;
		_actor.Animator.SetFloat("Speed", Speed);
	}

	/// <summary>
	/// Reset the speed to its original speed before the speed bonus applied.
	/// </summary>
	/// <param name="value">A value in % of the speed bonus applied before.</param>
	public void ResetSpeed(float value) {
		Speed = Speed + (Constant.ACTOR_BASE_SPEED * value);
		_actor.NavMeshAgent.speed = Speed;
		_actor.Animator.SetFloat("Speed", Speed);
	}

	/// <summary>
	/// Increase speed by a percentage of the base speed.
	/// </summary>
	/// <param name="value">A value in %</param>
	public void IncreaseSpeed(float value) {
		Speed = Constant.ACTOR_BASE_SPEED + (Constant.ACTOR_BASE_SPEED * value);
		_actor.NavMeshAgent.speed = Speed;
		_actor.Animator.SetFloat("Speed", Speed);
	}
	#endregion

	#region Hunger
	/// <summary>
	/// Increase the hunger rate by X.
	/// </summary>
	/// <param name="value">Amount of hunger rate to increase</param>
	public void IncreaseHungerRate(float value) {
		HungerRate += value;
	}

	/// <summary>
	/// Reduce the hunger rate by X.
	/// </summary>
	/// <param name="value">Amount of hunger to deduct.</param>
	public void ReduceHungerRate(float value) {
		HungerRate -= value;
	}
	#endregion

	#region Food
	/// <summary>
	/// Increase the food bar by X. Remove any Hungry status effects.
	/// </summary>
	/// <param name="value">Amount of food to restore.</param>
	public void IncreaseFood(float value) {
		Food += value;

		StatusEffectScheduler.Instance(_actor.Guid).RemoveStatusEffect(Constant.STARVING);

		if (Food > Constant.ACTOR_BASE_FOOD) {
			Food = Constant.ACTOR_BASE_FOOD;
		}

		OnUpdateFoodEvent?.Invoke(Food);
	}

	/// <summary>
	/// Reduce the food by X. Apply a Hungry status effect if the food equals to 0.
	/// </summary>
	/// <param name="value">Amount of food to deduct.</param>
	public void ReduceFood(float value) {
		Food -= value;

		if (Food <= 0) {
			Food = 0;
			StatusEffectScheduler.Instance(_actor.Guid).AddStatusEffect(new StatusEffectSystem.Status(_actor, 0.5f, StatusEffectManager.GetStatusEffectData(Constant.STARVING)));
		}

		OnUpdateFoodEvent?.Invoke(Food);
	}
	#endregion

	#region Temperature
	public void AdjustTemperature(float targetTemperature, float duration) {
		if (!ReferenceEquals(_temperatureLerp, null)) {
			LeanTween.cancel(_temperatureLerp.id);
			_temperatureLerp = null;
		}

		_temperatureLerp = LeanTween.value(_actor.gameObject, Temperature, targetTemperature, duration).setOnUpdate((float value) => {
			Temperature = value;
			ApplyTemperatureEffect();
		});
	}

	// TODO: Add cold/hot animation, Add feedback: Cold breath, sweat.
	private void ApplyTemperatureEffect() {
		float adjustedTemperature = (Temperature > 0) ? Temperature - _actor.Statistics.GetStatistic(StatisticType.HEAT_RESISTANCE) : Temperature - _actor.Statistics.GetStatistic(StatisticType.COLD_RESISTANCE);

		// TODO replace this by CONSTANT
		if (adjustedTemperature > 25) {
			StatusEffectScheduler.Instance(_actor.Guid).AddStatusEffect(new StatusEffectSystem.Status(_actor, 0.5f, StatusEffectManager.GetStatusEffectData("Hot")));
		} else {
			StatusEffectScheduler.Instance(_actor.Guid).RemoveStatusEffect("Hot");
		}

		if (adjustedTemperature < 0) {
			StatusEffectScheduler.Instance(_actor.Guid).AddStatusEffect(new StatusEffectSystem.Status(_actor, 0.5f, StatusEffectManager.GetStatusEffectData("Cold")));
		} else {
			StatusEffectScheduler.Instance(_actor.Guid).RemoveStatusEffect("Cold");
		}
	}
	#endregion
}

using System;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem.EquipmentSystem;

public class Statistics {

	// Event triggered whenever a statistic is updated.
	public event Action<StatisticType, int> OnUpdateStatisticEvent;

	private Dictionary<StatisticType, int> _currentStatistics = new Dictionary<StatisticType, int>();

	/// <summary>
	/// Initially the statistics are all at zero. On load game, the equiment, status, etc. will alter the statistics.
	/// </summary>
	public Statistics() {
		foreach (StatisticType statisticType in Enum.GetValues(typeof(StatisticType))) {
			_currentStatistics.Add(statisticType, 0);
		}
	}

	/// <summary>
	/// Get Statistic by StatisticType
	/// </summary>
	/// <param name="statisticType">StatisticType</param>
	/// <returns>The current value.</returns>
	public int GetStatistic(StatisticType statisticType) {
		if (_currentStatistics.TryGetValue(statisticType, out int value)) {
			return value;
		} else {
			return 0;
		}
	}

	/// <summary>
	/// Adjust multiple statistics depending whether or not its positive or negative.
	/// </summary>
	/// <param name="statistics">The type of statistic to be adjusted.</param>
	/// <param name="increment">If true, increase the value. Else decrease.</param>
	public void UpdateStatistics(List<Statistic> statistics, bool increment) {
		foreach (Statistic statistic in statistics) {
			UpdateStatistic(statistic.StatisticType, statistic.Value, increment);
		}
	}

	/// <summary>
	/// Adjust a statistic by a value either positive or negative based on the increment parameter.
	/// </summary>
	/// <param name="statisticType">The type of statistic to be adjusted.</param>
	/// <param name="value">The value to update the value by.</param>
	/// <param name="increment">If true, increase the value. Else decrease.</param>
	public void UpdateStatistic(StatisticType statisticType, int value, bool increment) {
		if (_currentStatistics.ContainsKey(statisticType)) {
			int offsetValue = increment ? value : -value;
			_currentStatistics[statisticType] += offsetValue;
			OnUpdateStatisticEvent?.Invoke(statisticType, offsetValue);
		} else {
			throw new UnityException("The StatisticType key does not exist, please define it in the Statistics class dictionary.");
		}
	}
}

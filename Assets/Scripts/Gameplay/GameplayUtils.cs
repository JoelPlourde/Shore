using System;
using UnityEngine;

public static class GameplayUtils {

	/// <summary>
	/// Calculate the repeat rate (time between strikes) based on the value of speed.
	/// </summary>
	/// <param name="speed">The value of Speed.</param>
	/// <returns>The repeat rate in seconds.</returns>
	public static float CalculateRepeatRateBasedOnSpeed(float speed) {
		return (0.01f * speed) + 1;
	}

	/// <summary>
	/// Calculate if its a critical strike based on the Luck statistic.
	/// </summary>
	/// <param name="luck">The value of the luck.</param>
	/// <returns>Whether or not it was a critical strike.</returns>
	public static bool CalculateIfCriticalStrikeBasedOnLuck(int luck) {
		return UnityEngine.Random.Range(0, Constant.MAXIMUM_LUCK) <= luck;
	}

	/// <summary>
	/// Calculate the damage based on the ability of the character to harvest.
	/// </summary>
	/// <param name="durability">The durability of the node.</param>
	/// <param name="damage">The damage inflicted to the node.</param>
	/// <param name="luck">The value of the luck of the character, 0, if luck is not a factor.</param>
	/// <returns>Return the calculated damage based on the input parameters.</returns>
	public static int CalculateDamageBasedOnStrength(int durability, int damage, int luck) {
		int result = Mathf.RoundToInt(Mathf.Clamp(damage - durability, 0, int.MaxValue) * ((CalculateIfCriticalStrikeBasedOnLuck(luck)) ? 1.5f : 1f));
		return Mathf.Max(result, 1);
	}
}

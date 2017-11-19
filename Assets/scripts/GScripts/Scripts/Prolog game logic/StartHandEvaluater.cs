using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartHandEvaluater
{
	/// <summary>
	/// Evaluates the start hand. range[0,1]
	/// exampl: pair of aces will be evaluated to 1
	/// </summary>
	/// <returns>The start hand.</returns>
	/// <param name="cards">Cards.</param>
	public static float evaluateStartHand (List<Card> cards)
	{
		float strength = strengthCard (cards [0].CardValue) + strengthCard (cards [1].CardValue);
		if (cards [0].CardValue == cards [1].CardValue)
			strength *= 2;
		if (cards [0].suitValue.Equals (cards [0].suitValue))
			strength += 2;

		int value1 = cards [0].CardValue;
		int value2 = cards [1].CardValue;

		if (value1 == 1)
			value1 = 14;
		if (value2 == 1)
			value2 = 14;

		int gap = Mathf.Abs (value1 - value2);

		strength -= gapEvaluation (gap);

		return strength / 40.0f;
	}

	private static int gapEvaluation (int gap)
	{
		switch (gap) {
		case 1:
			return 1;
		case 2:
			return 2;
		case 3:
			return 4;
		default :
			return 5;
		}
	}

	public static float strengthCard (int value)
	{
		switch (value) {
		case 1:
			return 10;
		case 13:
			return 8;
		case 12:
			return 7;
		case 11:
			return 6;
		case 10:
			return 5;
		case 9:
			return 4.5f;
		case 8:
			return 4;
		case 7:
			return 3.5f;
		case 6:
			return 3;
		case 5:
			return 2.5f;
		case 4:
			return 2;
		case 3:
			return 1.5f;
		default:
			return 1;
		}
	}
}

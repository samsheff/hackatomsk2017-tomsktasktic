using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandStrengthEvaluater
{
	public static float Evaluate (string handType, List<Card>  PlayerCards, List<Card> Cards, GameManager.GameStatus gameStatus)
	{

		float value1 = StartHandEvaluater.strengthCard (PlayerCards [0].CardValue);
		float value2 = StartHandEvaluater.strengthCard (PlayerCards [1].CardValue);
	
		float c1 = StartHandEvaluater.strengthCard (Cards [0].CardValue);
		//float c2 = StartHandEvaluater.strengthCard (Cards [1].CardValue);
		float c3 = StartHandEvaluater.strengthCard (Cards [2].CardValue);
		float c4 = StartHandEvaluater.strengthCard (Cards [3].CardValue);
		float c5 = StartHandEvaluater.strengthCard (Cards [4].CardValue);

		float min = Mathf.Min (value1, value2);
		float max = Mathf.Max (value1, value2);

		int handRank = rank (handType);
		if (handRank >= 9)
			return 100;

		if (handRank == 1)
			return 0;

		//one Pair
		if (handRank == 2) {

			if (value2 == value1)//pair in his pocket
				return StartHandEvaluater.evaluateStartHand (PlayerCards) * 20;

			if (value1 == c1)
				return (value1 * 1.6f + value2 * 0.4f);
			if (value2 == c1)
				return (value2 * 1.6f + value1 * 0.4f);
			else //pair on the ground
				return StartHandEvaluater.evaluateStartHand (PlayerCards) * 15
				* gameStateEffectForOnePairTworPairThreeOfKind (gameStatus);

		}

		// two pair
		if (handRank == 3) {
			// two pair with his hand
			if (((value1 == c1) && (value2 == c3)) || ((value1 == c3) && (value2 == c1))) {
				return 20 + max * 1.6f + min * 0.4f;
			}
			// got one pair and the other is on the ground
			if ((value1 == c1) || (value1 == c3))
				return 10 + (value1 * 1.7f + value2 * 0.3f);
			if ((value2 == c1) || (value2 == c3))
				return 10 + (value2 * 1.7f + value1 * 0.3f);
	
			// two pair on the ground
			return (20 + max) * gameStateEffectForOnePairTworPairThreeOfKind (gameStatus);
		}

		//three of kind
		if (handRank == 4) {
			if (value1 == value2)
				return 40 + 2 * value1;
			if (value1 == c1)
				return 40 + value1 + value2 * 0.5f;
			if (value2 == c1)
				return 40 + value2 + value1 * 0.5f;
			return (40 + max) * gameStateEffectForOnePairTworPairThreeOfKind (gameStatus);
		}

		if ((handRank == 5) || (handRank == 6))
			return ((65 + (c5 * 0.5f)) * gameStateEffectForStraightAndFlush (gameStatus));
		
		// flush out there!
		if (handRank == 7) {
			if (Cards.Contains (PlayerCards [0]) && Cards.Contains (PlayerCards [1]))
				return (75 + (max * 0.5f)) * gameStateEffectForStraightAndFlush (gameStatus);
			else if (Cards.Contains (PlayerCards [0]))
				return (70 + value1) * gameStateEffectForStraightAndFlush (gameStatus);
			else if (Cards.Contains (PlayerCards [1]))
				return (70 + value2) * gameStateEffectForStraightAndFlush (gameStatus);
			else
				return 0;// boom ya :)
		}

		if (handRank == 8) {
			// full house
			if ((value1 == c1) && (value2 == c4)) {
				return 80 + value1 + (value2 * 0.5f);
			}
			if ((value2 == c1) && (value1 == c4)) {
				return 80 + value2 + (value1 * 0.5f);
			}
			if (value1 == c1) {
				return (70 + (2 * value1));
			}
			if (value2 == c1) {
				return (70 + (2 * value2));
			}
			if (value1 == c4) {
				return (70 + value1);
			}
			if (value2 == c4) {
				return (70 + value2);
			}
			return 70;
		}
		return 0;
	}

	private static int adjustValue (int value)
	{
		if (value == 1)
			return 13;
		else
			return value - 1;
	}

	public static int rank (string handType)
	{
		switch (handType) {
		case "royalStraightFlush":
			return 11;
		case "straightFlush":
			return 10;
		case "fourOfKind":
			return 9;
		case "fullHouse":
			return 8;
		case "flush":
			return 7;
		case "royalStraight":
			return 6;
		case "straight":
			return 5;
		case "threeOfKind":
			return 4;
		case "twoPair":
			return 3;
		case "onePair":
			return 2;
		default :
			return 1;
		}
	}

	private static float gameStateEffectForOnePairTworPairThreeOfKind (GameManager.GameStatus gameStatus)
	{
		if (gameStatus.Equals (GameManager.GameStatus.preflop))
			return 1;
		else if (gameStatus.Equals (GameManager.GameStatus.flop))
			return 0.8f;
		else if (gameStatus.Equals (GameManager.GameStatus.turn))
			return 0.5f;
		else if (gameStatus.Equals (GameManager.GameStatus.river))
			return 0.1f;
		throw new UnityException ("game status not defined");
	}

	private static float gameStateEffectForStraightAndFlush (GameManager.GameStatus gameStatus)
	{
		if (gameStatus.Equals (GameManager.GameStatus.flop))
			return 1.3f;
		else if (gameStatus.Equals (GameManager.GameStatus.turn))
			return 1.1f;
		else if (gameStatus.Equals (GameManager.GameStatus.river))
			return 1f;
		return 0;
	}
}
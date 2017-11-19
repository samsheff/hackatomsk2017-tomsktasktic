using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prolog;

public class CompareHand
{

	KB kB;

	public CompareHand (KB kB)
	{
		this.kB = kB;
	}


	public List<int> getTheWinner (PlayersImplementation playersImplementation)
	{
		List<int> winners = new List<int> ();
		winners.Add (playersImplementation.PlayersInTheGame [0]);
		int result;
		for (int i = 1; i < playersImplementation.PlayersInTheGame.Count; i++) {
			result = compareTwoHands 
				(playersImplementation.players [winners [0]], 
				playersImplementation.players [playersImplementation.PlayersInTheGame [i]]);

			if (result == 0)
				winners.Clear ();
			if (result != 2)
				winners.Add (playersImplementation.PlayersInTheGame [i]);
		}
		return winners;
	}

	public int compareTwoHands (PlayerImplementation player1, PlayerImplementation player2)
	{

		int rank1 = HandStrengthEvaluater.rank (player1.evaluatedCards.name);
		int rank2 = HandStrengthEvaluater.rank (player2.evaluatedCards.name);

		string cardsEvaluated1 = "[";
		for (int i = 0; i < player1.evaluatedCards.Cards.Count; i++) {
			if (i < player1.evaluatedCards.Cards.Count - 1)
				cardsEvaluated1 += player1.evaluatedCards.Cards [i].CardValue + ",";
			else
				cardsEvaluated1 += player1.evaluatedCards.Cards [i].CardValue + "]";
		}

		string cardsEvaluated2 = "[";
		for (int i = 0; i < player2.evaluatedCards.Cards.Count; i++) {
			if (i < player2.evaluatedCards.Cards.Count - 1)
				cardsEvaluated2 += player2.evaluatedCards.Cards [i].CardValue + ",";
			else
				cardsEvaluated2 += player2.evaluatedCards.Cards [i].CardValue + "]";
		}

		string query = "X:result(" + rank1 + "," + rank2 + "," + cardsEvaluated1 + "," + cardsEvaluated2 + ",X).";
		int result = int.Parse (UnityExtensionMethods.SolveForParsed (kB, query).ToString ());

		return result;
	}
}
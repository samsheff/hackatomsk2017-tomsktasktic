using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prolog;
using UnityEngine;


public class HandEvaluater
{
	KB kB;

	public HandEvaluater (KB kB)
	{
		this.kB = kB;
	}

	public EvaluatedHand evaluateHand (List<Card> cards)
	{
		if (cards.Count <= 2)
			return null;

		assert (cards);
		string goal = "X:evaluate(X).";
		string result = UnityExtensionMethods.SolveForParsed (kB, goal).ToString ();
		
		List<Card> EvaluatedCards = new List<Card> ();
		for (int i = 1; i < 6; i++) {
			string cardValue = "X:cardValue(X," + i.ToString () + ").";
			string cardSuit = "Y:cardSuit(Y," + i.ToString () + ").";
			int v = int.Parse (UnityExtensionMethods.SolveForParsed (kB, cardValue).ToString ());
			var s = UnityExtensionMethods.SolveForParsed (kB, cardSuit);
			EvaluatedCards.Add (new Card (v, s.ToString ()));
		}

		string straightPotentialCase = "Y:straightPotential(Y).";
		int straightPotential = int.Parse (UnityExtensionMethods.SolveForParsed (kB, straightPotentialCase).ToString ());

		string flushPotentialCase = "Y:flushPotential(Y).";
		int flushPotential = int.Parse (UnityExtensionMethods.SolveForParsed (kB, flushPotentialCase).ToString ());

		EvaluatedHand evaluatedhand = new EvaluatedHand (result, EvaluatedCards, straightPotential, flushPotential);
		retract ();
		return evaluatedhand;
	}

	private void assert (List<Card> cards)
	{
		foreach (Card card in cards) {
			string goal = "assert(player(card(value(" + card.CardValue.ToString () + "),suit(" + card.suitValue + ")))).";
			UnityExtensionMethods.IsTrueParsed (kB, goal);
		}
	}

	private void retract ()
	{
		UnityExtensionMethods.IsTrueParsed (kB, "retractall(player(card(value(X),suit(Y)))).");
		UnityExtensionMethods.IsTrueParsed (kB, "retractall(cardSuit(X,Y)).");
		UnityExtensionMethods.IsTrueParsed (kB, "retractall(cardValue(X,Y)).");
		UnityExtensionMethods.IsTrueParsed (kB, "retractall(flushPotential(X)).");
		UnityExtensionMethods.IsTrueParsed (kB, "retractall(straightPotential(Y)).");
	}
}

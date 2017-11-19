using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EvaluatedHand
{
	public string name;
	public List<Card> Cards;

	public EvaluatedHand (string name, List<Card> cards, int straightPotential, int flushPotential)
	{
		this.name = name;
		this.Cards = cards;
		this.straightPotential = straightPotential;
		this.flushPotential = flushPotential;
	}

	public int straightPotential;
	public int flushPotential;

	public override string ToString ()
	{

		string hand = "Hand: " + name + "[ ";
		for (int i = 0; i < Cards.Count; i++) {
			if (i < Cards.Count - 1)
				hand += Cards [i].ToString () + ",";
			else
				hand += Cards [i].ToString ();
		}
		hand += " ]";
		return hand;
	}
}

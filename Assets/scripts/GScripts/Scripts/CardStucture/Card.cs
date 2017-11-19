using UnityEngine;
using System.Collections;

public class Card
{

	public Card (int value, string suit)
	{
		this.cardValue = value;
		switch (suit) {
		case "s":
			this.cardSuit = Suit.Spade;
			break;
		case "h":
			this.cardSuit = Suit.Heart;
			break;
		case "d":
			this.cardSuit = Suit.Diamond;
			break;
		case "c":
			this.cardSuit = Suit.Club;
			break;
		}
	}

	public Card (int value, Suit suit)
	{
		this.cardValue = value;
		this.cardSuit = suit;
	}

	public override bool Equals (object obj)
	{
		Card obj1 = (Card)obj;
		return obj1.cardSuit.Equals (this.cardSuit) && obj1.cardValue == this.cardValue;
	}

	public override int GetHashCode ()
	{
		return this.GetHashCode ();
	}

	public override string ToString ()
	{
		string name = replaceWithName (cardValue);
	
		return name + ":" + cardSuit.ToString ();
	}

	private int cardValue;
	private Suit cardSuit;

	public int CardValue {
		get {
			return cardValue;
		}
	}

	public string suitValue {
		get {
			return cardSuit.ToString ().Substring (0, 1).ToLower ();
		}
	}

	public static string replaceWithName (int value)
	{
		switch (value) {
		case 1:
			return "Ace";
		case 2:
			return "Two";
		case 3:
			return "Three";
		case 4:
			return "Four";
		case 5:
			return "Five";
		case 6:
			return "Six";
		case 7:
			return "Seven";
		case 8:
			return "Eight";
		case 9:
			return "Nine";
		case 10:
			return "Ten";
		case 11:
			return "Jack";
		case 12:
			return "Queen";
		case 13:
			return "King";
		default:
			throw new UnityException ("value not in the range[1,13]");
		}
	}

	public enum Suit
	{
		Club,
		Spade,
		Diamond,
		Heart
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CardsManager
{
	public List<Card> Cards 
	{ get { return cards; } }

	public CardsManager (GameManager gameManager)
	{
		this.gameManager = gameManager;
	}

	public void process ()
	{
		switch (gameManager.gameStatus ()) {
		case GameManager.GameStatus.preflop:
			{
				newCards ();
				giveCardsToPlayers ();
				break;
			}
		case GameManager.GameStatus.flop:
			{
				giveCardsToTable (3);
				break;
			}
		case GameManager.GameStatus.turn:
			{
				giveCardsToTable (1);
				break;
			}
		case GameManager.GameStatus.river:
			{
				giveCardsToTable (1);
				break;
			}
		}
	
	}

	private void giveCardsToPlayers ()
	{
		for (int i = 0; i < 2; i++) {
			foreach (PlayerImplementation player in gameManager.playersManager.playersImplementations.players) {
				player.getCard (cards [0]);
				cards.RemoveAt (0);
			}
		}
	}

	private void giveCardsToTable (int j)
	{
		for (int i = 0; i < j; i++) {
			cards.RemoveAt (0);
		}
		for (int i = 0; i < j; i++) {
			gameManager.communityCards.getCard (cards [0]);
			cards.RemoveAt (0);
		}
	}

	private static void Shuffle<T> (List<T> list)
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = Random.Range (0, n); 
			T value = list [k];  
			list [k] = list [n];  
			list [n] = value;  
		}  
	}


	private void newCards ()
	{
		List<Card> temp = new List<Card> ();
		temp.Add (new Card (1, "h"));
		temp.Add (new Card (2, "h"));
		temp.Add (new Card (3, "h"));
		temp.Add (new Card (4, "h"));
		temp.Add (new Card (5, "h"));
		temp.Add (new Card (6, "h"));
		temp.Add (new Card (7, "h"));
		temp.Add (new Card (8, "h"));
		temp.Add (new Card (9, "h"));
		temp.Add (new Card (10, "h"));
		temp.Add (new Card (11, "h"));
		temp.Add (new Card (12, "h"));
		temp.Add (new Card (13, "h"));

		temp.Add (new Card (1, "d"));
		temp.Add (new Card (2, "d"));
		temp.Add (new Card (3, "d"));
		temp.Add (new Card (4, "d"));
		temp.Add (new Card (5, "d"));
		temp.Add (new Card (6, "d"));
		temp.Add (new Card (7, "d"));
		temp.Add (new Card (8, "d"));
		temp.Add (new Card (9, "d"));
		temp.Add (new Card (10, "d"));
		temp.Add (new Card (11, "d"));
		temp.Add (new Card (12, "d"));
		temp.Add (new Card (13, "d"));

		temp.Add (new Card (1, "s"));
		temp.Add (new Card (2, "s"));
		temp.Add (new Card (3, "s"));
		temp.Add (new Card (4, "s"));
		temp.Add (new Card (5, "s"));
		temp.Add (new Card (6, "s"));
		temp.Add (new Card (7, "s"));
		temp.Add (new Card (8, "s"));
		temp.Add (new Card (9, "s"));
		temp.Add (new Card (10, "s"));
		temp.Add (new Card (11, "s"));
		temp.Add (new Card (12, "s"));
		temp.Add (new Card (13, "s"));

		temp.Add (new Card (1, "c"));
		temp.Add (new Card (2, "c"));
		temp.Add (new Card (3, "c"));
		temp.Add (new Card (4, "c"));
		temp.Add (new Card (5, "c"));
		temp.Add (new Card (6, "c"));
		temp.Add (new Card (7, "c"));
		temp.Add (new Card (8, "c"));
		temp.Add (new Card (9, "c"));
		temp.Add (new Card (10, "c"));
		temp.Add (new Card (11, "c"));
		temp.Add (new Card (12, "c"));
		temp.Add (new Card (13, "c"));

		Shuffle (temp);
		cards = temp;
	}

	private GameManager gameManager;
	private List<Card> cards;
}

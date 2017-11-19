using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayersManager :MonoBehaviour, PlayersHandler
{
	#region PlayersHandler implementation

	public void winnerProcess (PlayerImplementation playerImplementation)
	{
		Debug.Log (playerImplementation.Id + " is the winnner");
	}

	public void AccumulateChips ()
	{
		List<Transform> chips = new List<Transform> ();
		foreach (PlayerManager player in players)
			chips.AddRange (player.chips ());
		
		foreach (Transform obj in chips)
			obj.SetParent (gameManager.chipsPanel);
	}

	public void UpdatePlayersList ()
	{
		players = transform.GetComponentsInChildren<PlayerManager> ().ToList ();
	}

	public void NewGame ()
	{
		foreach (PlayerManager player in players)
			player.NewGame ();
	}


	public void showOff ()
	{
		foreach (PlayerManager player in players)
			player.ShowOff ();
	}

	#endregion


	public void init (PlayerManager p)
	{
		i++;
		int childCount = transform.GetComponentsInChildren<PlayerManager> ().Count ();
		if (childCount == i) {
			gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
			playersImplementations = new PlayersImplementation (this, gameManager, GameManager.bigBlindChips,
				GameManager.bigBlindChips / 2);
		}
	}

	private int i = 0;

	public List<PlayerManager> Players 
	{ get { return players; } }

	private List<PlayerManager> players;
	[HideInInspector]
	public GameManager gameManager;
	public GameObject fiveHandredChips;
	public GameObject oneGrand;
	public PlayersImplementation playersImplementations;
}
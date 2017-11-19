using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayersImplementation
{
	public PlayersImplementation (PlayersManager playersManager, GameManager gameManager,
	                              int bigBlindChips, int smallBlindChips)
	{
		bigBlindIndex = -1;// new Game will increment the bigblind index;

		this.bigBlindChips = bigBlindChips;
		this.smallBlindChips = smallBlindChips;

		this.playersManager = playersManager;
		players = new List<PlayerImplementation> ();
		UpdatePlayersList ();
		chipsManager = new ChipsManager (this);
		chipsManager.newTournament ();
		this.gameManager = gameManager;
	}

	public void process ()
	{		
		if (gameManager.gameStatus ().Equals (GameManager.GameStatus.preflop))
			NewGame ();
		evaluateHand ();
		newRound ();
	}

	private void NewGame ()
	{
		ExecuteEvents.Execute<PlayersHandler> (playersManager.gameObject, null, (x, y) => x.NewGame ());

		bigBlindIndex = (bigBlindIndex + 1) % players.Count;
		smallBlindIndex = (bigBlindIndex + 1) % players.Count;

		currentPot = bigBlindChips + smallBlindChips;
		gameManager.editPotText (currentPot);

		chipsManager.takeChips (players [bigBlindIndex], bigBlindChips);
		chipsManager.takeChips (players [smallBlindIndex], smallBlindChips);

		playersInTheGame = new List<int> ();
		int order = bigBlindIndex;
		for (int i = 0; i < players.Count; i++) {
			playersInTheGame.Add (order);
			order = (order + 1) % players.Count;
		}
	}

	public void newRound ()
	{
		gameManager.humanPanel.UpdateControlling ();
		round = new Round (this, playersInTheGame, currentPot, bigBlindChips);
		round.RoundProcess ();
	}

	IEnumerator decideTheWinner ()
	{
		ExecuteEvents.Execute<PlayersHandler> (playersManager.gameObject, null, (x, y) => x.showOff ());
		List<int> winners = gameManager.compareHands.getTheWinner (this);
	
		gameManager.HandWinning (players [winners [0]].evaluatedCards);

		gameManager.editPotText (0);
		for (int i = 0; i < winners.Count; i++) {
			chipsManager.giveChips (players [winners [i]], currentPot / winners.Count);
			players [winners [i]].visualizeWinning ();	
		}

		yield return new WaitForSeconds (3);
		UpdatePlayersList ();
		gameManager.gameFinished ();
	}

	public void finishRound ()
	{
		currentPot = round.context.CurrentPot;
		playersInTheGame = round.context.playerStillInTheGame;
		ExecuteEvents.Execute<PlayersHandler> (playersManager.gameObject, null, (x, y) => x.AccumulateChips ());

		if (round.context.FinishedEarlier) {
			gameManager.StartCoroutine (everyBodyFold ());
			return;
		}
		if (round.context.state ().Equals (GameManager.GameStatus.river)) {
			gameManager.StartCoroutine (decideTheWinner ());
		} else {
			gameManager.continuePlaying ();
		}
	}

	public void recieveMessage (PlayerAction playerAction)
	{
		round.recieveMessageFromPlayer (playerAction);
	}

	private void evaluateHand ()
	{
		foreach (PlayerImplementation player in players)
			player.evaluateHand ();
	}

	void UpdatePlayersList ()
	{
		players.Clear ();
		playersManager.UpdatePlayersList ();
		foreach (PlayerManager player in playersManager.Players) {
			player.playerImplementation.destroyCards ();
			players.Add (player.playerImplementation);
		}
	}

	IEnumerator everyBodyFold ()
	{
		gameManager.editPotText (0);
		chipsManager.giveChips (players [playersInTheGame [0]], currentPot);
		yield return new WaitForSeconds (2);
		UpdatePlayersList ();
		gameManager.gameFinished ();
	}

	public void getPlayerAction (int playerIndex, Context context)
	{
		players [playerIndex].Action (context);
	}

	public ChipsManager chipsManager;

	public List<int> PlayersInTheGame
	{ get { return playersInTheGame; } }

	int bigBlindChips;
	int smallBlindChips;
	int bigBlindIndex;
	int smallBlindIndex;
	Round round;
	int currentPot;
	public PlayersManager playersManager;
	public List<PlayerImplementation> players;
	List<int> playersInTheGame;
	public GameManager gameManager;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context
{
	public Context (PlayersImplementation playersImplementation, List<int> PlayersInTheGame, int currentPot, int bigBlindChips)
	{
		this.playersImplementation = playersImplementation;
		this.PlayersInTheGame = new List<int> ();
		PlayersToGetAction = PlayersInTheGame;
		this.pot = currentPot;
		this.bigBlind = bigBlindChips;
		minimumAmountToCall = 0;
		currentAction = Context.CurrentAction.checking;
		finishedEarlier = false;
	}

	public bool finished ()
	{
		return ((PlayersToGetAction.Count == 0) || (finishedEarlier));
	}

	public int CurrentPlayer ()
	{
		return PlayersToGetAction [0];
	}

	public void Update (PlayerAction LastPlayerAction)
	{
		if (LastPlayerAction.isRaised) {
			minimumAmountToCall = LastPlayerAction.raiseAction.Amount;
			playersImplementation.chipsManager.takeChips 
			(playersImplementation.players [PlayersToGetAction [0]], minimumAmountToCall);
			updatePot (minimumAmountToCall);

			PlayersToGetAction.AddRange (PlayersInTheGame);//make new round;
			
			PlayersInTheGame.Clear ();
			PlayersInTheGame.Add (PlayersToGetAction [0]);// untill now the one who raise still in the game
			
			ContextCurrentAction = CurrentAction.calling;// change the status to calling
		} else {
			if (LastPlayerAction.getAction.Equals (PlayerAction.Action.Fold)) {
				playersImplementation.players [PlayersToGetAction [0]].visualizeFold ();
			}
			if (LastPlayerAction.getAction.Equals (PlayerAction.Action.Call)) {
				playersImplementation.chipsManager.takeChips 
				(playersImplementation.players [PlayersToGetAction [0]], minimumAmountToCall);
				updatePot (minimumAmountToCall);
				
				PlayersInTheGame.Add (PlayersToGetAction [0]);
			}
			
			if (LastPlayerAction.getAction.Equals (PlayerAction.Action.Check)) {
				PlayersInTheGame.Add (PlayersToGetAction [0]);
			}
		}
			
		// if he make an action , in the current round he will never been asked again else someone raised
		// and he still in the game
		PlayersToGetAction.RemoveAt (0);

		specialCaseSmallBlindPlayerAtPreflop ();
		if (specialCaseIfSomeOneRaiseAndNoBodyCall ())
			finishedEarlier = true;
	}

	private void specialCaseSmallBlindPlayerAtPreflop ()
	{
		counter++;
		if (counter == 1) {
			// if the small blind player -> adjust the amount to call if the first didn't raise;
			if ((minimumAmountToCall == 0) && (state ().Equals (GameManager.GameStatus.preflop)))
				minimumAmountToCall = bigBlind / 2; 
		} else if ((counter == 2) && (state ().Equals (GameManager.GameStatus.preflop))) {
			// after small blind player adjust the amount to call if it didn't raise;
			if (minimumAmountToCall == bigBlind / 2)
				minimumAmountToCall = bigBlind;
		}
		//else don't do any thing..
	}

	private bool specialCaseIfSomeOneRaiseAndNoBodyCall ()
	{
		return PlayersToGetAction.Count == 0 && PlayersInTheGame.Count == 1;
	}

	private void updatePot (int toAdd)
	{
		pot += toAdd;
		playersImplementation.gameManager.editPotText (pot);
	}

	public List<int> playerStillInTheGame {
		get {
			return PlayersInTheGame;
		}
	}

	public List<int> playersToGetAction {
		get {
			return PlayersToGetAction;
		}
	}

	public CurrentAction currentAction {
		get {
			return ContextCurrentAction;
		}
		set {
			ContextCurrentAction = value;
		}
	}

	CurrentAction ContextCurrentAction;
	PlayersImplementation playersImplementation;
	int pot;
	int bigBlind;
	int minimumAmountToCall;
	List<int> PlayersInTheGame;
	List<int> PlayersToGetAction;
	bool finishedEarlier;
	/// <summary>
	/// special case small blind player , amount call is half the big blind
	/// </summary>
	int counter;

	public bool FinishedEarlier
	{ get { return finishedEarlier; } }

	public int CallAmount {
		get {
			return minimumAmountToCall;
		}
	}

	public int CurrentPot {
		get {
			return pot;
		}
	}

	public int BigBlind {
		get {
			return bigBlind;
		}
	}

	public GameManager.GameStatus state ()
	{
		return playersImplementation.gameManager.gameStatus ();
	}

	public enum CurrentAction
	{
		calling,
		checking
	}
}
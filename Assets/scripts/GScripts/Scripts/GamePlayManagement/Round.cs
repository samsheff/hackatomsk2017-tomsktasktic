using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Round
{
	public void RoundProcess ()
	{
		// send first message;
		sendMessageToPlayer ();
	}


	public Round (PlayersImplementation playersImplementation, List<int> playerStillInTheGame, int currentPot, int bigBlindChips)
	{
		this.playersImplementation = playersImplementation;
		context = new Context (playersImplementation, playerStillInTheGame, currentPot, bigBlindChips);
	}


	void sendMessageToPlayer ()
	{
		playersImplementation.getPlayerAction (context.CurrentPlayer (), context);
	}

	public void recieveMessageFromPlayer (PlayerAction playerAction)
	{
		if ((playersImplementation.gameManager.gameStatus ().Equals (GameManager.GameStatus.preflop)) && (counter == 0))
			context.currentAction = Context.CurrentAction.calling;

		context.Update (playerAction);
		if (!context.finished ()) {
			sendMessageToPlayer ();
		} else
			playersImplementation.finishRound ();

		counter++;
	}

	int counter = 0;
	PlayerAction LastPlayerAction;
	PlayersImplementation playersImplementation;
	public Context context;
}
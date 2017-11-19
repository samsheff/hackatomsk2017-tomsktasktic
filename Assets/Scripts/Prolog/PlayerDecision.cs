using UnityEngine;
using System.Collections;
using Prolog;

public class PlayerDecision : MonoBehaviour
{

	KB kB;

	public PlayerDecision (KB kB)
	{
		this.kB = kB;
	}


	public PlayerAction MakeDecision (PlayerImplementation Player, Context context)
	{
		PlayerAction playerAction = new PlayerAction ();
		assert (Player, context);
		bool raised = false;
		int raiseAmount;
		string decision = "Y:finalDecision(Y).";
		string result = UnityExtensionMethods.SolveForParsed (kB, decision).ToString ();

		if (result.Equals ("fold")) {
			playerAction.set (PlayerAction.Action.Fold);
		}
		if (result.Equals ("call")) {
			playerAction.set (PlayerAction.Action.Call);
		}
		if (result.Equals ("raise")) {
			raised = true;
			string raiseAmountQuery = "X:raiseAmount(X).";
			raiseAmount = int.Parse (UnityExtensionMethods.SolveForParsed (kB, raiseAmountQuery).ToString ());
			if (raiseAmount == 0)
				playerAction.set (PlayerAction.Action.Check);
			else
				playerAction.raise (new PlayerAction.Raise (raiseAmount));
		}

		retract (raised);

		return playerAction;
	}

	private void assert (PlayerImplementation Player, Context context)
	{
		
		int PlayerCount = context.playersToGetAction.Count;
		int Pot = context.CurrentPot;
		int bigBlind = context.BigBlind;
		string status = context.currentAction.ToString ();
		float HandStrength = Player.HandStrength;
		float HandPotential = Player.HandPotential;
		string state = context.state ().ToString ();
		int callAmount = context.CallAmount;
		int PlayerMoney = Player.Chips;

		Debug.Log (PlayerCount);
		Debug.Log (Pot);
		Debug.Log (bigBlind);
		Debug.Log (status);
		Debug.Log (HandStrength);
		Debug.Log (HandPotential);
		Debug.Log (state);
		Debug.Log (callAmount);
		Debug.Log (PlayerMoney);
		Debug.Log ("*******");

		string goal = "decisionResult(" + PlayerCount + "," +
		              Pot + "," + bigBlind + "," + status + "," + HandStrength + "," +
		              HandPotential + "," + state + "," + callAmount + "," + PlayerMoney + ").";

		UnityExtensionMethods.IsTrueParsed (kB, goal);
	}

	private void retract (bool raised)
	{
		UnityExtensionMethods.IsTrueParsed (kB, "retractall(finalDecision(X)).");
		if (raised)
			UnityExtensionMethods.IsTrueParsed (kB, "retractall(raiseAmount(X)).");
	}
}

using UnityEngine;
using System.Collections;

public class PlayerAction
{
	public void set (Action action)
	{
		this.action = action;
	}

	public void raise (Raise raise)
	{
		this.raiseAction = raise;
		raiseStatus = true;
		action = Action.Raise;
	}

	public Raise raiseAction;
	Action action;
	bool raiseStatus = false;

	public Action getAction {
		get {
			return action;
		}
	}

	public bool isRaised {
		get {
			return raiseStatus;
		}
	}
	public enum Action
	{
		Check,
		Call,
		Fold,
		Raise
	}

	public class Raise
	{
		public Raise (int amount)
		{
			this.amount = amount;
		}
		int amount;
		public int Amount {
			get {
				return amount;
			}
		}
	}
}

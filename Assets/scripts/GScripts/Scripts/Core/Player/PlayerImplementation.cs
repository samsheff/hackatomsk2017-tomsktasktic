using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerImplementation
{
	public PlayerImplementation (PlayerManager PlayerMananger, GameManager gameManager, string id)
	{
		Player = PlayerMananger;
		this.gameManager = gameManager;	
		this.id = id;
		cards = new List<Card> ();
	}

	public EvaluatedHand evaluatedCards 
	{ get { return evaluatedhand; } }

	public int Chips
	{ get { return gameManager.playersManager.playersImplementations.chipsManager.Player (this); } }

	public string Id 
	{ get { return id; } }

	public float HandStrength
	{ get { return handStrength; } }

	public float HandPotential
	{ get { return handPotential; } }

	public void getCard (Card card)
	{
		if (cards.Count > 2)
			throw new UnityException ("player can only get two private cards");

		cards.Add (card);
		ExecuteEvents.Execute<PlayerHandler> (Player.gameObject, null, (x, y) => x.getCard (card));
	}

	public void visualizeWinning ()
	{
		ExecuteEvents.Execute<PlayerHandler> (Player.gameObject, null, (x, y) => x.visualizeWinning ());

	}

	public void Action (Context context)
	{
		ExecuteEvents.Execute<PlayerHandler> (Player.gameObject, null, (x, y) => x.StartAction ());

		if (Player.tag.Equals ("GameController")) {
			gameManager.humanPanel.disappear (false);
			gameManager.humanPanel.showPanel (context.currentAction);
		} else {
			//gameManager.makeDecision (this, context);
			gameManager.StartCoroutine (RandDecision (context));
		}
	}

	public void visualizeChips (int chips)
	{
		ExecuteEvents.Execute<PlayerHandler> (Player.gameObject, null, (x, y) => x.VisualizeChips (chips));
	}

	public void visualizeFold ()
	{
		ExecuteEvents.Execute<PlayerHandler> (Player.gameObject, null, (x, y) => x.visualizeFold ());
	}

	public void setAction (PlayerAction playerAction)
	{
		if (Player.tag.Equals ("GameController")) {
			gameManager.humanPanel.disappear (true);
		}
		gameManager.playersManager.playersImplementations.recieveMessage (playerAction);

		ExecuteEvents.Execute<PlayerHandler> (Player.gameObject, null, (x, y) => x.EndOfAction ());
	}

	public void destroyCards ()
	{
		cards.Clear ();
		ExecuteEvents.Execute<PlayerHandler> (Player.gameObject, null, (x, y) => x.destroyCards ());
	}

	public void evaluateHand ()
	{
		List<Card> allCards = new List<Card> ();
		allCards.AddRange (cards);
		if (gameManager.communityCards.cards.Count > 0) {
			allCards.AddRange (gameManager.communityCards.cards);
			gameManager.evaluateHand (allCards, this);
		} else {
			handStrength = StartHandEvaluater.evaluateStartHand (allCards);
			handPotential = 0;
			ExecuteEvents.Execute<PlayerHandler> (Player.gameObject, null, (x, y) => x.HandIsEvaluated ());
		}
	}

	public void setHandEvaluated (EvaluatedHand evaluatedhand)
	{
		this.evaluatedhand = evaluatedhand;

		handStrength = HandStrengthEvaluater.Evaluate 
				(evaluatedhand.name, cards, evaluatedhand.Cards, gameManager.gameStatus ()) / 100f;
		int rank = HandStrengthEvaluater.rank (evaluatedhand.name);

		handPotential = HandPotentialEvaluater.Evaluate 
			(rank, evaluatedhand.flushPotential, evaluatedhand.straightPotential, gameManager.gameStatus ()) / 10f;
		
		ExecuteEvents.Execute<PlayerHandler> (Player.gameObject, null, (x, y) => x.HandIsEvaluated ());
	}

	IEnumerator RandDecision (Context context)
	{
		yield return new WaitForSeconds (Random.Range (0.2f, 0.7f));
		PlayerAction d = new PlayerAction ();

		if (context.currentAction.Equals (Context.CurrentAction.calling)) {
			d.set (PlayerAction.Action.Call);
		} else {
			d.set (PlayerAction.Action.Check);
		}
		setAction (d);
		yield return null;
	}

	public PlayerManager PlayerInstance
	{ get { return Player; } }

	public List<Card> Cards
	{ get { return cards; } }

	PlayerManager Player;
	private string id;
	private List<Card> cards;
	private float handStrength;
	private float handPotential;
	private EvaluatedHand evaluatedhand;
	private GameManager gameManager;
}
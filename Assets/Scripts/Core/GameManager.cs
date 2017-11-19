using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Prolog;

public class GameManager : MonoBehaviour
{
	void Start ()
	{
		kB = transform.Find ("GlobalKB").GetComponent<KB> ();
		canvas = GameObject.Find ("Canvas").GetComponent<Canvas> ();
		communityCards = canvas.transform.Find ("GamePlay").Find ("Table").Find ("CommunityCards").GetComponent<CommunityCards> ();
		humanPanel = canvas.transform.Find ("GamePlay").Find ("HumanPanel").GetComponent<HumanPanel> ();
		playersManager = canvas.transform.Find ("GamePlay").Find ("Table").Find ("Players").GetComponent<PlayersManager> ();
		chipsPanel = canvas.transform.Find ("GamePlay").Find ("Table").Find ("Pot");
		CardsObjects = transform.Find ("Cards").gameObject;
		ChipsObjects = transform.Find ("Chips").gameObject;
		ClosedCard = transform.Find ("ClosedCard").GetComponent<Image> ();
		PotAmountText = canvas.transform.Find ("GamePlay").Find ("Table").Find ("PotAmount").GetComponent<Text> ();
		HandType = canvas.transform.Find ("GamePlay").Find ("Table").Find ("HandType").GetComponent<Text> ();
		soundManager = GetComponent<SoundManager> ();
		handEvaluater = new HandEvaluater (kB);
		playerDecision = new PlayerDecision (kB);
		compareHands = new CompareHand (kB);
		orders = new Queue<EvaluateHandOrder> ();
		EvaluationProcess = false;
		cardsManager = new CardsManager (this);
		gameStatusCounter = -1;
		waitingState = true;
	}

	public GameObject instantiateNewCard (Card card)
	{
		string name = card.CardValue + card.suitValue;
		GameObject obj = Instantiate (CardsObjects.transform.Find (name).gameObject)as GameObject;
		obj.name = name;
		return obj;
	}

	public List<GameObject> instantiateChip (int Chips)
	{
		List<GameObject> objs = new List<GameObject> ();
		int fiveHandredChips = Chips / 500;
		for (int i = 0; i < fiveHandredChips; i++) {
			objs.Add (Instantiate (ChipsObjects.transform.Find ("500").gameObject)as GameObject);
		}	
		return objs;
	}

	public void continuePlaying ()
	{
		StartCoroutine (continuePlayingProcess ());
	}

	IEnumerator continuePlayingProcess ()
	{
		float waitinTime = waitingState ? waitingStatePeriod : 0;
		UpdateGameStatus ();
		yield return new WaitForSeconds (waitinTime);
		cardsManager.process ();
		yield return new WaitForSeconds (waitinTime);
		playersManager.playersImplementations.process ();
	}

	private void UpdateGameStatus ()
	{
		gameStatusCounter = (gameStatusCounter + 1) % 4;
	}

	public void gameFinished ()
	{
		ClearHandWinning ();
		communityCards.NewGame ();
		gameStatusCounter = -1;
	}

	public void HandWinning (EvaluatedHand EvalHand)
	{
		HandType.text = EvalHand.name;
	}

	private void ClearHandWinning ()
	{
		HandType.text = "";
	}

	public void destroyPotChips ()
	{
		for (int i = 0; i < chipsPanel.childCount; i++) {
			Destroy (chipsPanel.GetChild (i).gameObject);
		}
	}

	public void editPotText (int newAmount)
	{
		if (newAmount == 0)
			PotAmountText.text = "";
		else
			PotAmountText.text = newAmount + " $";
	}

	public void evaluateHand (List<Card> cards, PlayerImplementation playerImplementation)
	{
		orders.Enqueue (new EvaluateHandOrder (cards, playerImplementation));
		if (!EvaluationProcess)
			StartCoroutine (evaluateHandProcess ());
	}

	IEnumerator evaluateHandProcess ()
	{
		EvaluationProcess = true;
		while (orders.Count > 0) {
			EvaluateHandOrder order = orders.Dequeue ();
			order.player.setHandEvaluated (handEvaluater.evaluateHand (order.cards));
		}
		EvaluationProcess = false;
		yield return null;
	}

	public void makeDecision (PlayerImplementation player, Context context)
	{
		StartCoroutine (makeDecisionProcess (player, context));
	}

	IEnumerator makeDecisionProcess (PlayerImplementation player, Context context)
	{
		PlayerAction action = playerDecision.MakeDecision (player, context);
		yield return new WaitForSeconds (Random.Range (0.2f, 0.7f));
		player.setAction (action);
		yield return null;
	}

	class EvaluateHandOrder
	{
		public EvaluateHandOrder (List<Card> cards, PlayerImplementation player)
		{
			this.cards = cards;
			this.player = player;
		}

		public List<Card> cards;
		public PlayerImplementation player;
	}

	Queue<EvaluateHandOrder> orders;

	KB kB;

	[HideInInspector]
	Text PotAmountText;
	[HideInInspector]
	public CompareHand compareHands;
	private Canvas canvas;
	bool EvaluationProcess;
	[HideInInspector]
	public HumanPanel humanPanel;
	private CardsManager cardsManager;
	private PlayerDecision playerDecision;
	private HandEvaluater handEvaluater;
	[HideInInspector]
	public CommunityCards communityCards;
	[HideInInspector]
	public PlayersManager playersManager;
	[HideInInspector]
	public Transform chipsPanel;
	private int gameStatusCounter;
	private GameObject CardsObjects;
	private GameObject ChipsObjects;
	private Text HandType;
	[HideInInspector]
	public SoundManager soundManager;
	[HideInInspector]
	public Image ClosedCard;

	public GameStatus gameStatus ()
	{
		switch (gameStatusCounter) {
		case 0:
			return GameStatus.preflop;
		case 1:
			return GameStatus.flop;
		case 2:
			return GameStatus.turn;
		case 3:
			return GameStatus.river;
		}
		throw new UnityException ("gameStatus error");
	}

	public enum GameStatus
	{
		preflop,
		flop,
		turn,
		river
	}

	public const int waitingPlayer = 2;
	public const float waitingStatePeriod = 1f;
	public const int bigBlindChips = 1000;
	/// <summary>
	/// The state of the waiting. if you want to make tree search make this false;
	/// </summary>
	private bool waitingState;
}

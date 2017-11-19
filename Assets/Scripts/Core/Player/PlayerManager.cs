using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class PlayerManager : MonoBehaviour,PlayerHandler
{
	#region PlayerHandler implementation

	public void StartAction ()
	{
		StartCoroutine (waitingProcess ());
	}

	public void EndOfAction ()
	{
		decisionHaveBeenMade = true;
	}

	public void HandIsEvaluated ()
	{
		//Debug.Log (playerImplementation.Id + " Hand Strength: " + playerImplementation.HandStrength);
	}

	public List<Transform> chips ()
	{
		List<Transform> trans = new List<Transform> ();
		for (int i = 0; i < ChipsPanel.childCount; i++)
			trans.Add (ChipsPanel.GetChild (i).transform);
		return trans;
	}

	public void FoldVisualize ()
	{
		transform.localScale = new Vector3 (.8f, .8f, 1);
	}

	public void getCard (Card card)
	{
		Transform trans = gameManager.instantiateNewCard (card).transform;
		trans.SetParent (CardsHolder);
		trans.localScale = new Vector3 (1, 1, 1);
        if (!gameObject.CompareTag("GameController"))
    		trans.GetComponent<Image> ().enabled = false;
	}

	public void ShowOff ()
	{
		ShowCards (true);
		interactable = false;
	}

	public void ShowCards (bool status)
	{
		if (!interactable)
			return;
		for (int i = 0; i < CardsHolder.childCount; i++)
			CardsHolder.GetChild (i).GetComponent<Image> ().enabled = status;
	}

	public void destroyCards ()
	{
		for (int i = 0; i < CardsHolder.childCount; i++)
			Destroy (CardsHolder.GetChild (i).gameObject);


		for (int i = 0; i < ChipsPanel.childCount; i++)
			Destroy (ChipsPanel.GetChild (i).gameObject);
	}

	public void visualizeFold ()
	{
		transform.localScale = new Vector3 (0.7f, 0.7f, 1);
	}

	public void visualizeWinning ()
	{
		bool con = false;
		foreach (Card card in playerImplementation.evaluatedCards.Cards) {
			con = false;
			foreach (Card card1 in playerImplementation.Cards) {
				if (card1.Equals (card)) {
					CardsHolder.Find (card1.CardValue + card1.suitValue).
							transform.localScale = new Vector3 (1.3f, 1.3f, 1);
					con = true;
					break;
				}
			}
			if (con)
				continue;
			foreach (Card card2 in gameManager.communityCards.cards) {
				if (card2.Equals (card)) {
					gameManager.communityCards.transform.Find (card2.CardValue + card2.suitValue).
							transform.localScale = new Vector3 (1.2f, 1.2f, 1);
					break;
				}
			}
		}
	}

	public void VisualizeChips (int chips)
	{
		List<GameObject> objs = gameManager.instantiateChip (chips);
		foreach (GameObject obj in objs) {
			obj.transform.SetParent (ChipsPanel);
			obj.transform.localScale = new Vector3 (1, 1, 1);
		}
	}

	public void NewGame ()
	{
		transform.localScale = new Vector3 (1, 1, 1);
		interactable = true;
	}

	public void editChipsText (int newAmount)
	{
		playerMoney.text = newAmount + " $";
	}

	#endregion


	// Use this for initialization
	void Start ()
	{
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		playerImplementation = new PlayerImplementation (this, gameManager, transform.name);

		CardsHolder = transform.Find ("Cards");
		waitingImage = transform.Find ("Image").Find ("Waiting").GetComponent<Image> ();
		ChipsPanel = transform.Find ("Chips");
		Transform center = GameObject.Find ("Canvas").transform.Find ("Center");
		playerMoney = transform.Find ("money").Find ("ChipsData").GetComponent<Text> ();

		float y = center.position.y - ChipsPanel.position.y;
		float x = center.position.x - ChipsPanel.position.x;
		float angle = (Mathf.Atan2 (y, x) * Mathf.Rad2Deg) - 90;
		ChipsPanel.Rotate (new Vector3 (0, 0, angle));
		ChipsPanel.position += radius * ChipsPanel.TransformDirection (Vector3.up);

		transform.GetComponentInParent<PlayersManager> ().init (this);
	}

	Text playerMoney;
	float radius = 44;
	[HideInInspector]
	public PlayerImplementation playerImplementation;
	[HideInInspector]
	public Transform CardsHolder;
	Transform ChipsPanel;
	Image waitingImage;
	bool decisionHaveBeenMade;
	bool interactable;
	[HideInInspector]
	public GameManager gameManager;

	IEnumerator waitingProcess ()
	{
		float waitingPeriod = GameManager.waitingPlayer;
		waitingImage.fillAmount = 1;
		decisionHaveBeenMade = false;
		while (!decisionHaveBeenMade) {
			waitingImage.fillAmount -= 0.01f;
			yield return new WaitForSeconds (0.02f);
			waitingPeriod -= Time.deltaTime;
		}
		waitingImage.fillAmount = 1;
	}

}
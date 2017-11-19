using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommunityCards : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		cards = new List<Card> ();
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		queue = new Queue<Card> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void getCard (Card card)
	{
		cards.Add (card);
		gameManager.StartCoroutine (getCardProcess (card));
	}

	IEnumerator getCardProcess (Card card)
	{
		queue.Enqueue (card);
		while (queue.Count > 0) {
			Card temp = queue.Dequeue ();
			yield return new WaitForSeconds (0.3f);
			gameManager.soundManager.PlayCardSound ();
			Transform trans = gameManager.instantiateNewCard (temp).transform;
			trans.SetParent (this.transform);
			trans.localScale = new Vector3 (1, 1, 1);
		}
	}

	public void NewGame ()
	{
		cards.Clear ();
		for (int i = 0; i < transform.childCount; i++)
			Destroy (transform.GetChild (i).gameObject);
		
	}

	public List<Card> cards;
	GameManager gameManager;
	Queue<Card> queue;
}

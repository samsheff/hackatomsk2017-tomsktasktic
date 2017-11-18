using UnityEngine;
using System.Collections;

public class Shared : MonoBehaviour {

	private int n=0;
	public GameObject card1;
	public GameObject card2;
	public GameObject card3;
	public GameObject card4;
	public GameObject card5;

	public void addCard(string name)
	{
		n++;
		switch (n) {
		case 1:
			card1.GetComponent<Card>().SetCard(name);
			break;
		case 2:
			card2.GetComponent<Card>().SetCard(name);
			break;
		case 3:
			card3.GetComponent<Card>().SetCard(name);
			break;
		case 4:
			card4.GetComponent<Card>().SetCard(name);
			break;
		case 5:
			card5.GetComponent<Card>().SetCard(name);
			break;
		}
	}

	public void clearCards ()
	{
		n = 0;
		card1.GetComponent<Card>().SetCard("back");
		card2.GetComponent<Card>().SetCard("back");
		card3.GetComponent<Card>().SetCard("back");
		card4.GetComponent<Card>().SetCard("back");
		card5.GetComponent<Card>().SetCard("back");
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}

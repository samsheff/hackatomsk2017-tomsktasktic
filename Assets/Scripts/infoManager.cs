using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class infoManager : MonoBehaviour
{
	void Start ()
	{
		infoPanel = this.transform.GetComponent<Image> ();
		PlayerName = this.transform.Find ("Name").GetComponent<Text> ();
		handType = this.transform.Find ("Hand").GetComponent<Text> ();
		HisCards = this.transform.Find ("Cards").transform;
	}

	public void showInfo (PlayerManager player)
	{
		infoPanel.enabled = true;
		PlayerName.text = player.name;
		handType.text = player.playerImplementation.evaluatedCards.name;
		for (int i = 0; i < player.CardsHolder.childCount; i++) {
			GameObject obj = Instantiate (player.CardsHolder.GetChild (i).gameObject)as GameObject;
			obj.transform.localScale = new Vector3 (1, 1, 1);
			obj.transform.SetParent (HisCards);
		}
	}

	public void test ()
	{
		Debug.Log ("test");
	}

	public void hideInfo ()
	{
		PlayerName.text = "";
		handType.text = "";
		for (int i = 0; i < HisCards.childCount; i++)
			Destroy (HisCards.GetChild (i).gameObject);
		infoPanel.enabled = false;
	}

	Text PlayerName;
	Text handType;
	Transform HisCards;
	Image infoPanel;
}

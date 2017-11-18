using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public GameObject card1;
    public GameObject card2;
	public GameObject nick;
	public GameObject arrow;
    public void setCards(string name1, string name2)
    {
        card1.GetComponent<Card>().SetCard(name1);
        card2.GetComponent<Card>().SetCard(name2);
    }

    public void setNick (string nazwa)
    {
  
        nick.GetComponent<Text>().text = nazwa;
    }

    public string getNick()
    {
        return nick.GetComponent<Text>().text;
	}

	public void setCurrent(bool state)
	{
		arrow.SetActive (state);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

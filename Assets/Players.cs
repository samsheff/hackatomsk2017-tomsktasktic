using UnityEngine;
using System.Collections;

public class Players : MonoBehaviour {

    public GameObject player0;
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public GameObject player5;
    public GameObject player6;
    public GameObject player7;
	public GameObject player8;
	public GameObject you;

    // Use this for initialization
    void Start () {
	
	}

	public void SetPlayers(Stack players){
		player0.SetActive (false);
		player1.SetActive (false);
		player2.SetActive (false);
		player3.SetActive (false);
		player4.SetActive (false);
		player5.SetActive (false);
		player6.SetActive (false);
		player7.SetActive (false);
		player8.SetActive (false);

		if (players.Count == 0)
			return;
		player0.SetActive (true);
		player0.GetComponent<Player>().setNick( (string)players.Pop ());

		if (players.Count == 0)
			return;
		player1.SetActive (true);
		player1.GetComponent<Player>().setNick( (string)players.Pop ());

		if (players.Count == 0)
			return;
		player2.SetActive (true);
		player2.GetComponent<Player>().setNick( (string)players.Pop ());

		if (players.Count == 0)
			return;
		player3.SetActive (true);
		player3.GetComponent<Player>().setNick( (string)players.Pop ());

		if (players.Count == 0)
			return;
		player4.SetActive (true);
		player4.GetComponent<Player>().setNick( (string)players.Pop ());

		if (players.Count == 0)
			return;
		player5.SetActive (true);
		player5.GetComponent<Player>().setNick( (string)players.Pop ());

		if (players.Count == 0)
			return;
		player6.SetActive (true);
		player6.GetComponent<Player>().setNick( (string)players.Pop ());

		if (players.Count == 0)
			return;
		player7.SetActive (true);
		player7.GetComponent<Player>().setNick( (string)players.Pop ());

		if (players.Count == 0)
			return;
		player8.SetActive (true);
		player8.GetComponent<Player>().setNick( (string)players.Pop ());
	}

	public void setCurrent(string name){
		player0.GetComponent<Player> ().setCurrent (false);
		player1.GetComponent<Player> ().setCurrent (false);
		player2.GetComponent<Player> ().setCurrent (false);
		player3.GetComponent<Player> ().setCurrent (false);
		player4.GetComponent<Player> ().setCurrent (false);
		player5.GetComponent<Player> ().setCurrent (false);
		player6.GetComponent<Player> ().setCurrent (false);
		player7.GetComponent<Player> ().setCurrent (false);
		player8.GetComponent<Player> ().setCurrent (false);
		you.GetComponent<Player> ().setCurrent (false);

		GetPlayer (name).GetComponent<Player> ().setCurrent (true);
	}
	
	public GameObject GetPlayer (string name)
    {
		if (player0.GetComponent<Player> ().getNick ().Equals (name)) {
			return player0;
		} else if (player1.GetComponent<Player> ().getNick ().Equals (name)) {
			return player1;
		} else if (player2.GetComponent<Player> ().getNick ().Equals (name)) {
			return player2;
		} else if (player3.GetComponent<Player> ().getNick ().Equals (name)) {
			return player3;
		} else if (player4.GetComponent<Player> ().getNick ().Equals (name)) {
			return player4;
		} else if (player5.GetComponent<Player> ().getNick ().Equals (name)) {
			return player5;
		} else if (player6.GetComponent<Player> ().getNick ().Equals (name)) {
			return player6;
		} else if (player7.GetComponent<Player> ().getNick ().Equals (name)) {
			return player7;
		} else if (player8.GetComponent<Player> ().getNick ().Equals (name)) {
			return player8;
		} else if (you.GetComponent<Player> ().getNick ().Equals (name)) {
			return you;
		} else {
			return null;
		}
    }

	public GameObject GetYou(){
		return you;
	}

	public void clearCards ()
	{
		player0.GetComponent<Player> ().setCards("back", "back");
		player1.GetComponent<Player> ().setCards("back", "back");
		player2.GetComponent<Player> ().setCards("back", "back");
		player3.GetComponent<Player> ().setCards("back", "back");
		player4.GetComponent<Player> ().setCards("back", "back");
		player5.GetComponent<Player> ().setCards("back", "back");
		player6.GetComponent<Player> ().setCards("back", "back");
		player7.GetComponent<Player> ().setCards("back", "back");
		player8.GetComponent<Player> ().setCards("back", "back");
	}

    // Update is called once per frame
    void Update () {
	
	}
}


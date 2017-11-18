using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject Popup;
	public GameObject Canvas; 
	public GameObject Players;
	public GameObject Shared;
	public GameObject ConnectPanel;
	public GameObject ControlPanel;

	public GameObject InputIP;
	public GameObject InputName;

	public GameObject Status;
	public GameObject InputBet;
	public GameObject Bet;
	public GameObject Call;
	public GameObject Fold;
	public GameObject Allin;

	private String YourName;
	private StreamReader sr;
	private StreamWriter sw;

	private int bank;
	private int bet;
	private int yourbet;
	private int money;
	private int yourid;

	private Queue msgs = Queue.Synchronized(new Queue());

	public void Connect(){
		TcpClient client;
		YourName = InputName.GetComponent<InputField> ().text.Replace (" ", "_");
		if (YourName.Length == 0) {
			YourName = "Player";
		}
		try{
			String[] ip = InputIP.GetComponent<InputField> ().text.Split(':');
			client = new TcpClient(ip[0], Int32.Parse(ip[1]));
			Stream s = client.GetStream();
			sr = new StreamReader(s);
			sw = new StreamWriter(s);
			sw.AutoFlush = true;
			sw.WriteLine("READY " + YourName);

			new Thread(() => {
				while(client.Connected){
					String str = sr.ReadLine();
					msgs.Enqueue(str);
				}
			}).Start();

			ControlPanel.SetActive(true);
			ConnectPanel.SetActive(false);
		}catch(IndexOutOfRangeException){
			ShowPopup ("Wrong IP address. (IP_address:port)"); 
		}catch(Exception e){
			ShowPopup ("Cannot connect to server: " + e.Message);
		}
	}

	public void ShowPopup(String text){
		GameObject obj = Instantiate (Popup);
		RectTransform rt = obj.GetComponent<RectTransform> ();
		rt.offsetMax = new Vector2(Screen.width, Screen.height);
		obj.transform.localScale = new Vector3(1,1,1);
		obj.transform.SetParent(Canvas.transform);
		obj.GetComponentInChildren<Text> ().text = text;
		Debug.Log (text);
	}

	public void Bcall(){
		money -= bet - yourbet;
		yourbet = bet;
		sw.WriteLine ("BET " + bet);
		Status.GetComponent<Text>().text = "Bank: " + bank + "\nCurrent Bet: " + bet + "\nYour Bet: " + yourbet + "\nYour Money: " + money;
	}

	public void Bbet(){
		int b;
		try{
			b = Int32.Parse(InputBet.GetComponent<InputField>().text);
		}catch(Exception){
			ShowPopup ("This is not a correct value!");
			return;
		}
		if (b < bet) {
			ShowPopup ("Your bet it too small.");
		} else if (b > yourbet + money) {
			ShowPopup ("You don't have enough money.");
		} else {
			money = money + yourbet - b;
			yourbet = b;
			sw.WriteLine ("BET " + b);
		}
		Status.GetComponent<Text>().text = "Bank: " + bank + "\nCurrent Bet: " + bet + "\nYour Bet: " + yourbet + "\nYour Money: " + money;
	}

	public void Bfold(){
		sw.WriteLine ("FOLD");
		Status.GetComponent<Text>().text = "Bank: " + bank + "\nCurrent Bet: " + bet + "\nYour Bet: " + yourbet + "\nYour Money: " + money;
	}

	public void Ballin(){
		sw.WriteLine ("BET " + bet);
		yourbet += money;
		money = 0;
		Status.GetComponent<Text>().text = "Bank: " + bank + "\nCurrent Bet: " + bet + "\nYour Bet: " + yourbet + "\nYour Money: " + money;
	}

	// Use this for initialization 
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (sr != null) {
			try{
				if(msgs.Count > 0 && FindObjectOfType(typeof(PopupMark)) == null){
					String[] msg = ((String)msgs.Dequeue()).Split(' ');
					switch(msg[0]){
					case "START": 
						Stack players = new Stack();
						for(int i=2; i<msg.Length; i++){
							if(msg[i].Equals(YourName)){
								Players.GetComponent<Players>().GetYou().GetComponent<Player>().setNick(YourName);
								yourid = i - 2;
							}else{
								players.Push(msg[i]);
							}
						}
						Players.GetComponent<Players>().SetPlayers(players);
						break;
					case "ROUND": 
						bank = Int32.Parse(msg[1]);
						money = Int32.Parse(msg[yourid + 2]);
						Shared.GetComponent<Shared>().clearCards();
						Players.GetComponent<Players>().clearCards();
						break;
					case "DEALER": 
						ShowPopup(msg[1] + " is a dealer now.");
						break;
					case "SBLIND": 
						bet = Int32.Parse(msg[2]);
						ShowPopup(msg[1] + " bet small blind.");
						break;
					case "BBLIND": 
						bet = Int32.Parse(msg[2]);
						ShowPopup(msg[1] + " bet big blind.");
						break;
					case "CARDS": 
						Players.GetComponent<Players>().GetPlayer(YourName).GetComponent<Player>().setCards(msg[1], msg[2]);
						break;
					case "MOVE": 
						Players.GetComponent<Players>().setCurrent(msg[1]);
						if(msg[1].Equals(YourName)){
							if(yourbet + money > bet){
								Call.GetComponent<Button>().interactable = true;
								Bet.GetComponent<Button>().interactable = true;
								InputBet.GetComponent<InputField>().interactable = true;
							}else if(yourbet + money == bet){
								Bet.GetComponent<Button>().interactable = true;
								InputBet.GetComponent<InputField>().interactable = true;
							}
							Fold.GetComponent<Button>().interactable = true;
							Allin.GetComponent<Button>().interactable = true;
						}
						break;
					case "BET": 
						ShowPopup(msg[2] + " bets "+msg[1]+".");
						if(bet < Int32.Parse(msg[1])){
							bet = Int32.Parse(msg[1]);
						}
						if(msg[2].Equals(YourName)){
							Call.GetComponent<Button>().interactable = false;
							Bet.GetComponent<Button>().interactable = false;
							InputBet.GetComponent<InputField>().interactable = false;
							Fold.GetComponent<Button>().interactable = false;
							Allin.GetComponent<Button>().interactable = false;
						}
						break;
					case "FOLD": 
						ShowPopup(msg[1] + " folds.");
						break;
					case "CARD": 
						Shared.GetComponent<Shared>().addCard(msg[1]);
						break;
					case "WON": 
						ShowPopup(msg[1] + " won " + msg[2] + " because of " + msg[3]);
						break;
					case "ENDCARDS": 
						Players.GetComponent<Players>().GetPlayer(msg[1]).GetComponent<Player>().setCards(msg[2], msg[3]);
						break;
					}
					Status.GetComponent<Text>().text = "Bank: " + bank + "\nCurrent Bet: " + bet + "\nYour Bet: " + yourbet + "\nYour Money: " + money;
				}
			}catch(Exception e){
				ShowPopup ("Error: " + e.Message);
			}
		}
	}
}

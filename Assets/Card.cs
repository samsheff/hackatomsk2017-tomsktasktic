using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetCard(string name){
		if (name.Length == 2)
			name = name [0] + " " + name [1];
		GetComponent<Image>().sprite = Resources.Load<Sprite>("cards/"+name);

    }
}

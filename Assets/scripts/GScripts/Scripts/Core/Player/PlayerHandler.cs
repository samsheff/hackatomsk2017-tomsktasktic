using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public interface PlayerHandler : IEventSystemHandler
{
	void StartAction ();

	void EndOfAction ();

	void HandIsEvaluated ();

	void visualizeWinning ();

	List<Transform> chips ();

	void visualizeFold ();

	void ShowOff ();

	void NewGame ();

	void VisualizeChips (int chips);

	void getCard (Card card);

	void destroyCards ();

	void editChipsText (int newAmount);

}

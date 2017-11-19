using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public interface PlayersHandler : IEventSystemHandler
{

	void winnerProcess (PlayerImplementation playerImplementation);

	void AccumulateChips ();

	void UpdatePlayersList ();

	void showOff ();

	void NewGame ();
}

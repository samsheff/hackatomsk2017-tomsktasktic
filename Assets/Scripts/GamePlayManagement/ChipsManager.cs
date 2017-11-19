using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ChipsManager
{
	public ChipsManager (PlayersImplementation playersImplementation)
	{
		Chips = new Dictionary<string,int> ();
		this.playersImplementation = playersImplementation;
	}

	public void newTournament ()
	{
		int amountToStart = 100000;
		foreach (PlayerImplementation player in playersImplementation.players) {
			Chips.Add (player.Id, 100000);
			ExecuteEvents.Execute<PlayerHandler> (player.PlayerInstance.gameObject, null, (x, y) => x.editChipsText (amountToStart));
		}
	}

	public int Player (PlayerImplementation player)
	{
		return Chips [player.Id];
	}

	public void takeChips (PlayerImplementation player, int chips)
	{
		if (chips == 0)
			return;
		int hisChips = Chips [player.Id];
		hisChips -= chips;
		if (hisChips < 0)
			throw new UnityException ("chips manipulation");
		player.PlayerInstance.gameManager.soundManager.PlayChipsSound ();
		Chips [player.Id] = hisChips;
		ExecuteEvents.Execute<PlayerHandler> (player.PlayerInstance.gameObject, null, (x, y) => x.VisualizeChips (chips));
		ExecuteEvents.Execute<PlayerHandler> (player.PlayerInstance.gameObject, null, (x, y) => x.editChipsText (hisChips));
	}

	public void giveChips (PlayerImplementation player, int chips)
	{
		if (chips < 0)
			throw new UnityException ("chips manipulation");
		player.PlayerInstance.gameManager.destroyPotChips ();
		player.PlayerInstance.gameManager.soundManager.PlayChipsSound ();
		player.PlayerInstance.VisualizeChips (chips);
		int hisChips = Chips [player.Id];
		hisChips += chips;
		Chips [player.Id] = hisChips;
		ExecuteEvents.Execute<PlayerHandler> (player.PlayerInstance.gameObject, null, (x, y) => x.editChipsText (hisChips));
	}

	Dictionary<string,int> Chips;
	PlayersImplementation playersImplementation;
}

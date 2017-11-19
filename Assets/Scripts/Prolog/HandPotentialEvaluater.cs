using UnityEngine;
using System.Collections;

public class HandPotentialEvaluater
{

	public static float Evaluate (int rank, int straightFlag, int FlushFlag, GameManager.GameStatus state)
	{
		float HandPotential = 0;
		if (state.Equals (GameManager.GameStatus.flop)) {
			if (FlushFlag == 1)
				HandPotential += 7.5f * 0.35f;
			if (straightFlag == 2)
				HandPotential += 6 * 0.315f;
			if (straightFlag == 1)
				HandPotential += 6 * 0.165f;
			if (rank == 4) {
				HandPotential += 8 * 0.278f;
				HandPotential += 10 * 0.043f;
			} else if (rank == 3) {
				HandPotential += 10 * 0.165f;
			} else if (rank == 2) {
				HandPotential += 2 * 0.204f;
			}

		}
		if (state.Equals (GameManager.GameStatus.turn)) {
			if (FlushFlag == 1)
				HandPotential += 7.5f * 0.196f;
			if (straightFlag == 2)
				HandPotential += 6 * 0.174f;
			if (straightFlag == 1)
				HandPotential += 6 * 0.087f;
			if (rank == 4) {
				HandPotential += 8 * 0.152f;
				HandPotential += 10 * 0.022f;
			} else if (rank == 3) {
				HandPotential += 10 * 0.087f;
			} else if (rank == 2) {
				HandPotential += 2 * 0.109f;
			}
		}
		return HandPotential;
	}
}

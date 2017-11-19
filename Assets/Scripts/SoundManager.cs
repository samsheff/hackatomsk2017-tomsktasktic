using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		CardSound = GetComponents<AudioSource> () [0];
		ChipsSound = GetComponents<AudioSource> () [1];
	}

	public void PlayChipsSound ()
	{
		ChipsSound.Play ();
	}

	public void PlayCardSound ()
	{
		CardSound.Play ();
	}

	AudioSource ChipsSound;
	AudioSource CardSound;
}

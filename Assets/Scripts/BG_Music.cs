using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Music : MonoBehaviour {

	public AudioClip[] mainTheme;
	int index = 0;

	// Use this for initialization
	void Start ()
	{
		GetComponent<AudioSource> ().clip = mainTheme [index];
		GetComponent<AudioSource> ().Play ();

		index++;

		Invoke ("playNext", GetComponent<AudioSource> ().clip.length + 0.2f);
	}
	
	void playNext() 
	{
		GetComponent<AudioSource> ().Stop (); //just in case

		if (index > Constants.lastClip)
			index = Constants.beginClip;

	    GetComponent<AudioSource> ().clip = mainTheme [index];
	    GetComponent<AudioSource> ().Play ();

		index++;

		Invoke ("playNext", GetComponent<AudioSource> ().clip.length + 0.2f);
			
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour {

	public AudioClip[] mainTheme;
	public AudioClip battleTheme,
	nearWinTheme,
	nearLossTheme,
	victoryTheme,
	lossTheme,
	beginWar,
	placeArmy,
	battleSound,
	lumberSound,
	woolSound,
	brickSound,
	oreSound,
	grainSound,
	winSound,
	loseSound,
	buySound,
	sellSound;
	AudioSource audiosource;
	GameObject ParentAudio;
	GameObject soundFX;
	GameObject conditionalThemes;
	int index = 0;


	// Use this for initialization
	void Start ()
	{
		ParentAudio = this.gameObject;
		soundFX = transform.GetChild(0).gameObject;
		conditionalThemes = transform.GetChild (1).gameObject;

		audiosource = GetComponent<AudioSource> ();
		audiosource.clip = mainTheme [index];
		audiosource.Play ();

		index++;

		Invoke ("playNext", audiosource.clip.length); 	
	}

	void playNext() 
	{
		audiosource = GetComponent<AudioSource> ();
		audiosource.Stop (); //just in case

		if (index > Constants.lastClip)
			index = Constants.beginClip;

		audiosource.clip = mainTheme [index];
		audiosource.Play ();

		index++;

		Invoke ("playNext", audiosource.clip.length);

	}

	void fadeAudioOut(GameObject gameAudioOut)
	{
		audiosource = gameAudioOut.GetComponent<AudioSource> ();
		float fadeTimeOut = 0.1f;
		//int volume = 5;
		float fadeVolumeOut;

		for (int volume = 5; volume > 0; volume--)
		{
			fadeVolumeOut = volume / 10;
			audiosource.volume = fadeVolumeOut;
			AudioWait (fadeTimeOut);
		}
	}

	void fadeAudioIn(GameObject gameAudioIn)
	{
		audiosource = gameAudioIn.GetComponent<AudioSource> ();
		float fadeTimeIn = 0.1f;
		float fadeVolumeIn;

		for (int volume = 1; volume < 10; volume++)
		{
			fadeVolumeIn = volume / 10;
			audiosource.volume = fadeVolumeIn;
			AudioWait (fadeTimeIn);
		}
	}

	IEnumerator AudioWait(float audioTime)
	{
		yield return new WaitForSeconds (audioTime);
	}

	public void playBeginGame()
	{
		//soundFX = transform.GetChild (0).gameObject;
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = beginWar;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playMainTheme()
	{
		fadeAudioOut (conditionalThemes);
		fadeAudioIn (ParentAudio);
	}

	public void playBattleMusic()
	{
		//conditionalThemes = transform.GetChild (1).gameObject;
		fadeAudioOut (ParentAudio);
		audiosource = conditionalThemes.GetComponent<AudioSource> ();
		audiosource.clip = battleTheme;
		audiosource.Play ();
	}

	public void playNearLoss()
	{
		fadeAudioOut (ParentAudio);
		audiosource = conditionalThemes.GetComponent<AudioSource> ();
		audiosource.clip = nearLossTheme;
		audiosource.Play ();
	}

	public void playNearWin()
	{
		fadeAudioOut (ParentAudio);
		audiosource = conditionalThemes.GetComponent<AudioSource> ();
		audiosource.clip = nearWinTheme;
		audiosource.Play ();
	}

	public void playLossTheme()
	{
		fadeAudioOut (ParentAudio);
		audiosource = conditionalThemes.GetComponent<AudioSource> ();
		audiosource.clip = lossTheme;
		audiosource.Play ();
	}

	public void playWinTheme()
	{
		fadeAudioOut (ParentAudio);
		audiosource = conditionalThemes.GetComponent<AudioSource> ();
		audiosource.clip = victoryTheme;
		audiosource.Play ();
	}

	public void playBuySound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = buySound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playSellSound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = sellSound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playBattleSound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = battleSound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playPlaceArmy()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = placeArmy;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playLumberSound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = lumberSound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playWoolSound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = woolSound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playBrickSound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = brickSound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playOreSound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = oreSound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playGrainSound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = grainSound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playWinSound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = winSound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

	public void playLoseSound()
	{
		audiosource = soundFX.GetComponent<AudioSource> ();
		audiosource.clip = loseSound;
		audiosource.Play ();
		AudioWait (audiosource.clip.length);
		audiosource.Stop ();
	}

}
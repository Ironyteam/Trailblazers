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
	AudioSource audio;
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

		audio = GetComponent<AudioSource> ();
		audio.clip = mainTheme [index];
		audio.Play ();

		index++;

		Invoke ("playNext", audio.clip.length); 	
	}

	void playNext() 
	{
		audio = GetComponent<AudioSource> ();
		audio.Stop (); //just in case

		if (index > Constants.lastClip)
			index = Constants.beginClip;

		audio.clip = mainTheme [index];
		audio.Play ();

		index++;

		Invoke ("playNext", audio.clip.length);

	}

	void fadeAudioOut(GameObject gameAudioOut)
	{
		audio = gameAudioOut.GetComponent<AudioSource> ();
		float fadeTimeOut = 0.1f;
		//int volume = 5;
		float fadeVolumeOut;

		for (int volume = 5; volume > 0; volume--)
		{
			fadeVolumeOut = volume / 10;
			audio.volume = fadeVolumeOut;
			AudioWait (fadeTimeOut);
		}
	}

	void fadeAudioIn(GameObject gameAudioIn)
	{
		audio = gameAudioIn.GetComponent<AudioSource> ();
		float fadeTimeIn = 0.1f;
		float fadeVolumeIn;

		for (int volume = 1; volume < 10; volume++)
		{
			fadeVolumeIn = volume / 10;
			audio.volume = fadeVolumeIn;
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
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = beginWar;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
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
		audio = conditionalThemes.GetComponent<AudioSource> ();
		audio.clip = battleTheme;
		audio.Play ();
	}

	public void playNearLoss()
	{
		fadeAudioOut (ParentAudio);
		audio = conditionalThemes.GetComponent<AudioSource> ();
		audio.clip = nearLossTheme;
		audio.Play ();
	}

	public void playNearWin()
	{
		fadeAudioOut (ParentAudio);
		audio = conditionalThemes.GetComponent<AudioSource> ();
		audio.clip = nearWinTheme;
		audio.Play ();
	}

	public void playLossTheme()
	{
		fadeAudioOut (ParentAudio);
		audio = conditionalThemes.GetComponent<AudioSource> ();
		audio.clip = lossTheme;
		audio.Play ();
	}

	public void playWinTheme()
	{
		fadeAudioOut (ParentAudio);
		audio = conditionalThemes.GetComponent<AudioSource> ();
		audio.clip = victoryTheme;
		audio.Play ();
	}

	public void playBuySound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = buySound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playSellSound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = sellSound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playBattleSound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = battleSound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playPlaceArmy()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = placeArmy;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playLumberSound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = lumberSound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playWoolSound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = woolSound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playBrickSound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = brickSound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playOreSound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = oreSound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playGrainSound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = grainSound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playWinSound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = winSound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

	public void playLoseSound()
	{
		audio = soundFX.GetComponent<AudioSource> ();
		audio.clip = loseSound;
		audio.Play ();
		AudioWait (audio.clip.length);
		audio.Stop ();
	}

}
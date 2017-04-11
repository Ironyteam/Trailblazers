using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class optionsMenu : MonoBehaviour {

	public Slider masterVolumeControl;
	public Slider musicVolumeControl;
	public Slider SFXvolumeControl;
	public Switch soundSwtich;
	public AudioSource musicSounds = new AudioSource();
	public AudioSource conditionalThemes = new AudioSource();
	public AudioSource SFXsounds = new AudioSource();

	void Start()
	{
		musicSounds = GameObject.Find("MUSIC").GetComponent<AudioSource>();
		conditionalThemes = GameObject.Find("Conditional Themes").GetComponent<AudioSource>();
		SFXsounds = GameObject.Find("SoundFX").GetComponent<AudioSource>();
	}
	
	// Changes sound settings
	public void masterVolumeChange()
	{
    	AudioListener.volume = masterVolumeControl.value;
    }

	public void musicVolumeChange()
	{
		musicSounds.volume = musicVolumeControl.value;
		conditionalThemes.volume = musicVolumeControl.value;
	}

	public void soundFXChange()
	{
		SFXsounds.volume = SFXvolumeControl.value;
	}
	
	public void soundToggle()
	{
		if(soundSwtich.isOn == false)
			AudioListener.pause = true;
		else
			AudioListener.pause = false;
	}
}
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class optionsMenu : MonoBehaviour {

	public Slider masterVolumeControl;
	public Slider musicVolumeControl;
	public Switch soundSwtich;
	public AudioSource sounds = new AudioSource();

	// Changes sound settings
	public void masterVolumeChange()
	{
    	AudioListener.volume = masterVolumeControl.value;
    }

	public void musicVolumeChange()
	{
		sounds.volume = musicVolumeControl.value;
	}

	public void soundToggle()
	{
		if(soundSwtich.isOn == false)
			AudioListener.pause = true;
		else
			AudioListener.pause = false;
	}
}
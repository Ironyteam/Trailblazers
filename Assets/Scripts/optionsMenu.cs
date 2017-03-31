using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class optionsMenu : MonoBehaviour {

    public Dropdown qualitySetting;
	public Dropdown resolutionSetting;
	public Slider volumeControl;
	public AudioSource sounds = new AudioSource();

	// Changes quality settings
    public void qualityChange()
	{
		switch(qualitySetting.value)
		{
		    case 0:
                QualitySettings.SetQualityLevel(5, true);
			    break;
			case 1:
			    QualitySettings.SetQualityLevel(3, true);
			    break;
			case 2:
			    QualitySettings.SetQualityLevel(0, true);
			    break;
		}
    }

	// Changes resolution settings
	public void resolutionChange()
	{
		switch(resolutionSetting.value)
		{
			case 0:
				Screen.SetResolution(1680, 1050, true);
			    break;
			case 1:
				Screen.SetResolution(1600, 900, true);
			    break;
			case 2:
				Screen.SetResolution(1440, 900, true);
				break;
			case 3:
				Screen.SetResolution(1400, 1050, true);
			    break;
			case 4:
				Screen.SetResolution(1366, 768, true);
				break;
			case 5:
				Screen.SetResolution(1360, 768, true);
				break;
			case 6:
				Screen.SetResolution(1280, 1024, true);
				break;
			case 7:
				Screen.SetResolution(1280, 960, true);
				break;
			case 8:
				Screen.SetResolution(1280, 800, true);
				break;
			case 9:
				Screen.SetResolution(1280, 768, true);
				break;
			case 10:
				Screen.SetResolution(1280, 720, true);
				break;
			case 11:
				Screen.SetResolution(1280, 600, true);
				break;
			case 12:
				Screen.SetResolution(1152, 864, true);
				break;
			case 13:
				Screen.SetResolution(1024, 768, true);
				break;
			case 14:
				Screen.SetResolution(800, 600, true);
				break;
			case 15:
				Screen.SetResolution(640, 480, true);
				break;
			case 16:
				Screen.SetResolution(640, 400, true);
				break;
			case 17:
				Screen.SetResolution(512, 384, true);
				break;
			case 18:
			    Screen.SetResolution(400, 300, true);
				break;
			case 19:
			    Screen.SetResolution(320, 240, true);
				break;
			case 20:
			    Screen.SetResolution(320, 200, true);
				break;
	    }
         
	}

	// Changes sound settings
	public void soundChange()
	{
    	AudioListener.volume = volumeControl.value;
    }

	public void soundToggle()
	{

	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class audioManager : MonoBehaviour {

	private GameObject ParentAudio,
		               soundFX,
		               conditionalThemes;
	public AudioClip[] mainTheme;
	public AudioClip titleTheme,
	                 waveSFX,
	                 battleTheme,
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
	private AudioSource mainAudio,
	                    effectsAudio,
	                    conditionalAudio;
	int   index         = 0;
	float fadeVolumeOut = 0.2f,
	      fadeVolumeIn  = 0.2f,
	      fadeTimeOut   = 0.1f,
	      fadeTimeIn    = 0.1f;

	// Use this for initialization
	void Start ()
	{
		ParentAudio       = this.gameObject;
		soundFX           = transform.GetChild (0).gameObject;
		conditionalThemes = transform.GetChild (1).gameObject;

		mainAudio        = ParentAudio.GetComponent<AudioSource> ();
		effectsAudio     = soundFX.GetComponent<AudioSource> ();
		conditionalAudio = conditionalThemes.GetComponent<AudioSource> ();

		playTitleTheme ();
	}

	private void OnEnable ()
	{
		SceneManager.sceneLoaded += sceneChange;
	}

	private void OnDisable ()
	{
		SceneManager.sceneLoaded -= sceneChange;
	}

	private void sceneChange (Scene currentScene, LoadSceneMode mode)
	{
		if (currentScene.name == "In Game Scene") 
			StartCoroutine(playInGameTheme ());

		if (currentScene.name == "Menu")
			playTitleTheme ();
	}

	void Awake()
	{
		ParentAudio = GameObject.Find ("MUSIC");
		if (ParentAudio == null)
        {
                //If this object does not exist then it does the following:
                //1. Sets the object this script is attached to as the music player
                ParentAudio       = this.gameObject;
			    soundFX           = transform.GetChild (0).gameObject;
                conditionalThemes = transform.GetChild (1).gameObject;
				//2. Renames THIS object to "MUSIC" for next time
				ParentAudio.name = "MUSIC";
				//3. Tells THIS object not to die when changing scenes.
				DontDestroyOnLoad (ParentAudio);
				DontDestroyOnLoad (soundFX);
			    DontDestroyOnLoad (conditionalThemes);
        } 
        else
        {
            if (this.gameObject.name != "MUSIC")
            {
                //If there WAS an object in the scene called "MUSIC" (because we have come back to
                //the scene where the music was started) then it just tells this object to 
                //destroy itself if this is not the original
                Destroy (this.gameObject);
                Destroy (transform.GetChild (0).gameObject);
                Destroy (transform.GetChild (1).gameObject);
            }
        }
    }

	private void playTitleTheme ()
	{
		if (mainAudio.clip != titleTheme) 
		{
			mainAudio.Stop ();
			conditionalAudio.Stop ();
			effectsAudio.Stop ();
            mainAudio.clip = titleTheme;
            conditionalAudio.clip   = waveSFX;
            conditionalAudio.volume = 0.3f;
            mainAudio.Play ();
            conditionalAudio.Play ();
		}
	}

	private IEnumerator playInGameTheme ()
	{
		while (mainAudio.volume > 0f)
		{
			mainAudio.volume -= fadeVolumeOut;
			yield return new WaitForSeconds(fadeTimeOut);
		}

		conditionalAudio.Stop ();
		mainAudio.Stop ();
		mainAudio.clip = mainTheme [index];
		mainAudio.Play ();

		while (mainAudio.volume < 1f)
		{
			mainAudio.volume += fadeVolumeIn;
			yield return new WaitForSeconds(fadeTimeIn);
		}

		conditionalAudio.volume = 1f;

		index++;

		Invoke ("playNext", mainAudio.clip.length); 	
	}

	private void playNext() 
	{
		mainAudio.Stop (); //just in case

		if (index > Constants.lastClip)
			index = Constants.beginClip;

		mainAudio.clip = mainTheme [index];
		mainAudio.Play ();

		index++;

		Invoke ("playNext", mainAudio.clip.length);

	}

	public void playBeginGame()
	{
		effectsAudio.clip = beginWar;
		effectsAudio.Play ();
	}

	public IEnumerator playMainTheme()
	{
		while (conditionalAudio.volume > 0f)
		{
			conditionalAudio.volume -= fadeVolumeOut;
			yield return new WaitForSeconds(fadeTimeOut);
		}

		conditionalAudio.Stop ();

		while (mainAudio.volume < 1f)
		{
			mainAudio.volume += fadeVolumeIn;
			yield return new WaitForSeconds(fadeTimeIn);
		}
	}

	public IEnumerator playBattleMusic()
	{
		while (mainAudio.volume > 0f)
		{
			mainAudio.volume -= fadeVolumeOut;
			yield return new WaitForSeconds(fadeTimeOut);
		}

		conditionalAudio.clip = battleTheme;
		conditionalAudio.Play ();
	}

	public IEnumerator playNearLoss()
	{
		while (mainAudio.volume > 0f)
		{
			mainAudio.volume -= fadeVolumeOut;
			yield return new WaitForSeconds(fadeTimeOut);
		}

		conditionalAudio.clip = nearLossTheme;
		conditionalAudio.Play ();
	}

	public IEnumerator playNearWin()
	{
		while (mainAudio.volume > 0f)
		{
			mainAudio.volume -= fadeVolumeOut;
			yield return new WaitForSeconds(fadeTimeOut);
		}

		conditionalAudio.clip = nearWinTheme;
		conditionalAudio.Play ();
	}

	private void playLossTheme()
	{
		mainAudio.Stop ();
		conditionalAudio.clip = lossTheme;
		conditionalAudio.Play ();
	}

	private void playWinTheme()
	{
		mainAudio.Stop ();
		conditionalAudio.clip = victoryTheme;
		conditionalAudio.Play ();
	}

	public void playBuySound()
	{
		effectsAudio.clip = buySound;
		effectsAudio.Play ();
	}

	public void playSellSound()
	{
		effectsAudio.clip = sellSound;
		effectsAudio.Play ();
	}

	public void playBattleSound()
	{
		effectsAudio.clip = battleSound;
		effectsAudio.Play ();
	}

	public void playPlaceArmy()
	{
		effectsAudio.clip = placeArmy;
		effectsAudio.Play ();
	}

	public void playLumberSound()
	{
		effectsAudio.clip = lumberSound;
		effectsAudio.Play ();
	}

	public void playWoolSound()
	{
		effectsAudio.clip = woolSound;
		effectsAudio.Play ();
	}

	public void playBrickSound()
	{
		effectsAudio.clip = brickSound;
		effectsAudio.Play ();
	}

	public void playOreSound()
	{
		effectsAudio.clip = oreSound;
		effectsAudio.Play ();
	}

	public void playGrainSound()
	{
		effectsAudio.clip = grainSound;
		effectsAudio.Play ();
	}

	public IEnumerator playWinSound()
	{
        while (mainAudio.volume > 0f)
        {
            mainAudio.volume -= fadeVolumeOut;
            yield return new WaitForSeconds(fadeTimeOut);
        }

        effectsAudio.clip = winSound;
		effectsAudio.Play ();

        yield return new WaitForSeconds(effectsAudio.clip.length);
        playWinTheme();
	}

	public IEnumerator playLoseSound()
	{
        while (mainAudio.volume > 0f)
        {
            mainAudio.volume -= fadeVolumeOut;
            yield return new WaitForSeconds(fadeTimeOut);
        }

        effectsAudio.clip = loseSound;
		effectsAudio.Play ();

        yield return new WaitForSeconds(effectsAudio.clip.length);
        playLossTheme();
	}
}
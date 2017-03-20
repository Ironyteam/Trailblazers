using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class musicPlayer : MonoBehaviour {

	static bool AudioBegin = true;
	public GameObject audioPlayer;
	//public AudioClip beginGame;

	void Update()
	{
		Scene currentScene = SceneManager.GetActiveScene ();
		AudioSource audio = GetComponent<AudioSource> ();

		string sceneName = currentScene.name;

		if (sceneName == "hexboard") 
		{
			//audio.volume = 0.5f;
			//audio.clip = beginGame;
			//audio.Play();
			//AudioWait (audio.clip.length);
			audio.Stop ();
			AudioBegin = false;
		}



	}

	void Awake()
	{
		if (AudioBegin) {
			//When the scene loads it checks if there is an object called "MUSIC".
			audioPlayer = GameObject.Find ("MUSIC");
			if (audioPlayer == null) {
				//If this object does not exist then it does the following:
				//1. Sets the object this script is attached to as the music player
				audioPlayer = this.gameObject;
				//2. Renames THIS object to "MUSIC" for next time
				audioPlayer.name = "MUSIC";
				//3. Tells THIS object not to die when changing scenes.
				DontDestroyOnLoad (audioPlayer);
			} else {
				if (this.gameObject.name != "MUSIC") {
					//If there WAS an object in the scene called "MUSIC" (because we have come back to
					//the scene where the music was started) then it just tells this object to 
					//destroy itself if this is not the original
					Destroy (this.gameObject);
				}
			}
		}
	}

	//IEnumerator AudioWait(float audioTime)
	//{
	//	yield return new WaitForSeconds (audioTime);
	//}
}

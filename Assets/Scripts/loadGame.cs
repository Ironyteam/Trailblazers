using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loadGame : MonoBehaviour {

	AsyncOperation async;
	public Canvas loadingCanvas;
	public Canvas main;
	//public Canvas options;
	//public Canvas create;
	//public Canvas quit;
	//public Canvas credit;
	//public GameObject loadingScreenBG;
	public Slider loadBar;
	public Text loadText;

	//public bool isFakeLoadingBar = false;

	public void loadBoard()
	{

		main.enabled = false;

		loadingCanvas.enabled = true;
		loadBar.enabled = true;
		loadText.enabled = true;
		//loadText.text = "Loading...";

		StartCoroutine (loadProgress());
	}

	IEnumerator loadProgress()
	{
		yield return new WaitForSeconds (1);
		async = SceneManager.LoadSceneAsync (1);
		async.allowSceneActivation = false;

		while (!async.isDone)
		{
			loadBar.value = async.progress;
			loadText.text = "Loading... " + ((int)(async.progress * 100)).ToString() + "%"; 
			
			if (async.progress == 0.9f)
			{
				loadText.text = "Press 'F' To Continue";
				if (Input.GetKeyDown (KeyCode.F))
				{
					async.allowSceneActivation = true;
				}
			}

			Debug.Log ((int)(async.progress * 10));
			yield return null;
		}

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loadgameSpin : MonoBehaviour {

	AsyncOperation async;
	public Canvas loadingCanvas;
	public Canvas main;
	public Image spinner;
	public Text loadText;
	RectTransform spinnerTransform;
	int spinCounter = 0;


	private void Start()
	{
		loadingCanvas.enabled = false;
		spinnerTransform = spinner.GetComponent<RectTransform>();
	}

	private void Update()
	{
		spinCounter++;
		if (spinCounter % 15 == 0)
			spinnerTransform.Rotate(new Vector3(0, 0, -36));
	}

	public void loadBoard()
	{

		main.enabled = false;

		loadingCanvas.enabled = true;

		StartCoroutine (loadWithSpin ());
	}

	IEnumerator loadWithSpin()
	{
		characterSelect.startGame();
		yield return new WaitForSeconds (1);
		async = SceneManager.LoadSceneAsync ("In Game Scene");
	}
	
	public IEnumerator networkLoad()
	{
		characterSelect.startGame();
		yield return new WaitForSeconds (1);
		async = SceneManager.LoadSceneAsync ("In Game Scene");
		async.allowSceneActivation = false;

		while (!async.isDone && !async.allowSceneActivation)
		{
			yield return null;
		}
	}
}

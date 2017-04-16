using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class loadMainMenu : MonoBehaviour {

	AsyncOperation async;
	public Image spinner;
	RectTransform spinnerTransform;
	int spinCounter = 0;


	private void Start()
	{
		spinnerTransform = spinner.GetComponent<RectTransform>();
	}

	private void FixedUpdate()
	{
		spinCounter++;
		if (spinCounter % 5 == 0)
			spinnerTransform.Rotate(new Vector3(0, 0, -36));
	}

	public void loadIntoGame()
	{
		StartCoroutine (loadMenuSpin ());
	}

	IEnumerator loadMenuSpin()
	{
		characterSelect.startGame();
		yield return new WaitForSeconds (1);
		async = SceneManager.LoadSceneAsync ("Menu");

		yield return null;
	}
}

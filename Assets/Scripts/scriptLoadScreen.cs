using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class scriptLoadScreen : MonoBehaviour {

	// Use this for initialization
   void Start()
   {
       UnityEngine.SceneManagement.SceneManager.LoadScene(1);
   }
	
	// Update is called once per frame
	void Update () {
		
	}
}

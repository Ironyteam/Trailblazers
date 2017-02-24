using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class NavigationScript : MonoBehaviour
{
    public Canvas createCanvas;
    public Canvas creditCanvas;
    public Canvas mainCanvas;
    public Canvas optionsCanvas;
    public Canvas quitCanvas;
	public Canvas playNowCanvas;

    void Awake()
    {
       optionsCanvas.enabled = false;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
    }

    public void optionsOn()
    {
       optionsCanvas.enabled = true;
       mainCanvas.enabled    = false;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
    }

	public void playNow()
	{
		optionsCanvas.enabled = false;
		mainCanvas.enabled    = false;
		createCanvas.enabled  = false;
		creditCanvas.enabled  = false;
		quitCanvas.enabled    = false;
		playNowCanvas.enabled = true;
	}

    public void createOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = false;
       createCanvas.enabled  = true;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
    }

    public void creditOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = false;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = true;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
    }

    public void returnOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = true;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
    }

    public void quitOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = false;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = true;
	   playNowCanvas.enabled = false;
    }

    public void exitGame()
    {
       Application.Quit();
    } 

    public void loadOn()
    {
 	  UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
    
    public void netLobbyOn()
    {
	   UnityEngine.SceneManagement.SceneManager.LoadScene(3);
    }
    
	public void PreGameBoardSceneOn()
    {
        BoardManager.startingGame = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(7);
    }
	
	public void characterSelectOn()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(5);
	}
	
	public void boardManagerOn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(7);
    }
}
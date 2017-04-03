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
	public Canvas gameRulesCanvas;
	public static bool networkGame = false;

    void Awake()
    {
       optionsCanvas.enabled   = false;
       createCanvas.enabled    = false;
       creditCanvas.enabled    = false;
       quitCanvas.enabled      = false;
	   playNowCanvas.enabled   = false;
	   gameRulesCanvas.enabled = false;
    }

	void Update()
	{
		if(Input.GetKeyDown("escape"))
		{
			if(mainCanvas.enabled == true)
				quitOn();
			else
				returnOn();
		}
	}

    public void optionsOn()
    {
       optionsCanvas.enabled = true;
       mainCanvas.enabled    = false;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
	   gameRulesCanvas.enabled = false;
    }

	public void playNow()
	{
		optionsCanvas.enabled = false;
		mainCanvas.enabled    = false;
		createCanvas.enabled  = false;
		creditCanvas.enabled  = false;
		quitCanvas.enabled    = false;
	    gameRulesCanvas.enabled = false;
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
	   gameRulesCanvas.enabled = false;
    }

    public void creditOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = false;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = true;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
	   gameRulesCanvas.enabled = false;
    }

    public void returnOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = true;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
	   gameRulesCanvas.enabled = false;
    }

    public void quitOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = false;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = true;
	   playNowCanvas.enabled = false;
	   gameRulesCanvas.enabled = false;
    }

	public void rulesOn()
	{
	   optionsCanvas.enabled = false;
       mainCanvas.enabled    = false;
       createCanvas.enabled  = false;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
	   gameRulesCanvas.enabled = true;
	}

    public void exitGame()
    {
       Application.Quit();
    } 
    
    public void netLobbyOn()
    {
		 networkGame = true;
	    UnityEngine.SceneManagement.SceneManager.LoadScene("Network lobby");
    }
    
	public void PreGameBoardSceneOn()
    {
        BoardManager.startingGame = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("BoardManager");
    }
	
	public void characterSelectOn()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Character Select");
	}
	
	public void boardManagerOn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BoardManager");
    }

	public void setLocal()
	{
		networkGame = false;
		PreGameBoardSceneOn();
	}
}
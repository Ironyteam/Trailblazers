using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class NavigationScript : MonoBehaviour
{
    public Canvas creditCanvas;
    public Canvas mainCanvas;
    public Canvas optionsCanvas;
    public Canvas quitCanvas;
	public Canvas playNowCanvas;
	public Canvas gameRulesCanvas;
	public static bool networkGame = false;

    public Button nextPage;
    public Button previousPage;

    public Text bannerOneFirst;
    public Text bannerTwoFirst;
    public Text bannerOneSecond;
    public Text bannerTwoSecond;

    public Text textOneFirst;
    public Text textTwoFirst;
    public Text textOneSecond;
    public Text textTwoSecond;

    void Awake()
    {
        optionsCanvas.enabled   = false;
        creditCanvas.enabled    = false;
        quitCanvas.enabled      = false;
	    playNowCanvas.enabled   = false;
	    gameRulesCanvas.enabled = false;

        nextPage.gameObject.SetActive(true);
        previousPage.gameObject.SetActive(false);

        bannerOneFirst.enabled = true;
        bannerTwoFirst.enabled = true;
        textOneFirst.enabled = true;
        textTwoFirst.enabled = true;

        bannerOneSecond.enabled = false;
        bannerTwoSecond.enabled = false;
        textOneSecond.enabled = false;
        textTwoSecond.enabled = false;
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
;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
	   gameRulesCanvas.enabled = false;
    }

	public void playNow()
	{
		optionsCanvas.enabled = false;
		mainCanvas.enabled    = false;
		creditCanvas.enabled  = false;
		quitCanvas.enabled    = false;
	    gameRulesCanvas.enabled = false;
		playNowCanvas.enabled = true;
	}

    public void creditOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = false;
;
       creditCanvas.enabled  = true;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
	   gameRulesCanvas.enabled = false;
    }

    public void returnOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = true;
;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = false;
	   playNowCanvas.enabled = false;
	   gameRulesCanvas.enabled = false;
    }

    public void quitOn()
    {
       optionsCanvas.enabled = false;
       mainCanvas.enabled    = false;
;
       creditCanvas.enabled  = false;
       quitCanvas.enabled    = true;
	   playNowCanvas.enabled = false;
	   gameRulesCanvas.enabled = false;
    }

	public void rulesOn()
	{
	   optionsCanvas.enabled = false;
       mainCanvas.enabled    = false;
;
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

    public void NextPageRules()
    {
        nextPage.gameObject.SetActive(false);
        previousPage.gameObject.SetActive(true);

        bannerOneFirst.enabled = false;
        bannerTwoFirst.enabled = false;
        textOneFirst.enabled = false;
        textTwoFirst.enabled = false;

        bannerOneSecond.enabled = true;
        bannerTwoSecond.enabled = true;
        textOneSecond.enabled = true;
        textTwoSecond.enabled = true;
    }

    public void PreviousPageRules()
    {
        nextPage.gameObject.SetActive(true);
        previousPage.gameObject.SetActive(false);

        bannerOneFirst.enabled = true;
        bannerTwoFirst.enabled = true;
        textOneFirst.enabled = true;
        textTwoFirst.enabled = true;

        bannerOneSecond.enabled = false;
        bannerTwoSecond.enabled = false;
        textOneSecond.enabled = false;
        textTwoSecond.enabled = false;
    }
}
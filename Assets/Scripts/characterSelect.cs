using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterSelect : MonoBehaviour {

	public const int NUM_OF_CHARACTERS = 12;

	public Canvas[] characterCanvasArray = new Canvas[NUM_OF_CHARACTERS];
	public Image[] characterSelectImages = new Image[NUM_OF_CHARACTERS]; 

	void Start()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
			characterSelectImages[count].enabled = false;

		for(int count = 6; count < NUM_OF_CHARACTERS - 2; count++)
			characterCanvasArray[count].enabled = false;
	}

	public void startGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(2);
	}

	public void normalCharacterShow()
	{
		for(int count = 0; count < (NUM_OF_CHARACTERS * 0.5f); count++)
		{
			if(characterCanvasArray[count].enabled == true)
				characterCanvasArray[count].enabled = false;
		}

		for(int count = 6; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterCanvasArray[count].enabled == false)
				characterCanvasArray[count].enabled = true;
		}
	}

	public void specialCharacterShow()
	{
		for(int count = 6; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterCanvasArray[count].enabled == true)
				characterCanvasArray[count].enabled = false;
		}

		for(int count = 0; count < (NUM_OF_CHARACTERS * 0.5f); count++)
		{
			if(characterCanvasArray[count].enabled == false)
				characterCanvasArray[count].enabled = true;
		}
	}

	public void showMinerCharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[4].enabled = true;
	}

	public void showBrickLayerCharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[2].enabled = true;
	}

	public void showFarmerCharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[3].enabled = true;
	}

	public void showShepherdCharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[1].enabled = true;
	}

	public void showLumberJackCharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[0].enabled = true;
	}

	public void showBankerCharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[5].enabled = true;
	}

	public void showNormalGuy1CharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[6].enabled = true;
	}

	public void showNormalGuy2CharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[7].enabled = true;
	}

	public void showNormalGuy3CharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[8].enabled = true;
	}

	public void showNormalGuy4CharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[9].enabled = true;
	}

	public void showNormalGuy5CharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[10].enabled = true;
	}

	public void showNormalGuy6CharacterSelection()
	{
		for(int count = 0; count < NUM_OF_CHARACTERS; count++)
		{
			if(characterSelectImages[count].enabled == true)
				characterSelectImages[count].enabled = false;
		}

		characterSelectImages[11].enabled = true;
	}

	public void returnToNetLobby()
	{
        Application.Quit();
	}
}

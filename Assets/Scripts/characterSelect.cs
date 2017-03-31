﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterSelect : MonoBehaviour {

	public const int NUM_OF_CHARACTERS   = 8;
    public const int MAX_PLAYERS         = 6;
	public int currentPicker             = 0;
    public int selectedCharacter         = 0;
    public Image selectedCharacterImage;
	public Canvas[] characterCanvasArray = new Canvas[NUM_OF_CHARACTERS];
    public string[] imageNames           = new string[NUM_OF_CHARACTERS] {"Natty Bumppo", "Scary Harry", "Ganzo",  "Gamly the Red", "Maiden of Dunshire", "Queen Apala", "Abiha the Exiled", "Rosa del Fuego"};
    public Image[] playerChoiceImages    = new Image[MAX_PLAYERS];
    public static string[] selectedCharacters = new string[MAX_PLAYERS] {"Natty Bumppo", "Ganzo", "Maiden of Dunshire",  "Queen Apala", "Gamly the Red", "Rosa del Fuego"};
	public Switch[] isAi = new Switch[MAX_PLAYERS];

    void Awake()
    {
        for(int count = 0; count < MAX_PLAYERS; count++)
            playerChoiceImages[count].enabled = false;
    }
    
	void Start()
	{        
        for(int count = 0; count < BoardManager.numOfPlayers; count++)
            playerChoiceImages[count].enabled = true;
	}

	public void startGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("In Game Scene");
	}

    public void showNattyBumppoCharacterSelection()
	{
        selectedCharacter = 0;
        selectedCharacterImage.sprite = Resources.Load<Sprite>(imageNames[selectedCharacter]) as Sprite;
	}
    
    public void showScaryHarryCharacterSelection()
	{
        selectedCharacter = 1;
        selectedCharacterImage.sprite = Resources.Load<Sprite>(imageNames[selectedCharacter]) as Sprite;
        
	}
    
	public void showGanzoCharacterSelection()
	{
        selectedCharacter = 2;
        selectedCharacterImage.sprite = Resources.Load<Sprite>(imageNames[selectedCharacter]) as Sprite;
	}

	public void showGamlyTheRedCharacterSelection()
	{
        selectedCharacter = 3;
        selectedCharacterImage.sprite = Resources.Load<Sprite>(imageNames[selectedCharacter]) as Sprite;
	}
    
    public void showMaidenOfDunshireCharacterSelection()
	{
        selectedCharacter = 4;
        selectedCharacterImage.sprite = Resources.Load<Sprite>(imageNames[selectedCharacter]) as Sprite;
	}


	public void showQueenApalaCharacterSelection()
	{
        selectedCharacter = 5;
        selectedCharacterImage.sprite = Resources.Load<Sprite>(imageNames[selectedCharacter]) as Sprite;
	}

	public void showAbihaTheExiledSelection()
	{
        selectedCharacter = 6;
        selectedCharacterImage.sprite = Resources.Load<Sprite>(imageNames[selectedCharacter]) as Sprite;
	}

	public void showRosaDelFuegoSelection()
	{
        selectedCharacter = 7;
        selectedCharacterImage.sprite = Resources.Load<Sprite>(imageNames[selectedCharacter]) as Sprite;
	}
    
    public void selectCharacter() 
    {
        playerChoiceImages[currentPicker].sprite = Resources.Load<Sprite>(imageNames[selectedCharacter]) as Sprite;
        
        if(currentPicker < BoardManager.numOfPlayers)
        {
            selectedCharacters[currentPicker] = imageNames[selectedCharacter];
			currentPicker++;
        }
    }
    
	public void returnToNetLobby()
	{
        Application.Quit();
	}
}

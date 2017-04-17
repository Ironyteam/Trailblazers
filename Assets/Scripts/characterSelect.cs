using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterSelect : MonoBehaviour
{

   public const int NUM_OF_CHARACTERS = 8;
   public const int MAX_PLAYERS = 6;
   public int currentPicker = 0; // The current player choosing a character.
   public int selectedCharacter = 0; // This is what shows the preview of the player.
   public Image selectedCharacterImage;      // The image preview of the character
   public Image[] playerChoiceImages = new Image[Constants.MaxPlayers]; // These are the images that have been selected.
   public static int[] selectedCharacters; // These are the indexes for the characters chosen. 
   public Switch[] isAi = new Switch[Constants.MaxPlayers]; // The AI switch for local play.
   public static bool[] isAiBool = new bool[Constants.MaxPlayers];
   public bool CanChoose = true;
   public Button SelectCharacterBTN;
   public NetworkManager NetworkObject;
   public Text characterAbilitiesText;

   void Awake()
   {   
        for(int count = 0; count < Constants.MaxPlayers; count++)
		{
			isAi[count].gameObject.SetActive(false);
            playerChoiceImages[count].enabled = false;
		}
   }

   void Start()
   {
      // Sets the number of characters chosen from BoardManager.
      selectedCharacters = new int[BoardManager.numOfPlayers];

      // Setup the gameobjects if in a network game
      SelectCharacterBTN = GameObject.Find("Select Character").GetComponent<Button>();
      if (NavigationScript.networkGame)
      {
         SelectCharacterBTN.gameObject.SetActive(false);
         NetworkObject = GameObject.Find("Network Handler").GetComponent<NetworkManager>();
      }

		selectedCharacters = new int[BoardManager.numOfPlayers];
		if(NavigationScript.networkGame == true)
		{
			for(int count = 0; count < BoardManager.numOfPlayers; count++)
			{
				playerChoiceImages[count].enabled = true;
				selectedCharacters[count] = -1;
			}
		}
		else
		{
			for(int count = 0; count < BoardManager.numOfPlayers; count++)
			{
				playerChoiceImages[count].enabled = true;
				selectedCharacters[count] = -1;
				isAi[count].gameObject.SetActive(true);
			}
		}
   }

   private void Update()
   {
      if (NavigationScript.networkGame)
      {
         if (currentPicker == NetworkObject.myPlayer.playerIndex)
         {
            CanChoose = true;
         }
         else
         {
            CanChoose = false;
         }
      }

      if (CanChoose)
      {
         SelectCharacterBTN.gameObject.SetActive(true);
      }
   }

   // After all players have chosen, proceed to the in-game scene. If a player has not been chosen, chooses a random unchosen character.
   public static void startGame()
   {
      // Checks if all players have chosen a player.
      for (int count = 0; count < BoardManager.numOfPlayers; count++)
      {
         // If player has not chosen a character one is randomly assigned from the list of unchosen characters.
         if (selectedCharacters[count] == -1)
         {
            int playerToChoose;
            do
            {
               playerToChoose = UnityEngine.Random.Range(0, 8);
            } while (Characters.PlayerChosen[playerToChoose] == true);

            selectedCharacters[count] = playerToChoose;
            Characters.PlayerChosen[playerToChoose] = true;
         }

      }

      // Resets all players to unchosen.
      Characters.ResetPlayers();
   }

   // This function shows the preview of the character.
   public void showNattyBumppoCharacterSelection()
   {
      selectedCharacter = Characters.Frontiersman;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[selectedCharacter]) as Sprite;
	  characterAbilitiesText.text = Characters.abilitiesText[selectedCharacter];
   }

   public void showGamlyTheRedCharacterSelection()
   {
      selectedCharacter = Characters.General;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[selectedCharacter]) as Sprite;
	  characterAbilitiesText.text = Characters.abilitiesText[selectedCharacter];
   }

   public void showScaryHarryCharacterSelection()
   {
      selectedCharacter = Characters.ConspiracyTheorist;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[selectedCharacter]) as Sprite;
	  characterAbilitiesText.text = Characters.abilitiesText[selectedCharacter];
   }

   public void showGanzoCharacterSelection()
   {
      selectedCharacter = Characters.Merchant;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[selectedCharacter]) as Sprite;
	  characterAbilitiesText.text = Characters.abilitiesText[selectedCharacter];
   }

   public void showQueenApalaCharacterSelection()
   {
      selectedCharacter = Characters.Queen;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[selectedCharacter]) as Sprite;
	  characterAbilitiesText.text = Characters.abilitiesText[selectedCharacter];
   }


   public void showRosaDelFuegoSelection()
   {
      selectedCharacter = Characters.Engineer;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[selectedCharacter]) as Sprite;
	  characterAbilitiesText.text = Characters.abilitiesText[selectedCharacter];
   }

   public void showAbihaTheExiledSelection()
   {
      selectedCharacter = Characters.Nomad;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[selectedCharacter]) as Sprite;
	  characterAbilitiesText.text = Characters.abilitiesText[selectedCharacter];
   }

   public void showMaidenOfDunshireCharacterSelection()
   {
      selectedCharacter = Characters.Knight;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[selectedCharacter]) as Sprite;
	  characterAbilitiesText.text = Characters.abilitiesText[selectedCharacter];
   }

   public void selectCharacter()
   {
      Debug.Log("Character Chosen = " + Characters.PlayerChosen[selectedCharacter]);
      if (currentPicker < BoardManager.numOfPlayers && Characters.PlayerChosen[selectedCharacter] != true)
      {
         playerChoiceImages[currentPicker].sprite = Resources.Load<Sprite>(Characters.Names[selectedCharacter]) as Sprite;
         selectedCharacters[currentPicker] = selectedCharacter;
         Characters.PlayerChosen[selectedCharacter] = true;
         
         if (NavigationScript.networkGame)
         {
            SelectCharacterBTN.gameObject.SetActive(false);
            CanChoose = false;
            GameObject.Find("Network Handler").GetComponent<NetworkManager>().sendCharacterSelect(selectedCharacter, currentPicker);
         }
         currentPicker++;
         
         if(currentPicker >= BoardManager.numOfPlayers)
            SelectCharacterBTN.interactable = false;
      }
   }

   public void returnToNetLobby()
   {
      // Resets all players to unchosen.
      Characters.ResetPlayers();
      Application.Quit();
   }
}

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
   public Canvas[] characterCanvasArray = new Canvas[NUM_OF_CHARACTERS]; // These are the character images that can be clicked and selected.
   public Image[] playerChoiceImages = new Image[MAX_PLAYERS]; // These are the images that have been selected.
   public static int[] selectedCharacters; // These are the indexes for the characters chosen.
   public Switch[] isAi = new Switch[MAX_PLAYERS]; // The AI switch for local play. 
   public bool CanChoose = true;
   public Button SelectCharacterBTN;
   public NetworkManager NetworkObject;

   void Awake()
   {
      // Disables all of the player choice iamges.
      for (int count = 0; count < MAX_PLAYERS; count++)
         playerChoiceImages[count].enabled = false;
   }

   void Start()
   {
      // Setup the gameobjects if in a network game
      SelectCharacterBTN = GameObject.Find("Select Character").GetComponent<Button>();
      if (NavigationScript.networkGame)
      {
         SelectCharacterBTN.gameObject.SetActive(false);
         NetworkObject = GameObject.Find("Network Handler").GetComponent<NetworkManager>();
      }


      // Sets the number of characters chosen from BoardManager.
      selectedCharacters = new int[BoardManager.numOfPlayers];

      // Enables as many player choice images as the BoadManager has set, and initializes the character choice to unchosen.
      for (int count = 0; count < BoardManager.numOfPlayers; count++)
      {
         playerChoiceImages[count].enabled = true;
         selectedCharacters[count] = -1;
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
   public void startGame()
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

      UnityEngine.SceneManagement.SceneManager.LoadScene("In Game Scene");
   }

   // This function shows the preview of the character.
   public void showNattyBumppoCharacterSelection()
   {
      selectedCharacter = Characters.Frontiersman;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[Characters.Frontiersman]) as Sprite;
   }

   public void showGamlyTheRedCharacterSelection()
   {
      selectedCharacter = Characters.General;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[Characters.General]) as Sprite;
   }

   public void showScaryHarryCharacterSelection()
   {
      selectedCharacter = Characters.ConspiracyTheorist;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[Characters.ConspiracyTheorist]) as Sprite;

   }

   public void showGanzoCharacterSelection()
   {
      selectedCharacter = Characters.Merchant;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[Characters.Merchant]) as Sprite;
   }

   public void showQueenApalaCharacterSelection()
   {
      selectedCharacter = Characters.Queen;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[Characters.Queen]) as Sprite;
   }


   public void showRosaDelFuegoSelection()
   {
      selectedCharacter = Characters.Engineer;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[Characters.Engineer]) as Sprite;
   }

   public void showAbihaTheExiledSelection()
   {
      selectedCharacter = Characters.Nomad;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[Characters.Nomad]) as Sprite;
   }

   public void showMaidenOfDunshireCharacterSelection()
   {
      selectedCharacter = Characters.Knight;
      selectedCharacterImage.sprite = Resources.Load<Sprite>(Characters.Names[Characters.Knight]) as Sprite;
   }

   public void selectCharacter()
   {
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
      }
   }

   public void returnToNetLobby()
   {
      // Resets all players to unchosen.
      Characters.ResetPlayers();
      Application.Quit();
   }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Text       mainHelpText;
    public GameObject leftUpPointTutrorialPanel;
    public Text       leftUpPointTutrorialText;
    public GameObject leftDownPointTutrorialPanel;
    public Text       leftDownPointTutrorialText;
    public GameObject rightDownPointTutrorialPanel;
    public Text       rightDownPointTutrorialText;
    public GameObject rightUpPointTutrorialPanel;
    public Text       rightUpPointTutrorialText;

    // Constants for different Map Editor tutorial steps box LOCations
    private readonly Vector3 SELECT_RESOURCE_LOC     = new Vector3(120, 635, 0);
    private readonly Vector3 SELECT_DICE_NUM_LOC     = new Vector3(380, 470, 0);
    private readonly Vector3 CLICK_ADD_PORT_LOC      = new Vector3(320, 125, 0);
    private readonly Vector3 SELECT_HEX_FOR_PORT_LOC = new Vector3(410, 470, 0);
    private readonly Vector3 ADD_PORT_LOC            = new Vector3(410, 470, 0);

    // Constants for different In Game tutorial steps box LOCations
    private readonly Vector3 ROLL_DICE_LOC  = new Vector3(100, 100, 0);
    private readonly Vector3 BUILD_ROAD_LOC = new Vector3(100, 100, 0);
    private readonly Vector3 EXPLAIN_RESOURCES_LOC = new Vector3(100, 100, 0);

    // Constant messages for Map Editor tutorial
    public const string SELECT_RESOURCE_MESSAGE     = "Select a resource a click on a hexagon to add it to the map.";
    public const string SELECT_DICE_NUM_MESSAGE     = "Select a dice number and click on the hexagon to add it.";
    public const string ADD_PORT_CLICKED_MESSAGE    = "Click here to add a port to the map.";
    public const string CHOOSE_HEX_FOR_PORT_MESSAGE = "Click a hexagon to add a port to.";
    public const string ADD_PORT_MESSAGE            = "Build a port on one of the available spots";

    // Constant messages for In Game tutorial
    public const string PLACE_INITIAL_SETTLEMENT_MESSAGE = "Click a highlighted place on the map to build a settlement.";
    public const string PLACE_INITIAL_ROAD_MESSAGE       = "Click a highlighted place on the map to build a road out from your settlement.";
    public const string ROLL_DICE_MESSAGE                = "Click the dice to see what resources your settlements have earned";
    public const string BUILD_ROAD_MESSAGE               = "Click here when you want to build a road";
    public const string EXPLAIN_RESOURCES_MESSAGE        = "A road can only be built when you have the number of resources corresponding with the cost";

    private int currentTutorialStep = 1;
    private bool dontShowAgain = false;

    void Awake()
    {
        if (mainHelpText != null)
            mainHelpText.enabled = false;
        if (leftUpPointTutrorialPanel != null)
            leftUpPointTutrorialPanel.gameObject.SetActive(false);
        if (leftUpPointTutrorialText != null)
            leftUpPointTutrorialText.enabled = false;
        if (rightUpPointTutrorialPanel != null)
            rightUpPointTutrorialPanel.gameObject.SetActive(false);
        if (rightUpPointTutrorialText != null)
            rightUpPointTutrorialText.enabled = false;
        if (leftDownPointTutrorialPanel != null)
            leftDownPointTutrorialPanel.gameObject.SetActive(false);
        if (mainHelpText != null)
            leftDownPointTutrorialText.enabled = false;
        if (rightDownPointTutrorialPanel != null)
            rightDownPointTutrorialPanel.gameObject.SetActive(false);
        if (rightDownPointTutrorialText != null)
            rightDownPointTutrorialText.enabled = false;
    }

    /*****************************/
    /* Map Editor Tutorial steps */
    /*****************************/

    public void showSelectResourceTutorialBox()
    {
        if (dontShowAgain == false)
        {
            moveAndShowTutorialBox(leftDownPointTutrorialPanel, leftDownPointTutrorialText, SELECT_RESOURCE_MESSAGE,
                                   SELECT_RESOURCE_LOC, rightDownPointTutrorialPanel, rightDownPointTutrorialText);
            currentTutorialStep += 1;
        }
    }

    public void showSelectDiceNumTutorialBox()
    {
        if (dontShowAgain == false && currentTutorialStep == 2)
        {
            moveAndShowTutorialBox(leftDownPointTutrorialPanel, leftDownPointTutrorialText, SELECT_DICE_NUM_MESSAGE,
                                   SELECT_DICE_NUM_LOC, rightDownPointTutrorialPanel, rightDownPointTutrorialText);
            currentTutorialStep += 1;
        }
    }

    public void showClickAddPortTutorialBox()
    {
        if (dontShowAgain == false && currentTutorialStep == 3)
        {
            moveAndShowTutorialBox(leftUpPointTutrorialPanel, leftUpPointTutrorialText, ADD_PORT_CLICKED_MESSAGE,
                                   CLICK_ADD_PORT_LOC, leftDownPointTutrorialPanel, leftDownPointTutrorialText);
            currentTutorialStep += 1;
        }
    }

    public void showSelectHexForPortTutorialBox()
    {
        if (dontShowAgain == false && currentTutorialStep == 4)
        {
            moveAndShowTutorialBox(rightDownPointTutrorialPanel, rightDownPointTutrorialText, CHOOSE_HEX_FOR_PORT_MESSAGE,
                                   SELECT_HEX_FOR_PORT_LOC, leftUpPointTutrorialPanel, leftUpPointTutrorialText);
            currentTutorialStep += 1;
        }
    }

    public void showAddPortTutorialBox()
    {
        if (dontShowAgain == false && currentTutorialStep == 5)
        {
            moveAndShowTutorialBox(rightDownPointTutrorialPanel, rightDownPointTutrorialText, ADD_PORT_MESSAGE,
                                   ADD_PORT_LOC, leftDownPointTutrorialPanel, leftDownPointTutrorialText);
            currentTutorialStep += 1;
        }
    }

    /*****************************/
    /* In Game Tutorial steps    */
    /*****************************/

    public void showPlaceInitialSettlementMessage()
    {
        if (dontShowAgain == false && currentTutorialStep == 1 || currentTutorialStep == 3)
        {
            showMessage(mainHelpText, PLACE_INITIAL_SETTLEMENT_MESSAGE);
            currentTutorialStep += 1;
        }
        else
            Debug.Log("ShowPlaceInitialSettlementMessage called out of turn.");
    }

    public void showPlaceInitialRoadMessage()
    {
        if (dontShowAgain == false && currentTutorialStep == 2 || currentTutorialStep == 4)
        {
            showMessage(mainHelpText, PLACE_INITIAL_ROAD_MESSAGE);
            currentTutorialStep += 1;
        }
        else
            Debug.Log("ShowPlaceInitialRoadMessage called out of turn.");
    }

    public void showRollDiceTutorialBox()
    {
        if (dontShowAgain == false && currentTutorialStep == 5)
        {
            moveAndShowTutorialBox(rightDownPointTutrorialPanel, rightDownPointTutrorialText, ROLL_DICE_MESSAGE,
                                   ROLL_DICE_LOC);
            currentTutorialStep += 1;
        }
    }

    public void showBuildRoadTutorialBox()
    {
        if (dontShowAgain == false && currentTutorialStep == 6)
        {
            moveAndShowTutorialBox(leftDownPointTutrorialPanel, leftDownPointTutrorialText, BUILD_ROAD_MESSAGE,
                                   BUILD_ROAD_LOC, rightDownPointTutrorialPanel, rightDownPointTutrorialText);
            currentTutorialStep += 1;
        }
    }

    public void showExplainResourcesTutorialBox()
    {
        if (dontShowAgain == false && currentTutorialStep == 7)
        {
            moveAndShowTutorialBox(leftDownPointTutrorialPanel, leftDownPointTutrorialText, EXPLAIN_RESOURCES_MESSAGE,
                                   EXPLAIN_RESOURCES_LOC, rightDownPointTutrorialPanel, rightDownPointTutrorialText);
            currentTutorialStep += 1;
        }
    }

    /*****************************/
    /* General Tutorial methods  */
    /*****************************/

    public void endTutorial()
    {
        if (mainHelpText != null)
            mainHelpText.enabled = false;
        if (leftUpPointTutrorialPanel != null)
            leftUpPointTutrorialPanel.gameObject.SetActive(false);
        if (leftUpPointTutrorialText != null)
            leftUpPointTutrorialText.enabled = false;
        if (rightUpPointTutrorialPanel != null)
            rightUpPointTutrorialPanel.gameObject.SetActive(false);
        if (rightUpPointTutrorialText != null)
            rightUpPointTutrorialText.enabled = false;
        if (leftDownPointTutrorialPanel != null)
            leftDownPointTutrorialPanel.gameObject.SetActive(false);
        if (mainHelpText != null)
            leftDownPointTutrorialText.enabled = false;
        if (rightDownPointTutrorialPanel != null)
            rightDownPointTutrorialPanel.gameObject.SetActive(false);
        if (rightDownPointTutrorialText != null)
            rightDownPointTutrorialText.enabled = false;

        dontShowAgain = true;
    }

    // Moves either a left-pointing or a right-pointing (type) tutorial box to the specified
    // position and displays a message in it.
    public void moveAndShowTutorialBox(GameObject boxToShow, Text textBoxToShow, string message,
                                       Vector3 newPosition, GameObject boxToHide = null, Text textBoxToHide = null)
    {
        if (boxToHide != null)
            boxToHide.gameObject.SetActive(false);
        if (textBoxToHide != null)
            textBoxToHide.enabled = false;

        boxToShow.transform.position = newPosition;
        textBoxToShow.text = message;
        boxToShow.gameObject.SetActive(true);
        textBoxToShow.enabled = true;
    }

    private void showMessage(Text textBox, string message)
    {
        textBox.text = message;
        textBox.enabled = true;
    }

    public void closeDialogue(GameObject BtnParent)
    {
        BtnParent.gameObject.SetActive(false);
        if (dontShowAgain == true)
        {
            endTutorial();
        }
    }

    public void dontShowAgainClicked()
    {
        dontShowAgain = !dontShowAgain;
    }

    public void goToNextStep(GameObject BtnParent)
    {
        closeDialogue(BtnParent);

        if (currentTutorialStep == 7)
            showExplainResourcesTutorialBox();
    }
}
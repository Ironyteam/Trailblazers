using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using System.IO;


public class BoardManager : MonoBehaviour
{
    public Tutorial tutorial;  
    
    public Camera mainCamera; //++++++++++++++++++++++++++
    public Camera screenShotCamera; //++++++++++++++++++++++++++
	public Camera lookAtIslandCamera; //++++++++++++++++++++++++++

    public GameObject hexPrefab;
    public GameObject diceNumText;
    public GameObject ListBtn;
    public GameObject mapListPanelPG;
    public GameObject mapListPanelMOD;
    public GameObject portPrefab;

    public Color ORANGE = new Color(1, .3f, 0);
    
    private int numToAppendToName; // The number that will have to appended to the map name for it to be saved

    public Sprite[] DiceNumImages;

    private bool addPortEnabled = false;
    private bool choosingPort = false;

    private List<GameObject> hexesBorderingWater = new List<GameObject>();
    private Dictionary<GameObject, int> availablePortHexes = new Dictionary<GameObject, int>();

    private List<int> mapErrors = new List<int>();

    public Slider numOfPlayersSlider,
				  numOfVPSlider,
				  turnTimerSlider;
	public Toggle turnTimerToggleLocal,
				  characterAbilitiesToggle;
	public InputField gameLobbyNameNetwork;
	public Text createCanvasMapName, //********************
				createCanvasRecommendedVpTop,
				createCanvasRecommendedVpBottom,
			    turnTimerToggleText, 
				turnTimerValueText,
				numOfPlayersValueText,
				numOfVPValueText;
	public Button gameLobbyButtonNetwork,
				  characterSelectButtonLocal;
	public Image createCanvasScreenShot;
	public Light boardManagerDirectionalLight;
	public StylizedWater boardManagerOcean;

    // Confirm map deletion panel elements
    public Canvas confirmDeletionCanvas;
    public Button confirmDeletionBtn;
    public Button cancelDeletionBtn;
    public Text deleteMapText;
    
	public static int  numOfPlayers  = 2;
	public static int  victoryPoints = 5;
	public static int  turnTimerMax  = 30;
   public static bool turnTimerOn   = true;
   public static bool characterAbilitiesOn = true;
   public static int  localPlayerIndex;
                       
    string[] resources = new string[6]
    {
       "Wood", "Ore", "Grain", "Wool", "Brick", "Desert"
    };

    private List<GameObject> mapBtns = new List<GameObject>();        // Dyanmically created buttons for list of available boards

    private int currentResource = -1;     // The resource type that is currently being used to modify board
    private int currentDiceNum  = -1;     // The dice number that is currently being used to modify board

    public Canvas boardCreationCanvas;
    public Canvas boardSelctionCanvasMOD; // The board selection canvas for MODifying a board
    public Canvas boardSelctionCanvasPG;  // The board selection canvas for Pre Game selection
    public Canvas HexCanvas;              // Canvas that the 2d hex board is spawned on
	public Canvas createCanvas;
    public InputField mapNameField;       // Text field for naming a map that you are saving
    public Text mapNameTextMOD;           // Text box displaying the map names IN MODIFY BOARD SELCTION MENU
    public Text mapNameTextPG;            // Text box displaying the map names IN PRE GAME SELECTION MENU
    public Text warningText;
    public Text mapDetailsTextPG;
    public Text mapDetailsTextMOD;
    public Image MapScreenShortImageMOD;
    public Image MapScreenShortImagePG;
    public Text MissingScreenShotTextPG;
    public Text MissingScreenShotTextMOD;

    // Map error popup window buttons and text
    public Canvas errorsCanvas;
    public Button addRandomDesertBtn;
    public Button addRandomDiceNumsBtn;
    public Button ignoreWarningsBtn;
    public Text missingDesertText;
    public Text missingDiceNumsText;
    public Text warningsErrorText;
    public Button autoFixAllBtn;
    public Button returnAndFixBtn;
    public Button appendAndSaveBtn;
    public Button enterNewNameBtn;
    public Text mainErrorText;
    public InputField newNameField;
    public Button confirmNewNameBtn;
    public Button okayBtn;               //++++++++++++++++++++++++++++++++++

    public const string DefaultMapsPath = FileHandler.DefaultMapsPath;
    public const string SavedMapsPath   = FileHandler.SavedMapsPath;

    public const int MAP_NAME_CONFLICT_ERROR = 1;
    public const int MISSING_DESERT_ERROR = 2;
    public const int MISSING_DICE_NUMS_ERROR = 3;
    public const int WARNINGS_ERROR = 4;
    public const int TOO_FEW_HEXES_ERROR = 5; //++++++++++++++++++++++++++
    public const int NAME_FILTER_ERROR = 6;  //+++++++++++++++++++++++++
    public const int NAME_FORMAT_ERROR = 7;  //+++++++++++++++++++++++++++

    public const int MINIMUM_HEXES = 7; //++++++++++++++++++++++++++
    public const int MAX_MAPS_WITH_SAME_NAME = 99;

    private bool nameAppended = false;
    private bool ignoreWarnings = false;

    private int savedMapsStartindex;      // The index of the first saved (non-default) map in the array of maps
    private int board_index;              // The index of the current board being shown for selection

    public static bool startingGame = false; // Global variable to determine which canvas to show when board manager scene is loaded

    const int WIDTH  = HexTemplate.WIDTH;
    const int HEIGHT = HexTemplate.HEIGHT;

    public Vector3 currentHexTrans;

    const int LEFT  = -1;  // Index value sent to go left in the list of maps 
    const int RIGHT = -2;  // Index value sent to go right in the list of maps

    public const int SCREEN_SHOT_WIDTH = 1000;
    public const int SCREEN_SHOT_LENGTH = 1000;
    
    public const int OFFSCREEN_X_OFFSET = 500;
    public const int OFFSCREEN_Z_OFFSET = 500;

    private int[] resourceCounts = new int[6];
    private int[] diceNumCounts = new int[11];

    private GameObject hexToReceivePort = null;
    private GameObject mousedOverHex = null;
    private GameObject portGO = null;

    private HexTemplate[] maps; // The paths of all available maps
    private List<Sprite> screenShots = new List<Sprite>();
    public Sprite[] defaultMapImages;

    public static HexTemplate template = new HexTemplate();

    void Start()
    {
        FileHandler handler = new FileHandler();
        handler.checkForFiles();             //++++++++++++++++++++++++++++

        tutorial = GameObject.Find("BoardManager").GetComponent<Tutorial>();

        numOfPlayers = (int)numOfPlayersSlider.value;
		victoryPoints = (int)numOfVPSlider.value;
		turnTimerMax = (int)turnTimerSlider.value;
        

		lookAtIslandCamera.enabled = true; //**************************
		boardManagerDirectionalLight.gameObject.SetActive(false);
		boardManagerOcean.gameObject.SetActive(false);
        mainCamera.enabled = false; //++++++++++++++++++++++++++
        screenShotCamera.enabled = false; //++++++++++++++++++++++++++
        boardCreationCanvas.enabled = false;
        boardSelctionCanvasPG.enabled = false;
        boardSelctionCanvasMOD.enabled = false;
		createCanvas.enabled = false;
        errorsCanvas.enabled = false;
		showConditionalButtons();

        if (startingGame == true)
            preGameBoardSelectOn();
        else
            MapEditorSelectOn();
        startingGame = false;
    }

    void Update()
    {
        // See if raycast hit an object
        Ray toMouse = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (!EventSystem.current.IsPointerOverGameObject()) //+++++++++++++++++++++++++++
        {
            RaycastHit rhInfo;
            bool didHit = Physics.Raycast(toMouse, out rhInfo, 500.0f);

            if (didHit)
            {
                GameObject ourHitObject = rhInfo.collider.transform.gameObject;
                int x = ourHitObject.GetComponent<HexData>().x_index;
                int y = ourHitObject.GetComponent<HexData>().y_index;

                // Process left click
                if (Input.GetMouseButtonDown(0))
                {
                    // See if dice number or resource type change is necessary
                    if (currentResource != -1)
                        changeHex(x, y);
                    else if (currentDiceNum != -1)
                        changeDiceNumber(x, y);

                    // See if port selecting actions are necessary
                    else if (addPortEnabled)
                    {
                        // See if port placement is in progress
                        if (choosingPort == true)
                        {
                            if (availablePortHexes.ContainsKey(ourHitObject))
                                addPort(ourHitObject);
                        }

                        // See if a port that borders water is being selected
                        else
                        {
                            if (hexesBorderingWater.Contains(ourHitObject))
                                chooseHexForPort(ourHitObject);
                        }
                    }
                }

                // Reset a hexagon to water if a right click is detected
                else if (Input.GetMouseButtonDown(1))
                {
                    if (template.hex[x, y].resource >= 0)
                    {
                        resetHex(x, y);
                    }
                    else if (template.hex[x, y].hexOwningPort != null)
                    {
                        deletePort(x, y);
                    }
                }

                // Perform mouse over effect on a hexagon if necessary
                else
                {
                    // Transition mouse over effect if necessary
                    if (availablePortHexes.ContainsKey(ourHitObject))
                    {
                        if (choosingPort == true)
                        {
                            portGO.GetComponent<Renderer>().enabled = true;
                            if (mousedOverHex != ourHitObject)
                            {
                                mousedOverHex = ourHitObject;
                                changeMouseOverPort(ourHitObject);
                            }
                        }
                    }

                    // Remove mouse effect when cursor exits available hexagon (Cursor still within map area)
                    else
                    {
                        if (choosingPort == true && mousedOverHex != null)
                        {
                            portGO.GetComponent<Renderer>().enabled = false;
                        }
                    }
                }
            }

            // Remove mouse effect when cursor exits available hexagon (Cursor outside of map area)
            else
            {
                if (choosingPort == true && mousedOverHex != null)
                {
                    portGO.GetComponent<Renderer>().enabled = false;
                }
            }
        }
    }
        
    public void preGameBoardSelectOn()
    {
        boardSelctionCanvasPG.enabled = true;
		createCanvas.enabled = false;
		mainCamera.enabled = false;
		boardManagerDirectionalLight.gameObject.SetActive(false);
		boardManagerOcean.gameObject.SetActive(false);
		lookAtIslandCamera.enabled = true;		
        displayMaps();
    }

    public void MapEditorSelectOn()
    {
        boardCreationCanvas.enabled    = false;
        boardSelctionCanvasPG.enabled  = false;
		createCanvas.enabled    = false;
        boardSelctionCanvasMOD.enabled = true;
        displayMaps();
    }

    public void ReturnToMapEditorSelect()
    {
		mainCamera.enabled = false;
		boardManagerDirectionalLight.gameObject.SetActive(false);
		boardManagerOcean.gameObject.SetActive(false);
		lookAtIslandCamera.enabled = true;
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Destroy(template.hex[x, y].portGO);
                Destroy(template.hex[x, y].hex_go);
            }
        }
        boardCreationCanvas.enabled = false;
        errorsCanvas.enabled = false;
        boardSelctionCanvasMOD.enabled = true;
        displayMaps();
    }

    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void startGame()
    {
        boardSelctionCanvasPG.enabled = false;
        FileHandler reader = new FileHandler();
        if (board_index < savedMapsStartindex)
            template = reader.retrieveMap(maps[board_index].mapName, true);
        else
            template = reader.retrieveMap(maps[board_index].mapName, false);
        
        createCanvasScreenShot.sprite = screenShots[board_index];
		createCanvasRecommendedVpTop.text    = "Recommended Minimum Vp: " + template.minVP;
		createCanvasRecommendedVpBottom.text = "Recommended Maximum Vp: " + template.maxVP;
        createCanvasMapName.text = maps[board_index].mapName;
        createCanvas.enabled = true;
    }

        public void revertHex(GameObject hex)        // Temp function to simulate glow
    {
        switch (template.hex[hex.GetComponent<HexData>().x_index, hex.GetComponent<HexData>().y_index].resource)
        {
            case 0:
                hex.GetComponentInChildren<Renderer>().material.color = Color.black;
                break;
            case 1:
                hex.GetComponentInChildren<Renderer>().material.color = Color.grey;
                break;
            case 2:
                hex.GetComponentInChildren<Renderer>().material.color = Color.green;
                break;
            case 3:
                hex.GetComponentInChildren<Renderer>().material.color = Color.white;
                break;
            case 4:
                hex.GetComponentInChildren<Renderer>().material.color = Color.red;
                break;
            case 5:
                hex.GetComponentInChildren<Renderer>().material.color = Color.yellow;
                break;
        }
    }
    
    public void displayMaps()
    {
        List<string> mapNames = new List<string>();


        MissingScreenShotTextPG.enabled = false;
        MissingScreenShotTextMOD.enabled = false;

        if (boardSelctionCanvasPG.enabled)
        {
            makeScrollingMapList(mapListPanelPG);

            foreach (HexTemplate template in maps)
            {
                mapNames.Add(template.mapName);
            }

            screenShots.Clear();

            if (savedMapsStartindex == defaultMapImages.Length)
                screenShots.AddRange(defaultMapImages);
            else
                Debug.Log("Error in function displayMaps:" +
                         "\nNumber of defaultMaps does not equal number of defaultMapImages");

            FileHandler reader = new FileHandler();
            screenShots.AddRange(reader.getScreenShots(mapNames, savedMapsStartindex));

            if (screenShots[board_index] != null)            //vvvvvvvvvvvvvvvvvvv
                MapScreenShortImagePG.sprite = screenShots[board_index];
            else
            {
                MapScreenShortImagePG.sprite = null;
                MissingScreenShotTextPG.text = "No preview available";
                MissingScreenShotTextPG.enabled = true;
            }

            mapNameTextPG.text = maps[board_index].mapName;
            mapDetailsTextPG.text = "Minimum Victory Points: " + maps[board_index].minVP + "\n" +
                                    "Maximum Victory Points: " + maps[board_index].maxVP;
        }
        else if (boardSelctionCanvasMOD.enabled)
        {
            makeScrollingMapList(mapListPanelMOD);

            foreach (HexTemplate template in maps)
            {
                mapNames.Add(template.mapName);
            }

            screenShots.Clear();

            if (savedMapsStartindex == defaultMapImages.Length)
                screenShots.AddRange(defaultMapImages);
            else
                Debug.Log("Error in function displayMaps:" +
                         "\nNumber of defaultMaps does not equal number of defaultMapImages");

            FileHandler reader = new FileHandler();
            screenShots.AddRange(reader.getScreenShots(mapNames, savedMapsStartindex));

            if (screenShots[board_index] != null)               //vvvvvvvvvvvvvvvvvvv
                MapScreenShortImageMOD.sprite = screenShots[board_index];
            else
            {
                MapScreenShortImageMOD.sprite = null;
                MissingScreenShotTextMOD.text = "No preview available";
                MissingScreenShotTextMOD.enabled = true;
            }

            mapNameTextMOD.text = maps[board_index].mapName;
            mapDetailsTextMOD.text = "Minimum Victory Points: " + maps[board_index].minVP + "\n" +
                                    "Maximum Victory Points: " + maps[board_index].maxVP;
        }

       
    }

    public void deleteMap()
    {
        if (board_index >= savedMapsStartindex)
        {
            FileHandler handler = new FileHandler();
            handler.deleteMap(maps[board_index].mapName);
            displayMaps();
        }
    }

    public void AddPortClicked()  // Glowing effect code added
    {
        addPortEnabled = true;
        currentResource = -1;
        currentDiceNum = -1;

        hexesBorderingWater = getHexesBorderedByWater(template);

        // Add glowing effect to hexagons that are bordered by water
        foreach(GameObject hex in hexesBorderingWater)
        {
            //changeGlowEffect(hex, true);
            hex.GetComponentInChildren<Renderer>().material.color = ORANGE;
        }
        
        tutorial.showSelectHexForPortTutorialBox();
    }      

    public void chooseHexForPort(GameObject chosenHex)       // Glowing effect code added
    {
        GameObject[] hexes;
        int x = chosenHex.GetComponent<HexData>().x_index;
        int y = chosenHex.GetComponent<HexData>().y_index;

        // Remove glowing effect from hexagons that are bordered by water
        foreach (GameObject hex in hexesBorderingWater)
        {
            //changeGlowEffect(hex, false);
            revertHex(hex);
        }

        // Find each water hexagon surrounding the chosen hexagon and add a glowing effect to them
        hexes = getSurroundingHexes(x, y);
        for (int index = 0; index < hexes.Length; index++)
        {
            if (template.hex[hexes[index].GetComponent<HexData>().x_index, hexes[index].GetComponent<HexData>().y_index].resource == -1)
            {
                //changeGlowEffect(hexes[index], true);
                hexes[index].GetComponentInChildren<Renderer>().material.color = ORANGE;
                availablePortHexes.Add(hexes[index], index);
            }
        }

        tutorial.showAddPortTutorialBox();
        
        choosingPort = true;
        hexToReceivePort = chosenHex;
        currentHexTrans = hexToReceivePort.transform.position;
        portGO = Instantiate(portPrefab, new Vector3(0, 0, 0), Quaternion.identity, HexCanvas.transform);
        portGO.GetComponent<Renderer>().enabled = false;
    }

    public int changeMouseOverPort(GameObject newHex)
    {
        int portSideIndex = -1;

        if (availablePortHexes.TryGetValue(newHex, out portSideIndex))
        {
            switch (portSideIndex)
            {
                
                case 0:
                    
                    portGO.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0)); ;
                    portGO.transform.position = new Vector3(currentHexTrans.x, currentHexTrans.y, currentHexTrans.z + .7f);
                    Debug.Log(portSideIndex);
                    break;
                case 1:
                    portGO.transform.rotation = Quaternion.Euler(new Vector3(90, 0, -60)); ;
                    portGO.transform.position = new Vector3(currentHexTrans.x + .655f, currentHexTrans.y, currentHexTrans.z + .315f);
                    Debug.Log(portSideIndex);

                    
                    break;
                case 2:
                    portGO.transform.rotation = Quaternion.Euler(new Vector3(90, 0, -120)); ;
                    portGO.transform.position = new Vector3(currentHexTrans.x + .585f, currentHexTrans.y, currentHexTrans.z - .38f);
                    Debug.Log(portSideIndex);

                    break;
                case 3:
                    portGO.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0)); ;
                    portGO.transform.position = new Vector3(currentHexTrans.x, currentHexTrans.y, currentHexTrans.z - .7f);
                    Debug.Log(portSideIndex);

                    break;
                case 4:
                    portGO.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 120)); ;
                    portGO.transform.position = new Vector3(currentHexTrans.x - .585f, currentHexTrans.y, currentHexTrans.z - .38f);
                    Debug.Log(portSideIndex);

                    break;
                case 5:
                    portGO.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 60)); ;
                    portGO.transform.position = new Vector3(currentHexTrans.x - .655f, currentHexTrans.y, currentHexTrans.z + .315f);
                    Debug.Log(portSideIndex);

                    break;

            }
        }
        return portSideIndex;
    }

    public void addPort(GameObject chosenHex)
    {
        int portSideIndex;
        int hex_x = hexToReceivePort.GetComponent<HexData>().x_index;
        int hex_y = hexToReceivePort.GetComponent<HexData>().y_index;

        availablePortHexes.TryGetValue(chosenHex, out portSideIndex);

        if (mousedOverHex != chosenHex)
            changeMouseOverPort(chosenHex);

        template.hex[hex_x, hex_y].portSide = portSideIndex;
        template.hex[hex_x, hex_y].portGO = portGO;
        template.hex[hex_x, hex_y].waterPortHex = chosenHex;
        template.hex[chosenHex.GetComponent<HexData>().x_index, chosenHex.GetComponent<HexData>().y_index].hexOwningPort = template.hex[hex_x, hex_y].hex_go;

        tutorial.endTutorial();
        
        resetPortAddingChanges();
    }

    private void resetPortAddingChanges()       // Glowing effect code added
    {
        // Remove glowing effect from water hexagons surrounding chosen hexagon if necessary
        if (choosingPort == true)
        {
            Dictionary<GameObject, int>.KeyCollection hexes = availablePortHexes.Keys;
            foreach(GameObject hex in hexes)
            {
                //changeGlowEffect(hex, false);
                hex.GetComponentInChildren<Renderer>().material.color = Color.blue;
            }
        }

        // Remove glowing effect from hexagons bordered by water if necessary
        else
        {
            foreach (GameObject hex in hexesBorderingWater)
            {
                //changeGlowEffect(hex, false);
                revertHex(hex);
            }
        }

        mousedOverHex = null;
        portGO = null;
        hexToReceivePort = null;
        availablePortHexes.Clear();
        hexesBorderingWater.Clear();
        addPortEnabled = false;
        choosingPort = false;
    }

    private void changeGlowEffect(GameObject hex, bool turningOn)
    {
        // Turn glowing effect on
        if (turningOn)
        {

        }

        // Turn glowing effect off
        else
        {

        }
    }

    public GameObject[] getSurroundingHexes(int x, int y)
    {
        GameObject[] hexes = new GameObject[6]
        {
            null, null, null, null, null, null
        };

        // Get hexes surrounding the hex if it is on an odd column
        if (x % 2 == 1)
        {
            if (y < (HEIGHT - 1))
            {
                hexes[0] = template.hex[x, y + 1].hex_go;
               
            }
            if (x < (WIDTH - 1) && y < (HEIGHT - 1))
            {
                hexes[1] = template.hex[x + 1, y + 1].hex_go;
                
            }
            if (x < (WIDTH - 1))
            {
                hexes[2] = template.hex[x + 1, y].hex_go;
                
            }
            if (y > 0)
            {
                hexes[3] = template.hex[x, y - 1].hex_go;
                
            }
            if (x > 0)
            {
                hexes[4] = template.hex[x - 1, y].hex_go;
                
            }
            if (x > 0 && y < (HEIGHT - 1))
            {
                hexes[5] = template.hex[x - 1, y + 1].hex_go;
                
            }

        }

        // Get hexes surrounding the hex if it is on an even column
        else
        {
            if (y < (HEIGHT - 1))
            {
                hexes[0] = template.hex[x, y + 1].hex_go;
                
            }
            if (x < (WIDTH - 1))
            {
                hexes[1] = template.hex[x + 1, y].hex_go;
                
            }
            if (x < (WIDTH - 1) && y > 0)
            {
                hexes[2] = template.hex[x + 1, y - 1].hex_go;
                
            }
            if (y > 0)
            {
                hexes[3] = template.hex[x, y - 1].hex_go;
                
            }
            if (x > 0 && y > 0)
            {
                hexes[4] = template.hex[x - 1, y - 1].hex_go;
                
            }
            if (x > 0)
            {
                hexes[5] = template.hex[x - 1, y].hex_go;
                
            }
        }
        return hexes;
    }

    public List<GameObject> getHexesBorderedByWater(HexTemplate template)
    {
        List<GameObject> hexesBorderedByWater = new List<GameObject>();
        GameObject[] borderedWaterHexes;
        bool bordersWater = false;

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (template.hex[x, y].resource >= 0 && template.hex[x, y].portGO == null)
                {
                    borderedWaterHexes = getSurroundingHexes(x, y);

                    for (int index = 0; index < borderedWaterHexes.Length; index++)
                    {
                        if (borderedWaterHexes[index] != null)
                        {
                            if (template.hex[borderedWaterHexes[index].GetComponent<HexData>().x_index, borderedWaterHexes[index].GetComponent<HexData>().y_index].resource == -1)
                            {
                                bordersWater = true;
                                index = borderedWaterHexes.Length;
                            }
                        }
                    }

                    if (bordersWater == true)
                    {
                        hexesBorderedByWater.Add(template.hex[x, y].hex_go);
                    }
                    bordersWater = false;
                }
            }
        }
        return hexesBorderedByWater;
    }

    public bool addRandomDesert(HexTemplate template)
    {
        List<GameObject> hexesBorderedByWater = getHexesBorderedByWater(template);
        GameObject randomHex;
        GameObject[] hexes;
        bool desertAdditionSuccessful;

        if (hexesBorderedByWater.Count > 0)
        {
            randomHex = hexesBorderedByWater[UnityEngine.Random.Range(0, hexesBorderedByWater.Count)];
            hexes = getSurroundingHexes(randomHex.GetComponent<HexData>().x_index, randomHex.GetComponent<HexData>().y_index);

            for (int index = 0; index < hexes.Length; index++)
            {
                if (template.hex[hexes[index].GetComponent<HexData>().x_index, hexes[index].GetComponent<HexData>().y_index].resource == -1)
                {
                    hexes[index].GetComponentInChildren<Renderer>().material.color = Color.yellow;
                    hexes[index].transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = DiceNumImages[5];
                    template.hex[hexes[index].GetComponent<HexData>().x_index, hexes[index].GetComponent<HexData>().y_index].setDiceNum(7);
                    template.hex[hexes[index].GetComponent<HexData>().x_index, hexes[index].GetComponent<HexData>().y_index].setResource(5);
                    resourceCounts[5] += 1;
                    diceNumCounts[5] += 1;
                    index = hexes.Length;
                }
            }
            desertAdditionSuccessful = true;
        }
        else
            desertAdditionSuccessful = false;

        return desertAdditionSuccessful;
    }

    private bool printAnyWarnings()
    {
        string warning = "Warning: ";
        bool resourceErrorsExist = false;
        bool diceNumErrorsExist = false;

        warningText.text = "";

        for (int index = 0; index < resourceCounts.Length; index++)
        {
            if (resourceCounts[index] == 0)
            {
                if (resourceErrorsExist == false)
                {
                    warning = String.Concat(warning, "Unused Resource Types: ");
                    resourceErrorsExist = true;
                    warning = String.Concat(warning, resources[index]);
                }
                else
                    warning = String.Concat(warning, ", " + resources[index]);
            }
        }
        if (resourceErrorsExist)
            warning = String.Concat(warning, "\n");

        for (int index = 0; index < diceNumCounts.Length; index++)
        {
            if (diceNumCounts[index] == 0)
            {
                if (diceNumErrorsExist == false)
                {
                    warning = String.Concat(warning, "Unused Dice Numbers: ");
                    diceNumErrorsExist = true;
                    warning = String.Concat(warning, (index + 2));
                }
                else
                    warning = String.Concat(warning, ", " + (index + 2));
            }
        }
        if (diceNumErrorsExist)
            warning = String.Concat(warning, "\n");
        warning = String.Concat(warning, "Gameplay will be affected.");

        if (resourceErrorsExist || diceNumErrorsExist)
            warningText.text = warning;
        return (resourceErrorsExist || diceNumErrorsExist);
    }

    public void autoFixMissingDesertError()
    {
        addRandomDesert(template);
        SaveBoard();
    }

    public void autoFixDiceNumError()
    {
        addRandomDiceNums();
        SaveBoard();
    }

    public void ignoreAllWarnings()
    {
        ignoreWarnings = true;
        SaveBoard();
    }

    public void autoFixAllErrors()
    {
        if (mapErrors.Contains(MISSING_DESERT_ERROR))
            addRandomDesert(template);
        if (mapErrors.Contains(MISSING_DICE_NUMS_ERROR))
            addRandomDiceNums();
        if (mapErrors.Contains(WARNINGS_ERROR))
            ignoreWarnings = true;
        SaveBoard();
    }

    public void returnAndFixErrors()
    {
        errorsCanvas.enabled = false;
        boardCreationCanvas.gameObject.SetActive(true);
        ignoreWarnings = false;
        printAnyWarnings();
    }

    public void enterNewName()
    {
        appendAndSaveBtn.gameObject.SetActive(false);
        enterNewNameBtn.gameObject.SetActive(false);
        boardCreationCanvas.gameObject.SetActive(false);
        
        confirmNewNameBtn.gameObject.SetActive(true);
        newNameField.gameObject.SetActive(true);
        mainErrorText.text = "\n\n\nPlease Enter a new name";
         
    }

    public void appendNameAndSaveMap()
    {
        name = mapNameField.text;
        mapNameField.text     = name + "(" + numToAppendToName + ")";
        nameAppended = true;
        SaveBoard();
    }

    public void confirmNewName()
    {
        string newName = newNameField.text;
        Debug.Log("here");
        if (newName != "")
        {
            mapNameField.text = newName;
            SaveBoard();
            newNameField.text = "";
        }
        
    }

    

    private void displayAnyErrors(List<int> errors, string mapName)
    {
        bool boardErrorExists = false;

        boardCreationCanvas.gameObject.SetActive(false);
        errorsCanvas.enabled = true;
        addRandomDesertBtn.gameObject.SetActive(false);
        addRandomDiceNumsBtn.gameObject.SetActive(false);
        ignoreWarningsBtn.gameObject.SetActive(false);
        autoFixAllBtn.gameObject.SetActive(false);
        returnAndFixBtn.gameObject.SetActive(false);
        appendAndSaveBtn.gameObject.SetActive(false);
        enterNewNameBtn.gameObject.SetActive(false);
        confirmNewNameBtn.gameObject.SetActive(false);
        mainErrorText.enabled = false;
        missingDesertText.enabled = false;
        missingDiceNumsText.enabled = false;
        warningsErrorText.enabled = false;
        newNameField.gameObject.SetActive(false);
        okayBtn.gameObject.SetActive(false);       //++++++++++++++++++++++++++++

        // Print error if the map has less than the minimum number of hexagons
        if (errors.Contains(TOO_FEW_HEXES_ERROR))      //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
        {
            mainErrorText.enabled = true;
            mainErrorText.text = "Please add more tiles to this map." +
                               "\nA map must have at least " + MINIMUM_HEXES + " tiles.";
            okayBtn.gameObject.SetActive(true);
        }
        else
        {
            if (errors.Contains(MISSING_DESERT_ERROR))
            {
                addRandomDesertBtn.gameObject.SetActive(true);
                missingDesertText.enabled = true;
                missingDesertText.text =
                    "Error: This map contains no desert resource. A desert resource is required.";
                boardErrorExists = true;
            }

            if (errors.Contains(MISSING_DICE_NUMS_ERROR))
            {
                addRandomDiceNumsBtn.gameObject.SetActive(true); ;
                missingDiceNumsText.enabled = true;
                missingDiceNumsText.text = "Error: Not all active map tiles have dice numbers assigned.";
                boardErrorExists = true;
            }

            if (errors.Contains(WARNINGS_ERROR))
            {
                ignoreWarningsBtn.gameObject.SetActive(true);
                warningsErrorText.enabled = true;
                warningsErrorText.text = "Error: Some unused resources and dice numbers. Gameplay will be affected.";
                boardErrorExists = true;
            }

            if (boardErrorExists)
            {
                autoFixAllBtn.gameObject.SetActive(true);
                returnAndFixBtn.gameObject.SetActive(true);
            }

            // Handle map name errors if any exist
            else
            {
                if (errors.Contains(NAME_FORMAT_ERROR))
                {
                    mainErrorText.enabled = true;

                    
                    confirmNewNameBtn.gameObject.SetActive(true);
                    newNameField.gameObject.SetActive(true);
                    mainErrorText.text = "Error: This map name is not in the proper format" +
                                       "\nNames can only contain letters and spaces" +
                                       "\nPlease enter a different name.";
                }

                else if (errors.Contains(NAME_FILTER_ERROR))
                {
                    mainErrorText.enabled = true;

                    confirmNewNameBtn.gameObject.SetActive(true);
                    newNameField.gameObject.SetActive(true);
                    mainErrorText.text = "Error: This map name did not pass the filter" +
                                       "\nPlease enter a different name.";
                }

                else if (errors.Contains(MAP_NAME_CONFLICT_ERROR))
                {
                   FileHandler fh = new FileHandler();
                    numToAppendToName = fh.findNumberToAppendToName(mapName, MAX_MAPS_WITH_SAME_NAME);    //+++++++++++++++++++++++

                    mainErrorText.enabled = true;

                    // If a valid within the limit was found display choice message
                    if (numToAppendToName >= 0)                         //++++++++++++++++++++++++++
                    {
                        appendAndSaveBtn.gameObject.SetActive(true);
                        enterNewNameBtn.gameObject.SetActive(true);
                        mainErrorText.text = "Error: Map already exists with the name " + mapName + "." +
                                              "\nMap will be saved as " + mapName + "(" + numToAppendToName + ").";
                    }
                    else
                    {
                        confirmNewNameBtn.gameObject.SetActive(false);
                        newNameField.gameObject.SetActive(true);
                        mainErrorText.text = "Error: This map name has been used too many times." +
                                              "\nPlease enter a different name.";
                    }
                }
            }
        }
    }

    private void makeScrollingMapList(GameObject panel)
    {
        FileHandler reader = new FileHandler();

        maps = reader.getAllMaps(out savedMapsStartindex).ToArray();
        board_index = 0;

        if (mapBtns.Count > 0)
        {
            foreach (GameObject btn in mapBtns)
            {
                Destroy(btn);
            }
            mapBtns.Clear();
        }

        for (int index = 0; index < maps.Length; index++)
        {
            GameObject tempGO = Instantiate(ListBtn) as GameObject;
            tempGO.transform.SetParent(panel.transform, false);

            int desiredIndex = index;

            mapBtns.Add(tempGO);
            tempGO.GetComponent<Button>().GetComponentInChildren<Text>().text = maps[index].mapName;
            tempGO.GetComponent<Button>().onClick.AddListener(() => { ChangeDisplayedMap(desiredIndex); });
        }
    }

    public void CreateNewBoard()
    {
		lookAtIslandCamera.enabled = false;
		boardManagerDirectionalLight.gameObject.SetActive(true);
		boardManagerOcean.gameObject.SetActive(true);
		mainCamera.enabled = true;
        boardSelctionCanvasPG.enabled = false;
        boardSelctionCanvasMOD.enabled = false;
        boardCreationCanvas.enabled = true;
        boardCreationCanvas.gameObject.SetActive(true);
        mapNameField.text = "";
        SpawnBoard(null, false);
        tutorial.init();
        tutorial.showSelectResourceTutorialBox();
    }

    public void ModifyMap()
    {
		lookAtIslandCamera.enabled = false;
		boardManagerDirectionalLight.gameObject.SetActive(true);
		boardManagerOcean.gameObject.SetActive(true);
		mainCamera.enabled = true;
        nameAppended = false;
        boardSelctionCanvasMOD.enabled = false;
        boardCreationCanvas.enabled = true;
        boardCreationCanvas.gameObject.SetActive(true);
        mapNameField.text = "";
        FileHandler reader = new FileHandler();
        if (board_index < savedMapsStartindex)
            template = reader.retrieveMap(maps[board_index].mapName, true);
        else
            template = reader.retrieveMap(maps[board_index].mapName, false);
        SpawnBoard(template, false);
        
    }

    public void changeSelectedResource(int resrouceNum)
    {
        if (addPortEnabled == true)
            resetPortAddingChanges();
        currentResource = resrouceNum;
        currentDiceNum  = -1;
    }

    public void resetHex(int x, int y)
    {
        // If the hexagon being reset has a port unlink and delete it
        if (template.hex[x, y].portGO != null)
        {
             deletePort(template.hex[x, y].waterPortHex.GetComponent<HexData>().x_index,
                        template.hex[x, y].waterPortHex.GetComponent<HexData>().y_index);
        }

        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.blue;
        resourceCounts[template.hex[x, y].resource] -= 1;
        if (template.hex[x, y].dice_number >= 2)
            diceNumCounts[template.hex[x, y].dice_number - 2] -= 1;
        template.hex[x, y].setDiceNum(-1);
        template.hex[x, y].setResource(-1);
        template.hex[x, y].hex_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
        printAnyWarnings();
    }

    private void deletePort(int waterHex_x, int waterHex_y)
    {
        int hexOwningPortGO_x = template.hex[waterHex_x, waterHex_y].hexOwningPort.GetComponent<HexData>().x_index;
        int hexOwningPortGO_y = template.hex[waterHex_x, waterHex_y].hexOwningPort.GetComponent<HexData>().y_index;

        Destroy(template.hex[hexOwningPortGO_x, hexOwningPortGO_y].portGO);
        template.hex[hexOwningPortGO_x, hexOwningPortGO_y].portGO = null;
        template.hex[hexOwningPortGO_x, hexOwningPortGO_y].waterPortHex = null;
        template.hex[hexOwningPortGO_x, hexOwningPortGO_y].portSide = -1;
        template.hex[waterHex_x, waterHex_y].hexOwningPort = null;
    }
    
    public void confirmDeletionPanelOn()
    {
        confirmDeletionCanvas.enabled = true;
        confirmDeletionBtn.gameObject.SetActive(true);
        cancelDeletionBtn.gameObject.SetActive(true);
        deleteMapText.enabled = true;
        deleteMapText.fontSize = 16;
        deleteMapText.text = "Are you sure you want to delete " + maps[board_index].mapName;
    }

    public void changeHex(int x, int y)
    {
        if (template.hex[x, y].resource == -1 && template.hex[x, y].hexOwningPort != null)
            deletePort(x, y);

        if (template.hex[x, y].resource == 5)
        {
            template.hex[x, y].hex_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
            template.hex[x, y].setDiceNum(-1);
            diceNumCounts[5] -= 1;
        }
        
        switch (currentResource)
        {
            case 0:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.black;
                if (template.hex[x, y].resource >= 0)
                {
                    resourceCounts[template.hex[x, y].resource] -= 1;
                }
                template.hex[x, y].setResource(currentResource);
                resourceCounts[0] += 1;
                break;
            case 1:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.grey;
                if (template.hex[x, y].resource >= 0)
                {
                    resourceCounts[template.hex[x, y].resource] -= 1;
                }
                template.hex[x, y].setResource(currentResource);
                resourceCounts[1] += 1;
                break;
            case 2:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.green;
                if (template.hex[x, y].resource >= 0)
                {
                    resourceCounts[template.hex[x, y].resource] -= 1;
                }
                template.hex[x, y].setResource(currentResource);
                resourceCounts[2] += 1;
                break;
            case 3:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.white;
                if (template.hex[x, y].resource >= 0)
                {
                    resourceCounts[template.hex[x, y].resource] -= 1;
                }
                template.hex[x, y].setResource(currentResource);
                resourceCounts[3] += 1;
                break;
            case 4:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.red;
                if (template.hex[x, y].resource >= 0)
                {
                    resourceCounts[template.hex[x, y].resource] -= 1;
                }
                template.hex[x, y].setResource(currentResource);
                resourceCounts[4] += 1;
                break;
            case 5:
                if (resourceCounts[5] == 0)
                {
                    template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.yellow;
                    if (template.hex[x, y].resource >= 0)
                    {
                        resourceCounts[template.hex[x, y].resource] -= 1;
                        if (template.hex[x, y].dice_number >= 2)
                            diceNumCounts[template.hex[x, y].dice_number - 2] -= 1;
                    }
                    template.hex[x, y].setResource(currentResource);
                    template.hex[x, y].hex_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = DiceNumImages[5];
                    template.hex[x, y].setDiceNum(7);
                    resourceCounts[5] += 1;
                    diceNumCounts[5] += 1;
                }
                break;
        }
        printAnyWarnings();
        tutorial.showSelectDiceNumTutorialBox();
    }

    public void changeDiceNumber(int x, int y)
    {
        if (template.hex[x, y].resource >= 0 && template.hex[x, y].resource != 5)
        {
            if (template.hex[x, y].dice_number >= 2)
                diceNumCounts[template.hex[x, y].dice_number - 2] -= 1;
            template.hex[x, y].setDiceNum(currentDiceNum);
            template.hex[x, y].hex_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = DiceNumImages[currentDiceNum - 2];
            diceNumCounts[template.hex[x, y].dice_number - 2] += 1;
            printAnyWarnings();
            
            tutorial.showClickAddPortTutorialBox();
        }
    }

    public void changeSelectedDiceNum(int diceNum)
    {
        if (addPortEnabled == true)
            resetPortAddingChanges();
        currentDiceNum  = diceNum;
        currentResource = -1;
    }

    public void ChangeDisplayedMap(int desiredIndex)
    {
        FileHandler fh = new FileHandler();

        MissingScreenShotTextPG.enabled = false;
        MissingScreenShotTextMOD.enabled = false;

        if (desiredIndex == RIGHT && board_index < (maps.Length - 1))
            board_index += 1;
        else if (desiredIndex == LEFT && board_index > 0)
            board_index -= 1;
        else if (desiredIndex >= 0 && desiredIndex < maps.Length)
            board_index = desiredIndex;

        if (boardSelctionCanvasPG.enabled)
        {
            mapNameTextPG.text = maps[board_index].mapName;
            mapDetailsTextPG.text = "Minimum Victory Points: " + maps[board_index].minVP + "\n" +
                                    "Maximum Victory Points: " + maps[board_index].maxVP;

            if (screenShots[board_index] == null)                  //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
            {
                template = fh.retrieveMap(maps[board_index].mapName, false);
                screenShots[board_index] = saveMapScreenShotOffScreen(template);

                for (int x = 0; x < WIDTH; x++)
                {
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        Destroy(template.hex[x, y].portGO);
                        Destroy(template.hex[x, y].hex_go);
                    }
                }
            }

            if (screenShots[board_index] != null)
                MapScreenShortImagePG.sprite = screenShots[board_index];
            else
            {
                MapScreenShortImagePG.sprite = null;
                MissingScreenShotTextPG.text = "No preview available";
                MissingScreenShotTextPG.enabled = true;
            }

        }
        else
        {
            mapNameTextMOD.text = maps[board_index].mapName;
            mapDetailsTextMOD.text = "Minimum Victory Points: " + maps[board_index].minVP + "\n" +
                                    "Maximum Victory Points: " + maps[board_index].maxVP;

            if (screenShots[board_index] == null)                  //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
            {
                template = fh.retrieveMap(maps[board_index].mapName, false);
                screenShots[board_index] = saveMapScreenShotOffScreen(template);

                for (int x = 0; x < WIDTH; x++)
                {
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        Destroy(template.hex[x, y].portGO);
                        Destroy(template.hex[x, y].hex_go);
                    }
                }
            }

            if (screenShots[board_index] != null)                 //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
                MapScreenShortImageMOD.sprite = screenShots[board_index];
            else
            {
                MapScreenShortImageMOD.sprite = null;
                MissingScreenShotTextMOD.text = "No preview available";
                MissingScreenShotTextMOD.enabled = true;
            }
        }
    }

    private List<int> getAnyErrors(string newMapName, string warnings)
    {
        List<int> errors = new List<int>();
        List<HexTemplate> maps = new List<HexTemplate>();
        FileHandler fileChecker = new FileHandler();
        NameChecker nameChecker = new NameChecker();    //++++++++++++++++++++++++++
        string formattedName = "";
        int savedMapsStartindex = 0;
        int totalHexes = 0;                                    //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvv

        if (nameAppended == false)
        {
            if (nameChecker.passesFormatCheck(newMapName, out formattedName))
            {
                if (nameChecker.passesFilter(formattedName) == false)
                {
                    errors.Add(NAME_FILTER_ERROR);
                }
            }
            else
            {
                errors.Add(NAME_FORMAT_ERROR);
            }
        }

        // See if the map contains the minimum number of hexagons
        for (int index = 0; index < resourceCounts.Length; index++)
        {
            totalHexes += resourceCounts[index];
        }
        if (totalHexes < MINIMUM_HEXES)
            errors.Add(TOO_FEW_HEXES_ERROR);

        // See if a map already exists with the desired name
        maps = fileChecker.getAllMaps(out savedMapsStartindex);
        for (int index = savedMapsStartindex; index < maps.Count; index++)
        {
            if (String.Compare(maps[index].mapName, newMapName) == 0)
            {
                errors.Add(MAP_NAME_CONFLICT_ERROR);
                index = maps.Count;
            }
        }

        // See if the desert hexagon is missing
        if (resourceCounts[5] == 0)
        {
            errors.Add(MISSING_DESERT_ERROR);
        }

        // See if there are missing dice numbers
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (template.hex[x, y].resource >= 0 && template.hex[x, y].dice_number == -1)
                {
                    errors.Add(MISSING_DICE_NUMS_ERROR);
                    x = WIDTH;
                    y = HEIGHT;
                }
            }
        }

        // See if there are still warnings
        if (warnings != "" && ignoreWarnings == false)
            errors.Add(WARNINGS_ERROR);

        return errors;
    }

    public void SaveBoard()
    {
        string mapName;
        string warnings;
        
        int minVP = 0;
        int maxVP = 0;
        
        mapName = mapNameField.text;

        if (mapName != "")
        {
            printAnyWarnings();
            warnings = warningText.text;

            mapErrors = getAnyErrors(mapName, warnings);

            if (mapErrors.Count > 0)
            {
                displayAnyErrors(mapErrors, mapName);
            }
            else
            {
                calculateVictoryPoints(out minVP, out maxVP);
                saveMapScreenShot(mapName, screenShotCamera);
                
                FileHandler writer = new FileHandler();
                writer.saveMap(template, mapName, "Bob", minVP, maxVP);             // Get user name for creator parameter
                ReturnToMapEditorSelect();
            }
        }
    }

    private void calculateVictoryPoints(out int minVP, out int maxVP)
    {
        int totalHexes = 0;

        for (int index = 0; index < resourceCounts.Length; index++)
        {
            totalHexes += resourceCounts[index];
        }

        minVP = totalHexes / 2 - 2;
        maxVP = totalHexes / 2 + 2;
        if (minVP <= 0)
            minVP = 1;
    }

    public void addRandomDiceNums()
    {
        template = randomizeDiceNumsInEditor();
    }

    private HexTemplate randomizeDiceNumsInEditor()
    {
        List<Hex> landHexes = new List<Hex>();
        int hexRandomNum;
        int diceRandomNum;
        List<int> diceNums = new List<int>()
        {
            2, 3, 4, 5, 6, 8, 9, 10, 11, 12
        };
        List<int> availableDiceNums = new List<int>(diceNums);

        // Form a list of all land hexagons
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (template.hex[x, y].resource >= 0)
                {
                    landHexes.Add(template.hex[x, y]);
                }
            }
        }

        // Reset all resource and dice number counts
        for (int index = 0; index < diceNumCounts.Length; index++)
        {
            diceNumCounts[index] = 0;
        }

        // Randomly assign available resource numbers to hexagons
        while (landHexes.Count > 0)
        {
            if (availableDiceNums.Count == 0)
            {
                availableDiceNums.AddRange(diceNums);
            }
            hexRandomNum = UnityEngine.Random.Range(0, landHexes.Count);
            diceRandomNum = UnityEngine.Random.Range(0, availableDiceNums.Count);
            if (landHexes[hexRandomNum].resource == 5)
            {
                landHexes.Remove(landHexes[hexRandomNum]);
                diceNumCounts[5] += 1;
            }
            else
            {
                landHexes[hexRandomNum].dice_number = availableDiceNums[diceRandomNum];
                diceNumCounts[availableDiceNums[diceRandomNum] - 2] += 1;
                landHexes[hexRandomNum].hex_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = DiceNumImages[availableDiceNums[diceRandomNum] - 2];
                landHexes.Remove(landHexes[hexRandomNum]);
                availableDiceNums.Remove(availableDiceNums[diceRandomNum]);
            }
            printAnyWarnings();
        }
        return template;
    }

    public HexTemplate SpawnBoard(HexTemplate template, bool spawningOffScreen)
    {
        float xOffset = 0.766f;
        float yOffset = 0.891f;
        float initial_y = 0.5f;
        float initial_x = 450;
        float initial_z = 100;

        if (spawningOffScreen)
        {
            initial_x += OFFSCREEN_X_OFFSET;
            initial_z += OFFSCREEN_Z_OFFSET;
        }

        ignoreWarnings = false;

        // Reset all resource and dice number counts
        for (int index = 0; index < resourceCounts.Length; index++)
        {
            resourceCounts[index] = 0;
        }
        for (int index = 0; index < diceNumCounts.Length; index++)
        {
            diceNumCounts[index] = 0;
        }

        if (template == null)
        {
            template = new HexTemplate();

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    template.hex[x, y] = new Hex(); // Be aware that these values could have change
                }
            }
        }

        //template = randomizeBoard(template, true, false, true);

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                hexPrefab.name = "hex " + x + "," + y;

                float zPos = y * yOffset + initial_z;
                if (x % 2 == 1 || x % 2 == -1)
                {
                    zPos += (yOffset * .5f);
                }

                switch (template.hex[x, y].resource)
                {
                    case 0:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, initial_y, zPos), Quaternion.Euler(90, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.black;
                        resourceCounts[0] += 1;
                        break;
                    case 1:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, initial_y, zPos), Quaternion.Euler(90, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.grey;
                        resourceCounts[1] += 1;
                        break;
                    case 2:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, initial_y, zPos), Quaternion.Euler(90, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.green;
                        resourceCounts[2] += 1;
                        break;
                    case 3:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, initial_y, zPos), Quaternion.Euler(90, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.white;
                        resourceCounts[3] += 1;
                        break;
                    case 4:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, initial_y, zPos), Quaternion.Euler(90, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.red;
                        resourceCounts[4] += 1;
                        break;
                    case 5:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, initial_y, zPos), Quaternion.Euler(90, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.yellow;
                        template.hex[x, y].hex_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = DiceNumImages[5];
                        template.hex[x, y].setDiceNum(7);
                        resourceCounts[5] += 1;
                        break;
                    default:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, initial_y, zPos), Quaternion.Euler(90, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.blue;
                        break;
                }
                template.hex[x, y].hex_go.name = x + "," + y;
                template.hex[x, y].hex_go.transform.SetParent(HexCanvas.transform);
                template.hex[x, y].hex_go.AddComponent<HexData>();
                template.hex[x, y].hex_go.GetComponent<HexData>().x_index = x;
                template.hex[x, y].hex_go.GetComponent<HexData>().y_index = y;

                if (template.hex[x, y].resource >= 0)
                {
                    if (template.hex[x, y].dice_number > 0)
                    {
                        template.hex[x, y].hex_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = DiceNumImages[template.hex[x, y].dice_number - 2];
                        diceNumCounts[template.hex[x, y].dice_number - 2] += 1;
                    }
                    else
                        template.hex[x, y].hex_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                }
                else
                {
                    template.hex[x, y].hex_go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;

                }
            }
        }

        // Loop through hexes creating ports if necessary
        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (template.hex[x, y].resource >= 0 && template.hex[x, y].portSide >= 0)
                {
                    GameObject[] hexes = getSurroundingHexes(x, y);

                    chooseHexForPort(template.hex[x, y].hex_go);
                    addPort(hexes[template.hex[x,y].portSide]);
                    template.hex[x, y].portGO.GetComponent<Renderer>().enabled = true;
                }
            }
        }
        return template;
    }

    public Sprite saveMapScreenShotOffScreen(HexTemplate template)
    {
        // Call retrieve map before calling this function

        screenShotCamera.transform.Translate(new Vector3(OFFSCREEN_X_OFFSET, OFFSCREEN_Z_OFFSET, 0));
        template = SpawnBoard(template, true);
        byte[] bytes = saveMapScreenShot(template.mapName, screenShotCamera);

        Texture2D texture = new Texture2D(SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH);
        texture.filterMode = FilterMode.Trilinear;
        texture.LoadImage(bytes);
        screenShots.Add(Sprite.Create(texture, new Rect(0, 0, SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH), new Vector2(0.5f, 0.0f), 1.0f));
        screenShotCamera.transform.Translate(new Vector3(-OFFSCREEN_X_OFFSET, -OFFSCREEN_Z_OFFSET, 0));

        return Sprite.Create(texture, new Rect(0, 0, SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH), new Vector2(0.5f, 0.0f), 1.0f);
    }

    public byte[] saveMapScreenShot(string mapName, Camera screenShotCamera)
    {
        errorsCanvas.enabled = false;

        RenderTexture rt = new RenderTexture(SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH, 24);
        screenShotCamera.targetTexture = rt;                                   //+++++++++++++++++++++++++++++++++++++
        Texture2D screenShot = new Texture2D(SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH, TextureFormat.RGB24, false);
        screenShotCamera.Render();                   //+++++++++++++++++++++++++++++++++++++
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH), 0, 0);
        screenShotCamera.targetTexture = null;                          //+++++++++++++++++++++++++++++++++++++
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/SavedMaps/" + mapName + ".png";  // +++++++++++++++++++++++++++
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
        boardCreationCanvas.gameObject.SetActive(false);

        return bytes;
    }

   public void goToGameLobby()
	{
      if (NavigationScript.networkGame)
      {
         NetworkManager networkObject = GameObject.Find("Network Handler").GetComponent<NetworkManager>(); // SILAS
         networkObject.setupGameSettings(numOfPlayers, turnTimerMax, victoryPoints, gameLobbyNameNetwork.text, maps[board_index].mapName, template); // SILAS
         UnityEngine.SceneManagement.SceneManager.LoadScene("Network Lobby");
      }
      else
         UnityEngine.SceneManagement.SceneManager.LoadScene("Character Select");

   }

	public void goToCharacterSelect()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Character Select");
	}
	
	public void netLobbyCanvasOn()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Network Lobby");
	}
	
	public void changeNumOfPlayers()
	{
		numOfPlayers = (int)numOfPlayersSlider.value;
		numOfPlayersValueText.text = numOfPlayers.ToString();
	}
	public void changeVP()
	{
		victoryPoints = (int)numOfVPSlider.value;
		numOfVPValueText.text = victoryPoints.ToString();
	}
	public void changeTimerValue()
	{
		turnTimerMax = (int)turnTimerSlider.value;
		turnTimerValueText.text = turnTimerMax.ToString();
	}

	public void toggleCharacterAbilities()
	{
		characterAbilitiesOn = characterAbilitiesToggle.isOn;
	}

	public void toggleTurnTimer()
	{
		turnTimerOn = turnTimerToggleLocal.isOn;
	}

	public void showConditionalButtons() //***********
	{
		if(NavigationScript.networkGame == true)
		{
			turnTimerToggleLocal.gameObject.SetActive(false);
			turnTimerToggleText.enabled = false;
			characterSelectButtonLocal.gameObject.SetActive(false);
			gameLobbyNameNetwork.gameObject.SetActive(true);
			gameLobbyButtonNetwork.enabled = true;
		}
		else
		{
			turnTimerToggleLocal.gameObject.SetActive(true);
			characterSelectButtonLocal.enabled = true;
			gameLobbyButtonNetwork.gameObject.SetActive(false);
			gameLobbyNameNetwork.gameObject.SetActive(false);
		}	
	}
}
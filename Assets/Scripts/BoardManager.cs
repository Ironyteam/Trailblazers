using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections.Generic;


public class BoardManager : MonoBehaviour
{
    public GameObject hexPrefab;
    public GameObject diceNumText;
    public GameObject ListBtn;
    public GameObject mapListPanelPG;
    public GameObject mapListPanelMOD;
    public GameObject portPrefab;

    private int numToAppendToName; // The number that will have to appended to the map name for it to be saved

    public Sprite[] DiceNumImages;

    private bool addPortEnabled = false;
    private bool choosingPort = false;

    private List<GameObject> hexesBorderingWater = new List<GameObject>();
    private Dictionary<GameObject, int> availablePortHexes = new Dictionary<GameObject, int>();

    private List<int> mapErrors = new List<int>();

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
	public Canvas createCanvasNetwork;
    public InputField mapNameField;       // Text field for naming a map that you are saving
    public Text mapNameTextMOD;           // Text box displaying the map names IN MODIFY BOARD SELCTION MENU
    public Text mapNameTextPG;            // Text box displaying the map names IN PRE GAME SELECTION MENU
    public Text warningText;
    public Text mapDetailsTextPG;
    public Text mapDetailsTextMOD;
    public Image MapScreenShortImageMOD;
    public Image MapScreenShortImagePG;

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
    public Text mapNameErrorText;
    public InputField newNameField;
    public Button confirmNewNameBtn;
	
	public Slider numOfPlayersSlider;
	public Slider numOfVPSlider;
	public Slider turnTimerSlider;

	public static int numOfPlayers = 2;
	public static int victoryPoints = 5;
	public static float turnTimerMax = 30;

    public const string DefaultMapsPath = FileHandler.DefaultMapsPath;
    public const string SavedMapsPath   = FileHandler.SavedMapsPath;

    public const int MAP_NAME_CONFLICT_ERROR = 1;
    public const int MISSING_DESERT_ERROR = 2;
    public const int MISSING_DICE_NUMS_ERROR = 3;
    public const int WARNINGS_ERROR = 4;

    public const int MAX_MAPS_WITH_SAME_NAME = 99;

    private bool ignoreWarnings = false;

    private int savedMapsStartindex;      // The index of the first saved (non-default) map in the array of maps
    private int board_index;              // The index of the current board being shown for selection

    public static bool startingGame = false; // Global variable to determine which canvas to show when board manager scene is loaded

    const int WIDTH  = HexTemplate.WIDTH;
    const int HEIGHT = HexTemplate.HEIGHT;

    const int LEFT  = -1;  // Index value sent to go left in the list of maps 
    const int RIGHT = -2;  // Index value sent to go right in the list of maps

    public const int SCREEN_SHOT_WIDTH = 800;
    public const int SCREEN_SHOT_LENGTH = 800;

    private int[] resourceCounts = new int[6];
    private int[] diceNumCounts = new int[11];

    private GameObject hexToReceivePort = null;
    private GameObject mousedOverHex = null;
    private GameObject portGO = null;

    private HexTemplate[] maps; // The paths of all available maps
    private Sprite[] screenShots;

    public static HexTemplate template = new HexTemplate();

    void Awake()
    {
        boardCreationCanvas.enabled = false;
        boardSelctionCanvasPG.enabled = false;
        boardSelctionCanvasMOD.enabled = false;
		createCanvasNetwork.enabled = false;
        errorsCanvas.enabled = false;
		createCanvasNetwork.enabled = false;

        if (startingGame == true)
            preGameBoardSelectOn();
        else
            MapEditorSelectOn();
        startingGame = false;
    }

    void Update()
    {
        // See if raycast hit an object
        Ray toMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
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

    public void preGameBoardSelectOn()
    {
        boardSelctionCanvasPG.enabled = true;
        displayMaps();
    }

    public void MapEditorSelectOn()
    {
        boardCreationCanvas.enabled    = false;
        boardSelctionCanvasPG.enabled  = false;
		createCanvasNetwork.enabled    = false;
        boardSelctionCanvasMOD.enabled = true;
		createCanvasNetwork.enabled    = false;
        displayMaps();
    }

    public void ReturnToMapEditorSelect()
    {
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
            template = reader.retrieveMap(DefaultMapsPath + "/" + maps[board_index].mapName + ".txt");
        else
            template = reader.retrieveMap(SavedMapsPath + "/" + maps[board_index].mapName + ".txt");
        createCanvasNetwork.enabled = true;
    }

    public void displayMaps()
    {
        List<string> mapNames = new List<string>();

        if (boardSelctionCanvasPG.enabled)
        {
            makeScrollingMapList(mapListPanelPG);

            foreach (HexTemplate template in maps)
            {
                mapNames.Add(template.mapName);
            }

            FileHandler reader = new FileHandler();
            screenShots = reader.getScreenShots(mapNames, savedMapsStartindex).ToArray();
            MapScreenShortImagePG.sprite = screenShots[board_index];

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

            FileHandler reader = new FileHandler();
            screenShots = reader.getScreenShots(mapNames, savedMapsStartindex).ToArray();
            MapScreenShortImageMOD.sprite = screenShots[board_index];

            mapNameTextMOD.text = maps[board_index].mapName;
            mapDetailsTextMOD.text = "Minimum Victory Points: " + maps[board_index].minVP + "\n" +
                                    "Maximum Victory Points: " + maps[board_index].maxVP;
        }

       
    }

    public void AddPortClicked()
    {
        addPortEnabled = true;
        currentResource = -1;
        currentDiceNum = -1;

        hexesBorderingWater = getHexesBorderedByWater(template);
    }

    public void chooseHexForPort(GameObject chosenHex)
    {
        GameObject[] hexes;
        int x = chosenHex.GetComponent<HexData>().x_index;
        int y = chosenHex.GetComponent<HexData>().y_index;

        Debug.Log("here");

        hexes = getSurroundingHexes(x, y);

        for (int index = 0; index < hexes.Length; index++)
        {
            if (template.hex[hexes[index].GetComponent<HexData>().x_index, hexes[index].GetComponent<HexData>().y_index].resource == -1)
               availablePortHexes.Add(hexes[index], index);
        }
        
        choosingPort = true;
        hexToReceivePort = chosenHex;
        portGO = Instantiate(portPrefab, new Vector3(0, 0.5F, 0), Quaternion.identity, HexCanvas.transform);
        Debug.Log("choosePort");
    }

    public void testNetworkFileHandling()
    {
        FileHandler handler = new FileHandler();
        string fileString = handler.ReadEntireMap(DefaultMapsPath + "/DefaultBoard1.txt");
        Debug.Log(fileString);
        handler.saveMap(fileString);
    }

    public int changeMouseOverPort(GameObject newHex)
    {
        int portSideIndex = -1;

        if (availablePortHexes.TryGetValue(newHex, out portSideIndex))
        {
            Vector3 hexTrans  = hexToReceivePort.transform.position;
            Vector3 portTrans;
            Quaternion newRotation;

            switch (portSideIndex)
            {
                
                case 0:
                    newRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    portGO.transform.rotation = newRotation;
                    portTrans = portGO.transform.position;
                    Debug.Log(hexTrans.z + " " + portTrans.z);
                    portGO.transform.Translate(new Vector3(hexTrans.x - portTrans.x, hexTrans.z - portTrans.z + 0.7f, 0));
                    Debug.Log(portSideIndex);
                    break;
                case 1:
                    newRotation = Quaternion.Euler(new Vector3(90, 0, -60));
                    portGO.transform.rotation = newRotation;
                    portTrans = portGO.transform.position;
                    portGO.transform.Translate(new Vector3(hexTrans.x - portTrans.x + .6f, hexTrans.z - portTrans.z + 1.0f, 0));
                    Debug.Log(portSideIndex);
                    break;
                case 2:
                    newRotation = Quaternion.Euler(new Vector3(90, 0, -120));
                    portGO.transform.rotation = newRotation;
                    portTrans = portGO.transform.position;
                    portGO.transform.Translate(new Vector3(hexTrans.x - portTrans.x - .6f, hexTrans.z - portTrans.z - 1.0f, 0));
                    Debug.Log(portSideIndex);
                    break;
                case 3:
                    newRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    portGO.transform.rotation = newRotation;
                    portTrans = portGO.transform.position;
                    portGO.transform.Translate(new Vector3(hexTrans.x - portTrans.x, hexTrans.z - portTrans.z - 0.7f, 0));
                    Debug.Log(portSideIndex);
                    break;
                case 4:
                    newRotation = Quaternion.Euler(new Vector3(90, 0, 120));
                    portGO.transform.rotation = newRotation;
                    portTrans = portGO.transform.position;
                    portGO.transform.Translate(new Vector3(hexTrans.x - portTrans.x + .6f, hexTrans.z - portTrans.z - 1.0f, 0));
                    Debug.Log(portSideIndex);
                    break;
                case 5:
                    newRotation = Quaternion.Euler(new Vector3(90, 0, 60));
                    portGO.transform.rotation = newRotation;
                    portTrans = portGO.transform.position;
                    portGO.transform.Translate(new Vector3(hexTrans.x - portTrans.x - .6f, hexTrans.z - portTrans.z + 1.0f, 0));
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

        //chosenHex.GetComponentInChildren<Renderer>().material.color = Color.magenta;
        resetPortAddingChanges();
    }

    private void resetPortAddingChanges()
    {
        mousedOverHex = null;
        portGO = null;
        hexToReceivePort = null;
        availablePortHexes.Clear();
        hexesBorderingWater.Clear();
        addPortEnabled = false;
        choosingPort = false;
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
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.cyan;
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
            ignoreAllWarnings();
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

        newNameField.gameObject.SetActive(false);
        
        confirmNewNameBtn.gameObject.SetActive(true);
        newNameField.gameObject.SetActive(true);
        mapNameErrorText.text = "Please Enter a new name";
         
    }

    public void appendNameAndSaveMap()
    {
        name = mapNameField.text;
        mapNameField.text = name + "(" + numToAppendToName + ")";
        SaveBoard();
    }

    public void confirmNewName()
    {
        string newName = newNameField.text;

        if (newName != "")
        {
            mapNameField.text = newName;
            SaveBoard();
        }
    }

    private void displayAnyErrors(List<int> errors, string mapName)
    {
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
        mapNameErrorText.enabled = false;
        missingDesertText.enabled = false;
        missingDiceNumsText.enabled = false;
        warningsErrorText.enabled = false;
        newNameField.gameObject.SetActive(false);

        // Handle map name error when it is the only error left
        if (errors.Count == 1 && errors.Contains(MAP_NAME_CONFLICT_ERROR))
        {
            List<HexTemplate> maps = new List<HexTemplate>();
            FileHandler fileChecker = new FileHandler();
            int savedMapsStartindex = 0;
            bool numberUnused = true;

            // Search for an number that has not yet been appended onto
            // the same name
            maps = fileChecker.getAllMaps(out savedMapsStartindex);
            int index;
            for (index = 2; index <= MAX_MAPS_WITH_SAME_NAME; index++)
            {
                numberUnused = true;
                for (int i = savedMapsStartindex; i < maps.Count; i++)
                {
                    if (String.Compare(maps[i].mapName, mapName + "(" + index + ")") == 0)
                    {
                        numberUnused = false;
                        i = maps.Count;
                    }
                }

                if (numberUnused)
                {
                    numToAppendToName = index;
                    index = MAX_MAPS_WITH_SAME_NAME + 1;
                }
            }
            mapNameErrorText.enabled = true;

            // If a valid within the limit was found display choice message
            if (numberUnused)
            {
                appendAndSaveBtn.gameObject.SetActive(true);
                enterNewNameBtn.gameObject.SetActive(true);
                mapNameErrorText.text = "Error: Map already exists with the name " + mapName + "." +
                                      "\nMap will be saved as " + mapName + "(" + numToAppendToName + ").";
            }
            else
            {
                confirmNewNameBtn.gameObject.SetActive(false);
                newNameField.gameObject.SetActive(true);
                mapNameErrorText.text = "Error: This map name has been used too many times." +
                                      "\nPlease enter a different name.";
            }                        
        }
        else
        {
            autoFixAllBtn.gameObject.SetActive(true);
            returnAndFixBtn.gameObject.SetActive(true);

            if (errors.Contains(MISSING_DESERT_ERROR))
            {
                addRandomDesertBtn.gameObject.SetActive(true);
                missingDesertText.enabled = true;
                missingDesertText.text =
                    "Error: This map contains no desert resource. A desert resource is required.";
            }

            if (errors.Contains(MISSING_DICE_NUMS_ERROR))
            {
                addRandomDiceNumsBtn.gameObject.SetActive(true); ;
                missingDiceNumsText.enabled = true;
                missingDiceNumsText.text = "Error: Not all active map tiles have dice numbers assigned.";
            }

            if (errors.Contains(WARNINGS_ERROR))
            {
                ignoreWarningsBtn.gameObject.SetActive(true);
                warningsErrorText.enabled = true;
                warningsErrorText.text = "Error: Some unused resources and dice numbers. Gameplay will be affected.";
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
        boardSelctionCanvasPG.enabled = false;
        boardSelctionCanvasMOD.enabled = false;
        boardCreationCanvas.enabled = true;
        boardCreationCanvas.gameObject.SetActive(true);
        mapNameField.text = "";
        SpawnBoard(null);
    }

    public void ModifyMap()
    {
        boardSelctionCanvasMOD.enabled = false;
        boardCreationCanvas.enabled = true;
        boardCreationCanvas.gameObject.SetActive(true);
        mapNameField.text = "";
        FileHandler reader = new FileHandler();
        if (board_index < savedMapsStartindex)
            template = reader.retrieveMap(DefaultMapsPath + "/" + maps[board_index].mapName + ".txt");
        else
            template = reader.retrieveMap(SavedMapsPath + "/" + maps[board_index].mapName + ".txt");
        SpawnBoard(template);
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

    public void changeHex(int x, int y)
    {
        if (template.hex[x, y].resource == -1 && template.hex[x, y].hexOwningPort != null)
            deletePort(x, y);

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
        if (desiredIndex == RIGHT && board_index < (maps.Length - 1))
            board_index += 1;
        else if (desiredIndex == LEFT && board_index > 0)
            board_index -= 1;
        else if (desiredIndex >= 0 && desiredIndex < maps.Length)
            board_index = desiredIndex;

        if (boardSelctionCanvasPG.enabled)
        {
            NetworkManager networkObject = GameObject.Find("Network Handler").GetComponent<NetworkManager>(); // SILAS
            networkObject.MapName = maps[board_index].mapName; // SILAS
            mapNameTextPG.text = maps[board_index].mapName;
            mapDetailsTextPG.text = "Minimum Victory Points: " + maps[board_index].minVP + "\n" +
                                    "Maximum Victory Points: " + maps[board_index].maxVP;
            if (screenShots[board_index] != null)
                MapScreenShortImagePG.sprite = screenShots[board_index];
            else
                MapScreenShortImagePG.sprite = null;

        }
        else
        {
            mapNameTextMOD.text = maps[board_index].mapName;
            mapDetailsTextMOD.text = "Minimum Victory Points: " + maps[board_index].minVP + "\n" +
                                    "Maximum Victory Points: " + maps[board_index].maxVP;
            if (screenShots[board_index] != null)
                MapScreenShortImageMOD.sprite = screenShots[board_index];
            else
                MapScreenShortImageMOD.sprite = null;
        }
        
    }

    private List<int> getAnyErrors(string newMapName, string warnings)
    {
        List<int> errors = new List<int>();
        List<HexTemplate> maps = new List<HexTemplate>();
        FileHandler fileChecker = new FileHandler();
        int savedMapsStartindex = 0;

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

    public void SaveBoard()                 // Determine why boards are getting saved twice if you choose to auto fix all errors
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
                saveMapScreenShot();
                
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
        randomizeBoard(template, false, true, false);
    }

    private HexTemplate randomizeBoard(HexTemplate template, bool randomizeResources,
                                       bool randomizeDiceNums, bool completedBoard)
    {
        int randomNum;
        Hex desertHex = null; // Desert hex that is temporarily removed from list during randomization
        List<Hex> landHexes = new List<Hex>();

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

        // Randomize a completed board according to the number of each resource
        // and the number of each dice number in original
        if (completedBoard)
        {
            if (randomizeResources == true)
            {
                List<int> availableResourceNums = new List<int>(); // List from which numbers will be drawn

                // Gather all resources in the original template
                foreach (Hex hex in landHexes)
                {
                    if (hex.resource != 5)
                    {
                        availableResourceNums.Add(hex.resource);
                    }
                }

                // Randomly choose a hexagon to be desert and remove it from the list
                // temporarily in order to preserve it
                randomNum = UnityEngine.Random.Range(0, availableResourceNums.Count);
                landHexes[randomNum].resource = 5;
                landHexes[randomNum].setDiceNum(7);
                desertHex = landHexes[randomNum];
                landHexes.Remove(desertHex);

                // Randomly assign available resource numbers to hexagons
                foreach (Hex hex in landHexes)
                {
                    if (availableResourceNums.Count > 0)
                    {
                        randomNum = UnityEngine.Random.Range(0, availableResourceNums.Count);
                        hex.resource = availableResourceNums[randomNum];
                        availableResourceNums.Remove(availableResourceNums[randomNum]);
                    }
                    else
                    {
                        Debug.Log("Error assigning resource numbers randomly: In fucntion randomizeBoard.");
                    }
                }
                landHexes.Add(desertHex);
            }


            if (randomizeDiceNums == true)
            {
                List<int> availableDiceNums = new List<int>(); // List from which numbers will be drawn

                // Gather all dice numbers in the original template
                foreach (Hex hex in landHexes)
                {
                    if (hex.resource == 5)
                    {
                        desertHex = hex;
                    }
                    else
                    {
                        availableDiceNums.Add(hex.dice_number);
                    }
                }

                // Temporarily remove desert hexagon to preserve dice number
                landHexes.Remove(desertHex);

                // Randomly assign available dice numbers to hexagons
                foreach (Hex hex in landHexes)
                {
                    if (availableDiceNums.Count > 0)
                    {
                        randomNum = UnityEngine.Random.Range(0, availableDiceNums.Count);
                        hex.dice_number = availableDiceNums[randomNum];
                        availableDiceNums.Remove(availableDiceNums[randomNum]);
                    }
                    else
                    {
                        Debug.Log("Error assigning dice numbers randomly: In fucntion randomizeBoard.");
                    }
                }
                landHexes.Add(desertHex);
            }
        }

        // Randomize dice numbers without regard to current setup (completedBoard == false)
        else
        {
            if (randomizeDiceNums == true)
            {
                int hexRandomNum;
                int diceRandomNum;
                List<int> diceNums = new List<int>()
                {
                    2, 3, 4, 5, 6, 8, 9, 10, 11, 12
                };
                List<int> availableDiceNums = new List<int>(diceNums);

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
            }
        }
        return template;
    }

    public void SpawnBoard(HexTemplate template)
    {
        float xOffset = 0.766f;
        float yOffset = 0.891f;
        float initial_x = 450;
        float initial_y = 0.5f;
        float initial_z = 100;

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
                    //template.hex[x, y].hex_go.transform.GetChild(0).gameObject.AddComponent<Texture>();
                    //template.hex[x, y].hex_go.transform.GetChild(0).gameObject.GetComponent<Texture>(). = port;

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

                    hexes[template.hex[x, y].portSide].GetComponentInChildren<Renderer>().material.color = Color.magenta;
                    addPort(hexes[template.hex[x, y].portSide]);
                }
            }
        }
        printAnyWarnings();
        BoardManager.template = template;
    }

    public void saveMapScreenShot()
    {
        string mapName = mapNameField.text;

        errorsCanvas.enabled = false;

        RenderTexture rt = new RenderTexture(SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH, 24);
        Camera.main.targetTexture = rt;
        Texture2D screenShot = new Texture2D(SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = "Assets/SavedMaps/" + mapName + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

    public void goToGameLobby()
	{	
		UnityEngine.SceneManagement.SceneManager.LoadScene("Network Lobby");
	}
	
	public void netLobbyCanvasOn()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(3);
	}
	
	public void changeNumOfPlayers()
	{
		numOfPlayers = (int)numOfPlayersSlider.value;
	}
	public void changeVP()
	{
		victoryPoints = (int)numOfVPSlider.value;
	}
	public void changeTimerValue()
	{
		turnTimerMax = turnTimerSlider.value;
	}
}
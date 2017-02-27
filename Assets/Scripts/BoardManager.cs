using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

// variables on line 286 and 287 (initial_x and intial_y)
// determine where the first bottom left hex is spawned for the modifiable game board

public class BoardManager : MonoBehaviour
{
    public GameObject hexPrefab;
    public GameObject diceNumText;
    public GameObject ListBtn;
    public GameObject mapListPanelPG;
    public GameObject mapListPanelMOD;

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

    public const string DefaultMapsPath = FileHandler.DefaultMapsPath;
    public const string SavedMapsPath   = FileHandler.SavedMapsPath;

    private int savedMapsStartindex;      // The index of the first saved (non-default) map in the array of maps
    private int board_index;              // The index of the current board being shown for selection

    public static bool startingGame = false; // Global variable to determine which canvas to show when board manager scene is loaded

    const int WIDTH  = HexTemplate.WIDTH;
    const int HEIGHT = HexTemplate.HEIGHT;

    const int LEFT  = -1;  // Index value sent to go left in the list of maps 
    const int RIGHT = -2;  // Index value sent to go right in the list of maps

    private string[] maps; // The paths of all available maps

    public static HexTemplate template = new HexTemplate();
    private Text[,] diceNumbers = new Text[WIDTH, HEIGHT];

    void Awake()
    {
        boardCreationCanvas.enabled = false;
        boardSelctionCanvasPG.enabled = false;
        boardSelctionCanvasMOD.enabled = false;
		createCanvasNetwork.enabled = false;
        if (startingGame == true)
            preGameBoardSelectOn();
        else
            MapEditorSelectOn();
        startingGame = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray toMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rhInfo;
            bool didHit = Physics.Raycast(toMouse, out rhInfo, 500.0f);

            if (didHit)
            {
                GameObject ourHitObject = rhInfo.collider.transform.gameObject;

                if (currentResource != -1)
                    changeHex(ourHitObject.GetComponent<HexData>().x_index,
                              ourHitObject.GetComponent<HexData>().y_index);
                else if (currentDiceNum != -1)
                    changeDiceNumber(ourHitObject.GetComponent<HexData>().x_index,
                                 ourHitObject.GetComponent<HexData>().y_index);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Ray toMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rhInfo;
            bool didHit = Physics.Raycast(toMouse, out rhInfo, 500.0f);

            if (didHit)
            {
                GameObject ourHitObject = rhInfo.collider.transform.gameObject;

                resetHex(ourHitObject.GetComponent<HexData>().x_index,
                          ourHitObject.GetComponent<HexData>().y_index);
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
        displayMaps();
    }

    public void ReturnToMapEditorSelect()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Destroy(template.hex[x, y].hex_go);
            }
        }
        boardCreationCanvas.enabled = false;
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
            template = reader.retrieveMap(DefaultMapsPath + "/" + maps[board_index] + ".txt");
        else
            template = reader.retrieveMap(SavedMapsPath + "/" + maps[board_index] + ".txt");
        createCanvasNetwork.enabled = true;
    }

    public void displayMaps()
    {
        if (boardSelctionCanvasPG.enabled)
        {
            makeScrollingMapList(mapListPanelPG);
            mapNameTextPG.text = maps[board_index];
        }
        else if (boardSelctionCanvasMOD.enabled)
        {
            makeScrollingMapList(mapListPanelMOD);
            mapNameTextMOD.text = maps[board_index];
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
            tempGO.transform.SetParent(panel.transform);

            int desiredIndex = index;

            mapBtns.Add(tempGO);
            tempGO.GetComponent<Button>().GetComponentInChildren<Text>().text = maps[index];
            tempGO.GetComponent<Button>().onClick.AddListener(() => { ChangeDisplayedMap(desiredIndex); });
        }
    }

    public void CreateNewBoard()
    {
        boardSelctionCanvasPG.enabled = false;
        boardSelctionCanvasMOD.enabled = false;
        boardCreationCanvas.enabled = true;
        SpawnBoard(null);
    }

    public void ModifyMap()
    {
        boardSelctionCanvasMOD.enabled = false;
        boardCreationCanvas.enabled = true;
        FileHandler reader = new FileHandler();
        if (board_index < savedMapsStartindex)
            template = reader.retrieveMap(DefaultMapsPath + "/" + maps[board_index] + ".txt");
        else
            template = reader.retrieveMap(SavedMapsPath + "/" + maps[board_index] + ".txt");
        SpawnBoard(template);
    }

    public void changeSelectedResource(int resrouceNum)
    {
        currentResource = resrouceNum;
        currentDiceNum  = -1;
    }

    public void resetHex(int x, int y)
    {
        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.blue;
        template.hex[x, y].setResource(-1);

    }

    public void changeHex(int x, int y)
    {
        switch (currentResource)
        {
            case 0:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.black;
                template.hex[x, y].setResource(currentResource);
                break;
            case 1:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.grey;
                template.hex[x, y].setResource(currentResource);
                break;
            case 2:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.green;
                template.hex[x, y].setResource(currentResource);
                break;
            case 3:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.white;
                template.hex[x, y].setResource(currentResource);
                break;
            case 4:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.red;
                template.hex[x, y].setResource(currentResource);
                break;
            case 5:
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.yellow;
                template.hex[x, y].setResource(currentResource);
                break;
        }
    }

    public void changeDiceNumber(int x, int y)
    {
        template.hex[x, y].setDiceNum(currentDiceNum);
        Debug.Log(template.hex[x, y].dice_number);
    }

    public void changeSelectedDiceNum(int diceNum)
    {
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
           mapNameTextPG.text = maps[board_index];
        else
            mapNameTextMOD.text = maps[board_index];
    }

    public void SaveBoard()
    {
        string mapName;

        mapName = mapNameField.text;

        FileHandler writer = new FileHandler();
        writer.saveMap(template, mapName, "Bob");                    // Get user name for creator parameter
        ReturnToMapEditorSelect();
    }

    public void SpawnBoard(HexTemplate template)
    {
        float xOffset = 13.9f;
        float yOffset = 15.8f;
        float initial_x = 450;
        float initial_y = 100;

        if (template == null)
        {
            template = new HexTemplate();
            bool[] portSides = { false, false, false, false, false, false };

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    template.hex[x, y] = new Hex(-1, 2, portSides, x, y); // Be aware that these values coul have change
                }
            }
        }

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                hexPrefab.name = "hex " + x + "," + y;

                float yPos = y * yOffset + initial_y;
                if (x % 2 == 1 || x % 2 == -1)
                {
                    yPos += (yOffset * .5f);
                }

                switch (template.hex[x, y].resource)
                {
                    case 0:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, yPos, 0), Quaternion.Euler(0, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.black;
                        break;
                    case 1:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, yPos, 0), Quaternion.Euler(0, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.grey;
                        break;
                    case 2:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, yPos, 0), Quaternion.Euler(0, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.yellow;
                        break;
                    case 3:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, yPos, 0), Quaternion.Euler(0, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.white;
                        break;
                    case 4:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, yPos, 0), Quaternion.Euler(0, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.red;
                        break;
                    case 5:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, yPos, 0), Quaternion.Euler(0, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.green;
                        break;
                    default:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, yPos, 0), Quaternion.Euler(0, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.blue;
                        break;
                }
                template.hex[x, y].hex_go.name = x + "," + y;
                template.hex[x, y].hex_go.transform.SetParent(HexCanvas.transform);
                template.hex[x, y].hex_go.AddComponent<HexData>();
                template.hex[x, y].hex_go.GetComponent<HexData>().x_index = x;
                template.hex[x, y].hex_go.GetComponent<HexData>().y_index = y;
            }
        }
        /*GameObject text = Instantiate(diceNumText, new Vector3(initial_x, initial_y, 1), Quaternion.identity);
        text.transform.SetParent(HexCanvas.transform);
        diceNumbers[0, 0] = text.GetComponent<Text>();
        diceNumbers[0, 0].text = "2";*/
        BoardManager.template = template;
    }
	
	public void goToGameLobby()
	{	
		UnityEngine.SceneManagement.SceneManager.LoadScene(4);
	}
	
	public void netLobbyCanvasOn()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(3);
	}
}
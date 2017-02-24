using UnityEngine;
//using System.Collections;
using UnityEngine.UI;
//using System.IO;

// Consider changing z to y because we are working in 2d
public class BoardManager : MonoBehaviour
{
    public GameObject hexPrefab;
    public GameObject diceNumText;
    public GameObject ListBtn;
    public GameObject mapListPanelPG;


    private Button[] mapBtns;
   // public GameObject mapsListGO;

    private int currentResource = -1;
    private int currentDiceNum  = -1;

    public Canvas boardDecisionCanvas;
    public Canvas boardCreationCanvas;
    public Canvas boardSelctionCanvasMOD; // The board selection canvas for MODifying a board
    public Canvas boardSelctionCanvasPG;  // The board selection canvas for Pre Game selection
    public Canvas HexCanvas;
	public Canvas createGameCanvas;
    public InputField mapNameField;
    public Text mapNameTextMOD;           // Text box displaying the map names IN MODIFY BOARD SELCTION MENU
    public Text mapNameTextPG;            // Text box displaying the map names IN PRE GAME SELECTION MENU

    public const string DefaultMapsPath = FileHandler.DefaultMapsPath;
    public const string SavedMapsPath = FileHandler.SavedMapsPath;

    private int savedMapsStartindex;
    private int board_index;

    public static bool startingGame = false;

    const int WIDTH = HexTemplate.WIDTH;
    const int HEIGHT = HexTemplate.HEIGHT;

    const int LEFT  = -1;
    const int RIGHT = -2;

    private string[] maps;

    const int ALL_BOARDS = 1;
    const int DEFAULT_BOARDS = 2;
    const int SAVED_BOARDS = 3;

    public static HexTemplate template = new HexTemplate();
    private Text[,] diceNumbers = new Text[WIDTH, HEIGHT];



    void Awake()
    {
        boardCreationCanvas.enabled = false;
        boardSelctionCanvasPG.enabled = false;
        boardSelctionCanvasMOD.enabled = false;
		createGameCanvas.enabled = false;
        boardDecisionCanvas.enabled = true;
        if (startingGame == true)
        {
            preGameBoardSelectOn();
            startingGame = false;
        }
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

    public void CreateNewBoard()
    {
        boardDecisionCanvas.enabled = false;
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
                template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.yellow;
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

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Destroy(template.hex[x, y].hex_go);
            }
        }

        boardCreationCanvas.enabled = false;
        boardDecisionCanvas.enabled = true;
    }

    public void preGameBoardSelectOn()
    {
        boardDecisionCanvas.enabled = false;
        boardSelctionCanvasPG.enabled = true;
        displayMaps();
    }

    public void ModifyBoardSelectOn()
    {
        boardDecisionCanvas.enabled = false;
        boardSelctionCanvasMOD.enabled = true;
        displayMaps();
    }

    public void displayMaps()
    {
        FileHandler reader = new FileHandler();
        float xPos;
        float nextyPos;
        float verticalGap;
		//int   desiredIndex;

        maps = reader.getAllMaps(out savedMapsStartindex).ToArray();
        board_index = 0;

        RectTransform PanelRT = (RectTransform)mapListPanelPG.transform;
        
        Vector3 panelPosition = mapListPanelPG.transform.position;
        xPos        = panelPosition.x;
        nextyPos    = panelPosition.y;
        verticalGap = 30;

        for (int index = 0; index < maps.Length; index++)
        {
            GameObject tempGO = Instantiate(ListBtn, new Vector3(xPos , nextyPos + (PanelRT.rect.height * .3F), 0), Quaternion.identity, mapListPanelPG.transform);

			int desiredIndex = index;
			
            Button tempBtn = tempGO.GetComponent<Button>();
            tempBtn.onClick.AddListener(() => { ChangeDisplayedMap(desiredIndex); });
            Text BtnText = tempBtn.GetComponentInChildren<Text>();            BtnText.text = maps[index];

            nextyPos -= verticalGap;
        }


        if (boardSelctionCanvasPG.enabled)
           mapNameTextPG.text = maps[board_index];
        else
            mapNameTextMOD.text = maps[board_index];

}

    public void startGame()
    {
        boardSelctionCanvasPG.enabled = false;
        FileHandler reader = new FileHandler();
        if (board_index < savedMapsStartindex)
            template = reader.retrieveMap(DefaultMapsPath + "/" + maps[board_index] + ".txt");
        else
            template = reader.retrieveMap(SavedMapsPath + "/" + maps[board_index] + ".txt");
        createGameCanvas.enabled = true;
    }
	

    public void SpawnBoard(HexTemplate template)
    {
        float xOffset = 13.9f;
        float yOffset = 15.8f;
        float initial_x = 240;
        float initial_y = 50;

        if (template == null)
        {
            template = new HexTemplate();
            bool[] portSides = { false, false, false, false, false, false };

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    template.hex[x, y] = new Hex(-1, 2, portSides); // Be aware that these values coul have change
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
                    default:
                        template.hex[x, y].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset + initial_x, yPos, 0), Quaternion.Euler(0, 0, 30)); ;
                        template.hex[x, y].hex_go.GetComponentInChildren<Renderer>().material.color = Color.blue;
                        break;
                }
                template.hex[x, y].hex_go.name = x + "," + y;
                template.hex[x, y].hex_go.transform.SetParent(HexCanvas.transform);
                //template.hex[x, y].hex_go.AddComponent<Canvas>();

                template.hex[x, y].hex_go.AddComponent<HexData>();
                template.hex[x, y].hex_go.GetComponent<HexData>().x_index = x;
                template.hex[x, y].hex_go.GetComponent<HexData>().y_index = y;
            }
        }
        GameObject text = Instantiate(diceNumText, new Vector3(initial_x, initial_y, 1), Quaternion.identity);
        text.transform.SetParent(HexCanvas.transform);
        diceNumbers[0, 0] = text.GetComponent<Text>();
        diceNumbers[0, 0].text = "2";
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


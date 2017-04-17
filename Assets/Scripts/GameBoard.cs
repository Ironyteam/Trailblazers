using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{

	public GameObject[] hexPrefabs;
	public GameObject hexBoard;
	public GameObject beach;
	public GameObject roadRect;
	public GameObject structure;
	public GameObject city;
	public GameObject boat;
	public GameObject dock;
	public GameObject token;
	public GameObject armySprite;
	public GameObject armyText;

	public const int WIDTH = HexTemplate.WIDTH;
	public const int HEIGHT = HexTemplate.HEIGHT;

	public HexTemplate template = new HexTemplate();

	float xOffset = 2.9f;
	float zOffset = 3.4f;
	float zPos;

    Tutorial tutorial;
	public DiceRollerScript diceRoller;

	public List<Road> Roads			  = new List<Road>();
	public List<Structure> Structures = new List<Structure>();
	public List<GameObject> Tokens    = new List<GameObject>();
	public Coordinate hexCoordinate;
	public Coordinate tempCoordinate = new Coordinate(0, 0);
	public Coordinate newCoordinateA;
	public Coordinate newCoordinateB;
	public Structure newStructure;
	public Road newRoad;
	public int structureIndex = 0;
	public int roadIndex = 0;
	public MKGlow MKGlowObject;
	public GuiManager GUIManager;
    public NetworkManager NetManager;
	public audioManager AudioManager;
    public int glowCounter = 0;

	public int CurrentPlayer = 0;
    public int LocalPlayer = 0;
    public float timeLeft;
	public Color timerRed = new Color(1, 0, 0);
	public Color timerWhite = new Color(1, 1, 1);
    public Text timerTextObject;
    public bool timerCoroutineStarted = false;
    public bool InitialPlacement = true;
	public bool FirstTurn = true;
	public bool goingUp = true;
	public Game LocalGame;

	public bool BuildSettlementButtonClicked = false;
	public bool BuildRoadButtonClicked = false;
	public bool BuildCityButtonClicked = false;
	public bool BuyingArmy = false;
	public bool Attacking = false;

	public Structure AttackingCity = null;
	public Structure DefendingCity = null;
    public Structure BuyingArmyCity = null;
    public Hex RobberLocation = null;
   /*
  sendEndTurn()
  sendStartTurn()
  sendSendChat(int chatMessageNumber)

    */
	void Start()
	{
		MKGlowObject = GameObject.Find("Main Camera").GetComponent<MKGlow>();
		GUIManager = GameObject.Find("Main Camera").GetComponent<GuiManager>();
		AudioManager = GameObject.Find("MUSIC").GetComponent<audioManager>();
        MKGlowObject.BlurSpread = .125f;
		MKGlowObject.BlurIterations = 3;
		MKGlowObject.Samples = 4;
        GUIManager.DisableGameCanvas();

		LocalGame = new Game();

        LocalGame.VictoryPointsToWin = BoardManager.victoryPoints;
        LocalGame.isNetwork = NavigationScript.networkGame;

        if (LocalGame.isNetwork)
            LocalPlayer = BoardManager.localPlayerIndex;

        timerTextObject = GameObject.Find("turnTimerText").GetComponent<Text>();

        if (BoardManager.turnTimerOn == false)
        {
            timerTextObject.gameObject.SetActive(false);
        }

        if (LocalGame.isNetwork)
			NetManager = GameObject.Find("Network Handler").GetComponent<NetworkManager>();

        foreach (int selectedCharacter in characterSelect.selectedCharacters)
		    LocalGame.PlayerList.Add(new Player(Characters.Names[selectedCharacter], BoardManager.characterAbilitiesOn ? selectedCharacter:-1));

		foreach (Player currentPlayer in LocalGame.PlayerList) 
		{
			if (currentPlayer.playerAbility == 3) 
			{
				currentPlayer.Wood = 2;
				currentPlayer.Ore = 2;
				currentPlayer.Wheat = 2;
				currentPlayer.Wool = 2;
				currentPlayer.Brick = 2;
			}
		}

		template = BoardManager.template;

		for (int z = 0; z < HEIGHT; z++)
		{
			for (int x = 0; x < WIDTH; x++)
			{
				// hexPrefab.name = "hex " + x + "," + z;

				zPos = z * zOffset;
				if (x % 2 == 1 || x % 2 == -1)
				{
					zPos += (zOffset * .5f);
				}

				if (template.hex[x, z].resource != -1) 
				{
					// Structure 1
					// if x is odd OR x-1, y != resource	
					if (x % 2 == 1 ||  x == 0 || template.hex [x-1, z].resource == -1)
					{
						// Spawn Structure 1
						spawnStructureOne(x, z);
					}
							
					// Road 1 - Always spawn
					spawnRoadOne(x, z);

					// Structure 2 - Always spawn
					spawnStructureTwo(x, z);

					// Road 2 - Always spawn
					spawnRoadTwo(x, z);

					// Structure 3 
					// if x is odd OR x+1, y-1 != resource
					if (x % 2 == 1 || z == 0 || template.hex [x+1, z-1].resource == -1) 
					{
						// Spawn Structure 3
						spawnStructureThree(x, z);
						
						// Spawn Road 3, same conditions apply
						spawnRoadThree(x, z);
					}


					//Structure 4
					if (x % 2 != 1) // x is even
					{   // if x+1, y-1 != resource && x, y-1 != resource
						if (z == 0 || template.hex [x+1, z-1].resource == -1 && template.hex [x, z-1].resource == -1) 
						{
							// Spawn Structure 4
							spawnStructureFour(x, z);
						}
					} 
					else // x is odd
					{	// if x, y-1 != resource
						if (z == 0 || template.hex [x, z-1].resource == -1)
						{
							// Spawn Structure 4
							spawnStructureFour(x, z);
						}
					}

					// Road 4
					// if x, y-1 != resource
					if (z == 0 || template.hex [x, z-1].resource == -1)
					{
						// Spawn Road 4
						spawnRoadFour(x, z);
					}

					// Structure 5
					// if x, y-1 != resource
					if (z == 0 || template.hex [x, z-1].resource == -1)
					{	// x is even
						if (x % 2 != 1)
						{	// First hexagon on board
							if ((x == 0 && z == 0)  || (x != 0 && z == 0) || (x == 0 && z != 0))
							{
								// Spawn Structure 5
								spawnStructureFive(x, z);
							}
							else if (x != 0 && z != 0)
							{
								// if x-1, y-1 != resource
								if (template.hex [x-1, z-1].resource == -1)
								{
									// Spawn Structure 5
									spawnStructureFive(x, z);
								}
							}

						}
						// x is odd
						else 
						{ 
							// if x-1, y != resource
							if (template.hex [x-1, z].resource == -1)
							{
								// Spawn Structure 5
								spawnStructureFive(x, z);
							}
						}
					}

					// Road 5
					// x is even
					if (x % 2 != 1)
					{	// First hexagon on board
						if ((x == 0 && z == 0)  || (x != 0 && z == 0) || (x == 0 && z != 0))
						{
							// Spawn Road 5
							spawnRoadFive(x, z);
						}
						else if (x != 0 && z != 0)
						{
							// if x-1, y-1 != resource
							if (template.hex [x-1, z-1].resource == -1)
							{
								// Spawn Road 5
								spawnRoadFive(x, z);
							}
						}

					}
					// x is odd
					else 
					{ 
						// if x-1, y != resource
						if (template.hex [x-1, z].resource == -1)
						{
							// Spawn Road 5
							spawnRoadFive(x, z);
						}
					}



					//Structure 6
					// x is even
					if (x % 2 != 1)
					{	// First hexagon on board
						if ((x == 0 && z == 0) || (x == 0 && z != 0))
						{
							// Spawn Structure 6
							spawnStructureSix(x, z);
						}
						else if (x != 0 && z != 0)
						{
							// if x-1, y-1 != resource
							if (template.hex [x-1, z-1].resource == -1 && template.hex [x-1, z].resource == -1)
							{
								// Spawn Structure 6
								spawnStructureSix(x, z);
							}
						}

					}
					// x is odd
					else
					{
						// x-1, y != resource
						if (template.hex [x-1, z].resource == -1)
						{
							// Spawn Structure 6
							spawnStructureSix(x, z);
						}
					}

					//Road 6
					// if x is odd OR x-1, y != resource
					if (x % 2 == 1 ||  x == 0 || template.hex [x-1, z].resource == -1)
					{
						// Spawn Road 6
						spawnRoadSix(x, z);
					}

				}

				if (template.hex[x, z].resource >= 0)
				{
					template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefabs[template.hex[x, z].resource], new Vector3(x * xOffset, .2f, zPos), Quaternion.Euler(0, 30, 0));
					Instantiate(beach, new Vector3(x * xOffset, -.2f, zPos), Quaternion.Euler(0, 30, 0));
					template.hex[x, z].hex_go.name = x + "," + z;
					template.hex[x, z].token_go = (GameObject)Instantiate(token, new Vector3(x * xOffset, 2f, zPos), Quaternion.Euler(0, -20, 0));
					Tokens.Add(template.hex[x, z].token_go);

					if (template.hex[x, z].dice_number == 7)
					{
						RobberLocation = template.hex[x, z];
						template.hex[x, z].hasRobber = true;
						template.hex[x, z].token_go.GetComponent<Renderer>().material = GetRobberTokenMaterial();
					}
					else
						template.hex[x, z].token_go.GetComponent<Renderer>().material = GetTokenMaterial(template.hex[x, z].dice_number);

					template.hex[x, z].token_go.GetComponent<Collider>().enabled = false;

					hexCoordinate = new Coordinate(x, z);
					template.hex[x, z].Coordinates[0] = MapUtility.CalculateVerticeOne(hexCoordinate);
					template.hex[x, z].Coordinates[1] = MapUtility.CalculateVerticeTwo(hexCoordinate);
					template.hex[x, z].Coordinates[2] = MapUtility.CalculateVerticeThree(hexCoordinate);
					template.hex[x, z].Coordinates[3] = MapUtility.CalculateVerticeFour(hexCoordinate);
					template.hex[x, z].Coordinates[4] = MapUtility.CalculateVerticeFive(hexCoordinate);
					template.hex[x, z].Coordinates[5] = MapUtility.CalculateVerticeSix(hexCoordinate);

					if (template.hex [x, z].portSide != -1)
						SpawnPort (template.hex [x, z]);
				}
			}
		}

		for (int z = 0; z < HEIGHT; z++)
		{
			for (int x = 0; x < WIDTH; x++)
			{
				if (template.hex [x, z].portSide != -1)
				{
					switch (template.hex [x, z].portSide)
					{
						case 0:
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 1, template.hex [x, z].resource);
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 2, template.hex [x, z].resource);
							break;
						case 1:
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 2, template.hex [x, z].resource);
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 3, template.hex [x, z].resource);
							break;
						case 2:
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 3, template.hex [x, z].resource);
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 4, template.hex [x, z].resource);
							break;
						case 3:
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 4, template.hex [x, z].resource);
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 5, template.hex [x, z].resource);
							break;
						case 4:
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 5, template.hex [x, z].resource);
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 6, template.hex [x, z].resource);
							break;
						case 5:
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 6, template.hex [x, z].resource);
							SetSettlementPortDiscount(template.hex [x, z].HexLocation, 1, template.hex [x, z].resource);
							break;
						default:
							break;
					}
				}

			}
		}
		

		InitialPlacement = true;

		tutorial = GameObject.Find("Tutorial").GetComponent<Tutorial>();
		tutorial.init();
		
        if (!LocalGame.isNetwork)
            ShowAvailableSettlementsInitial();
        else if (LocalPlayer == CurrentPlayer)
            ShowAvailableSettlementsInitial();

        StartCoroutine(turnTimer());
    }

	void FixedUpdate () {

        if (LocalGame.GameOver == false)
        {
            {
                glowCounter = 0;
                if (goingUp)
                {
                    if (MKGlowObject.GlowIntensity < .500f)
                        MKGlowObject.GlowIntensity += .002f;
                    else
                    {
                        goingUp = false;
                        MKGlowObject.GlowIntensity -= .002f;
                    }
                }
                else
                {
                    if (MKGlowObject.GlowIntensity > .400f)
                        MKGlowObject.GlowIntensity -= .002f;
                    else
                    {
                        goingUp = true;
                        MKGlowObject.GlowIntensity += .002f;
                    }
                }
            }
            glowCounter++;

            if (LocalGame.isNetwork)
            {
                if (LocalPlayer == CurrentPlayer)
                    GUIManager.EnableGameCanvas();
                else
                    GUIManager.DisableGameCanvas();
            }

            if (!InitialPlacement)
                GUIManager.UpdatePlayer();

            if (BoardManager.turnTimerOn)
            {
                if (timeLeft <= 10)
                {
                    timerTextObject.color = timerRed;
                }
                else
                {
                    timerTextObject.color = timerWhite;
                }

                if (timeLeft >= 0)
                {
                    timeLeft -= Time.deltaTime;
                    timerTextObject.text = Mathf.Floor(timeLeft).ToString();
                }
            }
        }
        else
            timerTextObject.text = "0";
   }

	void spawnStructureOne(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice One +++ //
		// Calculate coordinates of new structure at vertice one.
		// Rest hexagon coordinates
		hexCoordinate = new Coordinate (xCoord, yCoord);
		newCoordinateA = MapUtility.CalculateVerticeOne(hexCoordinate);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset - 1f, 0.4f, zPos + 1.6f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset - 1f, 1.6f, zPos + 1.6f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset -.55f, 1.8f, zPos + 1.9f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureTwo(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Two +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice two.
		newCoordinateA = MapUtility.CalculateVerticeTwo (hexCoordinate);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset + 1f, 0.4f, zPos + 1.6f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset + 1f, 1.6f, zPos + 1.6f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset + 1.45f, 1.8f, zPos + 1.9f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureThree(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Three +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice three.
		newCoordinateA = MapUtility.CalculateVerticeThree(hexCoordinate);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset + 1.9f, 0.4f, zPos - .16f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset + 1.9f, 1.6f, zPos - .16f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset + 2.4f, 1.8f, zPos + .2f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureFour(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Four +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice four.
		newCoordinateA = MapUtility.CalculateVerticeFour(hexCoordinate);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset + 1f, 0.4f, zPos - 1.9f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset + 1f, 1.6f, zPos - 1.9f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 ((xCoord * xOffset) + 1.5f, 1.8f, zPos - 1.55f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureFive(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Five +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice five.
		newCoordinateA = MapUtility.CalculateVerticeFive(hexCoordinate);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset - 1f, 0.4f, zPos - 1.9f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset - 1f, 1.6f, zPos - 1.9f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset -.5f, 1.8f, zPos - 1.53f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnStructureSix(int xCoord, int yCoord)
	{
		// +++ Structure at Vertice Six +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of new structure at vertice six.
		newCoordinateA = MapUtility.CalculateVerticeSix(hexCoordinate);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset - 1.9f, 0.4f, zPos - .16f), Quaternion.identity);
		newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		newStructure.ArmySprite_GO = (GameObject)Instantiate (armySprite, new Vector3 (xCoord * xOffset - 1.9f, 1.6f, zPos - .16f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmyNumber_GO = (GameObject)Instantiate (armyText, new Vector3 (xCoord * xOffset - 1.4f, 1.8f, zPos + .2f), Quaternion.Euler (60f, 0f, 0f));
		newStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
		newStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
		// Add structure to list of structures.
		Structures.Add (newStructure);
	}

	void spawnRoadOne(int xCoord, int yCoord)
	{
		// +++ Road One, between Vertice One and Vertice Two +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road one.
		newCoordinateA = MapUtility.CalculateVerticeOne(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road one.
		newCoordinateB = MapUtility.CalculateVerticeTwo(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + .020f, .4f, zPos + 1.8f), Quaternion.Euler (0f, 90f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadTwo(int xCoord, int yCoord)
	{
		// +++ Road Two, between Vertice Two and Vertice Three +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road two.
		newCoordinateA = MapUtility.CalculateVerticeTwo(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road two.
		newCoordinateB = MapUtility.CalculateVerticeThree(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + 1.44f, .4f, zPos + .92f), Quaternion.Euler (0f, -30f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadThree(int xCoord, int yCoord)
	{
		// +++ Road Three, between Vertice Three and Vertice Four +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road three.
		newCoordinateA = MapUtility.CalculateVerticeThree(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road three.
		newCoordinateB = MapUtility.CalculateVerticeFour(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + 1.48f, .4f, zPos - .84f), Quaternion.Euler (0f, 30f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadFour(int xCoord, int yCoord)
	{
		// +++ Road Four, between Vertice Four and Vertice Five +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road four.
		newCoordinateA = MapUtility.CalculateVerticeFour(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road four.
		newCoordinateB = MapUtility.CalculateVerticeFive(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + .02f, .4f, zPos - 1.8f), Quaternion.Euler (0f, 90f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadFive(int xCoord, int yCoord)
	{
		// +++ Road Five, between Vertice Five and Vertice Six +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road five.
		newCoordinateA = MapUtility.CalculateVerticeFive(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road five.
		newCoordinateB = MapUtility.CalculateVerticeSix(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset - 1.44f, .4f, zPos - .92f), Quaternion.Euler (0f, -30f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	void spawnRoadSix(int xCoord, int yCoord)
	{
		// +++ Road Six, between Vertice Six and Vertice One +++ //
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side A of road six.
		newCoordinateA = MapUtility.CalculateVerticeSix(hexCoordinate);
		// Reset the hex coordinates to the loop counters.
		hexCoordinate = new Coordinate (xCoord, yCoord);
		// Calculate coordinates of side B of road six.
		newCoordinateB = MapUtility.CalculateVerticeOne(hexCoordinate);
		// Instantiate new road based on coordinates and current unique road number.
		newRoad = new Road (newCoordinateA, newCoordinateB, roadIndex);
		// Increment unique road number.
		roadIndex++;
		// Spawn road in game world.
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset - 1.44f, .4f, zPos + .92f), Quaternion.Euler (0f, 30f, 0f));
		newRoad.Road_GO.GetComponent<Collider>().enabled = false;
		// Add road to list of roads.
		Roads.Add (newRoad);
	}

	public void SpawnPort (Hex HexWithPort)
	{
		Vector3 hexLocation = HexWithPort.hex_go.transform.position;

		switch (HexWithPort.portSide) 
		{
			case 0:
				Instantiate (boat, new Vector3 (hexLocation.x + .3f, hexLocation.y -.218f, hexLocation.z + 3.2f), Quaternion.Euler (0f, -60f, 0f));
				Instantiate (dock, new Vector3 (hexLocation.x - .02f, hexLocation.y - .1f, hexLocation.z + 2.45f), Quaternion.Euler (0f, 0f, 0f));
				break;
			case 1:
				Instantiate (boat, new Vector3 (hexLocation.x + 2.85f, hexLocation.y -.218f, hexLocation.z + 1f), Quaternion.Euler (0f, 0f, 0f));
				Instantiate (dock, new Vector3 (hexLocation.x + 2f, hexLocation.y - .1f, hexLocation.z + 1.3f), Quaternion.Euler (0f, 60f, 0f));
				break;
			case 2:
				Instantiate (boat, new Vector3 (hexLocation.x + 2.3f, hexLocation.y -.218f, hexLocation.z -2.05f), Quaternion.Euler (0f, 60f, 0f));
				Instantiate (dock, new Vector3 (hexLocation.x + 2f, hexLocation.y - .1f, hexLocation.z - 1.2f), Quaternion.Euler (0f, 120f, 0f));
				break;
			case 3:
				Instantiate (boat, new Vector3 (hexLocation.x - .7f, hexLocation.y -.218f, hexLocation.z - 3f), Quaternion.Euler (0f, 120f, 0f));
				Instantiate (dock, new Vector3 (hexLocation.x + .05f, hexLocation.y - .1f, hexLocation.z - 2.35f), Quaternion.Euler (0f, 180f, 0f));
				break;
			case 4:
				Instantiate (boat, new Vector3 (hexLocation.x - 2.9f, hexLocation.y -.218f, hexLocation.z - .9f), Quaternion.Euler (0f, 180f, 0f));
				Instantiate (dock, new Vector3 (hexLocation.x - 2.1f, hexLocation.y - .1f, hexLocation.z -1.2f), Quaternion.Euler (0f, -120f, 0f));
				break;
			case 5:
				Instantiate (boat, new Vector3 (hexLocation.x - 2.4f, hexLocation.y -.218f, hexLocation.z + 1.9f), Quaternion.Euler (0f, -120f, 0f));
				Instantiate (dock, new Vector3 (hexLocation.x - 2.05f, hexLocation.y - .1f, hexLocation.z + 1.15f), Quaternion.Euler (0f, -60f, 0f));
				break;
			default:
				break;
		}
	}

	public void SetSettlementPortDiscount(Coordinate hexCoord, int settlementNumber, int resourceType)
	{
		Coordinate tempCoord;
		switch (settlementNumber)
		{
			case 1:
				tempCoord = MapUtility.CalculateVerticeOne(hexCoord);
				foreach (Structure currentStructure in Structures)
				{
					if (CompareCoordinates(tempCoord, currentStructure.Location))
					{
						currentStructure.portDiscount = resourceType;
						break;
					}
				}
				break;
			case 2:
				tempCoord = MapUtility.CalculateVerticeTwo(hexCoord);
				foreach (Structure currentStructure in Structures)
				{
					if (CompareCoordinates(tempCoord, currentStructure.Location))
					{
						currentStructure.portDiscount = resourceType;
						break;
					}
				}
				break;
			case 3:
				tempCoord = MapUtility.CalculateVerticeThree(hexCoord);
				foreach (Structure currentStructure in Structures)
				{
					if (CompareCoordinates(tempCoord, currentStructure.Location))
					{
						currentStructure.portDiscount = resourceType;
						break;
					}
				}
				break;
			case 4:
				tempCoord = MapUtility.CalculateVerticeFour(hexCoord);
				foreach (Structure currentStructure in Structures)
				{
					if (CompareCoordinates(tempCoord, currentStructure.Location))
					{
						currentStructure.portDiscount = resourceType;
						break;
					}
				}
				break;
			case 5:
				tempCoord = MapUtility.CalculateVerticeFive(hexCoord);
				foreach (Structure currentStructure in Structures)
				{
					if (CompareCoordinates(tempCoord, currentStructure.Location))
					{
						currentStructure.portDiscount = resourceType;
						break;
					}
				}
				break;
			case 6:
				tempCoord = MapUtility.CalculateVerticeSix(hexCoord);
				foreach (Structure currentStructure in Structures)
				{
					if (CompareCoordinates(tempCoord, currentStructure.Location))
					{
						currentStructure.portDiscount = resourceType;
						break;
					}
				}
				break;
			default:
			break;
		}
	}

	IEnumerator FloatText(Vector3 spawnLocation, Color resourceColor, int numberChanged)
	{
		GameObject createdText = (GameObject)Instantiate (armyText, spawnLocation, Quaternion.Euler (30f, 0f, 0f));
		createdText.GetComponent<Renderer>().material.color = resourceColor;  // set text color
		createdText.GetComponent<TextMesh>().text = (numberChanged > 0 ? "+":"-") + numberChanged.ToString();
		float alpha = 1.0f;

		while (alpha > 0)
		{
			yield return new WaitForSeconds(.00001f);
			Vector3 temp = new Vector3(0f, .5f, 0f);
			createdText.transform.position += temp;
			alpha -= .05f; 
			Color color = createdText.GetComponent<Renderer>().material.color;
			color.a = alpha; 
			createdText.GetComponent<Renderer>().material.color = color;
		} 	

		Destroy(createdText); // text vanished - destroy itself
	}

	// Local building functions
	public void BuildSettlement(Structure settlementTarget)
	{
		AudioManager.playBuildSound();
		MeshRenderer mr = settlementTarget.Structure_GO.GetComponentInChildren<MeshRenderer> ();
		mr.material = GetPlayerMaterial(CurrentPlayer, 1);
		settlementTarget.PlayerOwner = CurrentPlayer;
        Debug.Log("PortDiscount: " + settlementTarget.portDiscount);
		if (settlementTarget.portDiscount != -1) 
		{
			switch (settlementTarget.portDiscount) 
			{
				case 0:
					LocalGame.PlayerList[CurrentPlayer].SetWoodDiscount();
					break;	
				case 1:
					LocalGame.PlayerList[CurrentPlayer].SetOreDiscount();
					break;	
				case 2:
					LocalGame.PlayerList[CurrentPlayer].SetWheatDiscount();
					break;	
				case 3:
					LocalGame.PlayerList[CurrentPlayer].SetWoolDiscount();
					break;	
				case 4:
					LocalGame.PlayerList[CurrentPlayer].SetBrickDiscount();
					break;	
				default:
					break;
			}
		}

		// Is not initial placement, charge player resources for building.
		if (!InitialPlacement)
			LocalGame.PlayerList [CurrentPlayer].BuildSettlement();
	}

	public void BuildCity(Structure settlementTarget)
	{
		AudioManager.playBuildSound();
		Vector3 oldStructCoords = settlementTarget.Structure_GO.transform.position;
		MeshRenderer mr;

		Destroy(settlementTarget.Structure_GO);
		settlementTarget.Structure_GO = Instantiate(city, oldStructCoords, Quaternion.identity);
        mr = settlementTarget.Structure_GO.GetComponentInChildren<MeshRenderer>();
        mr.material = GetPlayerMaterial(CurrentPlayer, 2);
        settlementTarget.IsCity = true;
        LocalGame.PlayerList[CurrentPlayer].Cities++;
        LocalGame.PlayerList[CurrentPlayer].Settlements--;

        if (LocalGame.PlayerList[CurrentPlayer].playerAbility == 5)
        {
            settlementTarget.Armies++;
            LocalGame.PlayerList[CurrentPlayer].Armies++;
        }

        if (LocalPlayer == CurrentPlayer)
		    LocalGame.PlayerList[CurrentPlayer].BuildCity();
    }

	public void BuildRoad(Road targetRoad)
	{
		AudioManager.playBuildSound();
		MeshRenderer mr = targetRoad.Road_GO.GetComponentInChildren<MeshRenderer>();
		targetRoad.PlayerOwner = CurrentPlayer;
		mr.material = GetPlayerMaterial(CurrentPlayer, 0);

		if (InitialPlacement)// Cycles to the next player and shows initial settlements
		{
            if (LocalGame.isNetwork)
            {
				if (CurrentPlayer == LocalPlayer)
					NextPlayer();
            }
			else
			{
				NextPlayer();
			}
		}
	
		else // Charge player for road and calculate longest road.
		{
			LocalGame.PlayerList[CurrentPlayer].BuildRoad();
			
			// Longest road logic.
			int tempRoadLength = 0;
			tempRoadLength = CalculateLongestRoad(CurrentPlayer);
			if (tempRoadLength > LocalGame.PlayerList[CurrentPlayer].LongestRoad)
			{
				LocalGame.PlayerList[CurrentPlayer].LongestRoad = tempRoadLength;

				if (tempRoadLength > LocalGame.LongestRoad)
				{
					int temp  = LocalGame.LongestRoadPlayer;
					LocalGame.PlayerList[CurrentPlayer].LongestRoadWinner = true;
					if (LocalGame.LongestRoadPlayer != -1)
						LocalGame.PlayerList[LocalGame.LongestRoadPlayer].LongestRoadWinner = false;
					LocalGame.LongestRoadPlayer = CurrentPlayer;
					LocalGame.LongestRoad = tempRoadLength;
					GUIManager.SetLongestRoadWinner(temp, CurrentPlayer);
                    LocalGame.PlayerList[CurrentPlayer].UpdateVictoryPoints();
                }

			}
		}
	}

	public void BuyArmy(Structure targetCity)
	{
		AudioManager.playPlaceArmy();
		targetCity.Armies++;
		LocalGame.PlayerList[CurrentPlayer].HireArmy();
        targetCity.ArmyNumber_GO.GetComponent<TextMesh>().text = targetCity.Armies.ToString();
		CalculateLargestArmy ();
    }

	public void ExecuteAttack()
	{
		AudioManager.playBattleSound();
		if (AttackingCity.Armies > DefendingCity.Armies)
		{
			if (DefendingCity.Armies == 0)
			{
				AttackingCity.Armies--;
			}
			else
			{
				AttackingCity.Armies -= DefendingCity.Armies;
				DefendingCity.Armies = 0;
			}

			Vector3 oldStructCoords = DefendingCity.Structure_GO.transform.position;

			Destroy(DefendingCity.Structure_GO);
			DefendingCity.Structure_GO = Instantiate(structure, oldStructCoords, Quaternion.identity);

			MeshRenderer mr = AttackingCity.Structure_GO.GetComponentInChildren<MeshRenderer>();
			mr = DefendingCity.Structure_GO.GetComponentInChildren<MeshRenderer>();
			mr.material = GetPlayerMaterial(DefendingCity.PlayerOwner, 2);
			DefendingCity.IsCity = false;
			DefendingCity.Structure_GO.GetComponent<Collider>().enabled = false;
            LocalGame.PlayerList[DefendingCity.PlayerOwner].Cities--;
            LocalGame.PlayerList[DefendingCity.PlayerOwner].Settlements++;
        }
		else if (AttackingCity.Armies < DefendingCity.Armies)
		{
			DefendingCity.Armies -= AttackingCity.Armies;
			AttackingCity.Armies = 0;
		}
		else // Armies are equal
		{
			AttackingCity.Armies = 0;
			DefendingCity.Armies = 0;
		}
		CalculateLargestArmy ();
	}

	public void MoveRobber(Hex hexTarget)
	{
		MeshRenderer mr = hexTarget.token_go.GetComponentInChildren<MeshRenderer>();
		mr.material = GetRobberTokenMaterial();
		hexTarget.hasRobber = true;

		mr = RobberLocation.token_go.GetComponentInChildren<MeshRenderer>();
		mr.material = GetTokenMaterial(RobberLocation.dice_number);
		RobberLocation.hasRobber = false;
		StealResources(hexTarget);
		RobberLocation = hexTarget;
	}


	// Network building functions
	public void BuildSettlementNetwork(int xCoord, int yCoord)
	{
		foreach (Structure targetStructure in Structures)
		{ 
			if (targetStructure.Location.X == xCoord && targetStructure.Location.Y == yCoord)
			{
				BuildSettlement(targetStructure);
				break;    
			}
		}
	}

	public void BuildCityNetwork(int xCoord, int yCoord)
	{
		foreach (Structure targetStructure in Structures)
		{
			if (targetStructure.Location.X == xCoord && targetStructure.Location.Y == yCoord)
			{
				BuildCity(targetStructure);
				break;
			}
		}
	}

	public void BuildRoadNetwork(int xSideA, int ySideA, int xSideB, int ySideB)
	{
		foreach (Road targetRoad in Roads)
		{
			if ((targetRoad.SideA.X == xSideA && targetRoad.SideA.Y == ySideA && targetRoad.SideB.X == xSideB && targetRoad.SideB.Y == ySideB)
				|| (targetRoad.SideA.X == xSideB && targetRoad.SideA.Y == ySideB && targetRoad.SideB.X == xSideA && targetRoad.SideB.Y == ySideA))
			{
				BuildRoad(targetRoad);
				break;
			}
		}
	}

	public void BuyArmyNetwork(int xCoord, int yCoord)
	{
		foreach (Structure targetStructure in Structures)
		{
			if (targetStructure.Location.X == xCoord && targetStructure.Location.Y == yCoord)
			{
				BuyArmy(targetStructure);
				break;
			}
		}
	}

	public void ExecuteAttackNetwork(int xCoordAttacker, int yCoordAttacker, int xCoordDefender, int yCoordDefender)
	{
		foreach (Structure targetStructure in Structures)
		{
			if (targetStructure.Location.X == xCoordAttacker && targetStructure.Location.Y == yCoordAttacker)
			{
				AttackingCity = targetStructure;
				break;
			}
		}

		foreach (Structure targetStructure in Structures)
		{
			if (targetStructure.Location.X == xCoordDefender && targetStructure.Location.Y == yCoordDefender)
			{
				DefendingCity = targetStructure;
				break;
			}
		}

		ExecuteAttack();
	}

	public void MoveRobberNetwork(int xCoord, int yCoord)
	{
		MoveRobber(template.hex[xCoord, yCoord]);
	}

	public void ShowAvailableSettlementsInitial()
	{
		bool isAvailable;
		
		tutorial.showPlaceInitialSettlementMessage();
		
		foreach (Structure currentStructure in Structures)
		{
			isAvailable = true;

			if (currentStructure.PlayerOwner == -1)
			{
				foreach (Road currentRoad in Roads)
				{
					if (CompareCoordinates(currentStructure.Location, currentRoad.SideA) || CompareCoordinates(currentStructure.Location, currentRoad.SideB))
					{
						foreach (Structure innerStructure in Structures)
						{
							if (CompareCoordinates(innerStructure.Location, currentRoad.SideA) || CompareCoordinates(innerStructure.Location, currentRoad.SideB))
							{
								if (innerStructure.PlayerOwner != -1)
									isAvailable = false;
							}
						}
					}
				}
				if (isAvailable)
				{
					currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
					currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 1);
				}
			}
		}
	}

	public void ShowAvailableSettlements()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuildSettlement())
		{
			bool isAvailable;

			foreach (Road currentRoad in Roads)
			{
				if (currentRoad.PlayerOwner == CurrentPlayer)
				{
					foreach (Structure currentStructure in Structures)
					{
						isAvailable = true;

						if (CompareCoordinates(currentStructure.Location, currentRoad.SideA) || CompareCoordinates(currentStructure.Location, currentRoad.SideB))
						{
							if (currentStructure.PlayerOwner == -1)
							{
								foreach (Road innerRoad in Roads)
								{
									if (CompareCoordinates(currentStructure.Location, innerRoad.SideA) || CompareCoordinates(currentStructure.Location, innerRoad.SideB))
									{
										foreach (Structure innerStructure in Structures)
										{
											if (CompareCoordinates(innerStructure.Location, innerRoad.SideA) || CompareCoordinates(innerStructure.Location, innerRoad.SideB))
											{
												if (innerStructure.PlayerOwner != -1)
													isAvailable = false;
											}
										}
									}
								}

								if (isAvailable)
								{
									currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
									currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 1);
								}
							}
						}
					}
				}
			}
		}
	}

	public void HideAvailableSettlements()
	{
		BuildSettlementButtonClicked = false;

		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner == -1)
			{
				currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(6, 1);
			}
			currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		}
	}

	public void ShowAvailableSettlementsToUpgrade()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuildCity())
		{
			foreach (Structure currentStructure in Structures)
			{
				if (currentStructure.PlayerOwner == CurrentPlayer && !currentStructure.IsCity)
				{
					currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
					currentStructure.Structure_GO.GetComponent<Renderer>().material = GetGlowingPlayerMaterial(CurrentPlayer, 1);
				}
			}
		}
	}

	public void HideAvailableSettlementsToUpgrade()
	{
		BuildCityButtonClicked = false;

		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity == false)
			{
				currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(CurrentPlayer, 1);
			}
			currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		}
	}

	public void ShowAvailableCitiesForArmies()
	{
		BuyingArmy = true;

        foreach (Structure currentStructure in Structures) 
		{
			if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity) {
				currentStructure.Structure_GO.GetComponent<Collider> ().enabled = true;
				currentStructure.Structure_GO.GetComponent<Renderer> ().material = GetGlowingPlayerMaterial (CurrentPlayer, 2);
				currentStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = true;
				currentStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = true;
				currentStructure.ArmyNumber_GO.GetComponent<TextMesh>().text = currentStructure.Armies.ToString ();
			}

		}
	}

    public void HideAvailableCitiesForArmiesInitial()
    {
        BuyingArmy = false;

        foreach (Structure currentStructure in Structures)
        {
            if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity && currentStructure.StructureID != BuyingArmyCity.StructureID)
            {
                currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
                currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(CurrentPlayer, 2);
                currentStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
                currentStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
            }
            else if (currentStructure.StructureID == BuyingArmyCity.StructureID)
            {
                currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
                currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(CurrentPlayer, 2);
            }

        }
    }

    public void HideAvailableCitiesForArmies()
	{
		BuyingArmy = false;
        BuyingArmyCity = null;

        foreach (Structure currentStructure in Structures) 
		{
			if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity) {
				currentStructure.Structure_GO.GetComponent<Collider> ().enabled = false;
				currentStructure.Structure_GO.GetComponent<Renderer> ().material = GetPlayerMaterial (CurrentPlayer, 2);
				currentStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
				currentStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
			}

		}
	}

	public void ShowAvailableCitiesForAttack()
	{
		Attacking = true;
		bool canAttack = false;
		bool anyFound = false;

		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity && currentStructure.Armies > 0) 
			{
				foreach (Road currentRoad in Roads) 
				{
					if (CompareCoordinates (currentRoad.SideA, currentStructure.Location)) 
					{
						foreach (Road innerRoad in Roads) 
						{
							if (CompareCoordinates(currentRoad.SideB, innerRoad.SideA) || CompareCoordinates(currentRoad.SideB, innerRoad.SideB))
							{
								foreach (Structure innerStructure in Structures)
								{
									if (CompareCoordinates(innerRoad.SideA, innerStructure.Location) || CompareCoordinates(innerRoad.SideB, innerStructure.Location))
									{
										if (innerStructure.IsCity && innerStructure.PlayerOwner != CurrentPlayer)
											canAttack = true;
									}
								}
							}
						}
					} 
					else if (CompareCoordinates (currentRoad.SideB, currentStructure.Location)) 
					{
						foreach (Road innerRoad in Roads) 
						{
							if (CompareCoordinates(currentRoad.SideA, innerRoad.SideA) || CompareCoordinates(currentRoad.SideA, innerRoad.SideB))
							{
								foreach (Structure innerStructure in Structures)
								{
									if (CompareCoordinates(innerRoad.SideA, innerStructure.Location) || CompareCoordinates(innerRoad.SideB, innerStructure.Location))
									{
										if (innerStructure.IsCity && innerStructure.PlayerOwner != CurrentPlayer)
											canAttack = true;
									}
								}
							}
						}
					}
				}
			}

			if (canAttack && currentStructure.PlayerOwner == CurrentPlayer) 
			{
				anyFound = true;
				currentStructure.Structure_GO.GetComponent<Collider> ().enabled = true;
				currentStructure.Structure_GO.GetComponent<Renderer> ().material = GetGlowingPlayerMaterial (CurrentPlayer, 2);
				currentStructure.ArmySprite_GO.GetComponent<Renderer> ().enabled = true;
				currentStructure.ArmyNumber_GO.GetComponent<Renderer> ().enabled = true;
				currentStructure.ArmyNumber_GO.GetComponent<TextMesh> ().text = currentStructure.Armies.ToString ();
			}

			canAttack = false;
		}

		if (!anyFound)
			Attacking = false;
	}

	public void HideAvailableCitiesForAttack()
	{
		Attacking = false;

		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner == CurrentPlayer && currentStructure.IsCity)
			{
				currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
				currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(CurrentPlayer, 2);
				currentStructure.ArmySprite_GO.GetComponent<Renderer>().enabled = false;
				currentStructure.ArmyNumber_GO.GetComponent<Renderer>().enabled = false;
			}

		}
	}


	public void ShowAvailableCitiesToAttack(Coordinate AttackingStructureLocation)
	{
		foreach (Road currentRoad in Roads)
		{
			if (CompareCoordinates(AttackingStructureLocation, currentRoad.SideA))
			{
				foreach (Road innerRoad in Roads)
				{
					if (CompareCoordinates (currentRoad.SideB, innerRoad.SideA) || CompareCoordinates (currentRoad.SideB, innerRoad.SideB)) 
					{
						foreach (Structure currentStructure in Structures)
						{
							if (CompareCoordinates (currentStructure.Location, innerRoad.SideA) || CompareCoordinates (currentStructure.Location, innerRoad.SideB)) 
							{
								if (currentStructure.PlayerOwner != -1 && currentStructure.PlayerOwner != CurrentPlayer && currentStructure.IsCity) 
								{
									currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
									currentStructure.Structure_GO.GetComponent<Renderer>().material = GetGlowingPlayerMaterial(currentStructure.PlayerOwner, 2);
								}
							}
						}

					}
				}
			}
			else if (CompareCoordinates(AttackingStructureLocation, currentRoad.SideB))
			{
				foreach (Road innerRoad in Roads)
				{
					if (CompareCoordinates (currentRoad.SideA, innerRoad.SideA) || CompareCoordinates (currentRoad.SideA, innerRoad.SideB)) 
					{
						foreach (Structure currentStructure in Structures)
						{
							if (CompareCoordinates (currentStructure.Location, innerRoad.SideA) || CompareCoordinates (currentStructure.Location, innerRoad.SideB)) 
							{
								if (currentStructure.PlayerOwner != -1 && currentStructure.PlayerOwner != CurrentPlayer && currentStructure.IsCity) 
								{
									currentStructure.Structure_GO.GetComponent<Collider>().enabled = true;
									currentStructure.Structure_GO.GetComponent<Renderer>().material = GetGlowingPlayerMaterial(CurrentPlayer, 2);
								}
							}
						}

					}
				}
			}
		}
	}

	public void HideAvailableCitiesToAttack()
	{
		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.IsCity && currentStructure.PlayerOwner != CurrentPlayer && currentStructure.PlayerOwner != -1)
			{
				currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(currentStructure.PlayerOwner, 2);
			}
			currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
		}
	}

	public void ShowAvailableRoads()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuildRoad())
		{
			foreach (Structure currentStructure in Structures)
			{
				if (currentStructure.PlayerOwner == CurrentPlayer)
				{
					foreach (Road currentRoad in Roads)
					{
						foreach (Road innerRoad in Roads)
						{
							if (currentRoad.PlayerOwner == CurrentPlayer && innerRoad.PlayerOwner == -1)
							{
								bool isAvailable = false;
								if (CompareCoordinates(currentRoad.SideA, innerRoad.SideA))
								{
									foreach (Structure innerStructure in Structures)
										if (CompareCoordinates(innerStructure.Location, innerRoad.SideA) && (innerStructure.PlayerOwner == CurrentPlayer || innerStructure.PlayerOwner == -1))
											isAvailable = true;
								}
								else if (CompareCoordinates(currentRoad.SideA, innerRoad.SideB))
								{
									foreach (Structure innerStructure in Structures)
										if (CompareCoordinates(innerStructure.Location, innerRoad.SideB) && (innerStructure.PlayerOwner == CurrentPlayer || innerStructure.PlayerOwner == -1))
											isAvailable = true;
								}
								else if (CompareCoordinates(currentRoad.SideB, innerRoad.SideA))
								{
									foreach (Structure innerStructure in Structures)
										if (CompareCoordinates(innerStructure.Location, innerRoad.SideA) && (innerStructure.PlayerOwner == CurrentPlayer || innerStructure.PlayerOwner == -1))
											isAvailable = true;
								}
								else if (CompareCoordinates(currentRoad.SideB, innerRoad.SideB))
								{
									foreach (Structure innerStructure in Structures)
										if (CompareCoordinates(innerStructure.Location, innerRoad.SideB) && (innerStructure.PlayerOwner == CurrentPlayer || innerStructure.PlayerOwner == -1))
											isAvailable = true;
								}

								if (isAvailable)
								{
									innerRoad.Road_GO.GetComponent<Collider>().enabled = true;
									innerRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 0);
								}
							}

						}

						if ((CompareCoordinates(currentStructure.Location, currentRoad.SideA) || CompareCoordinates(currentStructure.Location, currentRoad.SideB)) && currentRoad.PlayerOwner == -1)
						{
							currentRoad.Road_GO.GetComponent<Collider>().enabled = true;
							currentRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 0);
						}
					}
				}
			}
		}
	}

	public void ShowAvailableRoadsInitial(Coordinate initialSettlement)
	{
		
		tutorial.showPlaceInitialRoadMessage();
		
		foreach (Road currentRoad in Roads)
		{
			if ((CompareCoordinates(initialSettlement, currentRoad.SideA) || CompareCoordinates(initialSettlement, currentRoad.SideB)) && currentRoad.PlayerOwner == -1)
			{
				currentRoad.Road_GO.GetComponent<Collider>().enabled = true;
				currentRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1, 0);
			}
		}
	}

	public void HideAvailableRoads()
	{
		BuildRoadButtonClicked = false;

		foreach (Road currentRoad in Roads)
		{
			if (currentRoad.PlayerOwner == -1)
			{
				currentRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(6, 0);
			}
			currentRoad.Road_GO.GetComponent<Collider>().enabled = false;
		}
	}

	public int CalculateLongestRoad(int playerNumber)
	{
		List<int> CountedRoads = new List<int>();
		int finalRoadLength = 0;
		int tempRoadLength;
		Coordinate firstCoord = new Coordinate (-1, -1);

		foreach (Road currentRoad in Roads)
		{
			if (currentRoad.PlayerOwner == playerNumber)
			{ 
				CountedRoads = new List<int>();
				CountedRoads.Add(currentRoad.RoadID);
				tempRoadLength = 1;
				tempRoadLength += CalculateLongestRoadRecursive(currentRoad, firstCoord, CountedRoads, playerNumber);

				if (tempRoadLength > finalRoadLength)
					finalRoadLength = tempRoadLength;
			}
		}

		if (LocalGame.PlayerList [playerNumber].playerAbility == 6)
			finalRoadLength += Constants.LongestRoadBonus;

		return finalRoadLength;
	}

	public int CalculateLongestRoadRecursive(Road roadBuilt, Coordinate previousCoordinate, List<int> LocalCountedRoads, int playerNumber)
	{
		int roadLength = 0;
		int tempLargest = 0;
		int tempReturn = 0;

		foreach (Road currentRoad in Roads) 
		{
			if (!LocalCountedRoads.Contains(currentRoad.RoadID) && currentRoad.PlayerOwner == playerNumber && !CompareCoordinates(currentRoad.SideA, previousCoordinate) && !CompareCoordinates(currentRoad.SideB, previousCoordinate))
			{
				if (CompareCoordinates(currentRoad.SideA, roadBuilt.SideA))
				{
					foreach (Structure currentStructure in Structures)
						if (CompareCoordinates (currentStructure.Location, currentRoad.SideA) && (currentStructure.PlayerOwner == playerNumber || currentStructure.PlayerOwner == -1))
						{
							LocalCountedRoads.Add(currentRoad.RoadID);
							roadLength = 1;
							tempReturn = CalculateLongestRoadRecursive (currentRoad, currentRoad.SideA, LocalCountedRoads, playerNumber);
							if (tempReturn > tempLargest)
								tempLargest = tempReturn;
						}
				}
				else if (CompareCoordinates(currentRoad.SideA, roadBuilt.SideB))
				{
					foreach (Structure currentStructure in Structures)
						if (CompareCoordinates (currentStructure.Location, currentRoad.SideA) && (currentStructure.PlayerOwner == playerNumber || currentStructure.PlayerOwner == -1))
						{
							LocalCountedRoads.Add(currentRoad.RoadID);
							roadLength = 1;
							tempReturn = CalculateLongestRoadRecursive (currentRoad, currentRoad.SideA, LocalCountedRoads, playerNumber);
							if (tempReturn > tempLargest)
								tempLargest = tempReturn;
						}
				}
				else if (CompareCoordinates(currentRoad.SideB, roadBuilt.SideA))
				{
					foreach (Structure currentStructure in Structures)
						if (CompareCoordinates (currentStructure.Location, currentRoad.SideB) && (currentStructure.PlayerOwner == playerNumber || currentStructure.PlayerOwner == -1))
						{
							LocalCountedRoads.Add(currentRoad.RoadID); 
							roadLength = 1;
							tempReturn = CalculateLongestRoadRecursive (currentRoad, currentRoad.SideB, LocalCountedRoads, playerNumber);
							if (tempReturn > tempLargest)
								tempLargest = tempReturn;
						}
				}
				else if (CompareCoordinates(currentRoad.SideB, roadBuilt.SideB))
				{
					foreach (Structure currentStructure in Structures)
						if (CompareCoordinates (currentStructure.Location, currentRoad.SideB) && (currentStructure.PlayerOwner == playerNumber || currentStructure.PlayerOwner == -1))
						{
							LocalCountedRoads.Add(currentRoad.RoadID); 
							roadLength = 1;
							tempReturn = CalculateLongestRoadRecursive (currentRoad, currentRoad.SideB, LocalCountedRoads, playerNumber);
							if (tempReturn > tempLargest)
								tempLargest = tempReturn;
						}
				}
			}
		}

		roadLength += tempLargest;
		return roadLength;
	}
		
	public void CalculateLargestArmy()
	{
		int largestArmy = LocalGame.MostArmies;
		int largestArmyPlayer = -1;
        int previousLargestArmyPlayer = LocalGame.MostArmiesPlayer;

		for (int x = 0; x < LocalGame.PlayerList.Count; x++)
		{
			if ((LocalGame.PlayerList[x].Armies + (LocalGame.PlayerList[x].playerAbility == 7 ? Constants.LargestArmyBonus:0)) > largestArmy) 
			{
				largestArmy = (LocalGame.PlayerList[x].Armies + LocalGame.PlayerList[x].playerAbility == 7 ? Constants.LargestArmyBonus:0); 
				largestArmyPlayer = x;
			}
		}

		if (largestArmyPlayer != LocalGame.MostArmiesPlayer && largestArmyPlayer != -1) 
		{
			if (LocalGame.MostArmiesPlayer != -1)
				LocalGame.PlayerList [LocalGame.MostArmiesPlayer].LargestArmyWinner = false;
			
            LocalGame.MostArmies = largestArmy;
            LocalGame.MostArmiesPlayer = largestArmyPlayer;
            GUIManager.SetLargestArmyWinner(previousLargestArmyPlayer, largestArmyPlayer);
        }
    }

	public void ShowHexLocations()
	{
        diceRoller.HideDice();
        for (int z = 0; z < HEIGHT; z++) 
		{
			for (int x = 0; x < WIDTH; x++) 
			{
				if (template.hex [x, z].resource != -1 && !template.hex [x, z].hasRobber) 
				{
					MeshRenderer mr = template.hex [x, z].token_go.GetComponentInChildren<MeshRenderer> ();
					mr.material = GetGlowingTokenMaterial (template.hex [x, z].dice_number);
					template.hex[x, z].token_go.GetComponent<Collider>().enabled = true;
				}
			}
		}
	}

	public void HideHexLocations()
	{
		MeshRenderer mr;

		for (int z = 0; z < HEIGHT; z++) 
		{
			for (int x = 0; x < WIDTH; x++) 
			{
				if (template.hex [x, z].resource != -1) 
				{
					if (!template.hex[x, z].hasRobber)
					{
						mr = template.hex[x, z].token_go.GetComponentInChildren<MeshRenderer>();
						mr.material = GetTokenMaterial(template.hex[x, z].dice_number);
					}
					template.hex[x, z].token_go.GetComponent<Collider>().enabled = false;
				}
			}
		}
	}

	public Material GetPlayerMaterial(int playerNumber, int type)
	{
		Material playerMaterial;

		if (type == 0) // is road
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Roads/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Roads/0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Roads/1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Roads/2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Roads/3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Roads/4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Roads/5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}
		else if (type == 1) // is settlement
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Settlements/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Settlements/0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Settlements/1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Settlements/2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Settlements/3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Settlements/4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Settlements/5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}
		else if (type == 2) // is city
        {
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Cities/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Cities/0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Cities/1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Cities/2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Cities/3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Cities/4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Cities/5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}
        else
            playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;

        return playerMaterial;
	}

	public Material GetGlowingPlayerMaterial(int playerNumber, int type)
	{
		Material playerMaterial;

		if (type == 0) // is road
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Roads/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Roads/Glowing 0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Roads/Glowing 1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Roads/Glowing 2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Roads/Glowing 3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Roads/Glowing 4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Roads/Glowing 5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}
		else if (type == 1) // is settlement
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Settlements/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Settlements/Glowing 0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Settlements/Glowing 1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Settlements/Glowing 2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Settlements/Glowing 3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Settlements/Glowing 4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Settlements/Glowing 5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}
		else // is city
		{
			switch (playerNumber)
			{
			case -1:
				playerMaterial = Resources.Load("Cities/Glowing", typeof(Material)) as Material;
				break;
			case 0:
				playerMaterial = Resources.Load("Cities/Glowing 0", typeof(Material)) as Material; 
				break;
			case 1:
				playerMaterial = Resources.Load("Cities/Glowing 1", typeof(Material)) as Material; 
				break;
			case 2:
				playerMaterial = Resources.Load("Cities/Glowing 2", typeof(Material)) as Material; 
				break;
			case 3:
				playerMaterial = Resources.Load("Cities/Glowing 3", typeof(Material)) as Material; 
				break;
			case 4:
				playerMaterial = Resources.Load("Cities/Glowing 4", typeof(Material)) as Material; 
				break;
			case 5:
				playerMaterial = Resources.Load("Cities/Glowing 5", typeof(Material)) as Material; 
				break;
			default:
				playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
				break;
			}
		}

		return playerMaterial;
	}

	public Material GetTokenMaterial(int tokenNumber)
	{
		Material tokenMaterial;

		switch (tokenNumber)
		{
		case 2:
			tokenMaterial = Resources.Load("Tokens/Token 2", typeof(Material)) as Material; 
			break;
		case 3:
			tokenMaterial = Resources.Load("Tokens/Token 3", typeof(Material)) as Material; 
			break;
		case 4:
			tokenMaterial = Resources.Load("Tokens/Token 4", typeof(Material)) as Material; 
			break;
		case 5:
			tokenMaterial = Resources.Load("Tokens/Token 5", typeof(Material)) as Material; 
			break;
		case 6:
			tokenMaterial = Resources.Load("Tokens/Token 6", typeof(Material)) as Material; 
			break;
		case 7:
			tokenMaterial = Resources.Load("Tokens/Token 7", typeof(Material)) as Material;
			break;
		case 8:
			tokenMaterial = Resources.Load("Tokens/Token 8", typeof(Material)) as Material; 
			break;
		case 9:
			tokenMaterial = Resources.Load("Tokens/Token 9", typeof(Material)) as Material; 
			break;
		case 10:
			tokenMaterial = Resources.Load("Tokens/Token 10", typeof(Material)) as Material; 
			break;
		case 11:
			tokenMaterial = Resources.Load("Tokens/Token 11", typeof(Material)) as Material; 
			break;
		case 12:
			tokenMaterial = Resources.Load("Tokens/Token 12", typeof(Material)) as Material; 
			break;
		default:
			tokenMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
			break;
		}

		return tokenMaterial;
	}

	public Material GetRobberTokenMaterial()
	{
		return Resources.Load("Tokens/Robber", typeof(Material)) as Material;
	}

	public Material GetGlowingTokenMaterial(int tokenNumber)
	{
		Material tokenMaterial;

		switch (tokenNumber)
		{
		case 2:
			tokenMaterial = Resources.Load("Tokens/Glowing token 2", typeof(Material)) as Material; 
			break;
		case 3:
			tokenMaterial = Resources.Load("Tokens/Glowing token 3", typeof(Material)) as Material; 
			break;
		case 4:
			tokenMaterial = Resources.Load("Tokens/Glowing token 4", typeof(Material)) as Material; 
			break;
		case 5:
			tokenMaterial = Resources.Load("Tokens/Glowing token 5", typeof(Material)) as Material; 
			break;
		case 6:
			tokenMaterial = Resources.Load("Tokens/Glowing token 6", typeof(Material)) as Material; 
			break;
		case 7:
			tokenMaterial = Resources.Load("Tokens/Glowing token 7", typeof(Material)) as Material;
			break;
		case 8:
			tokenMaterial = Resources.Load("Tokens/Glowing token 8", typeof(Material)) as Material; 
			break;
		case 9:
			tokenMaterial = Resources.Load("Tokens/Glowing token 9", typeof(Material)) as Material; 
			break;
		case 10:
			tokenMaterial = Resources.Load("Tokens/Glowing token 10", typeof(Material)) as Material; 
			break;
		case 11:
			tokenMaterial = Resources.Load("Tokens/Glowing token 11", typeof(Material)) as Material; 
			break;
		case 12:
			tokenMaterial = Resources.Load("Tokens/Glowing token 12", typeof(Material)) as Material; 
			break;
		default:
			tokenMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
			break;
		}

		return tokenMaterial;
	}

	public bool CompareCoordinates(Coordinate A, Coordinate B)
	{
		bool tempBool;

		if (A.X == B.X && A.Y == B.Y)
			tempBool = true;
		else
			tempBool = false;

		return tempBool;
	}

    public void HideAll()
    {
        HideAvailableSettlements();
        HideAvailableSettlementsToUpgrade();
        HideAvailableCitiesForArmies();
        HideAvailableCitiesForAttack();
        HideAvailableCitiesToAttack();
        HideAvailableRoads();
        HideHexLocations();
        GUIManager.closeBarracks();
        GUIManager.closeShop();
    }


    public void NextPlayer()
	{
        HideAll();
        
		BuildRoadButtonClicked = false;
		BuildCityButtonClicked = false;
		BuildSettlementButtonClicked = false;
        BuyingArmy = false;
        Attacking = false;

      if (LocalGame.isNetwork && CurrentPlayer == LocalPlayer)
         NetManager.sendEndTurn(NetManager.hostConnectionID);

      if (InitialPlacement)
		{
			if (FirstTurn)
			{
                if (CurrentPlayer < LocalGame.PlayerList.Count - 1)
                {
                    CurrentPlayer++;
                    //The swap player doesnt work for first turn for local... yet
                    if (LocalGame.isNetwork)
                        GUIManager.NextPlayerNetwork();
                    else
                    {
                        LocalPlayer++;
                        GUIManager.NextPlayerLocal(LocalGame.LongestRoadPlayer, LocalGame.MostArmiesPlayer);
                    }
                    
                }
                else
                {
                    FirstTurn = false;
                }

			}
			else
			{
                if (CurrentPlayer > 0)
                {
                    CurrentPlayer--;
                    
                    if (LocalGame.isNetwork)
                        GUIManager.NextPlayerNetwork(true);
                    else
                    {
                        LocalPlayer--;
                        GUIManager.NextPlayerLocal(LocalGame.LongestRoadPlayer, LocalGame.MostArmiesPlayer, true);
                    }
                    
                }
                else
                {
                    InitialPlacement = false;
                    
                    if (LocalGame.isNetwork && LocalPlayer == CurrentPlayer)
                    {
                        RollDiceClick();
                    }
                    else
                    {
                        RollDiceClick();
                        GUIManager.EnableGameCanvas();
                    }
                }

            }
            
            if (InitialPlacement && LocalPlayer == CurrentPlayer)
                ShowAvailableSettlementsInitial();
        }
		else
		{
            if (CurrentPlayer < LocalGame.PlayerList.Count - 1)
            {
                CurrentPlayer++;
                if (!LocalGame.isNetwork)
                    LocalPlayer++;
            }
            else
            {
                CurrentPlayer = 0;
                if (!LocalGame.isNetwork)
                    LocalPlayer = 0;
                DistributeGold();
            }

         if (LocalGame.isNetwork)
         {
            GUIManager.NextPlayerNetwork();
            if (CurrentPlayer == LocalPlayer)
                RollDiceClick();
         }
         else
         {
            GUIManager.NextPlayerLocal(LocalGame.LongestRoadPlayer, LocalGame.MostArmiesPlayer, (FirstTurn == false && InitialPlacement == true) ? true : false);
            RollDiceClick();
         }
      }
      if (LocalGame.PlayerList[CurrentPlayer].isConnected == false && LocalGame.isNetwork)
         NextPlayer();
	 
	  if (!InitialPlacement && tutorial.dontShowAgain == false && CurrentPlayer == LocalPlayer)
		  tutorial.showBuildRoadTutorialBox();

		if(timerCoroutineStarted)
		{
			StopCoroutine(turnTimer());
			StartCoroutine(turnTimer());
		}
		else
		{
	  	   StartCoroutine(turnTimer());
		}
    }

	public void DistributeResources(int rollNumber)
	{
        if (rollNumber != -1)
        {
            diceRoller.HideDice();
            for (int z = 0; z < HEIGHT; z++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    if (template.hex[x, z].dice_number == rollNumber && template.hex[x, z].resource >= 0 && template.hex[x, z].hasRobber == false)
                    {
                        foreach (Structure currentStructure in Structures)
                        {
                            if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[0]))
                            {
                                if (currentStructure.PlayerOwner != -1)
                                    DistributeResource(currentStructure.Structure_GO.transform.position, template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
                                break;
                            }
                        }

                        foreach (Structure currentStructure in Structures)
                        {
                            if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[1]))
                            {
                                if (currentStructure.PlayerOwner != -1)
                                    DistributeResource(currentStructure.Structure_GO.transform.position, template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
                                break;
                            }
                        }

                        foreach (Structure currentStructure in Structures)
                        {
                            if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[2]))
                            {
                                if (currentStructure.PlayerOwner != -1)
                                    DistributeResource(currentStructure.Structure_GO.transform.position, template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
                                break;
                            }
                        }

                        foreach (Structure currentStructure in Structures)
                        {
                            if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[3]))
                            {
                                if (currentStructure.PlayerOwner != -1)
                                    DistributeResource(currentStructure.Structure_GO.transform.position, template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
                                break;
                            }
                        }

                        foreach (Structure currentStructure in Structures)
                        {
                            if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[4]))
                            {
                                if (currentStructure.PlayerOwner != -1)
                                    DistributeResource(currentStructure.Structure_GO.transform.position, template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
                                break;
                            }
                        }

                        foreach (Structure currentStructure in Structures)
                        {
                            if (CompareCoordinates(currentStructure.Location, template.hex[x, z].Coordinates[5]))
                            {
                                if (currentStructure.PlayerOwner != -1)
                                    DistributeResource(currentStructure.Structure_GO.transform.position, template.hex[x, z].resource, currentStructure.PlayerOwner, currentStructure.IsCity);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

	public void DistributeResource(Vector3 spawnLocation, int resourceNumber, int playerNumber, bool isCity)
	{
		int amountToDistribute = 0;
		switch (resourceNumber)
		{
		case 0:
		    AudioManager.playLumberSound();
			amountToDistribute = ((isCity ? 2 : 1) + (LocalGame.PlayerList [playerNumber].playerAbility == 0 ? 1 : 0));
			LocalGame.PlayerList [playerNumber].Wood += amountToDistribute;
			StartCoroutine(FloatText (spawnLocation, Color.black, amountToDistribute));
			break;
		case 1:
		    AudioManager.playOreSound();
			amountToDistribute = ((isCity ? 2 : 1) + (LocalGame.PlayerList [playerNumber].playerAbility == 5 ? 1 : 0));
			LocalGame.PlayerList [playerNumber].Ore += amountToDistribute;
			StartCoroutine(FloatText (spawnLocation, Color.grey, amountToDistribute));
			break;
		case 2:
		    AudioManager.playGrainSound();
			LocalGame.PlayerList[playerNumber].Wheat += (isCity ? 2 : 1);
			StartCoroutine(FloatText (spawnLocation, Color.yellow, (isCity ? 2 : 1)));
			break;
		case 3:
		    AudioManager.playWoolSound();
			amountToDistribute = ((isCity ? 2 : 1) + (LocalGame.PlayerList [playerNumber].playerAbility == 6 ? 1 : 0));
			LocalGame.PlayerList [playerNumber].Wool += amountToDistribute;
			StartCoroutine(FloatText (spawnLocation, Color.white, amountToDistribute));
			break;
		case 4:
		    AudioManager.playBrickSound();
			amountToDistribute = ((isCity ? 2 : 1) + (LocalGame.PlayerList [playerNumber].playerAbility == 7 ? 1 : 0));
			LocalGame.PlayerList [playerNumber].Brick += amountToDistribute;
			StartCoroutine(FloatText (spawnLocation, Color.red, amountToDistribute));
			break;
		case 5:
			// Desert, move robber code
			break;
		default:
			break;
		};
	}

	public void StealResources(Hex robbedHex)
	{
		bool?[] stealFromPlayer = new bool?[6];

		stealFromPlayer[0] = null;
		stealFromPlayer[1] = null;
		stealFromPlayer[2] = null;
		stealFromPlayer[3] = null;
		stealFromPlayer[4] = null;
		stealFromPlayer[5] = null;

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[0]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[1]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[2]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[3]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[4]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		foreach (Structure currentStructure in Structures)
		{
			if (CompareCoordinates(currentStructure.Location, robbedHex.Coordinates[5]))
			{
				if (currentStructure.PlayerOwner != -1)
				{
					if (currentStructure.IsCity && currentStructure.Armies > 1)
						stealFromPlayer[currentStructure.PlayerOwner] = false;
					else if (stealFromPlayer[currentStructure.PlayerOwner] != false)
						stealFromPlayer[currentStructure.PlayerOwner] = true;
				}
				break;
			}
		}

		if (stealFromPlayer[0] == true && LocalGame.PlayerList[0].playerAbility != 2)
			StartCoroutine(StealResources(0, robbedHex.token_go.transform.position));
		if (stealFromPlayer[1] == true && LocalGame.PlayerList[0].playerAbility != 2)
			StartCoroutine(StealResources(1, robbedHex.token_go.transform.position));
		if (stealFromPlayer[2] == true && LocalGame.PlayerList[0].playerAbility != 2)
			StartCoroutine(StealResources(2, robbedHex.token_go.transform.position));
		if (stealFromPlayer[3] == true && LocalGame.PlayerList[0].playerAbility != 2)
			StartCoroutine(StealResources(3, robbedHex.token_go.transform.position));
		if (stealFromPlayer[4] == true && LocalGame.PlayerList[0].playerAbility != 2)
			StartCoroutine(StealResources(4, robbedHex.token_go.transform.position));
		if (stealFromPlayer[5] == true && LocalGame.PlayerList[0].playerAbility != 2)
			StartCoroutine(StealResources(5, robbedHex.token_go.transform.position));

	}


	public void DistributeGold()
	{
		AudioManager.playSellSound();
		int goldToDistribute = 0;
		foreach (Structure currentStructure in Structures)
		{
			if (currentStructure.PlayerOwner != -1)
			{
				goldToDistribute = (currentStructure.IsCity ? 100 : 50);
				if (LocalGame.PlayerList [currentStructure.PlayerOwner].playerAbility == 4)
					goldToDistribute = (int)(goldToDistribute * Constants.QueenGoldBonus);
				LocalGame.PlayerList[currentStructure.PlayerOwner].Gold += (currentStructure.IsCity ? 100 : 50);
			}
		}
	}

	IEnumerator StealResources(int playerVictim, Vector3 robberTokenLocation)
	{
		Player victim = LocalGame.PlayerList[playerVictim];
		int resourceCount = victim.Brick + victim.Ore + victim.Wheat + victim.Wood + victim.Wool;
		int resourceToSteal;
		bool resourceStolen;
		int[] resourcesStolen = new int[5] {0, 0, 0, 0, 0};

		if (resourceCount > 0)
		{
			resourceCount /= 2;
			for (int x = resourceCount; x > 0; x--)
			{
				resourceStolen = false;
				do
				{
					resourceToSteal = UnityEngine.Random.Range(1, 6);
					switch (resourceToSteal)
					{
					case 1:
						if (victim.Brick > 0)
						{
							victim.Brick--;
							resourcesStolen[0]--;
							resourceStolen = true;
						}
						break;
					case 2:
						if (victim.Ore > 0)
						{
							victim.Ore--;
							resourcesStolen[1]--;
							resourceStolen = true;
						}
						break;
					case 3:
						if (victim.Wheat > 0)
						{
							victim.Wheat--;
							resourcesStolen[2]--;
							resourceStolen = true;
						}
						break;
					case 4:
						if (victim.Wood > 0)
						{
							victim.Wood--;
							resourcesStolen[3]--;
							resourceStolen = true;
						}
						break;
					case 5:
						if (victim.Wool > 0)
						{
							victim.Wool--;
							resourcesStolen[4]--;
							resourceStolen = true;
						}
						break;
					default:
						resourceStolen = true;
						break;
					}
				} while (!resourceStolen);
			}
		}
		
		if (resourcesStolen[0] < 0)
		{
			FloatText(robberTokenLocation, Color.red, resourcesStolen[0]);
			yield return new WaitForSeconds(1);
		}
		if (resourcesStolen[1] < 0)
		{
			FloatText(robberTokenLocation, Color.grey, resourcesStolen[1]);
			yield return new WaitForSeconds(1);
		}
		if (resourcesStolen[2] < 0)
		{
			FloatText(robberTokenLocation, Color.yellow, resourcesStolen[2]);
			yield return new WaitForSeconds(1);
		}
		if (resourcesStolen[3] < 0)
		{
			FloatText(robberTokenLocation, Color.black, resourcesStolen[3]);
			yield return new WaitForSeconds(1);
		}
		if (resourcesStolen[4] < 0)
		{
			FloatText(robberTokenLocation, Color.white, resourcesStolen[4]);
			yield return new WaitForSeconds(1);
		}
	}

	public void BuyWheat()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyWheat())
		{
			AudioManager.playBuySound();
			LocalGame.PlayerList[CurrentPlayer].BuyWheat();
		}
	}

	public void SellWheat()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Wheat > 0)
		{
			AudioManager.playSellSound();
			LocalGame.PlayerList[CurrentPlayer].SellWheat();
		}
	}

	public void BuyBrick()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyBrick())
		{
			AudioManager.playBuySound();
			LocalGame.PlayerList[CurrentPlayer].BuyBrick();
		}
	}

	public void SellBrick()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Brick > 0)
		{
			AudioManager.playSellSound();
			LocalGame.PlayerList[CurrentPlayer].SellBrick();
		}
	}

	public void BuyWool()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyWool())
		{
			AudioManager.playBuySound();
			LocalGame.PlayerList[CurrentPlayer].BuyWool();
		}
	}

	public void SellWool()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Wool > 0)
		{
			AudioManager.playSellSound();
			LocalGame.PlayerList[CurrentPlayer].SellWool();
		}
	}

	public void BuyWood()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyWood())
		{
			AudioManager.playBuySound();
			LocalGame.PlayerList[CurrentPlayer].BuyWood();
		}
	}

	public void SellWood()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Wood > 0)
		{
			AudioManager.playSellSound();
			LocalGame.PlayerList[CurrentPlayer].SellWood();
		}
	}

	public void BuyOre()
	{
		if (LocalGame.PlayerList[CurrentPlayer].CanBuyOre())
		{
			AudioManager.playBuySound();
			LocalGame.PlayerList[CurrentPlayer].BuyOre();
		}
	}

	public void SellOre()
	{
		if (LocalGame.PlayerList[CurrentPlayer].Ore > 0)
		{
			AudioManager.playSellSound();
			LocalGame.PlayerList[CurrentPlayer].SellOre();
		}
	}

	public void BuildSettlementClick()
	{
        HideAll();

        if (BuildSettlementButtonClicked) 
		{
			BuildSettlementButtonClicked = false;
			HideAvailableSettlements ();
		} 
		else 
		{
			BuildSettlementButtonClicked = true;
			ShowAvailableSettlements();
		}
	}

	public void BuildRoadClick()
	{
        HideAll();

        if (BuildRoadButtonClicked) 
		{
			BuildRoadButtonClicked = false;
			HideAvailableRoads();
		} 
		else 
		{
			BuildRoadButtonClicked = true;
			ShowAvailableRoads();
		}
	}

	public void BuildCityClick()
	{
        HideAll();

        if (BuildCityButtonClicked) 
		{
			BuildCityButtonClicked = false;
			HideAvailableSettlementsToUpgrade();
		} 
		else 
		{
			BuildCityButtonClicked = true;
			ShowAvailableSettlementsToUpgrade();
		}
	}

	public void AttackClick()
	{
		StartCoroutine(AudioManager.playBattleMusic());
        HideAll();

        if (Attacking) 
		{
			HideAvailableCitiesForAttack();
		} 
		else 
		{
			ShowAvailableCitiesForAttack();
		}
	}

    public void RollDiceClick()
    {
		int diceValue = -1;
        
        diceRoller.ShowDice();

        diceRoller.RollDice((value)=>{
			if (value == 7)
				ShowHexLocations();
			else
				DistributeResources(value);
			});
        
        if (LocalGame.isNetwork)
        {
           NetManager.sendDiceRoll(diceValue, NetManager.hostConnectionID);
        }
    }

    public void ReceiveDiceRoll(int diceRoll)
    {
        StartCoroutine(GUIManager.RollDice(diceRoll));

        if (diceRoll != 7)
            DistributeResources(diceRoll);
    }

    public void DiceRollNetwork(int diceRoll)
    {
        StartCoroutine(GUIManager.RollDice(diceRoll));

        if (diceRoll != 7)
            DistributeResources(diceRoll);
    }

    public void ShowBarracksClick()
    {
        HideAll();
        BuyingArmy = true;
        ShowAvailableCitiesForArmies();
        GUIManager.openBaracks();
    }

    public void HideBaracksClick()
    {
        HideAll();
        GUIManager.closeBarracks();
    }

    public void BuyArmyClick()
    {
        if (LocalGame.PlayerList[CurrentPlayer].CanHireArmy() && BuyingArmyCity != null)
        {
            BuyArmy(BuyingArmyCity);
            if (LocalGame.isNetwork)
                NetManager.sendBuildArmy(BuyingArmyCity.Location.X, BuyingArmyCity.Location.Y, NetManager.hostConnectionID);
        }
    }

	IEnumerator turnTimer()
	{
		timeLeft = BoardManager.turnTimerMax;
		timerCoroutineStarted = true;
		yield return new WaitUntil(() => timeLeft < 0);
		NextPlayer();
	}
}

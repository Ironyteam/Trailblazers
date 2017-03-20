using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{

    public GameObject[] hexPrefabs;
    public GameObject hexBoard;
	public GameObject roadRect;
	public GameObject structure;

    const int WIDTH = HexTemplate.WIDTH;
    const int HEIGHT = HexTemplate.HEIGHT;

    HexTemplate template = new HexTemplate();

    float xOffset = .750f;
	float zOffset = .875f;
	float zPos;
	
	public bool goingUp = true;

	public List<Road> Roads			  = new List<Road>();
	public List<Structure> Structures = new List<Structure>();
	public Coordinate hexCoordinate;
    public Coordinate tempCoordinate = new Coordinate(0, 0);
    public Coordinate newCoordinateA;
	public Coordinate newCoordinateB;
	public Structure newStructure;
	public Road newRoad;
	public int structureIndex = 0;
	public int roadIndex = 0;
	public MKGlow MKGlowObject;
    public int glowCounter = 0;

    public int CurrentPlayer = 0;
    public bool InitialPlacement;
	
    void Start()
    {
		MKGlowObject = GameObject.Find("Main Camera").GetComponent<MKGlow>();

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
							if (template.hex [x-1, z-1].resource == -1)
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
				   template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefabs[template.hex[x, z].resource], new Vector3(x * xOffset, 0.1f, zPos), Quaternion.Euler(0, 30, 0));
                   template.hex[x, z].hex_go.name = x + "," + z;
				}
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
		//if (glowCounter == 100)
		{
            glowCounter = 0;
			if (goingUp)
			{
				if (MKGlowObject.GlowIntensity < .500f)
					MKGlowObject.GlowIntensity += .001f;
				else
				{
					goingUp = false;
					MKGlowObject.GlowIntensity -= .001f;
				}
			}
			else
			{
				if (MKGlowObject.GlowIntensity > .400f)
					MKGlowObject.GlowIntensity -= .001f;
				else
				{
					goingUp = true;
					MKGlowObject.GlowIntensity += .001f;
				}
			}
		}
        glowCounter++;
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
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset - .25f, 0.2f, zPos + .4f), Quaternion.identity);
        newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
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
		//Debug.Log (newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset + .25f, 0.2f, zPos + .4f), Quaternion.identity);
        newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
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
		//Debug.Log(newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset + .475f, 0.2f, zPos - .04f), Quaternion.identity);
        newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
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
		//Debug.Log(newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset + .25f, 0.2f, zPos - .475f), Quaternion.identity);
        newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
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
		//Debug.Log(newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset - .25f, 0.2f, zPos - .475f), Quaternion.identity);
        newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
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
		//Debug.Log(newCoordinateA.X + newCoordinateA.Y);
		// Instantiate new structure based on coordinates and current unique structure number.
		newStructure = new Structure (newCoordinateA, structureIndex);
		// Increment unique structure number.
		structureIndex++;
		// Spawn structure in game world.
		newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (xCoord * xOffset - .475f, 0.2f, zPos - .04f), Quaternion.identity);
        newStructure.Structure_GO.GetComponent<Collider>().enabled = false;
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
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + .005f, 0.175f, zPos + .45f), Quaternion.Euler (90f, 0f, 90f));
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
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + .36f, 0.175f, zPos + .23f), Quaternion.Euler (90f, 0f, 30f));
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
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + .37f, 0.175f, zPos - .21f), Quaternion.Euler (90f, 0f, -30f));
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
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset + .005f, 0.175f, zPos - .45f), Quaternion.Euler (90f, 0f, 90f));
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
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset - .36f, 0.175f, zPos - .23f), Quaternion.Euler (90f, 0f, 30f));
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
		newRoad.Road_GO = (GameObject)Instantiate (roadRect, new Vector3 (xCoord * xOffset - .36f, 0.175f, zPos + .23f), Quaternion.Euler (90f, 0f, -30f));
        newRoad.Road_GO.GetComponent<Collider>().enabled = false;
        // Add road to list of roads.
        Roads.Add (newRoad);
	}

    public void ShowAvailableSettlementsInitial()
    {
        InitialPlacement = true;
        bool isAvailable;
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
                    currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1);
                }
            }
        }
    }

    public void ShowAvailableSettlements()
    {
        InitialPlacement = false;
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
                                currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1);
                            }
                        }
                    }
                }
            }
        }
    }

    public void HideAvailableSettlements()
    {
        foreach (Structure currentStructure in Structures)
        {
            if (currentStructure.PlayerOwner == -1)
            {
                currentStructure.Structure_GO.GetComponent<Collider>().enabled = false;
                currentStructure.Structure_GO.GetComponent<Renderer>().material = GetPlayerMaterial(6);
            }
        }
    }

    public void ShowAvailableRoads()
    {
        InitialPlacement = false;
        foreach (Structure currentStructure in Structures)
        {
            if (currentStructure.PlayerOwner == CurrentPlayer)
            {
                foreach (Road currentRoad in Roads)
                {
                    foreach (Road innerRoad in Roads)
                    {
                        if (currentRoad.PlayerOwner == CurrentPlayer && innerRoad.PlayerOwner == -1 &&
                            (CompareCoordinates(currentRoad.SideA, innerRoad.SideA) 
                            || CompareCoordinates(currentRoad.SideA, innerRoad.SideB)
                            || CompareCoordinates(currentRoad.SideB, innerRoad.SideA)
                            || CompareCoordinates(currentRoad.SideB, innerRoad.SideB)))
                        {
                            innerRoad.Road_GO.GetComponent<Collider>().enabled = true;
                            innerRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1);
                        }
                    }

                    if ((CompareCoordinates(currentStructure.Location, currentRoad.SideA) || CompareCoordinates(currentStructure.Location, currentRoad.SideB)) && currentRoad.PlayerOwner == -1)
                    {
                        currentRoad.Road_GO.GetComponent<Collider>().enabled = true;
                        currentRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1);
                    }
                }
            }
        }
    }

    public void ShowAvailableRoadsInitial(Coordinate initialSettlement)
    {
        foreach (Road currentRoad in Roads)
        {
            if ((CompareCoordinates(initialSettlement, currentRoad.SideA) || CompareCoordinates(initialSettlement, currentRoad.SideB)) && currentRoad.PlayerOwner == -1)
            {
                currentRoad.Road_GO.GetComponent<Collider>().enabled = true;
                currentRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(-1);
            }
        }
    }

    public void HideAvailableRoads()
    {
        foreach (Road currentRoad in Roads)
        {
            if (currentRoad.PlayerOwner == -1)
            {
                currentRoad.Road_GO.GetComponent<Collider>().enabled = false;
                currentRoad.Road_GO.GetComponent<Renderer>().material = GetPlayerMaterial(6);
            }
        }
    }

    public void ShowAvailableCities(int playerNumber)
    {

    }

    public Material GetPlayerMaterial(int playerNumber)
    {
        Material playerMaterial;

        switch (playerNumber)
        {
            case -1:
                playerMaterial = Resources.Load("Glowing", typeof(Material)) as Material;
                break;
            case 0:
                playerMaterial = Resources.Load("0", typeof(Material)) as Material; 
                break;
            case 1:
                playerMaterial = Resources.Load("1", typeof(Material)) as Material; 
                break;
            case 2:
                playerMaterial = Resources.Load("3", typeof(Material)) as Material; 
                break;
            case 3:
                playerMaterial = Resources.Load("4", typeof(Material)) as Material; 
                break;
            case 4:
                playerMaterial = Resources.Load("5", typeof(Material)) as Material; 
                break;
            case 5:
                playerMaterial = Resources.Load("6", typeof(Material)) as Material; 
                break;
            default:
                playerMaterial = Resources.Load("Invisible", typeof(Material)) as Material;
                break;
        }

        return playerMaterial;
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

    public void NextPlayer()
    {
        if (CurrentPlayer < 5)
            CurrentPlayer++;
        else
            CurrentPlayer = 0;
    }
}

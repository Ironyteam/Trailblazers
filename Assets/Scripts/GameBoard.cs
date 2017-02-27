using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{

    public GameObject[] hexPrefabs;
    public GameObject hexBoard;
	public GameObject roadRect;
	public GameObject structure;

    string[] resources = new string[6]
    {
       "Wood", "Ore", "Grain", "Wool", "Brick", "Desert"
    };

    const int WIDTH = HexTemplate.WIDTH;
    const int HEIGHT = HexTemplate.HEIGHT;

    HexTemplate template = new HexTemplate();

    float xOffset = .750f;
	float zOffset = .875f;

	public List<Road> Roads			  = new List<Road>();
	public List<Structure> Structures = new List<Structure>();

    void Start()
    {
        template = BoardManager.template;

        for (int z = 0; z < HEIGHT; z++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
               // hexPrefab.name = "hex " + x + "," + z;

                float zPos = z * zOffset;
                if (x % 2 == 1 || x % 2 == -1)
                {
                    zPos += (zOffset * .5f);
                }
				
				if (template.hex [x, z].resource != -1) 
				{
					// 1
					Coordinate newCoordinate = new Coordinate (x, z);
					Structure newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (x * xOffset - .25f, 0.2f, zPos + .4f), Quaternion.identity);
					Structures.Add (newStructure);

					// 2
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (x * xOffset + .25f, 0.2f, zPos + .4f), Quaternion.identity);
					Structures.Add (newStructure);


					// 3
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (x * xOffset + .475f, 0.2f, zPos - .04f), Quaternion.identity);
					Structures.Add (newStructure);
	
					// 4
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (x * xOffset + .25f, 0.2f, zPos - .475f), Quaternion.identity);
					Structures.Add (newStructure);

					// 5
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (x * xOffset - .25f, 0.2f, zPos - .475f), Quaternion.identity);
					Structures.Add (newStructure);


					// 6
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (structure, new Vector3 (x * xOffset - .475f, 0.2f, zPos - .04f), Quaternion.identity);
					Structures.Add (newStructure);


					// 1
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (roadRect, new Vector3 (x * xOffset + .005f, 0.175f, zPos + .45f), Quaternion.Euler (90f, 0f, 90f));
					Structures.Add (newStructure);

					// 2 
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (roadRect, new Vector3 (x * xOffset + .36f, 0.175f, zPos + .23f), Quaternion.Euler (90f, 0f, 30f));
					Structures.Add (newStructure);

					// 3
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (roadRect, new Vector3 (x * xOffset + .37f, 0.175f, zPos - .21f), Quaternion.Euler (90f, 0f, -30f));
					Structures.Add (newStructure);

					// 4
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (roadRect, new Vector3 (x * xOffset + .005f, 0.175f, zPos - .45f), Quaternion.Euler (90f, 0f, 90f));
					Structures.Add (newStructure);
		

					// 5
					newStructure = new Structure (newCoordinate);
					newStructure.Structure_GO = (GameObject)Instantiate (roadRect, new Vector3 (x * xOffset - .36f, 0.175f, zPos - .23f), Quaternion.Euler (90f, 0f, 30f));
					Structures.Add (newStructure);

					// 6
					newStructure = new Structure (newCoordinate); 
					newStructure.Structure_GO = (GameObject)Instantiate (roadRect, new Vector3 (x * xOffset - .36f, 0.175f, zPos + .23f), Quaternion.Euler (90f, 0f, -30f));
					Structures.Add (newStructure);
				}
				
				if (template.hex[x, z].resource >= 0)
				   template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefabs[template.hex[x, z].resource], new Vector3(x * xOffset, 0.1f, zPos), Quaternion.Euler(0, 30, 0));
			    else
				{
					//template.hex[x, z].hex_go = (GameObject)Instantiate(hexBoard, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                   // template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.blue;
				}
					

                /*switch (template.hex[x, z].resource)
                {
                    case 0:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity);
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.black;
                        break;
                    case 1:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.grey;
                        break;
                    case 2:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.green;
                        break;
                    case 3:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.white;
                        break;
                    case 4:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.red;
                        break;
                    case 5:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.yellow;
                        break;
                    default:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.blue;
                        break;
                }*/
				if (template.hex[x, z].resource >= 0)
                   template.hex[x, z].hex_go.name = x + "," + z;
            }
        }
    }
}

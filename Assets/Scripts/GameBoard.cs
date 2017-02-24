using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{

    public GameObject hexPrefab;
    public GameObject hexBoard;

    string[] resources = new string[5]
    {
       "Wood", "Ore", "Grain", "Wool", "Brick"
    };

    const int WIDTH = HexTemplate.WIDTH;
    const int HEIGHT = HexTemplate.HEIGHT;

    HexTemplate template = new HexTemplate();

    float xOffset = .751f;
    float zOffset = .886f;

    // Use this for initialization
    //public void SpawnBoard(Hex[,] hex, int WIDTH, int HEIGHT)
    //  {
    void Start()
    {
        FileHandler reader = new FileHandler();
        template = BoardManager.template;
        // reader.saveMap(template);

        for (int z = 0; z < HEIGHT; z++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                hexPrefab.name = "hex " + x + "," + z;

                float zPos = z * zOffset;
                if (x % 2 == 1 || x % 2 == -1)
                {
                    zPos += (zOffset * .5f);
                }

                switch (template.hex[x, z].resource)
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
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.yellow;
                        break;
                    case 3:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.white;
                        break;
                    case 4:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.red;
                        break;
                    default:
                        template.hex[x, z].hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(x * xOffset, 0.1f, zPos), Quaternion.identity); ;
                        template.hex[x, z].hex_go.GetComponentInChildren<Renderer>().material.color = Color.blue;
                        break;
                }
                template.hex[x, z].hex_go.name = x + "," + z;
            }
        }
    }
}

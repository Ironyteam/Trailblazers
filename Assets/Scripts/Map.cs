using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour
{

    public GameObject hexPrefab;

    // size of map in terms of number of hex tiles
    // not representative of amount of world space
    // (i.e. our tiles might be more or less than 1 Unity World space)
    int width = 6;
    int height = 6;

    float xOffset = 0.882f;
    float zOffset = 0.764f;



    // Use this for initialization
    void Start()
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xPos = x * xOffset;

                // Odd row?
                if (y % 2 == 1)
                {
                    xPos += xOffset / 2f;
                }

                GameObject hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(xPos, 0, y * zOffset), Quaternion.identity);

                hex_go.name = "Hex_" + x + "_" + y;

                hex_go.transform.SetParent(this.transform);

                hex_go.isStatic = true;

                MeshRenderer mr = hex_go.GetComponentInChildren<MeshRenderer>();

                switch (Random.Range(0, 5))
                {
                    case 1:
                        mr.material.color = Color.red;
                        break;
                    case 2:
                        mr.material.color = Color.blue;
                        break;
                    case 3:
                        mr.material.color = Color.green;
                        break;
                    case 4:
                        mr.material.color = Color.yellow;
                        break;
                    default:
                        mr.material.color = Color.white;
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

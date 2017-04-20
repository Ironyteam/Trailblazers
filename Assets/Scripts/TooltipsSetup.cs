using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipsSetup : MonoBehaviour
{
    public const int LINE_HEIGHT = 15;
    public const int LETTER_WIDTH = 10;

    private string[] tipText = new string[12]
    {
        "Brick",
        "Gold",
        "Ore",
        "Wool",
        "Wheat",
        "Wood",
        "Attack another"       + "\nplayer's city",
        "Buy armies for"       + "\nyour cities",
        "Upgrade a Settlement" + "\nto a city"
                               + "\nCost: " + Constants.WheatPerCity        + " wheat, "
                                            + Constants.OrePerCity          + " ore",

        "Build a Settlement"   + "\nCost: " + Constants.BricksPerSettlement + " brick, "
                                            + Constants.WoodPerSettlement   + " wood,\n"
                                            + Constants.WheatPerSettlement  + " wheat, "
                                            + Constants.WoolPerSettlement   + " wool",
        "Build a Road",
        "Buy and sell"         + "\nresources"
    };

    private string[] elementNames = new string[12]
    {
        "brickImage", "goldImage", "oreImage", "sheepImage", "wheatImage", "woodImage",
        "Attack Button", "Barracks btn", "City Button", "Settlement Button", "Road Button", "Market Button"
    };

    // Use this for initialization
    void Start()
    {
        // Set up tooltips for non-player ability elements
        for (int index = 0; index < elementNames.Length; index++)
        {
            // Create the tooltip text as a child of the game object
            GameObject GO = GameObject.Find(elementNames[index]);
            if (GO == null)
            {
                Debug.Log("No object found with the name" + elementNames[index]);
            }
            else
            {
                if (GO.GetComponent<BoxCollider>() == null && GO.GetComponent<Button>() == null)
                    GO.AddComponent<BoxCollider>();
                GameObject tooltip = new GameObject("tooltip");
                tooltip.transform.SetParent(GO.transform);

                tooltip.AddComponent<ContentSizeFitter>();
                tooltip.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                tooltip.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                if (GO.GetComponent<Button>() != null)
                {
                    // tooltip position for action buttons
                    tooltip.transform.localPosition = new Vector3(-35, 35, 0);
                }
                else
                {
                    // tooltip positions for resource types and gold images
                    tooltip.transform.localPosition = new Vector3(-35, 20, 0);
                }

                GO.AddComponent<Tooltips>();

                Text content = tooltip.AddComponent<Text>();
                content.enabled = false;

                if (tipText[index].Length <= 4)
                    tipText[index] = string.Concat(tipText[index], " ");
                content.text = tipText[index];
                content.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                content.fontSize = 16;
                content.color = Color.red;
            }
        }

        // Setup tooltips for player images (abilities)
        for (int player = 1; player <= Constants.MaxPlayers; player++)
        {
            GameObject image = GameObject.Find("3 player" + player + "Image");

            if (image == null)
            {
                Debug.Log("No object found with the name 3 player" + player + "Image");
            }
            else
            {
                if (image.GetComponent<BoxCollider>() == null && image.GetComponent<Button>() == null)
                    image.AddComponent<BoxCollider>();

                GameObject tooltip = new GameObject("tooltip");
                tooltip.transform.SetParent(image.transform);

                // tooltip position for character abilities on player images
                tooltip.transform.localPosition = new Vector3(10, -118, 0);

                image.AddComponent<AbilitiesTooltips>();

                Text content = tooltip.AddComponent<Text>();
                content.enabled = false;
                content.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                content.fontSize = 16;
                content.color = Color.red;
            }
        }
		
		// Set up tooltips on Longest Road images
		for (int player = 1; player <= Constants.MaxPlayers; player++)
        {
            GameObject image = GameObject.Find("7 player" + player + "LongestRoadBackground");

            if (image == null)
            {
                Debug.Log("No object found with the name 7 player" + player + "Image");
            }
            else
            {
                if (image.GetComponent<BoxCollider>() == null && image.GetComponent<Button>() == null)
                    image.AddComponent<BoxCollider>();

                GameObject tooltip = new GameObject("tooltip");
                tooltip.transform.SetParent(image.transform);

                // tooltip position for Longest road images
                tooltip.transform.localPosition = new Vector3(30, -140, 0);

                image.AddComponent<Tooltips>();

                Text content = tooltip.AddComponent<Text>();
                content.enabled = false;
                content.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                content.fontSize = 16;
                content.color = Color.red;
				content.text = "Longest Road";
            }
        }
		
		// Set up tooltips on Victory Points images
		for (int player = 1; player <= Constants.MaxPlayers; player++)
        {
            GameObject image = GameObject.Find("4 player" + player + "VPBackground");

            if (image == null)
            {
                Debug.Log("No object found with the name 4 player" + player + "VPBackground");
            }
            else
            {
                if (image.GetComponent<BoxCollider>() == null && image.GetComponent<Button>() == null)
                    image.AddComponent<BoxCollider>();

                GameObject tooltip = new GameObject("tooltip");
                tooltip.transform.SetParent(image.transform);

                // tooltip position for Victory Points images
                tooltip.transform.localPosition = new Vector3(10, -105, 0);

                image.AddComponent<Tooltips>();

                Text content = tooltip.AddComponent<Text>();
                content.enabled = false;
                content.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                content.fontSize = 16;
                content.color = Color.red;
				content.text = "Victory Points";
            }
        }
		
		// Set up tooltips on Largest Army images
		for (int player = 1; player <= Constants.MaxPlayers; player++)
        {
            GameObject image = GameObject.Find("8 player" + player + "LargestArmyBackground");

            if (image == null)
            {
                Debug.Log("No object found with the name 8 player" + player + "LargestArmyBackground");
            }
            else
            {
                if (image.GetComponent<BoxCollider>() == null && image.GetComponent<Button>() == null)
                    image.AddComponent<BoxCollider>();

                GameObject tooltip = new GameObject("tooltip");
                tooltip.transform.SetParent(image.transform);

                // tooltip position for Largest army images
                tooltip.transform.localPosition = new Vector3(20, -70, 0);

                image.AddComponent<Tooltips>();

                Text content = tooltip.AddComponent<Text>();
                content.enabled = false;
                content.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                content.fontSize = 16;
                content.color = Color.red;
				content.text = "Largest Army";
            }
        }
    }
}
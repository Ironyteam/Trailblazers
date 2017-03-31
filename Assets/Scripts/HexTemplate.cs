using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexTemplate
{
    public const int WIDTH = 12;
    public const int HEIGHT = 10;

    public string mapName;
    public int minVP;
    public int maxVP;

    public Hex[,] hex = new Hex[WIDTH, HEIGHT];

    public HexTemplate() { }

    public HexTemplate(string name, int minimumVP, int maximumVP)
    {
        mapName = name;
        minVP = minimumVP;
        maxVP = maximumVP;
    }

    //public int getMinVP() { return minVP; }
    // public int getMaxVP() { return maxVP; }

    // Returns a string formatted for saving the template to a file
    public string ToFileString()
    {
        string fileString;

        fileString = mapName + "\r\n" +
                     "Bob"   + "\r\n" +            // Add creator
                     minVP   + "\r\n" +
                     maxVP   + "\r\n";

        for (int x = 0; x < WIDTH; x++)
        {
            for (int z = 0; z < HEIGHT; z++)
            {
                fileString = string.Concat(fileString, hex[x, z].resource + " " + hex[x, z].dice_number +
                                           " " + hex[x, z].portSide + " | ");
            }
            if (x != (WIDTH - 1))
                fileString = string.Concat(fileString, "\r\n");
        }
        return fileString;
    }

    // Randomizes the resources of the template maintaining the current number of each resource type
    public void randomizeResources()
    {
        int randomNum;
        Hex desertHex = null; // Desert hex that is temporarily removed from list during randomization
        List<Hex> landHexes = new List<Hex>();
        List<int> availableResourceNums = new List<int>(); // List from which numbers will be drawn

        // Form a list of all land hexagons
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (this.hex[x, y].resource >= 0)
                {
                    landHexes.Add(this.hex[x, y]);
                }
            }
        }

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

    // Randomizes the dice numbers of the template maintaining the current amount of each dice number
    public void randomizeDiceNums()
    {
        int randomNum;
        Hex desertHex = null; // Desert hex that is temporarily removed from list during randomization
        List<Hex> landHexes = new List<Hex>();
        List<int> availableDiceNums = new List<int>(); // List from which numbers will be drawn

        // Form a list of all land hexagons
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                if (this.hex[x, y].resource >= 0)
                {
                    landHexes.Add(this.hex[x, y]);
                }
            }
        }

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

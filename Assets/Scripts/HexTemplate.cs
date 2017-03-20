using UnityEngine;
using System.Collections;

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
}

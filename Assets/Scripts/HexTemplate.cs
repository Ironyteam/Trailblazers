using UnityEngine;
using System.Collections;

public class HexTemplate
{
    public const int WIDTH = 12;
    public const int HEIGHT = 10;

    public Hex[,] hex = new Hex[WIDTH, HEIGHT];

    public HexTemplate() { }
}

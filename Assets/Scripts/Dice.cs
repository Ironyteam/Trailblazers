using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simulates the roll of a single die. 
public static class Dice
{
    public static int Roll()
    {
        return Random.Range(1, 6);
    }
}

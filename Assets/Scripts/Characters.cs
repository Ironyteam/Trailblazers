using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds character data for the project.
public static class Characters
{
    public static readonly string[] Names = {"Natty Bumppo", "Gamly the Red", "Scary Harry", "Ganzo", "Queen Apala", "Rosa del Fuego", "Abiha the Exiled", "Maiden of Dunshire"};

    public static bool[] PlayerChosen = new bool[8];

    public static void ResetPlayers()
    {
        for (int x = 0; x < 8; x++)
            PlayerChosen[x] = false;
    }

    public const int Frontiersman = 0;       // Receives +1 Wood whenever a Settlement or City gains Wood. Roads cost 2X Wood cost and 0X Brick cost.
    public const int General = 1;            // Receives a 50% discount when hiring an Army.
    public const int ConspiracyTheorist = 2; // Invulnerable to the robber, resources cannot be stolen.
    public const int Merchant = 3;           // Starts game with +2 of every resource.
    public const int Queen = 4;              // Receieves a 1.5x Gold bonus for each Settlement or City.
    public const int Engineer = 5;           // Receives +1 Ore whenever a Settlement or City gains Ore. One Army is added to a City at creation.
    public const int Nomad = 6;              // Receives +1 Wool whenever a Settlement or City gains Wool. Receieves a +2 bonus toward Longest Road calculation.
    public const int Knight = 7;             // Receives +1 Brick whenever a Settlement or City gains Brick. Receieves a +4 bonus toward Largest Army calculation.  
}

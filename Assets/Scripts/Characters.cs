using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds character data for the project.
public static class Characters
{
    public static readonly string[] Names = {"Natty Bumppo", "Gamly the Red", "Scary Harry", "Ganzo", "Queen Apala", "Rosa del Fuego", "Abiha the Exiled", "Maiden of Dunshire"};
	public static readonly string[] abilitiesText = {"Natty Bumppo - \r\n" + 
														 "\tA skilled frontiersman that is accustomed to life in the woods.\r\n" +
														 "\tHe is very crafty with lumber and can make just about anything with it.\r\n\r\n" + 
														 "\tAbility: Roads cost two wood instead of one wood and one brick.\r\n" +
														 "\tWood resource tiles give one extra wood.",
													 "Gamly the Red -\r\n" +
														 "\tA proud dwarf general that boasts in his strength and his beard.\r\n" +
														 "\tSoldiers flock to him for leadership because of his strength.\r\n\r\n" +
														 "\tAbility: Cost of hiring an army is 50% less.",
													 "Scary Harry -\r\n" +
													 	"\tA paranoid conspiracy theorist who could convince anyone about his theories.\r\n" +
													 	"\tNot even a robber would want to go near this man's contagious paranoia.\r\n\r\n" +
														"\tAbility: The robber cannot steal any resources from this character.",
													 "Ganzo -\r\n" +
														"\tA diligent traveling merchant who is always prepared for any situation.\r\n" +
														"\tHe has traveled countless lands with a backpack and cart full of merchandise.\r\n\r\n" +
														"\tAbility: Begin game with two of every resource.", 
													 "Queen Apala -\r\n" +
														"\tA wealthy queen who stuns people with her beauty and elegance.\r\n" +
														"\tHer subjects are so charmed by her that they work extra hard to please her.\r\n\r\n" +
														"\tAbility: Settlements and cities gain 1.5x more gold.",
													 "Rosa del Fuego -\r\n" +
														"\tA talented engineer who knows nothing of a life without grit and elbow grease.\r\n" +
														"\tMany soldiers owe her a favor because of all the weapons she has repaired.\r\n\r\n" +
														"\tAbility: Cities automatically come with one army unit.\r\n" +
														"\tOre resource tiles give one extra ore.",
													 "Abiha the Exiled -\r\n" +
														"\tA wandering nomad who was exiled from her homeland for someone else's sin.\r\n" +
														"\tShe was once a shepherdess, but now she travels the world to find people to help.\r\n\r\n" +
														"\tAbility: Bonus two points is added to the longest road score.\r\n" +
        												"\tWool resource tiles give one extra wool.",
													 "Maiden of Dunshire -\r\n" +
													 	"\tA respected knight who is known for her valor, kindness, and origins.\r\n" +
														"\tBefore becoming a knight, she was an orphan that worked with clay and brick\r\n" +
														"\tto get by.\r\n\r\n" +
														"\tAbility: Bonus four points is added to the largest army score.\r\n" +
 												        "\tBrick resource tiles give one extra brick."};
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

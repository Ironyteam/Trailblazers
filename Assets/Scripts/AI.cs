using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AI
{
    // Every dice number listed from highest probability (index 0) to lowest probability (last index)
    private static readonly int[] diceNumsByProbability = new int[11]
    {
        6, 8, 9, 5, 4, 10, 11, 3, 12, 2, 7
    }; 

    public static void placeStartingItems(Player player)
    {
      /*  //var highestRankingVertex; //<-- Unknown type
        int hightRank = 1000; // The highest quality rating of a vertex found (lower the better)
        int vertexRank;     // Used to accumulate each vertex's rank
        int diceNumQuality;   // Used to accumulate the quality of dice numbers surrounding each vertex
        int[] diceNums;       // Stores the dice numbers surrounding each vertex
        int[] resources;      // Stores the resource types surrounding each vertex
        List<int> uniqueResources = new List<int>();
                              // Stores one instance of each resource that surrounds a vertex

        // Search vertices of active hexagons that are at least two roads away from other settlements
        // and determine which ones are surrounded by the best quality dice numbers and the greatest variety of
        // resources   */

        // Loop through all available vertices to find the best place for a settlement
     /* foreach (availableVertex)
        {
            // Determine the quality of the dice numbers surrounding the vertex
            diceNumQuality = 0;
            diceNums = getSurroundingDiceNums(vertex); // <-- Dummey Function
            foreach(int num in diceNums)
            {
                diceNumQuality += diceNumsByProbability.IndexOf(num);
            }
            vertexRank = diceNumQuality;

            // Determine the variety of the resources surrounding the vertex
            resources = getSurroundingResources(vertex); // <-- Dummey Function
            uniqueResources.Clear();
            uniqueResources.Add(resources[0]);
            
            for (int nextResource = 1; nextResource < resources.length; nextResource++)
            {
                if (!uniqueResources.Contains(resources[nextResource]))
                {
                    uniqueResources.Add(resources[nextResource]);
                }
            }
            vertexRank -= uniqueResources.Count;

            // Determine how close the vertex is to the first settlement placed if applicable
            if (PlacingSecondSettlement)
                vertexRank += getDistanceFromFirstSettlement(); // <-- Dummey Function

            // See if the current vertex is the best one found so far
            if (vertexRank < highestRank)
            {
                highestRankingVertex = vertex;
                highestRank = vertexRank;
            }
        }

        // BuildSettlement(highestRankingVertex);    // <-- Dummey Function

        // Determine which direction to build road
        // Toward other placed settlement???
     */
     
    }

    public static void executeTurn(Player player)
    {
     /*   int goldNeededToBuild;
        int brickNeeded;
        int oreNeeded;
        int wheatNeeded;
        int woodNeeded;
        int woolNeeded;
        bool buildingCompleted = true;

        // Attempt to upgrade settlements to cities
        if (settlements to upgrade to cities exists)
        {
            if (player.CanBuildCity())
            {
                player.BuildCity();
            }
            else
            {
                // Calculate additional resources needed to build city and associated cost
                brickNeeded       = player.Bricks - Constants.BricksPerCity;
                oreNeeded         = player.Ore - Constants.OrePerCity;
                wheatNeeded       = player.Wheat - Constants.WheatPerCity;
                woodNeeded        = player.Wood - Constants.WoodPerCity;
                woolNeeded        = player.Wool - Constants.WoolPerCity;
                goldNeededToBuild = brickNeeded + oreNeeded + wheatNeeded + woodNeeded + woolNeeded * Constansts.ResourceCost;

                // Buy needed resources if AI has enough gold and build the city
                if (player.Gold >= goldNeededToBuild)
                {
                    int index;
                    for (index = 0; index < brickNeeded; index++)
                        player.BuyBrick();
                    for (index = 0; index < oreNeeded; index++)
                        player.BuyOre();
                    for (index = 0; index < wheatNeeded; index++)
                        player.BuyWheat();
                    for (index = 0; index < woodNeeded; index++)
                        player.BuyWood();
                    for (index = 0; index < woolNeeded; index++)
                        player.BuyWool();

                    player.BuildCity();
                    buildingCompleted = true;
                }
                
            }
        }

        // Attempt to build a settlements
        if (buildingCompleted == false && places are available to build settlements)
        {
            if (player.CanBuildSettlement())
            {
                player.BuildSettlement();
            }
            else
            {
                // Calculate additional resources needed to build settlement and associated cost
                brickNeeded       = player.Bricks - Constants.BricksPerSettlement;
                oreNeeded         = player.Ore - Constants.OrePerSettlement;
                wheatNeeded       = player.Wheat - Constants.WheatPerSettlement;
                woodNeeded        = player.Wood - Constants.WoodPerSettlement;
                woolNeeded        = player.Wool - Constants.WoolPerSettlement;
                goldNeededToBuild = brickNeeded + oreNeeded + wheatNeeded + woodNeeded + woolNeeded * Constansts.ResourceCost;

                // Buy needed resources if AI has enough gold and build the settlement
                if (player.Gold >= goldNeededToBuild)
                {
                    int index;
                    for (index = 0; index < brickNeeded; index++)
                        player.BuyBrick();
                    for (index = 0; index < oreNeeded; index++)
                        player.BuyOre();
                    for (index = 0; index < wheatNeeded; index++)
                        player.BuyWheat();
                    for (index = 0; index < woodNeeded; index++)
                        player.BuyWood();
                    for (index = 0; index < woolNeeded; index++)
                        player.BuyWool();

                    player.BuildSettlement();
                    buildingCompleted = true;
                }

            }
        }

        // Attempt to build a road
        if (buildingCompleted == false && places are available to build roads      /*<-- Could this be false??*/ //)
    /*    {
            if (player.CanBuildRoad())
            {
                player.BuildRoad();
            }
            else
            {
                // Calculate additional resources needed to build road and associated cost
                brickNeeded       = player.Bricks - Constants.BricksPerRoad;
                oreNeeded         = player.Ore - Constants.OrePerRoad;
                wheatNeeded       = player.Wheat - Constants.WheatPerRoad;
                woodNeeded        = player.Wood - Constants.WoodPerRoad;
                woolNeeded        = player.Wool - Constants.WoolPerRoad;
                goldNeededToBuild = brickNeeded + oreNeeded + wheatNeeded + woodNeeded + woolNeeded * Constansts.ResourceCost;

                // Buy needed resources if AI has enough gold and build the road
                if (player.Gold >= goldNeededToBuild)
                {
                    int index;
                    for (index = 0; index < brickNeeded; index++)
                        player.BuyBrick();
                    for (index = 0; index < oreNeeded; index++)
                        player.BuyOre();
                    for (index = 0; index < wheatNeeded; index++)
                        player.BuyWheat();
                    for (index = 0; index < woodNeeded; index++)
                        player.BuyWood();
                    for (index = 0; index < woolNeeded; index++)
                        player.BuyWool();

                    player.BuildRoad();
                    buildingCompleted = true;
                }
            }
        }

        // Attempt to buy armies
        if (player.cities > 0)
        {
            // Check for cities owned by player that have less than 2 armies
        }*/
    }
	
}

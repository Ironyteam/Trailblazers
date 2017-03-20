using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapUtility
{
	public static Coordinate CalculateVerticeOne(Coordinate hexCoordinate)
	{
        // X Coord formula: (x*2)+1;
        // Y Coord formula: (x%2)+(y*2)+2;
        Coordinate newCoordinate = new Coordinate(((hexCoordinate.X * 2) + 1), ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2) + 2));

        return newCoordinate;
	}
	
	public static Coordinate CalculateVerticeTwo(Coordinate hexCoordinate)
	{
        // X Coord formula: (x*2)+2;
        // Y Coord formula: (x%2)+(y*2)+2;
        Coordinate newCoordinate = new Coordinate(((hexCoordinate.X * 2) + 2), ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2) + 2));

        return newCoordinate;
    }
	
	public static Coordinate CalculateVerticeThree(Coordinate hexCoordinate)
	{
        // X Coord formula: (x*2)+3;
        // Y Coord formula: (x%2)+(y*2)+1;
        Coordinate newCoordinate = new Coordinate(((hexCoordinate.X * 2) + 3), ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2) + 1));

        return newCoordinate;
    }
	
	public static Coordinate CalculateVerticeFour(Coordinate hexCoordinate)
	{
        // X Coord formula: (x*2)+2;
        // Y Coord formula: (x%2)+(y*2);
        Coordinate newCoordinate = new Coordinate(((hexCoordinate.X * 2) + 2), ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2)));

        return newCoordinate;
    }
	
	public static Coordinate CalculateVerticeFive(Coordinate hexCoordinate)
	{
        // X Coord formula: (x*2)+1;
        // Y Coord formula: (x%2)+(y*2); 
        Coordinate newCoordinate = new Coordinate(((hexCoordinate.X * 2) + 1), ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2)));

        return newCoordinate;
    }
	
	public static Coordinate CalculateVerticeSix(Coordinate hexCoordinate)
	{
        // X Coord formula: (x*2);
        // Y Coord formula: (x%2)+(y*2)+1;
        Coordinate newCoordinate = new Coordinate(((hexCoordinate.X * 2)), ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2) + 1));

        return newCoordinate;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapUtility
{
	public static Coordinate CalculateVerticeOne(Coordinate hexCoordinate)
	{
		hexCoordinate.X = ((hexCoordinate.X * 2) + 1); 						   // (x*2)+1	;
		hexCoordinate.Y = ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2) + 2); // (x%2)+(y*2)+2;

		return hexCoordinate;
	}
	
	public static Coordinate CalculateVerticeTwo(Coordinate hexCoordinate)
	{
		hexCoordinate.X = ((hexCoordinate.X * 2) + 2); 						   // (x*2)+2;
		hexCoordinate.Y = ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2) + 2); // (x%2)+(y*2)+2;

		return hexCoordinate;
	}
	
	public static Coordinate CalculateVerticeThree(Coordinate hexCoordinate)
	{
		hexCoordinate.X = ((hexCoordinate.X * 2) + 3); 						   // (x*2)+3;
		hexCoordinate.Y = ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2) + 1); // (x%2)+(y*2)+1;

		return hexCoordinate;
	}
	
	public static Coordinate CalculateVerticeFour(Coordinate hexCoordinate)
	{
		hexCoordinate.X = ((hexCoordinate.X * 2) + 2); 					   // (x*2)+2	;
		hexCoordinate.Y = ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2)); // (x%2)+(y*2);

		return hexCoordinate;
	}
	
	public static Coordinate CalculateVerticeFive(Coordinate hexCoordinate)
	{
		hexCoordinate.X = ((hexCoordinate.X * 2) + 1); 				   		   // (x*2)+1	;
		hexCoordinate.Y = ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2) + 2); // (x%2)+(y*2);

		return hexCoordinate;	
	}
	
	public static Coordinate CalculateVerticeSix(Coordinate hexCoordinate)
	{
		hexCoordinate.X = ((hexCoordinate.X * 2) + 1); 						   // (x*2);
		hexCoordinate.Y = ((hexCoordinate.X % 2) + (hexCoordinate.Y * 2) + 2); // (x%2)+(y*2)+1;

		return hexCoordinate;
	}
}

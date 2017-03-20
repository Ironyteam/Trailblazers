using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {

	public GameObject Road_GO = null;
	public int PlayerOwner = -1;
	public int RoadID;

	public Coordinate SideA;
	public Coordinate SideB;

	public Road(Coordinate A, Coordinate B, int roadID)
	{
		SideA = A;
		SideB = B;
		RoadID = roadID;
	}
}

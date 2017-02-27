using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {

	public GameObject Road_GO = null;
	public int PlayerOwner = -1;

	public Coordinate SideA;
	public Coordinate SideB;

	public Road(Coordinate A, Coordinate B)
	{
		SideA = A;
		SideB = B;
	}
}

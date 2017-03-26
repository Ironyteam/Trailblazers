using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure {

	public GameObject Structure_GO = null;
	public GameObject ArmySprite_GO = null;
	public GameObject ArmyNumber_GO = null;

	public int PlayerOwner = -1;
	public int StructureID;

	public Coordinate Location = null;

	public bool IsCity = false;
	public int Armies = 0;

	public Structure(Coordinate coordinate, int structureID)
	{
		Location = coordinate;
		StructureID = structureID;
	}
}

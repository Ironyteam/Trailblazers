using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure {

	public GameObject Structure_GO = null;
	public int PlayerOwner = -1;

	public Coordinate Location = null;

	public bool IsCity = false;
	public int Armies = 0;

	public Structure(Coordinate coordinate)
	{
		Location = coordinate;
	}
}

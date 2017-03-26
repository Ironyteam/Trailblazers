using UnityEngine;
using System.Collections;

public class Hex
{
	public int dice_number = -1;
	public int portSide = -1;

	public GameObject hex_go = null;
	public GameObject token_go = null;
	public GameObject portGO = null;
	public GameObject hexOwningPort = null; 
	public GameObject waterPortHex = null;

	public int resource = -1; // Consider initializtion
	public bool[] portSides = { false, false, false, false, false, false };

	public Coordinate HexLocation;
	public Coordinate[] Coordinates = new Coordinate[6];
	public bool hasRobber = false;

	public Hex() { }

	public Hex(int resource_type, int dice_num, int port, int x, int y)
	{
		resource = resource_type;
		dice_number = dice_num;
		portSide = port;
		HexLocation = new Coordinate(x, y);
	}

	public void setResource(int resource_type) { resource = resource_type; }
	public void setDiceNum(int dice_num) { dice_number = dice_num; }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hex
{
    public int dice_number = -1;
    public GameObject hex_go = null;
    public int resource = -1; // Consider initializtion
    public int portSide = -1;
    public GameObject portGO = null;
    public GameObject hexOwningPort = null; // Hexagon with a port in this hexagon
    public GameObject waterPortHex = null;
	//public Coordinate HexLocation;
	//public Coordinate[] Coordinates = new Coordinate[6];

    public Hex() { }

	public Hex(int resource_type, int dice_num, int port, int x, int y)
    {
        resource = resource_type;
        dice_number = dice_num;
        portSide = port;
    }

    public void setResource(int resource_type) { resource = resource_type; }
    public void setDiceNum(int dice_num) { dice_number = dice_num; }
}

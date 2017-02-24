using UnityEngine;
using System.Collections;

public class Hex
{
    public int dice_number;
    public GameObject hex_go = null;
    public int resource = -1; // Consider initializtion
    public bool[] portSides = { false, false, false, false, false, false };

    public Hex() { }

    public Hex(int resource_type, int dice_num, bool[] ports)
    {
        resource = resource_type;
        dice_number = dice_num;
        portSides = ports;
    }

    public void setResource(int resource_type) { resource = resource_type; }
    public void setDiceNum(int dice_num) { dice_number = dice_num; }
}

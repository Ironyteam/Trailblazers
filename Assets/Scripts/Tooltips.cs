using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tooltips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameBoard gameBoard;
    public const string roadBtn = "Road Button";

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameBoard = GameObject.Find("Map").GetComponent<GameBoard>();
        Text text;

        try
        {
            text = transform.FindChild("tooltip").GetComponent<Text>();
            if (string.Compare(name, roadBtn) == 0)
            {
                Debug.Log(gameBoard.LocalPlayer);
                text.text = "Build a Road\n" + (gameBoard.LocalGame.PlayerList[gameBoard.LocalPlayer].playerAbility == 0 ?
                            "Cost: 2 Wood" :
                            "Cost: " + Constants.WoodPerRoad + " Wood, " + Constants.BricksPerRoad + " Brick");
            }
            text.enabled = true;
        }
        catch(Exception)
        {
            Debug.Log("Error in OnPointerEnter: Unable to find tooltip for " + name);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        try
        {
            transform.FindChild("tooltip").GetComponent<Text>().enabled = false;
        }
        catch (Exception)
        {
            Debug.Log("Error in OnPointerExit: Unable to find tooltip for " + name);
        }
    }
}

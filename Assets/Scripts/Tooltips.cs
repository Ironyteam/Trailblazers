using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tooltips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            transform.FindChild("tooltip").GetComponent<Text>().enabled = true;
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

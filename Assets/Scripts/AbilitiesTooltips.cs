using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilitiesTooltips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        string spriteName;
        int nameIndex = -1;
		
        if (transform.FindChild("tooltip") != null)
        {
            spriteName = GetComponent<Image>().sprite.name;
            
            for (int index = 0; index < Characters.Names.Length; index++)
            {
                if (string.Compare(Characters.Names[index], spriteName) == 0)
                {
                    nameIndex = index;
                    index = Characters.Names.Length;
                }
            }

            if (nameIndex >= 0)
            {
                transform.FindChild("tooltip").GetComponent<Text>().text = "Abilities:\n" +
                                Characters.condensedAbilitiesText[nameIndex];
            }
            else
            {
                Debug.Log("Unable to retrieve text for player tooltip");
            }
            transform.FindChild("tooltip").GetComponent<Text>().enabled = true;
        }
        else
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
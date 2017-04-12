using UnityEngine;  
using System.Collections;  
using UnityEngine.EventSystems;  
using UnityEngine.UI;

public class HighlightButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
	Color32 titleColor = new Color32(255,122,51,238);
	public Text theText;

	public void OnPointerEnter(PointerEventData eventData)
	{
		theText.color = titleColor; //Or however you do your color
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		theText.color = Color.black; //Or however you do your color
	}
}
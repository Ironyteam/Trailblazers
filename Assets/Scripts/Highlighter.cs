using UnityEngine;  
using System.Collections;  
using UnityEngine.EventSystems;  
using UnityEngine.UI;

public class Highlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
	public Button myButton;
	public Sprite normalButton;
	public Sprite highlightButton;

	public void OnPointerEnter(PointerEventData eventData)
	{
		myButton.image.sprite = highlightButton; //Or however you do your color
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		myButton.image.sprite = normalButton; //Or however you do your color
	}
}
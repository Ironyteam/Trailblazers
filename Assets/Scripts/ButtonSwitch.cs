using UnityEngine;  
using System.Collections;  
using UnityEngine.EventSystems;  
using UnityEngine.UI;

public class ButtonSwitch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
	public Button SoundBtn;
	public Text   myText;

	void Start ()
	{
		SoundBtn.GetComponent<Button>().onClick.AddListener (TaskOnClick);
	}

	public void TaskOnClick()
	{
		if (SoundBtn.GetComponentInChildren<Text> ().text == "Sound On")
			SoundBtn.GetComponentInChildren<Text> ().text = "Sound Off";
		else
			SoundBtn.GetComponentInChildren<Text> ().text = "Sound On";
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		myText.color = Color.red;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		myText.color = Color.black;
	}
}
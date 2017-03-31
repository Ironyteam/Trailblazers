using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour 
{
	public GameObject TextToFloat;
    public float scroll = 0.05f;  // scrolling velocity
    public float duration = 1.5f; // time to die
    public float alpha;
	
	public FloatingText(GameObject createdText, Color resourceColor, int numberChanged)
	{
		TextToFloat = createdText;
		TextToFloat.GetComponent<Renderer>().material.color = resourceColor;  // set text color
	}

	void Start () 
	{
		alpha = 1;
	}
	
	void Update () 
	{
		if (alpha>0)
		{
			Vector3 temp = new Vector3(0f, (float)scroll*Time.deltaTime,0f);
			TextToFloat.transform.position += temp;
			alpha -= Time.deltaTime/duration; 
			Color color = TextToFloat.GetComponent<Renderer>().material.color;
			color.a = alpha; 
			TextToFloat.GetComponent<Renderer>().material.color = color;        
		} 	
		else 
		{
			Destroy(TextToFloat); // text vanished - destroy itself
		}
	}
}
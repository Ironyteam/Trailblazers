using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class camea : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	   if(Input.GetMouseButton(1))
       {
         GetComponent<Transform>().position = new Vector3(transform.position.x -
		 Input.GetAxis("Mouse X"), transform.position.y, transform.position.z  - 
		 Input.GetAxis("Mouse Y"));
       }
	}
}

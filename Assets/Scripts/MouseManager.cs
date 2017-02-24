using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class MouseManager : MonoBehaviour {

    public AudioClip[] audioClip;
    public gameUIScript gameScriptLink;
	
	// Update is called once per frame
	void Update () {

      //  Debug.Log("Mouse Position: " + Input.mousePosition);

        // this only works in orthographic view
        // Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Debug.Log("World Point: " + worldPoint);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (Physics.Raycast (ray, out hitInfo)) {
				GameObject ourHitObject = hitInfo.collider.transform.gameObject;

				//Debug.Log("Raycast hit: " + ourHitObject.name);


				if (Input.GetMouseButtonDown (0)) {
					MeshRenderer mr = ourHitObject.GetComponentInChildren<MeshRenderer> ();

					if (mr.material.color == Color.red) {
						mr.material.color = Color.yellow;
						PlaySound (0);
						gameScriptLink.moreSheep ();
					} else if (mr.material.color == Color.blue) {
						mr.material.color = Color.green;
						PlaySound (1);
						gameScriptLink.moreOre ();
					} else if (mr.material.color == Color.green) {
						mr.material.color = Color.white;
						PlaySound (2);
						gameScriptLink.moreWood ();
					} else if (mr.material.color == Color.yellow) {
						mr.material.color = Color.blue;
						PlaySound (3);
						gameScriptLink.moreWheat ();
					} else {
						mr.material.color = Color.red;
						PlaySound (4);
						gameScriptLink.moreBrick ();
					}
	               
				}
			}
		}
	}

    void PlaySound(int clip)
    {
        GetComponent<AudioSource>().clip = audioClip[clip];
        GetComponent<AudioSource>().Play();
    }
}

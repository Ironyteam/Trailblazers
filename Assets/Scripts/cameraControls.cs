using UnityEngine;
using System.Collections;

public class cameraControls : MonoBehaviour {
    // Update is called once per frame



    void Update ()
    {
        if((Input.GetAxis("Mouse ScrollWheel") > 0) && (GetComponent<Camera>().fieldOfView > 20))
            GetComponent<Camera>().fieldOfView -= 4;
        else if((Input.GetAxis("Mouse ScrollWheel") < 0) && (GetComponent<Camera>().fieldOfView < 100))
            GetComponent<Camera>().fieldOfView += 4;

        if(Input.GetMouseButton(1))
        {
			GetComponent<Transform>().position = new Vector3(Mathf.Clamp(transform.position.x -
				Input.GetAxis("Mouse X"), Constants.CAMERA_MIN_X, Constants.CAMERA_MAX_X), transform.position.y, Mathf.Clamp(transform.position.z  - 
					Input.GetAxis("Mouse Y"), Constants.CAMERA_MIN_Z, Constants.CAMERA_MAX_Z));
        }
    }
}

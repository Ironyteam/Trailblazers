using UnityEngine;
using System.Collections;

public class cameraControls : MonoBehaviour {
    // Update is called once per frame
    void Update ()
    {
        if((Input.GetAxis("Mouse ScrollWheel") > 0) && (GetComponent<Camera>().fieldOfView > 20))
            GetComponent<Camera>().fieldOfView -= 4;
        else if((Input.GetAxis("Mouse ScrollWheel") < 0) && (GetComponent<Camera>().fieldOfView < 60))
            GetComponent<Camera>().fieldOfView += 4;

        if(Input.GetMouseButton(1))
        {
            GetComponent<Transform>().position = new Vector3(transform.position.x -
            Input.GetAxis("Mouse X"), transform.position.y, transform.position.z  - 
            Input.GetAxis("Mouse Y"));
        }
    }
}

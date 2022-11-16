using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //This script allows simple camera movementfor a top down camera. 
    //This script was taken from https://dev.to/matthewodle/simple-3d-camera-movement-1lcj. Some alterations have been made. 
    private float moveSpeed = 0.075f;
    private float scrollSpeed = 100.0f;
    public GameObject ps; 
    public GameObject star; 
    //Star starData;

    void Start() {
        //starData = ps.GetComponent<Star>(); 
        //transform.LookAt(Vector3.zero);
        Camera.main.transform.position = new Vector3 (0.0f, star.transform.localScale.y + (star.transform.localScale.y/2.0f), 0.0f);      
    }

    void Update () {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            //transform.position += moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            Camera.main.transform.position += scrollSpeed * new Vector3(0, -Input.GetAxis("Mouse ScrollWheel"), 0);
            //Debug.Log(Camera.main.transform.position + " MOUSE SCROLL"); 
        }
    }
}

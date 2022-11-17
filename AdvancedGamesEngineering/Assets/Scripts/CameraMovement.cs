using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class CameraMovement : MonoBehaviour
{
    //This script allows simple camera movementfor a top down camera. 
    //This script was taken from https://dev.to/matthewodle/simple-3d-camera-movement-1lcj. Some alterations have been made. 
    private float moveSpeed = 0.075f;
    private float scrollSpeed = 100.0f;
    public GameObject ps; 
    public GameObject star; 
    GameObject[] celestialBodies;
    int cameraIndex = 0;
    //Star starData;

    void Start() {
        //starData = ps.GetComponent<Star>(); 
        //transform.LookAt(Vector3.zero);
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        Camera.main.transform.position = new Vector3 (0.0f, star.transform.localScale.y + (star.transform.localScale.y/2.0f), 0.0f);      
    }

    void Update () {
        //Cycle up through the cameras
        Debug.Log(cameraIndex); 
        if(Input.GetKeyDown(KeyCode.T) && cameraIndex < celestialBodies.Length-1){
            cameraIndex++;  
        }

        //Cycle down through the cameras
        if(Input.GetKeyDown(KeyCode.G) && cameraIndex > 0){
            cameraIndex--; 
        }
    }

    void LateUpdate(){
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            //transform.position += moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            //Camera.main.transform.position += scrollSpeed * new Vector3(celestialBodies[cameraIndex].transform.position.x, -Input.GetAxis("Mouse ScrollWheel"), celestialBodies[cameraIndex].transform.position.z);
        }
    }
}

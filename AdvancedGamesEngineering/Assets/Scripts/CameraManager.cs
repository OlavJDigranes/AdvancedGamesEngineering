using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera[] cameras; 
    GameObject[] celestialBodies;
    int cameraIndex = 0; 
    
    // Start is called before the first frame update
    void Start()
    {
        cameras = Camera.allCameras; 
        //Camera.GetAllCameras(cameras); 
        for(int i = 1; i < cameras.Length; i++){
            cameras[i].enabled = false; 
        }
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
    }

    // Update is called once per frame
    void Update()
    {
        //Cycle up through the cameras
        if(Input.GetKeyDown(KeyCode.T)){
            if(cameraIndex < cameras.Length){
                cameraIndex++; 
                cameras[cameraIndex].enabled = true; 
                cameras[cameraIndex - 1].enabled = false; 
            }
        }

        //Cycle down through the cameras
        if(Input.GetKeyDown(KeyCode.G)){
            if(cameraIndex > 0){
                cameraIndex--; 
                cameras[cameraIndex].enabled = true; 
                cameras[cameraIndex + 1].enabled = false; 
            }
        }
    }

    void LateUpdate() {
        //Counteract planet rotation
        for(int i = 1; i < cameras.Length; i++){
            cameras[i].transform.rotation = Quaternion.Euler(90.0f, celestialBodies[i].transform.rotation.y * -1.0f, 0.0f); 
        }   
    }
}

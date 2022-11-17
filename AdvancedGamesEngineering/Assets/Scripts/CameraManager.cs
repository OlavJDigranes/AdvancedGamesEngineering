using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;
using TMPro; 

public class CameraManager : MonoBehaviour
{
    Camera[] cameras; 
    GameObject[] celestialBodies;
    int cameraIndex = 0; 
    private float scrollSpeed = 100.0f;
    Vector3 scrolling = new Vector3(0.0f, 1.0f, 0.0f); 

    public GameObject star; 
    public TextMeshProUGUI numPlanOut; 
    public TextMeshProUGUI celBod;
    public TextMeshProUGUI posOut;
    public TextMeshProUGUI velOut; 
    public GameObject UIPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        cameras = Camera.allCameras; 
        //Camera.GetAllCameras(cameras); 
        //for(int i = 1; i < cameras.Length; i++){
            //cameras[i].enabled = false; 
        //}
        //Camera.main.transform.position += new Vector3 (0.0f, star.transform.localScale.y + (star.transform.localScale.y/2.0f), 0.0f);
        scrolling += new Vector3 (0.0f, star.transform.localScale.y + (star.transform.localScale.y/2.0f), 0.0f);
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        numPlanOut.text = (celestialBodies.Length-1).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //Cycle up through the cameras
        if(Input.GetKeyDown(KeyCode.T) && cameraIndex < celestialBodies.Length-1){
                cameraIndex++; 
                //cameras[cameraIndex].enabled = true; 
                //cameras[cameraIndex - 1].enabled = false; 
        }

        //Cycle down through the cameras
        if(Input.GetKeyDown(KeyCode.G) && cameraIndex > 0){
                cameraIndex--; 
                //cameras[cameraIndex].enabled = true; 
                //cameras[cameraIndex + 1].enabled = false; 
        }

        if(celBod.text != (cameraIndex+1).ToString()){
            celBod.text = (cameraIndex+1).ToString();
        }

        posOut.text = celestialBodies[cameraIndex].transform.position.ToString();
        velOut.text = celestialBodies[cameraIndex].GetComponent<Rigidbody>().velocity.ToString();

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    void LateUpdate() {
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            scrolling += scrollSpeed * new Vector3(0, -Input.GetAxis("Mouse ScrollWheel"), 0);    
        }
        Camera.main.transform.position = new Vector3(celestialBodies[cameraIndex].transform.position.x, scrolling.y, celestialBodies[cameraIndex].transform.position.z);   
    }
}

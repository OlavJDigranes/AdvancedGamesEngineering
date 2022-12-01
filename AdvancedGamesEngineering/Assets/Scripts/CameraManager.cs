using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;
using TMPro; 

public class CameraManager : MonoBehaviour
{
    Camera[] cameras; 
    //GameObject[] celestialBodies;
    List<GameObject> celestialBodies = new List<GameObject>(); 
    int cameraIndex = 0; 
    int planetIndex = 0;
    private float scrollSpeed = 100.0f;
    Vector3 scrolling = new Vector3(0.0f, 1.0f, 0.0f); 

    public GameObject star; 
    public TextMeshProUGUI numPlanOut; 
    public TextMeshProUGUI celBod;
    public TextMeshProUGUI planTypeOut;
    public TextMeshProUGUI habOut; 
    public TextMeshProUGUI infoOut1; 
    public TextMeshProUGUI infoOut2; 
    public TextMeshProUGUI info1; 
    public TextMeshProUGUI info2; 
    public TextMeshProUGUI nameOut; 
    public GameObject UIPanel;
    public PSManager ps; 
    
    // Start is called before the first frame update
    void Start()
    {
        cameras = Camera.allCameras; 
        scrolling += new Vector3 (0.0f, star.transform.localScale.y + (star.transform.localScale.y/2.0f), 0.0f);
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("CelestialBody")){
            celestialBodies.Add(g); 
        }
        //celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        numPlanOut.text = (celestialBodies.Count-1).ToString();

        GameObject psm = GameObject.Find("PlanetarySystem"); 
        ps = psm.GetComponent<PSManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        //Cycle up through the cameras
        if(Input.GetKeyDown(KeyCode.T) && cameraIndex < celestialBodies.Count-1){
            cameraIndex++; 
        }

        //Cycle down through the cameras
        if(Input.GetKeyDown(KeyCode.G) && cameraIndex > 0){
            cameraIndex--; 
        }

        //only updated on planet change
        if(celBod.text != (cameraIndex+1).ToString()){
            celBod.text = (cameraIndex+1).ToString();
            scrolling.y = star.transform.localScale.y + (star.transform.localScale.y/2.0f); 
            
            if(cameraIndex == 0){
                planTypeOut.text = "Star";
                habOut.text = "No"; 
                info1.text = "Spectral Type: "; 
                info2.text = "Absolute Magnitude (Mv):"; 
                infoOut1.text = MainMenuManager.spectralClassification; 
                infoOut2.text = MainMenuManager.absoluteMagnitude.ToString(); 
                nameOut.text = ps.starData.name; 
            }

            planetIndex = cameraIndex - 1; 

            if(planetIndex >= 0 && planetIndex < MainMenuManager.numOfPlanets){
                Debug.Log(planetIndex); 
                planTypeOut.text = "Planet";
                info1.text = "Planet Type: "; 
                info2.text = " "; 
                infoOut2.text = " "; 
                nameOut.text = ps.planetsOutput[planetIndex].name; 
                if(ps.planetsOutput[planetIndex].isHabitable == true){
                    habOut.text = "Yes";
                }
                if(ps.planetsOutput[planetIndex].isHabitable == false){
                    habOut.text = "No";
                }
                if(ps.planetsOutput[planetIndex].type == 1){
                    infoOut1.text = "Rocky";
                }
                if(ps.planetsOutput[planetIndex].type == 2){
                    infoOut1.text = "Gassy";
                }
            }

            if(cameraIndex > MainMenuManager.numOfPlanets){
                planTypeOut.text = "Asteroid";
                habOut.text = "No"; 
                info1.text = " ";
                info2.text = " ";
                infoOut1.text = " ";
                infoOut2.text = " ";
                int asteroidIndex = cameraIndex - (MainMenuManager.numOfPlanets + 1); 
                Debug.Log("AST IND: " + asteroidIndex + " CAN IND: " + cameraIndex); 
                nameOut.text = ps.asteroidsOutput[asteroidIndex].name; 
            }
        }

        //posOut.text = celestialBodies[cameraIndex].transform.position.ToString();
        //velOut.text = celestialBodies[cameraIndex].GetComponent<Rigidbody>().velocity.ToString();

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.R)){
            ps.GenerateAsteroid(); 
            celestialBodies.Clear(); 
            foreach(GameObject g in GameObject.FindGameObjectsWithTag("CelestialBody")){
                celestialBodies.Add(g); 
            }
        }
    }

    void LateUpdate() {
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            scrolling += scrollSpeed * new Vector3(0, -Input.GetAxis("Mouse ScrollWheel"), 0);    
        }
        Camera.main.transform.position = new Vector3(celestialBodies[cameraIndex].transform.position.x, scrolling.y, celestialBodies[cameraIndex].transform.position.z);   
    }
}

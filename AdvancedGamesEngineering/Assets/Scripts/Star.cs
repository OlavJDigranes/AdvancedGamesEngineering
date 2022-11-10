using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class Star : MonoBehaviour
{
    //Variables
    public double mass;
    public double luminocity;
    public double age;
    public double radius; 
    public string spectralClassification;
    public int absoluteMagnitude;
    public Color colour;  
    public double3 accumulatedForce; 
    public double3 position; 

    public GameObject star;
    public GameObject menu; 
    public Rigidbody rb; 
    MainMenuManager mainMenuMngr;

    Color[] colours; 

    //set up some varaiables
    void Awake() {
        mainMenuMngr = menu.GetComponent<MainMenuManager>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        //Ensure the star is at the point of origin. 
        star.transform.position = new Vector3(0, 0, 0);

        //Take in variables from menu
        spectralClassification = MainMenuManager.spectralClassification; 
        absoluteMagnitude = MainMenuManager.absoluteMagnitude; 

        Debug.Log("Sim Loaded");

        //Funcitons to generate star
        GenerateStar(absoluteMagnitude, spectralClassification);
   
    }

    void GenerateStar(int mv, string type){
        //Temp Variables
        Color tempCol; 
        float colourAlpha; 
        int mvMoved;
        int mvRev;
        float randMass = 0.0f; 
        float randScale = 0.0f; 
        float randKelvin = 0.0f;
        double scalarMass = 2*Math.Pow(10.0, 30.0); //kg
        double scalarScale = 695700.0; //km
        rb.GetComponent<Rigidbody>();

        if(type == "O"){
            randMass = (UnityEngine.Random.Range(16.1f, 150f)); 
            randScale = UnityEngine.Random.Range(6.6f, 1500.0f);
            randKelvin = UnityEngine.Random.Range(30000.0f, 40000.0f);
        }
        if(type == "B"){
            randMass = (UnityEngine.Random.Range(2.1f, 16f)); 
            randScale = UnityEngine.Random.Range(1.8f, 6.6f);
            randKelvin = UnityEngine.Random.Range(10000.0f, 30000.0f);
        }
        if(type == "A"){
            randMass = (UnityEngine.Random.Range(1.4f, 2.1f)); 
            randScale = UnityEngine.Random.Range(1.4f, 1.8f);
            randKelvin = UnityEngine.Random.Range(7500.0f, 10000.0f);
        }
        if(type == "F"){
            randMass = (UnityEngine.Random.Range(1.04f, 1.4f)); 
            randScale = UnityEngine.Random.Range(1.15f, 1.4f);
            randKelvin = UnityEngine.Random.Range(6000.0f, 7500.0f);
        }
        if(type == "G"){
            randMass = (UnityEngine.Random.Range(0.8f, 1.04f)); 
            randScale = UnityEngine.Random.Range(0.96f, 1.15f);
            randKelvin = UnityEngine.Random.Range(5200.0f, 6000.0f);
        }
        if(type == "K"){
            randMass = (UnityEngine.Random.Range(0.45f, 0.8f));
            randScale = UnityEngine.Random.Range(0.7f, 0.96f); 
            randKelvin = UnityEngine.Random.Range(3700.0f, 5200.0f);
        }
        if(type == "M"){
            randMass = (UnityEngine.Random.Range(0.08f, 0.45f)); 
            randScale = UnityEngine.Random.Range(0.1f, 0.7f);
            randKelvin = UnityEngine.Random.Range(2400.0f, 3700.0f);
        }
        
        //set mass
        mass = (double)randMass * scalarMass; 
        rb.mass = (float)mass; 

        var scale = star.transform.localScale; 

        //Set scale
        radius = (double)randScale * scalarScale; 
        scale *= (float)radius * 2.0f; 
        star.transform.localScale = scale; 

        //Colour 
        tempCol = Mathf.CorrelatedColorTemperatureToRGB(randKelvin);
        mvMoved = mv + 10;
        mvRev = 30 - mvMoved; 
        colourAlpha = (float)mvRev/30;
        tempCol.a = colourAlpha; 
        colour = tempCol; 
        var sphereRenderer = star.GetComponent<Renderer>();
        sphereRenderer.material.SetColor("_DetailColor", Color.white);
        sphereRenderer.material.SetColor("_StarColor", tempCol);
    }   
}

/*NOTES:
 - Star will always be at point 0, 0, 0.  
*/

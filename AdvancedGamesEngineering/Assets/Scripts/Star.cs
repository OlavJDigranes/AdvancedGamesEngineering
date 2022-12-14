using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

[Serializable]
public class Star : CelestialBody
{
    //Variables
    readonly double S = 1.0e-4; //Scale 
    public double luminocity;
    public double age;
    public string spectralClassification;
    public int absoluteMagnitude;
    public Color colour;  
    public double massDownscale;
    public double radDownscale; 
    public double temperature; 
    public double minHabitableRadius;
    public double maxHabitableRadius; 
    public string name; 

    MainMenuManager mainMenuMngr;

    //set up some varaiables
    public void GetMainMenuManager() {
        //mainMenuMngr = menu.GetComponent<MainMenuManager>(); 
    }

    // Start is called before the first frame update
    public void StarSetup()
    {
        //Take in variables from menu
        spectralClassification = MainMenuManager.spectralClassification; 
        absoluteMagnitude = MainMenuManager.absoluteMagnitude; 

        //Funcitons to generate star
        GenerateStar(absoluteMagnitude, spectralClassification);
    }

    public void GenerateStar(int mv, string type){
        //Temp Variables
        Color tempCol; 
        float colourAlpha; 
        int mvMoved;
        int mvRev;
        float randMass = 0.0f; 
        float randScale = 0.0f; 
        float randKelvin = 0.0f;
        double scalarMass = 2*(double)Math.Pow(10.0, 30.0); //kg
        Debug.Log(scalarMass); 
        double scalarScale = 696340.0; //km
        //rb.GetComponent<Rigidbody>();

        if(type.Equals("O") || type.Equals("o")){
            randMass = UnityEngine.Random.Range(16.1f, 150f); 
            randScale = UnityEngine.Random.Range(6.6f, 1500.0f);
            randKelvin = UnityEngine.Random.Range(30000.0f, 40000.0f);
        }
        if(type.Equals("B") || type.Equals("b")){
            randMass = UnityEngine.Random.Range(2.1f, 16f); 
            randScale = UnityEngine.Random.Range(1.8f, 6.6f);
            randKelvin = UnityEngine.Random.Range(10000.0f, 30000.0f);
        }
        if(type.Equals("A") || type.Equals("a")){
            randMass = UnityEngine.Random.Range(1.4f, 2.1f); 
            randScale = UnityEngine.Random.Range(1.4f, 1.8f);
            randKelvin = UnityEngine.Random.Range(7500.0f, 10000.0f);
        }
        if(type.Equals("F") || type.Equals("f")){
            randMass = UnityEngine.Random.Range(1.04f, 1.4f); 
            randScale = UnityEngine.Random.Range(1.15f, 1.4f);
            randKelvin = UnityEngine.Random.Range(6000.0f, 7500.0f);
        }
        if(type.Equals("G") || type.Equals("g")){
            randMass = UnityEngine.Random.Range(0.8f, 1.04f); 
            randScale = UnityEngine.Random.Range(0.96f, 1.15f);
            randKelvin = UnityEngine.Random.Range(5200.0f, 6000.0f);
        }
        if(type.Equals("K") || type.Equals("k")){
            randMass = UnityEngine.Random.Range(0.45f, 0.8f);
            randScale = UnityEngine.Random.Range(0.7f, 0.96f); 
            randKelvin = UnityEngine.Random.Range(3700.0f, 5200.0f);
        }
        if(type.Equals("M") || type.Equals("m")){
            randMass = UnityEngine.Random.Range(0.08f, 0.45f); 
            randScale = UnityEngine.Random.Range(0.1f, 0.7f);
            randKelvin = UnityEngine.Random.Range(2400.0f, 3700.0f);
        }
        if(type.Equals("T") || type.Equals("t")){
            randMass = 1.0f;
            randScale = 1.0f;
            randKelvin = 6000.0f; 
        }
        //Debug.Log(randMass + " STAR RAND MASS"); 
        //Debug.Log(randScale + " STAR RAND SCALE"); 
        //Debug.Log(randKelvin + " STAR RAND KELV"); 
        
        //set mass
        mass = (double)randMass * scalarMass; 
        //Debug.Log(mass + " STAR MASS"); 
        massDownscale = S * mass;   

        //Set scale
        radius = (double)randScale * scalarScale; 
        //Debug.Log(radius + " STAR RADIUS " + radius * 2 + " STAR DIAMETER"); 
        radDownscale = S * radius;
        //Debug.Log(radDownscale + " STAR RAD DOWNSCALE");   

        //Colour 
        temperature = randKelvin; 
        tempCol = Mathf.CorrelatedColorTemperatureToRGB(randKelvin);
        mvMoved = mv + 10;
        mvRev = 30 - mvMoved; 
        colourAlpha = (float)mvRev/30.0f;
        tempCol.a = colourAlpha; 
        colour = tempCol; 

        identifier = 0; 

        luminocity = (radius/scalarScale) * (temperature/5778) * 3.828e+26; 
        //luminocity = (radius/scalarScale) * (temperature/5778); 
        minHabitableRadius = System.Math.Sqrt((luminocity/3.828e+26)/1.1) * 149597870.691; 
        maxHabitableRadius = System.Math.Sqrt((luminocity/3.828e+26)/0.53) * 149597870.691;
        Debug.Log((minHabitableRadius * S) + " MIN HAB RANG SCALE");  
        Debug.Log((maxHabitableRadius * S) + " Max HAB RANG SCALE"); 

        GenerateName(type); 
    }   

    void GenerateName(string sc){
        char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        char[] generatedName = {'S', 'T', '-', alphabet[UnityEngine.Random.Range(0,26)], alphabet[UnityEngine.Random.Range(0,26)], alphabet[UnityEngine.Random.Range(0,26)], alphabet[UnityEngine.Random.Range(0,26)] };
        //string nameP2 = "-" + sc; 
        name = String.Concat(generatedName) + "-" + sc;
        Debug.Log(name); 
    }
}



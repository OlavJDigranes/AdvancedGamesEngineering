using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    //Variables
    int mass;
    int luminocity;
    int age;

    public GameObject star;
    public GameObject menu; 
    public Rigidbody rb; 
    MainMenuManager mainMenuMngr;

    Color[] colours; 

    //set up some varaiables
    void Awake() {
        FIllColours(); 
        mainMenuMngr = menu.GetComponent<MainMenuManager>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        //Ensure the star is at the point of origin. 
        star.transform.position = new Vector3(0, 0, 0);

        //Take in variables from menu
        mass = MainMenuManager.starMass;
        luminocity = MainMenuManager.starLuminocity;
        age = MainMenuManager.starAge;

        Debug.Log("Sim Loaded");

        //Funcitons to generate star
        DetermineColour(age);
        DetermineLightEmission(luminocity);
        DetermineSizeAndPull(mass);
   
    }

    void DetermineColour(int a){
        /*
        The age of the star determines the colour. This is a simple early implementation for the sake of proving the concept. 
        */
        var sphereRenderer = star.GetComponent<Renderer>();

        //Reverse and rotate the colour wheel as younger stars are more blue. A more accurate representation of star colour requires implementation of Hertzsprung-Russell values. 
        float rotatedAge = 360f - (float)a; 
        float ageHue = rotatedAge/360f; 

        Debug.Log(Color.HSVToRGB(ageHue, 1f, 1f));
        //Debug.Log(Color.HSVToRGB(a, Random.Range(0f, 1f), Random.Range(0f, 1f)));

        sphereRenderer.material.SetColor("_DetailColor", Color.white);
        sphereRenderer.material.SetColor("_StarColor", Color.HSVToRGB(ageHue, 1f, 1f));
    }

    void DetermineLightEmission(int l){
        //Turn the star into the light in the scene. Use colour of star to colour the emitted light. 

        //Turn alpha to a float between 0 and 1
        float alpha = (float)l/10.0f;

        //Colour handling 
        var sphereRenderer = star.GetComponent<Renderer>(); 
        Color tempColour; 
        tempColour = sphereRenderer.material.GetColor("_StarColor"); 
        tempColour.a = alpha; 
        sphereRenderer.material.SetColor("_StarColor", tempColour); 
    }

    void DetermineSizeAndPull(int m){
        //Directly set rigidbody mass.  
        rb.GetComponent<Rigidbody>();
        rb.mass = (m * 1); 
        var scale = star.transform.localScale; 

        //Set scale based on mass
        scale *= (m); 
        star.transform.localScale = scale; 
    }

    void FIllColours(){
        //This functions fills the colour array in a gradient from red to blue. 
        colours = new Color[12]; 
        colours[0] = Color.white; 
        colours[1] = Color.red;
        colours[2] = new Color(0.8f, 0.2f, 0.0f, 1.0f); 
        colours[3] = new Color(0.6f, 0.4f, 0.0f, 1.0f); 
        colours[4] = new Color(0.4f, 0.6f, 0.0f, 1.0f);
        colours[5] = new Color(0.2f, 0.8f, 0.0f, 1.0f);
        colours[6] = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        colours[7] = new Color(0.0f, 0.8f, 0.2f, 1.0f);
        colours[8] = new Color(0.0f, 0.6f, 0.4f, 1.0f);
        colours[9] = new Color(0.0f, 0.4f, 0.6f, 1.0f);
        colours[10] = new Color(0.0f, 0.2f, 0.8f, 1.0f);
        colours[11] = Color.blue; 
    }

    
}

/*NOTES:
 - Star will always be at point 0, 0, 0.  
*/

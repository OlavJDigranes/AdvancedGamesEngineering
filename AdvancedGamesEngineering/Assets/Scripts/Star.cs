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

        
        int tempAge = 0;

        if(a <= 12){
            tempAge = a;
        }
        if(a > 12){
            tempAge = 12; 
        }

        sphereRenderer.material.SetColor("_DetailColor", colours[tempAge - 1]);
        sphereRenderer.material.SetColor("_StarColor", colours[tempAge - 1]);
        
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    int mass;
    int luminocity;
    int age; 

    public GameObject star;
    public GameObject menu; 
    public Rigidbody rb; 
    MainMenuManager mainMenuMngr;

    Color[] colours; 

    void Awake() {
        FIllColours(); 
        mainMenuMngr = menu.GetComponent<MainMenuManager>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        //Ensure the star is at the point of origin. 
        star.transform.position = new Vector3(0, 0, 0);

        //mass = mainMenuMngr.starMass;
        //luminocity = mainMenuMngr.starLuminocity;
        //age = mainMenuMngr.starAge;

        mass = MainMenuManager.starMass;
        luminocity = MainMenuManager.starLuminocity;
        age = MainMenuManager.starAge;

        Debug.Log("Sim Loaded");
        //Debug.Log(mass);
        //Debug.Log(luminocity);
        //Debug.Log(age);

        DetermineColour(age);
        DetermineLightEmission(luminocity);
        DetermineSizeAndPull(mass);
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DetermineColour(int a){
        /*
        The age of the star determines the colour. 
        */
        var sphereRenderer = star.GetComponent<Renderer>();

        
        int tempAge = 0;

        if(a <= 12){
            tempAge = a;
        }
        if(a > 12){
            tempAge = 12; 
        }

        /*
        float r = 0.0f;
        float g = 0.0f;
        float b = 0.0f;

        float increment = 0.33f;

        for(int i = tempAge; i > 0; i--){
            if(i % 3 == 0){
                r += increment;
            }
            else if(i % 2 == 0){
                g += increment;
            }
            else{
                b += increment;
            }
        }
        Debug.Log(r + ", " + g + ", " + b); 
        */

        //Color starColour = new Color(r, g, b, 1.0f);
        sphereRenderer.material.SetColor("_Color", colours[tempAge - 1]);
        sphereRenderer.material.SetColor("_EmissionColor", colours[tempAge - 1]);
        
    }

    void DetermineLightEmission(int l){
        //Turn the star into the light in the scene. Use colour of star to colour the emitted light. 
        float alpha = (float)l/10.0f;
        var sphereRenderer = star.GetComponent<Renderer>(); 
        Color tempColour; 
        tempColour = sphereRenderer.material.GetColor("_EmissionColor"); 
        tempColour.a = alpha; 
        //Debug.Log(tempColour); 
        sphereRenderer.material.SetColor("_EmissionColor", tempColour); 
        Debug.Log(tempColour = sphereRenderer.material.GetColor("_EmissionColor")); 
    }

    void DetermineSizeAndPull(int m){
        //Directly set rigidbody mass. 
        //https://www.youtube.com/watch?v=kUXskc76ud8 <- make planets and moons. 
        //https://www.youtube.com/watch?v=RvIsJCGLsSU
        rb.GetComponent<Rigidbody>();
        rb.mass = (m); 
        var scale = star.transform.localScale; 
        scale *= m; 
        star.transform.localScale = scale; 
    }

    void FIllColours(){
        //This functions fills the colour array
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

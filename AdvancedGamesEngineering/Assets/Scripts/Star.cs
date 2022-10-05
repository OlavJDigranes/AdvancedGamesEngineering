using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    int mass;
    int luminocity;
    int age;

    public GameObject star;

    // Start is called before the first frame update
    void Start()
    {
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

        float r = 0.0f;
        float g = 0.0f;
        float b = 0.0f;

        float increment = 0.3f;

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
        Color starColour = new Color(r, g, b, 1.0f);
        sphereRenderer.material.SetColor("_Color", starColour);
    }

    void DetermineLightEmission(int l){

    }

    void DetermineSizeAndPull(int m){

    }
}

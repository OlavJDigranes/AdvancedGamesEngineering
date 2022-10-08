using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    int planetTotal;

    public GameObject menu; 
    MainMenuManager mainMenuMngr;

    void Awake() {
        mainMenuMngr = menu.GetComponent<MainMenuManager>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        planetTotal = MainMenuManager.numOfPlanets; 
        //Debug.Log(planetTotal); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //Take in the customising parameters and use them to generate the planetary system. 
    public static int numOfPlanets; 
    public static string spectralClassification;
    //public static int starLuminocity;
    public static int absoluteMagnitude; 

    public GameObject menuPanel;

    private string inputAbsoluteMagnitude; 
    private string inputSpectralClassification; 
    //private string inputStarLuminocity; 
    private string inputNumOfPlanets; 

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(menuPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // This function starts the simulatin using the gathered parameters. 
    public void StartSim(){
        /*
        if(int.TryParse(inputStarMass, out int mass)){
            starMass = mass; 
        } 
        if(int.TryParse(inputStarAge, out int age)){
            starAge = age; 
        } 
        */
        //if(int.TryParse(inputStarLuminocity, out int lumin)){
        //    starLuminocity = lumin; 
        //} 
        spectralClassification = inputSpectralClassification;
        if(int.TryParse(inputAbsoluteMagnitude, out int mv)){
            absoluteMagnitude = mv; 
        }
        if(int.TryParse(inputNumOfPlanets, out int nop)){
            numOfPlanets = nop; 
        } 
        SceneManager.LoadScene(1, LoadSceneMode.Single); 
    }

    //These functions are used for the InputFields in the main menu scene. Debug Logs are due to an editor bug as of 08.10.2022
    public void ReadStringinputSpectralClassification(string s){
        inputSpectralClassification = s;
        Debug.Log(s); 
    }

    public void ReadStringinputAbsoluteMagnitude(string s){
        inputAbsoluteMagnitude = s;
        Debug.Log(s); 
    }

    /*
    public void ReadStringinputStarLuminocity(string s){
        inputStarLuminocity = s;
        Debug.Log(s); 
    }
    */

    public void ReadStringinputNumOfPlanets(string s){
        inputNumOfPlanets = s;
        Debug.Log(s); 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //Take in the customising parameters and use them to generate the planetary system. Use a seperate class to hold the information. 
    public static int numOfPlanets; 
    public static int starMass;
    public static int starLuminocity;
    public static int starAge; 

    public GameObject menuPanel;

    private string inputStarMass; 
    private string inputStarAge; 
    private string inputStarLuminocity; 
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
        if(int.TryParse(inputStarMass, out int mass)){
            starMass = mass; 
        } 
        if(int.TryParse(inputStarAge, out int age)){
            starAge = age; 
        } 
        if(int.TryParse(inputStarLuminocity, out int lumin)){
            starLuminocity = lumin; 
        } 
        if(int.TryParse(inputNumOfPlanets, out int nop)){
            numOfPlanets = nop; 
        } 
        SceneManager.LoadScene(1, LoadSceneMode.Single); 
    }

    public void ReadStringinputStarMass(string s){
        inputStarMass = s;
        Debug.Log(s); 
    }

    public void ReadStringinputStarAge(string s){
        inputStarAge = s;
        Debug.Log(s); 
    }

    public void ReadStringinputStarLuminocity(string s){
        inputStarLuminocity = s;
        Debug.Log(s); 
    }

    public void ReadStringinputNumOfPlanets(string s){
        inputNumOfPlanets = s;
        Debug.Log(s); 
    }
}

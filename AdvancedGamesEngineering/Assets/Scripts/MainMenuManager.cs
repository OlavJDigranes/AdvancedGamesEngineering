using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //Take in the customising parameters and use them to generate the planetary system. Use a seperate class to hold the information. 
    public int numOfPlanets; 
    public int starMass;
    public float starLuminocity;
    public int starAge; 

    private string input1; 
    private string input2; 
    private string input3; 
    private string input4; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // This function starts the simulatin using the gathered parameters. 
    public void StartSim(){
        if(int.TryParse(input1, out int result1)){
            starMass = result1; 
        } 
        if(int.TryParse(input2, out int result2)){
            starAge = result2; 
        } 
        if(int.TryParse(input3, out int result3)){
            starLuminocity = result3; 
        } 
        if(int.TryParse(input4, out int result4)){
            numOfPlanets = result4; 
        } 
        SceneManager.LoadScene(1, LoadSceneMode.Single); 
    }

    public void ReadStringInput1(string s){
        input1 = s;
    }

    public void ReadStringInput2(string s){
        input2 = s;
    }

    public void ReadStringInput3(string s){
        input3 = s;
    }

    public void ReadStringInput4(string s){
        input4 = s;
    }
}

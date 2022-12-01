using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

[Serializable]
public class Planet : CelestialBody{
    //Variables
    public int type; //1 for rocky, 2 for gassy
    public int uniquePlanetID; 
    public bool isHabitable = false; 
    public string name; 

    public void GenerateName(){
        //Seed randomisation
        UnityEngine.Random.InitState(uniquePlanetID);

        char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        char[] generatedName = {'P', 'L', 'N', '-', alphabet[UnityEngine.Random.Range(0,26)], alphabet[UnityEngine.Random.Range(0,26)], alphabet[UnityEngine.Random.Range(0,26)], alphabet[UnityEngine.Random.Range(0,26)] };
        name = String.Concat(generatedName); 
        Debug.Log(name); 
    }
}

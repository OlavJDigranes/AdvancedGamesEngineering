using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : CelestialBody
{
    public int asteroidID; 
    public string name; 

    public void GenerateName(){
        //Seed randomisation
        UnityEngine.Random.InitState(asteroidID);

        char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        char[] generatedName = {'A', 'S', 'T', '-', alphabet[UnityEngine.Random.Range(0,26)], alphabet[UnityEngine.Random.Range(0,26)], alphabet[UnityEngine.Random.Range(0,26)], alphabet[UnityEngine.Random.Range(0,26)] };
        //string nameP2 = UnityEngine.Random.Range(1, 100).ToString(); 
        name = String.Concat(generatedName) + "-" + UnityEngine.Random.Range(1, 100).ToString(); 
    }
}

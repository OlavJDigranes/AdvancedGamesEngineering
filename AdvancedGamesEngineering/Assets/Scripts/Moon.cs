using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon
{
    //Variables
    public float mass; 
    public Vector3 scale = new Vector3(1, 1, 1); 
    public Vector3 position;

    //Calculate properties
    public void CalculateProperties(){
        scale *= mass;
    }
}

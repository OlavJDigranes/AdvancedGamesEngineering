using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet{
    //Variables
    public float mass; 
    public Vector3 scale = new Vector3(1, 1, 1); 
    public Vector3 position;

    //Simple calculation of properties. This will be expanded whne habitability is accounted for. 
    public void CalculateProperties(){
        scale *= (mass * 2);
    }
}

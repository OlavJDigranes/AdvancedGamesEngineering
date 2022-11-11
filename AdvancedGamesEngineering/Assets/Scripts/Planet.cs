using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class Planet : CelestialBody{
    //Variables
    //public double mass; 
    //public double3 scale; 
    //public double3 position;
    //public double3 velocity; 
    //public double3 accumulatedForce; 
    //public Vector3 scale = new Vector3(1, 1, 1); 
    //public Vector3 position;
    public int type; //1 for rocky, 2 for gassy
    public bool isHabitable = false; 

    //Simple calculation of properties. This will be expanded whne habitability is accounted for. 
    //public void CalculateProperties(){
    //    scale *= mass;
    //}
}

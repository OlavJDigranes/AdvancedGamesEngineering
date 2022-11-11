using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class CelestialBody
{
    public double mass; 
    public double3 scale; 
    public double3 position;
    public double3 velocity; 
    public double3 accumulatedForce; 

    //Simple calculation of properties. This will be expanded whne habitability is accounted for. 
    public void CalculateProperties(){
        scale *= mass;
    }
}

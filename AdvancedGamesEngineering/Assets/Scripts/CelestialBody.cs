using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class CelestialBody
{
    public double mass; 
    public double3 scale = new double3(1.0, 1.0, 1.0); 
    public double3 position;
    public double3 velocity; 
    public double3 accumulatedForce;
    public int identifier; //1 for planet, 2 for moon 

    //Simple calculation of properties. This will be expanded whne habitability is accounted for. 
    public void CalculateProperties(){
        Debug.Log(mass + " CB Mass in calcProp"); 
        scale *= (mass); //NEEDS WORK https://www.google.com/search?client=opera&q=planet+mass+to+radius+ratio&sourceid=opera&ie=UTF-8&oe=UTF-8&tpsf=openc 
        Debug.Log(scale + " CB Scale Proper"); 
    }
}

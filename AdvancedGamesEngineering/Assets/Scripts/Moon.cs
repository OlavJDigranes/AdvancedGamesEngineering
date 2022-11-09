using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class Moon : MonoBehaviour
{
    public int planetID; 
    public double mass; 
    public double3 scale; 
    public double3 position;
    public double3 velocity; 
    public double3 accumulatedForce; 
    //public Vector3 scale = new Vector3(1, 1, 1); 
    //public Vector3 position;

    public void CalculateProperties(){
        scale *= mass; 
    }
}

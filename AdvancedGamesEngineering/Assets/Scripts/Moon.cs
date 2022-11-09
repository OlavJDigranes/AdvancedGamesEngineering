using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class Moon : MonoBehaviour
{
    public int planetID; 
    public double mass; 
    public struct MoonScale
    {
        public double x;
        public double y;
        public double z;
    }
    public struct MoonPosition{
        public double x;
        public double y;
        public double z;
    }
    public struct MoonVelocity{
        public double x;
        public double y;
        public double z;
    }
    public double3 scale; 
    public double3 position;
    public double3 velocity; 
    //public Vector3 scale = new Vector3(1, 1, 1); 
    //public Vector3 position;

    public void CalculateProperties(){
        scale *= mass; 
    }
}

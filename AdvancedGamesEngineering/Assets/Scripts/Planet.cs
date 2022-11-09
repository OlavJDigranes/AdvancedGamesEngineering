using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class Planet{
    //Variables
    public double mass; 
    public struct PlanetScale
    {
        public double x;
        public double y;
        public double z;
    }
    public struct PlanetPosition{
        public double x;
        public double y;
        public double z;
    }
    public struct PlanetVelocity{
        public double x;
        public double y;
        public double z;
    }
    public double3 scale; 
    public double3 position;
    public double3 velocity; 
    //public Vector3 scale = new Vector3(1, 1, 1); 
    //public Vector3 position;

    //Simple calculation of properties. This will be expanded whne habitability is accounted for. 
    public void CalculateProperties(){
        scale *= mass;
    }
}

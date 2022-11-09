using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet{
    //Variables
    public double mass; 
    struct PlanetScale
    {
        public double x;
        public double y;
        public double z;
    }
    struct PlanetPosition{
        public double x;
        public double y;
        public double z;
    }
    struct PlanetVelocity{
        public double x;
        public double y;
        public double z;
    }
    public PlanetScale scale; 
    public PlanetPosition position;
    public PlanetVelocity velocity; 
    //public Vector3 scale = new Vector3(1, 1, 1); 
    //public Vector3 position;

    //Simple calculation of properties. This will be expanded whne habitability is accounted for. 
    public void CalculateProperties(){
        scale.x *= (mass * 2);
        scale.y *= (mass * 2);
        scale.z *= (mass * 2);
    }
}

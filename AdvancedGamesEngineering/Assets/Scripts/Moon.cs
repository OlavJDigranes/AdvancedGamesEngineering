using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    public int planetID; 
    public float mass; 
    public Vector3 scale = new Vector3(1, 1, 1); 
    public Vector3 position;

    public void CalculateProperties(){
        scale *= (mass * 2);
    }
}

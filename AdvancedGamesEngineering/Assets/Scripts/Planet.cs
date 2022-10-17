using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet{
    public float mass; 
    public Vector3 scale = new Vector3(1, 1, 1); 
    public Vector3 position;

    public void CalculateProperties(){
        scale *= mass;
        //Debug.Log(mass);
        //Debug.Log(scale); 
    }
}

/*
public class Planet : MonoBehaviour
{
    public GameObject planetTemplate;
    Rigidbody rbP;  
    public float mass; 
    public Vector3 scale; 
    public Vector3 position;

    void Awake() {

    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(planetTotal); 
        rbP = planetTemplate.GetComponent<Rigidbody>();
        rbP.mass = mass; 
        
        var scale = planetTemplate.transform.localScale; 
        scale *= mass;
        planetTemplate.transform.localScale = scale;
        planetTemplate.transform.position = position; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
*/
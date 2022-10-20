using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


public class TestPS : MonoBehaviour
{
    //readonly double G = 6.670e-11;
    readonly float G = 6.670e-11f;
    readonly float S = 10000.0f; //Scale 
    //readonly float G = 100.0f;

    public GameObject star; 
    public GameObject planet; 
    GameObject[] celestialBodies; 

    // Start is called before the first frame update
    void Start()
    {
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody");
        InitialOrbitalVelocity(); 
        for(int i = 0; i < celestialBodies.Length; i++){
            Debug.Log(celestialBodies[i].GetComponent<Rigidbody>().mass); 
        }
        //InitialOrbitalVelocity2(); 
    }

    // Update is called once per frame
    void Update()
    {
        GravitationalPull();  
        //GravitationalPull2();  
        //Physics.Simulate(1000);
    }

    void FixedUpdate() {
        //GravitationalPull2();
    }

    void GravitationalPull()
    {
        float tDt = Time.deltaTime; 
        //double tDt = 1000; 

        Vector3 starPosition = star.transform.position; 
        //starPosition.x = (double)star.transform.position.x;
        //starPosition.y = (double)star.transform.position.y;
        //starPosition.z = (double)star.transform.position.z;

        float m1 = star.GetComponent<Rigidbody>().mass * S; 
        float m2 = planet.GetComponent<Rigidbody>().mass * S; 

        //Vector3 planetPosition = planet.transform.position * S; 
        //convertedPlanetPosition.x = (double)planet.transform.position.x;
        //convertedPlanetPosition.y = (double)planet.transform.position.y;
        //convertedPlanetPosition.z = (double)planet.transform.position.z;

        //Distance
        float r = Vector3.Distance(star.transform.position, planet.transform.position) * S; 

        //Gravitational Pull
        float gp = (G * (m1 * m2)/(r * r)); 

        Vector3 dist = (star.transform.position - planet.transform.position).normalized * S; 
        
        //double3 normDist; 
        //normDist.x = (double)dist.x;
        //normDist.y = (double)dist.y;
        //normDist.z = (double)dist.z;

        Vector3 gravPullVec = dist * gp; 

        //Symplectic Euler
        Vector3 planetPos = planet.transform.position; 

        Vector3 planetVel = planet.GetComponent<Rigidbody>().velocity; 
        
        //planetVel.x = (double)planet.GetComponent<Rigidbody>().velocity.x;
        //planetVel.y = (double)planet.GetComponent<Rigidbody>().velocity.y;
        //planetVel.z = (double)planet.GetComponent<Rigidbody>().velocity.z;

        Vector3 accel = gravPullVec/m2; 
        planetVel = planetVel + (tDt * accel); 

        Vector3 newPlanetPos = planetPos + (tDt * planetVel); 

        //Vector3 convertedNewPlanetPosition;
        //convertedNewPlanetPosition.x = (float)newPlanetPos.x;
        //convertedNewPlanetPosition.y = (float)newPlanetPos.y;
        //convertedNewPlanetPosition.z = (float)newPlanetPos.z;

        planet.transform.position = newPlanetPos; 
    }

    void GravitationalPull2(){
        foreach(GameObject a in celestialBodies){
            foreach(GameObject b in celestialBodies){
                if(!a.Equals(b)){
                    float m1 = a.GetComponent<Rigidbody>().mass;
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);
                    a.GetComponent<Rigidbody>().AddForce((b.transform.position - a.transform.position).normalized * (100 * (m1* m2) / (r * r)));
                }
            }
        }
    }

    void InitialOrbitalVelocity()
    {
        float m1 = star.GetComponent<Rigidbody>().mass * S; 
        float m2 = planet.GetComponent<Rigidbody>().mass * S;

        Vector3 starPosition = star.transform.position; 

        //double3 starPosition; 
        //starPosition.x = (double)star.transform.position.x;
        //starPosition.y = (double)star.transform.position.y;
        //starPosition.z = (double)star.transform.position.z;

        planet.transform.LookAt(star.transform); 

        //Distance
        float r = Vector3.Distance(star.transform.position, planet.transform.position) * S; 

        //double3 convertedPlanetPosition; 
        //convertedPlanetPosition.x = (double)planet.transform.position.x;
        //convertedPlanetPosition.y = (double)planet.transform.position.y;
        //convertedPlanetPosition.z = (double)planet.transform.position.z;

        Vector3 dist = (planet.transform.position - star.transform.position).normalized * S;         
        
        //double3 normDist; 
        //normDist.x = (double)dist.x;
        //normDist.y = (double)dist.y;
        //normDist.z = (double)dist.z;

        Vector3 dir = planet.transform.right;
        
        //double3 dir; 
        //dir.x = (double)planet.transform.right.x;
        //dir.y = (double)planet.transform.right.y;
        //dir.z = (double)planet.transform.right.z;

        Vector3 vel = dir * (math.sqrt((((G * S) * m2) / r)) * S);

        //Debug.Log(math.sqrt((((G * S) * m2) / r))); 
        //Debug.Log(m1 + ", " + m2 + ", " + starPosition  + ", " + r  + ", " + dist + ", " + dir + ", " +  vel); 

        //Vector3 newVel;
        //newVel.x = (float)vel.x;
        //newVel.y = (float)vel.y;
        //newVel.z = (float)vel.z;

        planet.GetComponent<Rigidbody>().velocity += vel;
    }

    void InitialOrbitalVelocity2(){
        foreach(GameObject a in celestialBodies){
            foreach(GameObject b in celestialBodies){
                if(!a.Equals(b)){
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);
                    a.transform.LookAt(b.transform);
                    a.GetComponent<Rigidbody>().velocity += a.transform.right * Mathf.Sqrt((100 * m2) / r);
                    //Debug.Log(a.GetComponent<Rigidbody>().velocity + ", " + a.GetComponent<Rigidbody>().mass); 
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSManager : MonoBehaviour
{
    //This will manage the behaviour of stars and planets in the simulation. 
    readonly float G = 100.0f; 
    GameObject[] celestialBodies; 
    public GameObject star;
    public GameObject planet; 
    // Start is called before the first frame update
    void Start()
    {
        //celestialBodies = new GameObject[MainMenuManager.numOfPlanets + 1]; 
        //celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBodies"); 
        //celestialBodies[0] = star; 
        Random.InitState(7); 
        CreatePlanets();
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        //InitialOrbitVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void FixedUpdate() {
        //GravitationalPull(); 
    }

    void GravitationalPull(){
        foreach(GameObject a in celestialBodies){
            foreach(GameObject b in celestialBodies){
                if(!a.Equals(b)){
                    float m1 = a.GetComponent<Rigidbody>().mass;
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);
                    a.GetComponent<Rigidbody>().AddForce((b.transform.position - a.transform.position).normalized * (G * (m1* m2) / (r * r)));
                }
            }
        }
    }

    void InitialOrbitVelocity(){
        foreach(GameObject a in celestialBodies){
            foreach(GameObject b in celestialBodies){
                if(!a.Equals(b)){
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);
                    a.transform.LookAt(b.transform);
                    a.GetComponent<Rigidbody>().velocity += a.transform.right * Mathf.Sqrt((G * m2) / r);
                }
            }
        }
    }

    void CreatePlanets(){
        //Random r = new Random(); 
        //RandomNumberGenerator.Create();
        Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
        for(int i = 0; i < MainMenuManager.numOfPlanets; i++){
            Planet p = new Planet();
            //int randInt = RandomNumberGenerator.GetInt32(0,10);
            p.mass = Random.Range(0, MainMenuManager.starMass);
            p.position = new Vector3(i, 0, 0);
            //celestialBodies[i] = p;
            planets[i] = p; 
        }

        int planetNr = 1;
        foreach (Planet p in planets){
            GameObject g = planet; 
            Rigidbody rbG; 
            rbG = g.GetComponent<Rigidbody>();
            rbG.mass = p.mass; 
            g.transform.position = p.position; 
            g.transform.localScale = p.scale;
            //celestialBodies[planetNr] = g;
            Instantiate(g);
            planetNr++; 
        }
    }
}

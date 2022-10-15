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
    Material rockyPlanetMaterial; 
    Material gassyPlanetMaterial;
    Color rockyPanet = new Color(0.74f, 0.2f, 0.2f, 0.5f);
    Color gassyPanet = new Color(0.32f, 0.45f, 0.53f, 0.5f); 
    // Start is called before the first frame update
    void Start()
    {
        //celestialBodies = new GameObject[MainMenuManager.numOfPlanets + 1]; 
        //celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBodies"); 
        //celestialBodies[0] = star; 
        rockyPlanetMaterial = new Material(Shader.Find("Standard"));
        gassyPlanetMaterial = new Material(Shader.Find("Standard"));
        rockyPlanetMaterial.SetColor("_Color", rockyPanet);
        gassyPlanetMaterial.SetColor("_Color", gassyPanet);

        Random.InitState(7); 
        CreatePlanets();
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        InitialOrbitVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        //Rotational speed based on size. 
        foreach(GameObject x in celestialBodies){
            x.transform.Rotate(new Vector3(0, -(float)x.GetComponent<Rigidbody>().mass * 10.0f, 0) * Time.deltaTime); 
        }
        
    }
    
    private void FixedUpdate() {
        GravitationalPull(); 
    }

    void GravitationalPull(){
        //Has to be calculated in relation to the sun. 
        float m1 = MainMenuManager.starMass; 
        for (int i = 1; i < celestialBodies.Length; i++){
            float m2 = celestialBodies[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 
            celestialBodies[i].GetComponent<Rigidbody>().AddForce((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * (G * (m1 * m2) / (r * r))); 
        }

        /*
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
        */
    }

    void InitialOrbitVelocity(){
        //Has to be calculated in relation to the sun. 
        float m1 = MainMenuManager.starMass; 
        for (int i = 1; i < celestialBodies.Length; i++){
            float m2 = celestialBodies[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 
            celestialBodies[i].transform.LookAt(celestialBodies[0].transform); 
            celestialBodies[i].GetComponent<Rigidbody>().velocity += celestialBodies[i].transform.right * Mathf.Sqrt((G * m1) / r);  
        }

        /*
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
        */
    }

    void CreatePlanets(){
        //Random r = new Random(); 
        //RandomNumberGenerator.Create(); 

        Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
        for(int i = 0; i < MainMenuManager.numOfPlanets; i++){
            Planet p = new Planet();
            //int randInt = RandomNumberGenerator.GetInt32(0,10);
            if(i < (MainMenuManager.numOfPlanets/2)){
                p.mass = Random.Range(0, (float)MainMenuManager.starMass-2.0f);
            }
            if(i >= (MainMenuManager.numOfPlanets/2)){
                p.mass = Random.Range(0, (float)MainMenuManager.starMass);
            }
            //Debug.Log(p.mass); 
            p.position = new Vector3(MainMenuManager.starMass * (i + 2), 0, 0);
            p.CalculateProperties(); 
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
            var planetRenderer = g.GetComponent<Renderer>();
            if(planetNr <= (MainMenuManager.numOfPlanets/2)){
                planetRenderer.material = rockyPlanetMaterial; 
                //g.GetComponent<Renderer>().material.color = rockyPanet; 
                //Debug.Log("rock");
            }
            if(planetNr > (MainMenuManager.numOfPlanets/2)){               
                planetRenderer.material = gassyPlanetMaterial; 
                //g.GetComponent<Renderer>().material.color = gassyPanet; 
                //Debug.Log("gassy");
            }
            //celestialBodies[planetNr] = g;
            Instantiate(g);
            Debug.Log(g.transform.position); 
            planetNr++; 
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSManager : MonoBehaviour
{
    //This will manage the behaviour of stars and planets in the simulation. 
    readonly float G = 9.81f;
    //readonly float G = 100.0f; 
    //readonly float G2 = 100.0f; 
    GameObject[] celestialBodies; 
    GameObject[] moons; 
    float[] planetMasses; 
    //List<KeyValuePair<int, GameObject>> planetsAndMoons = new List<KeyValuePair<int, GameObject>>();
    List<KeyValuePair<int, int>> planetsAndMoons = new List<KeyValuePair<int, int>>(); 
    public GameObject star;
    public GameObject planet; 
    public GameObject moon; 
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
        GeneratePlanets();
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        GenerateMoons();
        moons = GameObject.FindGameObjectsWithTag("Moon");
        InitialOrbitVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        //Rotational speed based on size. 
        foreach(GameObject x in celestialBodies){
            x.transform.Rotate(new Vector3(0, -(float)x.GetComponent<Rigidbody>().mass * 10.0f, 0) * Time.deltaTime); 
        }

        foreach(GameObject m in moons){
            m.transform.Rotate(new Vector3(0, -(float)m.GetComponent<Rigidbody>().mass * 10.0f, 0) * Time.deltaTime); 
        }
        GravitationalPull(); 
        
    }
    
    private void FixedUpdate() {
        //GravitationalPull(); 
    }

    void GravitationalPull(){
        //Has to be calculated in relation to the sun. 
        //var tDt = (Time.deltaTime); 
        float tDt = 0.0167f; 
        float m1 = MainMenuManager.starMass; 
        for (int i = 1; i < celestialBodies.Length; i++){
            float m2 = celestialBodies[i].GetComponent<Rigidbody>().mass; 
            //float m2 = planetMasses[i]; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 
            celestialBodies[i].GetComponent<Rigidbody>().AddForce((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * (G * (m1 * m2) / (r * r)) * tDt); 
            //celestialBodies[i].GetComponent<Rigidbody>().AddForce((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * (G * (m1 * m2) / (r * r)));

            foreach (KeyValuePair<int, int> kvp in planetsAndMoons) {
                if((kvp.Key + 1) == i){
                    GameObject tempMoon = moons[kvp.Value]; 
                    float m3 = tempMoon.GetComponent<Rigidbody>().mass;
                    float r2 = Vector3.Distance(tempMoon.transform.position, celestialBodies[i].transform.position);
                    //celestialBodies[i].GetComponent<Rigidbody>().AddForce((kvp.Value.transform.position - celestialBodies[i].transform.position).normalized * ((G / 2) * (m2 * m3) / (r2 * r2)));
                    
                    tempMoon.GetComponent<Rigidbody>().AddForce((celestialBodies[i].transform.position - tempMoon.transform.position).normalized * (G * ((m2 * m3) / (r2 * r2))) * tDt); 
                    celestialBodies[i].GetComponent<Rigidbody>().AddForce((tempMoon.transform.position - celestialBodies[i].transform.position).normalized * (G * ((m2 * m3) / (r2 * r2))) * tDt); 

                    //tempMoon.GetComponent<Rigidbody>().AddForce((celestialBodies[i].transform.position - tempMoon.transform.position).normalized * (G * ((m2 * m3) / (r2 * r2)))); 
                    //celestialBodies[i].GetComponent<Rigidbody>().AddForce((tempMoon.transform.position - celestialBodies[i].transform.position).normalized * (G * ((m2 * m3) / (r2 * r2)))); 
                }
            }
        }

        /*
        foreach (KeyValuePair<int, GameObject> kvp in planetsAndMoons) {
            //GameObject moonInstance = kvp.Value; 
            float m3 = celestialBodies[kvp.Key + 1].GetComponent<Rigidbody>().mass;
            float m4 = kvp.Value.GetComponent<Rigidbody>().mass; 
            float r2 = Vector3.Distance(kvp.Value.transform.position, celestialBodies[kvp.Key + 1].transform.position);
            //Vector3 force = ((kvp.Value.transform.position - celestialBodies[kvp.Key].transform.position).normalized * (G * (m3 * m4) / (r2 * r2))); 
            kvp.Value.GetComponent<Rigidbody>().AddForce((celestialBodies[kvp.Key + 1].transform.position - kvp.Value.transform.position).normalized * (G * (m3 * m4) / (r2 * r2))); 
            //celestialBodies[kvp.Key + 1].GetComponent<Rigidbody>().AddForce((celestialBodies[kvp.Key + 1].transform.position - kvp.Value.transform.position). normalized * (G * (m3 * m4) / (r2* r2))); 
            //Debug.Log("GRAVITY: " + force); 
        }

        
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
            //float m2 = planetMasses[i]; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 
            celestialBodies[i].transform.LookAt(celestialBodies[0].transform); 
            celestialBodies[i].GetComponent<Rigidbody>().velocity += celestialBodies[i].transform.right * Mathf.Sqrt((G * m1) / r); 

            foreach(KeyValuePair<int, int> kvp in planetsAndMoons){
                if((kvp.Key + 1) == i){
                    GameObject moonInstance = moons[kvp.Value];
                    float m3 = moonInstance.GetComponent<Rigidbody>().mass;
                    float r2 = Vector3.Distance(moonInstance.transform.position, celestialBodies[i].transform.position);
                    moonInstance.transform.LookAt(celestialBodies[i].transform);
                    moonInstance.GetComponent<Rigidbody>().velocity += moonInstance.transform.right * Mathf.Sqrt((G * m2) / r2);
                }
            }
        }

        /*
        foreach (KeyValuePair<int, int> kvp in planetsAndMoons) {
            GameObject moonInstance = moons[kvp.Value]; 
            float m3 = planetMasses[kvp.Key]; 
            float m4 = moonInstance.GetComponent<Rigidbody>().mass; 
            float r2 = Vector3.Distance(celestialBodies[kvp.Key + 1].transform.position, moonInstance.transform.position);
            //kvp.Value.GetComponent<Rigidbody>().AddForce((kvp.Value.transform.position - celestialBodies[kvp.Key].transform.position).normalized * (G * (m3 * m4) / (r2 * r2))); 
            moonInstance.transform.LookAt(celestialBodies[kvp.Key + 1].transform);
            moonInstance.GetComponent<Rigidbody>().velocity += moonInstance.transform.right * Mathf.Sqrt((G2 * m3) / r2);  
            //Debug.Log("VEL: " + kvp.Value.GetComponent<Rigidbody>().velocity); 
        }

        
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

    void GeneratePlanets(){
        //Random r = new Random(); 
        //RandomNumberGenerator.Create(); 

        Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
        planetMasses = new float[MainMenuManager.numOfPlanets]; 
        for(int i = 0; i < MainMenuManager.numOfPlanets; i++){
            Planet p = new Planet();
            //int randInt = RandomNumberGenerator.GetInt32(0,10);
            if(i < (MainMenuManager.numOfPlanets/2)){
                float planetMass = Random.Range(1.0f, (float)MainMenuManager.starMass-2.0f);
                p.mass = planetMass; 
                planetMasses[i] = planetMass; 
            }
            if(i >= (MainMenuManager.numOfPlanets/2)){
                float planetMass = Random.Range(1.0f, (float)MainMenuManager.starMass);
                p.mass = planetMass; 
                planetMasses[i] = planetMass; 
            }
            Debug.Log(p.mass); 
            p.position = new Vector3((MainMenuManager.starMass * (i + 2.0f)) * 2.0f, 0, 0);
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
            planetNr++; 
        }
    }

    void GenerateMoons(){
        int listCounter = 0; 
        for(int i = 0; i < MainMenuManager.numOfPlanets; i++){
            if (planetMasses[i] > 2.5f){
                Moon m = new Moon(); 
                GameObject g = moon; 
                Rigidbody rbM; 
                rbM = g.GetComponent<Rigidbody>();
                
                float moonMass = Random.Range(0, planetMasses[i] * 0.12f);
                rbM.mass = moonMass; 
                m.mass = moonMass; 

                //Vector3 planetPosition = celestialBodies[i].transform.position; 
                Vector3 moonPosition = new Vector3((celestialBodies[i + 1].transform.position.x + 5.0f), 0.0f, 0.0f);
                g.transform.position =  moonPosition;
                m.position = moonPosition; 

                m.CalculateProperties(); 
                g.transform.localScale = m.scale; 

                g.GetComponent<Renderer>().sharedMaterial.color = Color.grey;

                Instantiate(g); 
                Debug.Log(i + 1); 
                planetsAndMoons.Add(new KeyValuePair<int, int>(i, listCounter));    

                //moons[listCounter] = g;
                listCounter++;              
            }
        }
    }
}

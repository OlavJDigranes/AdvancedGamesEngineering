using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class PSManager : MonoBehaviour
{
    //This will manage the behaviour of stars and planets in the simulation. 
    readonly double G = 6.670e-11f;
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

        UnityEngine.Random.InitState(7); 
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
        double tDt = 1000; 
        //float tDt = 0.0167f;
        //float m1 = MainMenuManager.starMass; 
        double3 starPosition;
        starPosition.x = (double)celestialBodies[0].transform.position.x;
        starPosition.y = (double)celestialBodies[0].transform.position.y;
        starPosition.z = (double)celestialBodies[0].transform.position.z;

        double m1 = (double)celestialBodies[0].GetComponent<Rigidbody>().mass; 
        for (int i = 1; i < celestialBodies.Length; i++){
            double m2 = (double)celestialBodies[i].GetComponent<Rigidbody>().mass; 
            //float m2 = planetMasses[i]; 

            //convert planet position
            double3 convPlanetPos;
            convPlanetPos.x = (double)celestialBodies[i].transform.position.x;
            convPlanetPos.y = (double)celestialBodies[i].transform.position.y;
            convPlanetPos.z = (double)celestialBodies[i].transform.position.z;

            //calculate distance
            double r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position);
            //double r = Vector3.Distance(starPosition, convPlanetPos); 

            //calculate gravitational pull vector
            double gp = (G * (m1 * m2) / (r * r));
            //Debug.Log(gp); 

            Vector3 dist = (celestialBodies[i].transform.position - celestialBodies[0].transform.position).normalized;
            double3 normalizedDistance;
            normalizedDistance.x = (double)dist.x;
            normalizedDistance.y = (double)dist.y;
            normalizedDistance.z = (double)dist.z;

            double3 gravitationalPullVector = (normalizedDistance * gp);
            //Vector3 gravitationalPullVector = Multiply((celestialBodies[i].transform.position - celestialBodies[0].transform.position).normalized, gp);
            //Debug.Log(gravitationalPullVector + ", " + G + ", " + m1 + ", " + m2 + ", " + r + ", " + (celestialBodies[i].transform.position - celestialBodies[0].transform.position).normalized);
            
            //Symplectic Euler
            double3 planetPos = convPlanetPos;
            double3 planetVel;
            planetVel.x = (double)celestialBodies[i].GetComponent<Rigidbody>().velocity.x;
            planetVel.y = (double)celestialBodies[i].GetComponent<Rigidbody>().velocity.y;
            planetVel.z = (double)celestialBodies[i].GetComponent<Rigidbody>().velocity.z; 

            double3 accel = gravitationalPullVector/m2;
            planetVel = planetVel + (tDt * accel);

            //celestialBodies[i].GetComponent<Rigidbody>().velocity += planetVel;
            double3 newPlanetPos = planetPos + (tDt * planetVel);

            Vector3 convertedNewPlanetPos;
            convertedNewPlanetPos.x = (float)newPlanetPos.x;
            convertedNewPlanetPos.y = (float)newPlanetPos.y;
            convertedNewPlanetPos.z = (float)newPlanetPos.z;
            celestialBodies[i].transform.position = convertedNewPlanetPos;
            //celestialBodies[i].GetComponent<Rigidbody>().AddForce((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * (G * (m1 * m2) / (r * r)) * (tDt)); 
            //celestialBodies[0].GetComponent<Rigidbody>().AddForce((celestialBodies[i].transform.position - celestialBodies[0].transform.position).normalized * (G * (m1 * m2) / (r * r)) * (tDt * 4)); 
            //celestialBodies[i].GetComponent<Rigidbody>().AddForce((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * (G * (m1 * m2) / (r * r)));

            foreach (KeyValuePair<int, int> kvp in planetsAndMoons) {
                if((kvp.Key + 1) == i){
                    GameObject tempMoon = moons[kvp.Value]; 
                    float m3 = tempMoon.GetComponent<Rigidbody>().mass;
                    float r2 = Vector3.Distance(tempMoon.transform.position, celestialBodies[i].transform.position);
                    //celestialBodies[i].GetComponent<Rigidbody>().AddForce((kvp.Value.transform.position - celestialBodies[i].transform.position).normalized * ((G / 2) * (m2 * m3) / (r2 * r2)));
                    
                    //tempMoon.GetComponent<Rigidbody>().AddForce((celestialBodies[i].transform.position - tempMoon.transform.position).normalized * (G * ((m2 * m3) / (r2 * r2))) * tDt); 
                    //celestialBodies[i].GetComponent<Rigidbody>().AddForce((tempMoon.transform.position - celestialBodies[i].transform.position).normalized * (G * ((m2 * m3) / (r2 * r2))) * tDt); 

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
        double m1 = MainMenuManager.starMass; 

        double3 yAxis = new double3(0, 1, 0);
        double3 zAxis = new double3(0, 0, 1);

        double3 starPosition;
        starPosition.x = (double)celestialBodies[0].transform.position.x;
        starPosition.y = (double)celestialBodies[0].transform.position.y;
        starPosition.z = (double)celestialBodies[0].transform.position.z;

        for (int i = 1; i < celestialBodies.Length; i++){
            double m2 = celestialBodies[i].GetComponent<Rigidbody>().mass; 
            //float m2 = planetMasses[i]; 
            double r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 

            celestialBodies[i].transform.LookAt(celestialBodies[0].transform); 

            double3 convPlanetPos;
            convPlanetPos.x = (double)celestialBodies[i].transform.position.x;
            convPlanetPos.y = (double)celestialBodies[i].transform.position.y;
            convPlanetPos.z = (double)celestialBodies[i].transform.position.z;

            Vector3 dist = (celestialBodies[i].transform.position - celestialBodies[0].transform.position).normalized;
            double3 normalizedDistance;
            normalizedDistance.x = (double)dist.x;
            normalizedDistance.y = (double)dist.y;
            normalizedDistance.z = (double)dist.z;

            //double3 dir = math.cross((starPosition - convPlanetPos), yAxis);
            double3 dir = (normalizedDistance + zAxis);
            //double3 dir; 
            //dir.x = (double)celestialBodies[i].transform.right.x;
            //dir.y = (double)celestialBodies[i].transform.right.y;
            //dir.z = (double)celestialBodies[i].transform.right.z;

            Debug.Log(dir);

            double3 vel = dir * math.sqrt(((G * m1) / r));
            Debug.Log(vel);

            Vector3 newVel;
            newVel.x = (float)vel.x;
            newVel.y = (float)vel.y;
            newVel.z = (float)vel.z;

            //celestialBodies[i].GetComponent<Rigidbody>().velocity += celestialBodies[i].transform.right * Mathf.Sqrt((G * m2) / r); 
            celestialBodies[i].GetComponent<Rigidbody>().velocity += newVel;

            foreach(KeyValuePair<int, int> kvp in planetsAndMoons){
                if((kvp.Key + 1) == i){
                    GameObject moonInstance = moons[kvp.Value];
                    float m3 = moonInstance.GetComponent<Rigidbody>().mass;
                    float r2 = Vector3.Distance(moonInstance.transform.position, celestialBodies[i].transform.position);
                    moonInstance.transform.LookAt(celestialBodies[i].transform);
                    //moonInstance.GetComponent<Rigidbody>().velocity += moonInstance.transform.right * Mathf.Sqrt((G * m3) / r2);
                }
            }
            //Debug.Log(celestialBodies[i].GetComponent<Rigidbody>().velocity);
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
        
        //float localStarMass = celestialBodies[0].GetComponent<Rigidbody>().mass; 
        Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
        planetMasses = new float[MainMenuManager.numOfPlanets]; 
        for(int i = 0; i < MainMenuManager.numOfPlanets; i++){
            Planet p = new Planet();
            //int randInt = RandomNumberGenerator.GetInt32(0,10);
            if(i < (MainMenuManager.numOfPlanets/2)){
                float planetMass = UnityEngine.Random.Range(1.0f, (((float)MainMenuManager.starMass * 1) * 0.09f) - 2.0f);
                //float planetMass = Random.Range(1.0f, (localStarMass * 0.09f) - 2.0f);
                p.mass = planetMass; 
                planetMasses[i] = planetMass; 
            }
            if(i >= (MainMenuManager.numOfPlanets/2)){
                float planetMass = UnityEngine.Random.Range(1.0f, (((float)MainMenuManager.starMass * 1) * 0.09f));
                //float planetMass = Random.Range(1.0f, (localStarMass * 0.09f));
                p.mass = planetMass; 
                planetMasses[i] = planetMass; 
            }
            //Debug.Log(p.mass); 
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
                
                float moonMass = UnityEngine.Random.Range(0.1f, planetMasses[i] * 0.12f);
                rbM.mass = moonMass; 
                m.mass = moonMass; 

                //Vector3 planetPosition = celestialBodies[i].transform.position; 
                Vector3 moonPosition = new Vector3((celestialBodies[i + 1].transform.position.x) + ((planetMasses[i] / 2) + m.scale.z + 2), 0.0f, 0.0f);
                g.transform.position =  moonPosition;
                m.position = moonPosition; 

                m.CalculateProperties(); 
                g.transform.localScale = m.scale; 

                g.GetComponent<Renderer>().sharedMaterial.color = Color.grey;

                Instantiate(g); 
                //Debug.Log(i + 1); 
                planetsAndMoons.Add(new KeyValuePair<int, int>(i, listCounter));    

                //moons[listCounter] = g;
                listCounter++;              
            }
        }
    }
}

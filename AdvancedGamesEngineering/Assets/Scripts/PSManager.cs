using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSManager : MonoBehaviour
{
    //This will manage the behaviour of stars and planets in the simulation. 
    //readonly float G = 100.0f; 
    readonly float G = 6.670e-11f;
    //readonly float S = 1.0f; 
    readonly float S = 1.0e+12f; //Scale 
    GameObject[] celestialBodies; 
    GameObject[] moons; 
    //Moon[] moons2; 
    List<Moon> moons2 = new List<Moon>();  
    Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
    public GameObject star;
    public GameObject planet; 
    public GameObject moon; 
    public Material rockyPlanetMaterial; 
    public Material gassyPlanetMaterial;
    public Shader pgg; 
    public Shader pgr; 
    //Material rockyPlanetMaterial; 
    //Material gassyPlanetMaterial; 
    Color rockyPanet = new Color(0.74f, 0.2f, 0.2f, 0.5f);
    Color gassyPanet = new Color(0.32f, 0.45f, 0.53f, 0.5f); 

    public int sizeX = 100;
    public int sizeZ = 50;
    public float tileSize = 1.0f;
    public int tileResolution = 8; 
    int moonCounter; 
    // Start is called before the first frame update
    void Start()
    {
        //celestialBodies = new GameObject[MainMenuManager.numOfPlanets + 1]; 
        //celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBodies"); 
        //celestialBodies[0] = star; 
        //rockyPlanetMaterial = new Material(Shader.Find("Shader Graphs/ProcGenRocky"));
        //gassyPlanetMaterial = new Material(Shader.Find("Shader Graphs/ProcGenGassy"));
        //rockyPlanetMaterial.SetColor("_Color", rockyPanet);
        //gassyPlanetMaterial.SetColor("_Color", gassyPanet);

        Random.InitState(7); 
        GeneratePlanets();
        //moons2 = new Moon[moonCounter]; 
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
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
    }
    
    private void FixedUpdate() {
        GravitationalPull(); 
        MoonGravPull(); 
    }

    void GravitationalPull(){
        //Has to be calculated in relation to the sun. 
        float m1 = MainMenuManager.starMass; 
        for (int i = 1; i < celestialBodies.Length; i++){
            float m2 = celestialBodies[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 
            celestialBodies[i].GetComponent<Rigidbody>().AddForce((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * ((G * S) * (m1 * m2) / (r * r))); 
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

    void MoonGravPull(){
        for(int i = 0; i < moonCounter; i++){
            float m1 = celestialBodies[moons2[i].planetID].GetComponent<Rigidbody>().mass;
            float m2 = moons[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[moons2[i].planetID].transform.position, moons[i].transform.position); 

            
            Vector3 force = (celestialBodies[moons2[i].planetID].transform.position - moons[i].transform.position).normalized * ((G * S) * (m1 * m2) / (r * r)); 
            Vector3 force2 = -(moons[i].transform.position - celestialBodies[moons2[i].planetID].transform.position).normalized * ((G * S) * (m1 * m2) / (r * r)); 
            Vector3 accel = (force + force2) / m2; 
            Vector3 vel = moons[i].GetComponent<Rigidbody>().velocity + (Time.deltaTime * accel);
            Vector3 pos = moons[i].transform.position + (Time.deltaTime * vel); 
            moons[i].transform.position = pos; 
            moons[i].GetComponent<Rigidbody>().velocity = vel; 

            //Debug.Log("ID: " + (i + 1) + "\nDistance: " + r + "\nForce: " + force + "\nAccel: " + accel + "\nVel:   " + vel + "\nPos:   " + pos); 
            
            //moons[i].GetComponent<Rigidbody>().AddForce((celestialBodies[moons2[i].planetID].transform.position - moons[i].transform.position).normalized * ((G * S) * (m1 * m2) / (r * r))); 
        }
    }

    void InitialOrbitVelocity(){
        //Has to be calculated in relation to the sun. 
        float m1 = MainMenuManager.starMass; 
        for (int i = 1; i < celestialBodies.Length; i++){
            float m2 = celestialBodies[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 
            celestialBodies[i].transform.LookAt(celestialBodies[0].transform); 
            celestialBodies[i].GetComponent<Rigidbody>().velocity += celestialBodies[i].transform.right * Mathf.Sqrt(((G * S) * m1) / r);  
        }


        for(int j = 0; j < moons.Length; j++){
            //float m3 = celestialBodies[moons2[j].planetID].GetComponent<Rigidbody>().mass; 
            float m3 = planets[moons2[j].planetID-1].mass;  
            float m4 = moons[j].GetComponent<Rigidbody>().mass; 
            float r2 = Vector3.Distance(celestialBodies[moons2[j].planetID].transform.position, moons[j].transform.position); 

            moons[j].transform.LookAt(celestialBodies[moons2[j].planetID].transform); 
            moons[j].GetComponent<Rigidbody>().velocity += moons[j].transform.right * Mathf.Sqrt(((G * S) * m3) / r2); 
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

    void GeneratePlanets(){        
        //Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
        for(int i = 0; i < MainMenuManager.numOfPlanets; i++){
            Planet p = new Planet();
            if(i < (MainMenuManager.numOfPlanets/2)){
                float planetMass = UnityEngine.Random.Range((((float)MainMenuManager.starMass * 1) * 0.003f), (((float)MainMenuManager.starMass * 1) * 0.09f));
                p.mass = planetMass; 
            }
            if(i >= (MainMenuManager.numOfPlanets/2)){
                float planetMass = UnityEngine.Random.Range((((float)MainMenuManager.starMass * 1) * 0.003f), (((float)MainMenuManager.starMass * 1) * 0.09f));
                p.mass = planetMass; 
            }
            p.position = new Vector3((MainMenuManager.starMass * (i + 2.0f)) * 2.0f, 0, 0);
            //p.position = new Vector3((MainMenuManager.starMass * p.mass), 0, 0);
            p.CalculateProperties(); 
            planets[i] = p; 
        }

        int planetNr = 1;
        foreach (Planet p in planets){
            GameObject g = planet;  
            Rigidbody rbG; 
            //Material mat;  

            //Generate values
            rbG = g.GetComponent<Rigidbody>();
            rbG.mass = p.mass; 
            g.transform.position = p.position; 
            g.transform.localScale = p.scale;
            var planetRenderer = g.GetComponent<Renderer>();
            
            //Assign correct material
            if(planetNr <= (MainMenuManager.numOfPlanets/2)){
                //Material mat = new Material(rockyPlanetMaterial.shader); 
                Material mat = new Material(pgr); 
                mat.SetColor("_Color", rockyPanet);
                float rand = Random.Range(1, 10); 
                mat.SetFloat("_Scale", rand); 
                planetRenderer.material = mat;
                
            }

            if(planetNr > (MainMenuManager.numOfPlanets/2)){   
                Material mat = new Material(pgg); 
                mat.SetColor("_Color", gassyPanet);
                float rand = Random.Range(1, 3); 
                mat.SetFloat("_CellDensity", rand);          
                planetRenderer.material = mat; 
                 
            }
            //celestialBodies[planetNr] = g;
            Instantiate(g);

            if(rbG.mass > 2.0f){
                GenerateMoon(p.mass, p.position, planetNr, p.scale); 
            }

            planetNr++; 
        }
    }

    void GenerateMoon(float planetMass, Vector3 planetPosition, int planetID, Vector3 planetScale){
        Moon m = new Moon(); 
        m.mass = planetMass * 0.27f;
        m.planetID = planetID;  
        m.position = new Vector3(planetPosition.x, planetPosition.y, planetPosition.z + planetScale.z); 
        m.CalculateProperties(); 

        moons2.Add(m); 

        GameObject gm = moon; 
        Rigidbody rbM; 

        rbM = gm.GetComponent<Rigidbody>(); 
        rbM.mass = m.mass; 

        gm.transform.position = m.position; 
        gm.transform.localScale = m.scale; 
        var moonRenderer = gm.GetComponent<Renderer>(); 
        moonRenderer.sharedMaterial.SetColor("_Color", Color.grey); 
        Instantiate(gm);
        moonCounter++;  
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class PSManager : MonoBehaviour
{
    //This will manage the behaviour of stars and planets in the simulation. 

    //Variables
    readonly double G = 6.670e-11;
    readonly float S = 1.0e-12f; //Scale 
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
    public Material moonMaterial; 
    public Shader pgg; 
    public Shader pgr;
    public Shader pgm;   
    Color rockyPanet = new Color(0.74f, 0.2f, 0.2f, 0.5f);
    Color gassyPanet = new Color(0.32f, 0.45f, 0.53f, 0.5f);  
    int moonCounter; 
    List<CelestialBody> celestialBodiesPhysics = new List<CelestialBody>(); 
    double[] masses = new double[MainMenuManager.numOfPlanets + 1];
    double3[] positions = new double3[MainMenuManager.numOfPlanets + 1];

    // Start is called before the first frame update
    void Start()
    {
        //Seed randomisation
        UnityEngine.Random.InitState(7);

        //Generate planets and fill celestial bodies array. Note: Star will always be at index 0 
        GeneratePlanets();
        //moons2 = new Moon[moonCounter]; 
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        moons = GameObject.FindGameObjectsWithTag("Moon"); 
        for(int i = 0; i < celestialBodies.Length; i++){
            celestialBodiesPhysics.Add(planets[i]); 
        }
        for(int j = 0; j < moons.Length; j++){
            celestialBodiesPhysics.Add(moons2[j]); 
        }
        masses[0] = star.GetComponent<Star>().mass;
        for(int i = 1; i < celestialBodies.Length; i++){
            masses[i] = planets[i].mass; 
        }
        
        InitialOrbitVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        //Rotational speed based on size. Simple implementation for demonstration purposes
        foreach(GameObject x in celestialBodies){
            x.transform.Rotate(new Vector3(0, -(float)x.GetComponent<Rigidbody>().mass * 10.0f, 0) * Time.deltaTime); 
        }
        foreach(GameObject m in moons){
            m.transform.Rotate(new Vector3(0, -(float)m.GetComponent<Rigidbody>().mass, 0) * (Time.deltaTime/2)); 
        }
        for(int i = 0; i < celestialBodies.Length; i++){
            if(i == 0){
                positions[0] = celestialBodies[0].GetComponent<Star>().position; 
            }
            else{
                positions[i] = planets[i-1].position; 
            }
        }
        OverallGravitationalPull(); 
        Integration(); 
    }
    
    private void FixedUpdate() {
        //calculate gravitational pull
        //GravitationalPull(); 
        //MoonGravPull(); 
    }

    void OverallGravitationalPull(){
        //Calculate gravitational pull using Newton's law of universil gravitation:
        //  F = G * ((m1 * m2)/r^2)

        //Figure out how to implement the moons here
        for(int i = 0; i < celestialBodies.Length; i++) {
            double3 force = new double3(); 
            for(int j = 0; j < celestialBodies.Length; j++) {
                if(!celestialBodies[i].Equals(celestialBodies[j])){
                    double m1 = celestialBodiesPhysics[i].mass;
                    double m2 = celestialBodiesPhysics[j].mass;
                    double x =  celestialBodiesPhysics[i].position.x - celestialBodiesPhysics[j].position.x;
                    double y =  celestialBodiesPhysics[i].position.y - celestialBodiesPhysics[j].position.y;
                    double z =  celestialBodiesPhysics[i].position.z - celestialBodiesPhysics[j].position.z;
                    double r = System.Math.Sqrt((x * x) + (y * y) + (z * z));
                    force += ((celestialBodiesPhysics[i].position - celestialBodiesPhysics[j].position)/r) * (G * (m1 * m2)/(r* r)); 
                    if(i == 0){
                        celestialBodies[0].GetComponent<Star>().accumulatedForce += force;
                    }
                    else{
                        celestialBodiesPhysics[i-1].accumulatedForce += force; 
                    }
                }
            }
        }
    }

    void Integration(){
        double3 vel;
        double3 pos; 
        for(int i = 0; i < celestialBodiesPhysics.Count; i++){
            double3 accel = celestialBodiesPhysics[i].accumulatedForce / celestialBodiesPhysics[i].mass; 
            vel = celestialBodiesPhysics[i].velocity + (accel * Time.deltaTime);
            pos = celestialBodiesPhysics[i].position + (vel * Time.deltaTime); 
            celestialBodiesPhysics[i].velocity = vel; 
            celestialBodiesPhysics[i].position = pos;  
        }

        for(int j = 0; j < planets.Length; j++){
            Vector3 tempPos;
            tempPos.x = (float)planets[j].position.x;
            tempPos.y = (float)planets[j].position.y;
            tempPos.z = (float)planets[j].position.z;

            Vector3 tempVel;
            tempVel.x = (float)planets[j].velocity.x;  
            tempVel.y = (float)planets[j].velocity.y;  
            tempVel.z = (float)planets[j].velocity.z;  

            celestialBodies[planets[j].uniquePlanetID].transform.position = tempPos * S; 
            celestialBodies[planets[j].uniquePlanetID].GetComponent<Rigidbody>().velocity = tempVel * S; 
        }

        for(int k = 0; k < moons2.Count; k++){
            Vector3 tempPos;
            tempPos.x = (float)moons2[j].position.x;
            tempPos.y = (float)moons2[j].position.y;
            tempPos.z = (float)moons2[j].position.z;

            Vector3 tempVel;
            tempVel.x = (float)moons2[j].velocity.x;  
            tempVel.y = (float)moons2[j].velocity.y;  
            tempVel.z = (float)moons2[j].velocity.z;  

            moons[moons2[j].uniqueMoonID].transform.position = tempPos * S; 
            moons[moons2[j].uniqueMoonID].GetComponent<Rigidbody>().velocity = tempVel * S; 
        }
    }

    /*
    void GravitationalPull(){
        //Calculate gravitational pull using Newton's law of universil gravitation:
        //  F = G * ((m1 * m2)/r^2)
        //Has to be calculated in relation to the sun. 
        float m1 = star.GetComponent<Star>().mass; 
        for (int i = 1; i < celestialBodies.Length; i++){
            float m2 = celestialBodies[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 
            celestialBodies[i].GetComponent<Rigidbody>().AddForce((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * ((G * S) * (m1 * m2) / (r * r))); 
        }
    }

    void MoonGravPull(){
        for(int i = 0; i < moonCounter; i++){
            double m1 = planets[moons2[i].planetID - 1].mass;
            double m2 = moons[i].mass; 
            double x = planets[moons2[i].planetID - 1].position.x - moons2[i].position.x;
            double y = planets[moons2[i].planetID - 1].position.y - moons2[i].position.y;
            double z = planets[moons2[i].planetID - 1].position.z - moons2[i].position.z;
            double r = System.Math.Sqrt((x * x) + (y * y) + (z * z)); 

            //Force handeling and integration using Symplectic euler. 
            double3 force = (planets[moons2[i].planetID - 1].position - moons[i].position).normalized * ((G) * (m1 * m2) / (r * r) * S); 
            double3 force2 = -(moons[i].transform.position - celestialBodies[moons2[i].planetID].transform.position).normalized * ((G) * (m1 * m2) / (r * r) * S); 
            double3 accel = (force + force2) / m2; 
            double3 vel = moons[i].GetComponent<Rigidbody>().velocity + (Time.deltaTime * accel);
            double3 pos = moons[i].transform.position + (Time.deltaTime * vel); 
            moons2[i].velocity = vel;
            moons2[i].position = pos;
            //moons[i].transform.position = pos; 
            //moons[i].GetComponent<Rigidbody>().velocity = vel; 
        }
    }
    */

    void InitialOrbitVelocity(){
        //Calculate initial orbital velocity using circular orbit instant velocity
        //Has to be calculated in relation to the sun. 
        double3 directionX = new double3(1.0, 0.0, 0.0);
        double3 directionZ = new double3(0.0, 0.0, 1.0);

        //Initial planet velocities
        for(int i = 1; i < celestialBodies.Length; i++){
            double m = planets[i-1].mass;
            double x = (double)star.transform.position.x - planets[i-1].position.x;
            double y = (double)star.transform.position.y - planets[i-1].position.y;
            double z = (double)star.transform.position.z - planets[i-1].position.z;
            double r = System.Math.Sqrt((x * x) + (y * y) + (z * z));
            double3 initialVelocity = directionZ * System.Math.Sqrt((G * m) / r);
            planets[i].velocity = initialVelocity; 

            //Float conversion for display
            Vector3 initVel; 
            initVel.x = (float)initialVelocity.x;
            initVel.y = (float)initialVelocity.y;
            initVel.z = (float)initialVelocity.z;
            celestialBodies[i].GetComponent<Rigidbody>().velocity += initVel; 
        }

        //Initial moon velocities
        for(int i = 0; i < moons.Length; i++){
            double x = planets[moons2[i].planetID - 1].position.x - moons2[i].position.x;
            double y = planets[moons2[i].planetID - 1].position.y - moons2[i].position.y;
            double z = planets[moons2[i].planetID - 1].position.z - moons2[i].position.z;
            double r = System.Math.Sqrt((x * x) + (y * y) + (z * z));
            double3 initialVelocity = directionX * System.Math.Sqrt((G * moons2[i].mass) / r);
            moons2[i].velocity = initialVelocity;
            
            //Float conversion for display
            Vector3 initVel; 
            initVel.x = (float)initialVelocity.x;
            initVel.y = (float)initialVelocity.y;
            initVel.z = (float)initialVelocity.z;
            moons[i].GetComponent<Rigidbody>().velocity += initVel; 
        }

        /*
        float m1 = star.GetComponent<Star>().mass; 
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
        */
    }

    void GeneratePlanets(){
        int moonCounter = 0;         
        //Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
        for(int i = 0; i < MainMenuManager.numOfPlanets; i++){
            Planet p = new Planet();
            float planetMass = UnityEngine.Random.Range((((float)star.GetComponent<Star>().mass) * 0.003f), (((float)star.GetComponent<Star>().mass) * 0.09f));
            p.mass = (double)planetMass; 
            p.position = new double3((star.GetComponent<Star>().mass * (i + 2.0)) * 2.0, 0, 0);
            p.identifier = 1; 
            p.uniquePlanetID = i+1; 
            p.CalculateProperties(); 
            planets[i] = p; 
        }

        //generate planet game objects
        int planetNr = 1;
        foreach (Planet p in planets){
            GameObject g = planet;  
            Rigidbody rbG; 

            //Generate values
            rbG = g.GetComponent<Rigidbody>();
            rbG.mass = (float)p.mass; 

            Vector3 posConversion;
            posConversion.x = (float)p.position.x;
            posConversion.y = (float)p.position.y;
            posConversion.z = (float)p.position.z;
            g.transform.position = posConversion; 

            Vector3 scaleConversion;
            scaleConversion.x = (float)p.scale.x;
            scaleConversion.y = (float)p.scale.y;
            scaleConversion.z = (float)p.scale.z;
            g.transform.localScale = scaleConversion;

            var planetRenderer = g.GetComponent<Renderer>();
            
            //Assign correct material
            if(planetNr <= (MainMenuManager.numOfPlanets/2)){
                Material mat = new Material(pgr); 
                mat.SetColor("_Color", rockyPanet);
                Color randColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1.0f);
                mat.SetColor("_DetailColor2", randColor); 
                float rand = UnityEngine.Random.Range(1, 10); 
                mat.SetFloat("_Scale", rand); 
                float rand2 = UnityEngine.Random.Range(1f, 3f); 
                mat.SetFloat("_Scale", rand2); 
                planetRenderer.material = mat; 
                p.type = 1;                
            }

            if(planetNr > (MainMenuManager.numOfPlanets/2)){   
                Material mat = new Material(pgg); 
                mat.SetColor("_Color", gassyPanet);
                Color randColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1.0f);
                mat.SetColor("_AccentColor", randColor); 
                float randDensity = UnityEngine.Random.Range(1, 3); 
                mat.SetFloat("_CellDensity", randDensity);  
                float randRingAccent = UnityEngine.Random.Range(1, 10); 
                mat.SetFloat("_RingAccents", randRingAccent);
                float randNumRings = UnityEngine.Random.Range(1, 5); 
                mat.SetFloat("_NumberOfRings", randNumRings);        
                planetRenderer.material = mat;
                p.type = 2;                  
            }
            Instantiate(g);

            if(rbG.mass > 3.0f){
                GenerateMoon(p.mass, p.position, planetNr, p.scale, moonCounter); 
                moonCounter++; 
            }

            planetNr++; 
        }
    }

    void GenerateMoon(double planetMass, double3 planetPosition, int planetID, double3 planetScale, int i){
        Moon m = new Moon(); 
        m.mass = planetMass * UnityEngine.Random.Range(0.08f, 0.27f);
        m.planetID = planetID;  
        m.position = new double3(planetPosition.x, planetPosition.y, planetPosition.z - (planetScale.z * 1.5)); 
        m.identifier = 2; 
        m.uniqueMoonID = i+1; 
        m.CalculateProperties(); 

        moons2.Add(m); 

        GameObject gm = moon; 
        Rigidbody rbM; 

        rbM = gm.GetComponent<Rigidbody>(); 
        rbM.mass = (float)m.mass; 

        Vector3 posConversion;
        posConversion.x = (float)m.position.x;
        posConversion.y = (float)m.position.y;
        posConversion.z = (float)m.position.z;
        gm.transform.position = posConversion; 

        Vector3 scaleConversion;
        scaleConversion.x = (float)m.scale.x;
        scaleConversion.y = (float)m.scale.y;
        scaleConversion.z = (float)m.scale.z;
        gm.transform.localScale = scaleConversion; 

        //Set up material
        var moonRenderer = gm.GetComponent<Renderer>(); 
        Material moonMat = new Material(pgm); 
        moonMat.SetFloat("_Roughness", UnityEngine.Random.Range(3f, 5f)); 
        moonMat.SetColor("_Color", Color.grey); 
        moonRenderer.material = moonMat; 
        //moonRenderer.sharedMaterial.SetColor("_Color", Color.grey); 
        Instantiate(gm);
        moonCounter++;  
    }
    
}

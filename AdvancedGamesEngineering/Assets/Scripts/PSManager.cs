using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class PSManager : MonoBehaviour
{
    //This will manage the behaviour of stars and planets in the simulation. 

    //Variables
    readonly double G = 6.670e-11;
    readonly double S = 1.0e-3; //Scale 
    GameObject[] celestialBodies; 
    GameObject[] moons; 
    //Moon[] moons2; 
    List<Moon> moons2 = new List<Moon>();  
    Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
    public Star starData; 
    public GameObject ps; 
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
        starData = ps.GetComponent<Star>(); 
        //Seed randomisation
        UnityEngine.Random.InitState(7);

        //Generate planets and fill celestial bodies array. Note: Star will always be at index 0 
        GeneratePlanets();
        //moons2 = new Moon[moonCounter]; 
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        moons = GameObject.FindGameObjectsWithTag("Moon"); 
        for(int i = 0; i < celestialBodies.Length - 1; i++){
            celestialBodiesPhysics.Add(planets[i]); 
        }
        for(int j = 0; j < moons.Length; j++){
            celestialBodiesPhysics.Add(moons2[j]); 
        }
        //masses[0] = starData.mass;
        for(int i = 1; i < celestialBodies.Length; i++){
            //masses[i] = planets[i].mass; 
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
                positions[0] = starData.position; 
            }
            else{
                positions[i] = planets[i-1].position; 
            }
        }
        //OverallGravitationalPull(); 
        //Integration(); 
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
                        starData.accumulatedForce += force;
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
            double3 tempPosDownscale = S * planets[j].position; 
            Vector3 tempPos;
            tempPos.x = (float)tempPosDownscale.x;
            tempPos.y = (float)tempPosDownscale.y;
            tempPos.z = (float)tempPosDownscale.z;

            double3 tempVelDownscale = S * planets[j].velocity;
            Vector3 tempVel;
            tempVel.x = (float)tempVelDownscale.x;  
            tempVel.y = (float)tempVelDownscale.y;  
            tempVel.z = (float)tempVelDownscale.z;  

            celestialBodies[planets[j].uniquePlanetID].transform.position = tempPos; 
            celestialBodies[planets[j].uniquePlanetID].GetComponent<Rigidbody>().velocity = tempVel; 
        }

        for(int k = 0; k < moons2.Count; k++){
            double3 tempPosDownscale = S * moons2[k].position;
            Vector3 tempPos;
            tempPos.x = (float)tempPosDownscale.x;
            tempPos.y = (float)tempPosDownscale.y;
            tempPos.z = (float)tempPosDownscale.z;

            double3 tempVelDownscale = S * moons2[k].velocity; 
            Vector3 tempVel;
            tempVel.x = (float)tempVelDownscale.x;  
            tempVel.y = (float)tempVelDownscale.y;  
            tempVel.z = (float)tempVelDownscale.z;  

            moons[moons2[k].uniqueMoonID].transform.position = tempPos; 
            moons[moons2[k].uniqueMoonID].GetComponent<Rigidbody>().velocity = tempVel; 
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
            double x = starData.position.x - planets[i-1].position.x;
            double y = starData.position.y - planets[i-1].position.y;
            double z = starData.position.z - planets[i-1].position.z;
            double r = System.Math.Sqrt((x * x) + (y * y) + (z * z));
            Debug.Log(r + " PLANET R"); 
            Debug.Log(System.Math.Sqrt((G * m) / r) + " PLANET SQRT"); 
            double3 initialVelocity = directionZ * System.Math.Sqrt((G * m) / r);
            planets[i-1].velocity = initialVelocity; 

            Debug.Log(initialVelocity + "INITIAL VELOCITY"); 

            //Float conversion for display
            double3 initVelDownscale = S * initialVelocity; 
            Debug.Log(initVelDownscale + "INITIAL VELOCITY DOWNSCALE"); 
            Vector3 initVel; 
            initVel.x = (float)initVelDownscale.x;
            initVel.y = (float)initVelDownscale.y;
            initVel.z = (float)initVelDownscale.z;
            Debug.Log(initVel + "INITVEL"); 
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
            double3 initVelDownscale = S * initialVelocity; 
            Vector3 initVel; 
            initVel.x = (float)initVelDownscale.x;
            initVel.y = (float)initVelDownscale.y;
            initVel.z = (float)initVelDownscale.z;
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
            float planetMass = UnityEngine.Random.Range((((float)starData.mass) * 0.003f), (((float)starData.mass) * 0.09f));
            p.mass = (double)planetMass; 
            Debug.Log(p.mass + " Planet Mass Proper"); 
            p.position = new double3((starData.mass * (i + 1.0)) * 100.0, 0, 0);
            Debug.Log(p.position + " Planet Pos Proper"); 
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
            double massDownscale = S * p.mass; 
            Debug.Log(massDownscale + " Planet Mass Downscale"); 
            rbG.mass = (float)massDownscale; 

            double3 posDownscale = S * p.position; 
            Debug.Log(posDownscale + " Planet Pos Downscale"); 
            Vector3 posConversion;
            posConversion.x = (float)posDownscale.x;
            posConversion.y = (float)posDownscale.y;
            posConversion.z = (float)posDownscale.z;
            Debug.Log(posConversion + " Planet pos down conversion"); 
            g.transform.position = posConversion; 

            double3 scaleDownscale = S * p.scale; 
            Debug.Log(scaleDownscale + " Planet scale downscale"); 
            Vector3 scaleConversion;
            scaleConversion.x = (float)scaleDownscale.x;
            scaleConversion.y = (float)scaleDownscale.y;
            scaleConversion.z = (float)scaleDownscale.z;
            Debug.Log(scaleConversion + " planet scale down conversion"); 
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
                //GenerateMoon(p.mass, p.position, planetNr, p.scale, moonCounter); 
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
        double massDownscale = S * m.mass; 
        rbM.mass = (float)massDownscale; 

        double3 posDownscale = S * m.position; 
        Vector3 posConversion;
        posConversion.x = (float)posDownscale.x;
        posConversion.y = (float)posDownscale.y;
        posConversion.z = (float)posDownscale.z;
        gm.transform.position = posConversion; 
        
        double3 scaleDownscale = S * m.scale; 
        Vector3 scaleConversion;
        scaleConversion.x = (float)scaleDownscale.x;
        scaleConversion.y = (float)scaleDownscale.y;
        scaleConversion.z = (float)scaleDownscale.z;
        gm.transform.localScale = scaleConversion; 

        //Set up material
        var moonRenderer = gm.GetComponent<Renderer>(); 
        Material moonMat = new Material(pgm); 
        moonMat.SetFloat("_Roughness", UnityEngine.Random.Range(3.0f, 5.0f)); 
        moonMat.SetColor("_Color", Color.grey); 
        moonRenderer.material = moonMat; 
        //moonRenderer.sharedMaterial.SetColor("_Color", Color.grey); 
        Instantiate(gm);
        moonCounter++;  
    }
    
}

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
    readonly float timeScale = 0.05f; 
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

    // Start is called before the first frame update
    void Start()
    {
        starData = ps.GetComponent<Star>(); 
        
        //Generate star data. 
        starData.GenerateStar(MainMenuManager.absoluteMagnitude, MainMenuManager.spectralClassification);
        SetupStarGameObject(); 

        //Seed randomisation
        UnityEngine.Random.InitState(7);

        //Generate planets and fill celestial bodies array. Note: Star will always be at index 0 of gameobjects vector
        GeneratePlanets();
        //moons2 = new Moon[moonCounter]; 
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        moons = GameObject.FindGameObjectsWithTag("Moon"); 
        
        celestialBodiesPhysics.Add(starData);

        for(int i = 0; i < celestialBodies.Length - 1; i++){
            celestialBodiesPhysics.Add(planets[i]); 
        }
        for(int j = 0; j < moons.Length; j++){
            celestialBodiesPhysics.Add(moons2[j]); 
        }
        
        InitialOrbitVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        //Rotational speed based on size. Simple implementation for demonstration purposes
        foreach(GameObject x in celestialBodies){
            x.transform.Rotate(new Vector3(0, -(float)x.GetComponent<Rigidbody>().mass * 10.0f, 0) * (Time.deltaTime * timeScale)); 
        }
        foreach(GameObject m in moons){
            m.transform.Rotate(new Vector3(0, -(float)m.GetComponent<Rigidbody>().mass, 0) * (Time.deltaTime * timeScale)); 
        }

        OverallGravitationalPull(); 
        Integration(); 
    }

    void OverallGravitationalPull(){
        //Calculate gravitational pull using Newton's law of universil gravitation:
        //  F = G * ((m1 * m2)/r^2)

        //Figure out how to implement the moons here
        for(int i = 0; i < celestialBodiesPhysics.Count; i++) {
            double3 force = new double3(); 
            for(int j = 0; j < celestialBodiesPhysics.Count; j++) {
                if(!celestialBodiesPhysics[i].Equals(celestialBodiesPhysics[j])){
                    double m1 = celestialBodiesPhysics[i].mass;
                    double m2 = celestialBodiesPhysics[j].mass;
                    double x =  celestialBodiesPhysics[i].position.x - celestialBodiesPhysics[j].position.x;
                    double y =  celestialBodiesPhysics[i].position.y - celestialBodiesPhysics[j].position.y;
                    double z =  celestialBodiesPhysics[i].position.z - celestialBodiesPhysics[j].position.z;
                    double r = System.Math.Sqrt((x * x) + (y * y) + (z * z)) * 1000.0;
                    Debug.Log(r + " Planet R");
                    force += (((celestialBodiesPhysics[i].position - celestialBodiesPhysics[j].position) * 1000.0)/r) * (G * (m1 * m2)/(r * r));
                    Debug.Log(force + " Planet: " + i + " force total");
                    celestialBodiesPhysics[i].accumulatedForce += force; 
                    /*
                    if(i == 0){
                        starData.accumulatedForce += force;
                    }
                    else{
                        celestialBodiesPhysics[i].accumulatedForce += force; 
                    }
                    */
                }
            }
        }
    }

    //Symplectic euler integration
    void Integration(){
        double3 vel;
        double3 pos; 
        for(int i = 0; i < celestialBodiesPhysics.Count; i++){
            double3 accel = celestialBodiesPhysics[i].accumulatedForce / celestialBodiesPhysics[i].mass; 
            vel = celestialBodiesPhysics[i].velocity + (accel * (Time.deltaTime * timeScale));
            pos = celestialBodiesPhysics[i].position + (vel * (Time.deltaTime * timeScale)); 
            celestialBodiesPhysics[i].velocity = vel; 
            celestialBodiesPhysics[i].position = pos;  

            if(i == 0){
                double3 tempPosDownscale = S * celestialBodiesPhysics[i].position; 
                Vector3 tempPos;
                tempPos.x = (float)tempPosDownscale.x;
                tempPos.y = (float)tempPosDownscale.y;
                tempPos.z = (float)tempPosDownscale.z;

                double3 tempVelDownscale = S * celestialBodiesPhysics[i].velocity;
                Vector3 tempVel;
                tempVel.x = (float)tempVelDownscale.x;  
                tempVel.y = (float)tempVelDownscale.y;  
                tempVel.z = (float)tempVelDownscale.z;

                celestialBodies[i].transform.position = tempPos; 
                celestialBodies[i].GetComponent<Rigidbody>().velocity = tempVel; 
            }
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

    void InitialOrbitVelocity(){
        //Calculate initial orbital velocity using circular orbit instant velocity
        double3 directionX = new double3(1.0, 0.0, 0.0);
        double3 directionZ = new double3(0.0, 0.0, 1.0);
        //double3 direction = new double3(1.0, 0.0, 1.0);

        //Initial planet velocities
        for(int i = 1; i < celestialBodies.Length; i++){
            double m = planets[i-1].mass;
            double x = starData.position.x - planets[i-1].position.x;
            double y = starData.position.y - planets[i-1].position.y;
            double z = starData.position.z - planets[i-1].position.z;
            double r = System.Math.Sqrt((x * x) + (y * y) + (z * z)) * 1000.0;
            Debug.Log(r + " PLANET R M"); 
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
            double r = System.Math.Sqrt((x * x) + (y * y) + (z * z)) * 1000.0;
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
    }

    void SetupStarGameObject(){
        Rigidbody rbS; 
        rbS = star.GetComponent<Rigidbody>(); 

        //Set star starting position
        star.transform.position = new Vector3(0, 0, 0);

        //set game object mass
        rbS.mass = (float)starData.massDownscale; 
        Debug.Log(rbS.mass + " RB MASS SUN");

        //Set game object scale
        var starScale = star.transform.localScale;
        starScale *= (float)starData.radDownscale * 2.0f; 
        Debug.Log(starScale + " STAR SCALE"); 
        star.transform.localScale = starScale;
        
        //Star colour handling. 
        var sphereRenderer = star.GetComponent<Renderer>();
        sphereRenderer.material.SetColor("_DetailColor", Color.white);
        sphereRenderer.material.SetColor("_StarColor", starData.colour);;
    }

    void GeneratePlanets(){
        int moonCounter = 0;         
        //Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
        for(int i = 0; i < MainMenuManager.numOfPlanets; i++){
            Planet p = new Planet();
            float planetMass = UnityEngine.Random.Range((((float)starData.mass) * 0.003f), (((float)starData.mass) * 0.09f));
            p.mass = (double)planetMass; 
            Debug.Log(p.mass + " Planet Mass Proper"); 
            p.position = new double3((starData.radius * (i + 1.0) * 100.0), 0, 0);
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
        m.mass = planetMass * (double)UnityEngine.Random.Range(0.08f, 0.27f);
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

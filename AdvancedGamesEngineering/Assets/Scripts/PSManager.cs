using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; 

public class PSManager : MonoBehaviour
{
    //This will manage the behaviour of stars and planets in the simulation. 

    //Variables
    readonly double G = 6.670e-11;
    readonly float G2 = 6.670e-11f;
    readonly double S = 1.0e-4; //Scale 
    readonly float S2 = 1.75e+4f; //DisplayScale
    readonly double S3 = 1.0e-18; //ForceScale 
    readonly double S4 = 1.0e-12; //RotScale 
    readonly double S5 = 1.0e-15; //DispMass 
    //GameObject[] celestialBodies; 
    List<GameObject> celestialBodies = new List<GameObject>(); 
    GameObject[] moons; 
    //Moon[] moons2; 
    List<Moon> moons2 = new List<Moon>();  
    Planet[] planets = new Planet[MainMenuManager.numOfPlanets]; 
    public Star starData; 
    public GameObject ps; 
    public GameObject star;
    public GameObject planet; 
    public GameObject moon; 
    public GameObject asteroidTemplate; 
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
    List<GameObject> asteroidsGO = new List<GameObject>();
    List<Asteroid> asteroids = new List<Asteroid>();   

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; 
        //starData = ps.GetComponent<Star>();
        starData = new Star();  
        
        //Generate star data. 
        starData.GenerateStar(MainMenuManager.absoluteMagnitude, MainMenuManager.spectralClassification);
        SetupStarGameObject(); 

        //Seed randomisation
        UnityEngine.Random.InitState(7);

        //Generate planets and fill celestial bodies array. Note: Star will always be at index 0 of gameobjects vector
        GeneratePlanets();
        //moons2 = new Moon[moonCounter]; 
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("CelestialBody")){
            celestialBodies.Add(g); 
        }
        //celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        moons = GameObject.FindGameObjectsWithTag("Moon"); 
        
        celestialBodiesPhysics.Add(starData);

        for(int i = 0; i < celestialBodies.Count - 1; i++){
            celestialBodiesPhysics.Add(planets[i]); 
        }
        for(int j = 0; j < moons.Length; j++){
            celestialBodiesPhysics.Add(moons2[j]); 
        }

        for(int i = 0; i < celestialBodiesPhysics.Count; i++){
            //Debug.Log(celestialBodiesPhysics[i].identifier); 
        }
        
        InitialOrbitVelocity();
        InitialRotationalVelocity(); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ClearForces();
        OverallGravitationalPull(); 
        Rotation(); 
        Integration();  
    }

    void Update(){
        Display(); 
        MoonDisplay();
        if(asteroids.Count > 0){
            AsteroidDisplay(); 
        } 
    }

    void ClearForces(){
        double3 clear = new double3(0.0, 0.0, 0.0);
        for(int i = 0; i < celestialBodiesPhysics.Count; i++){
            celestialBodiesPhysics[i].accumulatedForce = clear; 
            //celestialBodiesPhysics[i].rotationalForce = clear; 
        }
    }

    void OverallGravitationalPull(){
        //Calculate gravitational pull using Newton's law of universil gravitation:
        //  F = G * ((m1 * m2)/r^2)

        for(int i = 1; i < celestialBodiesPhysics.Count; i++) {
            double3 force = new double3(); 
            //for(int j = 0; j < i+1; j++) {
            for(int j = 1; j < celestialBodiesPhysics.Count; j++) {
                if(!celestialBodiesPhysics[i].Equals(celestialBodiesPhysics[j])){
                    double m1 = celestialBodiesPhysics[i].mass;
                    double m2 = celestialBodiesPhysics[j].mass;
                    double r = math.distance(celestialBodiesPhysics[i].position, celestialBodiesPhysics[j].position) * 1000.0; 
                    
                    double3 dir = ((((celestialBodiesPhysics[i].position - celestialBodiesPhysics[j].position)) * 1000.0)/r); 

                    force += dir * ((G * (m1 * m2)/(r * r)));

                    celestialBodiesPhysics[i].accumulatedForce += force; 
                    celestialBodiesPhysics[j].accumulatedForce -= force; 
                }
            }

            if(celestialBodiesPhysics[i].identifier == 3){
                Debug.Log(force + " TOTAL FORCE FOR CB " + i); 
            }
            
        }
    }

    void Rotation(){
        double3 rot; 
        double3 rotDownscale; 
        Vector3 tempRot; 
        Vector3 rotAxis; 

        rot = celestialBodiesPhysics[0].rotationalForce * Time.deltaTime; 
        rotDownscale = S * rot; 
        //Debug.Log(rotDownscale + " ROT DOWNSCALE"); 
        tempRot.x = (float)rotDownscale.x; 
        tempRot.y = (float)rotDownscale.y; 
        tempRot.z = (float)rotDownscale.z; 
        star.transform.GetComponent<Rigidbody>().angularVelocity = tempRot; 

        rotAxis.x = 0.0f; 
        rotAxis.y = (float)celestialBodiesPhysics[0].rotationalAxis.y; 
        rotAxis.z = (float)celestialBodiesPhysics[0].rotationalAxis.z;
        star.transform.Rotate(rotAxis);  

        for(int j = 0; j < planets.Length; j++){
            rot = planets[j].rotationalForce * Time.deltaTime; 
            rotDownscale = S * rot; 
            //Debug.Log(rotDownscale + " ROT DOWNSCALE"); 
            tempRot.x = (float)rotDownscale.x; 
            tempRot.y = (float)rotDownscale.y; 
            tempRot.z = (float)rotDownscale.z;  
            celestialBodies[planets[j].uniquePlanetID].GetComponent<Rigidbody>().angularVelocity = tempRot; 

            rotAxis.x = 0.0f; 
            rotAxis.y = (float)celestialBodiesPhysics[planets[j].uniquePlanetID].rotationalAxis.y; 
            rotAxis.z = (float)celestialBodiesPhysics[planets[j].uniquePlanetID].rotationalAxis.z;
            celestialBodies[planets[j].uniquePlanetID].transform.Rotate(rotAxis);
        }

        for(int k = 0; k < moons2.Count; k++){
            rot = moons2[k].rotationalForce * Time.deltaTime; 
            rotDownscale = S * rot; 
            //Debug.Log(rotDownscale + " ROT DOWNSCALE"); 
            tempRot.x = (float)rotDownscale.x; 
            tempRot.y = (float)rotDownscale.y; 
            tempRot.z = (float)rotDownscale.z; 
            celestialBodies[moons2[k].uniqueMoonID].GetComponent<Rigidbody>().angularVelocity = tempRot;

            rotAxis.x = 0.0f; 
            rotAxis.y = (float)celestialBodiesPhysics[moons2[k].uniqueMoonID].rotationalAxis.y; 
            rotAxis.z = (float)celestialBodiesPhysics[moons2[k].uniqueMoonID].rotationalAxis.z;
            celestialBodies[moons2[k].uniqueMoonID].transform.Rotate(rotAxis);
        }

        for(int l = 0; l < asteroids.Count; l++){
            rot = asteroids[l].rotationalForce * Time.deltaTime; 
            rotDownscale = S3 * rot; 
            //Debug.Log(rotDownscale + " ROT DOWNSCALE"); 
            tempRot.x = (float)rotDownscale.x; 
            tempRot.y = (float)rotDownscale.y; 
            tempRot.z = (float)rotDownscale.z; 
            asteroidsGO[l].GetComponent<Rigidbody>().angularVelocity = tempRot;

            rotAxis.x = 0.0f; 
            rotAxis.y = (float)asteroids[l].rotationalAxis.y; 
            rotAxis.z = (float)asteroids[l].rotationalAxis.z;
            asteroidsGO[l].transform.Rotate(rotAxis);

            Debug.Log(asteroidsGO[l].GetComponent<Rigidbody>().angularVelocity + " ANG VEL "); 
        }
    }

    //Symplectic euler integration
    void Integration(){
        double3 vel;
        double3 pos; 

        for(int i = 0; i < celestialBodiesPhysics.Count; i++){
            double3 rotFEffect = new double3(celestialBodiesPhysics[i].rotationalForce.x, 0.0, celestialBodiesPhysics[i].rotationalForce.z); 
            double3 accel = (celestialBodiesPhysics[i].accumulatedForce + rotFEffect) / celestialBodiesPhysics[i].mass; 
            vel = celestialBodiesPhysics[i].velocity + (accel * (Time.deltaTime));
            pos = celestialBodiesPhysics[i].position + (vel * (Time.deltaTime)); 
            celestialBodiesPhysics[i].velocity = vel; 
            celestialBodiesPhysics[i].position = pos; 
        }
    }
    
    void Display(){
        float m1 = celestialBodies[0].GetComponent<Rigidbody>().mass; 
        for (int i = 1; i < celestialBodies.Count; i++){
            float m2 = celestialBodies[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 
            celestialBodies[i].GetComponent<Rigidbody>().AddForce(((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * ((G2 * S2) * (m1 * m2) / (r * r))));
        }
        
        double3 forceDownscaleS = S3 * celestialBodiesPhysics[0].accumulatedForce; 
        Vector3 forceVectorS; 
        forceVectorS.x = (float)forceDownscaleS.x;
        forceVectorS.y = (float)forceDownscaleS.y;
        forceVectorS.z = (float)forceDownscaleS.z;
        celestialBodies[0].GetComponent<Rigidbody>().AddForce(forceVectorS); 

        for(int i = 0; i < planets.Length; i++){
            double3 forceDownscaleP = S3 * planets[i].accumulatedForce; 
            Vector3 forceVectorP; 
            forceVectorP.x = (float)forceDownscaleP.x;
            forceVectorP.y = (float)forceDownscaleP.y;
            forceVectorP.z = (float)forceDownscaleP.z;
            celestialBodies[planets[i].uniquePlanetID].GetComponent<Rigidbody>().AddForce(forceVectorP); 
        }
    }

    void MoonDisplay(){
        for(int i = 0; i < moonCounter; i++){
            float m1 = celestialBodies[moons2[i].planetID].GetComponent<Rigidbody>().mass;
            float m2 = moons[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[moons2[i].planetID].transform.position, moons[i].transform.position); 

            //Force handeling and integration using Symplectic euler. 
            Vector3 force = (celestialBodies[moons2[i].planetID].transform.position - moons[i].transform.position).normalized * ((G2) * (m1 * m2) / (r * r) * S2); 
            Vector3 force2 = -(moons[i].transform.position - celestialBodies[moons2[i].planetID].transform.position).normalized * ((G2) * (m1 * m2) / (r * r) * S2); 
            Vector3 accel = (force + force2) / m2; 
            Vector3 vel = moons[i].GetComponent<Rigidbody>().velocity + (Time.deltaTime * accel);
            Vector3 pos = moons[i].transform.position + (Time.deltaTime * vel); 
            moons[i].transform.position = pos; 
            moons[i].GetComponent<Rigidbody>().velocity = vel; 
        }

        for(int i = 0; i < moons2.Count; i++){
            double3 forceDownscaleM = S3 * moons2[i].accumulatedForce; 
            Vector3 forceVectorM; 
            forceVectorM.x = (float)forceDownscaleM.x;
            forceVectorM.y = (float)forceDownscaleM.y;
            forceVectorM.z = (float)forceDownscaleM.z;
            celestialBodies[moons2[i].uniqueMoonID].GetComponent<Rigidbody>().AddForce(forceVectorM); 
        }
    }

    void AsteroidDisplay(){
        
        /*
        Debug.Log("ASTEROID DISPLAY"); 
        float m1 = celestialBodies[0].GetComponent<Rigidbody>().mass; 
        for(int i = 0; i < asteroidsGO.Count; i++){
            float m2 = asteroidsGO[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, asteroidsGO[i].transform.position); 
            asteroidsGO[i].GetComponent<Rigidbody>().AddForce(((celestialBodies[0].transform.position - asteroidsGO[i].transform.position).normalized * ((G2 * S2) * (m1 * m2) / (r * r))));
            Debug.Log(asteroidsGO[i].GetComponent<Rigidbody>().velocity + " INITIAL RB VELOCITY NO UNIVERSAL"); 
        }

        for(int i = 0; i < asteroids.Count; i++){
            double3 forceDownscaleA = S3 * asteroids[i].accumulatedForce; 
            Vector3 forceVectorA; 
            forceVectorA.x = (float)forceDownscaleA.x;
            forceVectorA.y = (float)forceDownscaleA.y;
            forceVectorA.z = (float)forceDownscaleA.z;
            asteroidsGO[i].GetComponent<Rigidbody>().AddForce(forceVectorA); 
            Debug.Log(forceVectorA + " AST RB FORCE"); 
            Debug.Log(asteroidsGO[i].GetComponent<Rigidbody>().velocity + " INITIAL RB VELOCITY UNIVERSAL");
        }
        */
    }

    void InitialOrbitVelocity(){
        //Calculate initial orbital velocity using circular orbit instant velocity
        double3 directionX = new double3(1.0, 0.0, 0.0);
        double3 directionZ = new double3(0.0, 0.0, 1.0);
        double3 direction = new double3(-1.0, 0.0, 1.0);
        
        for(int i = 0; i < celestialBodiesPhysics.Count; i++) {
            double3 velocity = new double3();
            if(celestialBodiesPhysics[i].identifier == 0){
                //Float conversion for display
                double3 initVelDownscale = S * celestialBodiesPhysics[i].velocity; 
                
                Vector3 initVel; 
                initVel.x = (float)initVelDownscale.x;
                initVel.y = (float)initVelDownscale.y;
                initVel.z = (float)initVelDownscale.z;

                star.GetComponent<Rigidbody>().velocity += initVel; 
            }
            if(celestialBodiesPhysics[i].identifier == 1){
                double r = math.distance(celestialBodiesPhysics[0].position, celestialBodiesPhysics[i].position) * 1000; 
                    
                //Debug.Log(r/1000.0 + " PLANET R KM BETWEEN CB " + i + " AND STAR"); 
                //Debug.Log(System.Math.Sqrt((G * celestialBodiesPhysics[0].mass) / r) + " PLANET SQRT");

                velocity += directionZ * System.Math.Sqrt((G * celestialBodiesPhysics[0].mass) / r); 
                celestialBodiesPhysics[i].velocity = velocity;
                //Debug.Log(celestialBodiesPhysics[i].velocity + " INIT VEL"); 
            }
        }

        for(int i = 0; i < moons2.Count; i++){
            double3 velocity = new double3();
            //double m2 = planets[moons2[i].planetID - 1].mass; 
            double m2 = moons2[i].mass; 
            double r = math.distance(planets[moons2[i].planetID - 1].position, moons2[i].position);

            //Debug.Log(r/1000.0 + " PLANET R KM BETWEEN MOON " + i + " AND PLANET"); 
            //Debug.Log(System.Math.Sqrt((G * m2) / r) + " PLANET SQRT");

            velocity += directionX * System.Math.Sqrt((G * m2) / r); 
            moons2[i].velocity = velocity;
            //Debug.Log(moons2[i].velocity + " INIT VEL");
        }    

        for(int j = 0; j < planets.Length; j++){
            double3 tempVelDownscale = S * planets[j].velocity;
            //Debug.Log(tempVelDownscale + "INITIAL VELOCITY DOWNSCALE"); 

            Vector3 tempVel;
            tempVel.x = (float)tempVelDownscale.x;  
            tempVel.y = (float)tempVelDownscale.y;  
            tempVel.z = (float)tempVelDownscale.z;  
            //Debug.Log(tempVel + "INITVEL"); 

            celestialBodies[planets[j].uniquePlanetID].GetComponent<Rigidbody>().velocity += tempVel; 
        }

        for(int k = 0; k < moons2.Count; k++){
            double3 tempVelDownscale = S * moons2[k].velocity; 
            //Debug.Log(tempVelDownscale + "INITIAL VELOCITY DOWNSCALE"); 

            Vector3 tempVel;
            tempVel.x = (float)tempVelDownscale.x;  
            tempVel.y = (float)tempVelDownscale.y;  
            tempVel.z = (float)tempVelDownscale.z;  
            //Debug.Log(tempVel + "INITVEL"); 

            moons[moons2[k].uniqueMoonID].GetComponent<Rigidbody>().velocity += tempVel; 
        }
    }

    void InitialRotationalVelocity(){
        //Rotational force :
        // F = (m * v^2)/r
        for(int i = 0; i < celestialBodiesPhysics.Count; i++){
            double3 force = new double3(); 
            double3 linearVelocity = new double3(); 
            double3 angularVelocity= new double3(); 
            double3 tangentialVelocity= new double3(); 
            double m = celestialBodiesPhysics[i].mass; 
            double r = celestialBodiesPhysics[i].radius * 1000.0; //Coversion to meters
            double conversionToSeconds = 86400.0; 

            float obliquity = UnityEngine.Random.Range(0.03f, 82.23f); //In degrees
            double3 rotationalAxis = math.normalize(new double3(0.0, -1.0 * (double)Mathf.Sin(obliquity), -1.0 * (double)Mathf.Cos(obliquity))); 
            celestialBodiesPhysics[i].rotationalAxis = rotationalAxis; 
            
            //angularVelocity = rotationalAxis * System.Math.Sqrt((G * m)/r); 
            if(celestialBodiesPhysics[i].identifier == 0 || celestialBodiesPhysics[i].identifier == 2){
                tangentialVelocity = rotationalAxis * ((2.0 * 3.14159265359 * celestialBodiesPhysics[i].radius)/(27.0 * conversionToSeconds)); 
            }
            if(celestialBodiesPhysics[i].identifier == 1){
                tangentialVelocity = rotationalAxis * ((2.0 * 3.14159265359 * celestialBodiesPhysics[i].radius)/((double)UnityEngine.Random.Range(6.0f, 58.0f)) * conversionToSeconds); 
            }
            //Debug.Log(tangentialVelocity + " TANG VEL"); 

            linearVelocity = tangentialVelocity/r; 
            //Debug.Log(linearVelocity + " LIN VEL"); 

            double x = celestialBodiesPhysics[i].position.x + r; 
            double y = celestialBodiesPhysics[i].position.y + r; 
            double z = celestialBodiesPhysics[i].position.z + r; 
            double absoluteRadius = System.Math.Sqrt((x * x) + (y * y) + (z * z)); 
            angularVelocity = (r * linearVelocity)/(absoluteRadius); 
            //Debug.Log(angularVelocity + " ANG VEL"); 
            
            force = (m * (angularVelocity * angularVelocity)) / r; 

            celestialBodiesPhysics[i].rotationalForce += force;
            //Debug.Log(celestialBodiesPhysics[i].rotationalForce + " ROT FORCE");  
        }
 
        for(int i = 0; i < celestialBodies.Count; i++){
            celestialBodies[i].GetComponent<Rigidbody>().inertiaTensor = new Vector3(0.0f, 0.0f, 0.0f);
        }
        for(int i = 0; i < moons.Length; i++){
            moons[i].GetComponent<Rigidbody>().inertiaTensor = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    void SetupStarGameObject(){
        Rigidbody rbS; 
        rbS = star.GetComponent<Rigidbody>(); 

        //Set star starting position
        star.transform.position = new Vector3(0, 0, 0);

        //set game object mass
        double rbMass = starData.mass * S5; 
        rbS.mass = (float)rbMass; 
        //Debug.Log(rbS.mass + " RB MASS SUN");

        //Set game object scale
        var starScale = star.transform.localScale;
        starScale *= (float)starData.radDownscale * 2.0f; 
        //Debug.Log(starScale + " STAR SCALE"); 
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
            
            if(MainMenuManager.spectralClassification.Equals("T") || MainMenuManager.spectralClassification.Equals("t")){
                p.mass = 3.285e+23; 
                p.radius = 2439.7;
                p.position = new double3(58000000.0, 0.0, 0.0);
                p.identifier = 1; 
                p.uniquePlanetID = i+1;
                p.CalculateProperties(); 
                planets[i] = p;
            }
            else{
                double planetMassScalar = 2*(double)Math.Pow(10.0, 30.0); //Solar mass used as constant
                float planetMass = UnityEngine.Random.Range(0.0000001651f, 0.0009543f);
                p.mass = (double)planetMass * starData.mass; 
                //Debug.Log(p.mass + " Planet Mass Proper"); 
        
                double planetRadScalar = 696340.0; //Solar radius used as constant. 
                float planetRadRand = UnityEngine.Random.Range(0.0035514f, 0.10049f); //Random radius based on solar system
                p.radius = (double)planetRadRand * planetRadScalar; 

                double planetPosScalar = (starData.radius * (i + 1.0) * 100.0); 
                //double planetPosScalar = (starData.radius + p.radius); 

                p.position = new double3(planetPosScalar, 0, 0);
                //p.position = new double3(planetPosScalar, 0, planetPosScalar * -0.5);
                //Debug.Log(p.position + " Planet Pos Proper"); 

                p.identifier = 1; 
                p.uniquePlanetID = i+1; 
            
                p.CalculateProperties(); 

                //Calculate habitability. Stefan-Boltzmann Law
                double distFromStar = math.distance(starData.position, p.position); //in meters
                if(distFromStar > starData.minHabitableRadius && distFromStar < starData.maxHabitableRadius){
                    p.isHabitable = true; 
                }

                planets[i] = p;
            }
             
        }

        //generate planet game objects
        int planetNr = 1;
        foreach (Planet p in planets){
            GameObject g = planet;  
            Rigidbody rbG; 
            double planetMassScalar = 2*(double)Math.Pow(10.0, 30.0); //Solar mass used as constant

            //Display planet
            rbG = g.GetComponent<Rigidbody>();
            double massDownscale = S5 * p.mass; 
            //Debug.Log(massDownscale + " Planet Mass Downscale"); 
            rbG.mass = (float)massDownscale; 

            double3 posDownscale = S * p.position; 
            //Debug.Log(posDownscale + " Planet Pos Downscale"); 
            Vector3 posConversion;
            posConversion.x = (float)posDownscale.x;
            posConversion.y = (float)posDownscale.y;
            posConversion.z = (float)posDownscale.z;
            //Debug.Log(posConversion + " Planet pos down conversion"); 
            g.transform.position = (posConversion); 
            //g.transform.position = new Vector3((star.transform.localScale.x * ((planetNr-1) + 2.0f)) * 2.0f, 0, 0);

            double3 scaleDownscale = S * p.scale; 
            //Debug.Log(scaleDownscale + " Planet scale downscale"); 
            Vector3 scaleConversion;
            scaleConversion.x = (float)scaleDownscale.x;
            scaleConversion.y = (float)scaleDownscale.y;
            scaleConversion.z = (float)scaleDownscale.z;
            //Debug.Log(scaleConversion + " planet scale down conversion"); 
            g.transform.localScale = scaleConversion;

            var planetRenderer = g.GetComponent<Renderer>();
            
            //Assign correct material
            if(planetNr <= (MainMenuManager.numOfPlanets/2)){
                Material mat = new Material(pgr);
                if(p.isHabitable == false){
                    Debug.Log("UNINHABITABLE"); 
                    mat.SetInt("_IsHabitable", 0); 
                    mat.SetColor("_Color", rockyPanet);
                    Color randColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1.0f);
                    mat.SetColor("_AtmosphereColour", randColor); 
                    float rand = UnityEngine.Random.Range(1, 10); 
                    mat.SetFloat("_Scale", rand); 
                    float rand2 = UnityEngine.Random.Range(1f, 3f); 
                    mat.SetFloat("_Scale", rand2);
                }             
                if(p.isHabitable == true){
                    Debug.Log("HABITABLE");
                    mat.SetInt("_IsHabitable", 1); 
                    mat.SetFloat("_Scale3", UnityEngine.Random.Range(2.0f, 4.0f)); 
                    mat.SetFloat("_ForestScale", UnityEngine.Random.Range(5.0f, 10.0f)); 
                    mat.SetFloat("_MountainScale", UnityEngine.Random.Range(0.0f, 3.0f)); 
                    mat.SetFloat("_DesertScale", UnityEngine.Random.Range(0.0f, 3.0f)); 
                }
                 
                planetRenderer.material = mat; 
                p.type = 1;                
            }

            if(planetNr > (MainMenuManager.numOfPlanets/2)){   
                Material mat = new Material(pgg); 
                System.Random rnd = new System.Random();
                //int lookDecider = rnd.Next(0, 2);
                int lookDecider = UnityEngine.Random.Range(0, 2); 
                //With rings
                //lookDecider -= 1; 
                if(lookDecider == 0){
                    Debug.Log("RINGS");
                    //mat.SetColor("_Color", gassyPanet);
                    mat.SetColor("_Color", Color.HSVToRGB((UnityEngine.Random.Range(0.0f, 66.0f)/360.0f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
                    Color randColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1.0f);
                    mat.SetColor("_AccentColor", randColor); 
                    float randDensity = UnityEngine.Random.Range(1, 3); 
                    mat.SetFloat("_CellDensity", randDensity);  
                    float randRingAccent = UnityEngine.Random.Range(1, 10); 
                    mat.SetFloat("_RingAccents", randRingAccent);
                    float randNumRings = UnityEngine.Random.Range(1, 5); 
                    mat.SetFloat("_NumberOfRings", randNumRings);
                    mat.SetInt("_Rings", lookDecider); 
                }
                //without rings
                if(lookDecider == 1){
                    Debug.Log("FLAT");
                    mat.SetInt("_Rings", lookDecider); 
                    float randStrength = UnityEngine.Random.Range(1.0f, 4.0f); 
                    mat.SetFloat("_Version2Strength", randStrength); 
                    float randAngleOffset = UnityEngine.Random.Range(1.0f, 10.0f); 
                    mat.SetFloat("_Version2AngleOffset", randAngleOffset); 
                    float randHue = UnityEngine.Random.Range(241.0f, 300.0f); 
                    Color randColor = Color.HSVToRGB((randHue/360.0f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)); 
                    mat.SetColor("_Version2Color", randColor); 
                }
                planetRenderer.material = mat;
                p.type = 2;                  
            }
            Instantiate(g);

            if(p.mass > (planetMassScalar * 0.2447e-9)){
                GenerateMoon(p.mass, p.position, planetNr, p.scale, moonCounter, g.transform.position, g.transform.localScale); 
                moonCounter++; 
            }

            planetNr++; 
        }
    }

    void GenerateMoon(double planetMass, double3 planetPosition, int planetID, double3 planetScale, int i, Vector3 dispPlanPos, Vector3 dispPlanScale){
        Moon m = new Moon(); 
        m.mass = planetMass * (double)UnityEngine.Random.Range(0.008f, 0.027f);
        //Debug.Log(m.mass + " Moon Mass"); 
        m.planetID = planetID;  
        m.position = new double3(planetPosition.x, planetPosition.y, planetPosition.z + ((planetScale.z/2.0) * 20.0)); 
        m.identifier = 2; 
        m.uniqueMoonID = i; 
        double moonRadScalar = 696340.0; //Solar radius used as constant. 
        m.radius = moonRadScalar * 0.0024; 
        m.CalculateProperties(); 

        moons2.Add(m); 

        GameObject gm = moon; 
        Rigidbody rbM; 

        rbM = gm.GetComponent<Rigidbody>(); 
        double massDownscale = S5 * m.mass; 
        rbM.mass = (float)massDownscale; 

        double3 posDownscale = S * m.position; 
        Vector3 posConversion;
        posConversion.x = (float)posDownscale.x;
        posConversion.y = (float)posDownscale.y;
        posConversion.z = (float)posDownscale.z;
        gm.transform.position = (posConversion); 
        //gm.transform.position = new Vector3(dispPlanPos.x, dispPlanPos.y, dispPlanPos.z - (dispPlanScale.z * 3.5f));
        
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

    public void GenerateAsteroid(){
        Debug.Log("NEW ASTEROID"); 
        Asteroid a = new Asteroid(); 
        a.mass = 2e+30 * (double)UnityEngine.Random.Range(0.0000000013208f, 0.0009543f);
        Debug.Log(a.mass + " ASTEROID MASS"); 
        a.identifier = 3; 
        double radScalar = 696340.0;
        float astRadRand = UnityEngine.Random.Range(0.0024f, 0.10049f);
        a.radius = radScalar * (double)astRadRand;
        Debug.Log(a.radius + " ASTEROID RADIUS");  
        a.CalculateProperties(); 
        double astPosScalar = (starData.radius * 200.0); 
        a.position = new double3(astPosScalar, 0, astPosScalar * -0.5); 
        a.asteroidID = celestialBodiesPhysics.Count + 1; 

        //Initial Velocity
        double3 direction = new double3(-1.0, 0.0, 1.0);
        double3 velocity = new double3();
        double r = math.distance(celestialBodiesPhysics[0].position, a.position) * 1000; 
        velocity += direction * System.Math.Sqrt((G * celestialBodiesPhysics[0].mass) / r); 
        a.velocity = velocity;
        Debug.Log(a.velocity + " ASTEROID VELODITY"); 

        //Initial rotational velocity
        double3 rotForce = new double3(); 
        double3 linearVelocity = new double3(); 
        double3 angularVelocity= new double3(); 
        double3 tangentialVelocity= new double3(); 
        double m = a.mass; 
        double r2 = a.radius * 1000.0; //Coversion to meters
        double conversionToSeconds = 86400.0; 
        float obliquity = UnityEngine.Random.Range(0.03f, 82.23f); //In degrees
        double3 rotationalAxis = math.normalize(new double3(0.0, -1.0 * (double)Mathf.Sin(obliquity), -1.0 * (double)Mathf.Cos(obliquity))); 
        a.rotationalAxis = rotationalAxis; 
        Debug.Log(a.rotationalAxis + " ASTEROID ROTATIONAL AXIS"); 
        
        tangentialVelocity = rotationalAxis * ((2.0 * 3.14159265359 * a.radius)/((double)UnityEngine.Random.Range(6.0f, 58.0f)) * conversionToSeconds); 
        linearVelocity = tangentialVelocity/r2; 
        double x = a.position.x + r2; 
        double y = a.position.y + r2; 
        double z = a.position.z + r2; 
        double absoluteRadius = System.Math.Sqrt((x * x) + (y * y) + (z * z)); 
        angularVelocity = (r2 * linearVelocity)/(absoluteRadius); 
        
        rotForce = (m * (angularVelocity * angularVelocity)) / r2; 
        a.rotationalForce += rotForce;
        Debug.Log(a.rotationalForce + " ASTEROID ROTATIONAL FORCE"); 

        celestialBodiesPhysics.Add(a);
        asteroids.Add(a);

        //--------------------------------------------------------------------------------------------------------------------------------------------------
        //Game Object
        GameObject ga = asteroidTemplate; 
        Rigidbody rbA = ga.GetComponent<Rigidbody>();
        double massDownscale = S5 * a.mass;
        rbA.mass = (float)massDownscale; 

        double3 posDownscale = S * a.position; 
        Vector3 posConversion;
        posConversion.x = (float)posDownscale.x;
        posConversion.y = (float)posDownscale.y;
        posConversion.z = (float)posDownscale.z;
        ga.transform.position = posConversion; 
        //gm.transform.position = new Vector3(dispPlanPos.x, dispPlanPos.y, dispPlanPos.z - (dispPlanScale.z * 3.5f));
        
        double3 scaleDownscale = S * a.scale; 
        Vector3 scaleConversion;
        scaleConversion.x = (float)scaleDownscale.x;
        scaleConversion.y = (float)scaleDownscale.y;
        scaleConversion.z = (float)scaleDownscale.z;
        ga.transform.localScale = scaleConversion; 

        //Set up material
        var moonRenderer = ga.GetComponent<Renderer>(); 
        Material astMat = new Material(pgm); 
        astMat.SetFloat("_Roughness", UnityEngine.Random.Range(3.0f, 5.0f)); 
        astMat.SetColor("_Color", Color.grey); 
        moonRenderer.material = astMat; 

        //RB velocity
        double3 tempVel = velocity * S; 
        Vector3 rbVel; 
        rbVel.x = (float)tempVel.x; 
        rbVel.y = (float)tempVel.y; 
        rbVel.z = (float)tempVel.z;  
        ga.GetComponent<Rigidbody>().velocity = rbVel; 

        Instantiate(ga);
        Debug.Log(ga.GetComponent<Rigidbody>().velocity + " INITIAL RB VELOCITY"); 
        asteroidsGO.Add(ga); 
        Debug.Log(asteroidsGO[0].GetComponent<Rigidbody>().velocity + " INITIAL RB VELOCITY");
    } 
}

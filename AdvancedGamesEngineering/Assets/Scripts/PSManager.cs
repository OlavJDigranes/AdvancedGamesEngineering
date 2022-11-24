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
        celestialBodies = GameObject.FindGameObjectsWithTag("CelestialBody"); 
        moons = GameObject.FindGameObjectsWithTag("Moon"); 
        
        celestialBodiesPhysics.Add(starData);

        for(int i = 0; i < celestialBodies.Length - 1; i++){
            celestialBodiesPhysics.Add(planets[i]); 
        }
        for(int j = 0; j < moons.Length; j++){
            celestialBodiesPhysics.Add(moons2[j]); 
        }

        for(int i = 0; i < celestialBodiesPhysics.Count; i++){
            Debug.Log(celestialBodiesPhysics[i].identifier); 
        }
        
        InitialOrbitVelocity();
        //InitialRotationalVelocity(); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ClearForces();
        OverallGravitationalPull(); 
        //Rotation(); 
        Integration();   
        
    }

    void Update(){
        Display(); 
        MoonDisplay();
    }

    void ClearForces(){
        double3 clear = new double3(0.0, 0.0, 0.0);
        for(int i = 0; i < celestialBodiesPhysics.Count; i++){
            celestialBodiesPhysics[i].accumulatedForce = clear; 
        }
    }

    void OverallGravitationalPull(){
        //Calculate gravitational pull using Newton's law of universil gravitation:
        //  F = G * ((m1 * m2)/r^2)

        for(int i = 1; i < celestialBodiesPhysics.Count; i++) {
            double3 force = new double3(); 
            double3 force2 = new double3(); 
            //for(int j = 0; j < i+1; j++) {
            for(int j = 1; j < celestialBodiesPhysics.Count; j++) {
                if(!celestialBodiesPhysics[i].Equals(celestialBodiesPhysics[j])){
                    double m1 = celestialBodiesPhysics[i].mass;
                    double m2 = celestialBodiesPhysics[j].mass;
                    
                    //double x =  celestialBodiesPhysics[i].position.x - celestialBodiesPhysics[j].position.x;
                    //double y =  celestialBodiesPhysics[i].position.y - celestialBodiesPhysics[j].position.y;
                    //double z =  celestialBodiesPhysics[i].position.z - celestialBodiesPhysics[j].position.z;
                    //double r = System.Math.Sqrt((x * x) + (y * y) + (z * z)) * 1000.0;
                    double r = math.distance(celestialBodiesPhysics[i].position, celestialBodiesPhysics[j].position) * 1000.0; 
                    //Debug.Log(r + " Planet R");
                    
                    double3 dir = ((((celestialBodiesPhysics[i].position - celestialBodiesPhysics[j].position)) * 1000.0)/r); 

                    //force += dir * ((G * (m1 * m2)/(r * r)) + ((G * starData.mass)/(r * r)));
                    force += dir * ((G * (m1 * m2)/(r * r)));
                    //force += dir * ((G * starData.mass)/(r * r)); 

                    celestialBodiesPhysics[i].accumulatedForce += force; 
                    celestialBodiesPhysics[j].accumulatedForce -= force; 
                }
            }

            if(i != 0){
                //double x =  celestialBodiesPhysics[0].position.x - celestialBodiesPhysics[i].position.x;
                //double y =  celestialBodiesPhysics[0].position.y - celestialBodiesPhysics[i].position.y;
                //double z =  celestialBodiesPhysics[0].position.z - celestialBodiesPhysics[i].position.z;
                //double r2 = System.Math.Sqrt((x * x) + (y * y) + (z * z)) * 1000.0;
                double r2 = math.distance(celestialBodiesPhysics[0].position, celestialBodiesPhysics[i].position) * 1000.0; 
                double3 dir2 = ((((celestialBodiesPhysics[0].position - celestialBodiesPhysics[i].position)) * 1000.0)/r2);
                force2 += dir2 * (G * (starData.mass * celestialBodiesPhysics[i].mass)/(r2* r2));
                //force2 += dir2 * (G * (starData.mass)/(r2));
                celestialBodiesPhysics[i].accumulatedForce += force2; 
            }

            Debug.Log(force + " TOTAL FORCE FOR CB " + i); 
        }
    }

    void Rotation(){
        double3 rot; 
        double3 rotDownscale; 
        Vector3 tempRot; 

        rot = celestialBodiesPhysics[0].rotationalForce * Time.deltaTime; 
        rotDownscale = S * rot; 
        Debug.Log(rotDownscale + " ROT DOWNSCALE"); 
        tempRot.x = (float)rotDownscale.x; 
        tempRot.y = (float)rotDownscale.y; 
        tempRot.z = (float)rotDownscale.z; 
        star.transform.Rotate(tempRot); 

        for(int j = 0; j < planets.Length; j++){
            rot = planets[j].rotationalForce * Time.deltaTime; 
            rotDownscale = S * rot; 
            Debug.Log(rotDownscale + " ROT DOWNSCALE"); 
            tempRot.x = (float)rotDownscale.x; 
            tempRot.y = (float)rotDownscale.y; 
            tempRot.z = (float)rotDownscale.z; 
            celestialBodies[planets[j].uniquePlanetID].transform.Rotate(tempRot); 
        }

        for(int k = 0; k < moons2.Count; k++){
            rot = moons2[k].rotationalForce * Time.deltaTime; 
            rotDownscale = S * rot; 
            Debug.Log(rotDownscale + " ROT DOWNSCALE"); 
            tempRot.x = (float)rotDownscale.x; 
            tempRot.y = (float)rotDownscale.y; 
            tempRot.z = (float)rotDownscale.z; 
            celestialBodies[moons2[k].uniqueMoonID].transform.Rotate(tempRot);
        }
    }

    //Symplectic euler integration
    void Integration(){
        double3 vel;
        double3 pos; 

        for(int i = 0; i < celestialBodiesPhysics.Count; i++){
            //Debug.Log(celestialBodiesPhysics[i].accumulatedForce + " CB Accumulated Force for CB " + i); 
            double3 accel = celestialBodiesPhysics[i].accumulatedForce / celestialBodiesPhysics[i].mass; 
            vel = celestialBodiesPhysics[i].velocity + (accel * (Time.deltaTime));
            pos = celestialBodiesPhysics[i].position + (vel * (Time.deltaTime)); 
            celestialBodiesPhysics[i].velocity = vel; 
            celestialBodiesPhysics[i].position = pos; 

            /*
            if(celestialBodiesPhysics[i].identifier == 0){
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
            */
        }

        /*
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
        */
    }
    
    void Display(){
        float m1 = celestialBodies[0].GetComponent<Rigidbody>().mass; 
        for (int i = 1; i < celestialBodies.Length; i++){
            float m2 = celestialBodies[i].GetComponent<Rigidbody>().mass; 
            float r = Vector3.Distance(celestialBodies[0].transform.position, celestialBodies[i].transform.position); 
            celestialBodies[i].GetComponent<Rigidbody>().AddForce(((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * ((G2 * S2) * (m1 * m2) / (r * r))));
            celestialBodies[0].GetComponent<Rigidbody>().AddForce(-((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * ((G2 * S2) * (m1 * m2) / (r * r))));
            Debug.Log((celestialBodies[0].transform.position - celestialBodies[i].transform.position).normalized * ((G2 * S2) * (m1 * m2) / (r * r)));  
        }
        

        /*
        double3 forceDownscaleS = S2 * celestialBodiesPhysics[0].accumulatedForce; 
        Vector3 forceVectorS; 
        forceVectorS.x = (float)forceDownscaleS.x;
        forceVectorS.y = (float)forceDownscaleS.y;
        forceVectorS.z = (float)forceDownscaleS.z;
        celestialBodies[0].GetComponent<Rigidbody>().AddForce(forceVectorS); 

        for(int i = 0; i < planets.Length; i++){
            double3 forceDownscaleP = S2 * planets[i].accumulatedForce; 
            Vector3 forceVectorP; 
            forceVectorP.x = (float)forceDownscaleP.x;
            forceVectorP.y = (float)forceDownscaleP.y;
            forceVectorP.z = (float)forceDownscaleP.z;
            celestialBodies[planets[i].uniquePlanetID].GetComponent<Rigidbody>().AddForce(forceVectorP); 
        }

        for(int i = 0; i < moons2.Count; i++){
            double3 forceDownscaleM = S2 * moons2[i].accumulatedForce; 
            Vector3 forceVectorM; 
            forceVectorM.x = (float)forceDownscaleM.x;
            forceVectorM.y = (float)forceDownscaleM.y;
            forceVectorM.z = (float)forceDownscaleM.z;
            celestialBodies[moons2[i].uniqueMoonID].GetComponent<Rigidbody>().AddForce(forceVectorM); 
        }
        */
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
    }

    void InitialOrbitVelocity(){
        //Calculate initial orbital velocity using circular orbit instant velocity
        double3 directionX = new double3(1.0, 0.0, 0.0);
        double3 directionZ = new double3(0.0, 0.0, 1.0);
        double3 direction = new double3(-1.0, 0.0, 1.0);

        //double m = starData.mass; 
        
        /*
        for(int i = 0; i < celestialBodiesPhysics.Count; i++) {
            double3 velocity = new double3(); 
            for(int j = 0; j < celestialBodiesPhysics.Count; j++) {
                if(!celestialBodiesPhysics[i].Equals(celestialBodiesPhysics[j])){
                    double m = celestialBodiesPhysics[j].mass; 
                    
                    double x = celestialBodiesPhysics[i].position.x - celestialBodiesPhysics[j].position.x;
                    double y = celestialBodiesPhysics[i].position.y - celestialBodiesPhysics[j].position.y;
                    double z = celestialBodiesPhysics[i].position.z - celestialBodiesPhysics[j].position.z;
                    double r = System.Math.Sqrt((x * x) + (y * y) + (z * z)) * 1000.0;
                    
                    Debug.Log(r/1000.0 + " PLANET R KM BETWEEN CB " + i + " AND " + j); 
                    Debug.Log(System.Math.Sqrt((G * m) / r) + " PLANET SQRT");
                    
                    if(celestialBodiesPhysics[i].identifier == 1){
                        velocity += directionZ * System.Math.Sqrt((G * m) / r); 
                        celestialBodiesPhysics[i].velocity = velocity;
                        Debug.Log(celestialBodiesPhysics[i].velocity + " INIT VEL"); 
                    }
                    if(celestialBodiesPhysics[i].identifier == 2){
                        velocity += directionX * System.Math.Sqrt((G * m) / r); 
                        celestialBodiesPhysics[i].velocity = velocity;
                        Debug.Log(celestialBodiesPhysics[i].velocity + " INIT VEL"); 
                    }

                     
                }
            }
        */

        
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
                //(celestialBodiesPhysics[0].position - celestialBodiesPhysics[i].position).normalise;  
                //double x = celestialBodiesPhysics[0].position.x - celestialBodiesPhysics[i].position.x;
                //double y = celestialBodiesPhysics[0].position.y - celestialBodiesPhysics[i].position.y;
                //double z = celestialBodiesPhysics[0].position.z - celestialBodiesPhysics[i].position.z;
                //double r = System.Math.Sqrt((x * x) + (y * y) + (z * z)) * 1000.0;
                double r = math.distance(celestialBodiesPhysics[0].position, celestialBodiesPhysics[i].position) * 1000; 
                    
                Debug.Log(r/1000.0 + " PLANET R KM BETWEEN CB " + i + " AND STAR"); 
                Debug.Log(System.Math.Sqrt((G * celestialBodiesPhysics[0].mass) / r) + " PLANET SQRT");

                velocity += directionZ * System.Math.Sqrt((G * celestialBodiesPhysics[0].mass) / r); 
                celestialBodiesPhysics[i].velocity = velocity;
                Debug.Log(celestialBodiesPhysics[i].velocity + " INIT VEL"); 
            }
        }

        for(int i = 0; i < moons2.Count; i++){
            double3 velocity = new double3();
            //double m2 = planets[moons2[i].planetID - 1].mass; 
            double m2 = moons2[i].mass; 
            //double x = planets[moons2[i].planetID - 1].position.x - moons2[i].position.x;
            //double y = planets[moons2[i].planetID - 1].position.y - moons2[i].position.y;
            //double z = planets[moons2[i].planetID - 1].position.z - moons2[i].position.z;
            //double r = System.Math.Sqrt((x * x) + (y * y) + (z * z));
            double r = math.distance(planets[moons2[i].planetID - 1].position, moons2[i].position);

            Debug.Log(r/1000.0 + " PLANET R KM BETWEEN MOON " + i + " AND PLANET"); 
            Debug.Log(System.Math.Sqrt((G * m2) / r) + " PLANET SQRT");

            velocity += directionX * System.Math.Sqrt((G * m2) / r); 
            moons2[i].velocity = velocity;
            Debug.Log(moons2[i].velocity + " INIT VEL");
        }    

        for(int j = 0; j < planets.Length; j++){
            double3 tempVelDownscale = S * planets[j].velocity;
            Debug.Log(tempVelDownscale + "INITIAL VELOCITY DOWNSCALE"); 

            Vector3 tempVel;
            tempVel.x = (float)tempVelDownscale.x;  
            tempVel.y = (float)tempVelDownscale.y;  
            tempVel.z = (float)tempVelDownscale.z;  
            Debug.Log(tempVel + "INITVEL"); 

            celestialBodies[planets[j].uniquePlanetID].GetComponent<Rigidbody>().velocity += tempVel; 
        }

        for(int k = 0; k < moons2.Count; k++){
            double3 tempVelDownscale = S * moons2[k].velocity; 
            Debug.Log(tempVelDownscale + "INITIAL VELOCITY DOWNSCALE"); 

            Vector3 tempVel;
            tempVel.x = (float)tempVelDownscale.x;  
            tempVel.y = (float)tempVelDownscale.y;  
            tempVel.z = (float)tempVelDownscale.z;  
            Debug.Log(tempVel + "INITVEL"); 

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
            double3 rotationalAxis = new double3(0.0, -1.0 * (double)Mathf.Sin(obliquity), -1.0 * (double)Mathf.Cos(obliquity)); 
            
            //angularVelocity = rotationalAxis * System.Math.Sqrt((G * m)/r); 
            if(celestialBodiesPhysics[i].identifier == 0 || celestialBodiesPhysics[i].identifier == 2){
                tangentialVelocity = rotationalAxis * ((2.0 * 3.1415 * celestialBodiesPhysics[i].radius)/(27.0 * conversionToSeconds)); 
            }
            if(celestialBodiesPhysics[i].identifier == 1){
                tangentialVelocity = rotationalAxis * ((2.0 * 3.1415 * celestialBodiesPhysics[i].radius)/((double)UnityEngine.Random.Range(6.0f, 58.0f)) * conversionToSeconds); 
            }
            Debug.Log(tangentialVelocity + " TANG VEL"); 

            linearVelocity = tangentialVelocity/r; 
            Debug.Log(linearVelocity + " LIN VEL"); 

            double x = celestialBodiesPhysics[i].position.x + r; 
            double y = celestialBodiesPhysics[i].position.y + r; 
            double z = celestialBodiesPhysics[i].position.z + r; 
            double absoluteRadius = System.Math.Sqrt((x * x) + (y * y) + (z * z)); 
            angularVelocity = (r * linearVelocity)/(absoluteRadius); 
            Debug.Log(angularVelocity + " ANG VEL"); 
            
            force = (m * (angularVelocity * angularVelocity)) / r; 

            celestialBodiesPhysics[i].rotationalForce += force;
            Debug.Log(celestialBodiesPhysics[i].rotationalForce + " ROT FORCE");  
        }
 
        for(int i = 0; i < celestialBodies.Length; i++){
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
                Debug.Log(p.mass + " Planet Mass Proper"); 
        
                double planetRadScalar = 696340.0; //Solar radius used as constant. 
                float planetRadRand = UnityEngine.Random.Range(0.0035514f, 0.10049f); //Random radius based on solar system
                p.radius = (double)planetRadRand * planetRadScalar; 

                double planetPosScalar = (starData.radius * (i + 1.0) * 100.0); 
                //double planetPosScalar = (starData.radius + p.radius); 

                p.position = new double3(planetPosScalar, 0, 0);
                //p.position = new double3(planetPosScalar, 0, planetPosScalar * -0.5);
                Debug.Log(p.position + " Planet Pos Proper"); 

                p.identifier = 1; 
                p.uniquePlanetID = i+1; 
            
                p.CalculateProperties(); 
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
            //g.transform.position = posConversion; 
            g.transform.position = new Vector3((star.transform.localScale.x * ((planetNr-1) + 2.0f)) * 2.0f, 0, 0);

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
        Debug.Log(m.mass + " Moon Mass"); 
        m.planetID = planetID;  
        m.position = new double3(planetPosition.x, planetPosition.y, planetPosition.z + ((planetScale.z/2.0) * 10.0)); 
        m.identifier = 2; 
        m.uniqueMoonID = i; 
        double moonRadScalar = 696340.0; //Solar radius used as constant. 
        m.radius = moonRadScalar * 0.0024; 
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
        //gm.transform.position = posConversion; 
        gm.transform.position = new Vector3(dispPlanPos.x, dispPlanPos.y, dispPlanPos.z - (dispPlanScale.z * 3.5f));
        
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

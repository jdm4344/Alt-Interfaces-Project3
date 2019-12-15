using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Jordan Machalek
 */
public abstract class Vehicle : MonoBehaviour
{
    //Attributes
    public Vector3 vehiclePosition;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 direction;
    public Vector3 ultimateForce;
    //"Physical" properties
    public float mass;
    public float maxSpeed;
    public float maxForce;
    //Local Coordinates
    public Vector3 forward;
    public Vector3 right;
    //Obstacle Avoidance
    public float safeDistance; // for objects to be "close" to vehicle
    public float radius; // of vehicle's collision circle and future position circle
    public List<GameObject> obstacles;
    public Vector3 obstaclePosition; //For closest obstacle
    public float obstacleRadius;
    public float avoidanceWeight;
    public float wanderWeight;
    //Collision Bounds
    private float combinedRadii;
    //Separation
    public List<GameObject> companions;
    //Stay in bounds
    public float boundsWeight;
    //Wandering
    public float wanderAngle; //angle for generating wander vector
    public float angleAdjust;
    public Vector3 wanderForce;
    //Terrain Information
    Terrain terrain;
    //Flocking
    public float separationWeight;
    public float alignWeight;
    public float cohesionWeight;
    //Air/Fluid Resistance
    public GameObject[] fluids;
    public float dragWeight;
    //SceneManager
    public GameObject sceneManager;
    //Off-screen Detection
    private Camera cam;
    private float height;
    private float width;

    // Use this for initialization
    public virtual void Start()
    {
        sceneManager = GameObject.Find("SceneManager");
        //Get the initial position
        vehiclePosition = transform.position;

        // Get the obstacles to avoid
        //obstacles = sceneManager.GetComponent<SceneManager>().obstacleList;

        //Determine radius for collision checks
        combinedRadii = gameObject.GetComponent<BoxCollider>().bounds.extents.x * 2;

        //Get terrain data for height adjustment
        terrain = Terrain.activeTerrain;

        //Get array of fluids to be interacted with
        //fluids = sceneManager.GetComponent<SceneManager>().fluidArray;

        //Setup camera variables
        cam = Camera.main;
        height = 2f * cam.orthographicSize;
        width = height * cam.aspect;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Grab the position and rotation
        vehiclePosition = transform.position;
        //Determine Acceleration
        CalcSteeringForces();
        //Move
        UpdatePosition();
        //Update right and forward vectors
        right = transform.right;
        forward = transform.forward;
        //This is done here rather than 
    }

    //Moves the vehicle based upon acceleration
    void UpdatePosition()
    {
        //"New" movement formula - multiply by Time.deltaTime to move on a per-second basis rather than per-frame
        // - now framerate independent
        velocity += (acceleration * Time.deltaTime);
        //Scale velocity to max speed
        velocity.Normalize();
        velocity = velocity * maxSpeed;
        //Update Position
        vehiclePosition += (velocity * Time.deltaTime);
        //Grab current direction from velocity - new
        direction = velocity.normalized;
        //Adjust position for terrain height
        transform.position = new Vector3(vehiclePosition.x, terrain.SampleHeight(new Vector3(vehiclePosition.x, 0, vehiclePosition.z)) + 1, vehiclePosition.z);
        //Update Rotation
        Rotate();
        //Zero out acceleration - new
        acceleration = Vector3.zero;
    }

    //Helper Method - Updates rotation of vehicle towards current velocity
    void Rotate()
    {
        Vector3 dir = direction;

        transform.rotation = Quaternion.Euler(0, (Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg), 0);
    }

    //Applies a given force to an object
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    //Causes a vehicle to move at a maximum speed towards a target
    //Assumes the target's location is known
    public Vector3 Seek(Vector3 targetPosition)
    {
        //Find desired velocity (vector from vehicle to target)
        Vector3 desiredVelocity = targetPosition - vehiclePosition;

        //Scale desired velocity to max speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        return desiredVelocity - velocity;
    }

    //Causes a vehicle to move at a max speed away from a target
    public Vector3 Flee(Vector3 targetPosition)
    {
        //Desired velocity is away from terget - inverse of seek vector
        Vector3 desiredVelocity = vehiclePosition - targetPosition;

        //Scale to max speed
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;

        return desiredVelocity - velocity;
    }

    // Generates a force to keep a vehicle within the bounds of the game world
    public Vector3 StayInBounds()
    {
        Vector3 futurePos = vehiclePosition + (forward * safeDistance);

        //If future position is OOB, return vector towards center of world
        if (futurePos.x > 190 || futurePos.x < 10 || futurePos.z > 190 || futurePos.z < 10)
        {
            return Seek(new Vector3(100, 0, 100));
        }

        return Vector3.zero;
    }

    // Generates a force to cause a vehicle to wander in random directions
    public Vector3 Wander()
    {
        if (wanderAngle <= -360)
        {
            wanderAngle = 0;
        }

        //Set positioning circle at velocity vector's end
        Vector3 dirCircle = vehiclePosition + (velocity.normalized * safeDistance);

        //Get displacement vector
        Vector3 moveLoc = GetWanderAngle(new Vector3(1, 0, 0), wanderAngle);

        wanderAngle += (Random.Range(0, 1) * angleAdjust) - (angleAdjust * 0.5f);
        wanderForce = Seek(dirCircle + moveLoc);
        Debug.DrawLine(vehiclePosition, vehiclePosition + wanderForce, Color.white);
        return new Vector3(wanderForce.x, 0, wanderForce.z);
    }

    //Helper Method - Gets an angle to adjust the next position to wander towards
    Vector3 GetWanderAngle(Vector3 position, float angle)
    {
        //float mag = position.magnitude;
        position.x = Mathf.Cos(angle) * 1.5f;
        position.z = Mathf.Sin(angle) * 1.5f;

        return position;
    }

    //Calculates a steering vector based on obstacles in a vehicle's path
    public Vector3 AvoidObstacle(List<GameObject> objList)
    {
        List<GameObject> currentObstacles = new List<GameObject>();

        //Check if the obstacle is close to the vehicle and if it is in front of it
        foreach (GameObject obj in objList)
        {
            if (LocationCheck(obj.transform.position, obj.transform.forward.sqrMagnitude))
            {
                currentObstacles.Add(obj);
            }
        }

        //Check whether each obstacle is to the left or right
        foreach (GameObject obj in currentObstacles)
        {
            Vector3 distToObstacle = obj.transform.position - transform.position;
            float dot = DotProduct(right, distToObstacle);

            if (dot >= 0) //Obstacle is to the right or directly in front, turn left
            {
                return (-1 * right) * maxSpeed;
            }
            else if (dot < 0) //Obstacle is to the left, turn right
            {
                return right * maxSpeed;
            }
        }

        //No potential collision, return zero vector
        return Vector3.zero;
    }

    //Helper method - Returns the dot product of two vectors based on their X and Z values
    private float DotProduct(Vector3 a, Vector3 b)
    {
        return (a.x * b.x) + (a.z * b.z);
    }

    //Determines if an obstacle is close and in front of the vehicle
    private bool LocationCheck(Vector3 obsPosition, float obsRadius)
    {
        Vector3 futureLocation = forward * safeDistance; //also functions as minimum safe distance radius
        //Vector3 futureRadius = futureLocation * radius;
        Vector3 distToObstacle = obsPosition - transform.position;

        //Check if distance to obstacle < futureLocation; if so, is potential obstacle
        if (distToObstacle.sqrMagnitude < futureLocation.sqrMagnitude)
        {
            //Check if dot product is less than the sum of radii of the vehicle and obstacle; if so, is potential obstacle
            if (DotProduct(distToObstacle, right) < obsRadius + radius)
            {
                //Check if the obstacle is in front; if so, return true
                if (DotProduct(distToObstacle, forward) > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //Helper method - Checks distance between objects to determine collisions 
    public bool CrcCheck(GameObject obj)
    {
        //Get difference in X and Y positions for centers of the gameobject and another object
        float xDiff = gameObject.GetComponent<BoxCollider>().bounds.center.x - obj.GetComponent<BoxCollider>().bounds.center.x;
        float zDiff = gameObject.GetComponent<BoxCollider>().bounds.center.z - obj.GetComponent<BoxCollider>().bounds.center.z;

        //Square both differences
        xDiff = xDiff * xDiff;
        zDiff = zDiff * zDiff;

        //Combine differences and take root
        float centerDist = xDiff + zDiff;

        centerDist = Mathf.Sqrt(centerDist);

        //Check if 
        if (centerDist < combinedRadii)
        {
            return true;
        }

        return false;
    }

    //Returns a drag force to be applied if a vehicle is travelling through an area of resistance
    public Vector3 Drag(GameObject liquid)
    {
        if (CrcCheck(liquid) == true)
        {
            float speed = velocity.magnitude;

            //Negate and scale drag force
            Vector3 dragForce = velocity;
            dragForce.Normalize();

            dragForce *= (-0.9f * speed * speed);

            return dragForce;
        }

        return Vector3.zero;
    }

    //Keeps vehicle at a distance from its companions
    public Vector3 Separation()
    {
        //Stay twice as far away as the radius
        float wantedDistance = radius * 2f;

        Vector3 sum = Vector3.zero;

        //Check position against all other flockers
        foreach (GameObject comp in companions)
        {
            float distance = Vector3.Distance(vehiclePosition, comp.GetComponent<Vehicle>().vehiclePosition);

            //Rule out the calculating flocker
            if (comp != gameObject && distance < wantedDistance)
            {
                sum += vehiclePosition - comp.GetComponent<Vehicle>().vehiclePosition;
            }
        }

        return sum;
    }

    //Aligns vehicle with its flock
    public Vector3 Alignment()
    {
        Vector3 average = Vector3.zero;

        //Add the direction of each vehicle
        foreach (GameObject comp in companions)
        {
            //Rule out the calculating flocker
            if (comp == gameObject)
            {
                continue;
            }

            average += comp.GetComponent<Vehicle>().direction;
        }

        average = average / companions.Count;

        return average - velocity;
    }

    //Draws a vehicle towards the center of its flock
    public Vector3 Cohesion()
    {
        Vector3 average = Vector3.zero;

        //Add the position of each vehicle
        foreach (GameObject comp in companions)
        {
            //Rule out the calculating flocker
            if (comp == gameObject)
            {
                continue;
            }

            average += comp.GetComponent<Vehicle>().vehiclePosition;
        }

        average = average / companions.Count;

        return Seek(average - velocity);
    }

    public void Wrap()
    {
        //Check X position
        if (transform.position.x > width / 2)
        {
            vehiclePosition.x -= width;
        }
        else if (transform.position.x < 0 - (width / 2))
        {
            vehiclePosition.x += width;
        }

        //Check Y position
        if (transform.position.y > height / 2)
        {
            vehiclePosition.y -= height;
        }
        else if (transform.position.y < 0 - (height / 2))
        {
            vehiclePosition.y += height;
        }
    }

    //Determines sum of movement forces for a vehicle
    public abstract void CalcSteeringForces();
}

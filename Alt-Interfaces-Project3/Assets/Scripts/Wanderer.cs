using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Jordan Machalek
 */
public class Wanderer : Vehicle
{
    // Use this for initialization
    public override void Start()
    {
        base.Start();

        maxSpeed = Random.Range(1, 2);
        maxForce = Random.Range(100, 500);
        angleAdjust += Random.Range(0f, 1f);

        //companions = sceneManager.GetComponent<SceneManager>().flockList;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    //Generates movement force for the turkey
    public override void CalcSteeringForces()
    {
        //Reset force
        ultimateForce = Vector3.zero;

        //Avoid companions
        ultimateForce += (Separation() * separationWeight);

        //Wander
        ultimateForce += (Wander() * wanderWeight);

        //Avoid obstacles
        ultimateForce += (AvoidObstacle(obstacles) * avoidanceWeight);

        //Limit force
        ultimateForce.Normalize();
        ultimateForce = ultimateForce * maxForce;

        //Apply force to acceleration
        ApplyForce(ultimateForce);

        // Wrap screen edges
        Wrap();
    }
}

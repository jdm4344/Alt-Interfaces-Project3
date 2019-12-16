using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bCollisions : MonoBehaviour
{
    //Attributes
    [Header("Managers")]
    public ProjectileManager projectileManager;
    public TargetManager targetManager;

    public List<GameObject> obstacles;
    private Vector3 max;
    private Vector3 min;

    // Use this for initialization
    void Start()
    {
        //Get list of current obstacles by retrieving asteroid list from Asteroid Manager script
        //obstacles = GameObject.Find("SceneManager").GetComponent<AsteroidManager>().asteroidList;

        if (!projectileManager) projectileManager = GameObject.Find("GameManager").GetComponent<ProjectileManager>();
        if (!targetManager) targetManager = GameObject.Find("GameManager").GetComponent<TargetManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check each obstacle that currently exists
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (ColCheck(obstacles[i]))
            {
                //Check for civilian or hostile
                if (obstacles[i].name.Equals("Civilian(Clone)"))
                {
                    targetManager.civilians.Remove(obstacles[i]);
                    targetManager.civilianDeaths++;
                    //GameObject.Find("SceneManager").GetComponent<ScoreManager>().score += 20;
                }
                else if (obstacles[i].name.Equals("Hostile(Clone)"))
                {
                    targetManager.hostiles.Remove(obstacles[i]);
                    targetManager.hostileDeaths++;
                    //GameObject.Find("SceneManager").GetComponent<ScoreManager>().score += 50;
                }
                
                // Destroy target
                Destroy(obstacles[i]);
                
                obstacles.RemoveAt(i); // DO THIS LAST - Remove from this script's list to stop errors

                //Destroy bullet
                Destroy(gameObject);

                //Decrement bullet total
                projectileManager.SubBullets();
            }
        }
    }

    //Check for collision
    bool ColCheck(GameObject obs)
    {
        //Get max and min vectors from object
        max = gameObject.GetComponent<SpriteRenderer>().bounds.max;
        min = gameObject.GetComponent<SpriteRenderer>().bounds.min;

        Vector3 obsMax = obs.GetComponent<SpriteRenderer>().bounds.max;
        Vector3 obsMin = obs.GetComponent<SpriteRenderer>().bounds.min;

        //If intersecting return true
        if (obsMin.x + 0.5f < max.x && obsMax.x - 0.5f > min.x && obsMax.y - 0.5f > min.y && obsMin.y + 0.5f < max.y) // adjust obs min and max for sprite bounds (make collision check more precise)
        {
            return true;
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bCollisions : MonoBehaviour
{
    //Attributes
    [Header("Managers")]
    public ProjectileManager projectileManager;
    public List<GameObject> obstacles;
    private Vector3 max;
    private Vector3 min;

    // Use this for initialization
    void Start()
    {
        //Get list of current obstacles by retrieving asteroid list from Asteroid Manager script
        //obstacles = GameObject.Find("SceneManager").GetComponent<AsteroidManager>().asteroidList;

        if (!projectileManager) projectileManager = GameObject.Find("GameManager").GetComponent<ProjectileManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check each obstacle that currently exists
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (ColCheck(obstacles[i]))
            {
                //Determine points for destroying asteroid
                //Check for first or second level ship
                if (obstacles[i].name.Contains("eShip"))
                {
                    //GameObject.Find("SceneManager").GetComponent<ScoreManager>().score += 20;
                }
                else if (obstacles[i].name.Contains("dinghy"))
                {
                    //GameObject.Find("SceneManager").GetComponent<ScoreManager>().score += 50;
                }

                //GameObject.Find("SceneManager").GetComponent<AsteroidManager>().MakeNew(obstacles[i]);
                Destroy(obstacles[i]);
                //GameObject.Find("pShip").transform.Find("hullLarge (1)").GetComponent<pCollisions>().obstacles.RemoveAt(i); //remove from player obstacles
                obstacles.RemoveAt(i); // DO THIS LAST - Remove from this script's list

                //Destroy bullet, spawn second-level asteroids and destroy first-level asteroid
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

        //Check for first or second level ship
        if (obs.name.Contains("eShip"))
        {
            Vector3 obsMax = obs.transform.Find("hullLarge (1)").GetComponent<SpriteRenderer>().bounds.max;
            Vector3 obsMin = obs.transform.Find("hullLarge (1)").GetComponent<SpriteRenderer>().bounds.min;

            //If intersecting return true
            if (obsMin.x < max.x && obsMax.x > min.x && obsMax.y > min.y && obsMin.y < max.y)
            {
                return true;
            }
        }
        else if (obs.name.Contains("dinghy"))
        {
            Vector3 obsMax = obs.GetComponent<SpriteRenderer>().bounds.max;
            Vector3 obsMin = obs.GetComponent<SpriteRenderer>().bounds.min;

            //If intersecting return true
            if (obsMin.x < max.x && obsMax.x > min.x && obsMax.y > min.y && obsMin.y < max.y)
            {
                return true;
            }
        }

        return false;
    }
}

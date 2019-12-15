using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Jordan Machalek
 * Handles collisions between player ship and given obstacles using a Bounding Circle method
 * Implemented on hull for ship b/c ship prefab is made up of multiple objects and as such does
 * not have a single sprite renderer
 */
public class pCollisions : MonoBehaviour
{

    //Attributes
    public List<GameObject> obstacles;
    private Vector3 max;
    private Vector3 min;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Check each obstacle that currently exists
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (ColCheck(obstacles[i]))
            {
                Destroy(obstacles[i]);
                //GameObject.Find("SceneManager").GetComponent<AsteroidManager>().asteroidList.RemoveAt(i); //remove from overall list of asteroids
                obstacles.RemoveAt(i); // DO THIS LAST - Remove from this script's list

                //Remove health
                //GameObject.Find("SceneManager").GetComponent<HealthManager>().TakeDamage();
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

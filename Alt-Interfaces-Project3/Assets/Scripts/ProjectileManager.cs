using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Jordan Machalek
 * Class used to keep track of number of bullets that exist at any one time
 */
public class ProjectileManager : MonoBehaviour
{
    //Attributes
    public int maxBullets;
    public int numBullets;
    public bool canFire;

    // Use this for initialization
    void Start()
    {
        canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (numBullets < maxBullets)
        {
            canFire = true;
        }
    }

    //Increments numBullets, adjusts canFire to T if num < max or to F if num = max
    public void CheckBullets()
    {
        numBullets++;

        if (numBullets == maxBullets)
        {
            canFire = false;
        }
    }

    //Decrements numBullets
    public void SubBullets()
    {
        numBullets--;

        //Redundancy check + adujustment
        if (numBullets < 0)
        {
            numBullets = 0;
        }
    }
}

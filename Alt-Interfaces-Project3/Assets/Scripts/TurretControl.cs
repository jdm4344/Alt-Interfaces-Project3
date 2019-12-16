using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretControl : MonoBehaviour
{
    // Attributes
    [Header("Managers")]
    public ProjectileManager projectileManager;
    public TargetManager targetManager;
    [Header("Assets")]
    public GameObject projectile;
    [Header("Movement Data")]
    public Vector3 position;
    public Vector3 direction;
    public float anglePerFrame;
    public float totalRotation;
    private TankPhysics tankPhysics;
    [Header("Target Data")]
    public GameObject currentTarget;
    public bool targetSelected;
    public bool targetConfirmed;

    // Start is called before the first frame update
    void Start()
    {
        if (!projectileManager) projectileManager = GameObject.Find("GameManager").GetComponent<ProjectileManager>();
        if (!targetManager) targetManager = GameObject.Find("GameManager").GetComponent<TargetManager>();
        tankPhysics = GetComponentInParent<TankPhysics>();
        currentTarget = null;
        targetSelected = false;
        targetConfirmed = false;
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;

        // Allow keyboard rotation
        Rotate();

        if(targetSelected && targetConfirmed)
        {
            TrackTarget();
        }
    }

    // Fires a projectile from the turret
    public void Fire()
    {
        if (projectileManager.canFire == true && targetConfirmed == true) //Input.GetKeyDown(KeyCode.Space) && 
        {
            GameObject newProjectile;

            //Create bullet object
            newProjectile = Instantiate(projectile, position, gameObject.transform.rotation);

            //Set direction of created bullet to direction cannon is facing
            newProjectile.GetComponent<Projectile>().direction = direction;

            // Add list of obstacles to check against
            foreach(GameObject obj in targetManager.civilians)
            {
                newProjectile.GetComponent<bCollisions>().obstacles.Add(obj);
            }
            foreach (GameObject obj in targetManager.hostiles)
            {
                newProjectile.GetComponent<bCollisions>().obstacles.Add(obj);
            }

            //Account for new bullet
            projectileManager.CheckBullets();
        }
        else if(targetConfirmed == false)
        {
            // Do something to alert the player the target has to be confirmed
        }
    }

    public void PickTarget()
    {
        float targetVal = Random.Range(0f, 1f);

        if(targetVal < 0.75f) // Target Hostile
        {
            currentTarget = targetManager.hostiles[Random.Range(0, targetManager.hostiles.Count)];
        }
        else // Target civilian
        {
            currentTarget = targetManager.civilians[Random.Range(0, targetManager.civilians.Count)];
        }

        targetSelected = true;
    }

    public void ConfirmTarget()
    {
        targetConfirmed = true;
    }

    // Rotate the turret towards its target
    private void TrackTarget()
    {
        transform.LookAt(currentTarget.transform);
    }

    //Rotates vehicle each frame depending on keyboard input
    private void Rotate()
    {
        //Rotate left (positive)
        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D)) //Rotate right (negative)
        {
            RotateRight();
        }

        //Update transform component
        //transform.rotation = Quaternion.Euler(0, 0, totalRotation);
    }

    private void RotateLeft()
    {
        totalRotation += anglePerFrame;
        direction = Quaternion.Euler(0, 0, anglePerFrame) * direction;
        //Update transform component
        transform.rotation = Quaternion.Euler(0, 0, totalRotation);
    }

    private void RotateRight()
    {
        totalRotation -= anglePerFrame;
        direction = Quaternion.Euler(0, 0, -anglePerFrame) * direction;
        //Update transform component
        transform.rotation = Quaternion.Euler(0, 0, totalRotation);
    }
}

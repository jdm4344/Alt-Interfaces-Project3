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
    public GameObject crosshairPrefab;
    public GameObject confirmCrosshairPrefab;
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
    private GameObject crosshair;
    public float rotationSpeed;
    private Quaternion trackRotation;
    private Vector3 trackDirection;
    // Timer data
    private float timer;
    private int seconds;
    private int variance;

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
        //Rotate();

        if(targetSelected)
        {
            TrackTarget();
        }

        timer += Time.deltaTime;
        seconds = (int)(timer % 60);

        // Produce a random effect
        if(timer > (60 + variance))
        {
            float rdm = Random.Range(0, 1);

            if(rdm > 0.95f)
            {
                Automate();
            }
            else if(rdm > 0.54f && rdm < 0.63f)
            {
                Fire();
            }

            timer = 0;
            seconds = 0;
            variance = Random.Range(-15, 15);
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

        Destroy(crosshair);
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

        Destroy(crosshair);
        crosshair = Instantiate(confirmCrosshairPrefab, currentTarget.transform.position, Quaternion.identity);
    }

    // Rotate the turret towards its target
    private void TrackTarget()
    {
        if(!crosshair) crosshair = Instantiate(crosshairPrefab, currentTarget.transform.position, Quaternion.identity);

        crosshair.transform.position = currentTarget.transform.position;
        
        Vector3 difference = currentTarget.transform.position - transform.position;

        float rotationZ = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg) - 90;

        direction = Quaternion.Euler(0, 0, rotationZ) * direction;

        /////////////
        //Vector3 VectorResult;
        //float DotResult = DotProduct(transform.up, currentTarget.transform.position);
        //Debug.Log(DotResult);
        //if (DotResult > 0) // Right
        //{
        //    VectorResult = transform.position + currentTarget.transform.position;
        //    Debug.DrawRay(transform.position, VectorResult * 2, Color.red);
        //}
        //else // Left
        //{
        //    VectorResult = transform.position - currentTarget.transform.position;
        //    Debug.DrawRay(transform.position, VectorResult * 2, Color.blue);
        //}

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
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

    private float DotProduct(Vector3 a, Vector3 b)
    {
        return (a.x * b.x) + (a.y * b.y);
    }

    private void Automate()
    {
        PickTarget();

        ConfirmTarget();

        Fire();
    }
}

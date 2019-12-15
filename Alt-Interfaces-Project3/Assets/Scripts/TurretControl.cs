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

    // Start is called before the first frame update
    void Start()
    {
        if (!projectileManager) projectileManager = GameObject.Find("GameManager").GetComponent<ProjectileManager>();
        if (!targetManager) targetManager = GameObject.Find("GameManager").GetComponent<TargetManager>();
        tankPhysics = GetComponentInParent<TankPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;

        Rotate();

        Fire();
    }

    // Fires a projectile from the turret
    public void Fire()
    {
        GameObject newProjectile;

        if (Input.GetKeyDown(KeyCode.Space) && projectileManager.canFire == true)
        {
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
        transform.rotation = Quaternion.Euler(0, 0, totalRotation);
    }

    private void RotateLeft()
    {
        totalRotation += anglePerFrame;
        direction = Quaternion.Euler(0, 0, anglePerFrame) * direction;
        transform.rotation = Quaternion.Euler(0, 0, totalRotation);
    }

    private void RotateRight()
    {
        totalRotation -= anglePerFrame;
        direction = Quaternion.Euler(0, 0, -anglePerFrame) * direction;
        transform.rotation = Quaternion.Euler(0, 0, totalRotation);
    }
}

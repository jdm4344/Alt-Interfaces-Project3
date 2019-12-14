using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Attributes
    [Header("Managers")]
    public ProjectileManager projectileManager;
    [Header("Position")]
    public Vector3 position;
    public Vector3 direction;
    public Vector3 velocity;
    public float speed;
    [Header("Decelleration")]
    public Vector3 deceleration;
    public float decelRate;
    //Off-screen Detection
    private Camera cam;
    private float height;
    private float width;

    // Use this for initialization
    void Start()
    {
        //Calculate velocity based on speed given in Inspector and direcion assigned from Cannon.cs when Instantiated
        //Note: if not assigned from external script, cannonball will not have a direction
        velocity = direction * speed;
        //Get its initial posiiton from its transform
        position = transform.position;

        //Debug.Log("Bullet: dir = " + direction);

        //Setup camera variables
        cam = Camera.main;
        height = 2f * cam.orthographicSize;
        width = height * cam.aspect;

        if (!projectileManager) projectileManager = GameObject.Find("GameManager").GetComponent<ProjectileManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Have the cannonball move
        Move();

        //Slow it down over time
        Decelerate();

        //Check if the cannonball has gone off screen
        OutOfBounds();

        //Have the cannonball "fall into the water" if it hasn't gone off screen
        SlowDestroy();
    }

    // Moves cannonball over time
    void Move()
    {
        position += velocity;
        //Update transform
        transform.position = position;
    }

    // Causes cannonball to slow down over time
    void Decelerate()
    {
        //Calculate deceleration based on current direciton and assigned decelRate 
        deceleration = decelRate * direction;
        //Adjust velocity components
        velocity.x = velocity.x * 0.99f;
        velocity.y = velocity.y * 0.99f;
        //Add velocity to position - this may be arbitrary as this is also done in Move()
        position += velocity;
    }

    //Check if bullet goes off screen; if so, destroy it
    void OutOfBounds()
    {
        if (transform.position.x > width / 2 || transform.position.x < 0 - (width / 2) || transform.position.y > height / 2 || transform.position.y < 0 - (height / 2))
        {
            Destroy(gameObject);
            //Decrement bullet total
            projectileManager.SubBullets();
        }
    }

    //Destroys the cannonball if it falls below a certain speed
    void SlowDestroy()
    {
        if (Mathf.Abs(velocity.x) < 0.006 && Mathf.Abs(velocity.y) < 0.006)
        {
            Destroy(gameObject);
            //Decrement bullet total
            projectileManager.SubBullets();
        }
    }
}

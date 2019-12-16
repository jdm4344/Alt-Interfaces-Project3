using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPhysics : MonoBehaviour
{
    //Attributes
    [Header("Managers")]
    public SerialManager serialManager;
    [Header("Position")]
    public Vector3 position;
    public Vector3 direction;
    public Vector3 velocity;
    [Header("Rotation")]
    public float anglePerFrame;
    public float totalRotation;
    private TurretControl turretScript;
    [Header("Acceleration")]
    public Vector3 acceleration;
    public float accelRate;
    public float maxSpeed;
    [Header("Screen Wrapping")]
    private Camera cam;
    private float height;
    private float width;

    // Use this for initialization
    void Start()
    {
        if (!serialManager) serialManager = GameObject.Find("GameManager").GetComponent<SerialManager>();

        //Determine initial position based on Transform
        position = transform.position;
        //Setup camera variables
        cam = Camera.main;
        height = 2f * cam.orthographicSize;
        width = height * cam.aspect;

        turretScript = gameObject.GetComponentInChildren<TurretControl>();
    }

    // Update is called once per frame
    void Update()
    {
        // Allow keyboard rotation
        Rotate();
        // Allow keyboard movement
        Acceleration();

        Wrap();
    }

    //Rotates vehicle each frame depending on keyboard input
    private void Rotate()
    {
        //Rotate left (positive)
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            totalRotation += anglePerFrame;
            direction = Quaternion.Euler(0, 0, anglePerFrame) * direction;
            //turretScript.direction = Quaternion.Euler(0, 0, anglePerFrame) * direction;
        }
        else if (Input.GetKey(KeyCode.RightArrow)) //Rotate right (negative)
        {
            totalRotation -= anglePerFrame;
            direction = Quaternion.Euler(0, 0, -anglePerFrame) * direction;
            //turretScript.direction = Quaternion.Euler(0, 0, -anglePerFrame) * direction;
        }

        //Update transform component
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, totalRotation);
    }

    // Allows external control to rotate the tank right
    public void RotateRight()
    {
        totalRotation -= anglePerFrame;
        direction = Quaternion.Euler(0, 0, -anglePerFrame) * direction;
    }

    // Allows external control to rotate the tank left
    public void RotateLeft()
    {
        totalRotation += anglePerFrame;
        direction = Quaternion.Euler(0, 0, anglePerFrame) * direction;
    }

    //Handles acceleration and decelleration of vehicle
    private void Acceleration()
    {
        //Check input for acceleration, else decellerate
        if (Input.GetKey(KeyCode.UpArrow))
        {
            acceleration = accelRate * direction;
            //Debug.Log("Accel:" + acceleration);
            velocity += acceleration;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            //Debug.Log("Pos " + velocity);
            position += velocity;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            acceleration = accelRate * -direction;
            //Debug.Log("Accel:" + acceleration);
            velocity += acceleration;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            //Debug.Log("neg " + velocity);
            position += velocity;
        }
        else
        {
            acceleration = accelRate * direction;
            velocity.x = velocity.x * 0.85f;
            velocity.y = velocity.y * 0.85f;
            position += velocity;
        }
    }

    // Allows external control to move the tank forward
    public void Forward()
    {
        acceleration = accelRate * direction;
        //Debug.Log("Accel:" + acceleration);
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        //Debug.Log("Pos " + velocity);
        position += velocity;
    }

    // Allows external control to move the tank in reverse
    public void Reverse()
    {
        acceleration = accelRate * -direction;
        //Debug.Log("Accel:" + acceleration);
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        //Debug.Log("neg " + velocity);
        position += velocity;
    }

    //Wraps vehicle position from one edge of screen to opposite
    private void Wrap()
    {
        //Check X position
        if (transform.position.x > width / 2)
        {
            position.x -= width;
        }
        else if (transform.position.x < 0 - (width / 2))
        {
            position.x += width;
        }

        //Check Y position
        if (transform.position.y > height / 2)
        {
            position.y -= height;
        }
        else if (transform.position.y < 0 - (height / 2))
        {
            position.y += height;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretControl : MonoBehaviour
{
    // Attributes
    [Header("Rotation")]
    public Vector3 direction;
    public float anglePerFrame;
    public float totalRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    //Rotates vehicle each frame depending on keyboard input
    private void Rotate()
    {
        //Rotate left (positive)
        if (Input.GetKey(KeyCode.A))
        {
            totalRotation += anglePerFrame;
            direction = Quaternion.Euler(0, 0, anglePerFrame) * direction;
        }
        else if (Input.GetKey(KeyCode.D)) //Rotate right (negative)
        {
            totalRotation -= anglePerFrame;
            direction = Quaternion.Euler(0, 0, -anglePerFrame) * direction;
        }

        //Update transform component
        //transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, totalRotation);
    }
}

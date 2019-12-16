using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
/*
 * Jordan Machalek
 * Acts as interface between Unity and Arduino
 */
public class SerialManager : MonoBehaviour
{
    // Attributes
    [Header("External Scripts")]
    public TurretControl turretScript;
    public TankPhysics tankScript;
    [Header("Serial Data")]
    private SerialPort stream;
    public int swXVal;
    public int swYVal;
    public int targetVal;
    public int confirmVal;
    public int fireVal;
    // Timer data
    private float timer;
    private int seconds;

    // Start is called before the first frame update
    void Start()
    {
        if (!turretScript) turretScript = GameObject.Find("Turret").GetComponent<TurretControl>();

        stream = new SerialPort("COM3", 9600);
        stream.ReadTimeout = 50;
        stream.Open();
        // Hardcoded String expected from Arduino sketch
        stream.Write("A");

        timer = 0;
        seconds = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        seconds = (int)(timer % 60);

        //if(seconds >= 30)
        //{
        //    WriteToArduino("PING");
        //    seconds = 0;
        //}

        try
        {
            string value = stream.ReadLine();
            Debug.Log(value);
            string[] substrings = value.Split(',');
            //Arduino hopefully sending int,int,int,int,int
            swXVal = int.Parse(substrings[0]); //first of 3 values sent
            swYVal = int.Parse(substrings[1]);
            targetVal = int.Parse(substrings[2]);
            confirmVal = int.Parse(substrings[3]);
            fireVal = int.Parse(substrings[4]);

            stream.Write("A"); //tell arduino to keep going
        }
        catch (Exception e)
        {
            //no op
            Debug.Log(e);
        }

        CheckInputs();
    }

    // Evaluate inputs from the arduino
    private void CheckInputs()
    {
        // Joystick X Axis
        if(swXVal < 200)
        {
            tankScript.RotateLeft();
        }
        else if(swXVal > 900)
        {
            tankScript.RotateRight();
        }

        // Joystick Y Axis
        if(swYVal < 200)
        {
            tankScript.Forward();
        }
        else if (swYVal > 900)
        {
            tankScript.Reverse();
        }

        // Velostat/Pressure Sensor 0
        if(targetVal > 200)
        {
            turretScript.PickTarget();
        }

        // Velostat/Pressure Sensor 1
        if(confirmVal > 200)
        {
            turretScript.ConfirmTarget();
        }

        // Velostat/Pressure Sensor 2
        if(fireVal > 200)
        {
            turretScript.Fire();
        }
    }
}

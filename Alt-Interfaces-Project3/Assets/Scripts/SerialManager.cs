﻿using System.Collections;
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
    public Data dataScript;
    [Header("Serial Data")]
    private SerialPort stream;
    public string port;
    public int swXVal;
    public int swYVal;
    public int targetVal;
    public int confirmVal;
    public int fireVal;
    // Timer data
    private float timer;
    public float seconds;

    // Start is called before the first frame update
    void Start()
    {
        if (!turretScript) turretScript = GameObject.Find("Turret").GetComponent<TurretControl>();
        if (!dataScript) dataScript = GameObject.Find("SerialData").GetComponent<Data>();

        if (dataScript.serialPort == null || dataScript.serialPort == "")
        {
            port = "COM3";
        }
        else
        {
            port = dataScript.serialPort;
        }

        stream = new SerialPort(port, 9600);
        stream.ReadTimeout = 50;
        stream.Open();
        // Hardcoded String expected from Arduino sketch
        stream.Write("A");

        timer = 0;
        seconds = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if(seconds >= 0.5f)
        {
            try
            {
                string value = stream.ReadLine();
                //Debug.Log(value);
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

            timer = 0;
            seconds = 0;
        }

        timer += Time.deltaTime;
        seconds = (timer % 60);

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
        if(targetVal > 150)
        {
            turretScript.PickTarget();
        }

        // Velostat/Pressure Sensor 1
        if(confirmVal > 150)
        {
            turretScript.ConfirmTarget();
        }

        // Velostat/Pressure Sensor 2
        if(fireVal > 150)
        {
            turretScript.Fire();
        }
    }
}

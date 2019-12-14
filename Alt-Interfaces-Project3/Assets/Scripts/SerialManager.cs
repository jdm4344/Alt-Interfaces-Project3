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
    SerialPort stream;

    float timer;
    int seconds;

    // Start is called before the first frame update
    void Start()
    {
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
        //timer += Time.deltaTime;
        //seconds = (int)(timer % 60);

        //if(seconds >= 30)
        //{
        //    WriteToArduino("PING");
        //    seconds = 0;
        //}

        //try
        //{
        //    string value = stream.ReadLine();
        //    string[] substrings = value.Split(',');
        //    //Arduino hopefully sending int,int,int,int,int
        //    float newy = float.Parse(substrings[0]); //first of 3 values sent
        //    newy = (newy - 512f) / 200;
        //    Debug.Log(value);

        //    stream.Write("A"); //tell arduino to keep going
        //    transform.position = new Vector3(transform.position.x, newy, transform.position.z);
        //    //this.transform.position.y = newy;
        //}
        //catch (Exception e)
        //{
        //    //no op
        //    Debug.Log(e);
        //}
    }
}

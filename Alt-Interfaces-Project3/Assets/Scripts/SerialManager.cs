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

    // Start is called before the first frame update
    void Start()
    {
        stream = new SerialPort("COM3", 9600);
        stream.ReadTimeout = 50;
        stream.Open();

        WriteToArduino("PING");

        StartCoroutine(
            AsyncReadFromArduino(
                (string s) => Debug.Log(s),     // Callback
                () => Debug.LogError("Error!"), // Error callback
                10000f                          // Timeout in ms
            )
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WriteToArduino(string msg)
    {
        stream.WriteLine(msg);
        stream.BaseStream.Flush();
    }

    public string ReadFromArduino(int timeout = 0)
    {
        stream.ReadTimeout = timeout;

        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public IEnumerator AsyncReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield break; // terminate coroutine
            }
            else
            {
                yield return null; // wait for next frame
            }

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;
        }
        while (diff.Milliseconds < timeout);

        if(fail != null)
        {
            fail();
        }
        yield return null;
    }
}

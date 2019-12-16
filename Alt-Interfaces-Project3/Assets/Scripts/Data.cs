using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public string serialPort;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        serialPort = "COM3";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

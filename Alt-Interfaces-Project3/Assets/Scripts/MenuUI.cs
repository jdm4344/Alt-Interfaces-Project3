using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public Data serialData;
    private int menuIndex;

    public GameObject newsImage;
    public GameObject introText;
    public Text buttonText;
    public GameObject serialInput;
    public Text serialInputText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(menuIndex)
        {
            case 0:
                newsImage.SetActive(true);
                break;
            case 1:
                newsImage.SetActive(false);
                serialInput.SetActive(true);
                break;
            case 2:
                serialInput.SetActive(false);
                introText.SetActive(true);
                buttonText.text = "Begin Operation";
                break;
            case 3:
                SceneManager.LoadScene("Game");
                break;
        }

        // Update serial port
        serialData.serialPort = serialInputText.text;
    }

    public void advance()
    {
        menuIndex++;
    }
}

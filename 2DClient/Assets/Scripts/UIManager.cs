using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public InputField ipField;

    private void Awake()    //setting up singleton
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void FillLocalHost()
    {
        ipField.text = "127.0.0.1";
    }

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        ipField.interactable = false;
        Client.instance.ip = ipField.text;
        Client.instance.ConnectToServer();
    }
}

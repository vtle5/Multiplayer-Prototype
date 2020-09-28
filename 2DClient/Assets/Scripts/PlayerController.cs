using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera mainCam;
    public PlayerManager manager;
    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        manager.cursor = mainCam.ScreenToWorldPoint(Input.mousePosition); //updates mouse position

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClientSend.PlayerShoot();
        }
    }
    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };

        ClientSend.PlayerMovement(_inputs);
    }
}

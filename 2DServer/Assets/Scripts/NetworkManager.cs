using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public GameObject projectilePrefab;
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

    private void Start()
    {
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 0;     //dont worry, clients can use vsync
        Application.targetFrameRate = 60;   //reducing cpu usage by 10%...

        Server.Start(24, 7020);           //port 7020 cause why not
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }

    public Projectile InstantiateProjectile(Vector3 _shootOrigin,Quaternion _angle)
    {
        return Instantiate(projectilePrefab, _shootOrigin, _angle).GetComponent<Projectile>();
    }
}

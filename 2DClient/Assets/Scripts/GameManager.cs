using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();

    public GameObject localPlayerPrefab;
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

    public void SpawnPlayer(int _id, string _username, Vector3 _position)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, Quaternion.identity);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, Quaternion.identity);
        }

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void SpawnProjectile(int _id, Vector3 _position,Quaternion _rotation,int _owner)
    {
        GameObject _projectile = Instantiate(projectilePrefab, _position, _rotation);
        _projectile.GetComponent<ProjectileManager>().Initialize(_id,_owner);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }
}

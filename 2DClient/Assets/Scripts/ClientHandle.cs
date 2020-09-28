using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour   //this script handles all data it receives, you will add to it
{
    public static void Welcome(Packet _packet)  //welcome packet from the server
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();          //reading packet info

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)  //spawns in a player from a connection
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();

        GameManager.instance.SpawnPlayer(_id, _username, _position);
    }

    public static void PlayerPosition(Packet _packet)   //updates all players positions
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
    }

    public static void PlayerCursorPos(Packet _packet)   //updates all players rotations
    {
        int _id = _packet.ReadInt();
        Vector2 _cursor = _packet.ReadVector2();

        GameManager.players[_id].cursor = _cursor;
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }
    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.players[_id].SetHealth(_health);
    }
    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].Respawn();
    }

    public static void SpawnProjectile(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        int _owner = _packet.ReadInt();
        GameManager.instance.SpawnProjectile(_projectileId, _position, _rotation,_owner);
    }

    public static void ProjectilePosition(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        GameManager.projectiles[_projectileId].transform.position = _position;
        GameManager.projectiles[_projectileId].transform.rotation = _rotation;
    }

    public static void ProjectileHit(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        GameManager.projectiles[_projectileId].Hit(_position);
    }
}

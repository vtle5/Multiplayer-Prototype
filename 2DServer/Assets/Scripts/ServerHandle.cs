using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle   //this script handles all data it receives, you will add to it
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)  //client response to the welcome message
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected sucessfully: Username:{_username}, ID:{_fromClient}");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");   //this should never happen
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)  //receiving player input to process...
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Vector2 _cursor = _packet.ReadVector2();

        Server.clients[_fromClient].player.SetInput(_inputs, _cursor);
    }

    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        //Vector3 _cursor = _packet.ReadVector2();

        Server.clients[_fromClient].player.Shoot();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend         //this script handles all data it will send to clients, check the packet region
{
    private static void SendTCPData(int _toClient, Packet _packet)   //prepares packet to be sent
    {
        _packet.WriteLength(); //writes length of data to the beginning of the packet
        Server.clients[_toClient].tcp.SendData(_packet);    //send the data to the client
    }

    private static void SendUDPData(int _toClient, Packet _packet)   //prepares packet to be sent
    {
        _packet.WriteLength(); //writes length of data to the beginning of the packet
        Server.clients[_toClient].udp.SendData(_packet);    //send the data to the client
    }

    private static void SendTCPDataToAll(Packet _packet)    //sends data to all clients
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)    //sends data to all clients except one
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    private static void SendUDPDataToAll(Packet _packet)    //sends data to all clients
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)    //sends data to all clients except one
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    public static void Welcome(int _toClient, string _msg)  //the welcome packet
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))  //with a using block, it will automatically dispose of the packet
        {                                                               //packets need to be disposed of due to its IDisposable inheritance
            _packet.Write(_msg);
            _packet.Write(_toClient);    //writting info to packet...

            SendTCPData(_toClient, _packet);    //sends off the packet to be sent
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)       //must be sent as TCP as UDP may have packet loss
    {                                                                   //this packet is particularlly important
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);

            SendTCPData(_toClient, _packet);    //spawns in the player and sends the info to the player
        }
    }

    public static void PlayerPosition(Player _player)   //broadcasts all player positions   
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerCursorPos(Player _player)   //this is separate as client cursor control is high prio vs server
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerCursorPos))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.cursor);

            SendUDPDataToAll(_player.id, _packet);  //its ok if this lags a bit
        }
    }

    public static void PlayerDisconnected(int _playerId)   //broadcasts that a player has disconnected
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerHealth(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerRespawned(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
        {
            _packet.Write(_player.id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnProjectile(Projectile _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnProjectile))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_projectile.transform.rotation);
            _packet.Write(_projectile.owner);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ProjectilePosition(Projectile _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectilePosition))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_projectile.transform.rotation);

            SendUDPDataToAll(_packet);
        }
    }
    public static void ProjectileHit(Projectile _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectileHit))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendTCPDataToAll(_packet);
        }
    }
    #endregion
}
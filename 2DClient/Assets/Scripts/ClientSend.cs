using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ClientSend : MonoBehaviour     //this script handles all packets sent to the server, check the Packets Region
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()    //tells the server we received the welcome packet
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs)   //tells the server our inputs
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.players[Client.instance.myId].cursor);

            SendUDPData(_packet); //sending via UDP as it is faster and unimportant if some data is lost.
        }
    }

    public static void PlayerShoot()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            //_packet.Write(_cursor);

            SendTCPData(_packet); //sending via UDP as it is faster and unimportant if some data is lost.
        }
    }
    #endregion
}

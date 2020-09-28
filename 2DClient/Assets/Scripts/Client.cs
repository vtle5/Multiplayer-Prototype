using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour //Remember to update the InitializeClientData with new ClientHandle packets!
{
    public static Client instance;
    public static int dataBufferSize = 4096;    //4 mb for some reason

    public string ip = "127.0.0.1";
    public int port = 7020;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

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

    private void OnApplicationQuit()
    {
        Disconnect();           //auto disconnects when application closes
    }

    public void ConnectToServer()
    {
        tcp = new TCP();
        udp = new UDP();        //sets up our network protocols

        InitializeClientData(); //self explanatory
        isConnected = true;
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;   //network connections are continuous
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()   //initializes connection to the server
        {
            socket = new TcpClient      //setting up socket
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);   //connects to server
        }

        private void ConnectCallback(IAsyncResult _result)  //when the server responds for the first time
        {
            socket.EndConnect(_result); //end the connection proccess

            if (!socket.Connected)  //if connection failed, exit out of method
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();    //generates a new received packet

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);  //begin reading from server
        }

        public void SendData(Packet _packet)    //generic send data function
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try         //inside a try catch so that the client does not crash on error
            {
                int _byteLength = stream.EndRead(_result);  //received data from server
                if (_byteLength <= 0)                         //if no data received, exit method
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);  //copy received data

                receivedData.Reset(HandleData(_data));      //resets the data if the dataHandle allows it
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);  //continue streaming from server
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving TCP data: {_ex}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();   //sets the length to the first int value in the packet
                if (_packetLength <= 0)                  //if there is no more data allow it to be reset
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });

                _packetLength = 0;

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();   //sets the length to the first int value in the packet
                    if (_packetLength <= 0)                  //if there is no more data allow it to be reset
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending to server via UDP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                Disconnect();
            }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }

        private void Disconnect()
        {
            instance.Disconnect();
            endPoint = null;
            socket = null;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome},
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer},
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition},
            { (int)ServerPackets.playerCursorPos, ClientHandle.PlayerCursorPos},
            { (int)ServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected},
            { (int)ServerPackets.playerHealth, ClientHandle.PlayerHealth},
            { (int)ServerPackets.playerRespawned, ClientHandle.PlayerRespawned},
            { (int)ServerPackets.spawnProjectile, ClientHandle.SpawnProjectile},
            { (int)ServerPackets.projectilePosition, ClientHandle.ProjectilePosition},
            { (int)ServerPackets.projectileHit, ClientHandle.ProjectileHit},
        };
        Debug.Log("Initialized packets.");
    }

    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();
        }

        Debug.Log("Disconnected from server.");
    }
}

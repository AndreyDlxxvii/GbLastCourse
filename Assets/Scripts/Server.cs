using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private const int MAX_CONNECTION = 10;
    private const int Port = 5805;
    
    private int _hostID;
    private int _reliableChannel;
    private bool _isStarted = false;
    private byte _error;
    private string _userName;
    private List<int> _connectionIDs = new List<int>();
    private Dictionary<int, string> _userDict = new Dictionary<int, string>();
    public void StartServer()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        _reliableChannel = cc.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
        _hostID = NetworkTransport.AddHost(topology, Port);
        _isStarted = true;
    }
    public void ShutDownServer()
    {
        if (!_isStarted) return;
        NetworkTransport.RemoveHost(_hostID);
        NetworkTransport.Shutdown();
        _isStarted = false;
    }


    void Update()
    {
        if (!_isStarted) return;
        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out
            channelId, recBuffer, bufferSize, out dataSize, out _error);


        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.ConnectEvent:
                    _connectionIDs.Add(connectionId);
                    SendMessageToAll($"Player {connectionId} has connected.");
                    Debug.Log($"Player {connectionId} has connected.");
                    break;
                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    var name = message.Substring(0, message.IndexOf("#"));
                    message = message.Substring(message.IndexOf("#"));
                    SendMessageToAll($"{name} : {message.Substring(message.IndexOf("#")+1)}");
                    Debug.Log($"{name} : {message}");
                    _userDict.Add(connectionId, name);
                    // SendMessageToAll($"{name} {connectionId}: {message.Substring(message.IndexOf("#"))}");
                    // Debug.Log($"{name} {connectionId}: {message}");
                    break;
                case NetworkEventType.DisconnectEvent:
                    _connectionIDs.Remove(connectionId);
                    SendMessageToAll($"Player {connectionId} has disconnected.");
                    Debug.Log($"Player {connectionId} has disconnected.");
                    break;
                case NetworkEventType.BroadcastEvent:
                    break;
            }
            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                bufferSize, out dataSize, out _error);
        }
    }
    public void SendMessageToAll(string message)
    {
        for (int i = 0; i < _connectionIDs.Count; i++)
        {
            SendMessage(message, _connectionIDs[i]);
        }
    }
    public void SendMessage(string message, int connectionID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(_hostID, connectionID, _reliableChannel, buffer, message.Length *
                                                                             sizeof(char), out _error);
        if ((NetworkError)_error != NetworkError.Ok) Debug.Log((NetworkError)_error);
    }
}

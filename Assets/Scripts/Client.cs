using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    [SerializeField] private InputNameUser _inputNameUser;
    public delegate void OnMessageReceive(object message);
    public event OnMessageReceive onMessageReceive;
    private const int MAX_CONNECTION = 10;
    private int _port = 0;
    private int _serverPort = 5805;
    private int _hostID;
    private int _reliableChannel;
    private int _connectionID;
    private bool _isConnected;
    private byte _error;
    private string _nameUser;
    private bool _isNamed;
    public void Connect()
    {
        _inputNameUser.gameObject.SetActive(true);
        _inputNameUser.NameEntered += ConnectWithName;
    }

    private void ConnectWithName()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        _reliableChannel = cc.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);
        _hostID = NetworkTransport.AddHost(topology, _port);
        _connectionID = NetworkTransport.Connect(_hostID, "127.0.0.1", _serverPort, 0, out _error);
        if ((NetworkError) _error == NetworkError.Ok)
        {
            _isConnected = true;
            _nameUser = _inputNameUser.UserName;
        }
            
        else
            Debug.Log((NetworkError)_error);
    }
    public void Disconnect()
    {
        if (!_isConnected) return;
        NetworkTransport.Disconnect(_hostID, _connectionID, out _error);
        _isConnected = false;
    }
    void Update()
    {
        if (!_isConnected && !_isNamed) return;
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
                    onMessageReceive?.Invoke($"You have been connected to server.");
                    Debug.Log($"You have been connected to server.");
                    break;
                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    message = message.Substring(message.IndexOf("#")+1);
                    onMessageReceive?.Invoke(message);
                    Debug.Log(message);
                    break;
                case NetworkEventType.DisconnectEvent:
                    _isConnected = false;
                    onMessageReceive?.Invoke($"You have been disconnected from server.");
                    Debug.Log($"You have been disconnected from server.");
                    break;
                case NetworkEventType.BroadcastEvent:
                    break;
            }
            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer,
                bufferSize, out dataSize, out _error);
        }
    }
    public void SendMessage(string message)
    {
        var t = $"{_inputNameUser.UserName}#{message}";
        byte[] buffer = Encoding.Unicode.GetBytes(t);
        NetworkTransport.Send(_hostID, _connectionID, _reliableChannel, buffer, t.Length * sizeof(char), out _error);
        if ((NetworkError)_error != NetworkError.Ok) Debug.Log((NetworkError)_error);
    }
}
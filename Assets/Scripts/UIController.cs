using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button buttonStartServer;
    [SerializeField] private Button buttonShutDownServer;
    [SerializeField] private Button buttonConnectClient;
    [SerializeField] private Button buttonDisconnectClient;
    [SerializeField] private Button buttonSendMessage;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextField textField;
    [SerializeField] private Server server;
    [SerializeField] private Client client;
    private void Start()
    {
        buttonStartServer.onClick.AddListener(() => StartServer());
        buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
        buttonConnectClient.onClick.AddListener(() => Connect());
        buttonDisconnectClient.onClick.AddListener(() => Disconnect());
        buttonSendMessage.onClick.AddListener(() => SendMessage());
        client.onMessageReceive += ReceiveMessage;
    }
    private void StartServer()
    {
        server.StartServer();
    }
    private void ShutDownServer()
    {
        server.ShutDownServer();
    }
    private void Connect()
    {
        client.Connect();
    }
    private void Disconnect()
    {
        client.Disconnect();
    }
    private void SendMessage()
    {
        client.SendMessage(inputField.text);
        inputField.text = "";
    }
    public void ReceiveMessage(object message)
    {
        textField.ReceiveMessage(message);
    }

    private void OnDestroy()
    {
        buttonStartServer.onClick.RemoveAllListeners();
        buttonShutDownServer.onClick.RemoveAllListeners();
        buttonConnectClient.onClick.RemoveAllListeners();
        buttonDisconnectClient.onClick.RemoveAllListeners();
        buttonSendMessage.onClick.RemoveAllListeners();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputNameUser : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _button;
    private string _userName;
    private bool _isNamed = false;

    public delegate void Action();
    public event Action NameEntered;

    public bool IsNamed => _isNamed;

    public string UserName => _userName;

    private void Start()
    {
        _button.onClick.AddListener(ReadUserName);
    }

    private void ReadUserName()
    {
        _userName = _inputField.text;
        _isNamed = true;
        gameObject.SetActive(false);
        NameEntered?.Invoke();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }
}

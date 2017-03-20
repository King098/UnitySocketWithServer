using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// demo类，测试客户端和服务器端的连接测试
/// </summary>
public class UIController : MonoBehaviour
{
    public InputField username;
    public InputField password;

    public GameObject LoginObj;
    public GameObject RegisterObj;

    public InputField r_account;
    public InputField r_usernamet;
    public InputField r_password;

    public bool login;

    private static UIController m_self = null;
    public static UIController Instance
    {
        get
        {
            return m_self;
        }
    }

    void Awake()
    {
        m_self = this;
    }

    public void Login()
    {
        if (SocketManager.Instance != null && SocketManager.Instance.mSocket != null && SocketManager.Instance.mSocket.IsConnected && !login)
        {
            if (!string.IsNullOrEmpty(username.text) && !string.IsNullOrEmpty(password.text))
            {
                NetworkController.RequestLogin(username.text, password.text);
            }
        }
    }

    public void Register()
    {
        if (SocketManager.Instance != null && SocketManager.Instance.mSocket != null && SocketManager.Instance.mSocket.IsConnected)
        {
            RegisterObj.SetActive(true);
            LoginObj.SetActive(false);
        }
    }

    public void CancelRegister()
    {
        RegisterObj.SetActive(false);
        LoginObj.SetActive(true);
    }

    public void RegisterPlayer()
    {
        if (SocketManager.Instance != null && SocketManager.Instance.mSocket != null && SocketManager.Instance.mSocket.IsConnected)
        {
            Debug.Log("注册用户");
            NetworkController.RequestRegister(r_account.text,r_usernamet.text,r_password.text);
        }
    }
}

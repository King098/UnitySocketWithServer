using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

public class SocketManager : MonoBehaviour
{

    private static SocketManager m_self = null;
    public static SocketManager Instance
    {
        get
        {
            return m_self;
        }
    }

    void Awake()
    {
        m_self = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public ClientSocket mSocket;

    public bool connectWhenStart;
    public string ip;
    public int port;
    public int time_out;

    public bool needAliveLink;
    public float time_count;

    private float CurTimeCount;
    private bool IsAliveLinking;


    // Use this for initialization
    void Start()
    {
        if (connectWhenStart)
        {
            ConnectToServer();
        }
    }

    /// <summary>
    /// 连接到服务器的接口
    /// </summary>
    public void ConnectToServer()
    {
        mSocket = new ClientSocket();
        mSocket.ConnectServer(ip, port, time_out);
    }

    public void Reconnect()
    {
        mSocket.Close();
        ConnectToServer();
    }

    void OnDestory()
    {
        if (mSocket != null)
        {
            mSocket.Close();
        }
    }

    public void OnConnetTimeOut()
    {
        //TODO:连接超时处理
    }

    public void OnConnectError(string error)
    {
        //TODO:连接出错处理
    }

    public void OnSendError(string error)
    {
        //TODO:当发送数据出错处理
    }

    public void OnReceiveError(string error)
    {
        //TODO:当接收数据出错处理
        Debug.LogWarning("Error:" + error);
    }

    //开始一个心跳连接
    public void StartAliveLink()
    {
        if (needAliveLink)
        {
            CurTimeCount = time_count;
            IsAliveLinking = false;
        }
    }

    void Update()
    {
        if (needAliveLink && !IsAliveLinking)
        {
            CurTimeCount -= Time.deltaTime;
            if (CurTimeCount <= 0f)
            {
                IsAliveLinking = true;
                CurTimeCount = time_count;
                //发送一条连接
                NetworkController.RequestAlive();
            }
        }
    }

}

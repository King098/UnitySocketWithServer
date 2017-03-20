using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using System;

public class EasyLoom : MonoBehaviour 
{
    private static EasyLoom m_self = null;
    public static EasyLoom Instance
    {
        get
        {
            return m_self;
        }
    }

    void Awake()
    {
        m_self = this;
        SendList = new List<LoomInfo>();
        ReceiveList = new List<LoomInfo>();
    }

    private List<LoomInfo> SendList;
    private List<LoomInfo> ReceiveList;
    private LoomInfo SendTemp;
    private LoomInfo ReceiveTemp;

    public void AddSendInfo(LoomInfo info)
    {
        SendList.Add(info);
    }

    public void DeleteSendInfo()
    {
        SendList.RemoveAt(0);
    }

    public void AddReceiveInfo(LoomInfo info)
    {
        ReceiveList.Add(info);
    }

    public void DeleteReceiveInfo()
    {
        ReceiveList.RemoveAt(0);
    }


    void SendData()
    {
        if (SendList != null && SendList.Count > 0)
        {
            SendTemp = SendList[0];
            DeleteSendInfo();
            NetworkController.SendData(SendTemp.proto, SendTemp.data);
        }
    }

    void ReceiveData()
    {
        if (ReceiveList != null && ReceiveList.Count > 0)
        {
            ReceiveTemp = ReceiveList[0];
            DeleteReceiveInfo();
            NetworkController.ReceiveData(ReceiveTemp.proto, ReceiveTemp.data);
        }
    }

    void Update()
    {
        if (SocketManager.Instance != null && SocketManager.Instance.mSocket != null && SocketManager.Instance.mSocket.IsConnected)
        {
            ReceiveData();
            SendData();
        }
    }
}

public class LoomInfo
{
    public CmdEnum proto;
    public byte[] data;

    public LoomInfo(CmdEnum _proto, byte[] _data)
    {
        proto = _proto;
        data = new byte[_data.Length];
        Array.Copy(_data,0, data, 0, _data.Length);
    }
}

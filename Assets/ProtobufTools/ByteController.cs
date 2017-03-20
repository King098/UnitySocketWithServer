using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using UnityEngine;
using ProtoBuf;

/// <summary>
/// 字节控制类，一个单例类，用于缓存服务器发过来的所有byte数据，用于解决黏包、分包的问题
/// </summary>
public class ByteController
{
    private static ByteController m_self = null;
    public static ByteController Instance
    {
        get
        {
            if (m_self == null)
            {
                m_self = new ByteController();
            }
            return m_self;
        }
    }

    private ByteController() { }

    private List<byte> Datas = new List<byte>();
    private ByteBuffer buffer;

    public void AddBytes(byte[] data)
    {
        //lock (Datas)
        {
            Datas.AddRange(data);
            buffer = ReadBytes();
            while (buffer != null)
            {
                int datalength = buffer.ReadShort();
                int typeId = buffer.ReadInt();
                byte[] pbdata = buffer.ReadBytes();

                Debug.Log("接收到新的消息，消息长度为:" + (datalength + sizeof(ushort)) + ";剩余消息长度:" + Datas.Count);

                NetworkController.Receive((CmdEnum)typeId, pbdata);

                buffer = ReadBytes();
            }
        }
    }

    public ByteBuffer ReadBytes()
    {
        //lock (Datas)
        {
            if (Datas.Count <= 0)
                return null;

            byte[] b = new byte[ProtobufTool.headLength];
            Array.Copy(Datas.ToArray(), 0, b, 0, b.Length);
            int dataLength = (int)BitConverter.ToInt16(b,0);
            //reader.Close();
            //取出数据长度段
            //byte[] len = BitConverter.GetBytes((ushort)dataLength);
            if (Datas.Count >= ProtobufTool.headLength + dataLength)
            {
                //取出数据段
                byte[] data = new byte[ProtobufTool.headLength + dataLength];
                Array.Copy(Datas.ToArray(), 0, data, 0, dataLength + ProtobufTool.headLength);
                Datas.RemoveRange(0, ProtobufTool.headLength + dataLength);

                return new ByteBuffer(data);
            }
            else
            {
                return null;
            }
        }
    }

    public void UpdateMessage()
    {
        buffer = ReadBytes();
        if(buffer != null)
        {
            int datalength = buffer.ReadShort();
            int typeId = buffer.ReadInt();
            byte[] pbdata = buffer.ReadBytes();

            Debug.Log("接收到新的消息，消息长度为:" + (datalength + sizeof(ushort)) + ";剩余消息长度:" + Datas.Count);

            NetworkController.Receive((CmdEnum)typeId, pbdata);
        }
    }
}

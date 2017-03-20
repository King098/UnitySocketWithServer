using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System;
using UnityEngine;
using System.Net;
using System.Threading;

namespace ProtoBuf
{
	public class ClientSocket 
	{
	    private static byte[] result = new byte[10];  
        private static Socket clientSocket;
        private Thread receiveThread;
        //是否已连接的标识
        public bool IsConnected = false;  
  
        public ClientSocket()
        {  
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
        }

        public void ConnectServer(string ip, int port, int time_out)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
                IAsyncResult result = clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), clientSocket);
                bool success = result.AsyncWaitHandle.WaitOne(time_out, true);
                if (!success)//超时
                {
                    Debug.LogWarning("连接超时");
                    clientSocket.Disconnect(true);
                    //超时处理
                    SocketManager.Instance.OnConnetTimeOut();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                //当连接出错处理
                SocketManager.Instance.OnConnectError(e.ToString());
            }
        }

        void OnConnect(IAsyncResult result)
        {
            if (result.AsyncState == clientSocket)
            {
                try
                {
                    clientSocket.EndConnect(result);
                    IsConnected = true;

                    Debug.Log("连接服务器成功");

                    //开始心跳连接
                    SocketManager.Instance.StartAliveLink();

                    ReceiveSocket();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    //当连接出错处理
                    SocketManager.Instance.OnConnectError(e.ToString());
                }
            }
        }

        /// <summary>
        /// 关闭当前的链接
        /// </summary>
        public void Close()
        {
            if (!IsConnected)
                return;
            IsConnected = false;
            if (receiveThread != null)
            {
                receiveThread.Abort();
                receiveThread = null;
            }

            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Disconnect(true);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                clientSocket = null;
            }
        }

        /// <summary>
        /// 断线重连
        /// </summary>
        void ReConnect()
        {
            SocketManager.Instance.Reconnect();
        }

        private void ReceiveSocket()
        {
            //while (true)
            {
                if (!clientSocket.Connected)
                {
                    IsConnected = false;
                    //重连接
                    ReConnect();
                    return;
                }

                try
                {
                    clientSocket.BeginReceive(result, 0, result.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
                }
                catch (Exception e)
                {
                    //当连接出错处理
                    SocketManager.Instance.OnReceiveError(e.ToString());
                    clientSocket.Disconnect(true);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
        }

        void OnReceive(IAsyncResult res)
        {
            if (res.AsyncState == clientSocket)
            {
                try
                {
                    int receiveLength = clientSocket.EndReceive(res);
                    if (receiveLength > 0)
                    {
                        //Debug.Log("接收到新的消息，消息长度为:" + receiveLength);
                        //ByteBuffer buff = new ByteBuffer(result);
                        //int datalength = buff.ReadShort();

                        byte[] r = new byte[receiveLength];
                        Array.Copy(result, 0, r, 0, r.Length);

                        //Debug.Log("新消息:" + r.Length);

                        ByteController.Instance.AddBytes(r);

                        //ByteController.Instance.ReadBytes();

                        //NetworkController.Receive((CmdEnum)typeId, pbdata);
                    }
                    //在进行接收数据
                    ReceiveSocket();
                }
                catch (Exception e)
                {
                    //当连接出错处理
                    SocketManager.Instance.OnReceiveError(e.ToString());
                    clientSocket.Disconnect(true);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            
        }


        public void SendMessage(byte[] data)
        {
            if (!clientSocket.Connected)
            {
                IsConnected = false;
                //重连接
                ReConnect();
                return;
            }

            try
            {
                clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSend), clientSocket);
            }
            catch (Exception e)
            {
                SocketManager.Instance.OnSendError(e.ToString());
                clientSocket.Disconnect(true);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                return;
            }
        }


        public void OnSend(IAsyncResult result)
        {
            if (result.AsyncState == clientSocket && IsConnected)
            {
                try
                {
                    clientSocket.EndSend(result);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    SocketManager.Instance.OnSendError(e.ToString());
                    IsConnected = false;
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
        }
    }  	
}

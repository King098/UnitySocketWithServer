using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using UnityEngine.SceneManagement;
public class NetworkController
{
    #region 与服务器数据交互，通过EasyLoom类进行线程间通信
    public static void Receive(CmdEnum cmd,byte[] data)
    {
        EasyLoom.Instance.AddReceiveInfo(new LoomInfo(cmd, data));
    }

    public static void Request(CmdEnum cmd,byte[] data)
    {
        EasyLoom.Instance.AddSendInfo(new LoomInfo(cmd, data));
    }
    #endregion


    public static void SendData(CmdEnum cmd, byte[] data)
    {
        SocketManager.Instance.mSocket.SendMessage(data);
    }

    public static void ReceiveData(CmdEnum cmd, byte[] data)
    {
        if (cmd == CmdEnum.Res_Login)
        {
            ResponseLogin login = SerializeTool.Deserialize<ResponseLogin>(data);
            ResponseLogin(login);
        }
        else if (cmd == CmdEnum.Res_Alive)
        {
            ResponseAlive alive = SerializeTool.Deserialize<ResponseAlive>(data);
            ResponseAlive(alive);
        }
        else if (cmd == CmdEnum.Res_Register)
        {
            ResponseRegister response = SerializeTool.Deserialize<ResponseRegister>(data);
            ResponseRegister(response);
        }
        else if (cmd == CmdEnum.Res_CreateRoom)
        {
            ResponseCreateRoom response = SerializeTool.Deserialize<ResponseCreateRoom>(data);
            ResponseCreateRoom(response);
        }
        else if (cmd == CmdEnum.Res_GetRooms)
        {
            ResponseGetRooms response = SerializeTool.Deserialize<ResponseGetRooms>(data);
            ResponseGetRooms(response);
        }
        else if (cmd == CmdEnum.Res_DeleteRoom)
        {
            ResponseDeleteRoom response = SerializeTool.Deserialize<ResponseDeleteRoom>(data);
            ResponseDeleteRoom(response);
        }
    }

    #region 发收消息的具体逻辑处理

    //请求注册
    public static void RequestRegister(string account, string username, string password)
    {
        RequestRegister req = new RequestRegister()
        {
            proto = (int)CmdEnum.Req_Register,
            account = account,
            username = username,
            password = password
        };
        byte[] data = ProtobufTool.CreateData(req.proto, req);
        Request((CmdEnum)req.proto, data);
    }

    //返回注册
    public static void ResponseRegister(ResponseRegister response)
    {
        if (string.IsNullOrEmpty(response.error))
        {
            Debug.Log("注册成功:account" + response.player_info.account);
            UIController.Instance.CancelRegister();
        }
    }

    //请求登录
    public static void RequestLogin(string username, string password)
    {
        RequestLogin req = new RequestLogin()
        {
            proto = (int)CmdEnum.Req_Login,
            account = username,
            password = password
        };
        byte[] data = ProtobufTool.CreateData(req.proto, req);
        Request((CmdEnum)req.proto, data);
    }

    //返回登录
    public static void ResponseLogin(ResponseLogin result)
    {
        #region 测试代码
        if(UIController.Instance != null)
        {
            if (result.error != "")
            {
                Debug.LogWarning(result.error + result.result);
            }
            else
            {
                Debug.Log(result.result);
                UIController.Instance.login = true;
                GameController.Instance.player = result.player;
                SceneManager.LoadScene("sample-2");
            }
        }
        #endregion
    }

    //请求心跳
    public static void RequestAlive()
    {
        RequestAlive req = new RequestAlive()
        {
            proto = (int)CmdEnum.Req_Alive
        };
        byte[] data = ProtobufTool.CreateData(req.proto, req);
        Request((CmdEnum)req.proto, data);
    }

    //返回心跳
    public static void ResponseAlive(ResponseAlive result)
    {
        SocketManager.Instance.StartAliveLink();
    }

    //请求创建房间
    public static void RequestCreateRoom(string room_name,string pass_word,bool can_watch,int count)
    {
        RequestCreateRoom req = new RequestCreateRoom()
        {
            proto = (int)CmdEnum.Req_CreateRoom,
            room_name = room_name,
            password = pass_word,
            can_watch = can_watch,
            player_number = count,
            player_id = GameController.Instance.player.id
        };
        byte[] data = ProtobufTool.CreateData(req.proto, req);
        Request((CmdEnum)req.proto, data);
    }

    //返回创建房间
    public static void ResponseCreateRoom(ResponseCreateRoom response)
    {
        //关闭创建界面，然后再次请求房间列表
        RoomUIController.Instance.ClickCancel();
    }

    //请求房间列表
    public static void RequestGetRooms()
    {
        RequestGetRooms req = new RequestGetRooms()
        {
            proto = (int)CmdEnum.Req_GetRooms
        };
        byte[] data = ProtobufTool.CreateData(req.proto, req);
        Request((CmdEnum)req.proto, data);
    }

    //返回房间列表
    public static void ResponseGetRooms(ResponseGetRooms response)
    {
        if (response != null)
        {
            RoomUIController.Instance.CreateRoomList(response.rooms);
        }
    }

    //请求删除房间
    public static void RequestDeleteRoom()
    { 
    }

    //返回删除房间
    public static void ResponseDeleteRoom(ResponseDeleteRoom response)
    { 
    }
    #endregion
}

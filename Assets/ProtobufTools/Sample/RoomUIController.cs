using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using UnityEngine.UI;

public class RoomUIController : MonoBehaviour 
{
    private static RoomUIController m_self = null;
    public static RoomUIController Instance
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

    public VerticalLayoutGroup grid;
    public GameObject room;

    public GameObject create_obj;
    public InputField roomNameInput;
    public Toggle needPasswordToggle;
    public InputField passwordInput;
    public Toggle canWatchToggle;
    public InputField playerNumberInput;

    void Start()
    {
        NetworkController.RequestGetRooms();
    }

    public void CreateRoomList(List<GameRoomInfo> rooms)
    {
        room.SetActive(false);
        ClearChild(grid.gameObject, false);
        for (int i = 0; i < rooms.Count; i++)
        {
            GameObject obj = Instantiate(room) as GameObject;
            if (obj != null)
            {
                Debug.Log("房间号:" + rooms[i].room_id);
                obj.transform.SetParent(grid.transform, false);
                obj.SetActive(true);
                Text t = obj.GetComponentInChildren<Text>();
                if(t != null)
                {
                   t.text = t.text.Replace("room_id", rooms[i].room_id.ToString()).Replace("room_name", rooms[i].room_name);
                }
                obj.name = rooms[i].room_id.ToString();
            }
        }
    }

    public void ClickPasswrodToggle(bool ison)
    {
        needPasswordToggle.isOn = ison;
        passwordInput.gameObject.SetActive(needPasswordToggle.isOn);
    }

    public void ClickCanWatchToggle(bool ison)
    {
        canWatchToggle.isOn = ison;
        playerNumberInput.gameObject.SetActive(canWatchToggle.isOn);
    }

    public void ClickCreate()
    {
        if (roomNameInput.text != "")
        {
            if (!needPasswordToggle.isOn || passwordInput.text != "")
            {
                if (!canWatchToggle.isOn || playerNumberInput.text != "")
                {
                    NetworkController.RequestCreateRoom(roomNameInput.text,needPasswordToggle.isOn ? "" : passwordInput.text,canWatchToggle.isOn,int.Parse(playerNumberInput.text));
                }
            }
        }
    }

    public void ClickCancel()
    {
        create_obj.SetActive(false);
        NetworkController.RequestGetRooms();
    }

    public void ClearChild(GameObject parent, bool include_inactive)
    {
        Transform[] trans = parent.GetComponentsInChildren<Transform>(include_inactive);
        for (int i = 1; i < trans.Length; i++)
        {
            Destroy(trans[i].gameObject);
        }
    }
}

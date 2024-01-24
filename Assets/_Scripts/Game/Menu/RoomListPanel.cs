using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : MonoBehaviour
{
    public GameObject buttonRoom;
    public List<RoomInfo> roomList;
    public Transform roomContainer;

    public void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        roomList = p_list;
        ClearRoomList();

        foreach (RoomInfo a in roomList)
        {
            GameObject newRoomButton = Instantiate(buttonRoom, roomContainer);
            JoinRoomButton joinRoomButton = newRoomButton.GetComponent<JoinRoomButton>();
            joinRoomButton.Setup(a);
        }
    }

    private void ClearRoomList()
    {
        foreach (Transform t in roomContainer) Destroy(t.gameObject);
    }
}
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class JoinRoomButton : MonoBehaviour
{
    public RoomInfo roomInfo;

    public TextMeshProUGUI roomNameTMP;
    public TextMeshProUGUI mapNameTMP;
    public TextMeshProUGUI gameModeTMP;
    public TextMeshProUGUI playersCountTMP;

    public void Setup(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        roomNameTMP.text = roomInfo.Name;
        playersCountTMP.text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;

        if (roomInfo.CustomProperties.ContainsKey("map"))
            mapNameTMP.text = Launcher.Instance.maps[(int)roomInfo.CustomProperties["map"]].name;
        else
            mapNameTMP.text = "-----";

        if (roomInfo.CustomProperties.ContainsKey("mode"))
            gameModeTMP.text = System.Enum.GetName(typeof(GameMode), (int)roomInfo.CustomProperties["mode"]);
        else
            gameModeTMP.text = "-----";
    }

    public void OnClick()
    {
        if (roomInfo != null)
        {
            Launcher.Instance.LoadGameSettings(roomInfo);
            PhotonNetwork.JoinRoom(roomNameTMP.text);
        }
    }
}
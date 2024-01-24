using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FFARoomInfo : RoomInfoPanel
{
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI currentPlayers;
    public TextMeshProUGUI map;
    public TextMeshProUGUI mode;

    public Button startButton;

    public GameObject playerInfoPrefab;
    public Transform playersContainer;

    private Dictionary<int, RoomPlayerInfo> playerInfos = new();

    public override void Setup()
    {
        roomName.text = "Room name: " + PhotonNetwork.CurrentRoom.Name;
        currentPlayers.text = "Players: " + PhotonNetwork.CurrentRoom.Players.Count.ToString();
        map.text = "Map: " + Launcher.Instance.maps[(int)PhotonNetwork.CurrentRoom.CustomProperties["map"]].name;
        mode.text = "Mode: " + Enum.GetName(typeof(GameMode), (int)PhotonNetwork.CurrentRoom.CustomProperties["mode"]);

        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        SetupPlayers();
    }

    public void SetupPlayers()
    {
        foreach (var p in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject instance = Instantiate(playerInfoPrefab, playersContainer);
            RoomPlayerInfo roomPlayerInfo = instance.GetComponent<RoomPlayerInfo>();
            roomPlayerInfo.Setup(p.Value);
            playerInfos[p.Key] = roomPlayerInfo;
        }
    }

    public void StartGame()
    {
        Launcher.Instance.StartGame();
    }

    public override void OnMasterClientSwitched()
    {
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Launcher.Instance.TabOpenMainMenu();
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        currentPlayers.text = "Players: " + PhotonNetwork.CurrentRoom.Players.Count.ToString();
        GameObject instance = Instantiate(playerInfoPrefab, playersContainer);
        RoomPlayerInfo roomPlayerInfo = instance.GetComponent<RoomPlayerInfo>();
        roomPlayerInfo.Setup(player);
        playerInfos[player.ActorNumber] = roomPlayerInfo;
    }
    public override void OnPlayerLeftRoom(Player player)
    {
        currentPlayers.text = "Players: " + PhotonNetwork.CurrentRoom.Players.Count.ToString();
        Destroy(playerInfos[player.ActorNumber].gameObject);
        playerInfos.Remove(player.ActorNumber);
    }
}
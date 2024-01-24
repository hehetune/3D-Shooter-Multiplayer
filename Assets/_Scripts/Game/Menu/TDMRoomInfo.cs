using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TDMRoomInfo : RoomInfoPanel
{
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI map;
    public TextMeshProUGUI mode;

    public Button startButton;

    public GameObject playerInfoPrefab;
    public Transform milkPlayersContainer;
    public Transform chocoPlayersContainer;
    public Button milkJoin;
    public Button chocoJoin;

    private Dictionary<int, RoomPlayerInfo> milkPlayerInfos = new();
    private Dictionary<int, RoomPlayerInfo> chocoPlayerInfos = new();

    public bool IsWaiting = true;
    public bool IsMilkTeam = true;

    public override void Setup()
    {
        roomName.text = "Room name: " + PhotonNetwork.CurrentRoom.Name;
        map.text = "Map: " + Launcher.Instance.maps[(int)PhotonNetwork.CurrentRoom.CustomProperties["map"]].name;
        mode.text = "Mode: " + Enum.GetName(typeof(GameMode), (int)PhotonNetwork.CurrentRoom.CustomProperties["mode"]);

        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        SetupPlayers();
    }

    public void SetupPlayers()
    {
        foreach (var p in PhotonNetwork.CurrentRoom.Players)
        {
            if (!p.Value.CustomProperties.ContainsKey("IsMilkTeam")) continue;
            GameObject instance = null;
            bool isMilkTeam = (bool)p.Value.CustomProperties["IsMilkTeam"];
            if (isMilkTeam)
                instance = Instantiate(playerInfoPrefab, milkPlayersContainer);
            else
                instance = Instantiate(playerInfoPrefab, chocoPlayersContainer);
            RoomPlayerInfo roomPlayerInfo = instance.GetComponent<RoomPlayerInfo>();
            roomPlayerInfo.Setup(p.Value);
            if (isMilkTeam)
                milkPlayerInfos[p.Key] = roomPlayerInfo;
            else chocoPlayerInfos[p.Key] = roomPlayerInfo;
        }

        Hashtable hehe = new()
        {
            {"IsMilkTeam", null},
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(hehe);

        ShowJoinButton();
    }

    private void ShowJoinButton()
    {
        if (IsWaiting)
        {
            milkJoin.gameObject.SetActive(true);
            chocoJoin.gameObject.SetActive(true);
        }
        else
        {
            milkJoin.gameObject.SetActive(false);
            chocoJoin.gameObject.SetActive(false);
            if (IsMilkTeam && chocoPlayerInfos.Count < 5) chocoJoin.gameObject.SetActive(true);
            if (!IsMilkTeam && milkPlayerInfos.Count < 5) milkJoin.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
            {
                if (!kvp.Value.CustomProperties.ContainsKey("IsMilkTeam"))
                {
                    PhotonNetwork.CloseConnection(kvp.Value);
                }
            }
        }

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

    public void JoinMilkTeam()
    {
        if (!IsWaiting && IsMilkTeam) return;

        GameObject instance = Instantiate(playerInfoPrefab, milkPlayersContainer);
        RoomPlayerInfo roomPlayerInfo = instance.GetComponent<RoomPlayerInfo>();
        roomPlayerInfo.Setup(PhotonNetwork.LocalPlayer);
        milkPlayerInfos[PhotonNetwork.LocalPlayer.ActorNumber] = roomPlayerInfo;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "IsMilkTeam", true } });

        if (!IsWaiting && !IsMilkTeam)
        {
            Destroy(chocoPlayerInfos[PhotonNetwork.LocalPlayer.ActorNumber].gameObject);
            chocoPlayerInfos.Remove(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        IsWaiting = false;
        IsMilkTeam = true;
        ShowJoinButton();

        photonView.RPC(nameof(NotifyTeamChange), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, true);
    }

    public void JoinChocoTeam()
    {
        if (!IsWaiting && !IsMilkTeam) return;

        GameObject instance = Instantiate(playerInfoPrefab, chocoPlayersContainer);
        RoomPlayerInfo roomPlayerInfo = instance.GetComponent<RoomPlayerInfo>();
        roomPlayerInfo.Setup(PhotonNetwork.LocalPlayer);
        chocoPlayerInfos[PhotonNetwork.LocalPlayer.ActorNumber] = roomPlayerInfo;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "IsMilkTeam", false } });

        if (!IsWaiting && IsMilkTeam)
        {
            Destroy(milkPlayerInfos[PhotonNetwork.LocalPlayer.ActorNumber].gameObject);
            milkPlayerInfos.Remove(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        IsWaiting = false;
        IsMilkTeam = false;
        ShowJoinButton();

        photonView.RPC(nameof(NotifyTeamChange), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, false);
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        base.OnPlayerEnteredRoom(player);
    }
    public override void OnPlayerLeftRoom(Player player)
    {
        if (milkPlayerInfos.ContainsKey(player.ActorNumber))
        {
            Destroy(milkPlayerInfos[player.ActorNumber].gameObject);
            milkPlayerInfos.Remove(player.ActorNumber);
        }
        else if (chocoPlayerInfos.ContainsKey(player.ActorNumber))
        {
            Destroy(chocoPlayerInfos[player.ActorNumber].gameObject);
            chocoPlayerInfos.Remove(player.ActorNumber);
        }

        base.OnPlayerLeftRoom(player);
    }

    [PunRPC]
    public void NotifyTeamChange(int actorNumber, bool isMilkTeam)
    {
        if (isMilkTeam)
        {
            if (chocoPlayerInfos.ContainsKey(actorNumber))
            {
                Destroy(chocoPlayerInfos[actorNumber].gameObject);
                chocoPlayerInfos.Remove(actorNumber);
            }

            GameObject instance = Instantiate(playerInfoPrefab, milkPlayersContainer);
            RoomPlayerInfo roomPlayerInfo = instance.GetComponent<RoomPlayerInfo>();
            roomPlayerInfo.Setup(PhotonNetwork.CurrentRoom.Players[actorNumber]);
            milkPlayerInfos[actorNumber] = roomPlayerInfo;
        }
        else
        {
            if (chocoPlayerInfos.ContainsKey(actorNumber))
            {
                Destroy(chocoPlayerInfos[actorNumber].gameObject);
                chocoPlayerInfos.Remove(actorNumber);
            }

            GameObject instance = Instantiate(playerInfoPrefab, chocoPlayersContainer);
            RoomPlayerInfo roomPlayerInfo = instance.GetComponent<RoomPlayerInfo>();
            roomPlayerInfo.Setup(PhotonNetwork.CurrentRoom.Players[actorNumber]);
            chocoPlayerInfos[actorNumber] = roomPlayerInfo;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    public static ProfileData myProfile = new();

    public MainMenu tabMainMenu;
    public RoomListPanel tabRoomList;
    public CreateRoomPanel tabCreateRoom;
    public List<RoomInfoPanel> tabsRoomInfo;
    public Dictionary<GameMode, RoomInfoPanel> roomInfoDic = new();
    public RoomInfoPanel CurrentRoomInfo => roomInfoDic[GameSettings.GameMode];

    public MapData[] maps;
    public MapData CurrentMap => maps[currentMapIndex];
    public int currentMapIndex = 0;


    public void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        foreach (var room in tabsRoomInfo)
        {
            roomInfoDic.Add(room.gameMode, room);
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.EnableCloseConnection = true;

        myProfile = Data.LoadProfile();
        if (!string.IsNullOrEmpty(myProfile.username))
        {
            tabMainMenu.usernameField.text = myProfile.username;
        }

        Connect();
    }

    public void Connect()
    {
        Debug.Log("Trying to connect...");
        PhotonNetwork.GameVersion = "0.0.0";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ChangeMap()
    {
        currentMapIndex++;
        if (currentMapIndex >= maps.Length) currentMapIndex = 0;
    }

    public void LoadGameSettings(RoomInfo roomInfo)
    {
        GameSettings.GameMode = (GameMode)roomInfo.CustomProperties["mode"];
    }

    public void SyncPlayerInfo()
    {
        if (string.IsNullOrEmpty(tabMainMenu.usernameField.text))
        {
            myProfile.username = "RANDOM_USER_" + Random.Range(100, 1000);
        }
        else
        {
            myProfile.username = tabMainMenu.usernameField.text;
        }

        Hashtable hehe = new()
        {
            {"Username", myProfile.username},
            {"Level", myProfile.level},
            {"Xp", myProfile.xp},
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(hehe);
    }

    public void StartGame()
    {
        SyncPlayerInfo();

        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        // if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        // {
        Data.SaveProfile(myProfile);
        PhotonNetwork.LoadLevel(CurrentMap.scene);
        // }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
        PhotonNetwork.JoinLobby();

        TabOpenMainMenu();

        base.OnConnectedToMaster();
    }

    // public override void OnCreatedRoom()
    // {
    //     PhotonNetwork.JoinRoom(PhotonNetwork.CurrentRoom.Name);
    //     // TabOpenRoomInfo();
    //     base.OnCreatedRoom();
    // }

    public override void OnJoinedRoom()
    {
        SyncPlayerInfo();
        TabOpenRoomInfo();
        base.OnJoinedRoom();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        CurrentRoomInfo.OnMasterClientSwitched();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // tabCreateRoom.Create();
        Debug.Log("OnJoinRandomFailed");
        base.OnJoinRandomFailed(returnCode, message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        Debug.Log("OnRoomListUpdate");
        tabRoomList.OnRoomListUpdate(p_list);

        base.OnRoomListUpdate(p_list);
    }

    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     CurrentRoomInfo.OnPlayerEnteredRoom(newPlayer);
    //     base.OnPlayerEnteredRoom(newPlayer);
    // }
    // public override void OnPlayerLeftRoom(Player otherPlayer)
    // {
    //     CurrentRoomInfo.OnPlayerLeftRoom(otherPlayer);
    //     base.OnPlayerLeftRoom(otherPlayer);
    // }

    public void TabCloseAll()
    {
        tabMainMenu.gameObject.SetActive(false);
        tabRoomList.gameObject.SetActive(false);
        tabCreateRoom.gameObject.SetActive(false);
        CurrentRoomInfo.gameObject.SetActive(false);
    }
    public void TabOpenMainMenu()
    {
        TabCloseAll();
        tabMainMenu.gameObject.SetActive(true);
    }
    public void TabOpenRoomList()
    {
        TabCloseAll();
        tabRoomList.gameObject.SetActive(true);
    }
    public void TabOpenCreateRoom()
    {
        currentMapIndex = 0;
        TabCloseAll();
        tabCreateRoom.Refresh();
        tabCreateRoom.gameObject.SetActive(true);
    }
    public void TabOpenRoomInfo()
    {
        TabCloseAll();
        CurrentRoomInfo.Setup();
        CurrentRoomInfo.gameObject.SetActive(true);
    }

}

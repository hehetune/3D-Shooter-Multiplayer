using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class CreateRoomPanel : MonoBehaviour
{
    public TMP_InputField roomNameField;
    public UnitValueSlider maxPlayersSlider;
    public TextMeshProUGUI maxPlayersValue;
    public TextMeshProUGUI mapValue;
    public TextMeshProUGUI modeValue;

    public void Refresh()
    {
        roomNameField.text = "";
        mapValue.text = Launcher.Instance.CurrentMap.name.ToUpper();
        maxPlayersSlider.valuePerUnit = (float)1 / GameSettings.MAX_PLAYERS;
        maxPlayersSlider.OnValueChangedAction += () =>
        {
            maxPlayersValue.text = maxPlayersSlider.curUnit.ToString();
        };
        maxPlayersSlider.UpdateValue(GameSettings.MAX_PLAYERS);
        maxPlayersValue.text = maxPlayersSlider.curUnit.ToString();

        GameSettings.GameMode = 0;
        modeValue.text = Enum.GetName(typeof(GameMode), 0);
    }

    public void Create()
    {
        RoomOptions options = new()
        {
            MaxPlayers = (byte)maxPlayersSlider.curUnit,
            CleanupCacheOnLeave = true,
        };

        ExitGames.Client.Photon.Hashtable properties = new()
        {
            { "map", Launcher.Instance.currentMapIndex },
            { "mode", (int)GameSettings.GameMode },
        };
        options.CustomRoomProperties = properties;
        options.CustomRoomPropertiesForLobby = new string[] { "map", "mode" };

        PhotonNetwork.CreateRoom(roomNameField.text, options);
    }

    public void ChangeMap()
    {
        Launcher.Instance.ChangeMap();
        mapValue.text = Launcher.Instance.CurrentMap.name.ToUpper();
    }

    public void ChangeMode()
    {
        int newMode = (int)GameSettings.GameMode + 1;
        if (newMode >= Enum.GetValues(typeof(GameMode)).Length) newMode = 0;
        GameSettings.GameMode = (GameMode)newMode;
        modeValue.text = Enum.GetName(typeof(GameMode), newMode);
    }
}
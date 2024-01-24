using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public Image background;
    public Image underline;

    public TextMeshProUGUI levelTMP;
    public TextMeshProUGUI usernameTMP;
    public TextMeshProUGUI scoreTMP;
    public TextMeshProUGUI kdaTMP;

    public void Setup(PlayerMatchData playerMatchData, GameMode gameMode)
    {
        levelTMP.text = playerMatchData.profile.level.ToString("00");
        usernameTMP.text = playerMatchData.profile.username;
        scoreTMP.text = (playerMatchData.kills * 100).ToString();
        kdaTMP.text = playerMatchData.kills.ToString() + "/" + playerMatchData.deaths.ToString() + "/" + playerMatchData.assists.ToString();

        if (gameMode == GameMode.TDM)
        {
            if (playerMatchData.isMilkTeam)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == playerMatchData.actor)
                    SetupColor(ColorData.milkPlayerBackgroundColor, ColorData.milkPlayerTextColor);
                else SetupColor(ColorData.milkPlayerBackgroundColor, ColorData.milkPlayerTextColor);
            }
            else
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == playerMatchData.actor)
                    SetupColor(ColorData.chocoPlayerBackgroundColor, ColorData.chocoPlayerTextColor);
                else SetupColor(ColorData.chocoPlayerBackgroundColor, ColorData.chocoPlayerTextColor);
            }
        }
        else if (gameMode == GameMode.FFA)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerMatchData.actor) SetupColor(ColorData.milkPlayerBackgroundColor, ColorData.milkPlayerTextColor);
            else SetupColor(ColorData.chocoBackgroundColor, ColorData.chocoTextColor);
        }
    }

    private void SetupColor(Color32 backgroundColor, Color32 textColor)
    {
        levelTMP.color = textColor;
        usernameTMP.color = textColor;
        scoreTMP.color = textColor;
        kdaTMP.color = textColor;
        background.color = backgroundColor;
        underline.color = backgroundColor;
    }
}
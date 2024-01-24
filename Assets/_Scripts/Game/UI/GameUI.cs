using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject ui_leaderboard;
    public GameObject ui_endGame;
    public GameObject ui_pauseGame;
    public LeaderBoard FFA_LeaderBoard;
    public LeaderBoard TDM_LeaderBoard;
    public LeaderBoard EndGame_TDM_LeaderBoard;
    public LeaderBoard EndGame_FFA_LeaderBoard;
    public TextMeshProUGUI timerTMP;
    public TextMeshProUGUI chocoScoreTMP;
    public TextMeshProUGUI milkScoreTMP;

    public void ShowLeaderBoard(bool show)
    {
        if (ui_leaderboard.gameObject.activeSelf == show) return;
        ui_leaderboard.gameObject.SetActive(show);
        if (show) RefreshLeaderBoard(GameSettings.GameMode, GameManager.Instance.playerDataList.Values.ToList());
    }

    private void SetupLeaderBoard(LeaderBoard leaderBoard, List<PlayerMatchData> playerMatchDatas)
    {
        leaderBoard.Setup(playerMatchDatas);
    }

    public void RefreshLeaderBoard(LeaderBoard leaderBoard, List<PlayerMatchData> playerMatchDatas)
    {
        if (!leaderBoard.initialized) SetupLeaderBoard(leaderBoard, playerMatchDatas);
        else
        {
            leaderBoard.Refresh(playerMatchDatas);
        }
    }

    public void RefreshLeaderBoard(GameMode gameMode, List<PlayerMatchData> playerMatchDatas)
    {
        if (gameMode == GameMode.FFA)
        {
            RefreshLeaderBoard(FFA_LeaderBoard, playerMatchDatas);
        }
        else if (gameMode == GameMode.TDM)
        {
            RefreshLeaderBoard(TDM_LeaderBoard, playerMatchDatas);
        }
    }

    public void SetupEndGameLeaderBoard(GameMode gameMode, List<PlayerMatchData> playerMatchDatas)
    {
        if (gameMode == GameMode.FFA)
        {
            SetupLeaderBoard(EndGame_FFA_LeaderBoard, playerMatchDatas);
        }
        else if (gameMode == GameMode.TDM)
        {
            SetupLeaderBoard(EndGame_TDM_LeaderBoard, playerMatchDatas);
        }
    }

    public void Resume()
    {
        GameManager.Instance.TogglePause();
    }

    public void Quit()
    {
        GameManager.Instance.Quit();
    }
}

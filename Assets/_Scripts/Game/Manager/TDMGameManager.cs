using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TDMGameManager : GameManager
{
    #region Fields

    [SerializeField]
    private Transform attackerSpawnpointsParent;
    [SerializeField]
    private Transform defenderSpawnpointsParent;
    private bool milkIsAttacker;

    const int MATCH_POINT_SCORE = 12;
    const int POINT_GAP = 2;
    private int milkScore;
    private int chocoScore;

    private bool milkWin;

    #endregion

    #region MonoBehaviour Callbacks

    protected override void Awake()
    {
        if (GameSettings.GameMode != GameMode.TDM)
        {
            gameObject.SetActive(false);
            return;
        }

        base.Awake();

        if (Instance == null) Instance = this;

        milkIsAttacker = Random.Range(0, 2) == 1;
        SetupTeam();
    }

    #endregion

    #region Photon

    public override void NewPlayer_S(ProfileData p)
    {
        bool isMilkTeam = (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsMilkTeam"];
        GameSettings.IsMilkTeam = isMilkTeam;

        object[] package = new object[8];

        package[0] = p.username;
        package[1] = p.level;
        package[2] = p.xp;
        package[3] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[4] = (short)0;
        package[5] = (short)0;
        package[6] = (short)0;
        package[7] = isMilkTeam;

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
        );
    }
    public override void NewPlayer_R(object[] data)
    {
        int actorNumber = (int)data[3];
        PlayerMatchData p = new(
            new ProfileData(
                (string)data[0],
                (int)data[1],
                (int)data[2]
            ),
            actorNumber,
            (short)data[4],
            (short)data[5],
            (short)data[6],
            (bool)data[7]
        );

        playerDataList[actorNumber] = p;
    }

    public override void UpdateGameState_R(object[] data)
    {
        DetectTeamWin();
        base.UpdateGameState_R(data);
    }

    #endregion

    #region Methods

    public void SetupTeam()
    {
        foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
        {
            if ((bool)kvp.Value.CustomProperties["IsMilkTeam"])
            {
                photonView.RPC(nameof(SyncTeam), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, milkIsAttacker);
            }
            else
            {
                photonView.RPC(nameof(SyncTeam), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, !milkIsAttacker);
            }
        }
    }

    [PunRPC]
    private void SyncTeam(int actorNumber, bool isAttacker)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            ExitGames.Client.Photon.Hashtable hehe = new()
            {
                {"IsAttacker", isAttacker},
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(hehe);
        }
    }

    public bool CheckEndGame()
    {
        if (milkScore > MATCH_POINT_SCORE && chocoScore < MATCH_POINT_SCORE)
        {
            UpdateGameState_S(GameState.Ending);
            return true;
        }
        if (chocoScore > MATCH_POINT_SCORE && milkScore < MATCH_POINT_SCORE)
        {
            UpdateGameState_S(GameState.Ending);
            return true;
        }
        if (milkScore >= MATCH_POINT_SCORE && chocoScore >= MATCH_POINT_SCORE && Mathf.Abs(milkScore - chocoScore) >= POINT_GAP)
        {
            UpdateGameState_S(GameState.Ending);
            return true;
        }

        return false;
    }

    private void DetectTeamWin()
    {
        if (milkScore > MATCH_POINT_SCORE && chocoScore < MATCH_POINT_SCORE)
        {
            milkWin = true;
        }
        if (chocoScore > MATCH_POINT_SCORE && milkScore < MATCH_POINT_SCORE)
        {
            milkWin = false;
        }

        if (milkScore >= MATCH_POINT_SCORE && chocoScore >= MATCH_POINT_SCORE)
        {
            if (milkScore - chocoScore >= POINT_GAP)
            {
                milkWin = true;
            }
            if (chocoScore - milkScore >= POINT_GAP)
            {
                milkWin = false;
            }
        }
    }

    public void EndRound(bool attackerWin)
    {
        CalculateScore(attackerWin);

        if (milkScore + chocoScore == MATCH_POINT_SCORE)
            milkIsAttacker = !milkIsAttacker;

        if (CheckEndGame())
        {

        }
        else
        {
            StartTimer();
        }
    }

    private void CalculateScore(bool attackerWin)
    {
        if (attackerWin)
        {
            milkScore += milkIsAttacker ? 1 : 0;
            chocoScore += milkIsAttacker ? 0 : 1;
        }
        else
        {
            milkScore += milkIsAttacker ? 0 : 1;
            chocoScore += milkIsAttacker ? 1 : 0;
        }
    }

    #endregion

    #region Coroutines

    protected void StartTimer()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(Timer());
    }

    protected IEnumerator Timer()
    {
        currentMatchTime = prepareLength;
        RefreshTimerUI();
        UpdateGameState_S(GameState.Waiting);

        while (currentMatchTime > 0)
        {
            yield return 1f.Wait();
            currentMatchTime -= 1;
            RefreshTimer_S();
        }

        currentMatchTime = matchLength;
        RefreshTimerUI();
        UpdateGameState_S(GameState.Playing);

        while (currentMatchTime > 0)
        {
            yield return 1f.Wait();
            currentMatchTime -= 1;
            RefreshTimer_S();
        }

        timerCoroutine = null;
        EndRound(false);
    }

    #endregion
}
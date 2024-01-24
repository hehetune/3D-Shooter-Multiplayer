using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FFAGameManager : GameManager
{
    #region Fields
    const int PREPARE_TIME = 20;
    const int MATCH_TIME = 300;

    public int killcount = 5;
    public Transform randomSpawnPoints;
    private List<int> emptyIndexes = new();
    private List<int> busyIndexes = new();
    // private Dictionary<int, int> actorAndSpawnIndexMap;

    public Transform GetRandomSpawnPoint => randomSpawnPoints.GetChild(Random.Range(0, randomSpawnPoints.childCount));

    private int winner;
    #endregion

    #region MonoBehaviour Callbacks

    protected override void Awake()
    {
        if (GameSettings.GameMode != GameMode.FFA)
        {
            gameObject.SetActive(false);
            return;
        }

        base.Awake();

        if (Instance == null) Instance = this;
        prepareLength = PREPARE_TIME;
        matchLength = MATCH_TIME;
        for (int i = 0; i < randomSpawnPoints.childCount; i++) emptyIndexes.Add(i);
    }

    protected override void Start()
    {
        base.Start();

        //test
        // for (int i = 0; i < 10; i++)
        // {
        //     SpawnTestPlayer();
        // }
    }

    #endregion

    #region Photon

    public override void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200) return;

        EventCodes e = (EventCodes)photonEvent.Code;
        object[] o = (object[])photonEvent.CustomData;

        switch (e)
        {
            case EventCodes.NewPlayer:
                NewPlayer_R(o);
                break;

            case EventCodes.ChangeStat:
                ChangeStat_R(o);
                break;

            case EventCodes.RefreshTimer:
                RefreshTimer_R(o);
                break;

            case EventCodes.UpdateGameState:
                UpdateGameState_R(o);
                break;

            case EventCodes.SyncSpawnPoints:
                SyncSpawnPoints_R(o);
                break;
        }
    }

    public override void NewPlayer_S(ProfileData p)
    {
        base.NewPlayer_S(p);

        object[] package = new object[7];

        package[0] = p.username;
        package[1] = p.level;
        package[2] = p.xp;
        package[3] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[4] = (short)0;
        package[5] = (short)0;
        package[6] = (short)0;

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
        );
    }
    public override void NewPlayer_R(object[] data)
    {
        base.NewPlayer_R(data);

        PlayerMatchData p = new(
            new ProfileData(
                (string)data[0],
                (int)data[1],
                (int)data[2]
            ),
            (int)data[3],
            (short)data[4],
            (short)data[5],
            (short)data[6]
        );

        Debug.Log("receive actor: " + (int)data[3]);

        playerDataList[(int)data[3]] = p;
    }

    public override void ChangeStat_R(object[] data)
    {
        base.ChangeStat_R(data);

        CheckEndGame(data);
    }

    public override void UpdateGameState_R(object[] data)
    {
        base.UpdateGameState_R(data);
        DetectWinner();
    }

    public virtual void SyncSpawnPoints_S(int index, bool add)
    {
        object[] package = new object[2] { index, add };

        PhotonNetwork.RaiseEvent(
                (byte)EventCodes.SyncSpawnPoints,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                new SendOptions { Reliability = true }
         );
    }

    public virtual void SyncSpawnPoints_R(object[] data)
    {
        int index = (int)data[0];
        bool add = (bool)data[1];

        if (add)
        {
            emptyIndexes.Remove(index);
            busyIndexes.Add(index);
        }
        else
        {
            emptyIndexes.Add(index);
            busyIndexes.Remove(index);
        }
    }

    #endregion

    #region Methods

    //test
    // public void ReturnToEmptySpawnPoints(Transform go)
    // {
    //     go.SetParent(emptySpawnPointsParent);
    // }

    //test
    // public void SpawnTestPlayer()
    // {

    //     int randIdx = Random.Range(0, emptySpawnPointsParent.childCount);
    //     Transform t_spawn = emptySpawnPointsParent.GetChild(randIdx);
    //     t_spawn.SetParent(busySpawnPointsParent);
    //     PoolManager.Get<PoolObject>(playerPrefab, out var instance);
    //     instance.transform.SetPositionAndRotation(t_spawn.position, t_spawn.rotation);
    //     instance.transform.SetParent(t_spawn);
    //     PlayerController playerController = instance.GetComponent<PlayerController>();
    //     playerController.playerStatus.OnPlayerDie += () =>
    //     {
    //         instance.ReturnToPool();
    //         ReturnToEmptySpawnPoints(t_spawn);
    //         Invoke(nameof(SpawnTestPlayer), 2f);
    //     };
    // }

    public override void SpawnPlayer()
    {
        if (PhotonNetwork.IsConnected)
        {
            int randIdx = Random.Range(0, emptyIndexes.Count);
            Transform t_spawn = randomSpawnPoints.GetChild(emptyIndexes[randIdx]);
            emptyIndexes.Remove(randIdx);
            busyIndexes.Add(randIdx);
            SyncSpawnPoints_S(randIdx, true);
            // actorAndSpawnIndexMap[PhotonNetwork.LocalPlayer.ActorNumber] = randIdx;
            GameObject player_GO = PhotonNetwork.Instantiate("Prefabs/Player/" + playerPrefab.name, t_spawn.position, t_spawn.rotation);
            PlayerController playerController = player_GO.GetComponent<PlayerController>();
            playerController.playerStatus.OnPlayerDie += () =>
            {
                busyIndexes.Remove(randIdx);
                emptyIndexes.Add(randIdx);
                SyncSpawnPoints_S(randIdx, false);
            };
        }
    }

    public override void RespawnPlayer(GameObject player)
    {
        Transform t_spawn = randomSpawnPoints.GetChild(Random.Range(0, randomSpawnPoints.childCount));
        player.transform.position = t_spawn.position;
    }

    public bool CheckEndGame(object[] data)
    {
        if (!PhotonNetwork.IsMasterClient || State == GameState.Ending) return false;
        if (playerDataList[(int)data[0]].kills >= killcount) UpdateGameState_S(GameState.Ending);
        return true;
    }

    protected override void InitializeTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartTimer();
        }
    }

    private void DetectWinner()
    {
        int max = 0;
        foreach (var kvp in playerDataList)
        {
            if (kvp.Value.kills > max)
            {
                max = kvp.Value.kills;
                winner = kvp.Value.actor;
            }
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
        RefreshTimer_S();
        UpdateGameState_S(GameState.Waiting);

        while (currentMatchTime > 1)
        {
            yield return 1f.Wait();
            currentMatchTime -= 1;
            RefreshTimer_S();
        }

        currentMatchTime = matchLength;
        RefreshTimer_S();
        UpdateGameState_S(GameState.Playing);
        yield return 1f.Wait();

        while (currentMatchTime > 0)
        {
            yield return 1f.Wait();
            currentMatchTime -= 1;
            RefreshTimer_S();
        }

        timerCoroutine = null;
        UpdateGameState_S(GameState.Ending);
    }

    #endregion

}
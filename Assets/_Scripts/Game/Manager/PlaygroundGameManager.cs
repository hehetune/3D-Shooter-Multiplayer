using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlaygroundGameManager : GameManager
{
    #region Fields
    public Transform emptySpawnPointsParent;
    public Transform busySpawnPointsParent;

    public Transform GetRandomSpawnPoint => emptySpawnPointsParent.GetChild(Random.Range(0, emptySpawnPointsParent.childCount));

    #endregion

    #region MonoBehaviour Callbacks

    protected override void Awake()
    {
        if (GameSettings.GameMode != GameMode.TEST)
        {
            gameObject.SetActive(false);
            return;
        }

        base.Awake();

        if (Instance == null) Instance = this;
    }

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < 10; i++)
        {
            SpawnTestPlayer();
        }
    }

    #endregion

    #region Photon

    public override void NewPlayer_S(ProfileData p)
    {
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
            (short)data[6]
        );

        playerDataList[actorNumber] = p;
    }

    #endregion

    #region Methods

    public void ReturnToEmptySpawnPoints(Transform go)
    {
        go.SetParent(emptySpawnPointsParent);
    }

    public void SpawnTestPlayer()
    {
        int randIdx = Random.Range(0, emptySpawnPointsParent.childCount);
        Transform t_spawn = emptySpawnPointsParent.GetChild(randIdx);
        t_spawn.SetParent(busySpawnPointsParent);
        PoolManager.Get<PoolObject>(playerPrefab, out var instance);
        instance.transform.SetPositionAndRotation(t_spawn.position, t_spawn.rotation);
        instance.transform.SetParent(t_spawn);
        PlayerController playerController = instance.GetComponent<PlayerController>();
        playerController.playerStatus.OnPlayerDie += () =>
        {
            instance.ReturnToPool();
            ReturnToEmptySpawnPoints(t_spawn);
            Invoke(nameof(SpawnTestPlayer), 2f);
        };
    }

    public override void SpawnPlayer()
    {
        Debug.Log("SpawnPlayer1");
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("SpawnPlayer2");
            int randIdx = Random.Range(0, emptySpawnPointsParent.childCount);
            Transform t_spawn = emptySpawnPointsParent.GetChild(randIdx);
            t_spawn.SetParent(busySpawnPointsParent);
            GameObject player_GO = PhotonNetwork.Instantiate("Prefabs/Player/" + playerPrefab.name, t_spawn.position, t_spawn.rotation);
            player_GO.transform.SetParent(t_spawn);
        }
    }

    public override void RespawnPlayer(GameObject player)
    {
        Transform t_spawn = emptySpawnPointsParent.GetChild(Random.Range(0, emptySpawnPointsParent.childCount));
        player.transform.position = t_spawn.position;
    }

    #endregion

    #region Coroutines


    #endregion

}
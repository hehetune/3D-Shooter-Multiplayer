using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
	Waiting = 0,
	Playing = 2,
	Ending = 3
}

public enum EventCodes : byte
{
	//Menu
	Room_PlayerJoin,
	Room_PlayerLeave,

	//In game
	NewPlayer,
	UpdatePlayers,
	ChangeStat,
	NewMatch,
	RefreshTimer,
	UpdateGameState,

	//FFA
	SyncSpawnPoints
}

public enum PlayerStat
{
	Kill = 0,
	Dead = 1,
	Assist = 2,
}

public struct KillData
{
	public int killerActor;
	public int victimActor;
	public List<int> assistActors;
}

public abstract class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
	#region Fields
	public static GameManager Instance;

	public Dictionary<int, PlayerMatchData> playerDataList = new();
	public PlayerMatchData CurrentPlayerData => playerDataList[PhotonNetwork.LocalPlayer.ActorNumber];

	public int mainMenuSceneIndex = 0;

	public Camera mapCam;

	public Prefab playerPrefab;

	public bool paused = false;

	public GameState State = GameState.Waiting;
	public bool IsEnded => State == GameState.Ending;
	public int matchLength;
	public int prepareLength;

	protected bool disconnecting = false;

	protected int currentMatchTime;
	protected Coroutine timerCoroutine;

	protected GameUI gameUI;

	#endregion

	#region MonoBehaviour Callbacks
	protected virtual void Awake() { }

	protected virtual void Start()
	{
		gameUI = UIManager.Instance.gameUI;
		mapCam.gameObject.SetActive(false);

		ValidateConnection();
		InitializeTimer();
		NewPlayer_S(Launcher.myProfile);
	}

	public override void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	public override void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}
	#endregion

	#region Photon
	public virtual void OnEvent(EventData photonEvent)
	{
		if (photonEvent.Code >= 200) return;

		EventCodes e = (EventCodes)photonEvent.Code;
		object[] o = (object[])photonEvent.CustomData;

		switch (e)
		{
			case EventCodes.NewPlayer:
				NewPlayer_R(o);
				break;

			// case EventCodes.UpdatePlayers:
			// 	UpdatePlayers_R(o);
			// 	break;

			case EventCodes.ChangeStat:
				ChangeStat_R(o);
				break;

			// case EventCodes.NewMatch:
			// 	NewMatch_R();
			// 	break;

			case EventCodes.RefreshTimer:
				RefreshTimer_R(o);
				break;
			case EventCodes.UpdateGameState:
				UpdateGameState_R(o);
				break;
		}
	}

	// public override void OnLeftRoom()
	// {
	// 	base.OnLeftRoom();
	// 	SceneManager.LoadScene(mainMenuSceneIndex);
	// }

	public virtual void NewPlayer_S(ProfileData p)
	{
		SpawnPlayer();
	}
	public virtual void NewPlayer_R(object[] data) { }
	public virtual void ChangeStat_S(int actor, byte stat, byte amt)
	{
		object[] package = new object[] { actor, stat, amt };

		PhotonNetwork.RaiseEvent(
			(byte)EventCodes.ChangeStat,
			package,
			new RaiseEventOptions { Receivers = ReceiverGroup.All },
			new SendOptions { Reliability = true }
		);
	}
	public virtual void ChangeStat_R(object[] data)
	{
		switch ((byte)data[1])
		{
			case (byte)PlayerStat.Kill:
				playerDataList[(int)data[0]].kills += (byte)data[2];
				break;
			case (byte)PlayerStat.Dead:
				playerDataList[(int)data[0]].deaths += (byte)data[2];
				break;
			case (byte)PlayerStat.Assist:
				playerDataList[(int)data[0]].assists += (byte)data[2];
				break;
			default: break;
		}
	}

	public virtual void UpdateGameState_S(GameState state)
	{
		object[] package = new object[] { (int)state };

		PhotonNetwork.RaiseEvent(
			(byte)EventCodes.UpdateGameState,
			package,
			new RaiseEventOptions { Receivers = ReceiverGroup.All },
			new SendOptions { Reliability = true }
		);
	}

	public virtual void UpdateGameState_R(object[] data)
	{
		State = (GameState)data[0];
		StateCheck();
	}

	public void RefreshTimer_S()
	{
		object[] package = new object[] { currentMatchTime };

		PhotonNetwork.RaiseEvent(
			(byte)EventCodes.RefreshTimer,
			package,
			new RaiseEventOptions { Receivers = ReceiverGroup.All },
			new SendOptions { Reliability = true }
		);
	}
	public void RefreshTimer_R(object[] data)
	{
		currentMatchTime = (int)data[0];
		RefreshTimerUI();
	}


	#endregion

	#region Methods

	public virtual void SpawnPlayer() { }

	public virtual void RespawnPlayer(GameObject player) { }

	public void StateCheck()
	{
		if (State == GameState.Ending)
		{
			EndGame();
		}
	}

	protected void EndGame()
	{
		// set game state to ending
		State = GameState.Ending;

		// set timer to 0
		if (timerCoroutine != null) StopCoroutine(timerCoroutine);
		currentMatchTime = 0;
		RefreshTimerUI();

		// activate map camera
		// mapCam.gameObject.SetActive(true);

		// show end game ui
		gameUI.SetupEndGameLeaderBoard(GameSettings.GameMode, playerDataList.Values.ToList());
		gameUI.ui_endGame.SetActive(true);
	}

	protected virtual void InitializeTimer() { }

	protected void RefreshTimerUI()
	{
		string minutes = (currentMatchTime / 60).ToString("00");
		string seconds = (currentMatchTime % 60).ToString("00");
		gameUI.timerTMP.text = $"{minutes}:{seconds}";
	}

	protected void ValidateConnection()
	{
		if (PhotonNetwork.IsConnected) return;
		SceneManager.LoadScene(mainMenuSceneIndex);
	}

	public void TogglePause()
	{
		if (disconnecting) return;

		paused = !paused;

		UIManager.Instance.ShowPauseUI(paused);
		Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Confined;
		Cursor.visible = paused;
	}

	public void Quit()
	{
		disconnecting = true;
		PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene(mainMenuSceneIndex);
	}

	public void ReturnToMenu()
	{
		// disable room
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.DestroyAll();
		}

		Quit();
	}

	#endregion


	#region Coroutines



	#endregion
}
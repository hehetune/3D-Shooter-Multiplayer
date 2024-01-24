using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;
	public PlayerHUD playerHUD;
	public GameUI gameUI;

	private void Awake()
	{
		if (Instance == null) Instance = this;
	}

	public void ShowPauseUI(bool show)
	{
		gameUI.ui_pauseGame.SetActive(show);
	}
}

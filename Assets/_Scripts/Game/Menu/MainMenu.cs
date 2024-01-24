using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField usernameField;

    protected virtual void OnDisable()
    {
        Launcher.Instance.SyncPlayerInfo();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void JoinMatch()
    {
        Launcher.Instance.TabOpenRoomList();
    }

    public void CreateMatch()
    {
        Launcher.Instance.TabOpenCreateRoom();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
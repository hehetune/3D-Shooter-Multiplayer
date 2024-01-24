using Photon.Pun;
using UnityEngine;

public class PlayerInput : MonoBehaviourPunCallbacks
{
    public PlayerController playerController;

    [HideInInspector]
    public float h_move = 0;
    [HideInInspector]
    public float v_move = 0;

    [HideInInspector]
    public bool sprint;
    [HideInInspector]
    public bool jump;
    [HideInInspector]
    public bool slide;
    [HideInInspector]
    public bool aim;
    [HideInInspector]
    public bool crouch;
    [HideInInspector]
    public bool jet;

    public static bool cursorLocked = true;

    public void HandlePlayerInput()
    {
        if (!photonView.IsMine) return;

        UpdateCursorLock();
        HandleGamePlayInput();
    }

    private void HandleGamePlayInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.IsEnded)
        {
            GameManager.Instance.TogglePause();
        }

        if (Input.GetKey(KeyCode.Tab) && !GameManager.Instance.IsEnded)
        {
            UIManager.Instance.gameUI.ShowLeaderBoard(true);
        }
        else UIManager.Instance.gameUI.ShowLeaderBoard(false);

        //Pause
        if (GameManager.Instance.paused || playerController.playerStatus.IsDied || GameManager.Instance.IsEnded)
        {
            h_move = 0;
            v_move = 0;
            sprint = false;
            jump = false;
            crouch = false;
            slide = false;
            aim = false;

            return;
        }

        //Axis
        h_move = Input.GetAxisRaw("Horizontal");
        v_move = Input.GetAxisRaw("Vertical");

        //Controls
        sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        jump = Input.GetKeyDown(KeyCode.Space);
        slide = Input.GetKeyDown(KeyCode.E);
        crouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
        aim = Input.GetMouseButton(1);
        jet = Input.GetKey(KeyCode.Space);

        //test
        if (Input.GetKeyDown(KeyCode.U)) playerController.playerStatus.TakeDamage(500, -1);
    }

    private void UpdateCursorLock()
    {
        if (GameManager.Instance.IsEnded || GameManager.Instance.paused) cursorLocked = false;
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLocked = !cursorLocked;
        }
        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLocked;
    }
}
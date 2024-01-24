using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    #region Variables
    public PlayerMovement playerMovement;
    public PlayerLook playerLook;
    public PlayerWeapon playerWeapon;
    public PlayerStatus playerStatus;
    public PlayerInput playerInput;
    public PlayerAnimation playerAnimation;
    public PlayerDisplay playerDisplay;
    public ProfileData playerProfile;

    public Transform cameraParent;
    public Camera normalCam;
    public Camera weaponCam;

    public bool isMilkTeam;

    [SerializeField]
    private Transform modelTransform;
    [SerializeField]
    private Transform weaponTransform;

    #endregion

    #region Monobehaviour Callbacks

    private void Start()
    {
        // cameraParent.SetActive(photonView.IsMine);

        if (!photonView.IsMine)
        {
            LayerHelper.ChangeSpecificLayersRecursively(gameObject, LayerHelper.LocalPlayerLayer, LayerHelper.PlayerLayer);
            normalCam.gameObject.SetActive(false);
            weaponCam.gameObject.SetActive(false);
        }
        else
        {
            TrySync();
        }
    }

    private void Update()
    {
        playerInput.HandlePlayerInput();

        playerMovement.HandleMovementUpdate();

        playerLook.HandleLookUpdate();

        playerWeapon.HanldeWeaponUpdate();
    }

    private void LateUpdate()
    {
        playerMovement.HandleMovementLateUpdate();
    }

    private void FixedUpdate()
    {
        playerMovement.HandleMovementFixedUpdate();
    }

    #endregion

    #region Private Methods

    public void TrySync()
    {
        if (!photonView.IsMine) return;

        photonView.RPC(nameof(SyncProfile), RpcTarget.All, Launcher.myProfile.username, Launcher.myProfile.level, Launcher.myProfile.xp);

        // if (GameSettings.GameMode == GameMode.TDM)
        // {
        //     photonView.RPC("SyncTeam", RpcTarget.All, GameSettings.IsAwayTeam);
        // }
    }

    [PunRPC]
    private void SyncProfile(string p_username, int p_level, int p_xp)
    {
        Debug.Log("Sync Profile Received");
        playerProfile = new ProfileData(p_username, p_level, p_xp);
        UIManager.Instance.playerHUD.usernameText.text = Launcher.myProfile.username;
        playerDisplay.usernameTMP.text = playerProfile.username;
    }

    [PunRPC]
    private void SyncTeam(bool isMilkTeam)
    {
        this.isMilkTeam = isMilkTeam;

        if (this.isMilkTeam)
        {
            // ColorTeamIndicators(Color.red);
        }
        else
        {
            // ColorTeamIndicators(Color.blue);
        }
    }

    // private void ColorTeamIndicators(Color p_color)
    // {
    //     foreach (Renderer renderer in teamIndicators) renderer.material.color = p_color;
    // }

    #endregion

    #region Public Methods

    public void ShowPlayer(bool show)
    {
        modelTransform.gameObject.SetActive(show);
        weaponTransform.gameObject.SetActive(show);
    }

    #endregion
}
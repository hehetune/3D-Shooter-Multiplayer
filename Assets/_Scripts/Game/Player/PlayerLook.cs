using Photon.Pun;
using UnityEngine;

public class PlayerLook : MonoBehaviourPunCallbacks
{
    #region Variables
    public PlayerController playerController;
    public Transform weapon;

    public float xSensitivity;
    public float ySensitivity;
    public float maxAngle;

    private Quaternion centerAngle;
    // private Transform normalCam;
    private Transform cameraParent;
    [SerializeField]
    private float rotationSmoothTime = 10f;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        cameraParent = playerController.cameraParent;
        centerAngle = cameraParent.localRotation; //set rotation origin for cameras to camCenter
    }

    public void HandleLookUpdate()
    {
        if (playerController.playerStatus.IsDied || !photonView.IsMine || GameManager.Instance.IsEnded || GameManager.Instance.paused) return;
        SetY();
        SetX();
    }
    #endregion

    #region Private Methods
    void SetY()
    {
        // float t_input = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        // Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);
        // Quaternion t_delta = normalCam.localRotation * t_adj;

        // if (Quaternion.Angle(camCenter, t_delta) < maxAngle)
        // {
        //     playerController.normalCam.transform.localRotation = t_delta;
        // }

        // weapon.rotation = playerController.normalCam.transform.rotation;

        float mouseY = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.AngleAxis(mouseY, -Vector3.right);
        Quaternion newRotation = cameraParent.localRotation * deltaRotation;

        if (Quaternion.Angle(centerAngle, newRotation) < maxAngle)
        {
            cameraParent.localRotation = Quaternion.Slerp(cameraParent.localRotation, newRotation, rotationSmoothTime);
        }

        weapon.rotation = cameraParent.rotation;
    }

    void SetX()
    {
        // float t_input = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        // Quaternion t_adj = Quaternion.AngleAxis(t_input, Vector3.up);
        // Quaternion t_delta = playerController.transform.localRotation * t_adj;
        // playerController.transform.localRotation = t_delta;

        float mouseX = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion newRotation = playerController.transform.localRotation * deltaRotation;

        playerController.transform.localRotation = Quaternion.Slerp(playerController.transform.localRotation, newRotation, rotationSmoothTime);
    }
    #endregion
}
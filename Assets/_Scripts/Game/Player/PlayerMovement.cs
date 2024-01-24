using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    #region Variables
    public PlayerController playerController;

    //Movement Variables
    public float speed = 500;
    public float sprintModifier = 1.5f;
    public float jumpForce = 1000;
    public float slideTime = 5f;
    public float slideModifier = 2.5f;
    public float crouchModifier = 0.75f;
    public float jetForce;
    public float jetWait;
    public float jetRecovery;

    public float slideAmount;
    public float crouchAmount;
    public GameObject playerModel;

    //Ground Detect
    public Transform groundDetector;

    //State variables
    private bool isGrounded;
    private bool isJumping;
    private bool isSprinting;
    private bool isSliding;
    private bool isCrouching;
    private bool crouched;
    private float slideTimeLeft;
    private Vector3 slideDir;
    private bool canJet;
    private float current_recovery;

    private Rigidbody rig;

    //HeadBob
    private float movementCounter;
    private float idleCounter;
    private Vector3 targetWeaponBobPosition;
    private float headBobTimeModifier = 3f;

    //Weapon
    private Camera weaponCam;
    private Transform weaponParent;
    private Vector3 weaponParentOriginPosition;
    private Vector3 weaponParentCurrentPosition;

    //Camera
    private float baseFOV;
    private float sprintFOVModifier = 1.25f;
    private Camera normalCam;
    private Transform cameraParent;
    private Vector3 cameraParentOriginPosition;
    private Vector3 cameraParentCurrentPosition;

    private PlayerInput playerInput;
    private PlayerStatus playerStatus;
    private PlayerAnimation playerAnimation;

    #endregion



    #region Monobehaviour Callbacks
    private void Awake()
    {
        normalCam = playerController.normalCam;
        weaponCam = playerController.weaponCam;
        cameraParent = playerController.cameraParent;
        playerInput = playerController.playerInput;
        playerStatus = playerController.playerStatus;
        playerAnimation = playerController.playerAnimation;
        weaponParent = playerController.playerWeapon.weaponParent;
        baseFOV = normalCam.fieldOfView;
        weaponParentOriginPosition = weaponParent.transform.localPosition;
        weaponParentCurrentPosition = weaponParentOriginPosition;
        cameraParentOriginPosition = cameraParent.localPosition;
        cameraParentCurrentPosition = cameraParentOriginPosition;
        rig = transform.GetComponent<Rigidbody>();
        rig.useGravity = photonView.IsMine;
    }

    // private void Start()
    // {
    // }

    public void HandleMovementUpdate()
    {
        if (playerStatus.IsDied) return;

        // Sync my player on other client
        if (!photonView.IsMine)
        {
            //Camera Position
            if (slideTimeLeft > 0)
            {
                ChangeWeaponCurrentPosition(Vector3.down * slideAmount);
            }
            else if (crouched)
            {
                ChangeWeaponCurrentPosition(Vector3.down * crouchAmount);
            }
            else
            {
                ChangeWeaponCurrentPosition(Vector3.zero);
            }
            return;
        }

        //States
        isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.15f, LayerMaskHelper.GroundLayerMask);
        isJumping = playerInput.jump && isGrounded;
        isSprinting = playerInput.sprint && !isJumping && isGrounded && playerInput.v_move > 0;
        isSliding = isSprinting && playerInput.slide && !isSliding;
        isCrouching = playerInput.crouch && !isSprinting && !isJumping && isGrounded && slideTimeLeft <= 0;

        if (playerInput.aim) playerInput.aim &= !isSliding && !isSprinting;

        //Crouching
        if (isCrouching)
        {
            photonView.RPC(nameof(SetCrouch), RpcTarget.All, !crouched);
        }

        //Sliding
        if (isSliding)
        {
            slideDir = new Vector3(playerInput.h_move, 0, playerInput.v_move);
            slideDir.Normalize();
            slideTimeLeft = slideTime;
        }

        //Jumping
        if (isJumping)
        {
            rig.AddForce(Vector3.up * jumpForce);
            current_recovery = 0f;
        }

        //Jetting
        if (playerInput.jump && !isGrounded)
            canJet = true;
        if (isGrounded)
            canJet = false;

        if (canJet && playerInput.jet && playerStatus.current_fuel > 0)
        {
            rig.AddForce(jetForce * Time.deltaTime * Vector3.up, ForceMode.Acceleration);
            playerStatus.current_fuel = Mathf.Max(0, playerStatus.current_fuel - Time.deltaTime);
        }

        if (isGrounded)
        {
            if (current_recovery < jetWait)
                current_recovery = Mathf.Min(jetWait, current_recovery + Time.deltaTime);
            else
                playerStatus.current_fuel = Mathf.Min(playerStatus.max_fuel, playerStatus.current_fuel + Time.deltaTime * jetRecovery);
        }

        playerStatus.UpdateFuelBar();

        //Cancel crouch if meets conditions
        if (isSliding || isJumping || isSprinting)
        {
            if (crouched) photonView.RPC(nameof(SetCrouch), RpcTarget.All, false);
        }

        //Head Bob
        if (!isGrounded)
        {
            HeadBob(0f, 0f, 0f);
            headBobTimeModifier = 6f;
        }
        else if (slideTimeLeft > 0)
        {
            HeadBob(movementCounter, 0.2f, 0.1f);
            headBobTimeModifier = 5f;
            movementCounter += Time.deltaTime * headBobTimeModifier;
        }
        if (playerInput.h_move == 0 && playerInput.v_move == 0)
        {
            //idling
            HeadBob(idleCounter, 0.025f, 0.025f);
            headBobTimeModifier = 2f;
            idleCounter += Time.deltaTime * headBobTimeModifier;
        }
        else if (!isSprinting && !crouched)
        {
            //walking
            HeadBob(movementCounter, 0.035f, 0.035f);
            headBobTimeModifier = 3f;
            movementCounter += Time.deltaTime * headBobTimeModifier;
        }
        else if (crouched)
        {
            //crouching
            HeadBob(movementCounter, 0.02f, 0.02f);
            headBobTimeModifier = 1f;
            movementCounter += Time.deltaTime * headBobTimeModifier;
        }
        else if (isSprinting)
        {
            //sprinting
            HeadBob(movementCounter, 0.15f, 0.075f);
            headBobTimeModifier = 6f;
            movementCounter += Time.deltaTime * headBobTimeModifier;
        }

        //Camera Position
        if (slideTimeLeft > 0)
        {
            ChangeCameraCurrentPosition(Vector3.down * slideAmount);
            ChangeWeaponCurrentPosition(Vector3.down * slideAmount);
        }
        else if (crouched)
        {
            ChangeCameraCurrentPosition(Vector3.down * crouchAmount);
            ChangeWeaponCurrentPosition(Vector3.down * crouchAmount);
        }
        else
        {
            ChangeCameraCurrentPosition(Vector3.zero);
            ChangeWeaponCurrentPosition(Vector3.zero);
        }
    }

    public void HandleMovementLateUpdate()
    {
        if (playerStatus.IsDied) return;

        weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, weaponParentCurrentPosition, Time.deltaTime * 8f);
        if (!photonView.IsMine) return;

        weaponParent.localPosition += Vector3.Lerp(Vector3.zero, targetWeaponBobPosition, Time.deltaTime * headBobTimeModifier);
        cameraParent.localPosition = Vector3.Lerp(cameraParent.localPosition, cameraParentCurrentPosition, Time.deltaTime * 8f);
    }

    public void HandleMovementFixedUpdate()
    {
        if (!photonView.IsMine) return;

        //Movement
        Vector3 t_direction = new(playerInput.h_move, 0, playerInput.v_move);
        float t_adjustedSpeed = speed;

        //Pause
        if (GameManager.Instance.paused || playerStatus.IsDied)
        {
            t_adjustedSpeed = 0;
        }

        //Sliding
        if (slideTimeLeft > 0)
        {
            t_direction = slideDir;
            t_adjustedSpeed *= slideModifier;
            slideTimeLeft -= Time.fixedDeltaTime;
        }
        //Not Sliding
        else
        {
            t_direction.Normalize();
            //Sprinting
            if (isSprinting) t_adjustedSpeed *= sprintModifier;
            else if (isCrouching) t_adjustedSpeed *= crouchModifier;
        }

        //Set velocity
        Vector3 t_targetVelocity = t_adjustedSpeed * Time.fixedDeltaTime * transform.TransformDirection(t_direction);
        t_targetVelocity.y = rig.velocity.y;
        rig.velocity = t_targetVelocity;

        //Camera FOV
        if (slideTimeLeft > 0)
        {
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier * 1.15f, Time.fixedDeltaTime * 4f);
            weaponCam.fieldOfView = Mathf.Lerp(weaponCam.fieldOfView, baseFOV * sprintFOVModifier * 1.15f, Time.fixedDeltaTime * 4f);
        }
        else if (isSprinting)
        {
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier, Time.fixedDeltaTime * 4f);
            weaponCam.fieldOfView = Mathf.Lerp(weaponCam.fieldOfView, baseFOV * sprintFOVModifier, Time.fixedDeltaTime * 4f);
        }
        else if (playerController.playerWeapon.isAiming)
        {
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * playerController.playerWeapon.CurrentWeapon.adsFOV, Time.fixedDeltaTime * 4f);
            weaponCam.fieldOfView = Mathf.Lerp(weaponCam.fieldOfView, baseFOV * playerController.playerWeapon.CurrentWeapon.weaponAdsFOV, Time.fixedDeltaTime * 4f);
        }
        else
        {
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.fixedDeltaTime * 4f);
            weaponCam.fieldOfView = Mathf.Lerp(weaponCam.fieldOfView, baseFOV, Time.fixedDeltaTime * 4f);
        }

        if (photonView.IsMine)
        {
            //Animation
            playerAnimation.IsGrounded = isGrounded;
            playerAnimation.HorizontalSpeed = Mathf.Sqrt(rig.velocity.x * rig.velocity.x + rig.velocity.z * rig.velocity.z);
            playerAnimation.VerticalSpeed = rig.velocity.y;
            playerAnimation.ForwardSpeed = playerInput.v_move;
            playerAnimation.StrafeSpeed = playerInput.h_move;
        }
    }
    #endregion

    #region Private Methods
    private void HeadBob(float z, float x_intensity, float y_intensity)
    {
        float adjust = 1f;
        if (playerController.playerWeapon.isAiming)
        {
            adjust = 0.1f;
        }
        targetWeaponBobPosition = new Vector3(Mathf.Cos(z) * x_intensity * adjust, Mathf.Sin(z * 2) * y_intensity * adjust, 0f);
    }

    [PunRPC]
    private void SetCrouch(bool state)
    {
        if (crouched == state) return;
        crouched = state;
        if (crouched)
        {
            playerModel.transform.localScale = Vector3.one * 0.6f;
        }
        else
        {
            playerModel.transform.localScale = Vector3.one;
        }
    }

    private void ChangeCameraCurrentPosition(Vector3 verticalDir)
    {
        cameraParentCurrentPosition = cameraParentOriginPosition + verticalDir;
    }

    private void ChangeWeaponCurrentPosition(Vector3 verticalDir)
    {
        weaponParentCurrentPosition = weaponParentOriginPosition + verticalDir;
    }

    #endregion
}

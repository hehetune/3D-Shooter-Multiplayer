using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerWeapon : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Variables
    public PlayerController playerController;

    public List<GameObject> loadout = new();

    public Transform weaponParent;

    public bool isAiming = false;

    //Camera transform
    private Transform root;
    private Transform state_hip;
    private Transform state_ads;

    private List<Weapon> weapons = new();
    private int currentWeaponIndex;
    public Weapon CurrentWeapon => weapons[currentWeaponIndex];

    private float currentCooldown;

    private bool canAim;
    private float aimAngle;
    private float aimSpeed;

    private Coroutine reloadCoroutine;
    private Coroutine recoveryCoroutine;
    private Coroutine equipCoroutine;

    private PlayerInput playerInput;
    private Camera weaponCam;

    private bool scopeOn = false;
    #endregion

    #region Photon Callbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo message)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)(weaponParent.transform.localEulerAngles.x * 100f));
        }
        else
        {
            aimAngle = (int)stream.ReceiveNext() / 100f;
        }
    }

    #endregion


    #region Monobehaviour Callbacks
    private void Awake()
    {
        playerInput = playerController.playerInput;
        weaponCam = playerController.weaponCam;
        LoadWeapons();
        EquipRPC(0, true);
    }

    public void HanldeWeaponUpdate()
    {
        if (playerController.playerStatus.IsDied) return;

        if (photonView.IsMine)
        {
            if (GameManager.Instance.paused) return;
            if (Input.GetKeyDown(KeyCode.Alpha1))
                photonView.RPC(nameof(EquipRPC), RpcTarget.All, 0, false);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                photonView.RPC(nameof(EquipRPC), RpcTarget.All, 1, false);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                photonView.RPC(nameof(EquipRPC), RpcTarget.All, 2, false);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                photonView.RPC(nameof(EquipRPC), RpcTarget.All, 3, false);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                photonView.RPC(nameof(EquipRPC), RpcTarget.All, 4, false);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                photonView.RPC(nameof(EquipRPC), RpcTarget.All, 5, false);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                photonView.RPC(nameof(EquipRPC), RpcTarget.All, 6, false);
        }
        else
        {
            RefreshMultiplayerState();
        }

        if (CurrentWeapon != null && CurrentWeapon.gameObject.activeSelf)
        {
            if (photonView.IsMine)
            {
                HandleWeaponBehaviour();

                //cooldown
                if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
            }

            //weapon position elasticity
            CurrentWeapon.transform.localPosition = Vector3.Lerp(CurrentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 3f);
        }
    }
    #endregion

    #region Private Methods

    private void HandleWeaponBehaviour()
    {
        HandleAim();
        HandleAttack();
        HandleReload();
    }

    private void HandleAim()
    {
        SetAiming(playerInput.aim);

        //aim without scope
        if (isAiming)
        {
            //aim
            root.position = Vector3.Lerp(root.position, state_ads.position, Time.deltaTime * aimSpeed);
        }
        else
        {
            //hip
            root.position = Vector3.Lerp(root.position, state_hip.position, Time.deltaTime * aimSpeed);
        }

        //aim with scope
        if (!CurrentWeapon.aimWithScope) return;
        if (isAiming)
        {
            if ((root.position - state_ads.position).magnitude < 0.0025f)
            {
                SetShowScope(true);
            }
        }
        else
        {
            SetShowScope(false);
        }
    }

    private void SetShowScope(bool show)
    {
        if (show == scopeOn) return;

        scopeOn = show;
        weaponCam.gameObject.SetActive(!scopeOn);
        UIManager.Instance.playerHUD.ToggleScopeUI(scopeOn);
    }

    private void HandleAttack()
    {
        if (reloadCoroutine != null || recoveryCoroutine != null || equipCoroutine != null) return;

        if (CurrentWeapon.burst != 1)
        {
            if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
            {
                if (CurrentWeapon.CanAttack())
                    photonView.RPC(nameof(AttackRPC), RpcTarget.All);
                else if (CurrentWeapon.CanReload())
                    photonView.RPC(nameof(ReloadRPC), RpcTarget.All);
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && currentCooldown <= 0)
            {
                if (CurrentWeapon.CanAttack())
                    photonView.RPC(nameof(AttackRPC), RpcTarget.All);
                else if (CurrentWeapon.CanReload())
                    photonView.RPC(nameof(ReloadRPC), RpcTarget.All);
            }
        }
    }

    private void HandleReload()
    {
        if (!Input.GetKeyDown(KeyCode.R) || !CurrentWeapon.CanReload()) return;
        if (reloadCoroutine != null || equipCoroutine != null || recoveryCoroutine != null) return;
        photonView.RPC(nameof(ReloadRPC), RpcTarget.All);
    }

    private void LoadWeapons()
    {
        foreach (GameObject gun in loadout)
        {
            if (PoolManager.Get<PoolObject>(gun.GetComponent<Prefab>(), out var instance))
            {
                if (photonView.IsMine)
                {
                    LayerHelper.ChangeLayersRecursively(instance.gameObject, LayerHelper.LocalWeaponLayer);
                }
                instance.transform.SetPositionAndRotation(weaponParent.position, weaponParent.rotation);
                instance.transform.SetParent(weaponParent);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localEulerAngles = Vector3.zero;
                Weapon weapon = instance.gameObject.GetComponent<Weapon>();
                weapons.Add(weapon);
                weapon.Initialize(this);
                instance.gameObject.SetActive(false);
            }
        }
    }

    [PunRPC]
    private void ReloadRPC()
    {
        SetAiming(false);
        reloadCoroutine = StartCoroutine(ReloadCoroutine(CurrentWeapon.reloadTime));
    }
    IEnumerator ReloadCoroutine(float time)
    {
        CurrentWeapon.weaponAnimation.PlayAnim(WeaponAnimState.Reload, CurrentWeapon.reloadTime);

        yield return new WaitForSeconds(time);

        CurrentWeapon.Reload();
        reloadCoroutine = null;
    }

    [PunRPC]
    private void RecoveryRPC()
    {
        if (recoveryCoroutine != null) return;
        recoveryCoroutine = StartCoroutine(RecoveryCoroutine(CurrentWeapon.recoveryTime));
    }

    IEnumerator RecoveryCoroutine(float time)
    {
        CurrentWeapon.weaponAnimation.PlayAnim(WeaponAnimState.Recovery, CurrentWeapon.recoveryTime);

        yield return new WaitForSeconds(time);

        recoveryCoroutine = null;
    }

    [PunRPC]
    private void EquipRPC(int index, bool force = false)
    {
        if (index < 0 || index >= weapons.Count) return;
        if (!force && currentWeaponIndex == index) return;
        StopAllCoroutine();

        CurrentWeapon.UnEquip();
        currentWeaponIndex = index;
        CurrentWeapon.Equip();

        root = CurrentWeapon.transform.Find("Root");
        state_hip = CurrentWeapon.transform.Find("States/Hip");
        canAim = CurrentWeapon.CanAim();
        if (canAim)
        {
            state_ads = CurrentWeapon.transform.Find("States/ADS");
            aimSpeed = CurrentWeapon.aimSpeed;
        }

        equipCoroutine = StartCoroutine(EquipCoroutine(CurrentWeapon.equipTime));
    }

    IEnumerator EquipCoroutine(float time)
    {
        CurrentWeapon.weaponAnimation.PlayAnim(WeaponAnimState.Equip, CurrentWeapon.equipTime);

        yield return new WaitForSeconds(time);

        equipCoroutine = null;
    }

    private void StopAllCoroutine()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }
        if (equipCoroutine != null)
        {
            StopCoroutine(equipCoroutine);
            equipCoroutine = null;
        }
    }

    [PunRPC]
    private void AttackRPC()
    {
        //cooldown
        currentCooldown = CurrentWeapon.fireRate;

        Transform spawn = null;

        if (CurrentWeapon is ThrowableWeapon)
        {
            spawn = weaponParent;
        }
        else
        {
            spawn = playerController.cameraParent;
        }

        CurrentWeapon.Attack(photonView.IsMine, spawn, playerController.cameraParent.forward);

        if (!photonView.IsMine) return;

        //reload if out of ammo
        if (!CurrentWeapon.CanAttack() && CurrentWeapon.CanReload())
        {
            photonView.RPC(nameof(ReloadRPC), RpcTarget.All);
        }

        //recovery
        else if (CurrentWeapon.CanAttack() && CurrentWeapon.isRecovery)
        {
            photonView.RPC(nameof(RecoveryRPC), RpcTarget.All);
        }
    }

    private void RefreshMultiplayerState()
    {
        float cacheEulY = weaponParent.localEulerAngles.y;
        weaponParent.rotation = Quaternion.Slerp(weaponParent.rotation, Quaternion.identity * Quaternion.AngleAxis(aimAngle, Vector3.right), Time.deltaTime * 8f);
        weaponParent.localEulerAngles = new Vector3(weaponParent.localEulerAngles.x, cacheEulY, weaponParent.localEulerAngles.z);
    }

    [PunRPC]
    void PickupWeapon(string name)
    {
        Prefab prefab = WeaponLibrary.Weapons[name];

        if (PoolManager.Get<PoolObject>(prefab, out var instance))
        {
            if (photonView.IsMine) LayerHelper.ChangeLayersRecursively(instance.gameObject, LayerHelper.LocalWeaponLayer);
            instance.transform.SetPositionAndRotation(weaponParent.position, weaponParent.rotation);
            instance.transform.SetParent(weaponParent);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localEulerAngles = Vector3.zero;
            Weapon weapon = instance.gameObject.GetComponent<Weapon>();
            if (weapons.Count >= 2)
            {
                weapons[currentWeaponIndex].GetComponent<PoolObject>().ReturnToPool();
                weapons[currentWeaponIndex] = weapon;
                EquipRPC(currentWeaponIndex, true);
            }
            else
            {
                weapons.Add(weapon);
                EquipRPC(weapons.Count - 1);
            }
        }
    }

    #endregion

    #region Public Methods

    private void SetAiming(bool p_isAiming)
    {
        if (!CurrentWeapon || !canAim || reloadCoroutine != null || recoveryCoroutine != null || !CurrentWeapon.CanAttack())
        {
            isAiming = false;
            return;
        }
        isAiming = p_isAiming;
    }

    public void RefreshAmmo(int clip, int stash)
    {
        UIManager.Instance.playerHUD.ammoText.text = clip.ToString("D2") + " / " + stash.ToString("D2");
    }

    #endregion
}
using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;

    //Weapon statistics
    public int damage;
    public float fireRate;
    public float aimSpeed;
    public int burst;

    //equip
    public float equipTime;

    //reload
    public float reloadTime;

    //recovery
    public bool isRecovery;
    public float recoveryTime;

    //aim
    [Range(0, 1)] public float adsFOV;
    [Range(0, 1)] public float weaponAdsFOV;

    //drop
    public bool canDrop;
    public Prefab display;

    //Attack sound
    public AudioAsset attackSoundAsset;
    public float soundPitchRandomization;
    public float soundVolume;

    public WeaponAnimation weaponAnimation;
    public WeaponSway weaponSway;
    public LayerMask canBeAttackLayer;

    protected PlayerWeapon playerWeapon;

    public virtual void Initialize(PlayerWeapon playerWeapon)
    {
        this.playerWeapon = playerWeapon;
    }

    public virtual bool CanAim() => false;
    public bool aimWithScope;

    public virtual bool CanReload() { return false; }
    public virtual void Reload() { }
    // public Action OnWeaponReload;

    public abstract bool CanAttack();
    public abstract void Attack(bool isMine, Transform spawnPos, Vector3 direction);

    public virtual void Equip()
    {
        gameObject.SetActive(true);
        OnEquip();
    }
    public virtual void UnEquip()
    {
        gameObject.SetActive(false);
        OnUnEquip();
    }
    public virtual void OnEquip() { }
    public virtual void OnUnEquip() { }
}
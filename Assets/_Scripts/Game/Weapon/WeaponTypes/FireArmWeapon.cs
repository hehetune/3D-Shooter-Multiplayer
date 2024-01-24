using UnityEngine;

public abstract class FireArmWeapon : Weapon
{
    public int ammo;
    public int clipSize;

    public float bloom;
    public float recoil;
    public float kickback;

    private int stash; //current ammo
    private int clip; //current clip

    public Transform shootPoint;

    public override void Initialize(PlayerWeapon playerWeapon)
    {
        base.Initialize(playerWeapon);

        stash = ammo;
        clip = clipSize;
    }

    public bool FireBullet()
    {
        if (clip > 0)
        {
            clip -= 1;
            return true;
        }
        else return false;
    }

    public int GetStash() => stash;
    public int GetClip() => clip;

    public override bool CanAim() => true;

    public override bool CanAttack()
    {
        return clip > 0;
    }

    public override void Attack(bool isMine, Transform spawnPos, Vector3 direction)
    {
        if (clip > 0)
        {
            clip -= 1;
        }

        SpawnAttack(isMine, spawnPos, direction);

        //sound
        float pitch = 1 - soundPitchRandomization + Random.Range(-soundPitchRandomization, soundPitchRandomization);
        AudioManager.Play(attackSoundAsset, pitch, soundVolume);

        RefreshAmmo();
    }

    public virtual void SpawnAttack(bool isMine, Transform spawnPos, Vector3 direction) { }

    public override bool CanReload()
    {
        if (clip < clipSize && stash > 0) return true;
        return false;
    }
    public override void Reload()
    {
        stash += clip;
        clip = Mathf.Min(clipSize, stash);
        stash -= clip;
        // OnWeaponReload?.Invoke();
        RefreshAmmo();
    }

    private void RefreshAmmo()
    {
        playerWeapon?.RefreshAmmo(clip, stash);
    }

    public override void OnEquip()
    {
        base.OnEquip();

        RefreshAmmo();
    }
}
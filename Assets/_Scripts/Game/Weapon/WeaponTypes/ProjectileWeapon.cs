using UnityEngine;

public abstract class ProjectileWeapon : FireArmWeapon
{
    public Prefab bulletHolePrefab;
    public Prefab bulletImpactPrefab;
    public Prefab muzzleFlashPrefab;

    [SerializeField]
    protected int pellets;
    [SerializeField]
    protected float attackRange = 1000f;
    [SerializeField]
    protected float muzzleFlashLifeTime = 0.5f;
    public override void Attack(bool isMine, Transform spawnPos, Vector3 direction)
    {
        base.Attack(isMine, spawnPos, direction);

        //gun fx
        TriggerKickback();
        SpawnMuzzleFlash();
    }

    public virtual void TriggerKickback()
    {
        transform.Rotate(-recoil, 0, 0);
        transform.position -= transform.forward * kickback;
    }

    public virtual void SpawnMuzzleFlash()
    {
        PoolManager.Get<PoolObject>(muzzleFlashPrefab, out var instance);
        instance.transform.SetParent(shootPoint);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.LookAt(shootPoint.position + shootPoint.forward);
        instance.ReturnToPoolByLifeTime(muzzleFlashLifeTime);
    }
}
using UnityEngine;

public class RealProjectileWeapon : ProjectileWeapon
{
    public float projectileSpeed;
    public Prefab projectile;
    public Prefab bulletTrailPrefab;

    [SerializeField]
    protected float bulletTrailLifeTime = 0.5f;

    public virtual void SpawnBulletTrail(Vector3 targetPosition)
    {
        PoolManager.Get<PoolObject>(bulletTrailPrefab, out var instance);
        instance.gameObject.GetComponent<BulletTrail>().SetTargetPosition(shootPoint.position, targetPosition);
        instance.ReturnToPoolByLifeTime(bulletTrailLifeTime);
    }
}
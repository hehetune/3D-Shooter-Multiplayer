using UnityEngine;

public class RaycastProjectileWeapon : ProjectileWeapon
{
    // public Prefab bulletLinePrefab;
    public Prefab bulletTrailPrefab;
    
    [SerializeField]
    protected float bulletTrailLifeTime = 0.5f;

    public override void SpawnAttack(bool isMine, Transform spawnPos, Vector3 direction)
    {
        for (int i = 0; i < Mathf.Max(1, pellets); i++)
        {
            //bloom
            Vector3 bloomDir = spawnPos.position + direction.normalized * 1000f;
            Vector3 endPos = bloomDir;
            bloomDir += Random.Range(-bloom, bloom) * Vector3.Cross(direction, Vector3.right).normalized;
            bloomDir += Random.Range(-bloom, bloom) * Vector3.Cross(direction, Vector3.up).normalized;
            bloomDir -= spawnPos.position;
            bloomDir.Normalize();

            //raycast
            if (Physics.Raycast(spawnPos.position, bloomDir, out RaycastHit hit, 1000f, canBeAttackLayer))
            {
                if ((LayerMaskHelper.EnviromentLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    if (PoolManager.Get<PoolObject>(bulletHolePrefab, out var bulletHole))
                    {
                        bulletHole.transform.position = hit.point + hit.normal * 0.001f;
                        bulletHole.transform.LookAt(hit.point + hit.normal);
                        bulletHole.ReturnToPoolByLifeTime(5f);
                    }

                    if (PoolManager.Get<PoolObject>(bulletImpactPrefab, out var bulletImpact))
                    {
                        bulletImpact.transform.position = hit.point;
                        bulletImpact.transform.LookAt(hit.point + hit.normal);
                        bulletImpact.ReturnToPoolByLifeTime(1f);
                    }
                }
                else if (isMine)
                {
                    if (hit.collider.gameObject.layer == LayerHelper.PlayerLayer && hit.collider.gameObject.CompareTag("DamageReceiver"))
                    {
                        if (hit.collider.gameObject.TryGetComponent(out PlayerDamageReceiver playerDamageReceiver))
                        {
                            //give damage
                            if (playerDamageReceiver.TakeDamage(damage))
                            {
                                //show hitmarker
                                UIManager.Instance.playerHUD.ShowHitMarker();
                            }
                        }
                    }
                }
                SpawnBulletLine(hit.point);
            }
            else
            {
                SpawnBulletLine(endPos);
            }
        }
    }

    public virtual void SpawnBulletLine(Vector3 targetPosition)
    {
        PoolManager.Get<PoolObject>(bulletTrailPrefab, out var instance);
        instance.gameObject.GetComponent<BulletTrail>().SetTargetPosition(shootPoint.position, targetPosition);
        instance.ReturnToPoolByLifeTime(bulletTrailLifeTime);
    }
}
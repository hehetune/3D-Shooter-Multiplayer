using UnityEngine;

public class BombGrenade : ExplodeObject
{
    [SerializeField]
    private Prefab bombExplosion;

    private float existTime;
    private float explosionRadius = 3f;
    private LayerMask canBeAttackLayer;
    private int damage;
    private bool isMine;

    private void OnEnable()
    {
        CancelInvoke(nameof(Explode));
    }

    public override void Explode()
    {
        PoolManager.Get<PoolObject>(bombExplosion, out var instance);
        instance.transform.position = transform.position;
        instance.transform.rotation = Quaternion.identity;
        instance.ReturnToPoolByLifeTime(3f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, canBeAttackLayer);

        if (isMine)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.layer == LayerHelper.PlayerLayer && hitCollider.gameObject.CompareTag("DamageReceiver"))
                {
                    if (hitCollider.gameObject.TryGetComponent(out PlayerDamageReceiver playerDamageReceiver))
                    {
                        if (playerDamageReceiver.TakeDamage(damage))
                        {
                            UIManager.Instance.playerHUD.ShowHitMarker();
                        }
                    }
                }
            }
        }

        PlayExplodeSound();
        gameObject.GetComponent<PoolObject>().ReturnToPool();
    }

    public override void Throw(Vector3 direction, float force)
    {
        rb.AddForce(direction.normalized * force);
        Invoke(nameof(Explode), Mathf.Max(minDespawnTime, existTime));
    }

    public virtual void Setup(float minDespawnTime, int damage, float existTime, LayerMask canBeAttackLayer, bool isMine)
    {
        base.Setup(minDespawnTime);

        this.damage = damage;
        this.existTime = existTime;
        this.minDespawnTime = minDespawnTime;
        this.canBeAttackLayer = canBeAttackLayer;
        this.isMine = isMine;
    }
}
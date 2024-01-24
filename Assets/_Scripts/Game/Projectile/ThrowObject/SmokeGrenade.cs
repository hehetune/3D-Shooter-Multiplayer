using UnityEngine;

public class SmokeGrenade : ExplodeObject
{
    [SerializeField]
    private Prefab smokeExplosion;
    private float smokeDuration;
    private float startTime;
    private float landBeforeExplodeDuration = 0.25f;
    private float landTime = 0f;

    private bool exploded;

    private void OnEnable()
    {
        landTime = 0f;
        exploded = false;
    }

    private void FixedUpdate()
    {
        if (exploded) return;
        if (Time.time - startTime < minDespawnTime) return;
        if (landTime < landBeforeExplodeDuration)
        {
            if (Mathf.Abs(rb.velocity.y) < 0.05f)
            {
                landTime += Time.fixedDeltaTime;
            }
            return;
        }
        Explode();
    }

    public override void Explode()
    {
        exploded = true;
        PoolManager.Get<PoolObject>(smokeExplosion, out var instance);
        instance.transform.position = transform.position;
        instance.transform.rotation = Quaternion.identity;
        instance.ReturnToPoolByLifeTime(smokeDuration);
        PlayExplodeSound();
        gameObject.GetComponent<PoolObject>().ReturnToPool();
    }

    public virtual void Setup(float minDespawnTime, float smokeDuration)
    {
        base.Setup(minDespawnTime);
        this.smokeDuration = smokeDuration;
    }

    public override void Throw(Vector3 direction, float force)
    {
        startTime = Time.time;
        rb.AddForce(direction.normalized * force);
    }

}
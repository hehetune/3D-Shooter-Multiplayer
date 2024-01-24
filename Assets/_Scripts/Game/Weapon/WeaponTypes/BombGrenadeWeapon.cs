using UnityEngine;

public class BombGrenadeWeapon : ThrowableWeapon
{
    [SerializeField]
    private float existTime;

    public override void SpawnAttack(bool isMine, Transform spawnPos, Vector3 direction)
    {
        PoolManager.Get(objectPrefab, out throwObject);
        throwObject.transform.SetPositionAndRotation(shootPoint.position, Quaternion.identity);

        //sound
        float pitch = 1 - soundPitchRandomization + Random.Range(-soundPitchRandomization, soundPitchRandomization);
        AudioManager.Play(attackSoundAsset, pitch, soundVolume);

        BombGrenade bombGrenade = throwObject.gameObject.GetComponent<BombGrenade>();

        bombGrenade.Setup(minDespawnTime, damage, existTime, canBeAttackLayer, isMine);
        bombGrenade.Throw(direction, throwForce);

        dummyThrowObject.SetActive(CanAttack());
    }
}
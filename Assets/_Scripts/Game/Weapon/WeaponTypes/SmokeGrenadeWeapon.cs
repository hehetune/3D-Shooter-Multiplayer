using UnityEngine;

public class SmokeGrenadeWeapon : ThrowableWeapon
{
    [SerializeField]
    private float smokeDuration;

    public override void SpawnAttack(bool isMine, Transform spawnPos, Vector3 direction)
    {
        PoolManager.Get(objectPrefab, out throwObject);
        throwObject.transform.SetPositionAndRotation(shootPoint.position, Quaternion.identity);

        //sound
        float pitch = 1 - soundPitchRandomization + Random.Range(-soundPitchRandomization, soundPitchRandomization);
        AudioManager.Play(attackSoundAsset, pitch, soundVolume);

        SmokeGrenade smokeGrenade = throwObject.gameObject.GetComponent<SmokeGrenade>();
        smokeGrenade.Setup(minDespawnTime, smokeDuration);
        smokeGrenade.Throw(direction, throwForce);

        dummyThrowObject.SetActive(CanAttack());
    }
}
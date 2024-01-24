using UnityEngine;

public class ThrowableWeapon : FireArmWeapon
{
    public float throwForce;
    public float minDespawnTime;

    public Prefab objectPrefab;
    public GameObject dummyThrowObject;

    protected PoolObject throwObject;

    public override void SpawnAttack(bool isMine, Transform spawnPos, Vector3 direction)
    {
        PoolManager.Get(objectPrefab, out throwObject);
        throwObject.transform.SetPositionAndRotation(shootPoint.position, Quaternion.identity);

        //sound
        float pitch = 1 - soundPitchRandomization + Random.Range(-soundPitchRandomization, soundPitchRandomization);
        AudioManager.Play(attackSoundAsset, pitch, soundVolume);

        throwObject.gameObject.GetComponent<BaseThrowObject>().Throw(direction, throwForce);

        dummyThrowObject.SetActive(CanAttack());
    }

    public override void OnEquip()
    {
        base.OnEquip();
        dummyThrowObject.SetActive(CanAttack());
    }
}
using UnityEngine;

public abstract class ExplodeObject : BaseThrowObject
{
    public Rigidbody rb;
    protected float minDespawnTime;

    public AudioAsset explodeSoundAsset;
    public float soundPitchRandomization;
    public float soundVolume;

    public abstract void Explode();
    public virtual void Setup(float minDespawnTime)
    {
        this.minDespawnTime = minDespawnTime;
    }

    protected virtual void PlayExplodeSound()
    {
        float pitch = 1 - soundPitchRandomization + Random.Range(-soundPitchRandomization, soundPitchRandomization);
        AudioManager.Play(explodeSoundAsset, pitch, soundVolume);
    }
}
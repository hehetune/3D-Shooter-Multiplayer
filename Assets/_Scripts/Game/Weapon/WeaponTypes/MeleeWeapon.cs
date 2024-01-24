using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    private Coroutine attackCoroutine;

    [SerializeField]
    private float attackDelay;
    [SerializeField]
    private float attackDuration;
    [SerializeField]
    private Prefab hitPointPrefab;

    [SerializeField]
    private float comboAttackWait = 0.75f;

    [SerializeField]
    private float attackRange = 2f;

    public List<AnimationClip> attackAnimations;
    private int currentAttackAnimIndex = 0;
    private bool isComboAttack;

    private Coroutine comboAttackCoroutine;

    public override void Attack(bool isMine, Transform spawnPos, Vector3 direction)
    {
        if (attackCoroutine != null) return;
        attackCoroutine = StartCoroutine(AttackCoroutine(isMine, spawnPos, direction));
    }

    private IEnumerator AttackCoroutine(bool isMine, Transform spawnPos, Vector3 direction)
    {
        //sound
        float pitch = 1 - soundPitchRandomization + Random.Range(-soundPitchRandomization, soundPitchRandomization);
        AudioManager.Play(attackSoundAsset, pitch, soundVolume);

        if (isComboAttack)
        {
            currentAttackAnimIndex++;
            if (currentAttackAnimIndex == attackAnimations.Count) currentAttackAnimIndex = 0;
        }
        weaponAnimation.PlayAnim(attackAnimations[currentAttackAnimIndex].name, attackDuration);

        yield return attackDelay.Wait();

        SendDamage(isMine, spawnPos, direction);

        yield return (attackAnimations[currentAttackAnimIndex].length - attackDelay).Wait();

        attackCoroutine = null;
        TriggerComboAttack();
    }

    private void TriggerComboAttack()
    {
        if (comboAttackCoroutine != null) StopCoroutine(comboAttackCoroutine);
        comboAttackCoroutine = StartCoroutine(ComboAttackCoroutine());
    }

    private IEnumerator ComboAttackCoroutine()
    {
        isComboAttack = true;
        yield return comboAttackWait.Wait();
        isComboAttack = false;
        currentAttackAnimIndex = 0;
        comboAttackCoroutine = null;
    }

    protected virtual void SendDamage(bool isMine, Transform spawnPos, Vector3 direction)
    {
        //raycast
        if (Physics.Raycast(spawnPos.position, direction, out RaycastHit hit, attackRange, canBeAttackLayer))
        {
            if (PoolManager.Get<PoolObject>(hitPointPrefab, out var hitPoint))
            {
                hitPoint.transform.position = hit.point + hit.normal * 0.001f;
                hitPoint.transform.LookAt(hit.point + hit.normal);
                hitPoint.ReturnToPoolByLifeTime(5f);
            }

            if (isMine)
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
        }
    }

    public override bool CanAttack() => attackCoroutine == null;
}
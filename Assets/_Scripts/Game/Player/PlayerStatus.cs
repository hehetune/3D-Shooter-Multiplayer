using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
    #region Variables
    public PlayerController playerController;
    public Action OnPlayerDie;
    public float max_health;
    public float max_fuel;

    public float current_health;
    public float current_fuel;

    public bool IsDied => current_health <= 0;

    private Coroutine updateHealthBarCoroutine;
    private Coroutine updateFuelBarCoroutine;

    private HashSet<int> damagedActors = new();
    #endregion

    #region Monobehaviour Callbacks

    public override void OnEnable()
    {
        base.OnEnable();

        RevivePlayer();
    }

    #endregion

    #region Private Methods

    #endregion

    #region Public Methods


    [PunRPC]
    public void TakeDamage(int damage, int actor)
    {
        if (current_health <= 0) return;
        current_health = Mathf.Max(current_health - damage, 0);

        if (photonView.IsMine)
        {
            UpdateHealthBar();

            if (current_health <= 0)
            {
                GameManager.Instance.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, (byte)PlayerStat.Dead, 1);
                Die();

                if (actor >= 0)
                {
                    GameManager.Instance.ChangeStat_S(actor, (byte)PlayerStat.Kill, 1);
                }

                foreach (var kvp in damagedActors)
                {
                    GameManager.Instance.ChangeStat_S(kvp, (byte)PlayerStat.Assist, 1);
                }
            }
            else
            {
                if (!damagedActors.Contains(actor)) damagedActors.Add(actor);
            }
        }
        //test
        // else
        // {
        //     if (current_health <= 0)
        //     {
        //         if (actor >= 0)
        //         {
        //             GameManager.Instance.ChangeStat_S(actor, (byte)PlayerStat.Kill, 1);
        //         }

        //         OnPlayerDie?.Invoke();
        //         OnPlayerDie = null;
        //     }
        // }
    }

    public void UpdateFuelBar()
    {
        if (!photonView.IsMine) return;
        if (Mathf.Abs(UIManager.Instance.playerHUD.fuelBar.value - current_fuel / max_fuel) < Mathf.Epsilon) return;
        if (updateFuelBarCoroutine != null) StopCoroutine(updateFuelBarCoroutine);
        updateFuelBarCoroutine = StartCoroutine(UpdateFuelBarCoroutine());
    }
    #endregion

    #region Private Methods
    private void RevivePlayer()
    {
        current_health = max_health;
        current_fuel = max_fuel;
        UpdateHealthBar();
    }

    private void Die()
    {
        playerController.gameObject.SetActive(false);
        Invoke(nameof(Respawn), 2f);
    }

    private void Respawn()
    {
        playerController.gameObject.SetActive(true);
        RevivePlayer();
        GameManager.Instance.RespawnPlayer(playerController.gameObject);
    }

    private void UpdateHealthBar()
    {
        if (Mathf.Abs(UIManager.Instance.playerHUD.healthBar.value - current_health / max_health) < Mathf.Epsilon) return;
        if (updateHealthBarCoroutine != null) StopCoroutine(updateHealthBarCoroutine);
        updateHealthBarCoroutine = StartCoroutine(UpdateHealthBarCoroutine());
    }
    private IEnumerator UpdateHealthBarCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 0.25f)
        {
            elapsedTime += Time.deltaTime;
            UIManager.Instance.playerHUD.healthBar.value = Mathf.Lerp(UIManager.Instance.playerHUD.healthBar.value, current_health / max_health, elapsedTime / 0.25f);
            yield return null;
        }
        UIManager.Instance.playerHUD.healthBar.value = current_health / max_health;
    }
    private IEnumerator UpdateFuelBarCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 0.25f)
        {
            elapsedTime += Time.deltaTime;
            UIManager.Instance.playerHUD.fuelBar.value = Mathf.Lerp(UIManager.Instance.playerHUD.fuelBar.value, current_fuel / max_fuel, elapsedTime / 0.25f);
            yield return null;
        }
        UIManager.Instance.playerHUD.fuelBar.value = current_fuel / max_fuel;
    }
    #endregion
}
using Photon.Pun;
using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    public PlayerController playerController;
    public bool TakeDamage(int damage)
    {
        if (GameSettings.GameMode != GameMode.FFA && playerController.isMilkTeam != GameSettings.IsMilkTeam) return false;
        //test
        // playerController.playerStatus.TakeDamage(damage, PhotonNetwork.LocalPlayer.ActorNumber);
        playerController.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, damage, PhotonNetwork.LocalPlayer.ActorNumber);
        return true;
    }
}
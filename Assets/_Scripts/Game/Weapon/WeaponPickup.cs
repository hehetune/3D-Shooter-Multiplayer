using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponPickup : MonoBehaviourPunCallbacks
{
    public Weapon weapon;
    public float cooldown;
    public GameObject displayGO;
    public List<GameObject> targets;

    private PoolObject gunDisplay;
    private bool isDisabled;
    private float wait;

    private void Start()
    {
        if (gunDisplay != null)
        {
            gunDisplay.ReturnToPool();
            gunDisplay = null;
        }

        if (PoolManager.Get(weapon.display, out gunDisplay))
        {
            gunDisplay.transform.SetPositionAndRotation(displayGO.transform.position, displayGO.transform.rotation);
            gunDisplay.transform.SetParent(displayGO.transform);
        }
    }

    private void Update()
    {
        if (isDisabled)
        {
            if (wait > 0)
            {
                wait -= Time.deltaTime;
            }

            else
            {
                Enable();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if (other.attachedRigidbody.gameObject.CompareTag("Player"))
        {
            PlayerWeapon weaponController = other.attachedRigidbody.gameObject.GetComponent<PlayerWeapon>();
            weaponController.photonView.RPC("PickupWeapon", RpcTarget.All, weapon.name);
            photonView.RPC("Disable", RpcTarget.All);
        }
    }

    [PunRPC]
    public void Disable()
    {
        isDisabled = true;
        wait = cooldown;

        foreach (GameObject a in targets) a.SetActive(false);
    }

    private void Enable()
    {
        isDisabled = false;
        wait = 0;

        foreach (GameObject a in targets) a.SetActive(true);
    }
}

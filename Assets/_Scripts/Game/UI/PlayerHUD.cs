using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI usernameText;
    public Slider healthBar;
    public Slider fuelBar;
    public Image hitMarkerImage;

    public float hitMarkerWait = 0.25f;
    public AudioAsset hitMarkerAudioAsset;

    //crosshair
    public Image mainCrosshair;
    public Image scopeCrosshair;

    private Coroutine hitMarkerCoroutine;
    private float hitMarkerTimer;
    private Color TransparentColor = new(1, 1, 1, 0);

    private void Start()
    {
        hitMarkerImage.color = new Color(1, 1, 1, 0);
    }

    public void ShowHitMarker()
    {
        if (hitMarkerCoroutine != null) StopCoroutine(hitMarkerCoroutine);
        hitMarkerCoroutine = StartCoroutine(ShowHitMarkerCoroutine());
    }

    private IEnumerator ShowHitMarkerCoroutine()
    {
        // AudioManager.Play(hitMarkerAudioAsset);
        hitMarkerTimer = hitMarkerWait;

        while (hitMarkerTimer > 0)
        {
            float lerpFactor = 1 - (hitMarkerTimer / hitMarkerWait);
            hitMarkerImage.color = Color.Lerp(Color.white, TransparentColor, lerpFactor);

            hitMarkerTimer = Mathf.Max(0f, hitMarkerTimer - Time.deltaTime);
            yield return null;
        }

        hitMarkerImage.color = TransparentColor;
        hitMarkerCoroutine = null;
    }

    public void ToggleScopeUI(bool on)
    {
        mainCrosshair.gameObject.SetActive(!on);
        scopeCrosshair.gameObject.SetActive(on);
    }

    // public void ChangeKDA(PlayerMatchData data = null)
    // {
    //     if (data)
    //         ui_kda.text = data.kills.ToString() + "/" + data.deaths.ToString();
    //     else
    //         ui_kda.text = "0/0";
    // }

}

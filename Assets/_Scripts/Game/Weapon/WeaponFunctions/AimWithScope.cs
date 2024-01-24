using UnityEngine;

public class AimWithScope : MonoBehaviour
{
    public Canvas scopeCanvas;

    private bool aim;

    public void ToggleAim()
    {
        aim = !aim;
        scopeCanvas.gameObject.SetActive(aim);
    }
}
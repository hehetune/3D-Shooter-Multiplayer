using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    #region Variables
    public float intensity = 1;
    public float smooth = 10;

    private Quaternion origin_rotation;
    // private Quaternion target_rotation;
    // private Quaternion x_adj;
    // private Quaternion y_adj;

    // private float x_mouse;
    // private float y_mouse;
    #endregion


    #region Monobehaviour Callbacks
    private void Start()
    {
        origin_rotation = transform.localRotation;
    }
    private void Update()
    {
        if (GameManager.Instance.paused) return;
        UpdateSway();
    }
    #endregion

    #region Private Methods
    private void UpdateSway()
    {
        // //Controls
        // x_mouse = Input.GetAxis("Mouse X");
        // y_mouse = Input.GetAxis("Mouse Y");

        // if (!isMine)
        // {
        //     x_mouse = 0;
        //     y_mouse = 0;
        // }

        // //Calculate target rotation
        // x_adj = Quaternion.AngleAxis(-intensity * x_mouse, Vector3.up);
        // y_adj = Quaternion.AngleAxis(intensity * y_mouse, Vector3.right);
        // target_rotation = origin_rotation * x_adj * y_adj;

        //Rotate towards target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, origin_rotation, Time.deltaTime * smooth);
    }
    #endregion
}
using UnityEngine;

public static class LayerHelper
{
    public static readonly int DefaultLayer = LayerMask.NameToLayer("Default");

    public static readonly int PlayerLayer = LayerMask.NameToLayer("Player");
    public static readonly int LocalPlayerLayer = LayerMask.NameToLayer("LocalPlayer");

    public static readonly int GroundLayer = LayerMask.NameToLayer("Ground");
    public static readonly int WeaponLayer = LayerMask.NameToLayer("Weapon");
    public static readonly int LocalWeaponLayer = LayerMask.NameToLayer("LocalWeapon");

    public static void ChangeLayersRecursively(GameObject p_target, int p_layer)
    {
        p_target.layer = p_layer;
        foreach (Transform a in p_target.transform) ChangeLayersRecursively(a.gameObject, p_layer);
    }

    public static void ChangeSpecificLayersRecursively(GameObject target, int prevLayer, int afterLayer)
    {
        if (target.layer == prevLayer)
            target.layer = afterLayer;
        foreach (Transform a in target.transform) ChangeSpecificLayersRecursively(a.gameObject, prevLayer, afterLayer);
    }
}
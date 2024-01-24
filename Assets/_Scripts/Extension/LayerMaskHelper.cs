using UnityEngine;

public static class LayerMaskHelper
{
    public static readonly LayerMask GroundLayerMask = LayerMask.GetMask(new string[] { "Ground" });

    public static readonly LayerMask CanBeShotLayerMask = LayerMask.GetMask(new string[] { "Default", "Player", "Ground" });

    public static readonly LayerMask EnviromentLayerMask = LayerMask.GetMask(new string[] { "Ground", "Default" });
}
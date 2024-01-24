using Photon.Pun;

public abstract class RoomInfoPanel : MonoBehaviourPunCallbacks
{
    public GameMode gameMode;

    public abstract void Setup();

    public abstract void OnMasterClientSwitched();
}
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomPlayerInfo : MonoBehaviour
{
    public TextMeshProUGUI nameTMP;
    //level tmp

    public void Setup(Player player)
    {
        if (player.CustomProperties.ContainsKey("Username"))
            nameTMP.text = player.CustomProperties["Username"].ToString();
    }
}
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class LeaderBoard : MonoBehaviour
{
    public TextMeshProUGUI modeTMP;
    public TextMeshProUGUI mapTMP;

    public GameObject playerCardPrefab;
    public bool initialized = false;

    protected GameUI gameUI;
    protected List<PlayerCard> playerCards = new();

    public void Start()
    {
        gameUI = UIManager.Instance.gameUI;
    }

    public abstract List<PlayerMatchData> Sort(List<PlayerMatchData> playerMatchDatas);
    public virtual void Setup(List<PlayerMatchData> playerMatchDatas)
    {
        initialized = true;
    }
    public abstract void Refresh(List<PlayerMatchData> playerMatchDatas);
}
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TDMLeaderBoard : LeaderBoard
{
    public TextMeshProUGUI milkTMP;
    public TextMeshProUGUI chocoTMP;

    public Transform milkTeamContainer;
    public Transform chocoTeamContainer;

    public override void Setup(List<PlayerMatchData> playerMatchDatas)
    {
        base.Setup(playerMatchDatas);

        // clean up
        foreach(Transform t in milkTeamContainer)
        {
            Destroy(t.gameObject);
        }
        foreach(Transform t in chocoTeamContainer)
        {
            Destroy(t.gameObject);
        }

        // set details
        modeTMP.text = System.Enum.GetName(typeof(GameMode), GameSettings.GameMode);
        mapTMP.text = SceneManager.GetActiveScene().name;

        // set scores
        milkTMP.text = "0";
        chocoTMP.text = "0";

        // sort
        List<PlayerMatchData> sorted = Sort(playerMatchDatas);

        // display
        for (int i = 0; i <= sorted.Count; i++)
        {
            GameObject newcard = Instantiate(playerCardPrefab);
            PlayerCard playerCard = newcard.GetComponent<PlayerCard>();
            if (sorted[i].isMilkTeam) newcard.transform.SetParent(milkTeamContainer);
            else newcard.transform.SetParent(chocoTeamContainer);
            playerCards.Add(playerCard);
            playerCard.Setup(sorted[i], GameMode.TDM);
        }

        // activate
        gameUI.ui_leaderboard.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public override void Refresh(List<PlayerMatchData> playerMatchDatas)
    {
        // sort
        List<PlayerMatchData> sorted = Sort(playerMatchDatas);

        // display
        for (int i = 0; i <= sorted.Count; i++)
        {
            playerCards[i].Setup(sorted[i], GameMode.TDM);
        }

        // activate
        gameUI.ui_leaderboard.gameObject.SetActive(true);
    }

    public override List<PlayerMatchData> Sort(List<PlayerMatchData> playerMatchDatas)
    {
        List<PlayerMatchData> sorted = new();
        List<PlayerMatchData> homeSorted = new();
        List<PlayerMatchData> awaySorted = new();

        int homeSize = 0;
        int awaySize = 0;

        foreach (PlayerMatchData p in playerMatchDatas)
        {
            if (p.isMilkTeam) awaySize++;
            else homeSize++;
        }

        while (homeSorted.Count < homeSize)
        {
            // set defaults
            short highest = -1;
            PlayerMatchData selection = playerMatchDatas[0];

            // grab next highest player
            foreach (PlayerMatchData a in playerMatchDatas)
            {
                if (a.isMilkTeam) continue;
                if (homeSorted.Contains(a)) continue;
                if (a.kills > highest)
                {
                    selection = a;
                    highest = a.kills;
                }
            }

            // add player
            homeSorted.Add(selection);
        }

        while (awaySorted.Count < awaySize)
        {
            // set defaults
            short highest = -1;
            PlayerMatchData selection = playerMatchDatas[0];

            // grab next highest player
            foreach (PlayerMatchData a in playerMatchDatas)
            {
                if (!a.isMilkTeam) continue;
                if (awaySorted.Contains(a)) continue;
                if (a.kills > highest)
                {
                    selection = a;
                    highest = a.kills;
                }
            }

            // add player
            awaySorted.Add(selection);
        }

        sorted.AddRange(homeSorted);
        sorted.AddRange(awaySorted);
        return sorted;
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FFALeaderBoard : LeaderBoard
{
    public Transform playersContainer;
    public override void Setup(List<PlayerMatchData> playerMatchDatas)
    {
        base.Setup(playerMatchDatas);

        // clean up
        foreach (Transform t in playersContainer)
        {
            Destroy(t.gameObject);
        }

        // set details
        modeTMP.text = System.Enum.GetName(typeof(GameMode), GameSettings.GameMode);
        mapTMP.text = SceneManager.GetActiveScene().name;

        // sort
        List<PlayerMatchData> sorted = Sort(playerMatchDatas);

        // display
        for (int i = 0; i < sorted.Count; i++)
        {
            GameObject newcard = Instantiate(playerCardPrefab, playersContainer);
            PlayerCard playerCard = newcard.GetComponent<PlayerCard>();
            playerCards.Add(playerCard);
            playerCard.Setup(sorted[i], GameMode.FFA);
        }

        // activate
        gameObject.SetActive(true);
    }

    public override void Refresh(List<PlayerMatchData> playerMatchDatas)
    {
        // sort
        List<PlayerMatchData> sorted = Sort(playerMatchDatas);

        // display
        for (int i = 0; i <= sorted.Count; i++)
        {
            playerCards[i].Setup(sorted[i], GameMode.FFA);
        }

        // activate
        gameUI.ui_leaderboard.SetActive(true);
    }

    // public override List<PlayerMatchData> Sort(List<PlayerMatchData> playerMatchDatas)
    // {
    //     List<PlayerMatchData> sorted = new();
    //     while (sorted.Count < playerMatchDatas.Count)
    //     {
    //         // set defaults
    //         short highest = -1;
    //         PlayerMatchData selection = playerMatchDatas[0];

    //         // grab next highest player
    //         foreach (PlayerMatchData a in playerMatchDatas)
    //         {
    //             if (sorted.Contains(a)) continue;
    //             if (a.kills > highest)
    //             {
    //                 selection = a;
    //                 highest = a.kills;
    //             }
    //         }

    //         // add player
    //         sorted.Add(selection);
    //     }
    //     return sorted;
    // }

    public override List<PlayerMatchData> Sort(List<PlayerMatchData> playerMatchDatas)
    {
        return QuickSort(playerMatchDatas, 0, playerMatchDatas.Count - 1);
    }

    private List<PlayerMatchData> QuickSort(List<PlayerMatchData> playerMatchDatas, int left, int right)
    {
        if (left < right)
        {
            // Chọn pivot
            int pivotIndex = Partition(playerMatchDatas, left, right);

            // Sắp xếp bên trái pivot
            QuickSort(playerMatchDatas, left, pivotIndex - 1);

            // Sắp xếp bên phải pivot
            QuickSort(playerMatchDatas, pivotIndex + 1, right);
        }

        return playerMatchDatas;
    }

    private int Partition(List<PlayerMatchData> playerMatchDatas, int left, int right)
    {
        // Chọn phần tử pivot là phần tử đầu tiên
        var pivot = playerMatchDatas[left];

        // Duyệt qua danh sách từ đầu đến cuối
        int i = left + 1;
        for (int j = i; j <= right; j++)
        {
            // Nếu phần tử hiện tại nhỏ hơn pivot
            if (playerMatchDatas[j].kills < pivot.kills)
            {
                // Hoán đổi vị trí của hai phần tử
                (playerMatchDatas[j], playerMatchDatas[i]) = (playerMatchDatas[i], playerMatchDatas[j]);

                // Tăng i
                i++;
            }
        }

        // Hoán đổi vị trí của pivot và phần tử thứ i
        (playerMatchDatas[i - 1], playerMatchDatas[left]) = (playerMatchDatas[left], playerMatchDatas[i - 1]);

        // Trả về vị trí của pivot
        return i - 1;
    }
}
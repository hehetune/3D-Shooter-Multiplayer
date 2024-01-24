using UnityEngine;

public enum GameMode
{
    FFA = 0,
    TDM = 1,
    TEST = 2,
}

public class GameSettings : MonoBehaviour
{
    public static GameMode GameMode = GameMode.FFA;
    public static bool IsMilkTeam = false;
    public const int MAX_PLAYERS = 10;
}
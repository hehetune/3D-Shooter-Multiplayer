public class PlayerMatchData
{
    public ProfileData profile;
    public int actor;
    public short kills;
    public short deaths;
    public short assists;
    public bool isMilkTeam;

    public PlayerMatchData(ProfileData profile, int actor, short kills, short deaths, short assists, bool team = true)
    {
        this.profile = profile;
        this.actor = actor;
        this.kills = kills;
        this.deaths = deaths;
        this.assists = assists;
        isMilkTeam = team;
    }

    public void UpdateData(int actor, short kills, short deaths, short assists, bool team = true)
    {
        this.actor = actor;
        this.kills = kills;
        this.deaths = deaths;
        this.assists = assists;
        isMilkTeam = team;
    }
}
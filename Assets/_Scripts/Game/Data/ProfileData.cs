[System.Serializable]
public class ProfileData
{
    public string username;
    public int level;
    public int xp;

    public ProfileData()
    {
        username = "";
        level = 1;
        xp = 0;
    }

    public ProfileData(string u, int l, int x)
    {
        username = u;
        level = l;
        xp = x;
    }
}
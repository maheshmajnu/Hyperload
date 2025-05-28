using System.Collections.Generic;
using Photon.Realtime;

public class PlayerStatsManager
{
    public static Dictionary<int, PlayerStatsData> allStats = new Dictionary<int, PlayerStatsData>();

    public static PlayerStatsData Get(int actorNumber)
    {
        if (!allStats.ContainsKey(actorNumber))
            allStats[actorNumber] = new PlayerStatsData();

        return allStats[actorNumber];
    }

    public static void Clear()
    {
        allStats.Clear();
    }
}

public class PlayerStatsData
{
    public int kills = 0;
    public int deaths = 0;
    public int damageDone = 0;
}



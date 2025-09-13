public class PlayerStatisticsModel : IPlayerStatisticsModel
{
    private PlayerStatisticsData StatisticsData;

    public PlayerStatisticsModel(PlayerStatisticsData statisticsData)
    {
        StatisticsData = statisticsData;
    }

    public void AddGamePlayed(MinigameStatuses status)
    {
        StatisticsData.GamesPlayed++;
        
        if (status == MinigameStatuses.Success)
        {
            StatisticsData.GamesWon++;
        }
    }
}

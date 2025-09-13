public interface IPlayerProgressModel
{
    string GetName();
    void SetName(string name);
    IPlayerResourcesModel GetResources();
    IPlayerStatisticsModel GetStatistics();
    void Load();
    void Save();
    void Delete();
}

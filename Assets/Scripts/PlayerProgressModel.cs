using UnityEngine;

public class PlayerProgressModel : IPlayerProgressModel
{
    private const string Key = "PlayerData";

    private PlayerData PlayerData;
    // TODO implemet in this class?
    private IPlayerResourcesModel ResourcesModel;
    private IPlayerStatisticsModel StatisticsModel;

    public string GetName()
    {
        return PlayerData.Name;
    }

    public void SetName(string name)
    {
        PlayerData.Name = name;
    }

    public IPlayerResourcesModel GetResources()
    {
        return ResourcesModel;
    }

    public IPlayerStatisticsModel GetStatistics()
    {
        return StatisticsModel;
    }

    public void Load()
    {
        string jsonData = PlayerPrefs.GetString(Key, "");

        if (string.IsNullOrEmpty(jsonData))
        {
            PlayerData = new PlayerData
            {
                Resources = new PlayerResourcesData(),
                Statistics = new PlayerStatisticsData()
            };
        }
        else
        {
            PlayerData = JsonUtility.FromJson<PlayerData>(jsonData);
        }

        ResourcesModel = new PlayerResourcesModel(PlayerData.Resources);
        StatisticsModel = new PlayerStatisticsModel(PlayerData.Statistics);
    }

    public void Save()
    {
        string jsonData = JsonUtility.ToJson(PlayerData);
        PlayerPrefs.SetString(Key, jsonData);
        PlayerPrefs.Save();
    }

    public void Delete()
    {
        if (PlayerPrefs.HasKey(Key) == false)
        {
            PlayerPrefs.DeleteKey(Key);
        }

        Load();
    }
}

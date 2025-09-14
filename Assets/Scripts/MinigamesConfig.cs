using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "MinigamesConfig", menuName = "MiniGames/Config", order = 1)]
public class MinigamesConfig : ScriptableObject
{
    public List<MinigameConfig> Minigames;
}

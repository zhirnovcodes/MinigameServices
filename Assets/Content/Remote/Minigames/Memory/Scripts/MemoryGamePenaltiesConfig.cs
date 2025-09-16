using UnityEngine;

[CreateAssetMenu(menuName = "Minigames/Memory/Penalties Config", fileName = "PenaltiesConfig")]
public class MemoryGamePenaltiesConfig : ScriptableObject
{
    public MemoryGamePenaltyData[] Penalties;
}
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourcesModel : IPlayerResourcesModel
{
    private PlayerResourcesData ResourcesData;

    public PlayerResourcesModel(PlayerResourcesData resourcesData)
    {
        ResourcesData = resourcesData;
    }

    public int GetCash()
    {
        return ResourcesData.Cash;
    }

    public int GetDiamonds()
    {
        return ResourcesData.Diamonds;
    }

    public bool HasCharacter(CharacterCards character)
    {
        return ResourcesData.Characters.Contains(character);
    }

    public void GetCharacters(List<CharacterCards> result)
    {
        result.Clear();
        result.AddRange(ResourcesData.Characters);
    }

    public void AddCash(int amount)
    {
        ResourcesData.Cash += amount;
    }

    public void RemoveCash(int amount)
    {
        ResourcesData.Cash = Mathf.Max(0, ResourcesData.Cash - amount);
    }

    public void AddDiamonds(int amount)
    {
        ResourcesData.Diamonds += amount;
    }

    public void RemoveDiamonds(int amount)
    {
        ResourcesData.Diamonds = Mathf.Max(0, ResourcesData.Diamonds - amount);
    }

    public void AddCharacterCard(CharacterCards character)
    {
        if (!ResourcesData.Characters.Contains(character))
        {
            ResourcesData.Characters.Add(character);
        }
    }
}

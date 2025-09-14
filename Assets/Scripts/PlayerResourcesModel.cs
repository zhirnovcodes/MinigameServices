using System.Collections.Generic;
using System.Linq;
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
        foreach (var data in ResourcesData.Characters)
        {
            if (data.ID == character)
            {
                return true;
            }
        }

        return false;
    }

    public void GetCharacters(List<CharacterCardsData> result)
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

    public void AddCharacterCard(CharacterCards character, int countAdded)
    {
        for (int i = 0; i < ResourcesData.Characters.Count; i++)
        {
            if (ResourcesData.Characters[i].ID == character)
            {
                var count = ResourcesData.Characters[i].Count;
                ResourcesData.Characters[i] = new CharacterCardsData
                {
                    ID = character,
                    Count = count + countAdded
                };
                return;
            }
        }

        ResourcesData.Characters.Add(new CharacterCardsData
        {
            ID = character,
            Count = countAdded
        });
    }
}

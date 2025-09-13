using System.Collections.Generic;

public interface IPlayerResourcesModel
{
    int GetCash();
    int GetDiamonds();
    bool HasCharacter(CharacterCards character);
    void GetCharacters(List<CharacterCards> result);
    void AddCash(int amount);
    void RemoveCash(int amount);
    void AddDiamonds(int amount);
    void RemoveDiamonds(int amount);
    void AddCharacterCard(CharacterCards character);
}

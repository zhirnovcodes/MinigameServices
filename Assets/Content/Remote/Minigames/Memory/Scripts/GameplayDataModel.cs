public class GameplayDataModel
{
    private readonly GameplayData Data;

    public GameplayDataModel(GameplayData data)
    {
        Data = data;
    }

    public bool IsFirstSelected()
    {
        return Data.SelectedId1.HasValue;
    }

    public bool IsSecondSelected()
    {
        return Data.SelectedId2.HasValue;
    }

    public bool IsBothSelected()
    {
        return Data.SelectedId1.HasValue && Data.SelectedId2.HasValue;
    }

    public void SetFirstSelected(int index)
    {
        Data.SelectedId1 = index;
    }

    public void SetSecondSelected(int index)
    {
        Data.SelectedId2 = index;
    }

    public void DeselectAll()
    {
        Data.SelectedId1 = null;
        Data.SelectedId2 = null;
    }

    public int? GetFirstSelectedIndex()
    {
        return Data.SelectedId1;
    }

    public int? GetSecondSelectedIndex()
    {
        return Data.SelectedId2;
    }
}



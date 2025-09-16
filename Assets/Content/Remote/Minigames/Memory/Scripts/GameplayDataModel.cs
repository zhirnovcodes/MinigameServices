public class GameplayDataModel
{
    private readonly GameplayData Data;

    public GameplayDataModel(GameplayData data)
    {
        Data = data;
    }

    public bool IsFirstSelected()
    {
        return Data.SelectedIndex1.HasValue;
    }

    public bool IsSecondSelected()
    {
        return Data.SelectedIndex2.HasValue;
    }

    public bool IsBothSelected()
    {
        return Data.SelectedIndex1.HasValue && Data.SelectedIndex2.HasValue;
    }

    public bool IsSameSelected()
    {
        return Data.SelectedIndex1.HasValue && Data.SelectedIndex2.HasValue && Data.SelectedIndex1.Value == Data.SelectedIndex2.Value;
    }

    public void SetFirstSelected(int index)
    {
        Data.SelectedIndex1 = index;
    }

    public void SetSecondSelected(int index)
    {
        Data.SelectedIndex2 = index;
    }

    public void DeselectAll()
    {
        Data.SelectedIndex1 = null;
        Data.SelectedIndex2 = null;
    }

    public int? GetFirstSelectedIndex()
    {
        return Data.SelectedIndex1;
    }

    public int? GetSecondSelectedIndex()
    {
        return Data.SelectedIndex2;
    }
}



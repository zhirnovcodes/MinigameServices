using TMPro;
using UnityEngine;

public class CharacterCardMiniView : MonoBehaviour
{
    public Transform ImagePlaceholder;
    public TextMeshProUGUI CountText;

    public void SetParent(Transform child)
    {
        transform.SetParent(child, false);
    }

    public Transform GetPlaceholder()
    {
        return ImagePlaceholder;
    }

    public void SetCount(int text)
    {
        CountText.text = text.ToString();
    }
}

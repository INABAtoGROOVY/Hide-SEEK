using TMPro;
using UnityEngine;

public class ItemCountFrameView : MonoBehaviour
{
    public void Apply(int currentValue, int goalValue)
    {
        _text.text = $"{currentValue}/{goalValue}";
    }

    [SerializeField]
    private TextMeshProUGUI _text;
}

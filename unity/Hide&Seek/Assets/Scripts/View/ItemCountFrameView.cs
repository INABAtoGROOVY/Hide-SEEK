using TMPro;
using UnityEngine;

public class ItemCountFrameView : MonoBehaviour
{
    public void Apply(int currentValue, int goalValue)
    {
        text.text = $"{currentValue}/{goalValue}";
    }

    [SerializeField]
    private TextMeshProUGUI text;
}

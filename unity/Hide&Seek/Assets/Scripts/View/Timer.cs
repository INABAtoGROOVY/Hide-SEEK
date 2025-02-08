using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public void ApplyTime(int time)
    {
        text.text = $"{Mathf.FloorToInt(time / 60):D2}:{time % 60:D2}";
    }

    [SerializeField]
    private TextMeshProUGUI text;
}

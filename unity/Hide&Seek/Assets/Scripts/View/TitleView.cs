using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleView : MonoBehaviour
{
    public void Initialize(Action<bool> startCallback)
    {
        _startButton.onClick.AddListener(() => startCallback(false));
        _replayButton.onClick.AddListener(() => startCallback(true));
    }

    [SerializeField]
    Button _startButton = default;
    [SerializeField]
    Button _replayButton = default;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        // TODO  ボタンを押したときの処理を実装する
        _inGameView.Initialize(
            onClickActionButton: () => Debug.Log("on click action button"),
            onClickHideButton: () => Debug.Log("on click hide button")
        );

        _inGameSequence.Initialize(_inGameView, _unit, _3dCamera);

        _mainLoop = _inGameSequence.InGameExcecute();
        StartCoroutine(_mainLoop);
    }

    [SerializeField]
    private InGameView _inGameView = default;
    [SerializeField]
    private Unit _unit = default;
    [SerializeField]
    private Camera _3dCamera = default;

    IEnumerator _mainLoop;
    InGameSequence _inGameSequence = new InGameSequence();
}

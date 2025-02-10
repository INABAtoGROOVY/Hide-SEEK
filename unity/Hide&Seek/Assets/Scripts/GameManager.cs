using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        GameInitialize();

        _titleView.gameObject.SetActive(true);
        _titleView.Initialize(GameStart);

        _inGameSequence = GetComponent<InGameSequence>();
    }

    private void GameInitialize()
    {
        Application.targetFrameRate = 60;

        SoundManager.Instance.Initialize();
        SoundManager.Instance.PlayBGM(BGMData.BGMType.Title, false);
    }

    private void GameStart(bool isReplay)
    {
        _titleView.gameObject.SetActive(false);

        RecorderSystem.Instance.isReplay = isReplay;

        _inGameSequence.Initialize();
        _mainLoop = _inGameSequence.InGameExecute();
        StartCoroutine(_mainLoop);
    }

    [SerializeField]
    private TitleView _titleView = default;

    InGameSequence _inGameSequence;
    IEnumerator _mainLoop;

}

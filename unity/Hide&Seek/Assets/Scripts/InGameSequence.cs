using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSequence : MonoBehaviour
{
    public enum SequenceType
    {
        None,
        Init,
        Wait,
        Game,
        Finish
    }

    public void Initialize()
    {
        _sequenceType = SequenceType.Init;
        _isInGameEnd = false;

        _unit.Initialze(_inGameView, _3dCamera, _hideManager, SetFinish);

        _itemManager.Initalize(_inGameView.SetItemView, SetFinish);
        _hideManager.Initialize(_unit.modelTransform);
        _enemyUnitManager.Initialize(_unit.modelTransform);

        _inGameView.SetActiveInGameUI(true);
        _inGameView.SetTimerUI((int)GAME_TIME_LIMIT);

        if(RecorderSystem.Instance.isReplay)
        {
            RecorderSystem.Instance.LoadRecord("test");
        }
        else
        {
            RecorderSystem.Instance.StartRecord("test", 0f);
        }
    }

    public IEnumerator InGameExecute()
    {
        SoundManager.Instance.PlayBGM(BGMData.BGMType.InGame);

        while (!_isInGameEnd)
        {
            switch (_sequenceType)
            {
                case SequenceType.Init:
                    _sequenceType = SequenceType.Game;
                    break;
                case SequenceType.Wait:
                    break;
                case SequenceType.Game:
                    GameTimerExecute();
                    _unit.Excecute();
                    _itemManager.Execute();
                    _hideManager.Execute();
                    _enemyUnitManager.Execute();
                    break;
                case SequenceType.Finish:
                    if(!_isInGameEnd)
                    {
                        _isInGameEnd = true;

                        if (!RecorderSystem.Instance.isReplay)
                        {
                            RecorderSystem.Instance.SaveRecord();
                        }

                        _inGameView.SetActiveInGameUI(false);

                        _resultView.gameObject.SetActive(true);
                        _resultView.Setup(_isSuccess, Mathf.CeilToInt(GAME_TIME_LIMIT - _gameTimer), _itemManager.itemGetCount, _itemManager.itemGetLimit);
                    }
                    break;
            }

            yield return null;
        }

        yield break;
    }

    private void GameTimerExecute()
    {
        _gameTimer += Time.deltaTime;

        _unit.unitController.SetGameTime(_gameTimer);

        _inGameView.SetTimerUI(Mathf.CeilToInt(GAME_TIME_LIMIT - _gameTimer));

        if(_gameTimer >= GAME_TIME_LIMIT)
        {
            // タイムリミット判定
            SetFinish(false);
        }
    }

    private void SetFinish(bool isSuccess)
    {
        _sequenceType = SequenceType.Finish;
        _isSuccess = isSuccess;
    }

    private SequenceType _sequenceType = SequenceType.None;
    private bool _isInGameEnd;
    private float _gameTimer;
    private bool _isSuccess;

    private const float GAME_TIME_LIMIT = 60.0f;

    [SerializeField]
    private InGameView _inGameView = default;
    [SerializeField]
    private Unit _unit = default;
    [SerializeField]
    private Camera _3dCamera = default;
    [SerializeField]
    private ItemManager _itemManager = default;
    [SerializeField]
    private HideManager _hideManager = default;
    [SerializeField]
    private EnemyUnitManager _enemyUnitManager = default;
    [SerializeField]
    private ResultView _resultView = default;
}

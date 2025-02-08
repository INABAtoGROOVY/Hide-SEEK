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

        _unit.Initialze(_inGameView, _3dCamera, _hideManager);

        _itemManager.Initalize(_inGameView.SetItemView);
        _hideManager.Initialize(_unit.modelTransform);
        _enemyUnitManager.Initialize(_unit.modelTransform);

        _inGameView.SetTimerUI((int)GAME_TIME_LIMIT);

        RecorerSystem.Instance.StartRecord("test");
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
                    _unit.Excecute();
                    _itemManager.Execute();
                    _hideManager.Execute();
                    _enemyUnitManager.Execute();
                    GameTimerExecute();
                    break;
                case SequenceType.Finish:
                    break;
            }

            yield return null;
        }

        yield break;
    }

    private void GameTimerExecute()
    {
        _gameTimer += Time.deltaTime;

        //_unit.unitController.SetGameTime(_gameTimer);

        _inGameView.SetTimerUI(Mathf.CeilToInt(GAME_TIME_LIMIT - _gameTimer));

        if(_gameTimer >= GAME_TIME_LIMIT)
        {
            SetFinish();
        }
    }

    private void SetFinish()
    {
        _sequenceType = SequenceType.Finish;
    }

    private SequenceType _sequenceType = SequenceType.None;
    private bool _isInGameEnd;
    private float _gameTimer;

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
}

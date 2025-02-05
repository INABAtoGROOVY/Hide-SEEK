using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSequence : MonoBehaviour
{
    public enum SequenceType
    {
        Init,
        Wait,
        Game,
        Finish
    }

    public void Initialize(InGameView view, Unit unit, Camera gameCamera, ItemManager itemManager)
    {
        _sequenceType = SequenceType.Init;
        _isInGameEnd = false;

        _view = view;
        _unit = unit;
        _unit.Initialze(_view, gameCamera);

        _itemManager = itemManager;
        _itemManager.Initalize();
    }

    public IEnumerator InGameExcecute()
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
                    _itemManager.Excecute();
                    break;
                case SequenceType.Finish:
                    break;
            }

            yield return null;
        }

        yield break;
    }

    private SequenceType _sequenceType;
    private bool _isInGameEnd;

    private InGameView _view;
    private Unit _unit;
    private Camera _3dCamera;
    private ItemManager _itemManager;
}

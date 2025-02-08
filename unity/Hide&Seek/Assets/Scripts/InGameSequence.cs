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

        _unit.Initialze(_inGameView, _3dCamera);

        _itemManager.Initalize();
        _hideManager.Initialize(_unit.modelTransform);
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
                    _hideManager.Excecute();
                    break;
                case SequenceType.Finish:
                    break;
            }

            yield return null;
        }

        yield break;
    }

    private SequenceType _sequenceType = SequenceType.None;
    private bool _isInGameEnd;

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
}

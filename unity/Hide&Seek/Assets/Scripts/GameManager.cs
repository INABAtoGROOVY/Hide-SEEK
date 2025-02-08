using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        _inGameSequence = GetComponent<InGameSequence>();
        _inGameSequence.Initialize();

        _mainLoop = _inGameSequence.InGameExecute();
        StartCoroutine(_mainLoop);
    }

    InGameSequence _inGameSequence;
    IEnumerator _mainLoop;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSequence : MonoBehaviour
{
    public enum SequenceType
    {
        None,
        Title,
        InGame,
        Result
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator Excecute()
    {
        SoundManager.Instance.PlayBGM(BGMData.BGMType.InGame);

        yield break;
    }

    
}

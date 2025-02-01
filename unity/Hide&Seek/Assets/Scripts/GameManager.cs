using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // TODO  ボタンを押したときの処理を実装する
        inGameView.Initialize(
            onClickActionButton: () => Debug.Log("on click action button"),
            onClickHideButton: () => Debug.Log("on click hide button")
        );
        StartCoroutine(_sequence.Excecute());
    }

    // Update is called once per frame
    void Update()
    {

    }


    [SerializeField]
    private InGameView inGameView;

    GameSequence _sequence = new GameSequence();
}

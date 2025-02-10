using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    public void Setup(bool isSuccess, int time, int item, int itemLimit)
    {
        string result = "Result : ";
        if(isSuccess)
        {
            result += "Success";
        }
        else
        {
            result += "Failed";
        }
        _resultText.text = result;

        _timeText.text = "Time : " +  $"{Mathf.FloorToInt(time / 60):D2}:{time % 60:D2}";
        _itemText.text = "Get Item : " + item.ToString() + " / " + itemLimit.ToString();

        _reloadButton.onClick.AddListener(() => BackTitle());
    }

    private void BackTitle()
    {
        // 現在のシーンを取得
        string currentSceneName = SceneManager.GetActiveScene().name;

        // シーンを再読み込み
        SceneManager.LoadScene(currentSceneName);
    }

    [SerializeField]
    private TextMeshProUGUI _resultText;
    [SerializeField]
    private TextMeshProUGUI _timeText;
    [SerializeField]
    private TextMeshProUGUI _itemText;
    [SerializeField]
    private Button _reloadButton;

}

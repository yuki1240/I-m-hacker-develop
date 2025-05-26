using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] Text resultText;

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void Start()
    {
        resultText.text = GetResultText();
    }

    string GetResultText()
    {
        if (gameManager.player.Winflag)
        {
            return "プレイヤーの勝ちです";
        }
        else if (gameManager.enemy.Winflag)
        {
            return "敵の勝ちです";
        }
        else
        {
            return "??????";
        }
    }

    public void PushRestartButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void PushTitleButton()
    {
        SceneManager.LoadScene("Title");
    }
}

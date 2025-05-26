using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TMPro;

public class InputFieldManager : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text password;
    [SerializeField] PasswordInputPanelAnimation passwordInputPanelAnimation;

    GameManager gameManager;

    protected void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void Start()
    {
        inputField = inputField.GetComponent<TMP_InputField>();
        password = password.GetComponent<TMP_Text>();
        password.text = "";
    }

    /// <summary>
    /// ひらがなのみであるか判定
    /// </summary>
    /// <param name="str">判定する文字列</param>
    /// <returns>ひらがなのみであれば true</returns>
    bool IsHiraganaOnly(string str)
    {
        return Regex.IsMatch(str, @"^\p{IsHiragana}*$");
    }

    // 入力された文字がひらがなのみである場合に、passwordに入力された文字を代入して保持
    public void CanInputText(string inputText)
    {
        Debug.Log(inputText);
        if (IsHiraganaOnly(inputText))
        {
            password.text = inputText;

        }
        else
        {
            inputField.text = password.text;
        }
    }

    // パスワード入力フィールドに入力されたパスワードを取得
    public void PushButton()
    {
        if (inputField.text.Length != GameManager.Define.passwordLength)
        {
            inputField.text = "";
            return;
        }

        gameManager.player.Password = inputField.text;

        // プレイヤーが選択したパスワードカードを取得
        PasswordCardController[] playerSelectedPasswordCardList = gameManager.GetPlayerPasswordCards(true);
        // プレイヤーが選択したパスワードカードが正しいか判定
        // gameManager.IsCorrectPassword(playerSelectedPasswordCardList, gameManager.player.Password);

        // プレイヤーが選択したパスワードカードが正しい場合
        if (gameManager.InputCorrectPasswordflag)
        {
            return;
        }

        inputField.text = "";
    }

    /// <summary>
    /// パスワード決定ボタンが押されたときの処理
    /// </summary>
    public void PushConfirmButton()
    {
        // パスワードが正しいか判定
        if (inputField.text.Length != GameManager.Define.passwordLength)
        {
            inputField.text = "";
            return;
        }

        // プレイヤーが入力したパスワードが敵のパスワードと一致するか判定
        string inputEnenyPassword = inputField.text;
        for (int i = 0; i < GameManager.Define.passwordLength; i++)
        {
            if (inputEnenyPassword[i] != gameManager.enemy.Password[i])
            {
                inputField.text = "";
                gameManager.InputCorrectPasswordflag = true;
                gameManager.player.HackingFailedflag = true;
                return;
            }
        }

        gameManager.player.Winflag = true;
        gameManager.InputCorrectPasswordflag = true;
    }

    // パスワード入力フィールドがアクティブになったとき
    void OnEnable()
    {
        inputField.text = "";
        password.text = "";
        passwordInputPanelAnimation.ShowAnimation();
    }

    // パスワード入力フィールドが非アクティブになったとき
    void OnDisable()
    {
        passwordInputPanelAnimation.HideAnimation();
    }
}

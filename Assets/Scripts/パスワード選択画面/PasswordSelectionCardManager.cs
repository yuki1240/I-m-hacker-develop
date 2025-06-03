using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class PasswordSelectionCardManager : MonoBehaviour
{
    [HideInInspector] public TMP_Text explanationText;
    [SerializeField] public GameObject zoomBackPanel;
    [SerializeField] List<GameObject> cardPrefabs;
    [SerializeField] Transform[] cardPositions;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button submitButton;
    [SerializeField] GameManager gameManager;

    //フィールドにパスワードを生成
    [SerializeField] Transform playerPasswordFieldTransform;
    // [SerializeField] Transform enemyPasswordFieldTransform;

    // パスワードカードのプレハブ
    [SerializeField] PasswordCardController passwordCardPrefab;

    private GameObject[] tmpCard = new GameObject[3];
    private Dictionary<string, GameObject> cardDictionary = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedCards = new List<GameObject>();
    private String[] spawnKeyWords = new String[3];
    private int[] cardIds = new int[3];

    // 1つ前の入力値
    private string previousInput = "";

    private Dictionary<char, string> kanaGroups = new Dictionary<char, string>()
    {
        {'あ', "あ"}, {'い', "あ"}, {'う', "あ"}, {'え', "あ"}, {'お', "あ"},
        {'か', "か"}, {'き', "か"}, {'く', "か"}, {'け', "か"}, {'こ', "か"},
        {'さ', "さ"}, {'し', "さ"}, {'す', "さ"}, {'せ', "さ"}, {'そ', "さ"},
        {'た', "た"}, {'ち', "た"}, {'つ', "た"}, {'て', "た"}, {'と', "た"},
        {'な', "な"}, {'に', "な"}, {'ぬ', "な"}, {'ね', "な"}, {'の', "な"},
        {'は', "は"}, {'ひ', "は"}, {'ふ', "は"}, {'へ', "は"}, {'ほ', "は"},
        {'ま', "ま"}, {'み', "ま"}, {'む', "ま"}, {'め', "ま"}, {'も', "ま"},
        {'や', "や"}, {'ゆ', "や"}, {'よ', "や"},
        {'ら', "ら"}, {'り', "ら"}, {'る', "ら"}, {'れ', "ら"}, {'ろ', "ら"},
        {'わ', "わ"}, {'を', "わ"}, {'ん', "わ"},
        {'が', "か"}, {'ぎ', "か"}, {'ぐ', "か"}, {'げ', "か"}, {'ご', "か"},
        {'ざ', "さ"}, {'じ', "さ"}, {'ず', "さ"}, {'ぜ', "さ"}, {'ぞ', "さ"},
        {'だ', "た"}, {'ぢ', "た"}, {'づ', "た"}, {'で', "た"}, {'ど', "た"},
        {'ば', "は"}, {'び', "は"}, {'ぶ', "は"}, {'べ', "は"}, {'ぼ', "は"},
        {'ぱ', "は"}, {'ぴ', "は"}, {'ぷ', "は"}, {'ぺ', "は"}, {'ぽ', "は"}
    };

    void Start()
    {
        // プレハブリストを辞書に登録
        foreach (var prefab in cardPrefabs)
        {
            cardDictionary[prefab.name] = prefab;
        }

        // 初期状態で3枚のカードを配置
        SetInitialCards();

        // カードの説明文オブジェを取得
        explanationText = zoomBackPanel.transform.Find("ExplanationText").GetComponent<TMP_Text>();

        StartCoroutine(DelayedShow());

        // ZoomBackPanelにクリックイベントを追加
        EventTrigger trigger = zoomBackPanel.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { OnZoomBackPanelClick(); });
        trigger.triggers.Add(entry);

        inputField.onValueChanged.AddListener(OnTextEdit);
        submitButton.onClick.AddListener(OnSubmit);
    }

    /// <summary>
    /// 「決定」ボタンクリック時の処理
    /// </summary>
    public void OnSubmit()
    {
        // 3文字入力されているかを確かめて、正しい場合はGameManagerに通知
        if (inputField.text.Length == 3)
        {
            gameManager.InputCorrectPasswordflag = true;

            // "5"はパスワードカードを示す
            int cardType = 5;

            // プレハブを生成し、取得した文字でプレハブを初期化していけばOKじゃね？？
            SeachCardId();
            for (int i = 0; i < spawnKeyWords.Length; i++)
            {
                PasswordCardController passwordCard = Instantiate(passwordCardPrefab);
                passwordCard.transform.SetParent(playerPasswordFieldTransform, false);
                passwordCard.Init(cardType, cardIds[i], true);
                passwordCard.TurnDown();
                passwordCard.SetCanViewHP(true);
                passwordCard.IsNotSelectableCard(false);
            }
        }
    }

    /// <summary>
    /// カードIDを取得する
    /// </summary>
    private void SeachCardId()
    {
        for (int i = 0; i < spawnKeyWords.Length; i++)
        {
            switch (spawnKeyWords[i])
            {
                case "あ":
                    cardIds[i] = 1;
                    break;
                case "か":
                    cardIds[i] = 2;
                    break;
                case "さ":
                    cardIds[i] = 3;
                    break;
                case "た":
                    cardIds[i] = 4;
                    break;
                case "な":
                    cardIds[i] = 5;
                    break;
                case "は":
                    cardIds[i] = 6;
                    break;
                case "ま":
                    cardIds[i] = 7;
                    break;
                case "や":
                    cardIds[i] = 8;
                    break;
                case "ら":
                    cardIds[i] = 9;
                    break;
                case "わ":
                    cardIds[i] = 10;
                    break;
                default:
                    Debug.LogError($"パスワード設定の例外エラー");
                    break;
            }
        }
    }

    /// <summary>
    /// HackingPanelのアニメーション終了後にカードを表示する
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayedShow()
    {
        // すべてのカードを非表示にする
        for (int i = 0; i < 3; i++)
        {
            cardPositions[i].gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 3; i++)
        {
            cardPositions[i].gameObject.SetActive(true);
        }
        zoomBackPanel.SetActive(false);
    }

    /// <summary>
    /// 初期状態で3枚のカードを配置
    /// </summary>
    private void SetInitialCards()
    {
        List<string> initialCardNames = new List<string> { "あ", "か", "さ" };

        // 既存のカードを削除
        foreach (var card in spawnedCards) Destroy(card);
        spawnedCards.Clear();

        for (int i = 0; i < initialCardNames.Count && i < cardPositions.Length; i++)
        {
            if (cardDictionary.TryGetValue(initialCardNames[i], out GameObject cardPrefab))
            {
                tmpCard[i] = Instantiate(cardPrefab, cardPositions[i]);
                var cardScript = tmpCard[i].GetComponent<PasswordSelectionCard>();
                if (cardScript != null)
                {
                    cardScript.HiddenImage();
                }

                spawnedCards.Add(tmpCard[i]);
            }
        }
    }

    /// <summary>
    /// テキスト入力時の処理
    /// </summary>
    public void OnTextEdit(string text)
    {
        Debug.Log($"previousInput.Length: {previousInput.Length}, inputText.Length: {text.Length}");

        string inputText = text.Trim();
        if (previousInput == inputText) return;

        // 入力文字が前回よりも少ないかどうかのbool値を取得
        bool isShorterThanLastInput = inputText.Length < previousInput.Length;

        // 入力文字が減った（削除された）場合、カードを裏返す処理
        if (isShorterThanLastInput)
        {
            Debug.Log($"call {inputText.Length}");
            for (int i = inputText.Length; i < spawnedCards.Count && i < 3; i++)
            {
                spawnedCards[i].GetComponent<PasswordSelectionCard>().HiddenImage();
            }
            previousInput = inputText;
            return;
        }

        spawnKeyWords = new String[3];

        List<string> cardNames = new List<string>();

        // 入力された文字列の先頭文字を取得
        for (int i = 0; i < Mathf.Min(inputText.Length, 3); i++)
        {
            char c = inputText[i];
            if (kanaGroups.ContainsKey(c))
            {
                // 先頭文字に対応するカード名を取得
                string row = kanaGroups[c];
                spawnKeyWords[i] = row;

                // 重複を避けるため、リストに含まれていない場合のみ追加
                if (!cardNames.Contains(row)) cardNames.Add(row);
            }
        }

        // 先頭文字が重なっている場合は無効とし、入力内容をクリア
        if (cardNames.Count < inputText.Length)
        {
            Debug.Log("重複した先頭文字がありました。入力をリセットします。");
            inputField.text = ""; // 入力内容を消す

            return;
        }

        UpdateCards(cardNames, inputText.Length);
        previousInput = inputText;
    }

    /// <summary>
    /// カードを更新する
    /// </summary>
    /// <param name="cardNames"></param>
    private void UpdateCards(List<string> cardNames, int inputTextLength)
    {
        Debug.Log($"文字が増えたよ！: {cardNames.Count}");

        // 既存のカードを必要に応じて削除
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (i > inputTextLength) continue;
            Destroy(spawnedCards[i]);
        }

        spawnedCards.Clear();

        for (int i = 0; i < inputTextLength; i++)
        {
            Debug.Log($"{i}回目のループ");
            if (i < cardNames.Count && cardDictionary.ContainsKey(cardNames[i]))
            {
                tmpCard[i] = Instantiate(cardDictionary[cardNames[i]], cardPositions[i]);
                PasswordSelectionCard cardScript = tmpCard[i].GetComponent<PasswordSelectionCard>();

                cardScript.ShowImage();
                spawnedCards.Add(tmpCard[i]);
            }
            // else
            // {
            //     tmpCard[i] = Instantiate(cardDictionary["あ"], cardPositions[i]);
            //     PasswordSelectionCard cardScript = tmpCard[i].GetComponent<PasswordSelectionCard>();
            //     if (cardScript != null)
            //     {
            //         cardScript.HiddenImage();
            //     }

            //     spawnedCards.Add(tmpCard[i]);
            // }
        }
    }

    /// <summary>
    /// カード以外（ZoomBackPanel）をクリックしたときの処理
    /// </summary>
    public void OnZoomBackPanelClick()
    {
        PasswordSelectionCard[] cards = this.transform.parent.transform.GetComponentsInChildren<PasswordSelectionCard>();
        foreach (var card in cards)
        {
            card.ResetZoom();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class PasswordSelectionCardManager : MonoBehaviour
{
    [HideInInspector] public TMP_Text explanationText;
    [SerializeField] public GameObject zoomBackPanel;
    [SerializeField] List<GameObject> cardPrefabs;
    [SerializeField] Transform[] cardPositions;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button submitButton;
    [SerializeField] GameManager gameManager;
    [SerializeField] private RectTransform playerPasswordInputField;

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
    private string previousInput = "";
    private PasswordSelectionCard[] cards = new PasswordSelectionCard[3];

    // 母音グループの定義
    private readonly Dictionary<char, int> hiraganaToCardIndex = new Dictionary<char, int>
    {
        // あ行
        {'あ', 0}, {'い', 0}, {'う', 0}, {'え', 0}, {'お', 0},
        // か行
        {'か', 1}, {'き', 1}, {'く', 1}, {'け', 1}, {'こ', 1},
        // さ行
        {'さ', 2}, {'し', 2}, {'す', 2}, {'せ', 2}, {'そ', 2},
        // た行
        {'た', 3}, {'ち', 3}, {'つ', 3}, {'て', 3}, {'と', 3},
        // な行
        {'な', 4}, {'に', 4}, {'ぬ', 4}, {'ね', 4}, {'の', 4},
        // は行
        {'は', 5}, {'ひ', 5}, {'ふ', 5}, {'へ', 5}, {'ほ', 5},
        // ま行
        {'ま', 6}, {'み', 6}, {'む', 6}, {'め', 6}, {'も', 6},
        // や行
        {'や', 7}, {'ゆ', 7}, {'よ', 7},
        // ら行
        {'ら', 8}, {'り', 8}, {'る', 8}, {'れ', 8}, {'ろ', 8},
        // わ行
        {'わ', 9}, {'を', 9}, {'ん', 9},
        // 濁音、半濁音
        {'が', 1}, {'ぎ', 1}, {'ぐ', 1}, {'げ', 1}, {'ご', 1},
        {'ざ', 2}, {'じ', 2}, {'ず', 2}, {'ぜ', 2}, {'ぞ', 2},
        {'だ', 3}, {'ぢ', 3}, {'づ', 3}, {'で', 3}, {'ど', 3},
        {'ば', 5}, {'び', 5}, {'ぶ', 5}, {'べ', 5}, {'ぼ', 5},
        {'ぱ', 5}, {'ぴ', 5}, {'ぷ', 5}, {'ぺ', 5}, {'ぽ', 5}
    };

    void Start()
    {
        InitializeCardDictionary();
        SetupEventHandlers();
        StartCoroutine(InitializeCardsAfterAnimation());
    }

    private void InitializeCardDictionary()
    {
        // プレハブリストを辞書に登録
        foreach (var prefab in cardPrefabs)
        {
            cardDictionary[prefab.name] = prefab;
        }

        // カードの説明文オブジェを取得
        explanationText = zoomBackPanel.transform.Find("ExplanationText").GetComponent<TMP_Text>();
    }

    private void SetupEventHandlers()
    {
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

    private IEnumerator InitializeCardsAfterAnimation()
    {
        // PlayerPasswordInputFieldのアニメーション完了を待つ
        // アニメーション時間（1秒）+ 少し余裕を持たせる
        yield return new WaitForSeconds(1.2f);

        // 初期カードを生成
        SetInitialCards();

        // 全てのカードを一時的に非表示
        foreach (var card in spawnedCards)
        {
            if (card != null)
            {
                card.SetActive(false);
            }
        }

        // カードを順番に表示
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
            {
                spawnedCards[i].SetActive(true);
            }
        }
    }

    /// <summary>
    /// 初期状態で3枚のカードを配置
    /// </summary>
    private void SetInitialCards()
    {
        // 既存のカードを削除
        foreach (var card in spawnedCards) Destroy(card);
        spawnedCards.Clear();

        // 3枚の裏向きカードを配置
        for (int i = 0; i < 3; i++)
        {
            if (cardDictionary.TryGetValue("あ", out GameObject cardPrefab))
            {
                tmpCard[i] = Instantiate(cardPrefab, cardPositions[i]);
                var cardScript = tmpCard[i].GetComponent<PasswordSelectionCard>();
                if (cardScript != null)
                {
                    cardScript.HiddenImage();
                    cards[i] = cardScript;
                }
                spawnedCards.Add(tmpCard[i]);
            }
        }
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
    /// テキスト入力時の処理
    /// </summary>
    public void OnTextEdit(string newInput)
    {
        if (!IsValidHiraganaInput(newInput))
        {
            inputField.text = previousInput;
            return;
        }

        int effectiveLength = GetEffectiveLength(newInput);
        if (effectiveLength > 3)
        {
            inputField.text = previousInput;
            return;
        }

        UpdateCardsForInput(newInput);
        previousInput = newInput;
    }

    private void UpdateCardsForInput(string input)
    {
        // 入力された文字から有効な母音グループを計算
        var currentCards = new List<int>();
        var processedGroups = new HashSet<int>(); // 既に処理した母音グループを追跡

        foreach (char c in input)
        {
            if (hiraganaToCardIndex.TryGetValue(c, out int cardIndex))
            {
                if (!processedGroups.Contains(cardIndex))
                {
                    currentCards.Add(cardIndex);
                    processedGroups.Add(cardIndex);
                }
            }
        }

        // 既存のカードの状態を保存
        var existingCards = new List<(bool isShowing, string cardType, GameObject card)>();
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
            {
                var card = spawnedCards[i].GetComponent<PasswordSelectionCard>();
                existingCards.Add((card.isFaceUp, spawnedCards[i].name.Substring(0, 1), spawnedCards[i]));
            }
            else
            {
                existingCards.Add((false, null, null));
            }
        }

        // 文字数が減少した場合の処理
        if (input.Length < previousInput.Length)
        {
            // 現在表示されているカードと新しい状態を比較
            var currentShowingCards = spawnedCards.Where((card, index) =>
                card != null &&
                card.GetComponent<PasswordSelectionCard>().isFaceUp).ToList();

            // 新しい表示状態と異なるカードを裏返す
            for (int i = 0; i < spawnedCards.Count; i++)
            {
                if (spawnedCards[i] != null)
                {
                    var cardScript = spawnedCards[i].GetComponent<PasswordSelectionCard>();
                    bool shouldShow = i < currentCards.Count;

                    if (cardScript.isFaceUp && !shouldShow)
                    {
                        cardScript.HiddenImage();
                    }
                }
            }

            // アニメーション完了を待つため、この時点で次の更新を準備
            StartCoroutine(UpdateCardsAfterAnimation(currentCards));
            previousInput = input;
            return;
        }

        // カードの更新（文字数が同じか増加した場合）
        UpdateCardStates(currentCards);

        previousInput = input;
    }

    private IEnumerator UpdateCardsAfterAnimation(List<int> currentCards)
    {
        // アニメーション完了を待つ
        yield return new WaitForSeconds(0.5f);

        // カードの状態を更新
        UpdateCardStates(currentCards);
    }

    private void UpdateCardStates(List<int> currentCards)
    {
        for (int i = 0; i < 3; i++)
        {
            bool shouldBeShowing = i < currentCards.Count;
            string newCardType = shouldBeShowing ? GetCardNameFromIndex(currentCards[i]) : "あ";

            if (i < spawnedCards.Count && spawnedCards[i] != null)
            {
                var cardScript = spawnedCards[i].GetComponent<PasswordSelectionCard>();
                string currentCardType = spawnedCards[i].name.Substring(0, 1);

                if (currentCardType != newCardType)
                {
                    // カードの種類が変わる場合、新しいカードを生成
                    if (cardScript.isFaceUp)
                    {
                        cardScript.HiddenImage();
                    }
                    var newCard = Instantiate(cardDictionary[newCardType], cardPositions[i]);
                    var newCardScript = newCard.GetComponent<PasswordSelectionCard>();

                    if (shouldBeShowing)
                    {
                        newCardScript.ShowImage();
                    }
                    else
                    {
                        newCardScript.SetCardStateWithoutAnimation(false);
                    }

                    Destroy(spawnedCards[i]);
                    cards[i] = newCardScript;
                    spawnedCards[i] = newCard;
                }
                else if (cardScript.isFaceUp != shouldBeShowing)
                {
                    // 表示状態のみ変更する場合
                    if (shouldBeShowing)
                    {
                        cardScript.ShowImage();
                    }
                    else
                    {
                        cardScript.HiddenImage();
                    }
                }
            }
            else
            {
                // 新しいカードを生成
                var newCard = Instantiate(cardDictionary[newCardType], cardPositions[i]);
                var newCardScript = newCard.GetComponent<PasswordSelectionCard>();

                if (shouldBeShowing)
                {
                    newCardScript.ShowImage();
                }
                else
                {
                    newCardScript.SetCardStateWithoutAnimation(false);
                }

                cards[i] = newCardScript;
                if (i < spawnedCards.Count)
                {
                    spawnedCards[i] = newCard;
                }
                else
                {
                    spawnedCards.Add(newCard);
                }
            }
        }

        // spawnKeyWordsの更新
        spawnKeyWords = new string[3];
        for (int i = 0; i < currentCards.Count && i < 3; i++)
        {
            spawnKeyWords[i] = GetCardNameFromIndex(currentCards[i]);
        }
    }

    private string GetCardNameFromIndex(int index)
    {
        switch (index)
        {
            case 0: return "あ";
            case 1: return "か";
            case 2: return "さ";
            case 3: return "た";
            case 4: return "な";
            case 5: return "は";
            case 6: return "ま";
            case 7: return "や";
            case 8: return "ら";
            case 9: return "わ";
            default: return "あ";
        }
    }

    private bool IsValidHiraganaInput(string input)
    {
        return input.All(c => hiraganaToCardIndex.ContainsKey(c) || c == '\0');
    }

    private int GetEffectiveLength(string input)
    {
        return input.Where(c => hiraganaToCardIndex.ContainsKey(c))
                   .Select(c => hiraganaToCardIndex[c])
                   .Distinct()
                   .Count();
    }

    /// <summary>
    /// カード以外（ZoomBackPanel）をクリックしたときの処理
    /// </summary>
    public void OnZoomBackPanelClick()
    {
        zoomBackPanel.SetActive(false);
        foreach (var card in cards)
        {
            if (card != null)
            {
                card.ResetZoom();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// ゲーム全体の進行を管理するクラスb
/// </summary>
public class GameManager : MonoBehaviour
{
    // プレイヤーと敵の情報を管理
    public GamePlayerManager player;
    public GamePlayerManager enemy;

    //フィールドにパスワードを生成
    [SerializeField] Transform playerPasswordFieldTransform;
    [SerializeField] Transform enemyPasswordFieldTransform;

    //ファイアウォールカード置き場
    [SerializeField] Transform playerFirewallCardFieldTransform;
    [SerializeField] Transform enemyFirewallCardFieldTransform;

    //環境カード置き場
    [SerializeField] Transform starCardFieldTransform;

    //使用済みカード置き場
    [SerializeField] Transform usedCardFieldTransform;

    //デッキカード置き場
    [SerializeField] Transform deckCardFieldTransform;

    //マイグレーション3カードでめくったカード置き場
    [SerializeField] Transform playerMigration3CardFieldTransform;
    [SerializeField] Transform enemyMigration3CardFieldTransform;

    // ブルートフォースカードパネル
    [SerializeField] GameObject bruteForceCardPanel;

    //カードプレハブ
    [SerializeField] BruteForceCardController bruteForceCardPrefab;
    [SerializeField] FirewallCardController firewallCardPrefab;
    [SerializeField] EventCardController eventCardPrefab;
    [SerializeField] StarCardController starCardPrefab;
    [SerializeField] PasswordCardController passwordCardPrefab;

    //プレイヤーのパスワード入力フォーム
    [SerializeField] PasswordInputPanelAnimation playerPasswordInputPanel;
    [SerializeField] Transform passwordCardFieldTransform;

    //フェイズ終了ボタン
    [SerializeField] GameObject phaseEndButton;

    //ハッキング宣言ボタン
    [SerializeField] GameObject hackingButton;

    //パスワード入力フォーム（ハッキングパネル）
    [SerializeField] GameObject hackingPasswordPanel;
    [SerializeField] Transform hackingPasswordFieldTransform;

    //場の効果確認ボタン
    [SerializeField] Button fieldEffectButton;

    //メニューボタン
    [SerializeField] GameObject menuButtonImage;

    //メニュー画面
    [SerializeField] GameObject menuPanel;

    // カードマネージャ
    [SerializeField] CardManager cardManager;

    //メッセージログマネージャ
    [SerializeField] MessageLogManager messageLogManager;

    //ダイアログマネージャ
    [SerializeField] DialogueManager dialogueManager;

    // パスワードチェック
    [SerializeField] PasswordManager passwordManager;

    // ルールマネージャ
    [SerializeField] RuleManager rulePanel;

    // ナビゲーションメッセージマネージャ
    [SerializeField] NavigationMessagesManager navigationMessagesManager;

    // カードの詳細確認用パネルマネージャ
    [SerializeField] DetailPanelManager detailPanelManager;

    [SerializeField] TMP_Text debugText1;
    [SerializeField] TMP_Text debugText2;


    public bool IsPlayerTurn { get; private set; } = false;

    public CardController NowSelectingCard { get; set; }

    public bool CardSelectflag { get; set; }
    public bool InputCorrectPasswordflag { get; set; }

    /// <summary>
    /// カードの情報を保持するクラス
    /// </summary>
    public class Card
    {
        public int cardType;  // カードの種類
        public int cardID;    // カードのID

        public Card(int cardType, int cardID)
        {
            this.cardType = cardType;
            this.cardID = cardID;
        }
    }

    public List<Card> passwordDeck = new List<Card>();
    public List<Card> cardDeck = new List<Card>();

    public static GameManager Instance;

    // ゲームの定数を定義
    public static class Define
    {
        public const int maxHandCardNum = 5;           // 最大手札枚数
        public const int initPasswordCardNum = 10;     // 初期パスワードカード枚数
        public const int passwordLength = 3;           // パスワードの長さ
        public const int maxPasswordHP = 100;          // パスワードの最大HP
        public const int enemyPasswordNum = 50;        // 敵のパスワード数
        public const int selectableCardNum = 3;        // 選択可能なカード枚数
        public const int bruteForceCardNum = 3;        // ブルートフォースカードの枚数
        public const int eventCardNum = 20;            // イベントカードの枚数
    }

    // ゲームのフェーズを管理する列挙型
    public enum PHASE
    {
        START,      // 開始フェーズ
        MULLIGAN,   // マリガンフェーズ
        EVENT,      // イベントフェーズ
        ATTACK,     // 攻撃フェーズ
        RESULT      // 結果フェーズ
    }

    // アクションの種類を管理する列挙型
    public enum ACTION
    {
        NONE,    // アクションなし
        ATTACK,  // 攻撃
        HEAL     // 回復
    }

    public PHASE phase;
    public ACTION action;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    /// <summary>
    /// ゲーム開始時の処理
    /// </summary>
    void StartGame()
    {
        messageLogManager.ShowLog("ここにはログが表示され、いつでも見返すことができます。", true, 0.05f, 1f);

        // 用途不明（？？）
        // PushResumeButton();

        // 場の効果確認ボタンの非表示
        fieldEffectButton.gameObject.SetActive(false);

        // フラグの初期化
        player.ResetUseCardflag();
        enemy.ResetUseCardflag();
        player.Resetflag();
        enemy.Resetflag();
        player.ResetOpenPasswordflag();
        enemy.ResetOpenPasswordflag();
        CardSelectflag = false;
        InputCorrectPasswordflag = false;
        IsPlayerTurn = true;

        // パスワードの初期化
        player.Password = "";

        // ハッキング宣言ボタンの非表示
        hackingPasswordPanel.SetActive(false);

        // メニューボタンの非表示
        SetPhaseEndButtonActive(false);

        // メニュー画面の非表示
        SetHackingButtonActive(false);

        // Migration3カード置き場の非表示
        playerMigration3CardFieldTransform.gameObject.SetActive(false);
        enemyMigration3CardFieldTransform.gameObject.SetActive(false);

        // フェーズを"スタート"へ変更
        phase = PHASE.START;

        // Actionを"NONE"へ変更
        action = ACTION.NONE;

        // バトルフェーズの開始
        StartCoroutine(Battle());
    }

    void Update()
    {
        debugText1.text = $"IsPlayerTurn : {IsPlayerTurn}";
        debugText2.text = $"Phase : {phase}";
    }

    /// <summary>
    /// バトルフェーズの処理
    /// </summary>
    IEnumerator Battle()
    {
        yield return new WaitForSeconds(3.5f);

        navigationMessagesManager.ShowNavigationMessage("3文字のパスワードを決めてください");

        yield return new WaitForSeconds(3.5f);

        playerPasswordInputPanel.GetComponent<PasswordInputPanelAnimation>().ShowAnimation();

        // プレイヤーパスワードの入力待ち
        yield return new WaitUntil(() => InputCorrectPasswordflag);

        // プレイヤーパスワード入力フォームを非表示
        playerPasswordInputPanel.HideAnimation();

        //ブルートフォースカードの初期化
        cardManager.InitBruteForceCard();
        bruteForceCardPanel.SetActive(false);

        // 敵にパスワードカードを10枚配る
        ChangeTurn();
        InitPasswordDeck();
        InitPasswordCard();

        // ここでエラーが発生している
        // yield return StartCoroutine(SelectPasswordCard());

        //敵のパスワードを選択
        yield return StartCoroutine(DecideEnemyPasswordCard());


        ChangeTurn();

        // プレイヤーと敵のデックを初期化
        InitCardDeck();
        InitHandCard();

        phase = PHASE.MULLIGAN;

        // navigationMessagesManager.ShowNavigationMessage("あなたのマリガンフェーズです。\nいらないカードを捨ててください", 3f);

        //プレイヤーのマリガン
        yield return StartCoroutine(Mulligan());

        ChangeTurn();

        //敵のマリガン
        yield return StartCoroutine(Mulligan());

        ChangeTurn();


        NowSelectingCard = null;

        while (true)
        {
            //プレイヤーのターンと敵のターンを交互に繰り返す
            if (player.TurnSkipflag)
            {
                player.TurnSkipflag = false;
            }
            else
            {
                yield return StartCoroutine(PlayerTurn());
            }

            // 自分の勝敗判定
            if (player.Winflag)
            {
                Debug.Log("プレイヤーの勝ちです");
                SceneManager.LoadScene("Result");
                break;
            }

            if (enemy.TurnSkipflag)
            {
                enemy.TurnSkipflag = false;
            }
            else
            {
                yield return StartCoroutine(EnemyTurn());
            }

            // 敵の勝敗判定
            if (enemy.Winflag)
            {
                Debug.Log("敵の勝ちです");
                SceneManager.LoadScene("Result");
                break;
            }
        }
    }

    /// <summary>
    /// プレイヤーのターン
    /// </summary>
    IEnumerator PlayerTurn()
    {
        messageLogManager.ShowLog("あなたのターンです。");
        messageLogManager.HideLog(1f);

        yield return new WaitForSeconds(1);

        //場に出ている環境カードを取得
        StarCardController starCard = GetStarCard();
        if (starCard != null)
        {
            //場に出ている環境カードがゼロデイ攻撃Ⅰカードなら
            if (starCard.Model.EventCardType == EVENT_CARD_TYPE.ZERO_DAY_ATTACK)
            {
                NowSelectingCard = starCard;
                //プレイヤーのパスワードカードを取得
                CardController[] playerPasswordCardList = GetPlayerSelectableCardList(IsPlayerTurn);
                //パスワードカードを選択
                yield return StartCoroutine(SelectCard(playerPasswordCardList));
                PasswordCardController selectedPasswordCard = (PasswordCardController)NowSelectingCard;
                //選択したパスワードに10ダメージ
                yield return StartCoroutine(selectedPasswordCard.BruteForceAttack(starCard.Model.AttackToEnemy));
            }
        }
        //プレイヤーの手札が5枚未満ならカードをドロー
        CardController[] playerHandCard = GetPlayerHands(IsPlayerTurn);
        if (playerHandCard.Length < Define.maxHandCardNum)
        {
            messageLogManager.ShowLog("カードをドロー");
            DrawCard(cardDeck, IsPlayerTurn);
        }
        Debug.Log("イベントフェイズに移行します");
        yield return StartCoroutine(EventPhase());
        Debug.Log("アタックフェイズに移行します");
        yield return StartCoroutine(AttackPhase());

        if (!player.HackingFailedflag && player.Hackingflag)
        {
            // --- ここで透過ログで「アイムハッカー！！」を表示する ---

            messageLogManager.ShowLog("アイムハッカー宣言");
            yield return StartCoroutine(HackingEnemyPassword());
            if (player.Winflag)
            {
                yield break;
            }
        }

        //プレイヤーのパスワードカードを取得
        PasswordCardController[] passwordCardList = GetPlayerPasswordCards(IsPlayerTurn);
        foreach (PasswordCardController card in passwordCardList)
        {
            //な行カードのとき
            if (card.Model.PasswordCardType == PASSWORD_CARD_TYPE.NA_GYOU)
            {
                //表になっていてスキル発動可能HP以下になっているな行カードのみ
                if (card.Model.IsHacked || !card.Model.IsOpened || (card.Model.HP > card.Model.CanUseSkillHP)) continue;

                //前のターンに敵がDoS攻撃カードを使用してなければ効果発動
                if (enemy.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("相手のDoS攻撃カードの効果でプレイヤーのな行カードの効果は無効化された！");
                    break;
                }
                else
                {
                    NowSelectingCard = card;

                    //パスワードカードを選択
                    CardController[] healablePasswordCardList = GetHealableCardList(IsPlayerTurn);

                    //回復可能なカードがなければ終了
                    if (healablePasswordCardList.Length <= 0)
                    {
                        break;
                    }

                    yield return StartCoroutine(SelectCard(healablePasswordCardList));
                    PasswordCardController selectedPasswordCard = (PasswordCardController)NowSelectingCard;
                    //10回復
                    selectedPasswordCard.Maintenance(card.Model.Heal);
                }
            }
        }

        if (!enemy.TurnSkipflag)
        {
            TurnEndflagReset(IsPlayerTurn);
            ChangeTurn();
        }
    }

    /// <summary>
    /// 敵のターン
    /// </summary>
    IEnumerator EnemyTurn()
    {
        messageLogManager.ShowLog("相手のターンです");
        Debug.Log(IsPlayerTurn);
        yield return new WaitForSeconds(1);
        //場に出ている環境カードを取得
        StarCardController starCard = GetStarCard();
        if (starCard != null)
        {
            //場に出ている環境カードがゼロデイ攻撃Ⅰカードなら
            if (starCard.Model.EventCardType == EVENT_CARD_TYPE.ZERO_DAY_ATTACK)
            {
                NowSelectingCard = starCard;
                //敵のパスワードカードを取得
                CardController[] enemyPasswordCardList = GetPlayerSelectableCardList(IsPlayerTurn);
                //パスワードカードを選択
                yield return StartCoroutine(SelectCard(enemyPasswordCardList));
                PasswordCardController selectedPasswordCard = (PasswordCardController)NowSelectingCard;
                //選択したパスワードに10ダメージ
                yield return StartCoroutine(selectedPasswordCard.BruteForceAttack(starCard.Model.AttackToEnemy));
            }
        }
        //敵の手札が5枚未満ならカードをドロー
        CardController[] enemyHandCard = GetPlayerHands(IsPlayerTurn);
        if (enemyHandCard.Length < Define.maxHandCardNum)
        {
            messageLogManager.ShowLog("敵のカードをドロー");
            DrawCard(cardDeck, IsPlayerTurn);
        }
        messageLogManager.ShowLog("イベントフェイズ");
        yield return StartCoroutine(EventPhase());
        messageLogManager.ShowLog("アタックフェイズ");
        yield return StartCoroutine(AttackPhase());
        if (!enemy.HackingFailedflag && enemy.Hackingflag)
        {
            messageLogManager.ShowLog("アイムハッカー宣言");
            HackingPlayerPassword();
            if (player.Winflag)
            {
                yield break;
            }
        }

        //相手のパスワードカードを取得
        PasswordCardController[] playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
        foreach (PasswordCardController card in playerPasswordCardList)
        {
            //な行カードのとき
            if (card.Model.PasswordCardType == PASSWORD_CARD_TYPE.NA_GYOU)
            {
                //表になっていてスキル発動可能HP以下になっているな行カードのみ
                if (card.Model.IsHacked || !card.Model.IsOpened || (card.Model.HP > card.Model.CanUseSkillHP)) continue;

                //前のターンにプレイヤーがDoS攻撃カードを使用してなければ効果発動
                if (player.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("プレイヤーのDoS攻撃カードの効果で相手のな行カードの効果は無効化された！");
                    break;
                }
                else
                {
                    NowSelectingCard = card;

                    //パスワードカードを選択
                    CardController[] passwordCardList = GetHealableCardList(IsPlayerTurn);

                    //回復可能なカードがなければ終了
                    if (passwordCardList.Length <= 0)
                    {
                        break;
                    }

                    yield return StartCoroutine(SelectCard(passwordCardList));
                    PasswordCardController selectedPasswordCard = (PasswordCardController)NowSelectingCard;
                    //10回復
                    selectedPasswordCard.Maintenance(card.Model.Heal);
                }
            }
        }

        if (!player.TurnSkipflag)
        {
            TurnEndflagReset(IsPlayerTurn);
            ChangeTurn();
        }
    }

    /// <summary>
    /// ターンを変更する
    /// </summary>
    public void ChangeTurn()
    {
        IsPlayerTurn = !IsPlayerTurn;
    }

    /// <summary>
    /// パスワードデッキを初期化する
    /// </summary>
    public void InitPasswordDeck()
    {
        passwordDeck = new List<Card>() { new Card(5, 1), new Card(5, 1), new Card(5, 1),
                                          new Card(5, 2), new Card(5, 2), new Card(5, 2),
                                          new Card(5, 3), new Card(5, 3), new Card(5, 3),
                                          new Card(5, 4), new Card(5, 4), new Card(5, 4),
                                          new Card(5, 5), new Card(5, 5), new Card(5, 5),
                                          new Card(5, 6), new Card(5, 6), new Card(5, 6),
                                          new Card(5, 7), new Card(5, 7), new Card(5, 7),
                                          new Card(5, 8), new Card(5, 8), new Card(5, 8),
                                          new Card(5, 9), new Card(5, 9), new Card(5, 9),
                                          new Card(5, 10), new Card(5, 10), new Card(5, 10) };
        ShuffleDeck(passwordDeck);
        ShuffleDeck(passwordDeck);
        ShuffleDeck(passwordDeck);
    }

    /// <summary>
    /// デッキをシャッフルする
    /// </summary>
    public void ShuffleDeck(List<Card> deck)
    {
        // Fisher-Yatesアルゴリズムでシャッフル
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n);
            // カードの交換
            int tmpCardType = deck[k].cardType;
            int tmpCardID = deck[k].cardID;
            deck[k].cardType = deck[n].cardType;
            deck[k].cardID = deck[n].cardID;
            deck[n].cardType = tmpCardType;
            deck[n].cardID = tmpCardID;
        }
    }

    /// <summary>
    /// パスワードカードを初期化する
    /// </summary>
    public void InitPasswordCard()
    {
        //カードを10枚配る
        for (int i = 0; i < Define.initPasswordCardNum; i++)
        {
            DrawCard(passwordDeck, IsPlayerTurn);
        }
    }

    /// <summary>
    /// カードをドローする
    /// </summary>
    public void DrawCard(List<Card> list, bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            GiveCardToHand(list, cardManager.playerHandTransform);
        }
        else
        {
            GiveCardToHand(list, cardManager.enemyHandTransform);
        }
    }

    /// <summary>
    /// カードを手札に渡す
    /// </summary>
    void GiveCardToHand(List<Card> list, Transform hand)
    {
        if (list.Count == 0)//デッキにカードが残ってないとき
        {
            DeckReset();
        }
        int cardType = list[list.Count - 1].cardType;
        int cardID = list[list.Count - 1].cardID;
        list.RemoveAt(list.Count - 1);
        cardManager.CreateCard(cardType, cardID, hand);
    }

    /// <summary>
    /// 選択可能なカードを見つける
    /// </summary>
    void FindSelectableCard(CardController[] cardList)
    {

        foreach (CardController card in cardList)
        {
            // カードの種類に応じて選択可能かどうかを判定
            switch (card.Model.CardType)
            {
                case CARD_TYPE.BRUTE_FORCE:
                    BruteForceCardController bruteForceCard = (BruteForceCardController)card;
                    if (IsSelectableBruteForceCard(bruteForceCard))
                    {
                        card.SetCanSelect(true);
                    }
                    break;
                case CARD_TYPE.FIREWALL:
                    card.SetCanSelect(true);
                    break;
                case CARD_TYPE.EVENT:
                    EventCardController eventCard = (EventCardController)card;
                    if (IsSelectableEventCard(eventCard))
                    {
                        card.SetCanSelect(true);
                    }
                    break;
                case CARD_TYPE.STAR:
                    StarCardController starCard = (StarCardController)card;
                    if (IsSelectableStarCard(starCard))
                    {
                        card.SetCanSelect(true);
                    }
                    break;
                case CARD_TYPE.PASSWORD:
                    PasswordCardController passwordCard = (PasswordCardController)card;
                    if (IsSelectablePasswordCard(passwordCard))
                    {
                        card.SetCanSelect(true);
                    }
                    break;
                default:
                    card.SetCanSelect(false);
                    break;
            }
        }
    }

    /// <summary>
    /// ブルートフォースカードが選択可能かどうかを判定する
    /// </summary>
    bool IsSelectableBruteForceCard(BruteForceCardController bruteForceCard)
    {
        if (IsPlayerTurn && player.UseInfomationLeakageCardflag)
        {
            return false;
        }

        if (!IsPlayerTurn && enemy.UseInfomationLeakageCardflag)
        {
            return false;
        }

        switch (phase)
        {
            case PHASE.START:
            case PHASE.MULLIGAN:
            case PHASE.EVENT:
                return false;
            case PHASE.ATTACK:
                switch (bruteForceCard.Model.BruteForceCardType)
                {
                    case BRUTE_FORCE_CARD_TYPE.BRUTE_FORCE1:
                        return true;
                    case BRUTE_FORCE_CARD_TYPE.BRUTE_FORCE2:
                        PasswordCardController[] passwordCardList = GetPlayerPasswordCards(IsPlayerTurn);
                        foreach (PasswordCardController passwordCard in passwordCardList)
                        {
                            if (!passwordCard.Model.IsHacked && (passwordCard.Model.HP < Define.maxPasswordHP))
                            {
                                return true;
                            }
                        }
                        return false;
                    case BRUTE_FORCE_CARD_TYPE.BRUTE_FORCE3:
                        return true;
                }
                break;
        }
        return false;
    }

    /// <summary>
    /// 最小のパスワードHPを取得する
    /// </summary>
    public int GetMinPasswordHP(PasswordCardController[] passwordCardList)
    {
        int num = 101;
        foreach (PasswordCardController passwordCard in passwordCardList)
        {
            if (!passwordCard.Model.IsHacked && (num > passwordCard.Model.HP))
            {
                num = passwordCard.Model.HP;
            }
        }
        return num;
    }

    /// <summary>
    /// 最大のパスワードHPを取得する
    /// </summary>
    public int GetMaxPasswordHP(PasswordCardController[] passwordCardList)
    {
        int num = 1;
        foreach (PasswordCardController passwordCard in passwordCardList)
        {
            if (!passwordCard.Model.IsHacked && (num < passwordCard.Model.HP))
            {
                num = passwordCard.Model.HP;
            }
        }
        return num;
    }

    /// <summary>
    /// イベントカードが選択可能かどうかを判定する
    /// </summary>
    bool IsSelectableEventCard(EventCardController eventCard)
    {
        PasswordCardController[] allPasswordCardList;
        PasswordCardController[] playerPasswordCardList;
        PasswordCardController[] enemyPasswordCardList;
        CardController[] usedCardList;
        CardController[] playerHandCardList;
        StarCardController starCard;
        switch (phase)
        {
            case PHASE.START:
            case PHASE.MULLIGAN:
                return false;
            case PHASE.EVENT:
                switch (eventCard.Model.EventCardType)
                {
                    case EVENT_CARD_TYPE.MAINTENANCE:
                        playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
                        foreach (PasswordCardController passwordCard in playerPasswordCardList)
                        {
                            if (!passwordCard.Model.IsHacked && (passwordCard.Model.HP < Define.maxPasswordHP))
                            {
                                return true;
                            }
                        }
                        return false;
                    case EVENT_CARD_TYPE.MAINTENANCE2:
                        playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
                        int i = 2;
                        foreach (PasswordCardController passwordCard in playerPasswordCardList)
                        {
                            if (!passwordCard.Model.IsHacked && (passwordCard.Model.HP < Define.maxPasswordHP))
                            {
                                if (--i <= 0)
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    case EVENT_CARD_TYPE.XSS:
                        /*
                        playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
                        enemyPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
                        CardController[] passwordCardList = MergeCardList(playerPasswordCardList, enemyPasswordCardList);
                        allPasswordCardList = new PasswordCardController[passwordCardList.Length];
                        passwordCardList.CopyTo(allPasswordCardList, 0);
                        */
                        allPasswordCardList = GetAllPasswordCard();
                        int max = GetMaxPasswordHP(allPasswordCardList);
                        int min = GetMinPasswordHP(allPasswordCardList);
                        if (max == min)
                        {
                            return false;
                        }
                        return true;
                    case EVENT_CARD_TYPE.RESTORATION:
                        usedCardList = GetUsedCardList();
                        if (usedCardList.Length > 0)
                        {
                            return true;
                        }
                        return false;
                    case EVENT_CARD_TYPE.INFOMATION_LEAKAGE:
                        bool canSelectflag = false;

                        if (IsPlayerTurn && player.HackingFailedflag)
                        {
                            return false;
                        }

                        if (!IsPlayerTurn && enemy.HackingFailedflag)
                        {
                            return false;
                        }

                        if (IsPlayerTurn && player.UseInfomationLeakageCardflag)
                        {
                            return false;
                        }

                        if (!IsPlayerTurn && enemy.UseInfomationLeakageCardflag)
                        {
                            return false;
                        }

                        playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
                        foreach (PasswordCardController card in playerPasswordCardList)
                        {
                            if (!card.Model.IsOpened)
                            {
                                canSelectflag = true;
                                break;
                            }
                        }
                        if (!canSelectflag)
                        {
                            return false;
                        }
                        enemyPasswordCardList = GetPlayerPasswordCards(!IsPlayerTurn);
                        foreach (PasswordCardController card in enemyPasswordCardList)
                        {
                            if (!card.Model.IsOpened)
                            {
                                return true;
                            }
                        }
                        return false;
                    case EVENT_CARD_TYPE.PLUS_MINUS:
                        playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
                        foreach (PasswordCardController passwordCard in playerPasswordCardList)
                        {
                            if (!passwordCard.Model.IsHacked && (passwordCard.Model.HP < Define.maxPasswordHP))
                            {
                                return true;
                            }
                        }
                        return false;
                    case EVENT_CARD_TYPE.FALSIFICATION:
                        playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
                        enemyPasswordCardList = GetPlayerPasswordCards(!IsPlayerTurn);
                        if ((GetMinPasswordHP(playerPasswordCardList) > 50) && (GetMaxPasswordHP(enemyPasswordCardList) < 50))
                        {
                            return false;
                        }
                        return true;
                    case EVENT_CARD_TYPE.RECONSTITUITION:
                        playerHandCardList = GetPlayerHands(IsPlayerTurn);
                        if (playerHandCardList.Length > 0)
                        {
                            return true;
                        }
                        return false;
                    case EVENT_CARD_TYPE.OPTIMIZATION:
                        starCard = GetStarCard();
                        if (starCard == null)
                        {
                            return false;
                        }
                        return true;
                    case EVENT_CARD_TYPE.CTL_Z:
                        return true;
                    case EVENT_CARD_TYPE.MIGRATION2:
                        playerHandCardList = GetPlayerHands(IsPlayerTurn);
                        if (playerHandCardList.Length > 0)
                        {
                            return true;
                        }
                        return false;
                    case EVENT_CARD_TYPE.MIGRATION3:
                        return true;
                    case EVENT_CARD_TYPE.MIGRATION:
                        usedCardList = GetUsedCardList();
                        foreach (CardController card in usedCardList)
                        {
                            if (card.Model.CardType == CARD_TYPE.STAR)
                            {
                                return true;
                            }
                        }
                        return false;
                    case EVENT_CARD_TYPE.RECOVERY:
                    case EVENT_CARD_TYPE.SCALE_UP:
                    case EVENT_CARD_TYPE.HEAVY_LOAD:
                        return true;
                    case EVENT_CARD_TYPE.WHITELIST:
                        if (IsPlayerTurn && player.UseWhiteListCardflag)
                        {
                            return false;
                        }

                        if (!IsPlayerTurn && enemy.UseWhiteListCardflag)
                        {
                            return false;
                        }
                        return true;
                    case EVENT_CARD_TYPE.REBOOT:
                        if (IsPlayerTurn && player.UseRebootCardflag)
                        {
                            return false;
                        }

                        if (!IsPlayerTurn && enemy.UseRebootCardflag)
                        {
                            return false;
                        }
                        return true;
                    case EVENT_CARD_TYPE.NARROW_BAND:
                        if (IsPlayerTurn && player.UseNarrowBandCardflag)
                        {
                            return false;
                        }

                        if (!IsPlayerTurn && enemy.UseNarrowBandCardflag)
                        {
                            return false;
                        }
                        return true;
                    case EVENT_CARD_TYPE.MALWARE:
                        if (IsPlayerTurn && player.UseDoSAttackCardflag)
                        {
                            return false;
                        }

                        if (!IsPlayerTurn && enemy.UseDoSAttackCardflag)
                        {
                            return false;
                        }
                        return true;
                }
                return false;
            case PHASE.ATTACK:
                return false;
        }
        return false;
    }

    /// <summary>
    /// スターカードが選択可能かどうかを判定する
    /// </summary>
    bool IsSelectableStarCard(StarCardController starCard)
    {
        if (IsPlayerTurn && enemy.UseNarrowBandCardflag)
        {
            return false;
        }
        if (!IsPlayerTurn && player.UseNarrowBandCardflag)
        {
            return false;
        }

        switch (phase)
        {
            case PHASE.START:
            case PHASE.MULLIGAN:
                return false;
            case PHASE.EVENT:
                switch (starCard.Model.EventCardType)
                {
                    case EVENT_CARD_TYPE.PAIN_SPLIT:
                        return true;
                    case EVENT_CARD_TYPE.DISABLE_FIREWALL:
                        FirewallCardController[] playerFirewallCardList = GetPlayerFirewallCards(IsPlayerTurn);
                        FirewallCardController[] enemyFirewallCardList = GetPlayerFirewallCards(!IsPlayerTurn);
                        CardController[] firewallCardList = MergeCardList(playerFirewallCardList, enemyFirewallCardList);
                        if (firewallCardList.Length > 0)
                        {
                            return true;
                        }
                        return false;
                    case EVENT_CARD_TYPE.VILUS_BECOME_FEROCIOUS:
                    case EVENT_CARD_TYPE.SERVER_REINFORCEMENT:
                    case EVENT_CARD_TYPE.EARLY_SETTLEMENT:
                    case EVENT_CARD_TYPE.BUFFER_OVERFLOW:
                    case EVENT_CARD_TYPE.BUFFER_OVERFLOW2:
                    case EVENT_CARD_TYPE.ZERO_DAY_ATTACK:
                    case EVENT_CARD_TYPE.ZERO_DAY_ATTACK2:
                    case EVENT_CARD_TYPE.FILTERING:
                        return true;
                    case EVENT_CARD_TYPE.UPDATE:
                        PasswordCardController[] allPasswordCardList = GetAllPasswordCard();
                        foreach (PasswordCardController passwordCard in allPasswordCardList)
                        {
                            if (!passwordCard.Model.IsHacked && (passwordCard.Model.HP < Define.maxPasswordHP))
                            {
                                return true;
                            }
                        }
                        return false;
                }
                break;
            case PHASE.ATTACK:
                return false;
        }
        return false;
    }

    /// <summary>
    /// パスワードカードが選択可能かどうかを判定する
    /// </summary>
    bool IsSelectablePasswordCard(PasswordCardController passwordCard)
    {
        //パスワードカードのHPが0なら選択不可
        if (passwordCard.Model.IsHacked)
        {
            return false;
        }

        switch (phase)
        {
            case PHASE.START:
                return true;
            case PHASE.MULLIGAN:
                return false;
            case PHASE.EVENT:
                if (NowSelectingCard == null)
                {
                    if (passwordCard.Model.IsOpened)
                    {
                        return false;
                    }
                    if (passwordCard.Model.HP <= passwordCard.Model.CanUseSkillHP)
                    {
                        return true;
                    }
                    return false;
                }
                switch (NowSelectingCard.Model.CardType)
                {
                    case CARD_TYPE.FIREWALL:
                        if (!passwordCard.Model.IsProtected)
                        {
                            return true;
                        }
                        return false;
                    case CARD_TYPE.EVENT:
                        EventCardController eventCard = (EventCardController)NowSelectingCard;
                        switch (eventCard.Model.EventCardType)
                        {
                            case EVENT_CARD_TYPE.MAINTENANCE:
                            case EVENT_CARD_TYPE.MAINTENANCE2:
                                if (passwordCard.Model.HP < Define.maxPasswordHP)
                                {
                                    return true;
                                }
                                return false;
                            case EVENT_CARD_TYPE.XSS:
                                if (passwordCard.Select.IsSelected)
                                {
                                    return false;
                                }
                                return true;
                            case EVENT_CARD_TYPE.RESTORATION:
                            case EVENT_CARD_TYPE.INFOMATION_LEAKAGE:
                                return true;
                            case EVENT_CARD_TYPE.PLUS_MINUS:
                                switch (action)
                                {
                                    case ACTION.ATTACK:
                                        return true;
                                    case ACTION.HEAL:
                                        if (passwordCard.Model.HP < Define.maxPasswordHP)
                                        {
                                            return true;
                                        }
                                        return false;
                                }
                                return false;
                            case EVENT_CARD_TYPE.FALSIFICATION:
                                if (passwordCard.Model.IsPlayerCard && (passwordCard.Model.HP < 50))
                                {
                                    return true;
                                }

                                if (!passwordCard.Model.IsPlayerCard && (passwordCard.Model.HP > 50))
                                {
                                    return true;
                                }
                                return false;
                            case EVENT_CARD_TYPE.RECONSTITUITION:
                            case EVENT_CARD_TYPE.OPTIMIZATION:
                            case EVENT_CARD_TYPE.CTL_Z:
                            case EVENT_CARD_TYPE.MIGRATION2:
                            case EVENT_CARD_TYPE.MIGRATION3:
                            case EVENT_CARD_TYPE.MIGRATION:
                            case EVENT_CARD_TYPE.RECOVERY:
                            case EVENT_CARD_TYPE.SCALE_UP:
                                return false;
                            case EVENT_CARD_TYPE.HEAVY_LOAD:
                                if (IsPlayerTurn == passwordCard.Model.IsPlayerCard)
                                {
                                    return true;
                                }
                                return false;
                            case EVENT_CARD_TYPE.WHITELIST:
                            case EVENT_CARD_TYPE.REBOOT:
                            case EVENT_CARD_TYPE.NARROW_BAND:
                            case EVENT_CARD_TYPE.MALWARE:
                                return false;
                        }
                        return false;
                    case CARD_TYPE.STAR:
                        StarCardController starCard = (StarCardController)NowSelectingCard;
                        switch (starCard.Model.EventCardType)
                        {
                            case EVENT_CARD_TYPE.PAIN_SPLIT:
                                return true;
                            case EVENT_CARD_TYPE.DISABLE_FIREWALL:
                            case EVENT_CARD_TYPE.VILUS_BECOME_FEROCIOUS:
                            case EVENT_CARD_TYPE.SERVER_REINFORCEMENT:
                            case EVENT_CARD_TYPE.EARLY_SETTLEMENT:
                            case EVENT_CARD_TYPE.BUFFER_OVERFLOW:
                            case EVENT_CARD_TYPE.BUFFER_OVERFLOW2:
                                return false;
                            case EVENT_CARD_TYPE.ZERO_DAY_ATTACK:
                                if (IsPlayerTurn == passwordCard.Model.IsPlayerCard)
                                {
                                    return true;
                                }
                                return false;
                            case EVENT_CARD_TYPE.ZERO_DAY_ATTACK2:
                                if (!passwordCard.Model.IsProtected)
                                {
                                    return true;
                                }
                                return false;
                            case EVENT_CARD_TYPE.FILTERING:
                            case EVENT_CARD_TYPE.UPDATE:
                                return false;
                        }
                        return false;
                    case CARD_TYPE.PASSWORD:
                        PasswordCardController openedPasswordCard = (PasswordCardController)NowSelectingCard;
                        switch (openedPasswordCard.Model.PasswordCardType)
                        {
                            case PASSWORD_CARD_TYPE.A_GYOU:
                            case PASSWORD_CARD_TYPE.KA_GYOU:
                            case PASSWORD_CARD_TYPE.SA_GYOU:
                            case PASSWORD_CARD_TYPE.TA_GYOU:
                                return false;
                            case PASSWORD_CARD_TYPE.NA_GYOU:
                                if (passwordCard.Model.HP < Define.maxPasswordHP)
                                {
                                    return true;
                                }
                                return false;
                            case PASSWORD_CARD_TYPE.HA_GYOU:
                                if (!passwordCard.Model.IsProtected)
                                {
                                    return true;
                                }
                                return false;
                            case PASSWORD_CARD_TYPE.MA_GYOU:
                                return false;
                            case PASSWORD_CARD_TYPE.YA_GYOU:
                                return true;
                            case PASSWORD_CARD_TYPE.RA_GYOU:
                            case PASSWORD_CARD_TYPE.WA_GYOU_N:
                                return false;
                        }
                        return false;
                }
                return false;
            case PHASE.ATTACK:
                switch (action)
                {
                    case ACTION.ATTACK:
                        if (IsPlayerTurn == passwordCard.Model.IsPlayerCard)
                        {
                            return true;
                        }

                        if (!passwordCard.Model.IsProtected)
                        {
                            return true;
                        }
                        return false;
                    case ACTION.HEAL:
                        if (!passwordCard.Model.IsHacked && (passwordCard.Model.HP < Define.maxPasswordHP))
                        {
                            return true;
                        }
                        return false;
                }
                return false;
        }
        return false;
    }

    /// <summary>
    /// 選択可能なカードリストを取得する
    /// </summary>
    public CardController[] GetSelectableCardList(CardController[] cardList)
    {
        FindSelectableCard(cardList);
        return Array.FindAll(cardList, card => (card.Select.IsSelectable));
    }

    /// <summary>
    /// プレイヤーの手札を取得する
    /// </summary>
    public CardController[] GetPlayerHands(bool isPlayer)
    {
        CardController[] handCardList;
        CardController[] selectableHandCardList;
        if (isPlayer)
        {
            handCardList = cardManager.playerHandTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            handCardList = cardManager.enemyHandTransform.GetComponentsInChildren<CardController>();
        }
        selectableHandCardList = Array.FindAll(handCardList, card => !card.Select.IsSelected);
        return selectableHandCardList;
    }

    /// <summary>
    /// カードを選択する
    /// </summary>
    public IEnumerator SelectCard(CardController[] selectableCardList)
    {
        // プレイヤーのターンなら手札カードを選択可能にする
        foreach (CardController card in selectableCardList)
        {
            if (IsPlayerTurn)
            {
                card.IsNotSelectableCard(false);
            }
            card.SetCanSelect(true);
        }

        if (IsPlayerTurn)
        {
            while (true)
            {
                yield return new WaitUntil(() => CardSelectflag);
                CardSelectflag = false;

                if (NowSelectingCard == null)
                {
                    break;
                }

                yield return StartCoroutine(detailPanelManager.ShowDetailPanel(NowSelectingCard));

                if (detailPanelManager.isSelectFlg)
                {
                    break;
                }

                if (!detailPanelManager.isSelectFlg)
                {
                    detailPanelManager.isSelectFlg = false;
                    NowSelectingCard = null;

                }
            }
        }
        else
        {
            //     switch (phase)
            //     {
            //         case PHASE.START:
            //             messageLogManager.ShowLog("【パスワード設定】相手がカードを選択中");
            //             break;
            //         case PHASE.MULLIGAN:
            //             messageLogManager.ShowLog("【マリガン】相手がカードを選択中");
            //             break;
            //         case PHASE.EVENT:
            //             messageLogManager.ShowLog("【イベントフェーズ】相手がカードを選択中");
            //             break;
            //         case PHASE.ATTACK:
            //             messageLogManager.ShowLog("【攻撃フェーズ】相手がカードを選択中");
            //             break;
            //         default:
            //             messageLogManager.ShowLog("相手がカードを選択中");
            //             break;
            //     }
            yield return new WaitForSeconds(3);
            int num = UnityEngine.Random.Range(0, selectableCardList.Length);
            Debug.Log((num + 1).ToString() + "番目のカードを選択");
            NowSelectingCard = selectableCardList[num];
        }

        foreach (CardController card in selectableCardList)
        {
            card.IsNotSelectableCard(true);
            card.SetCanSelect(false);
        }
    }

    /// <summary>
    /// パスワードカードを選択する
    /// </summary>
    public IEnumerator SelectPasswordCard()
    {
        Debug.Log($"SelectPasswordCard");
        for (int i = 0; i < Define.passwordLength; i++)
        {
            CardController[] playerHandCardList = GetPlayerHands(true);
            CardController[] selectablePasswordCardList = GetSelectableCardList(playerHandCardList);
            // messageLogManager.ShowLog("プレイヤーの選択可能なカード数 : " + selectablePasswordCardList.Length.ToString());
            // messageLogManager.ShowLog("残りの選択数 : " + (Define.passwordLength - i).ToString());
            yield return StartCoroutine(SelectCard(selectablePasswordCardList));
            PasswordCardController nowSelectingPasswordCard = (PasswordCardController)NowSelectingCard;
            SetPasswordCardToField(nowSelectingPasswordCard);
            // messageLogManager.ShowLog(nowSelectingPasswordCard.Model.Name + "カードを選択");
            yield return new WaitForSeconds(2.0f);
        }
        Debug.Log($"SelectPasswordCard End");
        DestroyPlayerHandCard(IsPlayerTurn);

        Debug.Log($"SelectPasswordCard End");
    }


    // 指定したパスワードカードをフィールド上にセットする



    /// <summary>
    /// パスワードカードをフィールドにセットする
    /// </summary>
    /// <param name="isPlayer"></param>
    /// <returns></returns>
    public PasswordCardController[] GetPlayerPasswordCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerPasswordFieldTransform.GetComponentsInChildren<PasswordCardController>();
        }
        else
        {
            return enemyPasswordFieldTransform.GetComponentsInChildren<PasswordCardController>();
        }
    }



    /// <summary>
    /// 敵のパスワードカードを決定する
    /// </summary>
    IEnumerator DecideEnemyPasswordCard()
    {
        messageLogManager.ShowLog("相手がパスワードを設定中…");
        int passwordID = UnityEngine.Random.Range(0, Define.enemyPasswordNum) + 1;
        Debug.Log(passwordID);
        EnemyPasswordEntity enemyPasswordEntity = Resources.Load<EnemyPasswordEntity>("EnemyPasswordEntityList/EnemyPassword" + passwordID);
        enemy.Password = enemyPasswordEntity.password;
        for (int i = 0; i < Define.passwordLength; i++)
        {
            CardController[] enemyHandCardList = GetPlayerHands(false);
            Destroy(enemyHandCardList[0].gameObject);
            PasswordCardController passwordCard = Instantiate(passwordCardPrefab, enemyPasswordFieldTransform, false);
            passwordCard.Init(5, passwordManager.GetEnemyPasswordID(enemy.Password[i]), false);
            passwordCard.SetPasswordCharacter(enemy.Password[i]);
            passwordCard.SetCanViewHP(true);
            yield return new WaitForSeconds(2);
        }
        DestroyPlayerHandCard(false);
    }

    /// <summary>
    /// プレイヤーの手札を破棄する
    /// </summary>
    void DestroyPlayerHandCard(bool isPlayer)
    {
        CardController[] handCard = GetPlayerHands(isPlayer);
        foreach (CardController card in handCard)
        {
            Destroy(card.gameObject);
        }
    }

    /// <summary>
    /// カードデッキを初期化する
    /// </summary>
    public void InitCardDeck()
    {
        cardDeck = new List<Card>() { new Card(2, 1), new Card(2, 1), new Card(2, 1),
                                      new Card(2, 2), new Card(2, 2), new Card(2, 2),
                                      new Card(3, 1), new Card(3, 1), new Card(3, 1),
                                      new Card(3, 2), new Card(3, 2), new Card(3, 2),
                                      new Card(3, 3), new Card(3, 3), new Card(3, 3),
                                      new Card(3, 4), new Card(3, 4), new Card(3, 4),
                                      new Card(3, 5), new Card(3, 5), new Card(3, 5),
                                      new Card(3, 6), new Card(3, 6), new Card(3, 6),
                                      new Card(3, 7), new Card(3, 7), new Card(3, 7),
                                      new Card(3, 8), new Card(3, 8), new Card(3, 8),
                                      new Card(3, 9), new Card(3, 9), new Card(3, 9),
                                      new Card(3, 10), new Card(3, 10), new Card(3, 10),
                                      new Card(3, 11), new Card(3, 11), new Card(3, 11),
                                      new Card(3, 12), new Card(3, 12), new Card(3, 12),
                                      new Card(3, 13), new Card(3, 13), new Card(3, 13),
                                      new Card(3, 14), new Card(3, 14), new Card(3, 14),
                                      new Card(3, 15), new Card(3, 15), new Card(3, 15),
                                      new Card(3, 16), new Card(3, 16), new Card(3, 16),
                                      new Card(3, 17), new Card(3, 17), new Card(3, 17),
                                      new Card(3, 18), new Card(3, 18), new Card(3, 18),
                                      new Card(3, 19), new Card(3, 19), new Card(3, 19),
                                      new Card(3, 20), new Card(3, 20), new Card(3, 20),
                                      new Card(4, 1), new Card(4, 2), new Card(4, 3),
                                      new Card(4, 4), new Card(4, 5), new Card(4, 6),
                                      new Card(4, 7), new Card(4, 8), new Card(4, 9),
                                      new Card(4, 10), new Card(4, 11)
                                    };

        ShuffleDeck(cardDeck);
        ShuffleDeck(cardDeck);
        ShuffleDeck(cardDeck);
    }

    /// <summary>
    /// 初期手札を配る
    /// </summary>
    public void InitHandCard()
    {
        //カードを5枚配る
        for (int i = 0; i < Define.maxHandCardNum; i++)
        {
            DrawCard(cardDeck, IsPlayerTurn);
            ChangeTurn();
            DrawCard(cardDeck, IsPlayerTurn);
            ChangeTurn();
        }
    }

    /// <summary>
    /// イベントフェーズの処理
    /// </summary>
    IEnumerator EventPhase()
    {
        phase = PHASE.EVENT;

        //カードを最大3枚選択
        int selectCardNum;

        if (IsPlayerTurn)
        {
            selectCardNum = Define.selectableCardNum;
        }
        else
        {
            //1~3までの値ランダム
            selectCardNum = (UnityEngine.Random.Range(0, Define.selectableCardNum) + 1);
        }

        for (int i = selectCardNum; i > 0; i--)
        {
            //手札とパスワードカードを取得
            CardController[] selectableHandCardList = GetSelectableCardList(GetPlayerHands(IsPlayerTurn));
            CardController[] selectablePasswordCardList = GetSelectableCardList(GetPlayerPasswordCards(IsPlayerTurn));
            CardController[] selectableEventPhaseCardList = MergeCardList(selectableHandCardList, selectablePasswordCardList);

            if (selectableEventPhaseCardList.Length == 0)
            {
                //選択可能なカードがなければ終了
                messageLogManager.ShowLog("選択可能なカードがありません");
                yield break;
            }

            if (IsPlayerTurn)
            {
                if (i < Define.selectableCardNum)
                {
                    SetPhaseEndButtonActive(true);
                }
            }

            Debug.Log(NowSelectingCard);
            yield return StartCoroutine(SelectCard(selectableEventPhaseCardList));
            Debug.Log(NowSelectingCard);

            if (IsPlayerTurn && phaseEndButton.activeSelf)
            {
                SetPhaseEndButtonActive(false);
            }

            if (NowSelectingCard == null)
            {
                yield break;
            }

            CardController selectedCard = NowSelectingCard;
            selectedCard.SetIsSelect(true);

            //選択したカードの処理
            yield return StartCoroutine(selectedCard.UseCard());

            //選択したイベントカードを使用済みカード置き場に送る
            if (selectedCard.Model.CardType == CARD_TYPE.EVENT)
            {
                SetUsedCardToUsedCardField(selectedCard);
            }

            NowSelectingCard = null;
        }
    }

    /// <summary>
    /// アタックフェーズの処理
    /// </summary>
    IEnumerator AttackPhase()
    {
        phase = PHASE.ATTACK;
        //ブルートフォースカードを取得
        CardController[] bruteForceCardList = cardManager.GetBruteForceCards();
        CardController[] selectableCardList = GetSelectableCardList(bruteForceCardList);
        int num = GetAttackNum();
        if (IsPlayerTurn)
        {
            bruteForceCardPanel.SetActive(true);
            navigationMessagesManager.ShowNavigationMessage("プレイヤーのターンです。\n3枚のカードの中から攻撃カードを選択してください。");
            messageLogManager.ShowLog("プレイヤーのターンです。\n3枚のカードの中から攻撃カードを選択してください。", false, 0.05f, 0f);

            if (!player.HackingFailedflag)
            {
                SetHackingButtonActive(true);
            }
        }
        else
        {
            if (enemy.UseInfomationLeakageCardflag)
            {
                enemy.Hackingflag = true;
                yield break;
            }
        }
        while (num > 0)
        {
            //ブルートフォースカードを選択
            yield return StartCoroutine(SelectCard(selectableCardList));
            if (IsPlayerTurn && hackingButton.activeSelf)
            {
                SetHackingButtonActive(false);
            }
            if (NowSelectingCard == null)
            {
                yield break;
            }
            CardController selectedBruteForceCard = NowSelectingCard;
            Debug.Log(selectedBruteForceCard.Model.Name);
            //選択したカードの処理
            yield return StartCoroutine(selectedBruteForceCard.UseCard());
            num--;
        }

        //プレイヤーのパスワードカードを取得
        PasswordCardController[] playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
        foreach (PasswordCardController card in playerPasswordCardList)
        {
            //さ行カードのとき
            if (card.Model.PasswordCardType == PASSWORD_CARD_TYPE.SA_GYOU)
            {
                //表になっていてスキル発動可能HP以下になっているな行カードのみ
                if (card.Model.IsHacked || !card.Model.IsOpened || (card.Model.HP > card.Model.CanUseSkillHP)) continue;

                //前のターンに相手がDoS攻撃カードを使用してなければ効果発動
                if (IsPlayerTurn && enemy.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("相手のDoS攻撃カードの効果でプレイヤーのさ行カードの効果は無効化された！");
                    break;
                }

                //前のターンにプレイヤーがDoS攻撃カードを使用してなければ効果発動
                if (!IsPlayerTurn && player.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("プレイヤーのDoS攻撃カードの効果で相手のさ行カードの効果は無効化された！");
                    break;
                }

                NowSelectingCard = card;

                //敵のファイアウォールカードを取得
                FirewallCardController[] enemyFirewallCardList = GetPlayerFirewallCards(!IsPlayerTurn);

                //ファイアウォールカードがなければ終了
                if (enemyFirewallCardList.Length <= 0)
                {
                    break;
                }

                //ファイアウォールカードを選択
                yield return StartCoroutine(SelectCard(enemyFirewallCardList));
                FirewallCardController selectedFirewallCard = (FirewallCardController)NowSelectingCard;
                //10ダメージ
                selectedFirewallCard.BruteForceAttack(card.Model.Attack);
            }
        }

        NowSelectingCard = null;
    }


    /// <summary>
    /// プレイヤーのファイアウォールカードを取得する
    /// </summary>
    public FirewallCardController[] GetPlayerFirewallCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerFirewallCardFieldTransform.GetComponentsInChildren<FirewallCardController>();
        }
        else
        {
            return enemyFirewallCardFieldTransform.GetComponentsInChildren<FirewallCardController>();
        }
    }

    /// <summary>
    /// スターカードを取得する
    /// </summary>
    public StarCardController GetStarCard()
    {
        StarCardController[] starCardList = starCardFieldTransform.GetComponentsInChildren<StarCardController>();
        Debug.Log(starCardList.Length);
        if (starCardList.Length <= 0)
        {
            return null;
        }
        else if (starCardList.Length == 1)
        {
            return starCardList[0];
        }
        else
        {
            for (int i = 1; i < starCardList.Length; i++)
            {
                SetUsedCardToUsedCardField(starCardList[i]);
            }
            return starCardList[0];
        }
    }

    /// <summary>
    /// 使用済みカードリストを取得する
    /// </summary>
    public CardController[] GetUsedCardList()
    {
        Debug.Log($"GetUsedCardList");
        return usedCardFieldTransform.GetComponentsInChildren<CardController>();
    }

    /// <summary>
    /// カードリストをマージする
    /// </summary>
    public CardController[] MergeCardList(CardController[] cardList1, CardController[] cardList2)
    {
        CardController[] mergeCardList = new CardController[cardList1.Length + cardList2.Length];
        Array.Copy(cardList1, mergeCardList, cardList1.Length);
        Array.Copy(cardList2, 0, mergeCardList, cardList1.Length, cardList2.Length);

        return mergeCardList;
    }

    /// <summary>
    /// パスワードカードをフィールドにセットする
    /// </summary>
    public void SetPasswordCardToField(PasswordCardController passwordCard)
    {
        if (passwordCard.Model.IsPlayerCard)
        {
            passwordCard.transform.SetParent(playerPasswordFieldTransform);
            passwordCard.TurnDown();
        }
        else
        {
            passwordCard.transform.SetParent(enemyPasswordFieldTransform);
        }
        passwordCard.SetCanViewHP(true);
    }

    /// <summary>
    /// ファイアウォールカードをフィールドにセットする
    /// </summary>
    public void SetFirewallCardToField(FirewallCardController firewallCard, Vector3 passwordPos)
    {
        if (firewallCard.Model.IsPlayerCard)
        {
            firewallCard.transform.SetParent(playerFirewallCardFieldTransform);
        }
        else
        {
            firewallCard.transform.SetParent(enemyFirewallCardFieldTransform);
            firewallCard.Show();
        }
        RectTransform firewallRectTransform = firewallCard.gameObject.GetComponent<RectTransform>();
        Vector3 firewallPos = firewallRectTransform.anchoredPosition;
        firewallPos.x = passwordPos.x;
        firewallPos.y = passwordPos.y;
        //firewallRectTransform.anchoredPosition = firewallPos;
        firewallCard.MoveCard(firewallPos);
        //firewallRectTransform.DOAnchorPos(firewallPos, 0.5f);
        firewallCard.SetCanViewHP(true);
    }

    /// <summary>
    /// スターカードをフィールドにセットする
    /// </summary>
    public void SetStarCardToField(StarCardController starCard)
    {
        StarCardController usedStarCard = GetStarCard();
        if (usedStarCard != null)
        {
            SetUsedCardToUsedCardField(usedStarCard);
        }

        starCard.transform.SetParent(starCardFieldTransform);
        if (!starCard.Model.IsPlayerCard)
        {
            starCard.Show();
        }

        ShowFieldEffectButton(starCard);
    }

    /// <summary>
    /// イベントフェーズを終了する
    /// </summary>
    public void EndEventPhase()
    {
        NowSelectingCard = null;
        CardSelectflag = true;
    }

    /// <summary>
    /// プレイヤーの選択可能なカードリストを取得する
    /// </summary>
    public CardController[] GetPlayerSelectableCardList(bool isPlayer)
    {
        CardController[] playerPasswordCardList = GetPlayerPasswordCards(isPlayer);
        CardController[] selectablePasswordCardList = GetSelectableCardList(playerPasswordCardList);
        return selectablePasswordCardList;
    }

    /// <summary>
    /// 回復可能なカードリストを取得する
    /// </summary>
    public CardController[] GetHealableCardList(bool isPlayer)
    {
        action = GameManager.ACTION.HEAL;
        CardController[] passwordCardList = GetPlayerPasswordCards(isPlayer);
        CardController[] selectablePasswordCardList = GetSelectableCardList(passwordCardList);
        return selectablePasswordCardList;
    }

    /// <summary>
    /// 攻撃可能なカードリストを取得する
    /// </summary>
    public CardController[] GetAttackableCardList(bool isPlayer)
    {
        action = GameManager.ACTION.ATTACK;
        CardController[] passwordCardList = GetPlayerPasswordCards(isPlayer);
        CardController[] selectablePasswordCardList = GetSelectableCardList(passwordCardList);
        CardController[] firewallCardList = GetPlayerFirewallCards(isPlayer);
        CardController[] selectableFirewallCardList = GetSelectableCardList(firewallCardList);
        return MergeCardList(selectablePasswordCardList, selectableFirewallCardList);
    }

    /// <summary>
    /// すべてのパスワードカードを取得する
    /// </summary>
    public PasswordCardController[] GetAllPasswordCard()
    {
        CardController[] playerPasswordCardList = GetPlayerPasswordCards(true);
        CardController[] selectablePlayerCardList = GetSelectableCardList(playerPasswordCardList);
        CardController[] enemyPasswordCardList = GetPlayerPasswordCards(false);
        CardController[] selectableEnemyCardList = GetSelectableCardList(enemyPasswordCardList);
        CardController[] passwordCardList = MergeCardList(selectablePlayerCardList, selectableEnemyCardList);
        PasswordCardController[] allPasswordCard = new PasswordCardController[passwordCardList.Length];
        passwordCardList.CopyTo(allPasswordCard, 0);
        return allPasswordCard;
    }

    /// <summary>
    /// 使用済みカードを使用済みカード置き場にセットする
    /// </summary>
    public void SetUsedCardToUsedCardField(CardController usedCard)
    {
        Debug.Log($"SetUsedCardToUsedCardField");
        usedCard.transform.SetParent(usedCardFieldTransform);
        if (!usedCard.Model.IsPlayerCard)
        {
            usedCard.Show();
        }
    }

    /// <summary>
    /// 使用済みカードリストからカードを取得する
    /// </summary>
    public IEnumerator GetCardFromUsedCardList()
    {
        int cardType;
        int cardID;

        CardController[] usedCardList = GetUsedCardList();
        usedCardFieldTransform.gameObject.SetActive(true);
        yield return StartCoroutine(SelectCard(usedCardList));
        usedCardFieldTransform.gameObject.SetActive(false);
        Debug.Log(NowSelectingCard);
        CardController selectedCard = NowSelectingCard;
        cardType = (int)selectedCard.Model.CardType;
        switch (selectedCard.Model.CardType)
        {
            case CARD_TYPE.FIREWALL:
                FirewallCardController firewallCard = (FirewallCardController)selectedCard;
                cardID = (int)firewallCard.Model.FirewallCardType;
                break;
            case CARD_TYPE.EVENT:
                EventCardController eventCard = (EventCardController)selectedCard;
                cardID = (int)eventCard.Model.EventCardType;
                break;
            case CARD_TYPE.STAR:
                StarCardController starCard = (StarCardController)selectedCard;
                cardID = (int)starCard.Model.EventCardType - Define.eventCardNum;
                break;
            default:
                yield break;
        }
        if (IsPlayerTurn)
        {
            cardManager.CreateCard(cardType, cardID, cardManager.playerHandTransform);
        }
        else
        {
            cardManager.CreateCard(cardType, cardID, cardManager.enemyHandTransform);
        }
    }

    /// <summary>
    /// 攻撃回数を取得する
    /// </summary>
    int GetAttackNum()
    {
        int num = 1;
        StarCardController starCard = GetStarCard();

        if (starCard != null)
        {
            if (starCard.Model.EventCardType == EVENT_CARD_TYPE.SERVER_REINFORCEMENT)
            {
                num++;
            }
        }

        if (IsPlayerTurn && player.UseHeavyLoadCardNum > 0)
        {
            num += player.UseHeavyLoadCardNum;
        }

        if (!IsPlayerTurn && enemy.UseHeavyLoadCardNum > 0)
        {
            num += enemy.UseHeavyLoadCardNum;
        }

        //プレイヤーのわ行カードが表になったとき
        if (player.OpenWaGyouNCardflag)
        {
            //プレイヤーの表になったパスワードカードを取得
            PasswordCardController[] playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
            PasswordCardController[] openedPleyerPasswordCardList = Array.FindAll(playerPasswordCardList, card => card.Model.IsOpened);
            num += openedPleyerPasswordCardList.Length;
            player.OpenWaGyouNCardflag = false;
        }

        //敵のわ行カードが表になったとき
        if (enemy.OpenWaGyouNCardflag)
        {
            //敵の表になったパスワードカードを取得
            PasswordCardController[] enemyPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
            PasswordCardController[] openedEnemyPasswordCardList = Array.FindAll(enemyPasswordCardList, card => card.Model.IsOpened);
            num += openedEnemyPasswordCardList.Length;
            enemy.OpenWaGyouNCardflag = false;
        }

        return num;
    }

    /// <summary>
    /// カードをカードデッキに追加する
    /// </summary>
    public void AddCardToCardDeck(CardController card)
    {
        Debug.Log($"AddCardToCardDeck");

        int cardType;
        int cardID;

        cardType = (int)card.Model.CardType;
        switch (card.Model.CardType)
        {
            case CARD_TYPE.FIREWALL:
                FirewallCardController firewallCard = (FirewallCardController)card;
                cardID = (int)firewallCard.Model.FirewallCardType;
                break;
            case CARD_TYPE.EVENT:
                EventCardController eventCard = (EventCardController)card;
                cardID = (int)eventCard.Model.EventCardType;
                break;
            case CARD_TYPE.STAR:
                StarCardController starCard = (StarCardController)card;
                cardID = (int)starCard.Model.EventCardType - Define.eventCardNum;
                break;
            default:
                return;
        }
        cardDeck.Add(new Card(cardType, cardID));
        Destroy(card.gameObject);
        Debug.Log(card);
    }

    /// <summary>
    /// フェーズ終了ボタンをアクティブにする
    /// </summary>
    public void SetPhaseEndButtonActive(bool isActive)
    {
        phaseEndButton.SetActive(isActive);
    }

    /// <summary>
    /// フェーズ終了ボタンがアクティブかどうかを取得する
    /// </summary>
    public bool GetPhaseEndButtonActive()
    {
        return phaseEndButton.activeSelf;
    }

    /// <summary>
    /// マイグレーション3カードをフィールドにセットする
    /// </summary>
    public void SetMigration3CardToField(CardController card)
    {
        if (card.Model.IsPlayerCard)
        {
            card.transform.SetParent(playerMigration3CardFieldTransform);
        }
        else
        {
            card.transform.SetParent(enemyMigration3CardFieldTransform);
        }
    }

    /// <summary>
    /// マイグレーション3カードを取得する
    /// </summary>
    public CardController[] GetMigration3Cards(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerMigration3CardFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return enemyMigration3CardFieldTransform.GetComponentsInChildren<CardController>();
        }
    }

    /// <summary>
    /// カードをプレイヤーの手札にセットする
    /// </summary>
    public void SetCardToPlayerHand(CardController card)
    {
        if (card.Model.IsPlayerCard)
        {
            card.transform.SetParent(cardManager.playerHandTransform);
        }
        else
        {
            card.transform.SetParent(cardManager.enemyHandTransform);
        }
    }

    /// <summary>
    /// ターン終了フラグをリセットする
    /// </summary>
    public void TurnEndflagReset(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            player.UseInfomationLeakageCardflag = false;
            player.UseHeavyLoadCardNum = 0;
            player.UseWhiteListCardflag = false;
            if (player.UseRebootCardflag)
            {
                //すべての環境カードを山札に戻してシャッフル
                Reboot();
                player.UseRebootCardflag = false;
            }

            player.RecoveryCardHealBuff = 0;
            player.ScaleUpCardAttackBuff = 0;

            if (enemy.UseNarrowBandCardflag)
            {
                enemy.UseNarrowBandCardflag = false;
            }
            if (enemy.UseDoSAttackCardflag)
            {
                enemy.UseDoSAttackCardflag = false;
            }
        }
        else
        {
            enemy.UseInfomationLeakageCardflag = false;
            enemy.UseHeavyLoadCardNum = 0;
            enemy.UseWhiteListCardflag = false;
            if (enemy.UseRebootCardflag)
            {
                //すべての環境カードを山札に戻してシャッフル
                Reboot();
                enemy.UseRebootCardflag = false;
            }

            enemy.RecoveryCardHealBuff = 0;
            enemy.ScaleUpCardAttackBuff = 0;

            if (player.UseNarrowBandCardflag)
            {
                player.UseNarrowBandCardflag = false;
            }
            if (player.UseDoSAttackCardflag)
            {
                player.UseDoSAttackCardflag = false;
            }
        }
    }

    /// <summary>
    /// リブート処理
    /// </summary>
    void Reboot()
    {
        CardController[] playerHandCardList = GetPlayerHands(IsPlayerTurn);
        CardController[] enemyHandCardList = GetPlayerHands(!IsPlayerTurn);
        CardController[] usedCardList = GetUsedCardList();
        //プレイヤーの手札にある環境カード
        CardController[] playerStarCardList = Array.FindAll(playerHandCardList, card => (card.Model.CardType == CARD_TYPE.STAR));
        //敵の手札にある環境カード
        CardController[] enemyStarCardList = Array.FindAll(enemyHandCardList, card => (card.Model.CardType == CARD_TYPE.STAR));
        //使用済みカード置き場にある環境カード
        CardController[] usedStarCardList = Array.FindAll(usedCardList, card => (card.Model.CardType == CARD_TYPE.STAR));
        //場に出ている環境カード
        CardController starCard = GetStarCard();

        //すべての環境カードを山札に戻す
        if (playerStarCardList.Length > 0)
        {
            foreach (CardController card in playerStarCardList)
            {
                AddCardToCardDeck(card);
            }
        }
        if (enemyStarCardList.Length > 0)
        {
            foreach (CardController card in enemyStarCardList)
            {
                AddCardToCardDeck(card);
            }
        }
        if (usedStarCardList.Length > 0)
        {
            foreach (CardController card in usedStarCardList)
            {
                AddCardToCardDeck(card);
            }
        }
        if (starCard != null)
        {
            AddCardToCardDeck(starCard);
            DisableFieldEffectButton();
        }
        //シャッフル
        ShuffleDeck(cardDeck);
        ShuffleDeck(cardDeck);
        ShuffleDeck(cardDeck);
    }

    /// <summary>
    /// マイグレーション3フィールドを表示する
    /// </summary>
    public void ShowMigration3Field(bool isVisible)
    {
        if (IsPlayerTurn)
        {
            playerPasswordFieldTransform.gameObject.SetActive(!isVisible);
            playerMigration3CardFieldTransform.gameObject.SetActive(isVisible);
        }
        else
        {
            enemyPasswordFieldTransform.gameObject.SetActive(!isVisible);
            enemyMigration3CardFieldTransform.gameObject.SetActive(isVisible);
        }
    }

    /// <summary>
    /// マイグレーション3カードをドローする
    /// </summary>
    public void DrawMigration3Card(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            GiveCardToHand(cardDeck, playerMigration3CardFieldTransform);
        }
        else
        {
            GiveCardToHand(cardDeck, enemyMigration3CardFieldTransform);
        }
    }

    /// <summary>
    /// デッキカードをデッキカードリストに移動する
    /// </summary>
    public void DeckCardMoveToDeckCardList()
    {
        deckCardFieldTransform.gameObject.SetActive(true);
        for (int i = 0; i < cardDeck.Count; i++)
        {
            int cardType = cardDeck[i].cardType;
            int cardID = cardDeck[i].cardID;
            cardManager.CreateCard(cardType, cardID, deckCardFieldTransform);
        }
        deckCardFieldTransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// デッキカードリストを取得する
    /// </summary>
    public CardController[] GetDeckCardList()
    {
        return deckCardFieldTransform.GetComponentsInChildren<CardController>();
    }

    /// <summary>
    /// カードデッキからカードを取得する
    /// </summary>
    public IEnumerator GetCardFromCardDeck()
    {
        //山札のカードを取得
        CardController[] deckCardList = GetDeckCardList();
        deckCardFieldTransform.gameObject.SetActive(true);
        //山札のカードを選択
        yield return StartCoroutine(SelectCard(deckCardList));
        deckCardFieldTransform.gameObject.SetActive(false);
        Debug.Log(NowSelectingCard);
        CardController selectedCard = NowSelectingCard;
        //選択したカードを山札から削除
        int num = Array.IndexOf(deckCardList, selectedCard);
        DeckDebug(selectedCard);
        Debug.Log("カードの種類 : " + cardDeck[num].cardType + " " + "カードID : " + cardDeck[num].cardID);
        cardDeck.RemoveAt(num);
        //選択したカードを自分の手札に移動
        SetCardToPlayerHand(selectedCard);
    }

    /// <summary>
    /// デッキカードリストを破棄する
    /// </summary>
    public void DestroyDeckCardList()
    {
        CardController[] deckCard = GetDeckCardList();
        foreach (CardController card in deckCard)
        {
            Destroy(card.gameObject);
        }
    }

    /// <summary>
    /// デッキデバッグ
    /// </summary>
    void DeckDebug(CardController card)
    {
        int cardType = (int)card.Model.CardType;
        int cardID;
        switch (card.Model.CardType)
        {
            case CARD_TYPE.FIREWALL:
                FirewallCardController firewallCard = (FirewallCardController)card;
                cardID = (int)firewallCard.Model.FirewallCardType;
                break;
            case CARD_TYPE.EVENT:
                EventCardController eventCard = (EventCardController)card;
                cardID = (int)eventCard.Model.EventCardType;
                break;
            case CARD_TYPE.STAR:
                StarCardController starCard = (StarCardController)card;
                cardID = (int)starCard.Model.EventCardType - Define.eventCardNum;
                break;
            default:
                cardID = 0;
                break;
        }
        Debug.Log("カードの種類 : " + cardType + " " + "カードID : " + cardID);
    }

    /// <summary>
    /// ハッキングボタンをアクティブにする
    /// </summary>
    public void SetHackingButtonActive(bool isActive)
    {
        hackingButton.SetActive(isActive);
    }

    /// <summary>
    /// 攻撃フェーズを終了する
    /// </summary>
    public void EndAttackPhase()
    {
        NowSelectingCard = null;
        CardSelectflag = true;
        player.Hackingflag = true;
    }

    /// <summary>
    /// 敵のパスワードをハッキングする
    /// </summary>
    public IEnumerator HackingEnemyPassword()
    {
        if (player.HackingFailedflag)
        {
            yield break;
        }

        PasswordCardController[] enemyPasswordCardList = GetPlayerPasswordCards(!IsPlayerTurn);
        foreach (PasswordCardController passwordCard in enemyPasswordCardList)
        {
            passwordCard.transform.SetParent(hackingPasswordFieldTransform);
        }

        hackingPasswordPanel.SetActive(true);
        messageLogManager.ShowLog("相手のパスワードを入力してください");
        yield return new WaitUntil(() => InputCorrectPasswordflag);
        InputCorrectPasswordflag = false;
        hackingPasswordPanel.SetActive(false);

        foreach (PasswordCardController passwordCard in enemyPasswordCardList)
        {
            passwordCard.transform.SetParent(enemyPasswordFieldTransform);
        }
    }

    /// <summary>
    /// プレイヤーのパスワードをハッキングする
    /// </summary>
    public void HackingPlayerPassword()
    {
        if (enemy.HackingFailedflag)
        {
            return;
        }

        String guessPlayerPassword = "";

        PasswordCardController[] playerPasswordCardList = GetPlayerPasswordCards(IsPlayerTurn);
        for (int i = 0; i < Define.passwordLength; i++)
        {
            if (playerPasswordCardList[i].Model.IsHacked)
            {
                guessPlayerPassword += player.Password[i].ToString();
            }
            else
            {
                guessPlayerPassword += GuessPlayerPassword(playerPasswordCardList[i]).ToString();
            }
        }

        if (guessPlayerPassword == player.Password)
        {
            enemy.Winflag = true;
        }
        else
        {
            enemy.HackingFailedflag = true;
        }
    }

    /// <summary>
    /// プレイヤーのパスワードを推測する
    /// </summary>
    public char GuessPlayerPassword(PasswordCardController passwordCard)
    {
        int randNum;

        if (passwordCard.Model.IsOpened)
        {
            switch (passwordCard.Model.PasswordCardType)
            {
                case PASSWORD_CARD_TYPE.A_GYOU:
                    randNum = UnityEngine.Random.Range(0, 10);
                    switch (randNum)
                    {
                        case 0:
                            return 'あ';
                        case 1:
                            return 'い';
                        case 2:
                            return 'う';
                        case 3:
                            return 'え';
                        case 4:
                            return 'お';
                        case 5:
                            return 'ぁ';
                        case 6:
                            return 'ぃ';
                        case 7:
                            return 'ぅ';
                        case 8:
                            return 'ぇ';
                        case 9:
                            return 'ぉ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.KA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 10);
                    switch (randNum)
                    {
                        case 0:
                            return 'か';
                        case 1:
                            return 'き';
                        case 2:
                            return 'く';
                        case 3:
                            return 'け';
                        case 4:
                            return 'こ';
                        case 5:
                            return 'が';
                        case 6:
                            return 'ぎ';
                        case 7:
                            return 'ぐ';
                        case 8:
                            return 'げ';
                        case 9:
                            return 'ご';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.SA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 10);
                    switch (randNum)
                    {
                        case 0:
                            return 'さ';
                        case 1:
                            return 'し';
                        case 2:
                            return 'す';
                        case 3:
                            return 'せ';
                        case 4:
                            return 'そ';
                        case 5:
                            return 'ざ';
                        case 6:
                            return 'じ';
                        case 7:
                            return 'ず';
                        case 8:
                            return 'ぜ';
                        case 9:
                            return 'ぞ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.TA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 11);
                    switch (randNum)
                    {
                        case 0:
                            return 'た';
                        case 1:
                            return 'ち';
                        case 2:
                            return 'つ';
                        case 3:
                            return 'て';
                        case 4:
                            return 'と';
                        case 5:
                            return 'だ';
                        case 6:
                            return 'ぢ';
                        case 7:
                            return 'づ';
                        case 8:
                            return 'で';
                        case 9:
                            return 'ど';
                        case 10:
                            return 'っ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.NA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 5);
                    switch (randNum)
                    {
                        case 0:
                            return 'な';
                        case 1:
                            return 'に';
                        case 2:
                            return 'ぬ';
                        case 3:
                            return 'ね';
                        case 4:
                            return 'の';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.HA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 15);
                    switch (randNum)
                    {
                        case 0:
                            return 'は';
                        case 1:
                            return 'ひ';
                        case 2:
                            return 'ふ';
                        case 3:
                            return 'へ';
                        case 4:
                            return 'ほ';
                        case 5:
                            return 'ば';
                        case 6:
                            return 'び';
                        case 7:
                            return 'ぶ';
                        case 8:
                            return 'べ';
                        case 9:
                            return 'ぼ';
                        case 10:
                            return 'ぱ';
                        case 11:
                            return 'ぴ';
                        case 12:
                            return 'ぷ';
                        case 13:
                            return 'ぺ';
                        case 14:
                            return 'ぽ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.MA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 5);
                    switch (randNum)
                    {
                        case 0:
                            return 'ま';
                        case 1:
                            return 'み';
                        case 2:
                            return 'む';
                        case 3:
                            return 'め';
                        case 4:
                            return 'も';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.YA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 6);
                    switch (randNum)
                    {
                        case 0:
                            return 'や';
                        case 1:
                            return 'ゆ';
                        case 2:
                            return 'よ';
                        case 3:
                            return 'ゃ';
                        case 4:
                            return 'ゅ';
                        case 5:
                            return 'ょ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.RA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 5);
                    switch (randNum)
                    {
                        case 0:
                            return 'ら';
                        case 1:
                            return 'り';
                        case 2:
                            return 'る';
                        case 3:
                            return 'れ';
                        case 4:
                            return 'ろ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.WA_GYOU_N:
                    randNum = UnityEngine.Random.Range(0, 6);
                    switch (randNum)
                    {
                        case 0:
                            return 'わ';
                        case 1:
                            return 'ゐ';
                        case 2:
                            return 'ゑ';
                        case 3:
                            return 'を';
                        case 4:
                            return 'ん';
                        case 5:
                            return 'ゎ';
                    }
                    return '?';
            }
            return '?';
        }
        else
        {
            randNum = UnityEngine.Random.Range(0, 83);
            switch (randNum)
            {
                case 0:
                    return 'あ';
                case 1:
                    return 'い';
                case 2:
                    return 'う';
                case 3:
                    return 'え';
                case 4:
                    return 'お';
                case 5:
                    return 'ぁ';
                case 6:
                    return 'ぃ';
                case 7:
                    return 'ぅ';
                case 8:
                    return 'ぇ';
                case 9:
                    return 'ぉ';
                case 10:
                    return 'か';
                case 11:
                    return 'き';
                case 12:
                    return 'く';
                case 13:
                    return 'け';
                case 14:
                    return 'こ';
                case 15:
                    return 'が';
                case 16:
                    return 'ぎ';
                case 17:
                    return 'ぐ';
                case 18:
                    return 'げ';
                case 19:
                    return 'ご';
                case 20:
                    return 'さ';
                case 21:
                    return 'し';
                case 22:
                    return 'す';
                case 23:
                    return 'せ';
                case 24:
                    return 'そ';
                case 25:
                    return 'ざ';
                case 26:
                    return 'じ';
                case 27:
                    return 'ず';
                case 28:
                    return 'ぜ';
                case 29:
                    return 'ぞ';
                case 30:
                    return 'た';
                case 31:
                    return 'ち';
                case 32:
                    return 'つ';
                case 33:
                    return 'て';
                case 34:
                    return 'と';
                case 35:
                    return 'だ';
                case 36:
                    return 'ぢ';
                case 37:
                    return 'づ';
                case 38:
                    return 'で';
                case 39:
                    return 'ど';
                case 40:
                    return 'っ';
                case 41:
                    return 'な';
                case 42:
                    return 'に';
                case 43:
                    return 'ぬ';
                case 44:
                    return 'ね';
                case 45:
                    return 'の';
                case 46:
                    return 'は';
                case 47:
                    return 'ひ';
                case 48:
                    return 'ふ';
                case 49:
                    return 'へ';
                case 50:
                    return 'ほ';
                case 51:
                    return 'ば';
                case 52:
                    return 'び';
                case 53:
                    return 'ぶ';
                case 54:
                    return 'べ';
                case 55:
                    return 'ぼ';
                case 56:
                    return 'ぱ';
                case 57:
                    return 'ぴ';
                case 58:
                    return 'ぷ';
                case 59:
                    return 'ぺ';
                case 60:
                    return 'ぽ';
                case 61:
                    return 'ま';
                case 62:
                    return 'み';
                case 63:
                    return 'む';
                case 64:
                    return 'め';
                case 65:
                    return 'も';
                case 66:
                    return 'や';
                case 67:
                    return 'ゆ';
                case 68:
                    return 'よ';
                case 69:
                    return 'ゃ';
                case 70:
                    return 'ゅ';
                case 71:
                    return 'ょ';
                case 72:
                    return 'ら';
                case 73:
                    return 'り';
                case 74:
                    return 'る';
                case 75:
                    return 'れ';
                case 76:
                    return 'ろ';
                case 77:
                    return 'わ';
                case 78:
                    return 'ゐ';
                case 79:
                    return 'ゑ';
                case 80:
                    return 'を';
                case 81:
                    return 'ん';
                case 82:
                    return 'ゎ';
            }
            return '?';
        }
    }

    /// <summary>
    /// デッキをリセットする
    /// </summary>
    public void DeckReset()
    {
        Debug.Log($"DeckReset デッキの枚数 : {cardDeck.Count}");
        CardController[] usedCardList = GetUsedCardList();
        foreach (CardController card in usedCardList)
        {
            Debug.Log($"使用済みカード : {card.Model.Name}");
            AddCardToCardDeck(card);
        }
        ShuffleDeck(cardDeck);
        ShuffleDeck(cardDeck);
        ShuffleDeck(cardDeck);
    }

    /// <summary>
    /// 勝利判定を行う
    /// </summary>
    public void WinJudgement(bool isPlayerCard)
    {
        PasswordCardController[] passwordCardList = GetPlayerPasswordCards(isPlayerCard);
        foreach (PasswordCardController passwordCard in passwordCardList)
        {
            if (!passwordCard.Model.IsHacked)
            {
                return;
            }
        }
        if (isPlayerCard)
        {
            enemy.Winflag = true;
            Debug.Log("敵の勝ちです");
        }
        else
        {
            player.Winflag = true;
            Debug.Log("プレイヤーの勝ちです");
        }
        SceneManager.LoadScene("Result");
    }

    /// <summary>
    /// マリガン処理（最初の手札の一部を交換する）
    /// </summary>
    public IEnumerator Mulligan()
    {
        // プレイヤーのターンのとき
        if (IsPlayerTurn)
        {
            messageLogManager.ShowLog("【マリガンフェーズ】\n捨てるカードを選んでください。\n終わったら「完了」ボタンを押してください。");
            messageLogManager.HideLog(1f);
            SetPhaseEndButtonActive(true);
        }
        else
        {
            messageLogManager.ShowLog("【相手のマリガンフェーズ】\n相手がいらないカードを捨てています…");
            SetPhaseEndButtonActive(false);
        }

        //カードを最大5枚選択
        int selectCardNum;
        int drawCardNum = 0;
        if (IsPlayerTurn)
        {
            selectCardNum = Define.maxHandCardNum;
        }
        else
        {
            //1~5までの値ランダム
            selectCardNum = (UnityEngine.Random.Range(0, Define.maxHandCardNum) + 1);
        }

        //選択したカードを捨てる
        for (int i = selectCardNum; i > 0; i--)
        {
            //手札とパスワードカードを取得
            CardController[] handCardList = GetPlayerHands(IsPlayerTurn);

            // カードを選択
            yield return StartCoroutine(SelectCard(handCardList));

            if (NowSelectingCard == null)
            {
                break;
            }
            drawCardNum++;
            CardController selectedCard = NowSelectingCard;

            //選択したカードを使用済みカード置き場に送る
            SetUsedCardToUsedCardField(selectedCard);
            NowSelectingCard = null;
        }

        for (int i = drawCardNum; i > 0; i--)
        {
            DrawCard(cardDeck, IsPlayerTurn);
        }

        yield return null;
    }

    /// <summary>
    /// 使用済みカードフィールドを表示する
    /// </summary>
    public void ShowUsedCardField(bool isVisible)
    {
        usedCardFieldTransform.gameObject.SetActive(isVisible);
    }

    /// <summary>
    /// 選択ダイアログを表示する
    /// </summary>
    IEnumerator ShowSelectDialogue(CardController card)
    {
        while (true)
        {
            if (card.Model.CardType == CARD_TYPE.PASSWORD)
            {
                dialogueManager.meaningButton.interactable = false;

                if (!card.Model.IsPlayerCard)
                {
                    PasswordCardController passwordCard = (PasswordCardController)card;
                    if (passwordCard.Model.IsOpened)
                    {
                        dialogueManager.effectButton.interactable = true;
                        dialogueManager.dialogueText.text = passwordCard.Model.Name + "を選択しますか？";
                    }
                    else
                    {
                        dialogueManager.effectButton.interactable = false;
                        dialogueManager.dialogueText.text = "パスワードカードを選択しますか？";
                    }
                }
                else
                {
                    dialogueManager.effectButton.interactable = true;
                    dialogueManager.dialogueText.text = card.Model.Name + "を選択しますか？";
                }
            }
            else
            {
                dialogueManager.effectButton.interactable = true;
                dialogueManager.meaningButton.interactable = true;
                dialogueManager.dialogueText.text = card.Model.Name + "を選択しますか？";
            }

            dialogueManager.yesButton.interactable = true;
            dialogueManager.noButton.interactable = true;
            dialogueManager.yesButton.gameObject.SetActive(true);
            dialogueManager.noButton.gameObject.SetActive(true);
            dialogueManager.effectButton.gameObject.SetActive(true);
            dialogueManager.meaningButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => dialogueManager.selectYesFlag || dialogueManager.selectNoFlag || dialogueManager.selectEffectFlag || dialogueManager.selectMeaningFlag);

            if (dialogueManager.selectYesFlag || dialogueManager.selectNoFlag)
            {
                break;
            }

            if (dialogueManager.selectEffectFlag)
            {
                dialogueManager.selectEffectFlag = false;
                yield return StartCoroutine(dialogueManager.ShowEffectDialogue(card));
            }

            if (dialogueManager.selectMeaningFlag)
            {
                dialogueManager.selectMeaningFlag = false;
                yield return StartCoroutine(dialogueManager.ShowMeaningDialogue(card));
            }

            yield return null;
        }
    }

    /// <summary>
    /// フィールド効果ダイアログを表示する
    /// </summary>
    public void ShowFieldEffectDialogue()
    {
        StarCardController starCard = GetStarCard();
        switch (starCard.Model.EventCardType)
        {
            case EVENT_CARD_TYPE.PAIN_SPLIT:
            case EVENT_CARD_TYPE.DISABLE_FIREWALL:
                return;
            case EVENT_CARD_TYPE.VILUS_BECOME_FEROCIOUS:
                dialogueManager.dialogueText.text = "攻撃力+20";
                break;
            case EVENT_CARD_TYPE.SERVER_REINFORCEMENT:
                dialogueManager.dialogueText.text = "攻撃回数+1";
                break;
            case EVENT_CARD_TYPE.EARLY_SETTLEMENT:
                dialogueManager.dialogueText.text = "回復0pt";
                break;
            case EVENT_CARD_TYPE.BUFFER_OVERFLOW:
                dialogueManager.dialogueText.text = "自傷ダメージ×2";
                break;
            case EVENT_CARD_TYPE.BUFFER_OVERFLOW2:
                dialogueManager.dialogueText.text = "ダメージ×2\n回復×2";
                break;
            case EVENT_CARD_TYPE.ZERO_DAY_ATTACK:
                dialogueManager.dialogueText.text = "ターン開始時にパスワード1文字に10ダメージ";
                break;
            case EVENT_CARD_TYPE.ZERO_DAY_ATTACK2:
                dialogueManager.dialogueText.text = "カードを使うたびパスワードかファイアウォールに10ダメージ";
                break;
            case EVENT_CARD_TYPE.FILTERING:
                dialogueManager.dialogueText.text = "自傷ダメージ無効";
                break;
            case EVENT_CARD_TYPE.UPDATE:
                return;
            default:
                return;
        }

        dialogueManager.selectFieldEffectButtonFlag = true;

        dialogueManager.yesButton.interactable = false;
        dialogueManager.noButton.interactable = false;
        dialogueManager.effectButton.interactable = false;
        dialogueManager.meaningButton.interactable = false;
        dialogueManager.returnButton.interactable = true;

        dialogueManager.yesButton.gameObject.SetActive(false);
        dialogueManager.noButton.gameObject.SetActive(false);
        dialogueManager.effectButton.gameObject.SetActive(false);
        dialogueManager.meaningButton.gameObject.SetActive(false);
        dialogueManager.returnButton.gameObject.SetActive(true);

        dialogueManager.gameObject.SetActive(true);
    }

    /// <summary>
    /// 戻るボタンを押したときの処理
    /// </summary>
    public void PushReturnButton()
    {
        if (dialogueManager.selectFieldEffectButtonFlag)
        {
            dialogueManager.selectFieldEffectButtonFlag = false;
            dialogueManager.InitDialogue();
        }
        else
        {
            dialogueManager.selectReturnFlag = true;
        }
    }

    /// <summary>
    /// フィールド効果ボタンを表示する
    /// </summary>
    public void ShowFieldEffectButton(StarCardController starCard)
    {
        switch (starCard.Model.EventCardType)
        {
            case EVENT_CARD_TYPE.PAIN_SPLIT:
            case EVENT_CARD_TYPE.DISABLE_FIREWALL:
                fieldEffectButton.gameObject.SetActive(false);
                return;
            case EVENT_CARD_TYPE.VILUS_BECOME_FEROCIOUS:
            case EVENT_CARD_TYPE.SERVER_REINFORCEMENT:
            case EVENT_CARD_TYPE.EARLY_SETTLEMENT:
            case EVENT_CARD_TYPE.BUFFER_OVERFLOW:
            case EVENT_CARD_TYPE.BUFFER_OVERFLOW2:
            case EVENT_CARD_TYPE.ZERO_DAY_ATTACK:
            case EVENT_CARD_TYPE.ZERO_DAY_ATTACK2:
            case EVENT_CARD_TYPE.FILTERING:
                fieldEffectButton.gameObject.SetActive(true);
                return;
            case EVENT_CARD_TYPE.UPDATE:
                fieldEffectButton.gameObject.SetActive(false);
                return;
            default:
                fieldEffectButton.gameObject.SetActive(false);
                return;
        }
    }

    /// <summary>
    /// フィールド効果ボタンを無効にする
    /// </summary>
    public void DisableFieldEffectButton()
    {
        fieldEffectButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// メニューボタンを押したときの処理
    /// </summary>
    public void PushMenuButton()
    {
        menuButtonImage.SetActive(false);
        messageLogManager.gameObject.SetActive(false);
        hackingButton.SetActive(false);

        menuPanel.SetActive(true);
    }

    /// <summary>
    /// ルールボタンを押したときの処理
    /// </summary>
    public void PushRuleButton()
    {
        rulePanel.ShowRulePanel();
    }

    /// <summary>
    /// 再開ボタンを押したときの処理
    /// </summary>
    public void PushResumeButton()
    {
        menuButtonImage.SetActive(true);
        messageLogManager.gameObject.SetActive(true);
        hackingButton.SetActive(true);

        menuPanel.SetActive(false);
    }

    /// <summary>
    /// 降伏ボタンを押したときの処理
    /// </summary>
    public void PushSurrenderButon()
    {

    }

    /// <summary>
    /// ゲーム終了ボタンを押したときの処理
    /// </summary>
    public void PushQuitButton()
    {
        SceneManager.LoadScene("Title");
    }
}

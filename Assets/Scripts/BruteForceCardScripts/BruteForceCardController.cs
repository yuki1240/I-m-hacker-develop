using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// ブルートフォースカードの動作を制御するクラス
/// パスワードやファイアウォールに対して攻撃を行うカードの処理を管理
/// </summary>
public class BruteForceCardController : CardController
{
    [SerializeField] private Image cardFrontImage;
    [SerializeField] private EventTrigger eventTrigger;

    // メッセージログ表示用のマネージャー
    private MessageLogManager messageLogManager;


    // カードのデータモデル
    public new BruteForceCardModel Model { get; private set; }

    void Start()
    {
        // メッセージログマネージャーをシーン内から探して取得
        messageLogManager = FindObjectOfType<MessageLogManager>();
    }

    /// <summary>
    /// カードの初期化処理
    /// </summary>
    /// <param name="cardType">カードの種類</param>
    /// <param name="cardID">カードのID</param>
    /// <param name="isPlayer">プレイヤーのカードかどうか</param>
    public new void Init(int cardType, int cardID, bool isPlayer)
    {
        Debug.Log($"BruteForceCardController.Init: cardType: {cardType}, cardID: {cardID}, isPlayer: {isPlayer}");
        base.Init(cardType, cardID, isPlayer);
        Model = new BruteForceCardModel(cardType, cardID, isPlayer);
    }

    /// <summary>
    /// カードのアルファを変更
    /// </summary>
    /// <param name="alpha"></param>
    public void ChangeAlpha(float alpha)
    {
        if (cardFrontImage == null) cardFrontImage = transform.Find("CardFront").GetComponent<Image>();
        cardFrontImage.DOFade(alpha, 0f);
    }

    /// <summary>
    /// イベントトリガーの有効・無効を切り替える
    /// </summary>
    /// <param name="isActive"></param>
    public void ChangeActiveEventTrigger(bool isActive)
    {
        if (eventTrigger == null) eventTrigger = GetComponent<EventTrigger>();
        eventTrigger.enabled = isActive;
    }

    /// <summary>
    /// ブルートフォースカードの使用処理
    /// 対象を選択して攻撃を行い、カードの種類に応じた追加効果を適用
    /// </summary>
    public IEnumerator UseBruteForceCard()
    {
        // 攻撃対象の候補となるカードリストを取得
        CardController[] selectableCardList = gameManager.GetAttackableCardList(!gameManager.IsPlayerTurn);

        // 攻撃対象のカードを選択
        yield return StartCoroutine(gameManager.SelectCard(selectableCardList));
        CardController selectedCard = gameManager.NowSelectingCard;

        // カード使用のメッセージを表示
        messageLogManager.ShowLog(this.Model.Name + "カードを使用");

        // 選択したカードの種類に応じた攻撃処理
        switch (selectedCard.Model.CardType)
        {
            case CARD_TYPE.FIREWALL:
                // ファイアウォールカードへの攻撃
                FirewallCardController selectedFirewallCard = (FirewallCardController)selectedCard;
                selectedFirewallCard.BruteForceAttack(this.Model.AttackToEnemy);
                break;

            case CARD_TYPE.PASSWORD:
                // パスワードカードへの攻撃
                PasswordCardController selectedPasswordCard = (PasswordCardController)selectedCard;
                yield return StartCoroutine(selectedPasswordCard.BruteForceAttack(this.Model.AttackToEnemy));
                break;

            default:
                yield break;
        }

        // ブルートフォースカードの種類に応じた追加効果
        switch (this.Model.BruteForceCardType)
        {
            case BRUTE_FORCE_CARD_TYPE.BRUTE_FORCE1:
                // 通常攻撃のみ（追加効果なし）
                break;

            case BRUTE_FORCE_CARD_TYPE.BRUTE_FORCE2:
                // 攻撃後に味方のパスワードを回復する効果
                CardController[] healableCardList = gameManager.GetHealableCardList(gameManager.IsPlayerTurn);
                yield return StartCoroutine(gameManager.SelectCard(healableCardList));
                selectedCard = gameManager.NowSelectingCard;
                PasswordCardController healTarget = (PasswordCardController)selectedCard;
                healTarget.Maintenance(this.Model.Heal);
                break;

            case BRUTE_FORCE_CARD_TYPE.BRUTE_FORCE3:
                // 攻撃後に味方のパスワードにも攻撃（自傷効果）
                gameManager.action = GameManager.ACTION.ATTACK;
                CardController[] selfDamageTargets = gameManager.GetPlayerPasswordCards(gameManager.IsPlayerTurn);
                CardController[] selectableTargets = gameManager.GetSelectableCardList(selfDamageTargets);
                yield return StartCoroutine(gameManager.SelectCard(selectableTargets));
                selectedCard = gameManager.NowSelectingCard;
                PasswordCardController damageTarget = (PasswordCardController)selectedCard;
                yield return StartCoroutine(damageTarget.SelfHarm(this.Model.AttackToFriend));
                break;
        }
    }
}

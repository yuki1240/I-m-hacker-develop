using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour
{
    public CardView view; //見かけに関することの操作
    public CardModel Model { get; private set; }//データに関することの操作
    public CardSelect Select { get; private set; }//選択に関することの操作

    protected RectTransform cardRectTransform;

    protected GameManager gameManager;

    private Vector3 originalPosition;

    protected void Awake()
    {
        view = GetComponent<CardView>();
        Select = GetComponent<CardSelect>();
        gameManager = GameManager.Instance;

        cardRectTransform = this.gameObject.GetComponent<RectTransform>();
    }

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void Init(int cardType, int cardID, bool isPlayer)
    {
        Model = new CardModel(cardType, cardID, isPlayer);
        view.SetCard(Model);
        SetCanSelect(false);
        SetIsSelect(false);
    }

    public void SetCanSelect(bool canSelect)
    {
        Select.IsSelectable = canSelect;
    }

    public void SetIsSelect(bool isSelect)
    {
        Select.IsSelected = isSelect;
    }

    public void Show()
    {
        view.Show();
    }

    public void TurnDown()
    {
        view.TurnDown();
    }

    public void IsNotSelectableCard(bool isNotSelectable)
    {
        view.IsNotSelectableCard(isNotSelectable);
    }

    public IEnumerator UseCard()
    {
        switch (this.Model.CardType)
        {
            case CARD_TYPE.BRUTE_FORCE:
                BruteForceCardController bruteForceCard = (BruteForceCardController)this;
                yield return StartCoroutine(bruteForceCard.UseBruteForceCard());
                break;
            case CARD_TYPE.FIREWALL:
                FirewallCardController firewallCard = (FirewallCardController)this;
                yield return StartCoroutine(firewallCard.UseFirewallCard());
                break;
            case CARD_TYPE.EVENT:
                EventCardController eventCard = (EventCardController)this;
                yield return StartCoroutine(eventCard.UseEventCard());
                break;
            case CARD_TYPE.STAR:
                StarCardController starCard = (StarCardController)this;
                yield return StartCoroutine(starCard.UseStarCard());
                break;
            case CARD_TYPE.PASSWORD:
                PasswordCardController passwordCard = (PasswordCardController)this;
                yield return StartCoroutine(passwordCard.OpenPasswordCard());
                break;
        }

        //☆カードを取得
        StarCardController card = gameManager.GetStarCard();
        //場に出ている☆カードがゼロデイ攻撃Ⅱなら
        if (card != null)
        {
            if (card.Model.EventCardType == EVENT_CARD_TYPE.ZERO_DAY_ATTACK2)
            {
                gameManager.NowSelectingCard = card;
                //自分のパスワードカードとファイアウォールカードを取得
                CardController[] passwordCardList = gameManager.GetAttackableCardList(this.Model.IsPlayerCard);
                //パスワードカードを選択
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                PasswordCardController selectedCard = (PasswordCardController)gameManager.NowSelectingCard;
                //選択したパスワードを攻撃
                yield return StartCoroutine(selectedCard.SelfHarm(card.Model.AttackToFriend));
                yield break;
            }
        }
    }

    public void MoveCard(Vector3 anchoredPos)
    {
        cardRectTransform.DOAnchorPos(anchoredPos, 0.5f);
    }
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static GameManager;

public class CardManager : MonoBehaviour
{
    private GameManager gameManager;

    public Transform playerHandTransform; // プレイヤーの手札置き場
    public Transform enemyHandTransform;          // 敵の手札置き場
    public RectTransform playerHandRectTransform; // プレイヤーの手札の位置
    public RectTransform enemyHandRectTransform;  // 敵の手札の位置

    [SerializeField] private Transform usedCardTransform;
    [SerializeField] private Transform deckCardTransform;
    [SerializeField] private Transform bruteForceCardTransform;

    //ブルートフォースカード置き場
    [SerializeField] Transform bruteForceCardFieldTransform;

    [SerializeField] private BruteForceCardController bruteForceCardPrefab;
    [SerializeField] private FirewallCardController firewallCardPrefab;
    [SerializeField] private EventCardController eventCardPrefab;
    [SerializeField] private StarCardController starCardPrefab;
    [SerializeField] private PasswordCardController passwordCardPrefab;

    public List<GameManager.Card> cardDeck = new List<GameManager.Card>();
    public List<GameManager.Card> passwordDeck = new List<GameManager.Card>();

    // IsPlayerTurn（読み取り専用）
    private bool IsPlayerTurn => gameManager.IsPlayerTurn;

    private void Awake()
    {
        gameManager = GetComponentInParent<GameManager>();  // 修正
    }

    /// <summary>
    /// ブルートフォースカードを初期化する
    /// </summary>
    /// <param name="cardType"></param>
    /// <param name="cardID"></param>
    /// <param name="isPlayer"></param>
    public void InitBruteForceCard()
    {
        for (int i = 1; i <= Define.bruteForceCardNum; i++)
        {
            CreateCard(1, i, bruteForceCardFieldTransform);
        }
    }

    /// <summary>
    /// ブルートフォースカードを取得する
    /// </summary>
    public BruteForceCardController[] GetBruteForceCards()
    {
        return bruteForceCardFieldTransform.GetComponentsInChildren<BruteForceCardController>();
    }

    /// <summary>
    /// カードを生成する
    /// </summary>
    public void CreateCard(int cardType, int cardID, Transform hand)
    {
        switch (cardType)
        {
            case 1:
                BruteForceCardController bruteForceCard = Instantiate(bruteForceCardPrefab, hand, false);

                if (hand.name == "BruteForceCardField")
                {
                    bruteForceCard.Init(cardType, cardID, IsPlayerTurn);
                }
                break;
            case 2:
                FirewallCardController firewallCard = Instantiate(firewallCardPrefab, hand, false);
                if (hand.name == "DeckField")
                {
                    firewallCard.Init(cardType, cardID, IsPlayerTurn);
                    Vector3 firewallCardPos;
                    if (IsPlayerTurn)
                    {
                        firewallCardPos = playerHandRectTransform.anchoredPosition;
                    }
                    else
                    {
                        firewallCardPos = enemyHandRectTransform.anchoredPosition;
                    }
                    firewallCard.MoveCard(firewallCardPos);
                }
                else if (hand.name == "DeckCardField")
                {
                    firewallCard.Init(cardType, cardID, IsPlayerTurn);
                }
                else if ((hand.name == "PlayerHand") || (hand.name == "PlayerMigration3Field"))
                {
                    firewallCard.Init(cardType, cardID, true);
                }
                else
                {
                    firewallCard.Init(cardType, cardID, false);
                }
                break;
            case 3:
                EventCardController eventCard = Instantiate(eventCardPrefab, hand, false);
                if (hand.name == "DeckField")
                {
                    eventCard.Init(cardType, cardID, IsPlayerTurn);
                    Vector3 eventCardPos;
                    if (IsPlayerTurn)
                    {
                        eventCardPos = playerHandRectTransform.anchoredPosition;
                    }
                    else
                    {
                        eventCardPos = enemyHandRectTransform.anchoredPosition;
                    }
                    eventCard.MoveCard(eventCardPos);
                }
                else if (hand.name == "DeckCardField")
                {
                    eventCard.Init(cardType, cardID, IsPlayerTurn);
                }
                else if ((hand.name == "PlayerHand") || (hand.name == "PlayerMigration3Field"))
                {
                    eventCard.Init(cardType, cardID, true);
                }
                else
                {
                    eventCard.Init(cardType, cardID, false);
                }
                break;
            case 4:
                StarCardController starCard = Instantiate(starCardPrefab, hand, false);
                if (hand.name == "DeckField")
                {
                    starCard.Init(cardType, cardID, IsPlayerTurn);
                    Vector3 starCardPos;
                    if (IsPlayerTurn)
                    {
                        starCardPos = playerHandRectTransform.anchoredPosition;
                    }
                    else
                    {
                        starCardPos = enemyHandRectTransform.anchoredPosition;
                    }
                    starCard.MoveCard(starCardPos);
                }
                else if (hand.name == "DeckCardField")
                {
                    starCard.Init(cardType, cardID, IsPlayerTurn);
                }
                else if ((hand.name == "PlayerHand") || (hand.name == "PlayerMigration3Field"))
                {
                    starCard.Init(cardType, cardID, true);
                }
                else
                {
                    starCard.Init(cardType, cardID, false);
                }
                break;
            case 5:
                PasswordCardController passwordCard = Instantiate(passwordCardPrefab, hand, false);
                if (hand.name == "DeckField")
                {
                    passwordCard.Init(cardType, cardID, IsPlayerTurn);
                    Vector3 passwordCardPos;
                    if (IsPlayerTurn)
                    {
                        passwordCardPos = playerHandRectTransform.anchoredPosition;
                    }
                    else
                    {
                        passwordCardPos = enemyHandRectTransform.anchoredPosition;
                    }
                    passwordCard.MoveCard(passwordCardPos);
                }
                if (hand.name == "PlayerHand")
                {
                    passwordCard.Init(cardType, cardID, true);
                }
                else
                {
                    passwordCard.Init(cardType, cardID, false);
                }
                break;
            default:
                break;
        }
    }
}

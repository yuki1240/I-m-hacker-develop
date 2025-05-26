using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel
{
    public string Name { get; private set; }
    public Sprite CardSprite { get; private set; }
    public CARD_TYPE CardType { get; private set; }
    public string Explanation { get; private set; }
    public string Meaning { get; private set; }
    public CardEntity CardEntity { get; private set; }

    public bool IsPlayerCard { get; private set; }

    public CardModel(int cardType, int cardID, bool isPlayer)
    {
        switch (cardType)
        {
            case 1:
                CardEntity = Resources.Load<BruteForceCardEntity>("CardEntityList/BruteForceCard" + cardID);
                break;
            case 2:
                CardEntity = Resources.Load<FirewallCardEntity>("CardEntityList/FirewallCard" + cardID);
                break;
            case 3:
                CardEntity = Resources.Load<EventCardEntity>("CardEntityList/EventCard" + cardID);
                break;
            case 4:
                CardEntity = Resources.Load<StarCardEntity>("CardEntityList/StarCard" + cardID);
                break;
            case 5:
                CardEntity = Resources.Load<PasswordCardEntity>("CardEntityList/PasswordCard" + cardID);
                break;
            default:
                CardEntity = null;
                break;
        }
        Name = CardEntity.name;
        CardSprite = CardEntity.cardSprite;
        this.CardType = CardEntity.cardType;
        Explanation = CardEntity.explanation;
        Meaning = CardEntity.meaning;

        IsPlayerCard = isPlayer;
    }
}

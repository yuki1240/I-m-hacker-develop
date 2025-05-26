using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardModel : CardModel
{
    public int AttackToEnemy { get; private set; }
    public int AttackToFriend { get; private set; }
    public int Heal { get; private set; }
    public EVENT_CARD_TYPE EventCardType { get; private set; }

    public EventCardModel(int cardType, int cardID, bool isPlayer) : base(cardType, cardID, isPlayer)
    {
        EventCardEntity cardEntity = (EventCardEntity)base.CardEntity;
        AttackToEnemy = cardEntity.attackToEnemy;
        AttackToFriend = cardEntity.attackToFriend;
        Heal = cardEntity.heal;
        EventCardType = cardEntity.eventCardType;
    }
}

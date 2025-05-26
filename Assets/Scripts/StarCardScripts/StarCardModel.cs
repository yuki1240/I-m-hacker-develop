using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCardModel : EventCardModel
{
    public StarCardModel(int cardType, int cardID, bool isPlayer) : base(cardType, cardID, isPlayer)
    {
        StarCardEntity cardEntity = (StarCardEntity)base.CardEntity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallCardModel : CardModel
{
    private int hp;
    public int HP
    {
        get
        {
            return hp;
        }

        private set
        {
            if (value <= 0)
            {
                IsDisable = true;
                hp = 0;

                ProtectingPasswordCard.Model.SetIsProtected(false);
                ProtectingPasswordCard = null;
            }
            else if (value > MaxHP)
            {
                hp = MaxHP;
            }
            else
            {
                hp = value;
            }

            if (hp > (MaxHP * 0.5))
            {
                Color = Color.green;
            }
            else if (hp > (MaxHP * 0.2))
            {
                Color = Color.yellow;
            }
            else
            {
                Color = Color.red;
            }
        }
    }
    public int MaxHP { get; private set; }
    public FIREWALL_CARD_TYPE FirewallCardType { get; private set; }
    public bool IsDisable { get; private set; }

    public PasswordCardController ProtectingPasswordCard { get; private set; }

    public Color Color { get; private set; }

    public FirewallCardModel(int cardType, int cardID, bool isPlayer) : base(cardType, cardID, isPlayer)
    {
        FirewallCardEntity cardEntity = (FirewallCardEntity)base.CardEntity;
        MaxHP = cardEntity.hp;
        HP = cardEntity.hp;
        FirewallCardType = cardEntity.firewallCardType;
        Color = Color.green;
    }

    public void Damage(int damage)
    {
        HP -= damage;
    }

    public void Recovery(int recovery)
    {
        HP += recovery;
    }

    public void Disable()
    {
        HP = 0;
        ProtectingPasswordCard.Model.SetIsProtected(false);
        ProtectingPasswordCard = null;
    }

    public void SetProtectPasswordCard(PasswordCardController passwordCard)
    {
        ProtectingPasswordCard = passwordCard;
    }

    public void IncreaseMaxHP(int increaceHP)
    {
        MaxHP += increaceHP;
        HP += increaceHP;
    }
}

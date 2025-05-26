using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordCardModel : CardModel
{
    private int hp;
    public int HP
    {
        get
        {
            return hp;
        }

        set
        {
            if (value <= 0)
            {
                IsHacked = true;
                hp = 0;

                if (Color != Color.red)
                {
                    Color = Color.red;
                }
            }
            else if (value > GameManager.Define.maxPasswordHP)
            {
                hp = GameManager.Define.maxPasswordHP;
            }
            else
            {
                hp = value;
                if (hp > 50)
                {
                    Color = Color.green;
                }
                else if (hp > 20)
                {
                    Color = Color.yellow;
                }
                else
                {
                    Color = Color.red;
                }

                if (hp <= 50)
                {
                    if (!IsOpened)
                    {
                        CardOpenflag = true;
                    }
                }
            }
        }
    }
    public int Attack { get; private set; }
    public int Heal { get; private set; }
    public int CanUseSkillHP { get; private set; }
    public PASSWORD_CARD_TYPE PasswordCardType { get; private set; }

    public Color Color { get; private set; }

    public bool CardOpenflag { get; private set; }
    public bool IsOpened { get; private set; }
    public bool IsProtected { get; private set; }
    public bool IsHacked { get; private set; }

    public PasswordCardModel(int cardType, int cardID, bool isPlayer) : base(cardType, cardID, isPlayer)
    {
        PasswordCardEntity cardEntity = (PasswordCardEntity)base.CardEntity;
        HP = cardEntity.hp;
        Attack = cardEntity.attack;
        Heal = cardEntity.heal;
        CanUseSkillHP = cardEntity.canUseSkillHP;
        PasswordCardType = cardEntity.passwordCardType;
        CardOpenflag = false;
        IsOpened = false;
        IsProtected = false;
        IsHacked = false;
        Color = Color.green;
    }

    public void SetIsProtected(bool isProtedcted)
    {
        IsProtected = isProtedcted;
    }

    public void SetCardOpenflag(bool cardOpenflag)
    {
        CardOpenflag = cardOpenflag;
    }

    public void SetIsOpened(bool isOpened)
    {
        IsOpened = isOpened;
    }

    public void Damage(int damage)
    {
        HP -= damage;
    }

    public void Recovery(int recovery)
    {
        HP += recovery;
    }

    public void ChangeHP(int changeHP)
    {
        HP = changeHP;
    }
}

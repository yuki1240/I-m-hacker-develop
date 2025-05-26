using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordCardView : CardView
{
    [SerializeField] TextMeshProUGUI HP;
    [SerializeField] TextMeshProUGUI passwordLetter;

    public void SetPasswordCard(PasswordCardModel passwordCardModel)
    {
        HP.text = passwordCardModel.HP.ToString();
        passwordLetter.text = passwordCardModel.Name;
        passwordLetter.enabled = false;
        HP.enabled = false;
        HP.color = passwordCardModel.Color;
    }

    public void SetPasswordCharacter(char character)
    {
        passwordLetter.text = character.ToString();
    }

    public char GetPasswordCharacter()
    {
        char[] PasswordCharacter = passwordLetter.text.ToCharArray();
        return PasswordCharacter[0];
    }

    public void SetCanViewHP(bool canView)
    {
        HP.enabled = canView;
    }

    public void Hack()
    {
        passwordLetter.enabled = true;
    }

    public void Refresh(PasswordCardModel model)
    {
        HP.text = model.HP.ToString();
        HP.color = model.Color;
    }
}

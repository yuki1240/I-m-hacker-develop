using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FirewallCardView : CardView
{
    [SerializeField] TextMeshProUGUI HP;

    public void SetFirewallCard(FirewallCardModel firewallCardModel)
    {
        HP.text = firewallCardModel.HP.ToString();
        HP.color = firewallCardModel.Color;
        HP.enabled = false;
    }

    public void SetCanViewHP(bool canView)
    {
        HP.enabled = canView;
    }

    public void Refresh(FirewallCardModel model)
    {
        HP.text = model.HP.ToString();
        HP.color = model.Color;
    }
}

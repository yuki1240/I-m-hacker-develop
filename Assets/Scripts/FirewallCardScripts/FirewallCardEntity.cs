using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirewallCardEntity", menuName = "Create FirewallCardEntity")]
//ファイアウォールカードのデータそのもの
public class FirewallCardEntity : CardEntity
{
    public int hp;//ファイアウォールカードのHP
    public FIREWALL_CARD_TYPE firewallCardType;//ファイアウォールカードの種類
}

public enum FIREWALL_CARD_TYPE
{
    FIREWALL20 = 1,//ファイアウォールカード20
    FIREWALL30,//ファイアウォールカード30
}

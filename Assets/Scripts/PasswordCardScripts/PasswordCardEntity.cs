using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PasswordCardEntity", menuName = "Create PasswordCardEntity")]
//パスワードカードのデータそのもの
public class PasswordCardEntity : CardEntity
{
    public int hp;
    public int attack;
    public int heal;
    public int canUseSkillHP;
    public PASSWORD_CARD_TYPE passwordCardType;//パスワードカードの種類
}

public enum PASSWORD_CARD_TYPE
{
    A_GYOU = 1,//あ行
    KA_GYOU,//か行
    SA_GYOU,//さ行
    TA_GYOU,//た行
    NA_GYOU,//な行
    HA_GYOU,//は行
    MA_GYOU,//ま行
    YA_GYOU,//や行
    RA_GYOU,//ら行
    WA_GYOU_N,//わ行_ん
}

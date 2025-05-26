using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カードデータそのもの
public class CardEntity : ScriptableObject
{
    public new string name;//カードの名前
    public Sprite cardSprite;//カードの絵
    public CARD_TYPE cardType;//カードの種類
    public string explanation;//カードの説明
    public string meaning;//カードの意味
}

public enum CARD_TYPE
{
    BRUTE_FORCE = 1,//ブルートフォースカード
    FIREWALL,//ファイアウォールカード
    EVENT,//イベントカード
    STAR,//☆カード
    PASSWORD,//パスワードカード
}

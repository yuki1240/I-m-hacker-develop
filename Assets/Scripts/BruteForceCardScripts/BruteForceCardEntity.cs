using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BruteForceCardEntity", menuName = "Create BruteForceCardEntity")]
//ブルートフォースカードのデータそのもの
public class BruteForceCardEntity : CardEntity
{
    public int attackToEnemy;//敵に与えるダメージ
    public int attackToFriend;//味方に与えるダメージ
    public int heal;//回復量
    public BRUTE_FORCE_CARD_TYPE bruteForceCardType;//ブルートフォースカードの種類
}

public enum BRUTE_FORCE_CARD_TYPE
{
    BRUTE_FORCE1 = 1,//ブルートフォースカード1
    BRUTE_FORCE2,//ブルートフォースカード2
    BRUTE_FORCE3,//ブルートフォースカード3
}

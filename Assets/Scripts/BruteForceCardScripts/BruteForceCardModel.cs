using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ブルートフォースカードのデータモデルを管理するクラス
/// パスワードやファイアウォールに対する攻撃力や回復量などのパラメータを保持
/// </summary>
public class BruteForceCardModel : CardModel
{
    /// <summary>
    /// 敵に与えるダメージ量
    /// </summary>
    public int AttackToEnemy { get; private set; }

    /// <summary>
    /// 自分のパスワードに与えるダメージ量（自傷効果用）
    /// </summary>
    public int AttackToFriend { get; private set; }

    /// <summary>
    /// パスワードの回復量
    /// </summary>
    public int Heal { get; private set; }

    /// <summary>
    /// ブルートフォースカードの種類
    /// （通常攻撃・回復付き攻撃・自傷付き攻撃）
    /// </summary>
    public BRUTE_FORCE_CARD_TYPE BruteForceCardType { get; private set; }

    /// <summary>
    /// ブルートフォースカードの初期化
    /// </summary>
    /// <param name="cardType">カードの種類</param>
    /// <param name="cardID">カードのID</param>
    /// <param name="isPlayer">プレイヤーのカードかどうか</param>
    public BruteForceCardModel(int cardType, int cardID, bool isPlayer) : base(cardType, cardID, isPlayer)
    {
        // カードのデータを設定ファイルから読み込み
        BruteForceCardEntity cardEntity = (BruteForceCardEntity)base.CardEntity;

        // 各パラメータを設定
        AttackToEnemy = cardEntity.attackToEnemy;     // 敵への攻撃力
        AttackToFriend = cardEntity.attackToFriend;   // 自身への攻撃力（自傷）
        Heal = cardEntity.heal;                       // 回復量
        BruteForceCardType = cardEntity.bruteForceCardType;  // カードの種類
    }
}

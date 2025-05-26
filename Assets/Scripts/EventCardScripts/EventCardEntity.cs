using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventCardEntity", menuName = "Create EventCardEntity")]
//イベントカードのデータそのもの
public class EventCardEntity : CardEntity
{
    public int attackToEnemy;//敵に与えるダメージ
    public int attackToFriend;//味方に与えるダメージ
    public int heal;//回復量
    public EVENT_CARD_TYPE eventCardType;//イベントカードの種類
}

public enum EVENT_CARD_TYPE
{
    MAINTENANCE = 1,//メンテナンス
    MAINTENANCE2,//メンテナンス2
    XSS,//XSS
    RESTORATION,//復元
    INFOMATION_LEAKAGE,//情報漏洩
    PLUS_MINUS,//プラマイ
    FALSIFICATION,//改ざん
    RECONSTITUITION,//再構築
    OPTIMIZATION,//最適化
    CTL_Z,//Ctl+z
    MIGRATION2,//マイグレーション2
    MIGRATION3,//マイグレーション3
    MIGRATION,//マイグレーション
    RECOVERY,//リカバリー
    SCALE_UP,//スケールアップ
    HEAVY_LOAD,//高負荷
    WHITELIST,//ホワイトリスト
    REBOOT,//再起動
    NARROW_BAND,//ナローバンド
    MALWARE,//マルウェア
    PAIN_SPLIT,//痛み分け
    DISABLE_FIREWALL,//ファイアウォールの無効化
    VILUS_BECOME_FEROCIOUS,//ウイルスの凶暴化
    SERVER_REINFORCEMENT,//サーバー増強
    EARLY_SETTLEMENT,//早期決着
    BUFFER_OVERFLOW,//バッファオーバーフロー
    BUFFER_OVERFLOW2,//バッファオーバーフロー2
    ZERO_DAY_ATTACK,//ゼロデイ攻撃
    ZERO_DAY_ATTACK2,//ゼロデイ攻撃2
    FILTERING,//フィルタリング
    UPDATE,//アップデート
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCardController : EventCardController
{
    MessageLogManager messageLogManager;

    void Start()
    {
        // メッセージログマネージャーをシーン内から探して取得
        messageLogManager = FindObjectOfType<MessageLogManager>();
    }

    public new void Init(int cardType, int cardID, bool isPlayer)
    {
        base.Init(cardType, cardID, isPlayer);
    }

    public IEnumerator UseStarCard()
    {
        PasswordCardController[] playerPasswordCardList;
        FirewallCardController[] playerFirewallCardList;

        PasswordCardController[] enemyPasswordCardList;
        FirewallCardController[] enemyFirewallCardList;

        int i;

        gameManager.SetStarCardToField(this);
        messageLogManager.ShowLog(this.Model.Name + "カードを使用");
        messageLogManager.ShowLog(this.Model.Explanation);
        switch (base.Model.EventCardType)
        {
            case EVENT_CARD_TYPE.PAIN_SPLIT:
                //プレイヤーのパスワードカードとファイアウォールカードを取得
                playerPasswordCardList = gameManager.GetPlayerPasswordCards(this.Model.IsPlayerCard);
                playerFirewallCardList = gameManager.GetPlayerFirewallCards(this.Model.IsPlayerCard);
                i = 0;
                foreach (PasswordCardController passwordCard in playerPasswordCardList)
                {
                    if (passwordCard.Model.IsProtected)
                    {
                        messageLogManager.ShowLog("攻撃がファイアウォールに防がれた！");
                        playerFirewallCardList[i++].BruteForceAttack(this.Model.AttackToEnemy);
                    }
                    else
                    {
                        yield return StartCoroutine(passwordCard.BruteForceAttack(this.Model.AttackToEnemy));
                    }
                }

                //敵のパスワードカードとファイアウォールカードを取得
                enemyPasswordCardList = gameManager.GetPlayerPasswordCards(!this.Model.IsPlayerCard);
                enemyFirewallCardList = gameManager.GetPlayerFirewallCards(!this.Model.IsPlayerCard);
                i = 0;
                foreach (PasswordCardController passwordCard in enemyPasswordCardList)
                {
                    if (passwordCard.Model.IsProtected)
                    {
                        messageLogManager.ShowLog("攻撃がファイアウォールに防がれた！");
                        enemyFirewallCardList[i++].BruteForceAttack(this.Model.AttackToEnemy);
                    }
                    else
                    {
                        yield return StartCoroutine(passwordCard.BruteForceAttack(this.Model.AttackToEnemy));
                    }
                }
                yield break;
            case EVENT_CARD_TYPE.DISABLE_FIREWALL:
                playerFirewallCardList = gameManager.GetPlayerFirewallCards(this.Model.IsPlayerCard);
                foreach (FirewallCardController firewallCard in playerFirewallCardList)
                {
                    firewallCard.DisableFirewall();
                }

                enemyFirewallCardList = gameManager.GetPlayerFirewallCards(!this.Model.IsPlayerCard);
                foreach (FirewallCardController firewallCard in enemyFirewallCardList)
                {
                    firewallCard.DisableFirewall();
                }
                yield break;
            case EVENT_CARD_TYPE.VILUS_BECOME_FEROCIOUS:
                yield break;
            case EVENT_CARD_TYPE.SERVER_REINFORCEMENT:
                yield break;
            case EVENT_CARD_TYPE.EARLY_SETTLEMENT:
                yield break;
            case EVENT_CARD_TYPE.BUFFER_OVERFLOW:
                yield break;
            case EVENT_CARD_TYPE.BUFFER_OVERFLOW2:
                yield break;
            case EVENT_CARD_TYPE.ZERO_DAY_ATTACK:
                yield break;
            case EVENT_CARD_TYPE.ZERO_DAY_ATTACK2:
                yield break;
            case EVENT_CARD_TYPE.FILTERING:
                yield break;
            case EVENT_CARD_TYPE.UPDATE:
                //プレイヤーと敵の回復可能なカードをすべて取得
                playerPasswordCardList = (PasswordCardController[])gameManager.GetHealableCardList(this.Model.IsPlayerCard);
                enemyPasswordCardList = (PasswordCardController[])gameManager.GetHealableCardList(!this.Model.IsPlayerCard);
                PasswordCardController[] allHealableCardList = (PasswordCardController[])gameManager.MergeCardList(playerPasswordCardList, enemyPasswordCardList);
                //すべてのHPを20回復
                foreach (PasswordCardController passwordCard in allHealableCardList)
                {
                    passwordCard.Maintenance(this.Model.Heal);
                }
                yield break;
            default:
                yield break;
        }
    }
}

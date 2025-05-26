using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallCardController : CardController
{
    MessageLogManager messageLogManager;
    FirewallCardView firewallView; //見かけに関することの操作
    public new FirewallCardModel Model { get; private set; }//データに関することの操作

    private new void Awake()
    {
        base.Awake();
        firewallView = GetComponent<FirewallCardView>();
    }

    void Start()
    {
        // メッセージログマネージャーをシーン内から探して取得
        messageLogManager = FindObjectOfType<MessageLogManager>();
    }

    public new void Init(int cardType, int cardID, bool isPlayer)
    {
        base.Init(cardType, cardID, isPlayer);
        Model = new FirewallCardModel(cardType, cardID, isPlayer);
        firewallView.SetFirewallCard(Model);
    }

    public IEnumerator UseFirewallCard()
    {
        messageLogManager.ShowLog(this.Model.Name + "カードを使用");
        messageLogManager.ShowLog(this.Model.Explanation);
        switch (this.Model.FirewallCardType)
        {
            case FIREWALL_CARD_TYPE.FIREWALL20:
            case FIREWALL_CARD_TYPE.FIREWALL30:
                CardController[] passwordCardList = gameManager.GetPlayerPasswordCards(this.Model.IsPlayerCard);
                CardController[] selectablePasswordCardList = gameManager.GetSelectableCardList(passwordCardList);
                yield return StartCoroutine(gameManager.SelectCard(selectablePasswordCardList));
                PasswordCardController nowSelectingPasswordCard = (PasswordCardController)gameManager.NowSelectingCard;
                this.Model.SetProtectPasswordCard(nowSelectingPasswordCard);
                nowSelectingPasswordCard.Model.SetIsProtected(true);
                RectTransform passwordRectTransform = gameManager.NowSelectingCard.gameObject.GetComponent<RectTransform>();
                Vector3 passwordPos = passwordRectTransform.anchoredPosition;
                gameManager.SetFirewallCardToField(this, passwordPos);

                int increaceHP = 0;

                //プレイヤーのパスワードカードを取得
                PasswordCardController[] playerPasswordCardList = gameManager.GetPlayerPasswordCards(this.Model.IsPlayerCard);
                foreach (PasswordCardController card in playerPasswordCardList)
                {
                    //た行カードのとき
                    if (card.Model.PasswordCardType == PASSWORD_CARD_TYPE.TA_GYOU)
                    {
                        //表になっていてスキル発動可能HP以下になっているた行カードのみ
                        if (card.Model.IsHacked || !card.Model.IsOpened || (card.Model.HP > card.Model.CanUseSkillHP)) continue;

                        //前のターンに相手がDoS攻撃カードを使用してなければ効果発動
                        if (gameManager.IsPlayerTurn && gameManager.enemy.UseDoSAttackCardflag)
                        {
                            messageLogManager.ShowLog("相手のDoS攻撃カードの効果でプレイヤーのた行カードの効果は無効化された！");
                            break;
                        }

                        //前のターンにプレイヤーがDoS攻撃カードを使用してなければ効果発動
                        if (!gameManager.IsPlayerTurn && gameManager.player.UseDoSAttackCardflag)
                        {
                            messageLogManager.ShowLog("プレイヤーのDoS攻撃カードの効果で相手のた行カードの効果は無効化された！");
                            break;
                        }

                        increaceHP += card.Model.Heal;
                    }
                }

                //スキル使用可能なた行カードの枚数分HP + 20
                if (increaceHP > 0)
                {
                    messageLogManager.ShowLog("た行カードの効果でHPが+" + increaceHP + "ptされた！");
                    this.Model.IncreaseMaxHP(increaceHP);
                    RefreshView();
                }

                yield break;
        }
    }

    public void SetCanViewHP(bool canView)
    {
        firewallView.SetCanViewHP(canView);
    }

    public void BruteForceAttack(int attack)
    {
        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "に攻撃！");
        }
        else
        {
            messageLogManager.ShowLog("相手の" + this.Model.Name + "に攻撃！");
        }

        int damage = attack;
        //スケールアップカードを使った使った回数分攻撃+10
        if (gameManager.IsPlayerTurn && (gameManager.player.ScaleUpCardAttackBuff > 0))
        {
            messageLogManager.ShowLog("スケールアップカードの効果で攻撃が+" + gameManager.player.ScaleUpCardAttackBuff + "ptされた！");
            damage += gameManager.player.ScaleUpCardAttackBuff;
        }

        if (!gameManager.IsPlayerTurn && (gameManager.enemy.ScaleUpCardAttackBuff > 0))
        {
            messageLogManager.ShowLog("スケールアップカードの効果で攻撃が+" + gameManager.enemy.ScaleUpCardAttackBuff + "ptされた！");
            damage += gameManager.enemy.ScaleUpCardAttackBuff;
        }

        int attackBuff = 0;

        //プレイヤーのパスワードカードを取得
        PasswordCardController[] playerPasswordCardList = gameManager.GetPlayerPasswordCards(this.Model.IsPlayerCard);
        foreach (PasswordCardController card in playerPasswordCardList)
        {
            //あ行カードのとき
            if (card.Model.PasswordCardType == PASSWORD_CARD_TYPE.A_GYOU)
            {
                //表になっていてスキル発動可能HP以下になっているあ行カードのみ
                if (card.Model.IsHacked || !card.Model.IsOpened || (card.Model.HP > card.Model.CanUseSkillHP)) continue;

                //前のターンに相手がDoS攻撃カードを使用してなければ効果発動
                if (gameManager.IsPlayerTurn && gameManager.enemy.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("相手のDoS攻撃カードの効果でプレイヤーのあ行カードの効果は無効化された！");
                    break;
                }

                //前のターンにプレイヤーがDoS攻撃カードを使用してなければ効果発動
                if (!gameManager.IsPlayerTurn && gameManager.player.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("プレイヤーのDoS攻撃カードの効果で相手のあ行カードの効果は無効化された！");
                    break;
                }

                attackBuff += card.Model.Attack;
            }
        }

        //スキル使用可能なあ行カードの枚数分攻撃+10
        if (attackBuff > 0)
        {
            messageLogManager.ShowLog("あ行カードの効果で攻撃が+" + attackBuff + "ptされた！");
            damage += attackBuff;
        }

        int attackDebuff = 0;

        //相手のパスワードカードを取得
        PasswordCardController[] enemyPasswordCardList = gameManager.GetPlayerPasswordCards(!this.Model.IsPlayerCard);
        foreach (PasswordCardController card in enemyPasswordCardList)
        {
            //ま行カードのとき
            if (card.Model.PasswordCardType == PASSWORD_CARD_TYPE.MA_GYOU)
            {
                //表になっていてスキル発動可能HP以下になっているま行カードのみ
                if (card.Model.IsHacked || !card.Model.IsOpened || (card.Model.HP > card.Model.CanUseSkillHP)) continue;

                attackDebuff += card.Model.Attack;
            }
        }

        //スキル使用可能なま行カードの枚数分攻撃-10
        if (attackDebuff > 0)
        {
            if (this.Model.IsPlayerCard)
            {
                messageLogManager.ShowLog("相手のま行カードの効果でプレイヤーのダメージが-" + attackDebuff + "ptされた！");
            }
            else
            {
                messageLogManager.ShowLog("プレイヤーのま行カードの効果で相手のダメージが-" + attackDebuff + "ptされた！");
            }

            damage -= attackDebuff;
            if (damage < 0) damage = 0;
        }

        //場に出ている☆カードを取得
        StarCardController starCard = gameManager.GetStarCard();
        if (starCard != null)
        {
            switch (starCard.Model.EventCardType)
            {
                case EVENT_CARD_TYPE.VILUS_BECOME_FEROCIOUS://場に出ている☆カードがウイルスの凶暴化カードなら
                    damage += starCard.Model.AttackToEnemy;//ダメージ+20
                    break;
                case EVENT_CARD_TYPE.BUFFER_OVERFLOW2://バッファオーバーフローⅡカードなら
                    damage += attack * starCard.Model.AttackToEnemy;//ダメージ2倍
                    break;
            }
        }
        this.Model.Damage(damage);

        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "に" + damage + "ダメージ！");
        }
        else
        {
            messageLogManager.ShowLog("相手の" + this.Model.Name + "に" + damage + "ダメージ！");
        }

        RefreshView();
        CheckDisable();
    }

    public void Maintenance(int heal)
    {
        int recovery = heal;
        //リカバリーカードを使った回数分回復+20
        if (gameManager.IsPlayerTurn && (gameManager.player.RecoveryCardHealBuff > 0))
        {
            messageLogManager.ShowLog("リカバリーカードの効果で回復が+" + gameManager.player.RecoveryCardHealBuff + "ptされた！");
            recovery += gameManager.player.RecoveryCardHealBuff;
        }

        if (!gameManager.IsPlayerTurn && (gameManager.enemy.RecoveryCardHealBuff > 0))
        {
            messageLogManager.ShowLog("リカバリーカードの効果で回復が+" + gameManager.enemy.RecoveryCardHealBuff + "ptされた！");
            recovery += gameManager.enemy.RecoveryCardHealBuff;
        }

        int healDebuff = 0;

        //相手のパスワードカードを取得
        PasswordCardController[] enemyPasswordCardList = gameManager.GetPlayerPasswordCards(!this.Model.IsPlayerCard);
        foreach (PasswordCardController card in enemyPasswordCardList)
        {
            //か行カードのとき
            if (card.Model.PasswordCardType == PASSWORD_CARD_TYPE.KA_GYOU)
            {
                //表になっていてスキル発動可能HP以下になっているか行カードのみ
                if (card.Model.IsHacked || !card.Model.IsOpened || (card.Model.HP > card.Model.CanUseSkillHP)) continue;

                healDebuff += card.Model.Heal;
            }
        }

        //スキル使用可能なか行カードの枚数分回復-10
        if (healDebuff > 0)
        {
            if (this.Model.IsPlayerCard)
            {
                messageLogManager.ShowLog("相手のか行カードの効果でプレイヤーの回復が-" + healDebuff + "ptされた！");
            }
            else
            {
                messageLogManager.ShowLog("プレイヤーのか行カードの効果で相手の回復が-" + healDebuff + "ptされた！");
            }

            recovery -= healDebuff;
            if (recovery < 0) recovery = 0;
        }

        //場に出ている☆カードを取得
        StarCardController starCard = gameManager.GetStarCard();
        if (starCard != null)
        {
            switch (starCard.Model.EventCardType)
            {
                case EVENT_CARD_TYPE.EARLY_SETTLEMENT://場に出ている☆カードが早期決着カードなら
                    messageLogManager.ShowLog(starCard.Model.Name + "の効果で全ての回復は０ptになった！");
                    recovery = starCard.Model.Heal;//回復量0
                    break;
                case EVENT_CARD_TYPE.BUFFER_OVERFLOW2://バッファオーバーフローⅡカードなら
                    messageLogManager.ShowLog(starCard.Model.Name + "の効果で全ての回復は２倍された！");
                    recovery += heal * starCard.Model.Heal;//回復量2倍
                    break;
            }
        }
        this.Model.Recovery(recovery);

        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "を" + recovery + "回復");
        }
        else
        {
            messageLogManager.ShowLog("相手の" + this.Model.Name + "を" + recovery + "回復");
        }

        RefreshView();
    }

    public void DisableFirewall()
    {
        this.Model.Disable();
        RefreshView();
        CheckDisable();
    }

    public void RefreshView()
    {
        firewallView.Refresh(this.Model);
    }

    public void CheckDisable()
    {
        Debug.Log("無効化されているか : " + Model.IsDisable);
        if (Model.IsDisable)
        {
            if (this.Model.IsPlayerCard)
            {
                messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "が破壊された！");
            }
            else
            {
                messageLogManager.ShowLog("相手の" + this.Model.Name + "が破壊された！");
            }

            SetCanViewHP(false);
            gameManager.SetUsedCardToUsedCardField(this);
        }
    }
}

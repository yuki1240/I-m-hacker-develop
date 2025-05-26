using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordCardController : CardController
{
    MessageLogManager messageLogManager;
    PasswordCardView passwordView; //見かけに関することの操作
    public new PasswordCardModel Model { get; private set; }//データに関することの操作

    private new void Awake()
    {
        base.Awake();
        passwordView = GetComponent<PasswordCardView>();
    }

    void Start()
    {
        // メッセージログマネージャーをシーン内から探して取得
        messageLogManager = FindObjectOfType<MessageLogManager>();
    }

    public new void Init(int cardType, int cardID, bool isPlayer)
    {
        base.Init(cardType, cardID, isPlayer);
        Model = new PasswordCardModel(cardType, cardID, isPlayer);
        passwordView.SetPasswordCard(Model);
    }

    public void SetPasswordCharacter(char character)
    {
        passwordView.SetPasswordCharacter(character);
    }

    public char GetPasswordCharacter()
    {
        return passwordView.GetPasswordCharacter();
    }

    public void SetCanViewHP(bool canView)
    {
        passwordView.SetCanViewHP(canView);
    }

    public IEnumerator Damage(int damage)
    {
        this.Model.Damage(damage);
        RefreshView();
        CheckHacked();
        yield return StartCoroutine(CheckOpened());
    }

    public IEnumerator BruteForceAttack(int attack)
    {
        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "に攻撃！");
        }
        else
        {
            if (!this.Model.IsOpened)
            {
                messageLogManager.ShowLog("相手のパスワードカードに攻撃！");
            }
            else
            {
                messageLogManager.ShowLog("相手の" + this.Model.Name + "に攻撃！");
            }
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
                    messageLogManager.ShowLog(this.Model.Name + "カードの効果で攻撃が+２０された！");
                    damage += starCard.Model.AttackToEnemy;//ダメージ+20
                    break;
                case EVENT_CARD_TYPE.BUFFER_OVERFLOW2://バッファオーバーフローⅡカードなら
                    messageLogManager.ShowLog(starCard.Model.Name + "の効果で全てのダメージは2倍された！");
                    damage += attack * starCard.Model.AttackToEnemy;//ダメージ2倍
                    break;
            }
        }
        yield return StartCoroutine(Damage(damage));

        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "に" + damage + "ダメージ！");
        }
        else
        {
            if (!this.Model.IsOpened)
            {
                messageLogManager.ShowLog("相手のパスワードカードに" + damage + "ダメージ！");
            }
            else
            {
                messageLogManager.ShowLog("相手の" + this.Model.Name + "に" + damage + "ダメージ！");
            }
        }

        //前のターンに敵がDoS攻撃カードを使用してなければ
        if (!gameManager.IsPlayerTurn && !gameManager.enemy.UseDoSAttackCardflag)
        {
            //プレイヤーのダメージを受けたカードがは行カードなら
            if (this.Model.IsPlayerCard && !this.Model.IsHacked && this.Model.IsOpened && (this.Model.HP <= this.Model.CanUseSkillHP) && (this.Model.PasswordCardType == PASSWORD_CARD_TYPE.HA_GYOU))
            {
                CardController[] selectableCardList;
                CardController selectedCard;
                FirewallCardController selectedFirewallCard;
                PasswordCardController selectedPasswordCard;

                gameManager.NowSelectingCard = this;
                //敵のファイアウォールカードとパスワードカードを取得
                selectableCardList = gameManager.GetAttackableCardList(!this.Model.IsPlayerCard);
                //プレイヤーが敵のファイアウォールカードもしくはパスワードカードを選択
                gameManager.ChangeTurn();
                yield return StartCoroutine(gameManager.SelectCard(selectableCardList));
                selectedCard = gameManager.NowSelectingCard;
                switch (selectedCard.Model.CardType)
                {
                    case CARD_TYPE.FIREWALL://ファイアウォールカードならば
                        selectedFirewallCard = (FirewallCardController)selectedCard;
                        //ファイアウォールカードに攻撃
                        selectedFirewallCard.BruteForceAttack(this.Model.Attack);
                        break;
                    case CARD_TYPE.PASSWORD://パスワードカードならば
                        selectedPasswordCard = (PasswordCardController)selectedCard;
                        //パスワードカードに攻撃
                        yield return StartCoroutine(selectedPasswordCard.Revenge(this.Model.Attack));
                        break;
                    default:
                        break;
                }
                gameManager.ChangeTurn();
            }
        }

        //前のターンにプレイヤーがDoS攻撃カードを使用してなければ
        if (gameManager.IsPlayerTurn && !gameManager.player.UseDoSAttackCardflag)
        {
            //敵のダメージを受けたカードがは行カードなら
            if (this.Model.IsPlayerCard && !this.Model.IsHacked && this.Model.IsOpened && (this.Model.HP <= this.Model.CanUseSkillHP) && (this.Model.PasswordCardType == PASSWORD_CARD_TYPE.HA_GYOU))
            {
                CardController[] selectableCardList;
                CardController selectedCard;
                FirewallCardController selectedFirewallCard;
                PasswordCardController selectedPasswordCard;

                gameManager.NowSelectingCard = this;
                //プレイヤーのファイアウォールカードとパスワードカードを取得
                selectableCardList = gameManager.GetAttackableCardList(!this.Model.IsPlayerCard);
                //敵がプレイヤーのファイアウォールカードもしくはパスワードカードを選択
                gameManager.ChangeTurn();
                yield return StartCoroutine(gameManager.SelectCard(selectableCardList));
                selectedCard = gameManager.NowSelectingCard;
                switch (selectedCard.Model.CardType)
                {
                    case CARD_TYPE.FIREWALL://ファイアウォールカードならば
                        selectedFirewallCard = (FirewallCardController)selectedCard;
                        //ファイアウォールカードに攻撃
                        selectedFirewallCard.BruteForceAttack(this.Model.Attack);
                        break;
                    case CARD_TYPE.PASSWORD://パスワードカードならば
                        selectedPasswordCard = (PasswordCardController)selectedCard;
                        //パスワードカードに攻撃
                        yield return StartCoroutine(selectedPasswordCard.Revenge(this.Model.Attack));
                        break;
                    default:
                        break;
                }
                gameManager.ChangeTurn();
            }
        }
    }

    public IEnumerator Revenge(int attack)
    {
        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "に攻撃！");
        }
        else
        {
            if (!this.Model.IsOpened)
            {
                messageLogManager.ShowLog("相手のパスワードカードに攻撃！");
            }
            else
            {
                messageLogManager.ShowLog("相手の" + this.Model.Name + "に攻撃！");
            }
        }

        int damage = attack;

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

                //前のターンに相手がDoS攻撃カードを使用してなければ効果発動
                if (!gameManager.IsPlayerTurn && gameManager.enemy.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("相手のDoS攻撃カードの効果でプレイヤーのま行カードの効果は無効化された！");
                    break;
                }

                //前のターンにプレイヤーがDoS攻撃カードを使用してなければ効果発動
                if (gameManager.IsPlayerTurn && gameManager.player.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("プレイヤーのDoS攻撃カードの効果で相手のま行カードの効果は無効化された！");
                    break;
                }

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
                    messageLogManager.ShowLog(this.Model.Name + "カードの効果で攻撃が+２０された！");
                    damage += starCard.Model.AttackToEnemy;//ダメージ+20
                    break;
                case EVENT_CARD_TYPE.BUFFER_OVERFLOW2://バッファオーバーフローⅡカードなら
                    messageLogManager.ShowLog(starCard.Model.Name + "の効果で全てのダメージは2倍された！");
                    damage += attack * starCard.Model.AttackToEnemy;//ダメージ2倍
                    break;
            }
        }
        yield return StartCoroutine(Damage(damage));

        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "に" + damage + "ダメージ！");
        }
        else
        {
            if (!this.Model.IsOpened)
            {
                messageLogManager.ShowLog("相手のパスワードカードに" + damage + "ダメージ！");
            }
            else
            {
                messageLogManager.ShowLog("相手の" + this.Model.Name + "に" + damage + "ダメージ！");
            }
        }
    }

    public void Recovery(int recovery)
    {
        this.Model.Recovery(recovery);
        RefreshView();
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
        Recovery(recovery);

        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "を" + recovery + "回復");
        }
        else
        {
            if (!this.Model.IsOpened)
            {
                messageLogManager.ShowLog("相手のパスワードカードを回復");
            }
            else
            {
                messageLogManager.ShowLog("相手の" + this.Model.Name + "を" + recovery + "回復");
            }
        }
    }

    public IEnumerator SelfHarm(int attack)
    {
        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "に攻撃！");
        }
        else
        {
            if (!this.Model.IsOpened)
            {
                messageLogManager.ShowLog("相手のパスワードカードに攻撃！");
            }
            else
            {
                messageLogManager.ShowLog("相手の" + this.Model.Name + "に攻撃！");
            }
        }

        int damage = attack;
        //場に出ている☆カードを取得
        StarCardController starCard = gameManager.GetStarCard();
        if (starCard != null)
        {
            switch (starCard.Model.EventCardType)
            {
                case EVENT_CARD_TYPE.BUFFER_OVERFLOW://場に出ている☆カードがバッファオーバーフローⅠカードなら
                    messageLogManager.ShowLog(starCard.Model.Name + "の効果で自傷ダメージは2倍された！");
                    damage += attack * starCard.Model.AttackToFriend;//自傷ダメージ2倍
                    break;
                case EVENT_CARD_TYPE.BUFFER_OVERFLOW2://場に出ている☆カードがバッファオーバーフローⅡカードなら
                    messageLogManager.ShowLog(starCard.Model.Name + "の効果で全てのダメージは2倍された！");
                    damage += attack * starCard.Model.AttackToFriend;//自傷ダメージ2倍
                    break;
                case EVENT_CARD_TYPE.FILTERING://場に出ている☆カードがフィルタリングカードなら
                    messageLogManager.ShowLog(starCard.Model.Name + "の効果で自傷ダメージは無効化された！");
                    yield break;//自傷ダメージ無効化
            }
        }
        //ホワイトリストカードを使ったら自傷ダメージ無効化
        if (this.Model.IsPlayerCard && gameManager.player.UseWhiteListCardflag)
        {
            messageLogManager.ShowLog("プレイヤーのホワイトリストカードの効果で自傷ダメージは無効化された！");
            yield break;
        }

        if (!this.Model.IsPlayerCard && gameManager.enemy.UseWhiteListCardflag)
        {
            messageLogManager.ShowLog("相手のホワイトリストカードの効果で自傷ダメージは無効化された！");
            yield break;
        }

        yield return StartCoroutine(Damage(damage));

        if (this.Model.IsPlayerCard)
        {
            messageLogManager.ShowLog("プレイヤーの" + this.Model.Name + "に" + damage + "ダメージ！");
        }
        else
        {
            if (!this.Model.IsOpened)
            {
                messageLogManager.ShowLog("相手のパスワードカードに" + damage + "ダメージ！");
            }
            else
            {
                messageLogManager.ShowLog("相手の" + this.Model.Name + "に" + damage + "ダメージ！");
            }
        }
    }

    public IEnumerator ChangeHP(int changeHP)
    {
        this.Model.ChangeHP(changeHP);
        RefreshView();
        CheckHacked();
        yield return StartCoroutine(CheckOpened());
    }

    public void Hack()
    {
        passwordView.Hack();
    }

    public void RefreshView()
    {
        passwordView.Refresh(this.Model);
    }

    public IEnumerator CheckOpened()
    {
        if (Model.CardOpenflag)
        {
            Model.SetCardOpenflag(false);
            yield return StartCoroutine(OpenPasswordCard());
        }
    }

    public void CheckHacked()
    {
        if (Model.IsHacked)
        {
            Hack();

            if (this.Model.IsPlayerCard)
            {
                messageLogManager.ShowLog("プレイヤーのパスワードがハッキングされた！");
            }
            else
            {
                messageLogManager.ShowLog("相手のパスワードがハッキングされた！");
            }

            gameManager.WinJudgement(Model.IsPlayerCard);
        }
    }

    public IEnumerator OpenPasswordCard()
    {
        Show();
        this.Model.SetIsOpened(true);
        messageLogManager.ShowLog(this.Model.Name + "カードを開示");
        messageLogManager.ShowLog(this.Model.Explanation);
        switch (this.Model.PasswordCardType)
        {
            case PASSWORD_CARD_TYPE.A_GYOU:
            case PASSWORD_CARD_TYPE.KA_GYOU:
            case PASSWORD_CARD_TYPE.SA_GYOU:
            case PASSWORD_CARD_TYPE.TA_GYOU:
            case PASSWORD_CARD_TYPE.NA_GYOU:
            case PASSWORD_CARD_TYPE.HA_GYOU:
            case PASSWORD_CARD_TYPE.MA_GYOU:
                break;
            case PASSWORD_CARD_TYPE.YA_GYOU:
                //前のターンに相手がDoS攻撃を使用してたらパスワードカードの効果を無効化
                if (gameManager.IsPlayerTurn && gameManager.enemy.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("相手のDoS攻撃カードの効果でプレイヤーのや行カードの効果は無効化された！");
                    yield break;
                }

                if (!gameManager.IsPlayerTurn && gameManager.player.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("プレイヤーのDoS攻撃カードの効果で相手のや行カードの効果は無効化された！");
                    yield break;
                }

                //敵のファイアーウォールカードを取得
                FirewallCardController[] firewallCardList = gameManager.GetPlayerFirewallCards(!this.Model.IsPlayerCard);
                //すべて取り除く
                foreach (FirewallCardController card in firewallCardList)
                {
                    card.DisableFirewall();
                }
                gameManager.NowSelectingCard = this;
                //敵のパスワードカードを取得
                PasswordCardController[] enemyPasswordCardList = gameManager.GetPlayerPasswordCards(!this.Model.IsPlayerCard);
                //すべてのパスワードに20ダメージ
                foreach (PasswordCardController card in enemyPasswordCardList)
                {
                    yield return StartCoroutine(card.BruteForceAttack(this.Model.Attack));
                }
                yield break;
            case PASSWORD_CARD_TYPE.RA_GYOU:
                //前のターンに相手がDoS攻撃を使用してたらパスワードカードの効果を無効化
                if (gameManager.IsPlayerTurn && gameManager.enemy.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("相手のDoS攻撃カードの効果でプレイヤーのら行カードの効果は無効化された！");
                    yield break;
                }

                if (!gameManager.IsPlayerTurn && gameManager.player.UseDoSAttackCardflag)
                {
                    messageLogManager.ShowLog("プレイヤーのDoS攻撃カードの効果で相手のら行カードの効果は無効化された！");
                    yield break;
                }

                //山札のカードを山札カード置き場に移動
                gameManager.DeckCardMoveToDeckCardList();
                //山札から好きなカードを2枚選択
                for (int i = 0; i < 2; i++)
                {
                    yield return StartCoroutine(gameManager.GetCardFromCardDeck());
                    gameManager.NowSelectingCard = this;
                }
                //自分の手札のカードを取得
                CardController[] myHandCardList = gameManager.GetPlayerHands(this.Model.IsPlayerCard);
                //手札を2枚捨てる
                for (int i = 0; i < 2; i++)
                {
                    //自分の手札からカードを1枚選択
                    yield return StartCoroutine(gameManager.SelectCard(myHandCardList));
                    Debug.Log(gameManager.NowSelectingCard);
                    //使用済みカード置き場に移動
                    CardController selectedCard = gameManager.NowSelectingCard;
                    gameManager.SetUsedCardToUsedCardField(selectedCard);
                    gameManager.NowSelectingCard = this;
                }
                gameManager.DestroyDeckCardList();
                yield break;
            case PASSWORD_CARD_TYPE.WA_GYOU_N:
                //プレイヤーのパスワードカードなら
                if (this.Model.IsPlayerCard)
                {
                    if (gameManager.enemy.UseDoSAttackCardflag)
                    {
                        messageLogManager.ShowLog("相手のDoS攻撃カードの効果でプレイヤーのわ行カードの効果は無効化された！");
                    }
                    else
                    {
                        //プレイヤーのわ行んカードの開示フラグをONに
                        gameManager.player.OpenWaGyouNCardflag = true;
                    }
                }
                else//敵のパスワードカードなら
                {
                    if (gameManager.player.UseDoSAttackCardflag)
                    {
                        messageLogManager.ShowLog("プレイヤーのDoS攻撃カードの効果で相手のわ行カードの効果は無効化された！");
                    }
                    else
                    {
                        //敵のわ行んカードの開示フラグをONに
                        gameManager.enemy.OpenWaGyouNCardflag = true;
                    }
                }
                yield break;
        }
    }
}

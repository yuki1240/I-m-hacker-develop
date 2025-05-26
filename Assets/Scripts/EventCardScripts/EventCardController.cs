using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardController : CardController
{
    public new EventCardModel Model { get; private set; }//データに関することの操作
    MessageLogManager messageLogManager;

    void Start()
    {
        // メッセージログマネージャーをシーン内から探して取得
        messageLogManager = FindObjectOfType<MessageLogManager>();
    }

    public new void Init(int cardType, int cardID, bool isPlayer)
    {
        base.Init(cardType, cardID, isPlayer);
        Model = new EventCardModel(cardType, cardID, isPlayer);
    }

    public IEnumerator UseEventCard()
    {
        CardController[] passwordCardList;
        CardController[] selectablePasswordCardList;
        CardController selectedCard;
        PasswordCardController selectedPasswordCard;
        CardController[] myHandCardList;
        StarCardController starCard;
        int selectCardNum;
        int drawCardNum;
        messageLogManager.ShowLog(this.Model.Name + "カードを使用");
        messageLogManager.ShowLog(this.Model.Explanation);
        switch (this.Model.EventCardType)
        {
            case EVENT_CARD_TYPE.MAINTENANCE:
                gameManager.action = GameManager.ACTION.HEAL;
                //自分のパスワードカードを取得
                passwordCardList = gameManager.GetHealableCardList(this.Model.IsPlayerCard);
                //自分のパスワードカードを選択
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                //選択したパスワードカードを回復
                selectedPasswordCard = (PasswordCardController)gameManager.NowSelectingCard;
                selectedPasswordCard.Maintenance(this.Model.Heal);
                yield break;
            case EVENT_CARD_TYPE.MAINTENANCE2:
                gameManager.action = GameManager.ACTION.HEAL;
                //パスワードカードを2枚選択して回復
                for (int i = 0; i < 2; i++)
                {
                    //自分のパスワードカードを取得
                    passwordCardList = gameManager.GetHealableCardList(this.Model.IsPlayerCard);
                    //自分のパスワードカードを選択
                    yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                    //選択したパスワードカードを回復
                    selectedPasswordCard = (PasswordCardController)gameManager.NowSelectingCard;
                    selectedPasswordCard.Maintenance(this.Model.Heal);
                    gameManager.NowSelectingCard = this;
                }
                yield break;
            case EVENT_CARD_TYPE.XSS:
                //プレイヤーと敵のパスワードカードを取得
                passwordCardList = gameManager.GetAllPasswordCard();
                //プレイヤーと敵のパスワードカードから1枚選択
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                PasswordCardController passwordCard1 = (PasswordCardController)gameManager.NowSelectingCard;
                passwordCard1.SetIsSelect(true);

                gameManager.NowSelectingCard = this;

                //選択したカードと同じHP以外のパスワードカードを1枚選択
                PasswordCardController[] allPasswordCardList = gameManager.GetAllPasswordCard();
                selectablePasswordCardList = Array.FindAll(allPasswordCardList, card => (!card.Model.IsHacked && (card.Model.HP != passwordCard1.Model.HP)));
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                PasswordCardController passwordCard2 = (PasswordCardController)gameManager.NowSelectingCard;

                //選択した2つのカードのHPを入れ替える
                int tmpNum = passwordCard1.Model.HP;
                yield return StartCoroutine(passwordCard1.ChangeHP(passwordCard2.Model.HP));
                yield return StartCoroutine(passwordCard2.ChangeHP(tmpNum));
                passwordCard1.SetIsSelect(false);
                yield break;
            case EVENT_CARD_TYPE.RESTORATION:
                //使用済みカード置き場からカードを1枚取得
                yield return StartCoroutine(gameManager.GetCardFromUsedCardList());
                //自分の手札を取得
                this.SetIsSelect(true);
                myHandCardList = gameManager.GetPlayerHands(this.Model.IsPlayerCard);
                //自分の手札からカードを1枚選択
                yield return StartCoroutine(gameManager.SelectCard(myHandCardList));
                //選択したカードを使用済みカード置き場に移動
                selectedCard = gameManager.NowSelectingCard;
                gameManager.SetUsedCardToUsedCardField(selectedCard);
                this.SetIsSelect(false);
                yield break;
            case EVENT_CARD_TYPE.INFOMATION_LEAKAGE:
                //自分のパスワードカードを取得
                passwordCardList = gameManager.GetPlayerSelectableCardList(this.Model.IsPlayerCard);
                //自分のパスワードカードを選択
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                selectedPasswordCard = (PasswordCardController)gameManager.NowSelectingCard;
                //選択したカードを表に
                yield return StartCoroutine(selectedPasswordCard.OpenPasswordCard());

                gameManager.NowSelectingCard = this;

                //敵のパスワードカードを取得
                passwordCardList = gameManager.GetPlayerSelectableCardList(!this.Model.IsPlayerCard);
                //敵のパスワードカードを選択
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                selectedPasswordCard = (PasswordCardController)gameManager.NowSelectingCard;
                //選択したカードを表に
                yield return StartCoroutine(selectedPasswordCard.OpenPasswordCard());

                //情報漏洩カード使用フラグをONに
                if (this.Model.IsPlayerCard)
                {
                    gameManager.player.UseInfomationLeakageCardflag = true;
                }
                else
                {
                    gameManager.enemy.UseInfomationLeakageCardflag = true;
                }
                yield break;
            case EVENT_CARD_TYPE.PLUS_MINUS:
                //自分のパスワードカードを取得
                passwordCardList = gameManager.GetPlayerSelectableCardList(this.Model.IsPlayerCard);
                //自分のパスワードカードを選択
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                selectedPasswordCard = (PasswordCardController)gameManager.NowSelectingCard;
                //自傷ダメージ
                yield return StartCoroutine(selectedPasswordCard.SelfHarm(this.Model.AttackToFriend));

                gameManager.NowSelectingCard = this;

                //自分のパスワードカードを選択
                passwordCardList = gameManager.GetHealableCardList(this.Model.IsPlayerCard);
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                selectedPasswordCard = (PasswordCardController)gameManager.NowSelectingCard;
                //選択したカードを回復
                selectedPasswordCard.Maintenance(this.Model.AttackToFriend);
                yield break;
            case EVENT_CARD_TYPE.FALSIFICATION:
                //自分と敵のパスワードカードを取得
                passwordCardList = gameManager.GetAllPasswordCard();
                //パスワードカードを選択
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                selectedPasswordCard = (PasswordCardController)gameManager.NowSelectingCard;
                //HP変更
                yield return StartCoroutine(selectedPasswordCard.ChangeHP(50));
                yield break;
            case EVENT_CARD_TYPE.RECONSTITUITION:
                //自分の手札を取得
                this.SetIsSelect(true);
                myHandCardList = gameManager.GetPlayerHands(this.Model.IsPlayerCard);
                //手札のカードをすべて使用済みカード置き場に移動
                foreach (CardController card in myHandCardList)
                {
                    gameManager.SetUsedCardToUsedCardField(card);
                }
                //カードを5枚ドロー
                for (int i = 0; i < GameManager.Define.maxHandCardNum; i++)
                {
                    gameManager.DrawCard(gameManager.cardDeck, this.Model.IsPlayerCard);
                }
                this.SetIsSelect(false);
                yield break;
            case EVENT_CARD_TYPE.OPTIMIZATION:
                //場に出ている☆カードを取得
                starCard = gameManager.GetStarCard();
                if (starCard != null)
                {
                    //場に出ている☆カードがあれば使用済みカード置き場に移動
                    gameManager.SetUsedCardToUsedCardField(starCard);
                    gameManager.DisableFieldEffectButton();
                }
                yield break;
            case EVENT_CARD_TYPE.CTL_Z:
                //カードを1枚ドロー
                gameManager.DrawCard(gameManager.cardDeck, this.Model.IsPlayerCard);
                //自分の手札を取得
                this.SetIsSelect(true);
                myHandCardList = gameManager.GetPlayerHands(this.Model.IsPlayerCard);
                //自分の手札からカードを1枚選択
                yield return StartCoroutine(gameManager.SelectCard(myHandCardList));
                //選択したカードを山札の一番上に移動
                selectedCard = gameManager.NowSelectingCard;
                gameManager.AddCardToCardDeck(selectedCard);
                this.SetIsSelect(false);
                yield break;
            case EVENT_CARD_TYPE.MIGRATION2:
                drawCardNum = 0;
                //自分の手札を取得
                this.SetIsSelect(true);
                myHandCardList = gameManager.GetPlayerHands(this.Model.IsPlayerCard);
                if (this.Model.IsPlayerCard)
                {
                    selectCardNum = myHandCardList.Length;
                }
                else
                {
                    //1~手札の枚数での値ランダム
                    selectCardNum = (UnityEngine.Random.Range(0, myHandCardList.Length) + 1);
                }
                for (int i = selectCardNum; i > 0; i--)
                {
                    if (this.Model.IsPlayerCard)
                    {
                        if (i < selectCardNum)
                        {
                            gameManager.SetPhaseEndButtonActive(true);
                        }
                    }
                    //自分の手札からカードを1枚選択
                    yield return StartCoroutine(gameManager.SelectCard(myHandCardList));
                    Debug.Log(gameManager.NowSelectingCard);
                    if (this.Model.IsPlayerCard && gameManager.GetPhaseEndButtonActive())
                    {
                        gameManager.SetPhaseEndButtonActive(false);
                    }
                    if (gameManager.NowSelectingCard == null)
                    {
                        break;
                    }
                    drawCardNum++;
                    selectedCard = gameManager.NowSelectingCard;
                    //使用済みカード置き場に移動
                    gameManager.SetUsedCardToUsedCardField(selectedCard);
                    gameManager.NowSelectingCard = null;
                    myHandCardList = gameManager.GetPlayerHands(this.Model.IsPlayerCard);
                }
                Debug.Log(drawCardNum);
                //手札を捨てた枚数だけカードをドロー
                for (int i = drawCardNum; i > 0; i--)
                {
                    gameManager.DrawCard(gameManager.cardDeck, this.Model.IsPlayerCard);
                }
                this.SetIsSelect(false);
                yield break;
            case EVENT_CARD_TYPE.MIGRATION3:
                drawCardNum = 0;
                gameManager.ShowMigration3Field(true);
                //山札のカードを3枚ドロー
                for (int i = 0; i < 3; i++)
                {
                    gameManager.DrawMigration3Card(this.Model.IsPlayerCard);
                }
                //3枚のうち好きな枚数手札に加える
                CardController[] migration3CardList = gameManager.GetMigration3Cards(this.Model.IsPlayerCard);
                if (this.Model.IsPlayerCard)
                {
                    selectCardNum = migration3CardList.Length;
                }
                else
                {
                    //1~3までの値ランダム
                    selectCardNum = (UnityEngine.Random.Range(0, migration3CardList.Length) + 1);
                }
                for (int i = selectCardNum; i > 0; i--)
                {
                    if (this.Model.IsPlayerCard)
                    {
                        if (i < 3)
                        {
                            gameManager.SetPhaseEndButtonActive(true);
                        }
                    }
                    //カードを1枚選択
                    yield return StartCoroutine(gameManager.SelectCard(migration3CardList));
                    Debug.Log(gameManager.NowSelectingCard);
                    if (this.Model.IsPlayerCard && gameManager.GetPhaseEndButtonActive())
                    {
                        gameManager.SetPhaseEndButtonActive(false);
                    }
                    if (gameManager.NowSelectingCard == null)
                    {
                        break;
                    }
                    drawCardNum++;
                    selectedCard = gameManager.NowSelectingCard;
                    //選択したカードを手札に加える
                    gameManager.SetCardToPlayerHand(selectedCard);
                    gameManager.NowSelectingCard = null;
                    migration3CardList = gameManager.GetMigration3Cards(this.Model.IsPlayerCard);
                }
                //手札に加えた枚数だけカードを捨てる
                this.SetIsSelect(true);
                myHandCardList = gameManager.GetPlayerHands(this.Model.IsPlayerCard);
                for (int i = drawCardNum; i > 0; i--)
                {
                    //自分の手札からカードを1枚選択
                    yield return StartCoroutine(gameManager.SelectCard(myHandCardList));
                    Debug.Log(gameManager.NowSelectingCard);
                    //使用済みカード置き場に移動
                    selectedCard = gameManager.NowSelectingCard;
                    gameManager.SetUsedCardToUsedCardField(selectedCard);
                    gameManager.NowSelectingCard = null;
                    myHandCardList = gameManager.GetPlayerHands(this.Model.IsPlayerCard);
                }
                //加えなかった分を好きな順番で山札に戻す
                Debug.Log(migration3CardList.Length);
                for (int i = migration3CardList.Length; i > 0; i--)
                {
                    //カードを1枚選択
                    yield return StartCoroutine(gameManager.SelectCard(migration3CardList));
                    Debug.Log(gameManager.NowSelectingCard);
                    //選択したカードをを山札の一番上に移動
                    selectedCard = gameManager.NowSelectingCard;
                    gameManager.AddCardToCardDeck(selectedCard);
                    yield return null;
                    gameManager.NowSelectingCard = null;
                    migration3CardList = gameManager.GetMigration3Cards(this.Model.IsPlayerCard);
                }
                gameManager.ShowMigration3Field(false);
                this.SetIsSelect(false);
                yield break;
            case EVENT_CARD_TYPE.MIGRATION:
                //使用済みカードの中から☆カードを取得
                CardController[] usedCardList = gameManager.GetUsedCardList();
                CardController[] usedStarCardList = Array.FindAll(usedCardList, card => (card.Model.CardType == CARD_TYPE.STAR));
                if (usedStarCardList.Length > 0)
                {
                    //カードを1枚選択
                    gameManager.ShowUsedCardField(true);
                    yield return StartCoroutine(gameManager.SelectCard(usedStarCardList));
                    gameManager.ShowUsedCardField(false);
                    Debug.Log(gameManager.NowSelectingCard);
                    //選択したカードをを山札の一番上に移動
                    selectedCard = gameManager.NowSelectingCard;
                    gameManager.AddCardToCardDeck(selectedCard);
                }
                yield break;
            case EVENT_CARD_TYPE.RECOVERY:
                //リカバリーカード使用回数に+1
                if (this.Model.IsPlayerCard)
                {
                    messageLogManager.ShowLog("このターンプレイヤーの回復が" + this.Model.Heal + "増加");
                    gameManager.player.RecoveryCardHealBuff += this.Model.Heal;
                }
                else
                {
                    messageLogManager.ShowLog("このターン相手の回復が" + this.Model.Heal + "増加");
                    gameManager.enemy.RecoveryCardHealBuff += this.Model.Heal;
                }
                yield break;
            case EVENT_CARD_TYPE.SCALE_UP:
                //スケールアップカード使用回数に+1
                if (this.Model.IsPlayerCard)
                {
                    messageLogManager.ShowLog("このターンプレイヤーの攻撃力が" + this.Model.AttackToEnemy + "増加");
                    gameManager.player.ScaleUpCardAttackBuff += this.Model.AttackToEnemy;
                }
                else
                {
                    messageLogManager.ShowLog("このターン相手の攻撃力が" + this.Model.AttackToEnemy + "増加");
                    gameManager.enemy.ScaleUpCardAttackBuff += this.Model.AttackToEnemy;
                }
                yield break;
            case EVENT_CARD_TYPE.HEAVY_LOAD:
                //自分のパスワードカードを取得
                passwordCardList = gameManager.GetPlayerSelectableCardList(this.Model.IsPlayerCard);
                //自分のパスワードカードを選択
                yield return StartCoroutine(gameManager.SelectCard(passwordCardList));
                selectedPasswordCard = (PasswordCardController)gameManager.NowSelectingCard;
                //自傷ダメージ
                yield return StartCoroutine(selectedPasswordCard.SelfHarm(this.Model.AttackToFriend));
                //高負荷カード使用回数に+1
                if (this.Model.IsPlayerCard)
                {
                    gameManager.player.UseHeavyLoadCardNum++;
                    messageLogManager.ShowLog("このターンプレイヤーの攻撃回数+１");
                }
                else
                {
                    gameManager.enemy.UseHeavyLoadCardNum++;
                    messageLogManager.ShowLog("このターン相手の攻撃回数+１");
                }
                yield break;
            case EVENT_CARD_TYPE.WHITELIST:
                //ホワイトリストカード使用フラグをONに
                if (this.Model.IsPlayerCard)
                {
                    gameManager.player.UseWhiteListCardflag = true;
                    messageLogManager.ShowLog("このターンプレイヤーのすべての自傷ダメージを無効化");
                }
                else
                {
                    gameManager.enemy.UseWhiteListCardflag = true;
                    messageLogManager.ShowLog("このターン相手のすべての自傷ダメージを無効化");
                }
                yield break;
            case EVENT_CARD_TYPE.REBOOT:
                //再起動カード使用フラグをONに
                if (this.Model.IsPlayerCard)
                {
                    gameManager.player.UseRebootCardflag = true;
                }
                else
                {
                    gameManager.enemy.UseRebootCardflag = true;
                }
                messageLogManager.ShowLog("このターンすべての環境カードを山札に戻しシャッフル");
                yield break;
            case EVENT_CARD_TYPE.NARROW_BAND:
                //ナローバンドカード使用フラグをONに
                if (this.Model.IsPlayerCard)
                {
                    gameManager.player.UseNarrowBandCardflag = true;
                    messageLogManager.ShowLog("次のターン相手は環境カードを使えない！");
                }
                else
                {
                    gameManager.enemy.UseNarrowBandCardflag = true;
                    messageLogManager.ShowLog("次のターンプレイヤーは環境カードを使えない！");
                }
                yield break;
            case EVENT_CARD_TYPE.MALWARE:
                //マルウェアカード使用フラグをONに
                if (this.Model.IsPlayerCard)
                {
                    gameManager.player.UseDoSAttackCardflag = true;
                    messageLogManager.ShowLog("次のターン相手のパスワードカードの効果を無効化");
                }
                else
                {
                    gameManager.enemy.UseDoSAttackCardflag = true;
                    messageLogManager.ShowLog("次のターンプレイヤーのパスワードカードの効果を無効化");
                }
                yield break;
            default:
                break;
        }
    }
}

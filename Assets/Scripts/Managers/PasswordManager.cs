using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordManager : MonoBehaviour
{
    [SerializeField] private MessageLogManager messageLogManager;

    void Start()
    {

    }

    /// <summary>
    /// パスワードが正しいかどうかを判定する
    /// </summary>
    public bool IsCorrectPassword(PasswordCardController[] selectedPasswordCardList, string password)
    {
        if (password.Length != GameManager.Define.passwordLength)
        {
            return false;
        }
        Debug.Log(password);

        for (int i = 0; i < GameManager.Define.passwordLength; i++)
        {
            Debug.Log(selectedPasswordCardList[i].Model.Name + " " + password[i]);
            if (!CheckPassword(selectedPasswordCardList[i], password[i]))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// パスワードの正誤を判定する
    /// </summary>
    /// <param name="selectedPasswordCard"></param>
    /// <param name="passwordLetter"></param>
    /// <returns></returns>
    public bool CheckPassword(PasswordCardController selectedPasswordCard, char passwordLetter)
    {
        switch (passwordLetter)
        {
            case 'あ':
            case 'い':
            case 'う':
            case 'え':
            case 'お':
            case 'ぁ':
            case 'ぃ':
            case 'ぅ':
            case 'ぇ':
            case 'ぉ':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.A_GYOU)
                {
                    return true;
                }
                return false;
            case 'か':
            case 'き':
            case 'く':
            case 'け':
            case 'こ':
            case 'が':
            case 'ぎ':
            case 'ぐ':
            case 'げ':
            case 'ご':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.KA_GYOU)
                {
                    return true;
                }
                return false;
            case 'さ':
            case 'し':
            case 'す':
            case 'せ':
            case 'そ':
            case 'ざ':
            case 'じ':
            case 'ず':
            case 'ぜ':
            case 'ぞ':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.SA_GYOU)
                {
                    return true;
                }
                return false;
            case 'た':
            case 'ち':
            case 'つ':
            case 'て':
            case 'と':
            case 'だ':
            case 'ぢ':
            case 'づ':
            case 'で':
            case 'ど':
            case 'っ':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.TA_GYOU)
                {
                    return true;
                }
                return false;
            case 'な':
            case 'に':
            case 'ぬ':
            case 'ね':
            case 'の':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.NA_GYOU)
                {
                    return true;
                }
                return false;
            case 'は':
            case 'ひ':
            case 'ふ':
            case 'へ':
            case 'ほ':
            case 'ば':
            case 'び':
            case 'ぶ':
            case 'べ':
            case 'ぼ':
            case 'ぱ':
            case 'ぴ':
            case 'ぷ':
            case 'ぺ':
            case 'ぽ':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.HA_GYOU)
                {
                    return true;
                }
                return false;
            case 'ま':
            case 'み':
            case 'む':
            case 'め':
            case 'も':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.MA_GYOU)
                {
                    return true;
                }
                return false;
            case 'や':
            case 'ゆ':
            case 'よ':
            case 'ゃ':
            case 'ゅ':
            case 'ょ':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.YA_GYOU)
                {
                    return true;
                }
                return false;
            case 'ら':
            case 'り':
            case 'る':
            case 'れ':
            case 'ろ':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.RA_GYOU)
                {
                    return true;
                }
                return false;
            case 'わ':
            case 'ゐ':
            case 'ゑ':
            case 'を':
            case 'ん':
            case 'ゎ':
                if (selectedPasswordCard.Model.PasswordCardType == PASSWORD_CARD_TYPE.WA_GYOU_N)
                {
                    return true;
                }
                return false;
            default:
                messageLogManager.ShowLog("パスワードがひらがなのみではありません");
                return false;
        }
    }

    /// <summary>
    /// 敵のパスワードのIDを取得する
    /// </summary>
    /// <param name="passwordCharacter"></param>
    /// <returns></returns>
    public int GetEnemyPasswordID(char passwordCharacter)
    {
        switch (passwordCharacter)
        {
            case 'あ':
            case 'い':
            case 'う':
            case 'え':
            case 'お':
            case 'ぁ':
            case 'ぃ':
            case 'ぅ':
            case 'ぇ':
            case 'ぉ':
                return 1;
            case 'か':
            case 'き':
            case 'く':
            case 'け':
            case 'こ':
            case 'が':
            case 'ぎ':
            case 'ぐ':
            case 'げ':
            case 'ご':
                return 2;
            case 'さ':
            case 'し':
            case 'す':
            case 'せ':
            case 'そ':
            case 'ざ':
            case 'じ':
            case 'ず':
            case 'ぜ':
            case 'ぞ':
                return 3;
            case 'た':
            case 'ち':
            case 'つ':
            case 'て':
            case 'と':
            case 'だ':
            case 'ぢ':
            case 'づ':
            case 'で':
            case 'ど':
            case 'っ':
                return 4;
            case 'な':
            case 'に':
            case 'ぬ':
            case 'ね':
            case 'の':
                return 5;
            case 'は':
            case 'ひ':
            case 'ふ':
            case 'へ':
            case 'ほ':
            case 'ば':
            case 'び':
            case 'ぶ':
            case 'べ':
            case 'ぼ':
            case 'ぱ':
            case 'ぴ':
            case 'ぷ':
            case 'ぺ':
            case 'ぽ':
                return 6;
            case 'ま':
            case 'み':
            case 'む':
            case 'め':
            case 'も':
                return 7;
            case 'や':
            case 'ゆ':
            case 'よ':
            case 'ゃ':
            case 'ゅ':
            case 'ょ':
                return 8;
            case 'ら':
            case 'り':
            case 'る':
            case 'れ':
            case 'ろ':
                return 9;
            case 'わ':
            case 'ゐ':
            case 'ゑ':
            case 'を':
            case 'ん':
            case 'ゎ':
                return 10;
            default:
                messageLogManager.ShowLog("パスワードがひらがなのみではありません");
                return 0;
        }
    }

    /// <summary>
    /// プレイヤーのパスワードを推測する
    /// </summary>
    public char GuessPlayerPassword(PasswordCardController passwordCard)
    {
        int randNum;

        if (passwordCard.Model.IsOpened)
        {
            switch (passwordCard.Model.PasswordCardType)
            {
                case PASSWORD_CARD_TYPE.A_GYOU:
                    randNum = UnityEngine.Random.Range(0, 10);
                    switch (randNum)
                    {
                        case 0:
                            return 'あ';
                        case 1:
                            return 'い';
                        case 2:
                            return 'う';
                        case 3:
                            return 'え';
                        case 4:
                            return 'お';
                        case 5:
                            return 'ぁ';
                        case 6:
                            return 'ぃ';
                        case 7:
                            return 'ぅ';
                        case 8:
                            return 'ぇ';
                        case 9:
                            return 'ぉ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.KA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 10);
                    switch (randNum)
                    {
                        case 0:
                            return 'か';
                        case 1:
                            return 'き';
                        case 2:
                            return 'く';
                        case 3:
                            return 'け';
                        case 4:
                            return 'こ';
                        case 5:
                            return 'が';
                        case 6:
                            return 'ぎ';
                        case 7:
                            return 'ぐ';
                        case 8:
                            return 'げ';
                        case 9:
                            return 'ご';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.SA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 10);
                    switch (randNum)
                    {
                        case 0:
                            return 'さ';
                        case 1:
                            return 'し';
                        case 2:
                            return 'す';
                        case 3:
                            return 'せ';
                        case 4:
                            return 'そ';
                        case 5:
                            return 'ざ';
                        case 6:
                            return 'じ';
                        case 7:
                            return 'ず';
                        case 8:
                            return 'ぜ';
                        case 9:
                            return 'ぞ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.TA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 11);
                    switch (randNum)
                    {
                        case 0:
                            return 'た';
                        case 1:
                            return 'ち';
                        case 2:
                            return 'つ';
                        case 3:
                            return 'て';
                        case 4:
                            return 'と';
                        case 5:
                            return 'だ';
                        case 6:
                            return 'ぢ';
                        case 7:
                            return 'づ';
                        case 8:
                            return 'で';
                        case 9:
                            return 'ど';
                        case 10:
                            return 'っ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.NA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 5);
                    switch (randNum)
                    {
                        case 0:
                            return 'な';
                        case 1:
                            return 'に';
                        case 2:
                            return 'ぬ';
                        case 3:
                            return 'ね';
                        case 4:
                            return 'の';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.HA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 15);
                    switch (randNum)
                    {
                        case 0:
                            return 'は';
                        case 1:
                            return 'ひ';
                        case 2:
                            return 'ふ';
                        case 3:
                            return 'へ';
                        case 4:
                            return 'ほ';
                        case 5:
                            return 'ば';
                        case 6:
                            return 'び';
                        case 7:
                            return 'ぶ';
                        case 8:
                            return 'べ';
                        case 9:
                            return 'ぼ';
                        case 10:
                            return 'ぱ';
                        case 11:
                            return 'ぴ';
                        case 12:
                            return 'ぷ';
                        case 13:
                            return 'ぺ';
                        case 14:
                            return 'ぽ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.MA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 5);
                    switch (randNum)
                    {
                        case 0:
                            return 'ま';
                        case 1:
                            return 'み';
                        case 2:
                            return 'む';
                        case 3:
                            return 'め';
                        case 4:
                            return 'も';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.YA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 6);
                    switch (randNum)
                    {
                        case 0:
                            return 'や';
                        case 1:
                            return 'ゆ';
                        case 2:
                            return 'よ';
                        case 3:
                            return 'ゃ';
                        case 4:
                            return 'ゅ';
                        case 5:
                            return 'ょ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.RA_GYOU:
                    randNum = UnityEngine.Random.Range(0, 5);
                    switch (randNum)
                    {
                        case 0:
                            return 'ら';
                        case 1:
                            return 'り';
                        case 2:
                            return 'る';
                        case 3:
                            return 'れ';
                        case 4:
                            return 'ろ';
                    }
                    return '?';
                case PASSWORD_CARD_TYPE.WA_GYOU_N:
                    randNum = UnityEngine.Random.Range(0, 6);
                    switch (randNum)
                    {
                        case 0:
                            return 'わ';
                        case 1:
                            return 'ゐ';
                        case 2:
                            return 'ゑ';
                        case 3:
                            return 'を';
                        case 4:
                            return 'ん';
                        case 5:
                            return 'ゎ';
                    }
                    return '?';
            }
            return '?';
        }
        else
        {
            randNum = UnityEngine.Random.Range(0, 83);
            switch (randNum)
            {
                case 0:
                    return 'あ';
                case 1:
                    return 'い';
                case 2:
                    return 'う';
                case 3:
                    return 'え';
                case 4:
                    return 'お';
                case 5:
                    return 'ぁ';
                case 6:
                    return 'ぃ';
                case 7:
                    return 'ぅ';
                case 8:
                    return 'ぇ';
                case 9:
                    return 'ぉ';
                case 10:
                    return 'か';
                case 11:
                    return 'き';
                case 12:
                    return 'く';
                case 13:
                    return 'け';
                case 14:
                    return 'こ';
                case 15:
                    return 'が';
                case 16:
                    return 'ぎ';
                case 17:
                    return 'ぐ';
                case 18:
                    return 'げ';
                case 19:
                    return 'ご';
                case 20:
                    return 'さ';
                case 21:
                    return 'し';
                case 22:
                    return 'す';
                case 23:
                    return 'せ';
                case 24:
                    return 'そ';
                case 25:
                    return 'ざ';
                case 26:
                    return 'じ';
                case 27:
                    return 'ず';
                case 28:
                    return 'ぜ';
                case 29:
                    return 'ぞ';
                case 30:
                    return 'た';
                case 31:
                    return 'ち';
                case 32:
                    return 'つ';
                case 33:
                    return 'て';
                case 34:
                    return 'と';
                case 35:
                    return 'だ';
                case 36:
                    return 'ぢ';
                case 37:
                    return 'づ';
                case 38:
                    return 'で';
                case 39:
                    return 'ど';
                case 40:
                    return 'っ';
                case 41:
                    return 'な';
                case 42:
                    return 'に';
                case 43:
                    return 'ぬ';
                case 44:
                    return 'ね';
                case 45:
                    return 'の';
                case 46:
                    return 'は';
                case 47:
                    return 'ひ';
                case 48:
                    return 'ふ';
                case 49:
                    return 'へ';
                case 50:
                    return 'ほ';
                case 51:
                    return 'ば';
                case 52:
                    return 'び';
                case 53:
                    return 'ぶ';
                case 54:
                    return 'べ';
                case 55:
                    return 'ぼ';
                case 56:
                    return 'ぱ';
                case 57:
                    return 'ぴ';
                case 58:
                    return 'ぷ';
                case 59:
                    return 'ぺ';
                case 60:
                    return 'ぽ';
                case 61:
                    return 'ま';
                case 62:
                    return 'み';
                case 63:
                    return 'む';
                case 64:
                    return 'め';
                case 65:
                    return 'も';
                case 66:
                    return 'や';
                case 67:
                    return 'ゆ';
                case 68:
                    return 'よ';
                case 69:
                    return 'ゃ';
                case 70:
                    return 'ゅ';
                case 71:
                    return 'ょ';
                case 72:
                    return 'ら';
                case 73:
                    return 'り';
                case 74:
                    return 'る';
                case 75:
                    return 'れ';
                case 76:
                    return 'ろ';
                case 77:
                    return 'わ';
                case 78:
                    return 'ゐ';
                case 79:
                    return 'ゑ';
                case 80:
                    return 'を';
                case 81:
                    return 'ん';
                case 82:
                    return 'ゎ';
            }
            return '?';
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerManager : MonoBehaviour
{
    //パスワード
    public string Password { get; set; }
    //スキップフラグ
    public bool TurnSkipflag { get; set; }
    //勝利フラグ
    public bool Winflag { get; set; }
    //カード使用フラグ
    public bool UseInfomationLeakageCardflag { get; set; }
    public int UseHeavyLoadCardNum { get; set; }
    public bool UseWhiteListCardflag { get; set; }
    public bool UseRebootCardflag { get; set; }

    public int RecoveryCardHealBuff { get; set; }
    public int ScaleUpCardAttackBuff { get; set; }
    public bool UseNarrowBandCardflag { get; set; }
    public bool UseDoSAttackCardflag { get; set; }
    //パスワードカード開示フラグ
    public bool AttackHaGyouCardflag { get; set; }
    public bool OpenWaGyouNCardflag { get; set; }
    //ハッキング宣言フラグ
    public bool Hackingflag { get; set; }
    public bool HackingFailedflag { get; set; }

    public void Resetflag()
    {
        TurnSkipflag = false;
        Winflag = false;
        UseNarrowBandCardflag = false;
        UseDoSAttackCardflag = false;
        Hackingflag = false;
        HackingFailedflag = false;
    }

    public void ResetAllFlags()
    {
        Resetflag();
        ResetUseCardflag();
        ResetOpenPasswordflag();
    }

    public void ResetUseCardflag()
    {
        UseInfomationLeakageCardflag = false;
        UseHeavyLoadCardNum = 0;
        UseWhiteListCardflag = false;
        UseRebootCardflag = false;

        RecoveryCardHealBuff = 0;
        ScaleUpCardAttackBuff = 0;
        UseNarrowBandCardflag = false;
        UseDoSAttackCardflag = false;
    }

    public void ResetOpenPasswordflag()
    {
        AttackHaGyouCardflag = false;
        OpenWaGyouNCardflag = false;
    }
}

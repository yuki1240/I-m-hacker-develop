using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RuleTextEntity", menuName = "Create RuleTextEntity")]
//ルール画面のテキストのデータそのもの
public class RuleTextEntity : ScriptableObject
{
    [Multiline]
    public string ruleText;
}

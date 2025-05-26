using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPasswordEntity", menuName = "Create EnemyPasswordEntity")]
//敵パスワードのデータそのもの
public class EnemyPasswordEntity : ScriptableObject
{
    public string password;//敵のパスワード
}

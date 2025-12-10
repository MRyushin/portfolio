using System.Collections;
using UnityEngine;

public class BossAttackManager : MonoBehaviour
{
    [Header("攻撃設定")]
    public MonoBehaviour[] attackScripts; // IBossAttack

    // ランダム攻撃実行
    public IEnumerator TriggerRandomAttack()
    {
        if (attackScripts == null || attackScripts.Length == 0)
            yield break;

        int index = Random.Range(0, attackScripts.Length);
        IBossAttack attack = attackScripts[index] as IBossAttack;

        if (attack != null && attack.CanExecute())
        {
            attack.Execute(); // 攻撃開始
            yield return new WaitUntil(() => attack.CanExecute()); // 終了待ち
        }
    }
}

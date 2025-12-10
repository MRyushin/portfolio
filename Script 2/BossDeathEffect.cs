using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossDeathEffect : MonoBehaviour
{
    public static BossDeathEffect Instance;

    [Header("ボス演出")]
    public Sprite[] bossDeathFrames;   // ボスのコマ送り
    public float frameInterval = 0.1f;

    [Header("プレイヤー演出")]
    public Sprite[] playerFrames;      // プレイヤーのコマ送り
    public float playerFrameInterval = 0.1f;

    [Header("遷移設定")]
    public string nextSceneName = "StageSelect";

    void Awake() => Instance = this;

    // ボス撃破位置から演出開始
    public void PlayDeathAnimation(Vector3 bossPosition)
    {
        StartCoroutine(PlaySequence(bossPosition));
    }

    private IEnumerator PlaySequence(Vector3 bossPosition)
    {
        // ボスダミー生成
        GameObject bossDummy = new GameObject("BossDummy");
        bossDummy.transform.position = bossPosition;
        SpriteRenderer bossRenderer = bossDummy.AddComponent<SpriteRenderer>();

        // ボスのコマ送り
        if (bossDeathFrames != null && bossDeathFrames.Length > 0)
        {
            yield return StartCoroutine(PlayFrameAnimation(bossRenderer, bossDeathFrames, frameInterval));
            bossRenderer.sprite = bossDeathFrames[bossDeathFrames.Length - 1];
        }

        // プレイヤー停止
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 子オブジェクト非表示
            for (int i = 0; i < player.transform.childCount; i++)
                player.transform.GetChild(i).gameObject.SetActive(false);

            // 物理停止
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            // スクリプト停止
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var s in scripts) s.enabled = false;

            // プレイヤーのコマ送り
            SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
            if (playerRenderer != null && playerFrames != null && playerFrames.Length > 0)
            {
                yield return StartCoroutine(PlayFrameAnimation(playerRenderer, playerFrames, playerFrameInterval));
                playerRenderer.sprite = playerFrames[playerFrames.Length - 1];
            }
        }

        // シーン遷移
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextSceneName);
    }

    // 共通：コマ送り処理
    private IEnumerator PlayFrameAnimation(SpriteRenderer renderer, Sprite[] frames, float interval)
    {
        foreach (var frame in frames)
        {
            renderer.sprite = frame;
            yield return new WaitForSeconds(interval);
        }
    }
}

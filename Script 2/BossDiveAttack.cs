using System.Collections;
using UnityEngine;

public class BossDiveAttack : MonoBehaviour, IBossAttack
{
    [Header("上昇設定")]
    public float riseHeight = 5f;       // 上昇距離
    public float riseSpeed = 6f;        // 上昇速度

    [Header("突撃設定")]
    public float diveSpeed = 12f;       // 突撃速度
    public float diveDelay = 0.3f;      // 突撃前の待機時間
    public float stopDistance = 0.3f;   // 突撃終了距離

    [Header("アニメーション設定")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] riseFrames;         // 上昇中アニメ
    public Sprite[] diveFrames;         // 突撃中アニメ
    public float frameInterval = 0.1f;  // コマ間隔

    private int currentFrame = 0;
    private float frameTimer = 0f;

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // 実行可否判定
    public bool CanExecute()
    {
        return spriteRenderer != null && player != null && rb != null;
    }

    // 攻撃開始
    public void Execute()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        if (player == null) yield break;

        // 上昇フェーズ
        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + Vector2.up * riseHeight;

        while (transform.position.y < targetPos.y - 0.05f)
        {
            rb.linearVelocity = new Vector2(0, riseSpeed);
            UpdateAnimation(riseFrames);
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;

        // 突撃前待機
        Vector2 diveTarget = player.position; // プレイヤー位置を確定
        yield return new WaitForSeconds(diveDelay);

        // 突撃フェーズ
        Vector2 direction = (diveTarget - (Vector2)transform.position).normalized;
        spriteRenderer.flipX = (direction.x < 0);

        while (Vector2.Distance(transform.position, diveTarget) > stopDistance)
        {
            rb.linearVelocity = direction * diveSpeed;
            UpdateAnimation(diveFrames);
            yield return null;
        }

        // 停止
        rb.linearVelocity = Vector2.zero;
    }

    // コマ送り処理
    private void UpdateAnimation(Sprite[] frames)
    {
        if (frames == null || frames.Length == 0) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= frameInterval)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            spriteRenderer.sprite = frames[currentFrame];
        }
    }
}

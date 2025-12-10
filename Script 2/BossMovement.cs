using System.Collections;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 3f;      // 地上移動速度
    public float jumpForce = 10f;     // ジャンプの上昇力

    private Rigidbody2D rb;
    private bool isGrounded = true;   // 接地判定
    private bool isFacingRight = true; // 現在の向き

    [Header("アニメーション設定")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] walkSprites;      // 歩行時スプライト
    public Sprite[] jumpSprites;      // ジャンプ中スプライト
    public Sprite idleSprite;         // 待機時スプライト
    public float frameInterval = 0.15f;

    private int currentFrame = 0;
    private float frameTimer = 0f;

    [Header("攻撃管理")]
    public BossAttackManager attackManager;
    public float actionInterval = 2.5f;  // 次の行動までの間隔
    private bool isPerformingAction = false;

    [Header("行動確率")]
    [Range(0f, 1f)] public float moveForwardProb = 0.2f;
    [Range(0f, 1f)] public float moveBackwardProb = 0.2f;
    [Range(0f, 1f)] public float idleProb = 0.2f;
    [Range(0f, 1f)] public float jumpProb = 0.2f;
    [Range(0f, 1f)] public float attackProb = 0.2f;

    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // 一定間隔で行動を繰り返す
        StartCoroutine(ActionRoutine());
    }

    IEnumerator ActionRoutine()
    {
        while (true)
        {
            if (!isPerformingAction)
            {
                isPerformingAction = true;
                int action = ChooseAction();

                // プレイヤー方向を向く
                if (player != null)
                    StartCoroutine(FacePlayerContinuously());

                // 行動を選択
                switch (action)
                {
                    case 0: yield return MoveDirection(1); break;   // 前進
                    case 1: yield return MoveDirection(-1); break;  // 後退
                    case 2: yield return Idle(); break;             // 待機
                    case 3: yield return Jump(); break;             // ジャンプ
                    case 4:                                         // 攻撃
                        if (attackManager != null)
                            yield return attackManager.TriggerRandomAttack();
                        break;
                }

                isPerformingAction = false;
            }

            yield return new WaitForSeconds(actionInterval);
        }
    }

    // ランダムアクション
    int ChooseAction()
    {
        float total = moveForwardProb + moveBackwardProb + idleProb + jumpProb + attackProb;
        float rand = Random.value * total;

        if (rand < moveForwardProb) return 0;
        if (rand < moveForwardProb + moveBackwardProb) return 1;
        if (rand < moveForwardProb + moveBackwardProb + idleProb) return 2;
        if (rand < moveForwardProb + moveBackwardProb + idleProb + jumpProb) return 3;
        return 4;
    }

    // 指定方向へ移動
    IEnumerator MoveDirection(int dir)
    {
        float duration = Random.Range(1f, 2f);
        float timer = 0f;

        while (timer < duration)
        {
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
            UpdateAnimation();
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // 一定時間停止
    IEnumerator Idle()
    {
        float duration = Random.Range(1f, 2f);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        float timer = 0f;

        while (timer < duration)
        {
            spriteRenderer.sprite = idleSprite;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    // 着地まで待機
    IEnumerator Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        while (!isGrounded)
        {
            UpdateAnimation();
            yield return null;
        }
    }

    // スプライト切り替え
    void UpdateAnimation()
    {
        frameTimer += Time.deltaTime;

        if (!isGrounded)
        {
            if (frameTimer >= frameInterval)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % jumpSprites.Length;
                spriteRenderer.sprite = jumpSprites[currentFrame];
            }
            return;
        }

        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            if (frameTimer >= frameInterval)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[currentFrame];
            }
        }
        else
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    // 左右反転処理
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // プレイヤー方向を向く
    IEnumerator FacePlayerContinuously()
    {
        while (isPerformingAction)
        {
            if (player == null) yield break;

            bool shouldFlip =
                (player.position.x > transform.position.x && !isFacingRight) ||
                (player.position.x < transform.position.x && isFacingRight);

            if (shouldFlip)
                Flip();

            yield return null;
        }
    }

    // 接地フラグを戻す
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
            isGrounded = true;
    }
}

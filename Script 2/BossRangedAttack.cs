using System.Collections;
using UnityEngine;

public class BossRangedAttack : MonoBehaviour, IBossAttack
{
    [Header("攻撃設定")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 5f;
    public float attackDuration = 1f;
    public float projectileLifetime = 3f; // 弾の寿命（秒）

    [Header("アニメ")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] attackFrames;
    public float frameInterval = 0.1f;

    private int currentFrame = 0;
    private float frameTimer = 0f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // 実行可否
    public bool CanExecute()
    {
        return projectilePrefab != null && firePoint != null;
    }

    // 実行
    public void Execute()
    {
        StartCoroutine(AttackRoutine());
    }

    // モーション → 発射
    private IEnumerator AttackRoutine()
    {
        float timer = 0f;

        // モーション（コマ送り）
        while (timer < attackDuration)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameInterval && attackFrames != null && attackFrames.Length > 0)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % attackFrames.Length;
                spriteRenderer.sprite = attackFrames[currentFrame];
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 発射
        if (projectilePrefab != null && firePoint != null && player != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            Vector2 direction = (player.position - firePoint.position).normalized;
            if (rb != null)
                rb.linearVelocity = direction * projectileSpeed;

            // 弾の見た目の向き
            SpriteRenderer projRenderer = projectile.GetComponent<SpriteRenderer>();
            if (projRenderer != null)
                projRenderer.flipX = (player.position.x < firePoint.position.x);

            Destroy(projectile, projectileLifetime);
        }
    }
}

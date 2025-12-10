using System.Collections;
using UnityEngine;

public class BossBounceAttack : MonoBehaviour, IBossAttack
{
    [Header("äÓñ{ê›íË")]
    public float moveSpeed = 10f;        // à⁄ìÆë¨ìx
    public float attackDuration = 5f;    // çUåÇéûä‘
    public Sprite[] attackSprites;       // ÉRÉ}ëóÇËâÊëú
    public float frameInterval = 0.1f;   // ÉRÉ}ëóÇËä‘äu

    [Header("éQè∆")]
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Collider2D bossCollider;

    private bool isAttacking = false;
    private bool isFacingRight = true;
    private float attackTimer = 0f;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (bossCollider == null) bossCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!isAttacking) return;

        // ÉRÉ}ëóÇË
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameInterval)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % attackSprites.Length;
            spriteRenderer.sprite = attackSprites[currentFrame];
        }

        // çUåÇèIóπ
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackDuration)
            EndAttack();
    }

    void FixedUpdate()
    {
        if (isAttacking)
            rb.linearVelocity = new Vector2(isFacingRight ? moveSpeed : -moveSpeed, 0);
    }

    // IBossAttack
    public bool CanExecute()
    {
        return !isAttacking && rb != null && spriteRenderer != null;
    }

    public void Execute()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        StartAttack();
        yield return new WaitForSeconds(attackDuration);
        EndAttack();
    }

    void StartAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        attackTimer = 0f;
        frameTimer = 0f;
        currentFrame = 0;

        spriteRenderer.sprite = attackSprites[0];
        rb.gravityScale = 0f; // óéâ∫ñhé~
    }

    void EndAttack()
    {
        isAttacking = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1f; // í èÌÇ…ñﬂÇ∑
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAttacking) return;

        if (collision.collider.CompareTag("Wall"))
        {
            // îΩì]
            isFacingRight = !isFacingRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;

            // ñÑÇ‹ÇËñhé~
            Vector2 offset = isFacingRight ? Vector2.right * 0.2f : Vector2.left * 0.2f;
            transform.position = (Vector2)transform.position + offset;
        }
    }
}

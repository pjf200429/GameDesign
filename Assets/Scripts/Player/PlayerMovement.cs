using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public float moveForce = 50f;
    public float maxSpeed = 5f;
    public float jumpForce = 6f;
    public int maxJumps = 2;
    public float deathY = -10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public Transform respawnPoint;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackOffsetX = 1f;
    public GameObject hitEffectPrefab;

    [Header("Visuals")]
    public Transform visualRoot;

    private Rigidbody2D rb;
    private int jumpCount;
    private bool isGrounded;
    private bool isFacingRight = true;

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        UpdateAttackPointFacing();

        PlayerHealthController health = GetComponent<PlayerHealthController>();
        if (health != null)
        {
            health.OnPlayerDied += DieAndRespawn;
        }
    }

    void Update()
    {
        if (isDead) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && jumpCount != 0)
        {
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }

        if (transform.position.y < deathY)
        {
            DieAndRespawn();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlaySlashEffect();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();

        if (Mathf.Abs(rb.velocity.x) < maxSpeed || Mathf.Sign(moveInput) != Mathf.Sign(rb.velocity.x))
        {
            rb.AddForce(new Vector2(moveInput * moveForce, 0f));
        }

        if (moveInput == 0 && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);
        }

        if (anim != null)
        {
            bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;
            anim.SetBool("1_Move", isMoving);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        if (visualRoot != null)
        {
            Vector3 scale = visualRoot.localScale;
            scale.x *= -1;
            visualRoot.localScale = scale;
        }

        UpdateAttackPointFacing();
    }

    private void UpdateAttackPointFacing()
    {
        if (attackPoint != null)
        {
            Vector3 localPos = attackPoint.localPosition;
            localPos.x = Mathf.Abs(attackOffsetX) * (isFacingRight ? 1 : -1);
            attackPoint.localPosition = localPos;
        }
    }

    private void PlaySlashEffect()
    {
        if (hitEffectPrefab != null && attackPoint != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, attackPoint.position, Quaternion.identity);
            Vector3 scale = effect.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1 : -1);
            effect.transform.localScale = scale;
        }
    }

    public void DieAndRespawn()
    {
        if (isDead) return;

        isDead = true;
        rb.velocity = Vector2.zero;

        if (anim != null)
        {
            anim.SetBool("4_Death", true); //  修改这里的参数名
       
        }

        float deathAnimDuration = 0.8f; // 根据你的动画 clip 长度调整
        Invoke(nameof(Respawn), deathAnimDuration);
    }



    private void Respawn()
    {
        if (anim != null)
        {
            anim.SetBool("4_Death", false); //  改为匹配参数名
        }

        // 重置位置和状态
        transform.position = respawnPoint.position;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector2.zero;
        jumpCount = 0;

        GetComponent<PlayerHealthController>()?.ResetHealth();

        isDead = false;
    }



    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void OnDestroy()
    {
        PlayerHealthController health = GetComponent<PlayerHealthController>();
        if (health != null)
        {
            health.OnPlayerDied -= DieAndRespawn;
        }
    }
}
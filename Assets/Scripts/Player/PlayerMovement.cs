using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveForce = 50f;
    public float maxSpeed = 5f;
    public float jumpForce = 6f;
    public int maxJumps = 2;
    public float deathY = -10f;

    [Header("Ground Check (Local Offset)")]
    public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Respawn")]
    public Transform respawnPoint;  // Automatically finds the object tagged "RespawnPoint" after the scene is loaded

    [Header("Attack Point (for PlayerCombat)")]
    public Transform attackPoint;
    public float attackOffsetX = 1f;

    [Header("Visuals")]
    public Transform visualRoot;

    Rigidbody2D rb;
    Animator anim;
    int jumpCount;
    bool isGrounded;
    bool isFacingRight = true;
    bool isDead = false;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var go = GameObject.FindGameObjectWithTag("RespawnPoint");
        respawnPoint = go ? go.transform : null;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        UpdateAttackPointFacing();

        var health = GetComponent<PlayerHealthController>();
        if (health != null)
            health.OnPlayerDied += DieAndRespawn;
    }

    void Update()
    {
        if (isDead) return;

        // Ground check
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        isGrounded = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        if (isGrounded) jumpCount = 0;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }

        // Out of bounds (death zone)
        if (transform.position.y < deathY)
            DieAndRespawn();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float h = Input.GetAxisRaw("Horizontal");
        if (h > 0 && !isFacingRight) Flip();
        else if (h < 0 && isFacingRight) Flip();

        // ！！ Slope-based force calculation start ！！  
        // 1. Default horizontal force direction
        Vector2 forceDir = Vector2.right * h;

        // 2. Raycast downward from foot to get slope normal
        Vector2 footPos = (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D slopeHit = Physics2D.Raycast(footPos, Vector2.down, groundCheckRadius * 2f, groundLayer);

        if (slopeHit.collider != null && Mathf.Abs(h) > 0.01f)
        {
            // Slope tangent = normal rotated 90＜ clockwise
            Vector2 slopeTangent = new Vector2(slopeHit.normal.y, -slopeHit.normal.x);
            // Ensure tangent direction matches horizontal input
            forceDir = slopeTangent * Mathf.Sign(Vector2.Dot(slopeTangent, Vector2.right * h));

            // Debug visualization: draw green line for slope force direction
            Debug.DrawRay(footPos, slopeTangent * 0.5f, Color.green);
        }
        // ！！ Slope-based force calculation end ！！  

        // Apply force if below max speed or changing direction
        if (Mathf.Abs(rb.velocity.x) < maxSpeed || Mathf.Sign(h) != Mathf.Sign(rb.velocity.x))
        {
            rb.AddForce(forceDir.normalized * moveForce);
        }

        // Ground damping when no input
        if (Mathf.Approximately(h, 0f) && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);

        // Sync animation
        if (anim != null)
            anim.SetBool("1_Move", Mathf.Abs(rb.velocity.x) > 0.1f);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        if (visualRoot != null)
        {
            var s = visualRoot.localScale;
            s.x *= -1;
            visualRoot.localScale = s;
        }
        UpdateAttackPointFacing();
    }

    void UpdateAttackPointFacing()
    {
        if (attackPoint != null)
        {
            var lp = attackPoint.localPosition;
            lp.x = Mathf.Abs(attackOffsetX) * (isFacingRight ? 1 : -1);
            attackPoint.localPosition = lp;
        }
    }

    public void DieAndRespawn()
    {
        if (isDead) return;
        isDead = true;
        rb.velocity = Vector2.zero;
        if (anim != null) anim.SetBool("4_Death", true);

        Invoke(nameof(Respawn), 0.8f);
    }

    void Respawn()
    {
        if (anim != null) anim.SetBool("4_Death", false);
        if (respawnPoint == null)
        {
            Debug.LogError("[PlayerMovement] No RespawnPoint found, cannot respawn!");
            return;
        }
        transform.position = respawnPoint.position;
        rb.velocity = Vector2.zero;
        jumpCount = 0;
        GetComponent<PlayerHealthController>()?.ResetHealth();
        isDead = false;
    }

    void OnDrawGizmosSelected()
    {
        // Original ground check circle
        var pos = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, groundCheckRadius);

        // Visualize raycast for slope detection
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + Vector2.down * groundCheckRadius * 2f);
    }
}

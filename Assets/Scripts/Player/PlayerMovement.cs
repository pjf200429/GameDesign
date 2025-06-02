// PlayerMovement.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // ���������������� Inspector ���ֶ����ã����Ǵ� PlayerAttributes �Զ���ȡ
    private float moveForce;
    private float maxSpeed;
    private float jumpForce;

    [Header("Movement Settings")]
    public int maxJumps = 2;
    public float deathY = -10f;

    [Header("Ground Check (Local Offset)")]
    public Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Respawn")]
    public Transform respawnPoint;  // �������غ�ͨ�� Tag �Զ���ֵ

    [Header("Attack Point (for PlayerCombat)")]
    public Transform attackPoint;
    public float attackOffsetX = 1f;

    [Header("Visuals")]
    public Transform visualRoot;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerAttributes playerAttributes;

    private int jumpCount;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isDead = false;

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
        playerAttributes = GetComponent<PlayerAttributes>();
        if (playerAttributes == null)
        {
            Debug.LogError("[PlayerMovement] �Ҳ��� PlayerAttributes����ȷ��������ͬһ GameObject �ϡ�");
            return;
        }

        // 1. ���ˮƽ�ٶ�
        maxSpeed = playerAttributes.MovementSpeed;

        // 2. ���������е� JumpHeight ������� gravityScale ������Ծ����
        float globalGravityY = Physics2D.gravity.y;                            // ͨ�� = -9.81
        float actualG = Mathf.Abs(globalGravityY) * rb.gravityScale;            // ʵ���������ٶ�
        float desiredHeight = playerAttributes.JumpHeight;                      // ��Ҫ�����ĸ߶�
        float v0 = Mathf.Sqrt(2f * actualG * desiredHeight);                    // ������ٶ�
        jumpForce = rb.mass * v0;                                               // ���� = ���� �� ���ٶ�

        // 3. ����ˮƽ����
        moveForce = maxSpeed * 10f;   // ������maxSpeed �� 10 �ɿ��ٴﵽ�ٶ�����

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
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps - 1)
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

        // ���� Slope-based force calculation start ����  
        Vector2 forceDir = Vector2.right * h;
        Vector2 footPos = (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D slopeHit = Physics2D.Raycast(footPos, Vector2.down, groundCheckRadius * 2f, groundLayer);

        if (slopeHit.collider != null && Mathf.Abs(h) > 0.01f)
        {
            Vector2 slopeTangent = new Vector2(slopeHit.normal.y, -slopeHit.normal.x);
            forceDir = slopeTangent * Mathf.Sign(Vector2.Dot(slopeTangent, Vector2.right * h));
            Debug.DrawRay(footPos, slopeTangent * 0.5f, Color.green);
        }
        // ���� Slope-based force calculation end ����  

        if (Mathf.Abs(rb.velocity.x) < maxSpeed || Mathf.Sign(h) != Mathf.Sign(rb.velocity.x))
        {
            rb.AddForce(forceDir.normalized * moveForce);
        }

        if (Mathf.Approximately(h, 0f) && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);

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
        var pos = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, groundCheckRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + Vector2.down * groundCheckRadius * 2f);
    }
}

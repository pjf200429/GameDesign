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
    // ���ֶ��ϣ�����ʱ�Զ����ݳ����� Tag ����
    public Transform respawnPoint;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackOffsetX = 1f;
    public GameObject hitEffectPrefab;

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
        // ��֤�ڳ������غ���ҳ�����
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ÿ�γ����������ʱ�ص�
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        try
        {
            var go = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (go != null)
            {
                respawnPoint = go.transform;
                Debug.Log($"[PlayerMovement] �ڳ�����{scene.name}�����ҵ� RespawnPoint");
            }
            else
            {
                respawnPoint = null;
                Debug.Log($"[PlayerMovement] ������{scene.name}������ RespawnPoint�����޷�����");
            }
        }
        catch (UnityException)
        {
            respawnPoint = null;
            Debug.Log($"[PlayerMovement] δ���� Tag ��RespawnPoint�����������������");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        // ������峯��
        UpdateAttackPointFacing();

        var health = GetComponent<PlayerHealthController>();
        if (health != null)
            health.OnPlayerDied += DieAndRespawn;
    }

    void Update()
    {
        if (isDead) return;

        // ������
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        isGrounded = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);

        if (isGrounded && jumpCount > 0)
            jumpCount = 0;

        // ��Ծ
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }

        // ��������
        if (transform.position.y < deathY)
            DieAndRespawn();

        // ������Ч
        if (Input.GetKeyDown(KeyCode.K))
            PlaySlashEffect();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float h = Input.GetAxisRaw("Horizontal");
        if (h > 0 && !isFacingRight) Flip();
        else if (h < 0 && isFacingRight) Flip();

        if (Mathf.Abs(rb.velocity.x) < maxSpeed || Mathf.Sign(h) != Mathf.Sign(rb.velocity.x))
            rb.AddForce(Vector2.right * h * moveForce);

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

    void PlaySlashEffect()
    {
        if (hitEffectPrefab != null && attackPoint != null)
        {
            var fx = Instantiate(hitEffectPrefab, attackPoint.position, Quaternion.identity);
            var s = fx.transform.localScale;
            s.x = Mathf.Abs(s.x) * (isFacingRight ? 1 : -1);
            fx.transform.localScale = s;
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
            Debug.LogError("[PlayerMovement] �޿��� RespawnPoint���޷����");
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
    }
}

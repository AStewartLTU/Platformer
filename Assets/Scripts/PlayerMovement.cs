
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    private PlayerHealth playerHealth;

    private bool grounded;

    private Vector3 startPosition;
    private Camera mainCamera;
    private Vector3 cameraStart;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();

        startPosition = transform.position;

        mainCamera = Camera.main;

        if (mainCamera != null)
        {
            cameraStart = mainCamera.transform.position;
        }

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.collisionDetectionMode =
                CollisionDetectionMode2D.Continuous;
        }
    }

    void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
        {
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            return;
        }

        // Horizontal movement
        float move = 0f;

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow))
        {
            move = -1f;
        }

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            move = 1f;
        }

        rb.linearVelocity = new Vector2(
            move * moveSpeed,
            rb.linearVelocity.y
        );

        // Flip player sprite
        if (move != 0 && playerSprite != null)
        {
            playerSprite.flipX = move < 0;
        }

        // Jumping
        if ((Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.W) ||
             Input.GetKeyDown(KeyCode.UpArrow))
             && grounded)
        {
            grounded = false;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }

        FollowPlayer();

        // Fall detection
        if (transform.position.y < -18f)
        {
            playerHealth.TakeDamage(true);
        }
    }

    // Ground detection
    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.45f)
            {
                grounded = true;
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    // Enemy collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GameManager.Instance.IsPlaying)
            return;

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            bool stomp =
                rb.linearVelocity.y <= 0.5f &&
                transform.position.y >
                enemy.transform.position.y + 0.35f;

            if (stomp)
            {
                enemy.Die();

                GameManager.Instance.AddScore(500);

                // Bounce after defeating enemy
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    jumpForce * 0.72f
                );

                grounded = false;
            }
            else
            {
                playerHealth.TakeDamage(false);
            }
        }
        else if (collision.gameObject.name == "VictoryFlag")
        {
            GameManager.Instance.Victory();
        }
    }

    // Coin collection and victory detection
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.Instance.IsPlaying)
            return;

        if (other.gameObject.name.StartsWith("Coin"))
        {
            GameManager.Instance.AddScore(50);

            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.name == "VictoryFlag")
        {
            GameManager.Instance.Victory();
        }
    }

    // Camera follows player
    void FollowPlayer()
    {
        if (mainCamera == null)
            return;

        Vector3 target = mainCamera.transform.position;

        target.x = Mathf.Clamp(
            transform.position.x + 2f,
            cameraStart.x - 2f,
            13f
        );

        target.y = cameraStart.y;

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            target,
            Time.deltaTime * 5f
        );
    }

    // Reset player position
    public void ResetPlayerPosition()
    {
        transform.position = startPosition;

        grounded = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (mainCamera != null)
        {
            mainCamera.transform.position = cameraStart;
        }
    }
}

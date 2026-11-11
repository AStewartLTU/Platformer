
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 3;

    public float invulnerabilityDuration = 2.2f;

    private int health;

    private float invulnerableUntil;
    private float nextFlash;

    private bool flashVisible = true;

    private SpriteRenderer playerSprite;
    private Rigidbody2D rb;

    private Vector3 startPosition;

    void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        startPosition = transform.position;

        health = startingHealth;
    }

    void Update()
    {
        UpdateFlash();
    }

    // Take damage
    public void TakeDamage(bool respawn)
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
            return;

        // Prevent damage during invulnerability
        if (Time.time < invulnerableUntil)
            return;

        health--;

        invulnerableUntil =
            Time.time + invulnerabilityDuration;

        nextFlash = 0f;
        flashVisible = true;

        if (playerSprite != null)
        {
            playerSprite.enabled = true;
        }

        // Check if player is dead
        if (health <= 0)
        {
            GameManager.Instance.GameOver();
            return;
        }

        // Respawn after falling
        if (respawn)
        {
            transform.position = startPosition;
        }

        // Knock player upward
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                5f
            );
        }
    }

    // Flashing effect
    void UpdateFlash()
    {
        if (playerSprite == null)
            return;

        if (Time.time >= invulnerableUntil)
        {
            playerSprite.enabled = true;
            return;
        }

        if (Time.time >= nextFlash)
        {
            flashVisible = !flashVisible;

            playerSprite.enabled = flashVisible;

            nextFlash = Time.time + 0.12f;
        }
    }

    // Reset health when restarting
    public void ResetHealth()
    {
        health = startingHealth;

        invulnerableUntil = 0f;

        flashVisible = true;

        if (playerSprite != null)
        {
            playerSprite.enabled = true;
        }
    }

    public int GetHealth()
    {
        return health;
    }
}


using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 1.8f;
    public float moveDistance = 1.35f;

    private Vector3 startingPosition;

    void Awake()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
            return;

        MoveEnemy();
    }

    // Move enemy left and right
    void MoveEnemy()
    {
        float movement = Mathf.Sin(
            Time.time * moveSpeed
        ) * moveDistance;

        transform.position = new Vector3(
            startingPosition.x + movement,
            startingPosition.y,
            startingPosition.z
        );
    }

    // Defeat enemy
    public void Die()
    {
        gameObject.SetActive(false);
    }

    // Reset enemy after restarting
    public void ResetEnemy()
    {
        transform.position = startingPosition;

        gameObject.SetActive(true);
    }
}

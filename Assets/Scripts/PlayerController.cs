using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public float speed = 5f;
    public float maxSpeed = 10f;
    public float grassSlowdown = 0.3f;
    public float rotationSpeed = 20f;
    public bool TouchedGrass { get; private set; }
    public bool HitWall { get; private set; }

    private Rigidbody2D rb;
    private bool isOnGrassSand;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearDamping = 1;
    }

    public void Move(Vector2 direction)
    {
        rb.AddForce(direction * speed);
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);

        if (isOnGrassSand)
        {
            rb.linearVelocity *= grassSlowdown;
        }

        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.CompareTag("GrassSand"))
        {
            isOnGrassSand = true;
            TouchedGrass = true;
        }
        else if (trigger.CompareTag("FinishLine"))
        {
            CountdownTimer timer = UnityEngine.Object.FindObjectOfType<CountdownTimer>();
            LevelManager levelManager = UnityEngine.Object.FindObjectOfType<LevelManager>();

            if (timer != null) timer.PlayerWins();
            if (levelManager != null) levelManager.CompleteLevel();
        }
    }

    private void OnTriggerExit2D(Collider2D trigger)
    {
        if (trigger.CompareTag("GrassSand"))
        {
            isOnGrassSand = false;
        }
    }

    public void HandleCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.linearVelocity = Vector2.zero;
            HitWall = true;
        }
    }
}
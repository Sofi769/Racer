using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyCheckpointAI : MonoBehaviour
{
    [Header("Path")]
    public CheckpointPath path;
    public int startIndex = 0;
    public bool loop = true;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float turnSpeed = 240f;

    [Header("Steering")]
    public float arriveDistance = 0.8f;     // checkpoint'e yaklaşınca sonraki
    public float slowAngle = 50f;           // viraj açısı
    public float minThrottle = 0.55f;       // virajda minimum gaz

    [Header("Anti-stuck")]
    public float stuckCheckTime = 1.0f;
    public float stuckMinMove = 0.15f;
    public float reverseTime = 0.25f;

    Rigidbody2D rb;
    int index;
    Vector2 lastPos;
    float stuckTimer;
    float reverseTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPos = rb.position;
        index = startIndex;
    }

    void FixedUpdate()
    {
        if (path == null || path.Count == 0) return;

        // anti-stuck
        stuckTimer += Time.fixedDeltaTime;
        if (stuckTimer >= stuckCheckTime)
        {
            float moved = Vector2.Distance(rb.position, lastPos);
            lastPos = rb.position;
            stuckTimer = 0f;
            if (moved < stuckMinMove) reverseTimer = reverseTime;
        }

        if (reverseTimer > 0f)
        {
            reverseTimer -= Time.fixedDeltaTime;
            // geri + hafif dön
            Move(-1f, 0.8f);
            return;
        }

        Transform target = path.GetPoint(index);
        if (target == null) return;

        Vector2 toTarget = ((Vector2)target.position - rb.position);
        float dist = toTarget.magnitude;

        // checkpoint'e geldiyse sıradakine geç
        if (dist <= arriveDistance)
        {
            index++;
            if (loop) index %= path.Count;
            else index = Mathf.Min(index, path.Count - 1);
            return;
        }

        Vector2 forward = transform.up;
        Vector2 desiredDir = toTarget.normalized;

        // SignedAngle: sola +, sağa -
        float angle = Vector2.SignedAngle(forward, desiredDir);

        // steer -1..1
        float steer = Mathf.Clamp(angle / 60f, -1f, 1f);

        // virajda otomatik yavaşla
        float a = Mathf.Abs(angle);
        float throttle = Mathf.Lerp(1f, minThrottle, Mathf.InverseLerp(0f, slowAngle, a));

        Move(throttle, steer);
    }

    void Move(float throttle, float steer)
    {
        rb.MoveRotation(rb.rotation + (steer * turnSpeed * Time.fixedDeltaTime));
        rb.linearVelocity = (Vector2)transform.up * (throttle * moveSpeed);
    }
}

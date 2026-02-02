using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [Header("Speed")]
    public float maxSpeed = 10f;
    public float acceleration = 20f;
    public float deceleration = 25f;

    [Header("Steering")]
    public float turnSpeed = 220f;
    public float turnSpeedAtZero = 120f;

    [Header("Grip (Normal)")]
    [Range(0f, 1f)] public float lateralFriction = 0.9f;

    [Header("Drift (Space)")]
    [Range(0f, 1f)] public float driftLateralFriction = 0.55f;
    public float driftTurnMultiplier = 1.3f;
    public float driftMinSpeed = 2.5f;

    [Header("Surface")]
    [Range(0f, 1f)] public float grassMaxSpeedMultiplier = 0.6f;
    [Range(0f, 1f)] public float grassGripMultiplier = 0.7f;

    [Header("Drift Smoke")]
    public ParticleSystem driftSmoke;
    public float smokeMinSpeed = 2.5f;
    public float minSteerForSmoke = 0.1f;
    public float minThrottleForSmoke = 0.05f;

    [Tooltip("Drift başladığı anda bir kere tetiklemek için")]
    public int kickstartEmitCount = 3;

    public bool TouchedGrass { get; private set; }
    public bool HitWall { get; private set; }

    private Rigidbody2D rb;
    private float steer;
    private float throttle;
    private bool isOnGrassSand;
    private bool isDrifting;

    // smoke state
    private bool smokeWasOn = false;
    private float cachedSmokeRate = 40f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 999f;

        if (driftSmoke != null)
        {
            // ParticleSystem içindeki Rate'ı cache'leyelim (Inspector'da verdiğin normal sayı)
            var emission = driftSmoke.emission;
            emission.enabled = true; // modül açık kalsın

            // rateOverTimeMultiplier genelde en güvenlisi (curve/constant fark etmez)
            cachedSmokeRate = emission.rateOverTimeMultiplier;
            if (cachedSmokeRate <= 0.01f) cachedSmokeRate = 40f;

            // Başta duman kapalı (emission modülünü kapatacağız)
            emission.enabled = false;
        }
    }

    public void Move(Vector2 direction)
    {
        steer = Mathf.Clamp(direction.x, -1f, 1f);
        throttle = Mathf.Clamp(direction.y, -1f, 1f);
        isDrifting = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        ApplySteering();
        ApplyThrottle();
        ApplyGrip();
        UpdateDriftSmoke();
    }

    private void ApplySteering()
    {
        float speed = rb.linearVelocity.magnitude;
        float t = Mathf.InverseLerp(0f, maxSpeed, speed);
        float currentTurn = Mathf.Lerp(turnSpeedAtZero, turnSpeed, t);

        if (isDrifting && speed > driftMinSpeed)
            currentTurn *= driftTurnMultiplier;

        float newAngle = rb.rotation - steer * currentTurn * Time.fixedDeltaTime;
        rb.MoveRotation(newAngle);
    }

    private void ApplyThrottle()
    {
        float speedMultiplier = isOnGrassSand ? grassMaxSpeedMultiplier : 1f;
        float targetMaxSpeed = maxSpeed * speedMultiplier;

        Vector2 forward = transform.up;

        if (Mathf.Abs(throttle) > 0.01f)
        {
            rb.linearVelocity = Vector2.MoveTowards(
                rb.linearVelocity,
                forward * (throttle * targetMaxSpeed),
                acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            rb.linearVelocity = Vector2.MoveTowards(
                rb.linearVelocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, targetMaxSpeed);
    }

    private void ApplyGrip()
    {
        Vector2 forward = transform.up;
        Vector2 right = transform.right;

        float gripMultiplier = isOnGrassSand ? grassGripMultiplier : 1f;

        float baseGrip = lateralFriction;
        if (isDrifting && rb.linearVelocity.magnitude > driftMinSpeed)
            baseGrip = driftLateralFriction;

        float grip = Mathf.Clamp01(baseGrip * gripMultiplier);

        float forwardVel = Vector2.Dot(rb.linearVelocity, forward);
        float lateralVel = Vector2.Dot(rb.linearVelocity, right);

        lateralVel *= grip;
        rb.linearVelocity = forward * forwardVel + right * lateralVel;
    }

    private void UpdateDriftSmoke()
    {
        if (driftSmoke == null) return;

        bool shouldSmoke =
            isDrifting &&
            rb.linearVelocity.magnitude > smokeMinSpeed &&
            Mathf.Abs(steer) > minSteerForSmoke &&
            Mathf.Abs(throttle) > minThrottleForSmoke;

        var emission = driftSmoke.emission;

        if (shouldSmoke)
        {
            // Emission aç
            emission.enabled = true;

            // Rate'ı garanti sabitle (0'a düşmüş olma ihtimaline karşı)
            emission.rateOverTimeMultiplier = cachedSmokeRate;

            // Sistem kesin playing olsun
            if (!driftSmoke.isPlaying)
                driftSmoke.Play(true);

            // Drift yeni başladıysa "kick" ver
            if (!smokeWasOn)
                driftSmoke.Emit(kickstartEmitCount);
        }
        else
        {
            emission.enabled = false;
        }

        smokeWasOn = shouldSmoke;
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
    LevelManager levelManager = Object.FindFirstObjectByType<LevelManager>();
    if (levelManager != null) levelManager.CompleteLevel();
        }
    }

    private void OnTriggerExit2D(Collider2D trigger)
    {
        if (trigger.CompareTag("GrassSand"))
            isOnGrassSand = false;
    }

    public void HandleCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.linearVelocity = Vector2.zero;
            HitWall = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
    HandleCollision(collision);
    }

}

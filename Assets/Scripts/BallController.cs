using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Velocidades")]
    public float initialSpeed = 18f;
    public float speedIncrementPerScore = 2f;
    public float maxSpeed = 30f;
    public float minPlaySpeed = 8f;

    private Rigidbody rb;
    private Vector3 lastVelocity;
    private float currentSpeed;
    public bool isActive = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = false;
        currentSpeed = initialSpeed;
    }

    public void Launch(int direction = 0)
    {
        rb.velocity = Vector3.zero;

        int dir = (int)(direction != 0 ? Mathf.Sign(direction) : (Random.value < 0.5f ? -1 : 1));
        float yRange = 0.5f;
        float y = Random.Range(-yRange, yRange);

        Vector3 v = new Vector3(dir, y, 0f).normalized * currentSpeed;
        rb.velocity = v;
        isActive = true;
        Debug.Log($"Bola lanzada: velocidad={rb.velocity}, magnitud={rb.velocity.magnitude}");
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            float speed = rb.velocity.magnitude;
            if (speed > 0.01f && speed < minPlaySpeed)
            {
                rb.velocity = rb.velocity.normalized * minPlaySpeed;
            }
        }

        lastVelocity = rb.velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        Vector3 v = Vector3.Reflect(lastVelocity, col.contacts[0].normal);
        float speed = Mathf.Clamp(lastVelocity.magnitude, minPlaySpeed, maxSpeed);
        rb.velocity = v.normalized * speed;
    }

    public void IncreaseSpeed(int round)
    {
        currentSpeed = Mathf.Clamp(initialSpeed + round * speedIncrementPerScore, 0f, maxSpeed);

        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            rb.velocity = rb.velocity.normalized * currentSpeed;
        }
    }

    public void ResetSpeed()
    {
        currentSpeed = initialSpeed;

        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            rb.velocity = rb.velocity.normalized * currentSpeed;
        }
    }

    public void Stop()
    {
        isActive = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}

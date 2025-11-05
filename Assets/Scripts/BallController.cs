using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public float initialSpeed = 18;
    public float speedIncrementPerScore = 4;
    public float maxSpeed = 30f;

    private Rigidbody rb;
    private Vector3 lastVelocity;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        currentSpeed = initialSpeed;
    }

    // IMPORTANTE: Comentamos OnEnable para que no lance automáticamente
    // void OnEnable()
    // {
    //     Launch();
    // }

    public void Launch(int direction = 0)
    {
        // Aseguramos que la bola está parada antes de lanzar
        rb.velocity = Vector3.zero;

        int dir = (int)(direction != 0 ? Mathf.Sign(direction) : (Random.value < 0.5f ? -1 : 1));
        // randomizamos el rebote (para que no sea recto y aburrido)
        float y = Random.Range(-1.2f, 1.2f);
        Vector3 v = new Vector3(dir, y, 0f).normalized * currentSpeed;
        rb.velocity = v;
        // si no no me entero
        Debug.Log($"Bola lanzada: velocidad={rb.velocity}, magnitud={rb.velocity.magnitude}");
    }

    void FixedUpdate()
    {
        lastVelocity = rb.velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        Vector3 v = Vector3.Reflect(lastVelocity, col.contacts[0].normal);
        rb.velocity = v.normalized * Mathf.Clamp(lastVelocity.magnitude, 8f, maxSpeed);
    }

    public void IncreaseSpeed(int round)
    {
        currentSpeed = Mathf.Clamp(currentSpeed + round * speedIncrementPerScore, 0f, maxSpeed);
        rb.velocity = rb.velocity.normalized * currentSpeed;
    }

    public void ResetSpeed()
    {
        currentSpeed = initialSpeed;
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
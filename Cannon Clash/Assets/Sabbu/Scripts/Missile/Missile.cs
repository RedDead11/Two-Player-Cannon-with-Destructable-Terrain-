using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public static Vector3 CollisionPoint { get; private set; }

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private float torqueMultiplier = 0.43f;     // Adjust the torque multiplier to control rotation speed
    private float maxTorque = 50f;              // Maximum torque to be applied per frame
    private float damping = 4.7f;               // Damping factor to smooth out the rotation
    private float sideDrag = 0.3f;              // Adjust the drag to control perpendicular movement reduction

    public bool hasCollided = false;

    [Header("Effects")]
    public GameObject explosion;

    public AudioSource audio;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Invoke("Die", 7f); 
    }

    private void FixedUpdate()
    {
        RotateTowardsVelocity();
        ApplySideDrag();
    }
    void Die()
    {
        Destroy(gameObject);
    }

    void RotateTowardsVelocity()
    {
        Vector2 velocity = rb.velocity;
        if (velocity != Vector2.zero)
        {
            // Calculate the angle difference
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            float currentAngle = rb.rotation;
            float angleDifference = Mathf.DeltaAngle(currentAngle, angle);

            // Apply torque based on the angle difference, clamping it to the max torque
            float torque = Mathf.Clamp(-angleDifference * torqueMultiplier * Time.fixedDeltaTime, -maxTorque, maxTorque);
            rb.AddTorque(torque);

            // Apply damping to smooth out the rotation
            rb.angularVelocity *= (1 - damping * Time.fixedDeltaTime);
        }
    }

    void ApplySideDrag()
    {
        Vector2 velocity = rb.velocity;
        Vector2 forward = transform.right; // Using transform.right as forward direction in 2D
        float forwardVelocity = Vector2.Dot(velocity, forward);
        Vector2 sideVelocity = velocity - forward * forwardVelocity;

        // Apply custom drag to the side velocity
        sideVelocity *= (1 - sideDrag * Time.fixedDeltaTime);
        rb.velocity = forward * forwardVelocity + sideVelocity;
    }

    private IEnumerator DestroyDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
        audio.Play();

        ContactPoint2D contactPoint = collision.contacts[0];
        GameObject explosionObject = Instantiate(explosion, contactPoint.point, Quaternion.identity);
        boxCollider.enabled = false;
        Destroy(explosionObject, 1f);
        CollisionPoint = contactPoint.point;

        StartCoroutine(DestroyDelay(1f));
    }
}

using DTerrain;
using UnityEngine;

public class MissileCollisionHandler : MonoBehaviour
{
    public static Vector3 CollisionPoint { get; private set; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contactPoint = collision.contacts[0];

        CollisionPoint = contactPoint.point;

        // Optionally, log the collision point
        Debug.Log("Collision point: " + CollisionPoint);
    }
}

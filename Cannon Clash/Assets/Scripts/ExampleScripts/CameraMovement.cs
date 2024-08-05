using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Speed;
    [SerializeField] Vector3 offset;

    private void Start()
    {
        GameObject player1 = GameObject.Find("Player 1");
        offset.z = -1f;

        transform.position = player1.transform.position + offset;
    }
}

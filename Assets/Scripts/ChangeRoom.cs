using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    public Transform targetPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = targetPosition.position;
        }
    }
}

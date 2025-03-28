using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    public float launchForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MovementStateManager move = other.GetComponentInChildren<MovementStateManager>();
            if (move != null)
            {
                move.Launch(Vector3.up * launchForce);
            }
        }
    }
}

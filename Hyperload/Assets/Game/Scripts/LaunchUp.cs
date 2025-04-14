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
                // Set launching flag immediately to block jump
                move.isLaunching = true;

                // Reset velocity before applying launch
                move.ResetVerticalVelocity();
                move.TemporarilyDisableJumpForce(1f);
                // Launch!
                move.Launch(Vector3.up * launchForce);
            }
        }
    }
}

using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damageAmount = 25f;

    private void OnTriggerEnter(Collider other)
    {
        // Check if object has PlayerHealth script
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();

        if (health != null)
        {
            // Apply damage using Photon RPC
            if (health.photonView.IsMine)
            {
                health.photonView.RPC("TakeDamage", Photon.Pun.RpcTarget.All, damageAmount);
            }
        }
    }
}

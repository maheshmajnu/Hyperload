using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 5f;
    [HideInInspector] public WeaponManager weapon;
    [HideInInspector] public Vector3 dir;

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject playerHitEffectPrefab;

    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.gameObject.name);

        ContactPoint contact = collision.contacts[0];

        // Try to find PlayerHealth and PhotonView on parent
        PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
        PhotonView targetView = collision.gameObject.GetComponentInParent<PhotonView>();

        if (playerHealth != null && targetView != null && !targetView.IsMine)
        {
            // Calculate damage based on hit tag
            float finalDamage = GetDamageBasedOnTag(collision.collider.tag);

            // Send RPC to the correct player only
            targetView.RPC("TakeDamage", targetView.Owner, finalDamage);

            // Spawn player hit effect (e.g., blood)
            SpawnPlayerHitEffect(contact.point, contact.normal);
        }
        else
        {
            // Hit environment or non-player
            SpawnHitEffect(contact.point, contact.normal);
        }

        Destroy(this.gameObject); // Destroy bullet
    }

    private void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(effect, 5f);
        }
    }

    private void SpawnPlayerHitEffect(Vector3 position, Vector3 normal)
    {
        if (playerHitEffectPrefab != null)
        {
            GameObject effect = Instantiate(playerHitEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(effect, 1f);
        }
    }

    private float GetDamageBasedOnTag(string tag)
    {
        switch (tag)
        {
            case "Head":
                Debug.Log("Headshot!");
                return weapon.damage * 5f;
            case "Body":
                Debug.Log("Bodyshot!");
                return weapon.damage * 4f;
            case "Hand":
                Debug.Log("Handshot!");
                return weapon.damage * 3f;
            case "Leg":
                Debug.Log("Legshot!");
                return weapon.damage * 2f;
            default:
                return weapon.damage;
        }
    }
}

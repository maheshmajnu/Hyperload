using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    [HideInInspector] public WeaponManager weapon;
    [HideInInspector] public Vector3 dir;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject playerHitEffectPrefab;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }



    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.gameObject.name);

        PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
        ContactPoint contact = collision.contacts[0];

        if (playerHealth != null)
        {
            // Apply damage
            float finalDamage = GetDamageBasedOnTag(collision.collider.tag);
            playerHealth.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, finalDamage);

            // Spawn player hit effect (like blood)
            SpawnPlayerHitEffect(contact.point, contact.normal);
        }
        else
        {
            // Spawn regular hit effect (dust/sparks)
            SpawnHitEffect(contact.point, contact.normal);
        }

        Destroy(this.gameObject); // Bullet gone
    }


    private void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(effect, 5f); // destroy after 5 seconds
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
                return weapon.damage * 2.0f; // Headshot = double damage
            case "Body":
                Debug.Log("Bodyshot!");
                return weapon.damage * 1.0f; // Normal damage
            case "Hand":
                Debug.Log("Handshot!");
                return weapon.damage * 0.7f; // Slightly less
            case "Leg":
                Debug.Log("Legshot!");
                return weapon.damage * 0.5f; // Least damage
            default:
                return weapon.damage; // fallback
        }
    }

}

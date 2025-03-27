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


    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }



    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.gameObject.name);

        // Check if it hit a player
        PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            float finalDamage = GetDamageBasedOnTag(collision.collider.tag);
            playerHealth.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, finalDamage);
        }

        else
        {
            // Not a player = spawn hit particle
            SpawnHitEffect(collision.contacts[0].point, collision.contacts[0].normal);
        }

        Destroy(this.gameObject);
    }

    private void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(effect, 5f); // destroy after 5 seconds
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

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
        ContactPoint contact = collision.contacts[0];
        PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
        PhotonView targetView = collision.gameObject.GetComponentInParent<PhotonView>();

        if (playerHealth != null && targetView != null && !targetView.IsMine)
        {
            GameLogic shooterLogic = weapon.GetComponentInParent<GameLogic>();
            GameLogic targetLogic = targetView.GetComponentInParent<GameLogic>();

            if (shooterLogic == null || targetLogic == null)
            {
                Destroy(gameObject);
                return;
            }

            string shooterLayer = shooterLogic.StandOn;
            string targetLayer = targetLogic.StandOn;

            if (CanDamage(shooterLayer, targetLayer))
            {
                float finalDamage = GetDamageBasedOnTag(collision.collider.tag);
                // Get shooter stats
                int shooterID = weapon.GetComponentInParent<PhotonView>().OwnerActorNr;
                var shooterStats = PlayerStatsManager.Get(shooterID);
                shooterStats.damageDone += (int)finalDamage;

                // Get target health before damage
                float targetCurrentHealth = playerHealth.GetCurrentHealth();

                if (finalDamage >= targetCurrentHealth)
                {
                    shooterStats.kills++;
                }

                // Apply damage via RPC
                targetView.RPC("TakeDamage", targetView.Owner, finalDamage);

                SpawnPlayerHitEffect(contact.point, contact.normal);
            }
            else
            {
                Debug.Log(" Damage blocked due to layer mismatch.");
            }
        }
        else
        {
            SpawnHitEffect(contact.point, contact.normal);
        }

        Destroy(this.gameObject);
    }

    private bool CanDamage(string shooter, string target)
    {
        if ((shooter == "Ground" || shooter == "Default") && (target == "Ground" || target == "Default"))
            return true;

        if ((shooter == "Ground" || shooter == "Default") && IsColored(target))
            return false;

        if (IsColored(shooter) && (target == "Ground" || target == "Default"))
            return true;

        if (IsColored(shooter) && IsColored(target))
            return shooter == target;

        return false;
    }

    private bool IsColored(string layer)
    {
        return layer == "Red" || layer == "Green" || layer == "Blue" || layer == "Yellow";
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

using System.Collections;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public Transform groundCheckPoint;
    public float checkRadius = 0.3f;
    public LayerMask layerMaskToCheck;

    public string StandOn = "";

    private float groundTimer = 0f;
    private float damageInterval = 5f;

    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(groundCheckPoint.position, checkRadius, layerMaskToCheck);

        if (hits.Length > 0)
        {
            StandOn = LayerMask.LayerToName(hits[0].gameObject.layer);

            if (StandOn == "Ground" || StandOn == "Default")
            {
                groundTimer += Time.deltaTime;

                if (groundTimer >= damageInterval)
                {
                    playerHealth.TakeDamage(25f);
                    groundTimer = 0f; // reset timer after applying damage
                }
            }
            else
            {
                groundTimer = 0f; // reset timer if on safe layer
            }
        }
        else
        {
            StandOn = "None";
            groundTimer = 0f; // reset if standing on nothing
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPoint.position, checkRadius);
        }
    }
}

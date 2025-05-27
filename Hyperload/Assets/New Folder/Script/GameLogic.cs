using System.Collections;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public Transform groundCheckPoint; // empty GameObject at player's feet
    public float checkRadius = 0.3f;
    public LayerMask layerMaskToCheck; // combine all relevant layers here in Inspector

    public string StandOn = "";

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        StartCoroutine(CheckAndDamageRoutine());
    }

    private IEnumerator CheckAndDamageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            Collider[] hits = Physics.OverlapSphere(groundCheckPoint.position, checkRadius, layerMaskToCheck);

            if (hits.Length > 0)
            {
                StandOn = LayerMask.LayerToName(hits[0].gameObject.layer);

                if (StandOn == "Ground" || StandOn == "Default")
                {
                    playerHealth.TakeDamage(25f);
                    
                }
                else
                {
                    Debug.Log("Standing on: " + StandOn + " - Safe Zone");
                }
            }
            else
            {
                StandOn = "None";
                Debug.Log("Not standing on any recognized layer.");
            }
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

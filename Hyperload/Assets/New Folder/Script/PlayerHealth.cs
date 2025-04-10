using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI References")]
    public Image HealthBG;        // Screen Space (local player)
    public Image Health;          // Fill bar for local
    public Image WorldHealthBG;   // World Space (for others)
    public Image world_Health;    // Fill bar for others

    private void Start()
    {
        currentHealth = maxHealth;
        SetupUI();
        UpdateUI();
    }

    private void SetupUI()
    {
        if (photonView.IsMine)
        {
            // For local player
            if (HealthBG != null) HealthBG.gameObject.SetActive(true);
            if (WorldHealthBG != null) WorldHealthBG.gameObject.SetActive(false);
        }
        else
        {
            // For other players
            if (HealthBG != null) HealthBG.gameObject.SetActive(false);
            if (WorldHealthBG != null) WorldHealthBG.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        float fill = currentHealth / maxHealth;

        if (photonView.IsMine && Health != null)
            Health.fillAmount = fill;

        if (world_Health != null)
            world_Health.fillAmount = fill;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        // You can add respawn logic or ragdoll trigger here
    }
}

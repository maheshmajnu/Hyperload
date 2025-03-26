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
    public Image HealthBG;       // Screen Space Overlay (Local Player)
    public Image Health;         // Health bar inside Screen Space UI
    public Image WorldHealthBG; // World Space (For Other Players)
    public Image world_Health;   // Health bar inside World Space UI

    private Coroutine hideHealthCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;
        SetupUI();
        UpdateHealthUI();
    }

    private void SetupUI()
    {
        if (photonView.IsMine)
        {
            HealthBG.gameObject.SetActive(true);      // Local player sees Screen Space UI
            WorldHealthBG.gameObject.SetActive(false); // Hide world space UI for self
        }
        else
        {
            HealthBG.gameObject.SetActive(false);     // Other players shouldn't see our screen UI
            WorldHealthBG.gameObject.SetActive(false); // Show world space UI to others
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine) return; // Only the damaged player should update their own health

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        // Notify other players to show the world space health bar
        photonView.RPC("ShowWorldHealthUI", RpcTarget.Others);

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (photonView.IsMine)
        {
            Health.fillAmount = currentHealth / maxHealth; // Local Player UI
        }

        world_Health.fillAmount = currentHealth / maxHealth; // World Space UI
    }

    [PunRPC]
    private void ShowWorldHealthUI()
    {
        WorldHealthBG.gameObject.SetActive(true);

        // Stop any existing hide coroutine
        if (hideHealthCoroutine != null)
        {
            StopCoroutine(hideHealthCoroutine);
        }

        // Hide world health UI after 0.5 seconds
        hideHealthCoroutine = StartCoroutine(HideWorldHealthUI());
    }

    private IEnumerator HideWorldHealthUI()
    {
        yield return new WaitForSeconds(1f);
        WorldHealthBG.gameObject.SetActive(false);
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");

        // Disable player components
        if (photonView.IsMine)
        {
            GetComponent<PlayerController>().enabled = false;
            this.enabled = false;
        }

        // Optionally: Destroy or respawn the player (Handle in GameManager)
    }
}

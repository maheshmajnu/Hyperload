using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI References")]
    public Image HealthBG;         // Screen Space (local player)
    public Image Health;           // Fill bar for local
    public Image WorldHealthBG;    // World Space (for others)
    public Image world_Health;     // Fill bar for others
    public TMP_Text playerLives;

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
            if (HealthBG != null) HealthBG.gameObject.SetActive(true);
            if (WorldHealthBG != null) WorldHealthBG.gameObject.SetActive(false);
        }
        else
        {
            if (HealthBG != null) HealthBG.gameObject.SetActive(false);
            if (WorldHealthBG != null) WorldHealthBG.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        Debug.Log($"[RPC] {photonView.Owner.NickName} took {damage} damage.");
        if (!photonView.IsMine) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        // Sync health bar to others (world health)
        photonView.RPC("SyncWorldHealth", RpcTarget.Others, currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [PunRPC]
    public void SyncWorldHealth(float healthPercent)
    {
        if (!photonView.IsMine && world_Health != null)
        {
            world_Health.fillAmount = healthPercent;
        }
    }

    private void UpdateUI()
    {
        float fill = currentHealth / maxHealth;

        if (photonView.IsMine && Health != null)
            Health.fillAmount = fill;

        if (world_Health != null && !photonView.IsMine)
            world_Health.fillAmount = fill;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void SetLivesUI(int lives)
    {
        if (photonView.IsMine && playerLives != null)
        {
            playerLives.text = "Lives: " + lives.ToString();
        }
    }


    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        GameManager.Instance.TriggerRagdollEffect(gameObject);
        GameManager.Instance.HandlePlayerDeath(photonView.Owner);
        StartCoroutine(DestroyPlayerAfterDelay(5f));
    }

    private IEnumerator DestroyPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(transform.root.gameObject); // Destroy full prefab
    }


}

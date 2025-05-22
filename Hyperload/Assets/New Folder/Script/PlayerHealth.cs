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
    private bool isDead = false;

    [Header("UI References")]
    public Image HealthBG;
    public Image Health;
    public Image WorldHealthBG;
    public Image world_Health;
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
        if (!photonView.IsMine || isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();
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

        if (!photonView.IsMine && world_Health != null)
            world_Health.fillAmount = fill;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        UpdateUI();
    }

    public void SetLivesUI(int lives)
    {
        if (playerLives != null)
        {
            playerLives.text = lives.ToString();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{photonView.Owner.NickName} has died!");

        // Trigger ragdoll
        RagdollManager rag = GetComponent<RagdollManager>();
        if (rag != null)
            rag.TriggerRagdoll();

        if (photonView.IsMine)
        {
            GameManager.Instance.HandlePlayerDeath(PhotonNetwork.LocalPlayer);
        }

        StartCoroutine(DestroyPlayerAfterDelay(1f));
    }

    private IEnumerator DestroyPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(photonView.transform.root.gameObject);
        }
    }

}

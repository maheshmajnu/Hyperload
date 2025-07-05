using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [Header("Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("UI References")]
    public Image HealthBG;
    public Image Health;
    public Image WorldHealthBG;
    public Image world_Health;
    

    private void Start()
    {
        currentHealth = maxHealth;
        SetupUI();
        UpdateUI();
        
    }

    private void SetupUI()
    {
        bool mine = photonView.IsMine;
        if (HealthBG) HealthBG.gameObject.SetActive(mine);
        if (WorldHealthBG) WorldHealthBG.gameObject.SetActive(!mine);
    }

    private void UpdateUI()
    {
        float fill = currentHealth / maxHealth;

        if (photonView.IsMine && Health != null)
            Health.fillAmount = fill;

        if (!photonView.IsMine && world_Health != null)
            world_Health.fillAmount = fill;
    }

    

    [PunRPC]
    public void TakeDamage(float dmg)
    {
        if (!photonView.IsMine || isDead) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        // Ensure we’re connected and in room before sending RPC
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            photonView.RPC("SyncWorldHealth", RpcTarget.Others, currentHealth / maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [PunRPC]
    public void SyncWorldHealth(float fill)
    {
        if (!photonView.IsMine && world_Health != null)
        {
            world_Health.fillAmount = fill;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{photonView.Owner.NickName} died.");

        RagdollManager rag = GetComponent<RagdollManager>();
        if (rag) rag.TriggerRagdoll();

        var stats = PlayerStatsManager.Get(photonView.OwnerActorNr);
        stats.deaths++;


        StartCoroutine(DestroyPlayerAfterDelay(1f));
    }

    private IEnumerator DestroyPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (photonView.IsMine)
        {
            GameManager.Instance.HandleRespawn();
            PhotonNetwork.Destroy(photonView.transform.root.gameObject);
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}

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
    public TMP_Text playerLivesText;

    private void Start()
    {
        currentHealth = maxHealth;
        SetupUI();
        UpdateUI();
        UpdateLivesUI();
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

    private void UpdateLivesUI()
    {
        if (photonView.IsMine && playerLivesText != null)
        {
            int lives = GameManager.Instance.playerLives[PhotonNetwork.LocalPlayer];
            playerLivesText.text = $"{lives}";
        }
    }

    [PunRPC]
    public void TakeDamage(float dmg)
    {
        if (!photonView.IsMine || isDead) return;

        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        photonView.RPC("SyncWorldHealth", RpcTarget.Others, currentHealth / maxHealth);

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

        UpdateLivesUI();

        StartCoroutine(DestroyPlayerAfterDelay(2f));
    }

    private IEnumerator DestroyPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (photonView.IsMine)
        {
            GameManager.Instance.HandlePlayerDeath(photonView.transform.root.gameObject);
            PhotonNetwork.Destroy(photonView.transform.root.gameObject);
        }
    }
}

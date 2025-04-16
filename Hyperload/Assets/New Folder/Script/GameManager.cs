using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    public static GameManager Instance;
    public GameObject winnerPanel;
    public TMP_Text winnerText;

    private Dictionary<Player, int> playerLives = new Dictionary<Player, int>();
    private Dictionary<Player, GameObject> playerInstances = new Dictionary<Player, GameObject>();
    private Dictionary<Player, bool> isRespawning = new Dictionary<Player, bool>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (winnerPanel != null) winnerPanel.SetActive(false);
        if (winnerText != null) winnerText.text = "";

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Debug.Log("Spawning Player for: " + PhotonNetwork.LocalPlayer.NickName);
        int randomNumber = Random.Range(-10, 10);
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomNumber, 25 , randomNumber), Quaternion.identity);

        // Track lives
        if (!playerLives.ContainsKey(PhotonNetwork.LocalPlayer))
        {
            playerLives[PhotonNetwork.LocalPlayer] = 3;
        }

        // Track instance
        playerInstances[PhotonNetwork.LocalPlayer] = playerObj;

        // Reset health
        PlayerHealth health = playerObj.GetComponentInChildren<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealth();
            health.SetLivesUI(playerLives[PhotonNetwork.LocalPlayer]);
        }

        // Reset respawn lock
        isRespawning[PhotonNetwork.LocalPlayer] = false;
    }

    public void HandlePlayerDeath(Player deadPlayer)
    {
        if (!playerLives.ContainsKey(deadPlayer)) return;

        playerLives[deadPlayer]--;

        Debug.Log($"{deadPlayer.NickName} died. Remaining lives: {playerLives[deadPlayer]}");

        if (playerLives[deadPlayer] > 0)
        {
            if (deadPlayer == PhotonNetwork.LocalPlayer)
            {
                if (isRespawning.ContainsKey(deadPlayer) && isRespawning[deadPlayer])
                {
                    Debug.LogWarning("Already respawning this player: " + deadPlayer.NickName);
                    return;
                }

                isRespawning[deadPlayer] = true;
                StartCoroutine(RespawnAfterDelay(deadPlayer, 20f));
            }
        }
        else
        {
            if (deadPlayer == PhotonNetwork.LocalPlayer)
            {
                Debug.Log("You are out of lives!");
            }

            CheckWinner();
        }
    }


    private IEnumerator RespawnAfterDelay(Player player, float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Spawning Player for: " + PhotonNetwork.LocalPlayer.NickName);
        SpawnPlayer();
    }

    public void TriggerRagdollEffect(GameObject playerObject)
    {
        RagdollManager ragdoll = playerObject.GetComponentInChildren<RagdollManager>();
        if (ragdoll != null)
        {
            ragdoll.TriggerRagdoll();
        }
    }

    private void CheckWinner()
    {
        int aliveCount = 0;
        Player lastAlive = null;

        foreach (var kvp in playerLives)
        {
            if (kvp.Value > 0)
            {
                aliveCount++;
                lastAlive = kvp.Key;
            }
        }

        if (aliveCount == 1)
        {
            Debug.Log($" Winner is: {lastAlive.NickName}");
            if (winnerPanel != null) winnerPanel.SetActive(true);
            if (winnerText != null) winnerText.text = $" Winner: {lastAlive.NickName}";
        }
        else if (aliveCount == 0)
        {
            Debug.Log(" No one survived!");
            if (winnerPanel != null) winnerPanel.SetActive(true);
            if (winnerText != null) winnerText.text = " No one survived!";
        }
    }


}

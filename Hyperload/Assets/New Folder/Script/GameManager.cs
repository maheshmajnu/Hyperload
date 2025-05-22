using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [Header("References")]
    public GameObject playerPrefab;
    public GameObject winnerPanel;
    public TMP_Text winnerText;

    [Header("Tracking")]
    [SerializeField] public List<GameObject> alivePlayers = new List<GameObject>();

    private Dictionary<Player, int> playerLives = new Dictionary<Player, int>();
    private Dictionary<Player, bool> isRespawning = new Dictionary<Player, bool>();

    private PhotonView photonView;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (winnerPanel) winnerPanel.SetActive(false);
        if (winnerText) winnerText.text = "";

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        int rand = Random.Range(-10, 10);
        GameObject playerRoot = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(rand, 25, rand), Quaternion.identity);

        StartCoroutine(TrackPlayerAfterSpawn(playerRoot));
    }

    private IEnumerator TrackPlayerAfterSpawn(GameObject root)
    {
        PhotonView foundView = null;
        PlayerHealth foundHealth = null;

        float timer = 0f;
        while (timer < 10f)
        {
            foundView = root.GetComponentInChildren<PhotonView>();
            foundHealth = root.GetComponentInChildren<PlayerHealth>();

            if (foundView != null && foundHealth != null)
            {
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (foundView == null || foundHealth == null)
        {
            Debug.LogError("Failed to find PhotonView or PlayerHealth within 10 seconds.");
            yield break;
        }

        Player owner = foundView.Owner;

        if (!playerLives.ContainsKey(owner))
            playerLives[owner] = 3;

        foundHealth.ResetHealth();
        foundHealth.SetLivesUI(playerLives[owner]);

        isRespawning[owner] = false;

        if (!alivePlayers.Contains(root))
            alivePlayers.Add(root);
    }

    public void HandlePlayerDeath(Player player)
    {
        if (!playerLives.ContainsKey(player))
            return;

        playerLives[player]--;

        if (playerLives[player] > 0)
        {
            if (isRespawning.ContainsKey(player) && isRespawning[player])
                return;

            isRespawning[player] = true;
            StartCoroutine(RespawnPlayerAfterDelay(player, 5f));
        }
        else
        {
            RemoveFromAlive(player);
            StartCoroutine(CheckWinnerAfterDelay(10f));
        }
    }

    private void RemoveFromAlive(Player player)
    {
        GameObject objToRemove = null;

        foreach (var obj in alivePlayers)
        {
            PhotonView pv = obj.GetComponentInChildren<PhotonView>();
            if (pv != null && pv.Owner == player)
            {
                objToRemove = obj;
                break;
            }
        }

        if (objToRemove != null)
            alivePlayers.Remove(objToRemove);
    }

    private IEnumerator RespawnPlayerAfterDelay(Player player, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (PhotonNetwork.LocalPlayer == player)
        {
            SpawnPlayer();
        }
    }

    private IEnumerator CheckWinnerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        alivePlayers.RemoveAll(p => p == null); // clean nulls

        if (alivePlayers.Count == 1)
        {
            PhotonView pv = alivePlayers[0].GetComponentInChildren<PhotonView>();
            if (pv != null)
            {
                photonView.RPC("ShowWinner", RpcTarget.All, pv.Owner.NickName);
            }
        }
        else if (alivePlayers.Count == 0)
        {
            photonView.RPC("ShowWinner", RpcTarget.All, "No one survived!");
        }
    }

    [PunRPC]
    public void ShowWinner(string winnerName)
    {
        if (winnerPanel) winnerPanel.SetActive(true);
        if (winnerText) winnerText.text = "Winner: " + winnerName;
    }
}

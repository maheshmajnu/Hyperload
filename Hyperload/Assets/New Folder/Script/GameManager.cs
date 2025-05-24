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
    public int startingLives = 3;
    public float spawnDelay = 10f;
    public GameObject winnerPanel;
    public TMP_Text winnerText;
    public TMP_Text countdownText;

    [Header("Tracking")]
    public Dictionary<Player, int> playerLives = new Dictionary<Player, int>();
    public List<GameObject> alivePlayers = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (winnerPanel) winnerPanel.SetActive(false);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), 25f, Random.Range(-10f, 10f));
        GameObject playerRoot = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        PhotonView view = playerRoot.GetComponentInChildren<PhotonView>();
        if (view != null && view.IsMine)
        {
            Transform followTarget = playerRoot.transform.Find("Player/CameraFollowPos/RecoilFollowPos");
            if (followTarget != null && CameraController.Instance != null)
            {
                CameraController.Instance.SetCameraTarget(followTarget);
            }
        }

        // Track lives
        if (!playerLives.ContainsKey(PhotonNetwork.LocalPlayer))
            playerLives[PhotonNetwork.LocalPlayer] = startingLives;

        // Track alive player
        alivePlayers.Add(playerRoot);
    }

    public void HandlePlayerDeath(GameObject playerObj)
    {
        if (playerObj == null) return;

        alivePlayers.Remove(playerObj);

        PhotonView view = playerObj.GetComponentInChildren<PhotonView>();
        if (view == null) return;  // Safety check

        Player owner = view.Owner;

        if (playerLives.ContainsKey(owner))
        {
            playerLives[owner]--;

            if (playerLives[owner] > 0)
            {
                if (PhotonNetwork.LocalPlayer == owner)
                    StartCoroutine(RespawnCountdown(owner));
            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                    CheckForWinner();
            }
        }
    }


    private IEnumerator RespawnCountdown(Player owner)
    {
        float timeLeft = spawnDelay;
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        while (timeLeft > 0)
        {
            if (countdownText != null)
                countdownText.text = $"{Mathf.CeilToInt(timeLeft)}";
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (PhotonNetwork.LocalPlayer == owner)
        {
            SpawnPlayer();
        }
    }

    private void CheckForWinner()
    {
        if (alivePlayers.Count == 1)
        {
            PhotonView aliveView = alivePlayers[0].GetComponentInChildren<PhotonView>();
            if (aliveView != null)
            {
                string winnerName = aliveView.Owner.NickName;
                photonView.RPC("ShowWinner", RpcTarget.All, winnerName);
            }
        }
    }

    [PunRPC]
    private void ShowWinner(string winnerName)
    {
        Debug.Log($"Winner: {winnerName}");
        if (winnerPanel) winnerPanel.SetActive(true);
        if (winnerText) winnerText.text = $"Winner: {winnerName}";
        
    }
}

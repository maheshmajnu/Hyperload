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
    public float Countdown = 120f;  // 2 minutes
    public float spawnDelay = 10f;

    public GameObject winnerPanel;
    public TMP_Text winnerText;
    public TMP_Text countdownText;
    public TMP_Text spawncountdownText;
    public GameObject PlayersPanel; // reference to the panel holding all player rows
    public GameObject playerContentPrefab; // reference to the prefab with TMP fields

    private float countdownTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Clear old stats from previous game session
        PlayerStatsManager.Clear();

        if (winnerPanel) winnerPanel.SetActive(false);
        countdownTime = Countdown;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(GameCountdown());
            }
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
    }

    public void HandleRespawn()
    {
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        if (spawncountdownText != null)
        {
            spawncountdownText.gameObject.SetActive(true);

            float timer = spawnDelay;
            while (timer > 0)
            {
                spawncountdownText.text = $"{Mathf.CeilToInt(timer)}";
                yield return new WaitForSeconds(1f);
                timer -= 1f;
            }

            spawncountdownText.gameObject.SetActive(false);
        }

        // After countdown, spawn player
        SpawnPlayer();
    }


    private IEnumerator GameCountdown()
    {
        while (countdownTime > 0)
        {
            photonView.RPC("UpdateCountdownUI", RpcTarget.All, countdownTime);
            yield return new WaitForSeconds(1f);
            countdownTime -= 1f;
        }

        photonView.RPC("UpdateCountdownUI", RpcTarget.All, 0f);
        photonView.RPC("ShowWinnerPanel", RpcTarget.All, "Time's up!");
    }

    [PunRPC]
    private void UpdateCountdownUI(float timeLeft)
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        countdownText.text = $"{minutes:00}:{seconds:00}";
    }


    [PunRPC]
    private void ShowWinnerPanel(string winnerMessage)
    {
        winnerPanel.SetActive(true);
        winnerText.text = $"Winner: {winnerMessage}";

        //Clear existing player rows
        foreach (Transform child in PlayersPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Spawn player stat rows
        foreach (var kvp in PlayerStatsManager.allStats)
        {
            int actorNumber = kvp.Key;
            var stats = kvp.Value;

            Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (player != null)
            {

                GameObject row = Instantiate(playerContentPrefab, PlayersPanel.transform);
                row.transform.GetChild(0).GetComponent<TMP_Text>().text = player.NickName;
                row.transform.GetChild(1).GetComponent<TMP_Text>().text = stats.kills.ToString();
                row.transform.GetChild(2).GetComponent<TMP_Text>().text = stats.damageDone.ToString();
                row.transform.GetChild(3).GetComponent<TMP_Text>().text = stats.deaths.ToString();
            }
        }
    }


}

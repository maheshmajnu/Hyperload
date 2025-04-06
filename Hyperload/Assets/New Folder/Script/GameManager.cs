using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public static List<PlayerSetup> players = new List<PlayerSetup>();

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int randomNumber = Random.Range(-10, 10);
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomNumber, 0, randomNumber), Quaternion.identity);

            // Step 1: Get "Player" child (index 0)
            Transform player = playerObj.transform.GetChild(0); // "Player"

            // Step 2: Get PlayerSetup
            PlayerSetup setup = player.GetComponent<PlayerSetup>();
            if (setup != null)
            {
                players.Add(setup);

                // Step 3: Find _Player/Ch15
                Transform ch15 = player.Find("_Player/Ch15");
                if (ch15 != null)
                {
                    SkinnedMeshRenderer smr = ch15.GetComponent<SkinnedMeshRenderer>();
                    if (smr != null && smr.materials.Length >= 3)
                    {
                        // Step 4: Clone the shared material so it's unique per player
                        Material sharedMat = smr.materials[0]; // all 3 are the same
                        Material playerMatInstance = new Material(sharedMat);

                        // Step 5: Assign it to all 3 material slots
                        Material[] newMats = smr.materials;
                        newMats[0] = playerMatInstance;
                        newMats[1] = playerMatInstance;
                        newMats[2] = playerMatInstance;
                        smr.materials = newMats;

                        // Step 6: Assign to OutlineManager
                        OutlineManager om = player.GetComponent<OutlineManager>();
                        if (om != null)
                        {
                            om.myPlayer = setup;
                            om.outlineMat = playerMatInstance;
                        }
                    }
                }

                // Step 7: Assign other players for outline comparison (optional but safe)
                StartCoroutine(AssignMyPlayerRefs());
            }
        }
    }

    private IEnumerator AssignMyPlayerRefs()
    {
        yield return new WaitForSeconds(1f); // wait for all players to spawn

        foreach (var player in players)
        {
            OutlineManager om = player.GetComponent<OutlineManager>();
            if (om != null && om.myPlayer == null)
            {
                om.myPlayer = player;
            }
        }
    }
}

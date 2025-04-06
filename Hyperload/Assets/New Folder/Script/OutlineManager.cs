using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    [Header("Outline Settings")]
    public Material outlineMat;

    [Header("Player Reference")]
    public PlayerSetup myPlayer;

    private string currentPlatformLayer = "Ground";
    private string lastPlatformLayer = "";

    private void Start()
    {
        if (myPlayer == null)
            myPlayer = GetComponent<PlayerSetup>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        string layerName = LayerMask.LayerToName(hit.gameObject.layer);

        if (layerName == "Default")
            currentPlatformLayer = "Ground";
        else
            currentPlatformLayer = layerName;

        if (currentPlatformLayer != lastPlatformLayer)
        {
            Debug.Log($"{gameObject.name} is now on: {currentPlatformLayer}");
            lastPlatformLayer = currentPlatformLayer;
        }
    }

    private void Update()
    {
        if (outlineMat == null || GameManager.players.Count <= 1) return;

        int sameCount = 0;
        int groundCount = 0;
        int differentCount = 0;

        foreach (var player in GameManager.players)
        {
            if (player == myPlayer) continue;

            OutlineManager otherOM = player.GetComponent<OutlineManager>();
            if (otherOM == null) continue;

            string otherLayer = otherOM.GetCurrentPlatform();

            if (currentPlatformLayer == "Ground" && otherLayer == "Ground")
                groundCount++;
            else if (currentPlatformLayer == "Ground")
                differentCount++;
            else if (currentPlatformLayer == otherLayer)
                sameCount++;
            else
                differentCount++;
        }

        // Determine final color
        Color resultColor = Color.blue;

        if (currentPlatformLayer == "Ground" && groundCount == GameManager.players.Count - 1)
        {
            resultColor = Color.blue;
        }
        else if (currentPlatformLayer == "Ground")
        {
            resultColor = Color.blue;
        }
        else if (sameCount > 0)
        {
            resultColor = Color.red;
        }
        else if (differentCount > 0)
        {
            resultColor = Color.green;
        }

        // Boost color for HDR output (so it's visible!)
        Color boostedColor = resultColor * 5f;

        // Try setting both variations (just in case)
        outlineMat.SetColor("_Outline_color", boostedColor);  // Shader Graph exposed property
        outlineMat.SetColor("_OutlineColor", boostedColor);   // Unity fallback

        Debug.Log($"{gameObject.name} outline color set to: {boostedColor}");
    }

    public string GetCurrentPlatform()
    {
        return currentPlatformLayer;
    }
}

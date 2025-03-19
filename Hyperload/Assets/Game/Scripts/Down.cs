using UnityEngine;
using System.Collections;

public class BlockToggleMove : MonoBehaviour
{
    public float dropHeight = -1f; // Target Y position when player is on
    private float originalHeight;  // Initial Y position
    public float moveSpeed = 5f;   // Speed of movement
    private bool isPlayerOnBlock = false;
    private Coroutine moveCoroutine;

    private Renderer blockRenderer;
    public Color glowColor = Color.cyan; // Color of the glow
    public float glowIntensity = 5f;     // Intensity of the glow effect (higher for bloom)
    private Color originalColor;

    void Start()
    {
        originalHeight = transform.position.y;
        blockRenderer = GetComponent<Renderer>();

        // Save the original emission color (without glow)
        if (blockRenderer.material.HasProperty("_EmissionColor"))
        {
            originalColor = blockRenderer.material.GetColor("_EmissionColor");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerOnBlock)
        {
            isPlayerOnBlock = true;
            StartMovement(dropHeight);
            EnableGlow(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerOnBlock)
        {
            isPlayerOnBlock = false;
            StartMovement(originalHeight);
            EnableGlow(false);
        }
    }

    private void StartMovement(float targetY)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveBlock(targetY));
    }

    private IEnumerator MoveBlock(float targetY)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, targetY, transform.position.z);

        float elapsedTime = 0f;
        while (elapsedTime < 0.3f) // Smooth movement over 0.3 seconds
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / 0.3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // Ensure final position
    }

    private void EnableGlow(bool enable)
    {
        if (blockRenderer.material.HasProperty("_EmissionColor"))
        {
            if (enable)
            {
                // Multiply color by high intensity to trigger bloom
                Color emissionColor = glowColor * Mathf.LinearToGammaSpace(glowIntensity);
                blockRenderer.material.SetColor("_EmissionColor", emissionColor);
                blockRenderer.material.EnableKeyword("_EMISSION");
            }
            else
            {
                blockRenderer.material.SetColor("_EmissionColor", originalColor);
                blockRenderer.material.DisableKeyword("_EMISSION");
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public RectTransform crossUp, crossDown, crossLeft, crossRight;
    public WeaponBloom bloomScript; // Assign your weapon bloom script in inspector
    public float bloomMultiplier = 5f;

    void Update()
    {
        float bloomAmount = bloomScript.GetCurrentBloom(); // We'll add this function below

        crossUp.anchoredPosition = new Vector2(0, bloomAmount * bloomMultiplier);
        crossDown.anchoredPosition = new Vector2(0, -bloomAmount * bloomMultiplier);
        crossLeft.anchoredPosition = new Vector2(-bloomAmount * bloomMultiplier, 0);
        crossRight.anchoredPosition = new Vector2(bloomAmount * bloomMultiplier, 0);
    }
}

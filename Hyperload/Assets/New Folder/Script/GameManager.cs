using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviour
{
    

    public static GameManager instace;

    public bool mobileControl;

    private GameObject mobileCanvas;
    public TextMeshProUGUI display_Text;

    private void Awake()
    {
        if (instace == null)
        {
            instace = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        mobileCanvas = GameObject.Find("MobileInput");
        if (!mobileControl) Cursor.lockState = CursorLockMode.Locked;
        mobileCanvas.SetActive(mobileControl);
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        var avgFrameRate = (int)current;
        if (display_Text != null)
            display_Text.text = avgFrameRate.ToString() + " FPS";
    }
}

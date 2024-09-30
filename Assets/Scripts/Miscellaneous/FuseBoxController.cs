using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class FuseBoxController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private List<XRSlider> m_sliders;
    [SerializeField] private XRLever m_activationLever;
    [SerializeField] private GameObject m_pushButton;
    [SerializeField] private float[] slidersOrder;
    [SerializeField] private BlinkingMaterial m_greenLight;
    [SerializeField] private BlinkingMaterial m_redLight;

    // Non-Serialized Variables
    private bool slidersReady = false;
    private bool leverReady = false;
    private bool buttonReady = false;

    private void Start()
    {
        m_pushButton.GetComponent<XRPushButton>().enabled = false;
        m_redLight.StartBlinking();
    }

    void Update()
    {
        if (!slidersReady) { CheckAllSwitches(); }
        if (leverReady && !buttonReady) { CheckLever(); }
    }

    private void CheckAllSwitches()
    {
        int righSliders = 0;
        foreach (var slider in m_sliders)
        {
            if (CheckSwitchValue(slider.gameObject.GetComponent<SliderValueSync>(), m_sliders.IndexOf(slider)))
            {
                righSliders++;
            }
        }

        if (righSliders == m_sliders.Count)
        {
            foreach (var slider in m_sliders)
            {
                slider.enabled = false;
            }
            
            leverReady = true;
            slidersReady = true;
            m_redLight.StopBlinking();
            m_greenLight.StartBlinking();
            Debug.Log("LEVER ACTIVATED");
        }
    }

    private void CheckLever()
    {
        if (m_activationLever.gameObject.GetComponent<LeverValueSync>().GetSyncedValue())
        {
            m_pushButton.GetComponent<XRPushButton>().enabled = true;
            m_pushButton.transform.GetChild(0).GetComponent<BlinkingMaterial>().StartBlinking();
            m_activationLever.enabled = false;
            buttonReady = true;
            m_greenLight.StopBlinking();
            Debug.Log("BUTTON ACTIVATED");
        }
    }

    private bool CheckSwitchValue(SliderValueSync sliderSync, int index)
    {
        float tolerance = 0.01f; // Adjust as needed
        return Mathf.Abs(sliderSync.syncedValue - slidersOrder[index]) < tolerance;
    }
}

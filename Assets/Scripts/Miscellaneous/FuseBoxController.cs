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
    [SerializeField] private int[] slidersOrder;

    // Non-Serialized Variables
    private bool slidersReady = false;
    private bool leverReady = false;
    private bool buttonReady = false;

    private void Start()
    {
        m_pushButton.GetComponent<XRPushButton>().enabled = false;
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
            if (CheckSwitchValue(slider, m_sliders.IndexOf(slider)))
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
            Debug.Log("LEVER ACTIVATED");
        }
    }

    private void CheckLever()
    {
        if (m_activationLever.value)
        {
            m_pushButton.GetComponent<XRPushButton>().enabled = true;
            m_pushButton.transform.GetChild(0).GetComponent<BlinkingMaterial>().StartBlinking();
            m_activationLever.enabled = false;
            buttonReady = true;
            Debug.Log("BUTTON ACTIVATED");
        }
    }

    private bool CheckSwitchValue(XRSlider slider, int index)
    {
        return slider.value == slidersOrder[index] ? true : false;
    }
}

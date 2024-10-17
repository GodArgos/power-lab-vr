using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SpecialTransitionLogic : MonoBehaviour
{
    [SerializeField] private DoorActivationSystem _doorActivationSystem;
    [SerializeField] private List<GameObject> m_postParticles;
    [SerializeField] private GameObject m_explosionPrefab;

    private bool startedTrans = false;
    public XROrigin XROrigin;
    private VignetteController vignette;

    private void Start()
    {
        XROrigin = FindObjectOfType<XROrigin>();
        Debug.Log(XROrigin.transform.GetChild(0).GetChild(0).name);
        vignette = XROrigin.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<VignetteController>();

        foreach (GameObject go in m_postParticles)
        {
            go.SetActive(false);
        }
    }

    private void Update()
    {
        if (_doorActivationSystem.isActive && !startedTrans)
        {
            ActivateExplosion();
            StartCoroutine("EditVignette");
            startedTrans = true;
        }
    }

    private void ActivateExplosion()
    {
        GameObject exp = Instantiate(m_explosionPrefab, XROrigin.transform.position, Quaternion.identity);
    }

    private IEnumerator EditVignette()
    {
        // Mientras apertureSize y featheringEffect no lleguen a 0, reducirlos gradualmente
        while (vignette.apertureSize > 0 && vignette.featheringEffect > 0)
        {
            if (vignette.apertureSize > 0)
            {
                vignette.apertureSize = Mathf.Max(vignette.apertureSize - 0.05f, 0);  // Reducir hasta un mínimo de 0
            }

            if (vignette.featheringEffect > 0)
            {
                vignette.featheringEffect = Mathf.Max(vignette.featheringEffect - 0.01f, 0);  // Reducir hasta un mínimo de 0
            }

            yield return new WaitForSeconds(0.05f);  // Esperar un pequeño intervalo antes de la siguiente reducción
        }
    }
}

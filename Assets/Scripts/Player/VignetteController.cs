using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VignetteController : MonoBehaviour
{
    [SerializeField] private MeshRenderer vignette;
    [Range(0f, 1f)]
    [SerializeField] public float apertureSize;
    [Range(0f, 1f)]
    [SerializeField] public float featheringEffect;
    [SerializeField] private Color vignetteColor = Color.red;
    [SerializeField] private Color vignetteColorBlend = Color.black;

    private MaterialPropertyBlock propertyBlock;
    private Shader vignetteShader;
    public bool active = false;
    private bool alreadyactive = false;

    private void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        propertyBlock.SetFloat("_ApertureSize", apertureSize);
        propertyBlock.SetFloat("_FeatheringEffect", featheringEffect);
        propertyBlock.SetColor("_VignetteColor", vignetteColor);
        propertyBlock.SetColor("_VignetteColorBlend", vignetteColorBlend);

        vignette.SetPropertyBlock(propertyBlock);
    }
}

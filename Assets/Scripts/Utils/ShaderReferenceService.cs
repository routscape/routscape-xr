using System;
using UnityEngine;

public class ShaderReferenceService : MonoBehaviour
{
    [SerializeField] private Shader clippingColorShader;
    [SerializeField] private Shader clippingTextureShader;

    public static Shader DefaultLitShader;
    public static Shader ClippingColorShader;
    public static Shader ClippingTextureShader;

    void Awake()
    {
        DefaultLitShader = Shader.Find("Universal Render Pipeline/Lit");
        ClippingColorShader = clippingColorShader;
        ClippingTextureShader = clippingTextureShader;
    }
}

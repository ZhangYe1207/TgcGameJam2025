using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class XPostManualEffect : MonoBehaviour
{
    public Material postEffectMat;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (postEffectMat != null)
        {
            Graphics.Blit(src, dest, postEffectMat);
        }
        else
        {
            Graphics.Blit(src, dest); // fallback
        }
    }
}


using UnityEngine;

[ExecuteAlways]

public class DebugOverlayBlit : MonoBehaviour
{
    [SerializeField] public Material overlayMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (overlayMaterial != null)
        {
            Graphics.Blit(src, dest, overlayMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}

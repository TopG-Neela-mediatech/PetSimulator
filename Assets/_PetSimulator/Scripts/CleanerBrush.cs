using UnityEngine;

public class CleanerBrush : MonoBehaviour
{
    public RenderTexture maskTexture;
    public Material drawMaterial; // Shader that draws a circle
    public float brushSize = 0.05f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 uv = hit.textureCoord;
                DrawAtUV(uv);
            }
        }
    }

    void DrawAtUV(Vector2 uv)
    {
        drawMaterial.SetVector("_UV", new Vector4(uv.x, uv.y, brushSize, 0));
        RenderTexture temp = RenderTexture.GetTemporary(maskTexture.width, maskTexture.height);
        Graphics.Blit(maskTexture, temp);
        Graphics.Blit(temp, maskTexture, drawMaterial);
        RenderTexture.ReleaseTemporary(temp);
    }
}

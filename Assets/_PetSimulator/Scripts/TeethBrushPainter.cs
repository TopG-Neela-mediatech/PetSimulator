using UnityEngine;

public class TeethBrushPainter : MonoBehaviour
{
    public Camera mainCam;

    [Header("Brush")]
    public RenderTexture brushMask;
    public Material brushMat;
    public float brushSize = 0.05f; // Important: not too small!
    [SerializeField] private float cleanSpeed;

    public ParticleSystem ps;

    [Header("Foam")]
    [SerializeField] private RenderTexture foamMask;
    [SerializeField] private Material foamMaterial;
    [SerializeField] private float foamStrength = 0.08f;

    private void Start()
    {
        // Clear the mask to full black
        ResetRT();

    }

    public void ResetRT()
    {
        RenderTexture.active = brushMask;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = foamMask;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
    }


    private void Update()
    {
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            Vector2 screenPos = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;

            Ray ray = mainCam.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                ps.transform.position = worldPos;
                if (!ps.isPlaying)
                {
                    ps.Play();
                }
                Vector2 uv = hit.textureCoord;
                PaintAtUV(uv);
                Debug.Log($"Painting at UV: {uv}");
            }
            else
            {
                Debug.Log("No collider hit!");
            }
        }
    }

    void PaintAtUV(Vector2 uv)
    {
        // Set a low alpha so it builds up gradually
        // try 0.05 to 0.2 for tuning
        brushMat.SetColor("_BaseColor", new Color(1f, 1f, 1f, cleanSpeed));
        Graphics.SetRenderTarget(brushMask);
        GL.PushMatrix();
        GL.LoadOrtho();


        brushMat.SetPass(0);

        GL.Begin(GL.QUADS);

        float halfSize = brushSize * 0.5f;

        GL.TexCoord2(0, 0); GL.Vertex3(uv.x - halfSize, uv.y - halfSize, 0);
        GL.TexCoord2(1, 0); GL.Vertex3(uv.x + halfSize, uv.y - halfSize, 0);
        GL.TexCoord2(1, 1); GL.Vertex3(uv.x + halfSize, uv.y + halfSize, 0);
        GL.TexCoord2(0, 1); GL.Vertex3(uv.x - halfSize, uv.y + halfSize, 0);

        GL.End();
        GL.PopMatrix();
        Graphics.SetRenderTarget(null);

        Debug.Log($"[Painted] UV: {uv} Size: {brushSize}");

        PaintToMask(foamMask, foamMaterial, uv, foamStrength);
    }

    void PaintToMask(RenderTexture target, Material mat, Vector2 uv, float strength)
    {
        mat.SetColor("_BaseColor", new Color(1f, 1f, 1f, strength));

        Graphics.SetRenderTarget(target);
        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);
        GL.Begin(GL.QUADS);

        float halfSize = brushSize * 0.5f;

        GL.TexCoord2(0, 0); GL.Vertex3(uv.x - halfSize, uv.y - halfSize, 0);
        GL.TexCoord2(1, 0); GL.Vertex3(uv.x + halfSize, uv.y - halfSize, 0);
        GL.TexCoord2(1, 1); GL.Vertex3(uv.x + halfSize, uv.y + halfSize, 0);
        GL.TexCoord2(0, 1); GL.Vertex3(uv.x - halfSize, uv.y + halfSize, 0);

        GL.End();
        GL.PopMatrix();
        Graphics.SetRenderTarget(null);
    }
}

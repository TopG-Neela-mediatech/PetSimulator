using UnityEngine;

namespace TMKOC.PetSimulator
{
    public class TeethBrushPainter : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        [SerializeField] private GameObject targetTeethMesh;

        [Header("Brush")]
        public RenderTexture brushMask;
        public Material brushMat;
        public float brushSize = 0.05f; // Important: not too small!
        [SerializeField] private float cleanSpeed;

        [SerializeField] private ParticleSystem ps;

        [Header("Foam")]
        [SerializeField] private RenderTexture foamMask;
        [SerializeField] private Material foamMaterial;
        [SerializeField] private float foamStrength = 0.08f;

        [Header("Debug")]
        [Range(0, 1)] public float brushProgress = 0f; // <- NEW: Clean progress shown in Inspector

        private Texture2D tempTex; // <- NEW: used to read brushMask for progress
        private float lastProgressUpdateTime = 0f;
        private const float progressUpdateInterval = 0.25f; // Update every 0.25 seconds

        private void Start()
        {
            // Clear the mask to full black
            ResetRT();
            Debug.Log("Camera: " + mainCam.name);

            // Create tempTex for progress reading
            tempTex = new Texture2D(brushMask.width, brushMask.height, TextureFormat.RGBA32, false, true);
        }

        public void ResetRT()
        {
            RenderTexture.active = brushMask;
            GL.Clear(true, true, Color.black);
            //RenderTexture.active = foamMask;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;

            brushProgress = 0f; // Reset progress too
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) || Input.touchCount > 0)
            {
                Vector2 screenPos = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;

                Ray ray = mainCam.ScreenPointToRay(screenPos);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == targetTeethMesh)
                    {
                        var worldPos = mainCam.ScreenToWorldPoint(screenPos);

                        if (ps != null)
                        {
                            ps.transform.position = worldPos;
                            if (!ps.isPlaying)
                            {
                                ps.Play();
                            }
                        }
                        Vector2 uv = hit.textureCoord;
                        PaintAtUV(uv);
                    }
                }
                else
                {
                    Debug.Log("No collider m_hit!");
                }
            }

            // NEW: Periodically update brushProgress
            if (Time.time - lastProgressUpdateTime > progressUpdateInterval)
            {
                lastProgressUpdateTime = Time.time;
                brushProgress = CalculateBrushProgress();
            }
        }

        void PaintAtUV(Vector2 uv)
        {
            if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1) return;

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

        // NEW: Calculates the % of cleaned area (alpha channel avg)
        float CalculateBrushProgress()
        {
            if (brushMask == null) return 0f;

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = brushMask;

            tempTex.ReadPixels(new Rect(0, 0, brushMask.width, brushMask.height), 0, 0);
            tempTex.Apply();

            RenderTexture.active = currentRT;

            Color[] pixels = tempTex.GetPixels();
            float totalAlpha = 0f;

            for (int i = 0; i < pixels.Length; i++)
            {
                totalAlpha += pixels[i].a;
            }

            float progress = Mathf.Clamp01(totalAlpha / pixels.Length); // normalize to 0–1
            return progress;
        }
    }
}
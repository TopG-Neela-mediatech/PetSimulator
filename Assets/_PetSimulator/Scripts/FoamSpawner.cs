using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.PetSimulator
{
    public enum SurfaceType
    {
        None = 0,
        Teeth,
        Body,
    }

    public class FoamSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class FoamBubble
        {
            public Vector3 position;
            public Vector3 normal;
            public Color color;
            public float scale;
            public float rotation;  // Rotation in degrees
            public float age;
            public float lifetime;
        }

        [Header("Foam Settings")]
        public Material bubbleMaterial;
        public int maxBubbles = 200;
        public float minScale = 0.05f;
        public float maxScale = 0.1f;
        public float minDistanceBetweenBubbles = 0.05f;
        public float minLifetime = 1.5f;
        public float maxLifetime = 3.5f;

        [Header("Optional FX")]
        public ParticleSystem popParticleFX; // Can remain null

        private List<FoamBubble> bubbles = new List<FoamBubble>();
        private Mesh quadMesh;
        private Matrix4x4[] matrices;
        private MaterialPropertyBlock mpb;
        private const int batchSize = 1023;

        private Vector4[] baseColors;
        private float[] scaleValues;

        private RaycastHit m_hit;
        private Vector3 lastPlacedPosition;
        private bool hasPlacedFirst = false;

        void Start()
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // position sphere a little below current level.
            sphere.transform.Translate(new Vector2(0f, -2f));

            quadMesh = sphere.GetComponent<MeshFilter>().sharedMesh;
            matrices = new Matrix4x4[batchSize];
            baseColors = new Vector4[batchSize];
            scaleValues = new float[batchSize];
            mpb = new MaterialPropertyBlock();
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out m_hit))
                {
                    if (m_hit.collider.TryGetComponent(out FoamSurfaceType foamSurfaceType) &&
                        foamSurfaceType.SurfaceType == SurfaceType.Teeth)
                    {
                        Vector3 spawnPos = m_hit.point;

                        if (!hasPlacedFirst || Vector3.Distance(spawnPos, lastPlacedPosition) >= minDistanceBetweenBubbles)
                        {
                            AddBubble(spawnPos, m_hit.normal);
                            lastPlacedPosition = spawnPos;
                            hasPlacedFirst = true;
                        }
                    }
                }
            }
            else
            {
                hasPlacedFirst = false;
            }

            UpdateBubbles();
            DrawBubbles();
        }

        void AddBubble(Vector3 position, Vector3 normal)
        {
            if (GetActiveBubbleCount() >= maxBubbles) return;

            Vector3 offset = normal * Random.Range(0.001f, 0.005f);

            FoamBubble bubble = new FoamBubble
            {
                position = position + offset,
                normal = normal,
                color = GetSlightlyVariedColor(),
                scale = Random.Range(minScale, maxScale),
                rotation = Random.Range(0f, 360f),  // Random rotation
                age = 0f,
                lifetime = Random.Range(minLifetime, maxLifetime)
            };

            Vector3 randomMovement = new Vector3(Random.Range(-0.005f, 0.005f), Random.Range(0f, 0.005f), Random.Range(-0.005f, 0.005f));
            bubble.position += randomMovement;

            bubbles.Add(bubble);
        }

        void UpdateBubbles()
        {
            for (int i = bubbles.Count - 1; i >= 0; i--)
            {
                var b = bubbles[i];
                b.age += Time.deltaTime;

                float t = Mathf.Clamp01(1f - b.age / b.lifetime);
                b.scale = Mathf.Lerp(0f, b.scale, t);
                b.color.a = t;

                if (b.age >= b.lifetime || b.scale <= 0.001f)
                {
                    if (popParticleFX != null)
                    {
                        popParticleFX.transform.position = b.position;
                        popParticleFX.Play();
                    }

                    bubbles.RemoveAt(i);
                }
                else
                {
                    bubbles[i] = b;
                }
            }
        }

        void DrawBubbles()
        {
            int total = bubbles.Count;
            if (total == 0) return;

            for (int i = 0; i < total; i += batchSize)
            {
                int count = Mathf.Min(batchSize, total - i);

                for (int j = 0; j < count; j++)
                {
                    var b = bubbles[i + j];
                    baseColors[j] = b.color;
                    scaleValues[j] = b.scale;

                    // Apply rotation using the bubble's rotation field
                    Quaternion rotationQuat = Quaternion.Euler(0f, b.rotation, 0f); // Adjusting rotation on the Y-axis
                    matrices[j] = Matrix4x4.TRS(b.position, rotationQuat, Vector3.one * b.scale);
                }

                mpb.SetVectorArray("_BaseColor", baseColors);
                mpb.SetFloatArray("_Scale", scaleValues);

                Graphics.DrawMeshInstanced(quadMesh, 0, bubbleMaterial, matrices, count, mpb);
            }
        }

        Color GetSlightlyVariedColor()
        {
            Color baseCol = Color.white;
            float variation = Random.Range(0.85f, 1.05f);
            return baseCol * variation;
        }

        int GetActiveBubbleCount()
        {
            int count = 0;
            foreach (var b in bubbles)
            {
                if (b.age < b.lifetime) count++;
            }
            return count;
        }
    }
}

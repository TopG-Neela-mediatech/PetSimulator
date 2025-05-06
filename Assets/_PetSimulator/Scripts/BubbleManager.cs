using UnityEngine;
using System.Collections.Generic;

public class BubbleManager : MonoBehaviour
{
    [Header("Bubble Settings")]
    public Mesh bubbleMesh;
    public Material bubbleMaterial;
    public int instanceCount = 200;
    public float spawnArea = .1f;
    public Vector2 bubbleRadiusRange = new Vector2(0.05f, 0.15f);

    [Header("Water Particles")]
    public ParticleSystem waterParticles;

    private List<BubbleInstance> bubbles = new List<BubbleInstance>();
    private Matrix4x4[] matrices;
    private ParticleSystem.Particle[] waterParticleArray = new ParticleSystem.Particle[1000];

    void Start()
    {
        matrices = new Matrix4x4[instanceCount];
        GenerateBubbles();
    }

    void Update()
    {
        CheckWaterParticleHits();
        UpdateBubbles();
        RenderBubbles();
    }

    void GenerateBubbles()
    {
        bubbles.Clear();

        int bubblesPerAxis = Mathf.CeilToInt(Mathf.Pow(instanceCount, 1f / 3f));
        float minRadius = bubbleRadiusRange.x;
        float spacing = minRadius * 2f * 0.95f; // slight overlap allowed for density

        Vector3 start = -Vector3.one * (bubblesPerAxis * spacing * 0.5f);

        int count = 0;
        for (int x = 0; x < bubblesPerAxis && count < instanceCount; x++)
        {
            for (int y = 0; y < bubblesPerAxis && count < instanceCount; y++)
            {
                for (int z = 0; z < bubblesPerAxis && count < instanceCount; z++)
                {
                    Vector3 jitter = Random.insideUnitSphere * spacing * 0.1f;
                    Vector3 pos = start + new Vector3(x, y, z) * spacing + jitter;
                    float radius = Random.Range(bubbleRadiusRange.x, bubbleRadiusRange.y);

                    bubbles.Add(new BubbleInstance
                    {
                        position = pos,
                        radius = radius,
                        isDestroyed = false
                    });

                    count++;
                }
            }
        }
    }


    void CheckWaterParticleHits()
    {
        if (waterParticles == null) return;

        int count = waterParticles.GetParticles(waterParticleArray);

        for (int i = 0; i < count; i++)
        {
            Vector3 particlePos = waterParticleArray[i].position;

            for (int j = 0; j < bubbles.Count; j++)
            {
                if (bubbles[j].isDestroyed) continue;

                float dist = Vector3.Distance(particlePos, bubbles[j].position);
                if (dist < bubbles[j].radius * 5f)
                {
                    var b = bubbles[j];
                    b.isDestroyed = true;
                    bubbles[j] = b;
                }
            }
        }
    }

    void UpdateBubbles()
    {
        for (int i = 0; i < bubbles.Count; i++)
        {
            if (bubbles[i].isDestroyed)
            {
                var bubble = bubbles[i];
                bubble.radius = Mathf.Lerp(bubble.radius, 0, Time.deltaTime * 5f);
                if (bubble.radius < 0.01f)
                {
                    // Optionally, remove or respawn
                    bubble.radius = 0;
                }
                bubbles[i] = bubble;
            }
        }
    }

    void RenderBubbles()
    {
        int index = 0;
        for (int i = 0; i < bubbles.Count && index < matrices.Length; i++)
        {
            if (bubbles[i].radius <= 0f) continue;

            matrices[index] = Matrix4x4.TRS(
                bubbles[i].position,
                Quaternion.identity,
                Vector3.one * bubbles[i].radius * 5f
            );
            index++;
        }

        Graphics.DrawMeshInstanced(bubbleMesh, 0, bubbleMaterial, matrices, index);
    }

    public struct BubbleInstance
    {
        public Vector3 position;
        public float radius;
        public bool isDestroyed;
    }
}

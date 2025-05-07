using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBubbles : MonoBehaviour
{
    [Serializable]
    struct Bubble
    {
        public Vector3 localPosition;
        public float scale;
        public bool isPopped;
    }

    [Header("Bubble Settings")]
    public Mesh bubbleMesh;
    public Material bubbleMaterial;
    public Transform parentTransform;

    [Header("Water Particle Settings")]
    public ParticleSystem waterParticles;
    private ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[1000];

    private List<Bubble> bubbles = new List<Bubble>();
    private List<Matrix4x4> bubbleMatrices = new List<Matrix4x4>();
    private const int batchSize = 1023;

    void Update()
    {
        // Spawn bubbles on left click
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Hit: " + hit.collider.name);

                if (hit.collider.name == "Teeth.001")
                {
                    float size = UnityEngine.Random.Range(0.05f, 0.15f);

                    //// Add jitter and some depth based on surface normal
                    //Vector3 offset = hit.normal * UnityEngine.Random.Range(-0.03f, 0.05f);  // depth
                    //Vector3 jitter = UnityEngine.Random.insideUnitSphere * 0.01f;          // fuzziness

                    Vector3 finalPos = hit.point ;
                    Vector3 localPos = parentTransform.InverseTransformPoint(finalPos);

                    bubbles.Add(new Bubble
                    {
                        localPosition = localPos,
                        scale = size * .25f,
                        isPopped = false
                    });
                }
            }
        }

        // Destroy bubble near click on right click
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float sphereRadius = 0.2f;
            if (Physics.SphereCast(ray, sphereRadius, out RaycastHit hit, 100000f))
            {
                TryDestroyInstanceNear(hit.point, 0.3f);
            }
        }

        // Check collisions with water particles
        CheckWaterParticleHits();
    }

    void TryDestroyInstanceNear(Vector3 center, float destroyRadius)
    {
        for (int i = 0; i < bubbles.Count; i++)
        {
            if (bubbles[i].isPopped) continue;

            Vector3 worldPos = parentTransform.TransformPoint(bubbles[i].localPosition);
            if (Vector3.Distance(worldPos, center) < destroyRadius)
            {
                Bubble b = bubbles[i];
                b.scale = 0f;
                b.isPopped = true;
                bubbles[i] = b;
                break;
            }
        }
    }

    void CheckWaterParticleHits()
    {
        if (waterParticles == null) return;

        int count = waterParticles.GetParticles(particleArray);
        var main = waterParticles.main;

        for (int p = 0; p < count; p++)
        {
            Vector3 particlePos = main.simulationSpace == ParticleSystemSimulationSpace.World
                ? particleArray[p].position
                : waterParticles.transform.TransformPoint(particleArray[p].position);

            for (int i = 0; i < bubbles.Count; i++)
            {
                if (bubbles[i].isPopped) continue;

                Vector3 bubbleWorldPos = parentTransform.TransformPoint(bubbles[i].localPosition);
                float radius = bubbles[i].scale * 5f;

                if (Vector3.Distance(particlePos, bubbleWorldPos) < radius)
                {
                    Bubble b = bubbles[i];
                    b.scale = 0f;
                    b.isPopped = true;
                    bubbles[i] = b;
                    break;
                }
            }
        }
    }


    void LateUpdate()
    {
        bubbleMatrices.Clear();

        foreach (var bubble in bubbles)
        {
            if (bubble.isPopped || bubble.scale <= 0f)
                continue;

            Vector3 worldPos = parentTransform.TransformPoint(bubble.localPosition);
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one * bubble.scale;

            bubbleMatrices.Add(Matrix4x4.TRS(worldPos, rotation, scale));
        }

        int count = bubbleMatrices.Count;
        for (int i = 0; i < count; i += batchSize)
        {
            int len = Mathf.Min(batchSize, count - i);
            Graphics.DrawMeshInstanced(bubbleMesh, 0, bubbleMaterial, bubbleMatrices.GetRange(i, len));
        }
    }
}

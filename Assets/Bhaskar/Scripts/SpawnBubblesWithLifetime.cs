using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBubblesWithLifetime : MonoBehaviour
{
    [Serializable]
    struct Bubble
    {
        public Vector3 localPosition;
        public float scale;
        public float age;
        public float maxLifetime;
        public bool isPopped;
    }

    [Header("Bubble Settings")]
    public Mesh bubbleMesh;
    public Material bubbleMaterial;
    public Transform parentTransform;
    public int extraBubbleCount = 5;
    public float offsetRadius = 0.001f;

    public float minLifetime = 1.8f;
    public float maxLifetime = 2.8f;

    public float _bubbleScaleFactor = 0.35f;

    private List<Bubble> bubbles = new List<Bubble>();
    private List<Matrix4x4> bubbleMatrices = new List<Matrix4x4>();
    private const int batchSize = 1023;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.name == "Teeth.001")
                {
                    Vector3 hitPoint = hit.point;

                    // Main bubble
                    AddBubbleAtWorldPos(hitPoint, UnityEngine.Random.Range(0.05f, 0.15f), UnityEngine.Random.Range(minLifetime, maxLifetime));

                    // Extra mini-bubbles with random offsets
                    for (int i = 0; i < extraBubbleCount; i++)
                    {
                        Vector3 offset = Vector3.ProjectOnPlane(UnityEngine.Random.insideUnitSphere, hit.normal) * offsetRadius;
                        Vector3 miniBubblePos = hitPoint + offset;
                        if (Physics.Raycast(miniBubblePos + hit.normal * 0.02f, -hit.normal, out RaycastHit newHit))
                            miniBubblePos = newHit.point;
                        float miniScale = UnityEngine.Random.Range(0.025f, 0.075f);
                        float lifetime = UnityEngine.Random.Range(minLifetime, maxLifetime);
                        AddBubbleAtWorldPos(miniBubblePos, miniScale, lifetime);
                    }
                }
            }
        }

        // Update lifetime
        for (int i = 0; i < bubbles.Count; i++)
        {
            if (bubbles[i].isPopped) continue;

            Bubble b = bubbles[i];
            b.age += Time.deltaTime;
            if (b.age >= b.maxLifetime)
            {
                b.scale = 0f;
                b.isPopped = true;
            }
            bubbles[i] = b;
        }
    }

    void AddBubbleAtWorldPos(Vector3 worldPos, float scale, float lifetime)
    {
        Vector3 localPos = parentTransform.InverseTransformPoint(worldPos);
        bubbles.Add(new Bubble
        {
            localPosition = localPos,
            scale = scale * _bubbleScaleFactor,
            age = 0f,
            maxLifetime = lifetime,
            isPopped = false
        });
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

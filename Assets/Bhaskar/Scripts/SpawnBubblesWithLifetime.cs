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
        public Color color;
        public Color baseColor;
        public float colorShiftSpeed; // unique flicker rate

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

    [Header("Touch Control")]
    [Tooltip("Minimum screen distance in pixels to spawn new bubbles")]
    public float minTouchDeltaDistance = 100f;

    private List<Bubble> bubbles = new List<Bubble>();
    private List<Matrix4x4> bubbleMatrices = new List<Matrix4x4>();
    private const int batchSize = 1023;

    private Vector2 lastTouchPosition = Vector2.negativeInfinity;


    private Color baseColor;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 currentTouchPos = Input.mousePosition;

            if (lastTouchPosition == Vector2.negativeInfinity ||
                Vector2.Distance(currentTouchPos, lastTouchPosition) > minTouchDeltaDistance)
            {
                Ray ray = Camera.main.ScreenPointToRay(currentTouchPos);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.name == "Teeth.001")
                    {
                        Vector3 hitPoint = hit.point;

                        baseColor = bubbleMaterial.GetColor("_BaseColor");

                        // Main bubble
                        AddBubbleAtWorldPos(hitPoint, UnityEngine.Random.Range(0.05f, 0.15f), UnityEngine.Random.Range(minLifetime, maxLifetime));

                        // Extra mini-bubbles
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

                        lastTouchPosition = currentTouchPos;
                    }
                }
            }
        }
        else
        {
            lastTouchPosition = Vector2.negativeInfinity; // reset when touch ends
        }

        // Update lifetimes
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

    Color GetBrightnessVariedColor()
    {
        float factor = UnityEngine.Random.Range(0.85f, 1.1f);
        return new Color(baseColor.r * factor, baseColor.g * factor, baseColor.b * factor, baseColor.a);
    }

    private Color GetSlightlyVariedColor()
    {
        float hue, sat, val;
        Color.RGBToHSV(baseColor, out hue, out sat, out val);

        // Small variation range
        hue += UnityEngine.Random.Range(-0.015f, 0.015f);
        sat += UnityEngine.Random.Range(-0.05f, 0.05f);
        val += UnityEngine.Random.Range(-0.1f, 0.05f);

        hue = Mathf.Repeat(hue, 1f); // wrap hue
        sat = Mathf.Clamp01(sat);
        val = Mathf.Clamp01(val);

        return Color.HSVToRGB(hue, sat, val);
    }

    void AddBubbleAtWorldPos(Vector3 worldPos, float scale, float lifetime)
    {
        Vector3 localPos = parentTransform.InverseTransformPoint(worldPos);

        Color baseColor = GetBrightnessVariedColor(); // or slight HSV variation

        bubbles.Add(new Bubble
        {
            localPosition = localPos,
            scale = scale * _bubbleScaleFactor,
            age = 0f,
            maxLifetime = lifetime,
            isPopped = false,
            baseColor = baseColor,
            colorShiftSpeed = UnityEngine.Random.Range(0.5f, 1.5f)
        });
    }

    void LateUpdate()
    {
        bubbleMatrices.Clear();
        List<Vector4> colors = new List<Vector4>();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        foreach (var bubble in bubbles)
        {
            if (bubble.isPopped || bubble.scale <= 0f) continue;

            Vector3 worldPos = parentTransform.TransformPoint(bubble.localPosition);
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one * bubble.scale;
            bubbleMatrices.Add(Matrix4x4.TRS(worldPos, rotation, scale));

            // Pulse brightness
            Color finalColor = bubble.baseColor;

            float hue, sat, val;
            Color.RGBToHSV(finalColor, out hue, out sat, out val);

            float pulse = Mathf.Sin(Time.time * bubble.colorShiftSpeed * 2f) * 0.1f; // ±10% brightness
            val = Mathf.Clamp01(val + pulse);

            finalColor = Color.HSVToRGB(hue, sat, val);
            finalColor.a = baseColor.a;

            colors.Add(finalColor);
        }

        int count = bubbleMatrices.Count;
        for (int i = 0; i < count; i += batchSize)
        {
            int len = Mathf.Min(batchSize, count - i);
            mpb.Clear();
            mpb.SetVectorArray("_BaseColor", colors.GetRange(i, len));
            Graphics.DrawMeshInstanced(bubbleMesh, 0, bubbleMaterial, bubbleMatrices.GetRange(i, len), mpb);
        }
    }

}

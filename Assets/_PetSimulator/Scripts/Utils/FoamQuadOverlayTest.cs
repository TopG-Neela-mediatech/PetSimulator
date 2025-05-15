using UnityEngine;

[ExecuteAlways]
public class FoamQuadOverlayTest : MonoBehaviour
{
    [SerializeField] private Material foamMaterial; // Now shows up in Inspector

    private GameObject quadInstance;

    void OnEnable()
    {
        CreateFullScreenQuad();
    }

    void OnDisable()
    {
        if (quadInstance != null)
            DestroyImmediate(quadInstance);
    }

    void CreateFullScreenQuad()
    {
        if (foamMaterial == null)
        {
            Debug.LogWarning("FoamQuadOverlayTest: No material assigned.");
            return;
        }

        if (quadInstance != null)
            DestroyImmediate(quadInstance);

        quadInstance = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quadInstance.name = "Foam_Debug_Quad";
        DestroyImmediate(quadInstance.GetComponent<Collider>());
        quadInstance.transform.SetParent(this.transform);

        Camera cam = Camera.main;
        if (cam != null)
        {
            quadInstance.transform.position = cam.transform.position + cam.transform.forward * 0.5f;
            quadInstance.transform.rotation = cam.transform.rotation;

            float h = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * 2f * 0.5f;
            float w = h * cam.aspect;
            quadInstance.transform.localScale = new Vector3(w, h, 1);
        }

        quadInstance.GetComponent<MeshRenderer>().sharedMaterial = foamMaterial;
    }
}
